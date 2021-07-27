using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

namespace TNL.Entities
{
    using Network;
    using Structures;
    using Types;
    using Utils;

    public enum PacketType : byte
    {
        ConnectChallengeRequest = 0,
        ConnectChallengeResponse = 1,
        ConnectRequest = 2,
        ConnectReject = 3,
        ConnectAccept = 4,
        Disconnect = 5,
        Punch = 6,
        ArrangedConnectRequest = 7,
        FirstValidInfoPacketId = 8
    }

    public enum TerminationReason
    {
        ReasonTimedOut,
        ReasonFailedConnectHandshake,
        ReasonRemoteHostRejectedConnection,
        ReasonRemoteDisconnectPacket,
        ReasonDuplicateConnectionAttempt,
        ReasonSelfDisconnect,
        ReasonError,
    };

    public class NetInterface
    {
        public const uint ChallengeRetryCount = 4;
        public const uint ChallengeRetryTime = 2500;

        public const uint ConnectRetryCount = 4;
        public const uint ConnectRetryTime = 2500;

        public const uint PunchRetryCount = 6;
        public const uint PunchRetryTime = 2500;

        public const uint TimeoutCheckInterval = 1500;
        public const uint PuzzleSolutionTimeout = 30000;

        private readonly List<NetConnection> _connectionList = new();

        protected readonly Dictionary<IPEndPoint, NetConnection> Connections = new();
        protected readonly Dictionary<IPEndPoint, NetConnection> PendingConnections = new();
        protected readonly List<DelaySendPacket> SendPacketList = new();

        protected AsymmetricKey PrivateKey { get; set; }
        protected Certificate Certificate { get; set; }
        protected ClientPuzzleManager PuzzleManager { get; private set; }

        public TNLSocket Socket { get; set; }

        protected int CurrentTime { get; set; }
        protected bool RequiresKeyExchange { get; set; }
        protected int LastTimeoutCheckTime { get; set; }
        protected byte[] RandomHashData { get; set; }
        protected bool AllowConnections { get; set; }

        static NetInterface()
        {
            GhostConnection.RegisterNetClassReps();
        }

        public NetInterface(int port)
        {
            Socket = new TNLSocket(port);

            NetClassRep.Initialize();

            LastTimeoutCheckTime = 0;
            AllowConnections = true;
            RequiresKeyExchange = false;

            RandomHashData = new byte[12];
            RandomUtil.Read(RandomHashData, 12);

            CurrentTime = Environment.TickCount;
            PuzzleManager = new ClientPuzzleManager();
        }

        ~NetInterface()
        {
            Close();
        }

        public void Close()
        {
            lock (_connectionList)
                while (_connectionList.Count > 0)
                    Disconnect(_connectionList[0], TerminationReason.ReasonSelfDisconnect, "Shutdown");
        }

        public void SetPrivateKey(AsymmetricKey theKey)
        {
            PrivateKey = theKey;
        }

        public void SetRequiresKeyExchange(bool requires)
        {
            RequiresKeyExchange = requires;
        }

        public void SetCertificate(Certificate theCertificate)
        {
            Certificate = theCertificate;
        }

        public bool DoesAllowConnections()
        {
            return AllowConnections;
        }

        public void SetAllowConnections(bool allow)
        {
            AllowConnections = allow;
        }

        public TNLSocket GetSocket()
        {
            return Socket;
        }

        public NetError SendTo(IPEndPoint address, BitStream stream)
        {
            return Socket.Send(address, stream.GetBuffer(), stream.GetBytePosition());
        }

        public void SendToDelayed(IPEndPoint address, BitStream stream, int millisecondDelay)
        {
            var dataSize = stream.GetBytePosition();

            var dsp = new DelaySendPacket
            {
                RemoteAddress = address,
                SendTime = CurrentTime + millisecondDelay,
                PacketData = new byte[dataSize],
                PacketSize = dataSize
            };

            Array.Copy(stream.GetBuffer(), dsp.PacketData, dataSize);

            SendPacketList.Add(dsp);
        }

        protected uint ComputeClientIdentityToken(IPEndPoint address, Nonce theNonce)
        {
            var buff = new byte[40];

            Array.Copy(BitConverter.GetBytes(address.Port), 0, buff, 0, 4); // Port instead of transport + port
            Array.Copy(address.Address.GetAddressBytes(), 0, buff, 4, 4);
            Array.Copy(theNonce.Data, 0, buff, 20, 8);
            Array.Copy(RandomHashData, 0, buff, 28, 12);

            return BitConverter.ToUInt32(new SHA256Managed().ComputeHash(buff), 0);
        }

        protected NetConnection FindPendingConnection(IPEndPoint address)
        {
            return PendingConnections.ContainsKey(address) ? PendingConnections[address] : null;
        }

        protected void AddPendingConnection(NetConnection connection)
        {
            FindAndRemovePendingConnection(connection.GetNetAddress());

            var nc = FindConnection(connection.GetNetAddress());
            if (nc != null)
                Disconnect(nc, TerminationReason.ReasonSelfDisconnect, "Reconnecting");

            PendingConnections.Add(connection.GetNetAddress(), connection);
        }

        protected void RemovePendingConnection(NetConnection connection)
        {
            FindAndRemovePendingConnection(connection.GetNetAddress());
        }

        protected void FindAndRemovePendingConnection(IPEndPoint address)
        {
            PendingConnections.Remove(address);
        }

        public NetConnection FindConnection(IPEndPoint client)
        {
            return Connections.ContainsKey(client) ? Connections[client] : null;
        }

        protected virtual void RemoveConnection(NetConnection conn)
        {
            Connections.Remove(conn.GetNetAddress());

            lock (_connectionList)
                _connectionList.Remove(conn);
        }

        public virtual void AddConnection(NetConnection conn)
        {
            Connections.Add(conn.GetNetAddress(), conn);

            lock (_connectionList)
                _connectionList.Add(conn);
        }

        public void ProcessConnections()
        {
            CurrentTime = Environment.TickCount;
            PuzzleManager.Tick(CurrentTime);

            if (SendPacketList.Count > 0)
            {
                foreach (var dsp in SendPacketList.Where(dsp => dsp.SendTime > CurrentTime))
                {
                    Socket.Send(dsp.RemoteAddress, dsp.PacketData, dsp.PacketSize);

                    SendPacketList.Remove(dsp);
                }
            }

            NetObject.CollapseDirtyList();

            lock (_connectionList)
                foreach (var conn in _connectionList)
                    conn.CheckPacketSend(false, CurrentTime);

            if (CurrentTime > LastTimeoutCheckTime + TimeoutCheckInterval)
            {
                var removeList = new List<NetConnection>();

                foreach (var pair in PendingConnections)
                {
                    var pending = pair.Value;

                    if (pending.ConnectionState == NetConnectionState.AwaitingChallengeResponse && CurrentTime > pending.ConnectLastSendTime + ChallengeRetryTime)
                    {
                        if (pending.ConnectSendCount > ChallengeRetryCount)
                        {
                            pending.ConnectionState = NetConnectionState.ConnectTimedOut;
                            pending.OnConnectTerminated(TerminationReason.ReasonTimedOut, "Timeout");

                            removeList.Add(pending);

                            continue;
                        }

                        SendConnectChallengeRequest(pending);
                    }
                    else if (pending.ConnectionState == NetConnectionState.AwaitingConnectResponse && CurrentTime > pending.ConnectLastSendTime + ConnectRetryTime)
                    {
                        if (pending.ConnectSendCount > ConnectRetryCount)
                        {
                            pending.ConnectionState = NetConnectionState.ConnectTimedOut;
                            pending.OnConnectTerminated(TerminationReason.ReasonTimedOut, "Timeout");

                            removeList.Add(pending);

                            continue;
                        }

                        if (pending.GetConnectionParameters().IsArranged)
                            SendArrangedConnectRequest(pending);
                        else
                            SendConnectRequest(pending);
                    }
                    else if (pending.ConnectionState == NetConnectionState.SendingPunchPackets && CurrentTime > pending.ConnectLastSendTime + PunchRetryTime)
                    {
                        if (pending.ConnectSendCount > PunchRetryCount)
                        {
                            pending.ConnectionState = NetConnectionState.ConnectTimedOut;
                            pending.OnConnectTerminated(TerminationReason.ReasonTimedOut, "Timeout");

                            removeList.Add(pending);

                            continue;
                        }

                        SendPunchPackets(pending);
                    }
                    else if (pending.ConnectionState == NetConnectionState.ComputingPuzzleSolution && CurrentTime > pending.ConnectLastSendTime + PuzzleSolutionTimeout)
                    {
                        pending.ConnectionState = NetConnectionState.ConnectTimedOut;
                        pending.OnConnectTerminated(TerminationReason.ReasonTimedOut, "Timeout");

                        removeList.Add(pending);
                    }
                }

                foreach (var conn in removeList)
                    RemovePendingConnection(conn);

                removeList.Clear();

                LastTimeoutCheckTime = CurrentTime;

                lock (_connectionList)
                {
                    foreach (var conn in _connectionList)
                    {
                        if (conn.CheckTimeout(CurrentTime))
                        {
                            conn.ConnectionState = NetConnectionState.TimedOut;
                            conn.OnConnectTerminated(TerminationReason.ReasonTimedOut, "Timeout");

                            removeList.Add(conn);
                        }
                    }
                }

                foreach (var conn in removeList)
                    RemoveConnection(conn);
            }

            foreach (var pair in PendingConnections.Where(pair => pair.Value.ConnectionState == NetConnectionState.ComputingPuzzleSolution))
            {
                ContinuePuzzleSolution(pair.Value);
                break;
            }
        }

        public void CheckIncomingPackets()
        {
            CurrentTime = Environment.TickCount;

            var stream = new PacketStream();

            while (stream.RecvFrom(Socket, out IPEndPoint iep) == NetError.NoError)
                ProcessPacket(iep, stream);
        }

        public virtual void ProcessPacket(IPEndPoint sourceAddress, BitStream stream)
        {
            if ((stream.GetBuffer()[0] & 0x80) != 0)
            {
                // NetInterface::processPacket dispatches the non game info or connection packet to the appropriate
                // NetConnection by looking up the source address and calling readRawPacket
                if (Connections.TryGetValue(sourceAddress, out NetConnection nc) && nc != null)
                    nc.ReadRawPacket(stream);
            }
            else
            {
                stream.Read(out byte packetType);

                if (packetType >= (byte) PacketType.FirstValidInfoPacketId)
                    HandleInfoPacket(sourceAddress, packetType, stream);
                else
                {
                    switch ((PacketType)packetType)
                    {
                        case PacketType.ConnectChallengeRequest:
                            HandleConnectChallengeRequest(sourceAddress, stream);
                            break;

                        case PacketType.ConnectChallengeResponse:
                            HandleConnectChallengeResponse(sourceAddress, stream);
                            break;

                        case PacketType.ConnectRequest:
                            HandleConnectRequest(sourceAddress, stream);
                            break;

                        case PacketType.ConnectReject:
                            HandleConnectReject(sourceAddress, stream);
                            break;

                        case PacketType.ConnectAccept:
                            HandleConnectAccept(sourceAddress, stream);
                            break;

                        case PacketType.Disconnect:
                            HandleDisconnect(sourceAddress, stream);
                            break;

                        case PacketType.Punch:
                            HandlePunch(sourceAddress, stream);
                            break;

                        case PacketType.ArrangedConnectRequest:
                            HandleArrangedConnectRequest(sourceAddress, stream);
                            break;
                    }
                }
            }
        }

        public virtual void HandleInfoPacket(IPEndPoint address, byte packetType, BitStream reader)
        {
        }

        public int GetCurrentTime()
        {
            return CurrentTime;
        }

        public void StartConnection(NetConnection conn)
        {
            if (conn.ConnectionState == NetConnectionState.NotConnected)
                Console.WriteLine("Cannot start unless it is in the NotConnected state.");

            AddPendingConnection(conn);

            conn.ConnectSendCount = 0;
            conn.ConnectionState = NetConnectionState.AwaitingChallengeResponse;

            SendConnectChallengeRequest(conn);
        }

        protected void SendConnectChallengeRequest(NetConnection conn)
        {
            var stream = new PacketStream();

            stream.Write((byte) PacketType.ConnectChallengeRequest);

            var cParam = conn.GetConnectionParameters();

            cParam.Nonce.Write(stream);
            stream.WriteFlag(cParam.RequestKeyExchange);
            stream.WriteFlag(cParam.RequestCertificate);

            ++conn.ConnectSendCount;
            conn.ConnectLastSendTime = CurrentTime;

            stream.SendTo(Socket, conn.GetNetAddress());
        }

        protected void HandleConnectChallengeRequest(IPEndPoint addr, BitStream stream)
        {
            if (!AllowConnections)
                return;

            var clientNonce = new Nonce();
            clientNonce.Read(stream);

            var wantsKeyExchange = stream.ReadFlag();
            var wantsCertificate = stream.ReadFlag();

            SendConnectChallengeResponse(addr, clientNonce, wantsKeyExchange, wantsCertificate);
        }

        protected void SendConnectChallengeResponse(IPEndPoint addr, Nonce clientNonce, bool wantsKeyExchange, bool wantsCertificate)
        {
            var stream = new PacketStream();

            stream.Write((byte) PacketType.ConnectChallengeResponse);

            clientNonce.Write(stream);

            var identityToken = ComputeClientIdentityToken(addr, clientNonce); //  NOTE: correct?
            //identityToken = 0x123331; // false hash results in same behavior
            stream.Write(identityToken);

            PuzzleManager.CurrentNonce.Write(stream);
            stream.Write(PuzzleManager.CurrentDifficulty);

            // ReSharper disable PossibleNullReferenceException
            if (stream.WriteFlag(RequiresKeyExchange || (wantsKeyExchange && PrivateKey != null)))
                stream.Write(stream.WriteFlag(wantsCertificate && Certificate != null) ? Certificate : PrivateKey.GetPublicKey());
            // ReSharper restore PossibleNullReferenceException

            stream.SendTo(Socket, addr);
        }

        protected void HandleConnectChallengeResponse(IPEndPoint address, BitStream stream)
        {
            var conn = FindPendingConnection(address);
            if (conn == null || conn.ConnectionState != NetConnectionState.AwaitingChallengeResponse)
                return;

            var theNonce = new Nonce();
            theNonce.Read(stream);

            var cParams = conn.GetConnectionParameters();
            if (theNonce != cParams.Nonce)
                return;

            stream.Read(out cParams.ClientIdentity);
            cParams.ServerNonce.Read(stream);
            stream.Read(out cParams.PuzzleDifficulty);

            if (cParams.PuzzleDifficulty > ClientPuzzleManager.MaxPuzzleDifficulty)
                return;

            if (stream.ReadFlag())
            {
                if (stream.ReadFlag())
                {
                    cParams.Certificate = new Certificate(stream);
                    if (!cParams.Certificate.IsValid() || !conn.ValidateCertificate(cParams.Certificate, true))
                        return;

                    cParams.PublicKey = cParams.Certificate.GetPublicKey();
                }
                else
                {
                    cParams.PublicKey = new AsymmetricKey(stream);
                    if (!cParams.PublicKey.IsValid() || !conn.ValidatePublicKey(cParams.PublicKey, true))
                        return;
                }

                if (PrivateKey == null || PrivateKey.GetKeySize() != cParams.PublicKey.GetKeySize())
                    cParams.PrivateKey = new AsymmetricKey(cParams.PublicKey.GetKeySize());
                else
                    cParams.PrivateKey = PrivateKey;

                cParams.SharedSecret = cParams.PrivateKey.ComputeSharedSecretKey(cParams.PublicKey);
                RandomUtil.Read(cParams.SymmetricKey, SymmetricCipher.KeySize);

                cParams.UsingCrypto = true;
            }

            conn.ConnectionState = NetConnectionState.ComputingPuzzleSolution;
            conn.ConnectSendCount = 0;

            cParams.PuzzleSolution = 0;

            conn.ConnectLastSendTime = CurrentTime;

            ContinuePuzzleSolution(conn);
        }

        protected void ContinuePuzzleSolution(NetConnection conn)
        {
            var cParams = conn.GetConnectionParameters();
            if (!ClientPuzzleManager.SolvePuzzle(ref cParams.PuzzleSolution, cParams.Nonce, cParams.ServerNonce, cParams.PuzzleDifficulty, cParams.ClientIdentity))
                return;

            conn.ConnectionState = NetConnectionState.AwaitingConnectResponse;

            SendConnectRequest(conn);
        }

        protected void SendConnectRequest(NetConnection conn)
        {
            var stream = new PacketStream();
            var cParams = conn.GetConnectionParameters();

            stream.Write((byte) PacketType.ConnectRequest);
            cParams.Nonce.Write(stream);
            cParams.ServerNonce.Write(stream);
            stream.Write(cParams.ClientIdentity);
            stream.Write(cParams.PuzzleDifficulty);
            stream.Write(cParams.PuzzleSolution);

            var encryptPos = 0U;

            if (stream.WriteFlag(cParams.UsingCrypto))
            {
                stream.Write(cParams.PrivateKey.GetPublicKey());

                encryptPos = stream.GetBytePosition();
                stream.SetBytePosition(encryptPos);

                stream.Write(SymmetricCipher.KeySize, cParams.SymmetricKey);
            }

            stream.WriteFlag(cParams.DebugObjectSizes);
            stream.Write(conn.GetInitialSendSequence());
            stream.WriteString(conn.GetClassName());

            conn.WriteConnectRequest(stream);

            if (encryptPos > 0)
            {
                var theCipher = new SymmetricCipher(cParams.SharedSecret);

                stream.HashAndEncrypt(NetConnection.MessageSignatureBytes, encryptPos, theCipher);
            }

            ++conn.ConnectSendCount;
            conn.ConnectLastSendTime = CurrentTime;

            stream.SendTo(Socket, conn.GetNetAddress());
        }

        protected void HandleConnectRequest(IPEndPoint address, BitStream stream)
        {
            if (!AllowConnections)
                return;

            var cParams = new ConnectionParameters();

            cParams.Nonce.Read(stream);
            cParams.ServerNonce.Read(stream);
            stream.Read(out cParams.ClientIdentity);

            if (cParams.ClientIdentity != ComputeClientIdentityToken(address, cParams.Nonce))
                return;

            stream.Read(out cParams.PuzzleDifficulty);
            stream.Read(out cParams.PuzzleSolution);


            var connect = FindConnection(address);
            if (connect != null)
            {
                var cp = connect.GetConnectionParameters();

                if (cp.Nonce == cParams.Nonce && cp.ServerNonce == cParams.ServerNonce)
                {
                    SendConnectAccept(connect);
                    return;
                }
            }

            var result = PuzzleManager.CheckSolution(cParams.PuzzleSolution, cParams.Nonce, cParams.ServerNonce, cParams.PuzzleDifficulty, cParams.ClientIdentity);
            if (result != ErrorCode.Success)
            {
                SendConnectReject(cParams, address, "Puzzle");
                return;
            }

            if (stream.ReadFlag())
            {
                if (PrivateKey == null)
                    return;

                cParams.UsingCrypto = true;
                cParams.PublicKey = new AsymmetricKey(stream);
                cParams.PrivateKey = PrivateKey;

                var decryptPos = stream.GetBytePosition();

                stream.SetBytePosition(decryptPos);

                cParams.SharedSecret = cParams.PrivateKey.ComputeSharedSecretKey(cParams.PublicKey);

                var theCipher = new SymmetricCipher(cParams.SharedSecret);

                if (!stream.DecryptAndCheckHash(NetConnection.MessageSignatureBytes, decryptPos, theCipher))
                    return;

                stream.Read(SymmetricCipher.KeySize, cParams.SymmetricKey);
                RandomUtil.Read(cParams.InitVector, SymmetricCipher.KeySize);
            }

            cParams.DebugObjectSizes = stream.ReadFlag();

            stream.Read(out uint connectSequence);

            if (connect != null)
                Disconnect(connect, TerminationReason.ReasonSelfDisconnect, "NewConnection");

            stream.ReadString(out string connectionClass);

            var conn = NetConnectionRep.Create(connectionClass);
            if (conn == null)
                return;

            conn.SetConnectionParameters(cParams);
            conn.SetNetAddress(address);
            conn.SetInitialRecvSequence(connectSequence);
            conn.SetInterface(this);

            if (cParams.UsingCrypto)
                conn.SetSymmetricCipher(new SymmetricCipher(cParams.SymmetricKey, cParams.InitVector));

            string errorString = null;
            if (!conn.ReadConnectRequest(stream, ref errorString))
            {
                SendConnectReject(cParams, address, errorString);
                return;
            }

            AddConnection(conn);

            conn.ConnectionState = NetConnectionState.Connected;
            conn.OnConnectionEstablished();

            SendConnectAccept(conn);
        }

        protected void SendConnectAccept(NetConnection conn)
        {
            var stream = new PacketStream();

            stream.Write((byte) PacketType.ConnectAccept);

            var cParams = conn.GetConnectionParameters();

            cParams.Nonce.Write(stream);
            cParams.ServerNonce.Write(stream);

            var encryptPos = stream.GetBytePosition();
            stream.SetBytePosition(encryptPos);

            stream.Write(conn.GetInitialSendSequence());

            conn.WriteConnectAccept(stream);

            if (cParams.UsingCrypto)
            {
                stream.Write(SymmetricCipher.KeySize, cParams.InitVector);

                var theCipher = new SymmetricCipher(cParams.SharedSecret);

                stream.HashAndEncrypt(NetConnection.MessageSignatureBytes, encryptPos, theCipher);
            }

            stream.SendTo(Socket, conn.GetNetAddress());
        }

        protected void HandleConnectAccept(IPEndPoint address, BitStream stream)
        {
            var nonce = new Nonce();
            var serverNonce = new Nonce();

            nonce.Read(stream);
            serverNonce.Read(stream);

            var decryptPos = stream.GetBytePosition();

            stream.SetBytePosition(decryptPos);

            var conn = FindPendingConnection(address);
            if (conn == null || conn.ConnectionState != NetConnectionState.AwaitingConnectResponse)
                return;

            var cParams = conn.GetConnectionParameters();

            if (cParams.Nonce != nonce || cParams.ServerNonce != serverNonce)
                return;

            if (cParams.UsingCrypto)
            {
                var theCipher = new SymmetricCipher(cParams.SharedSecret);

                if (!stream.DecryptAndCheckHash(NetConnection.MessageSignatureBytes, decryptPos, theCipher))
                    return;
            }

            stream.Read(out uint recvSequence);
            conn.SetInitialRecvSequence(recvSequence);

            string errorString = null;
            if (!conn.ReadConnectAccept(stream, ref errorString))
            {
                RemovePendingConnection(conn);
                return;
            }

            if (cParams.UsingCrypto)
            {
                stream.Read(SymmetricCipher.KeySize, cParams.InitVector);

                conn.SetSymmetricCipher(new SymmetricCipher(cParams.SymmetricKey, cParams.InitVector));
            }

            AddConnection(conn);
            RemovePendingConnection(conn);

            conn.ConnectionState = NetConnectionState.Connected;
            conn.OnConnectionEstablished();
        }

        protected void SendConnectReject(ConnectionParameters conn, IPEndPoint address, string reason)
        {
            if (reason == null)
                return;

            var stream = new PacketStream();

            stream.Write((byte) PacketType.ConnectReject);

            conn.Nonce.Write(stream);
            conn.ServerNonce.Write(stream);

            stream.WriteString(reason);

            stream.SendTo(Socket, address);
        }

        protected void HandleConnectReject(IPEndPoint address, BitStream stream)
        {
            var nonce = new Nonce();
            var serverNonce = new Nonce();

            nonce.Read(stream);
            serverNonce.Read(stream);

            var conn = FindPendingConnection(address);
            if (conn == null || (conn.ConnectionState != NetConnectionState.AwaitingChallengeResponse && conn.ConnectionState != NetConnectionState.AwaitingConnectResponse))
                return;

            var cParams = conn.GetConnectionParameters();
            if (cParams.Nonce != nonce || cParams.ServerNonce != serverNonce)
                return;

            stream.ReadString(out string reason);

            if (reason == "Puzzle")
            {
                cParams.PuzzleRetried = true;

                conn.ConnectionState = NetConnectionState.AwaitingChallengeResponse;
                conn.ConnectSendCount = 0;

                cParams.Nonce.GetRandom();

                SendConnectChallengeRequest(conn);

                return;
            }

            conn.ConnectionState = NetConnectionState.ConnectRejected;
            conn.OnConnectTerminated(TerminationReason.ReasonRemoteHostRejectedConnection, reason);

            RemovePendingConnection(conn);
        }

        public void StartArrangedConnection(NetConnection conn)
        {
            conn.ConnectionState = NetConnectionState.SendingPunchPackets;

            AddPendingConnection(conn);

            conn.ConnectSendCount = 0;
            conn.ConnectLastSendTime = CurrentTime;

            SendPunchPackets(conn);
        }

        protected void SendPunchPackets(NetConnection conn)
        {
            var cParams = conn.GetConnectionParameters();

            var stream = new PacketStream();

            stream.Write((byte) PacketType.Punch);

            if (cParams.IsInitiator)
                cParams.Nonce.Write(stream);
            else
                cParams.ServerNonce.Write(stream);

            var encryptPos = stream.GetBytePosition();

            stream.SetBytePosition(encryptPos);

            if (cParams.IsInitiator)
                cParams.ServerNonce.Write(stream);
            else
            {
                cParams.Nonce.Write(stream);

                if (stream.WriteFlag(RequiresKeyExchange || (cParams.RequestKeyExchange && PrivateKey != null)))
                    stream.Write(stream.WriteFlag(cParams.RequestCertificate && Certificate != null) ? Certificate : PrivateKey.GetPublicKey());
            }

            var theCipher = new SymmetricCipher(cParams.ArrangedSecret);
            stream.HashAndEncrypt(NetConnection.MessageSignatureBytes, encryptPos, theCipher);

            foreach (var ep in cParams.PossibleAddresses)
            {
                stream.SendTo(Socket, ep);

                Console.WriteLine("Sending punch packet ({0}, {1}) to {2}", Convert.ToBase64String(cParams.Nonce.Data), Convert.ToBase64String(cParams.ServerNonce.Data), ep);
            }

            ++conn.ConnectSendCount;
            conn.ConnectLastSendTime = GetCurrentTime();
        }

        protected void HandlePunch(IPEndPoint address, BitStream stream)
        {
            var firstNonce = new Nonce();
            firstNonce.Read(stream);

            Console.WriteLine("Received punch packet from {0} - {1}", address, Convert.ToBase64String(firstNonce.Data));

            var found = false;
            NetConnection conn = null;

            foreach (var pair in PendingConnections)
            {
                conn = pair.Value;
                var cParams = conn.GetConnectionParameters();

                if (conn.ConnectionState != NetConnectionState.SendingPunchPackets)
                    continue;

                if ((cParams.IsInitiator && firstNonce != cParams.ServerNonce) || (!cParams.IsInitiator && firstNonce != cParams.Nonce))
                    continue;

                if (cParams.PossibleAddresses.Contains(address))
                {
                    if (cParams.IsInitiator)
                    {
                        found = true;
                        break;
                    }

                    continue;
                }

                var cont = cParams.PossibleAddresses.Any(ep => ep.Address.Equals(address.Address) && ep.AddressFamily == address.AddressFamily);

                if (!cont)
                    continue;

                if (cParams.PossibleAddresses.Count < 5)
                    cParams.PossibleAddresses.Add(address);

                if (cParams.IsInitiator)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return;

            var cParam = conn.GetConnectionParameters();
            var theCipher = new SymmetricCipher(cParam.ArrangedSecret);

            if (stream.DecryptAndCheckHash(NetConnection.MessageSignatureBytes, stream.GetBytePosition(), theCipher))
                return;

            var nextNonce = new Nonce();
            nextNonce.Read(stream);

            if (nextNonce != cParam.Nonce)
                return;

            if (stream.ReadFlag())
            {
                if (stream.ReadFlag())
                {
                    cParam.Certificate = new Certificate(stream);
                    if (!cParam.Certificate.IsValid() || !conn.ValidateCertificate(cParam.Certificate, true))
                        return;

                    cParam.PublicKey = cParam.Certificate.GetPublicKey();
                }
                else
                {
                    cParam.PublicKey = new AsymmetricKey(stream);
                    if (!cParam.PublicKey.IsValid() || !conn.ValidatePublicKey(cParam.PublicKey, true))
                        return;
                }

                if (PrivateKey == null || PrivateKey.GetKeySize() != cParam.PublicKey.GetKeySize())
                    cParam.PrivateKey = new AsymmetricKey(cParam.PublicKey.GetKeySize());
                else
                    cParam.PrivateKey = PrivateKey;

                cParam.SharedSecret = cParam.PrivateKey.ComputeSharedSecretKey(cParam.PublicKey);

                RandomUtil.Read(cParam.SymmetricKey, SymmetricCipher.KeySize);

                cParam.UsingCrypto = true;
            }
        }

        protected void SendArrangedConnectRequest(NetConnection conn)
        {
            var stream = new PacketStream();

            stream.Write((byte) PacketType.ArrangedConnectRequest);

            var cParams = conn.GetConnectionParameters();

            cParams.Nonce.Write(stream);

            var encryptPos = stream.GetBytePosition();
            var innerEncryptPos = 0U;
            stream.SetBytePosition(encryptPos);

            cParams.ServerNonce.Write(stream);

            if (stream.WriteFlag(cParams.UsingCrypto))
            {
                stream.Write(cParams.PrivateKey.GetPublicKey());

                innerEncryptPos = stream.GetBytePosition();
                stream.SetBytePosition(innerEncryptPos);

                stream.Write(SymmetricCipher.KeySize, cParams.SymmetricKey);
            }

            stream.WriteFlag(cParams.DebugObjectSizes);
            stream.Write(conn.GetInitialSendSequence());

            conn.WriteConnectRequest(stream);

            if (innerEncryptPos > 0)
            {
                var theCipher = new SymmetricCipher(cParams.SharedSecret);

                stream.HashAndEncrypt(NetConnection.MessageSignatureBytes, innerEncryptPos, theCipher);
            }

            var theCipher2 = new SymmetricCipher(cParams.ArrangedSecret);

            stream.HashAndEncrypt(NetConnection.MessageSignatureBytes, encryptPos, theCipher2);

            ++conn.ConnectSendCount;
            conn.ConnectLastSendTime = GetCurrentTime();

            stream.SendTo(Socket, conn.GetNetAddress());
        }

        protected void HandleArrangedConnectRequest(IPEndPoint client, BitStream reader)
        {
            var nonce = new Nonce();
            nonce.Read(reader);

            var oldConnection = FindConnection(client);
            if (oldConnection != null)
            {
                var cp = oldConnection.GetConnectionParameters();
                if (cp.Nonce == nonce)
                {
                    SendConnectAccept(oldConnection);
                    return;
                }
            }

            NetConnection conn = null;
            var found = false;

            foreach (var pair in PendingConnections)
            {
                conn = pair.Value;
                var cp = conn.GetConnectionParameters();

                if (conn.ConnectionState != NetConnectionState.SendingPunchPackets || cp.IsInitiator)
                    continue;

                if (nonce != cp.Nonce)
                    continue;

                if (cp.PossibleAddresses.Any(ep => ep.Address.Equals(client.Address) && ep.AddressFamily == client.AddressFamily))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return;

            var cParams = conn.GetConnectionParameters();
            var theCipher = new SymmetricCipher(cParams.ArrangedSecret);

            if (!reader.DecryptAndCheckHash(NetConnection.MessageSignatureBytes, reader.GetBytePosition(), theCipher))
                return;

            reader.SetBytePosition(reader.GetBytePosition());

            var serverNonce = new Nonce();
            serverNonce.Read(reader);
            if (serverNonce != cParams.ServerNonce)
                return;

            if (reader.ReadFlag())
            {
                if (PrivateKey == null)
                    return;

                cParams.UsingCrypto = true;
                cParams.PublicKey = new AsymmetricKey(reader);
                cParams.PrivateKey = PrivateKey;

                var decryptPos = reader.GetBytePosition();
                reader.SetBytePosition(decryptPos);

                cParams.SharedSecret = cParams.PrivateKey.ComputeSharedSecretKey(cParams.PublicKey);

                var theCipher2 = new SymmetricCipher(cParams.SharedSecret);

                if (!reader.DecryptAndCheckHash(NetConnection.MessageSignatureBytes, decryptPos, theCipher2))
                    return;

                reader.Read(NetConnection.MessageSignatureBytes, cParams.SymmetricKey);
                RandomUtil.Read(cParams.InitVector, SymmetricCipher.KeySize);
            }

            cParams.DebugObjectSizes = reader.ReadFlag();
            reader.Read(out uint connectSequence);

            if (oldConnection != null)
                Disconnect(oldConnection, TerminationReason.ReasonSelfDisconnect, "");

            conn.SetNetAddress(client);
            conn.SetInitialRecvSequence(connectSequence);

            if (cParams.UsingCrypto)
                conn.SetSymmetricCipher(new SymmetricCipher(cParams.SymmetricKey, cParams.InitVector));

            string errorString = null;
            if (!conn.ReadConnectRequest(reader, ref errorString))
            {
                SendConnectReject(cParams, client, errorString);
                RemovePendingConnection(conn);
                return;
            }

            AddConnection(conn);

            RemovePendingConnection(conn);

            conn.ConnectionState = NetConnectionState.Connected;
            conn.OnConnectionEstablished();

            SendConnectAccept(conn);
        }

        public void Disconnect(NetConnection conn, TerminationReason reason, string reasonString)
        {
            if (conn.ConnectionState == NetConnectionState.AwaitingChallengeResponse ||
                conn.ConnectionState == NetConnectionState.AwaitingConnectResponse)
            {
                conn.OnConnectTerminated(reason, reasonString);
                RemovePendingConnection(conn);
            }
            else if (conn.ConnectionState == NetConnectionState.Connected)
            {
                conn.ConnectionState = NetConnectionState.Disconnected;
                conn.OnConnectionTerminated(reason, reasonString);

                if (conn.IsNetworkConnection())
                {
                    var stream = new PacketStream();
                    stream.Write((byte) PacketType.Disconnect);

                    var cParams = conn.GetConnectionParameters();

                    cParams.Nonce.Write(stream);
                    cParams.ServerNonce.Write(stream);

                    var encryptPos = stream.GetBytePosition();
                    stream.SetBytePosition(encryptPos);

                    stream.WriteString(reasonString);

                    if (cParams.UsingCrypto)
                    {
                        var theCipher = new SymmetricCipher(cParams.SharedSecret);

                        stream.HashAndEncrypt(NetConnection.MessageSignatureBytes, encryptPos, theCipher);
                    }

                    stream.SendTo(Socket, conn.GetNetAddress());
                }

                RemoveConnection(conn);
            }
        }

        protected void HandleDisconnect(IPEndPoint client, BitStream reader)
        {
            var conn = FindConnection(client);
            if (conn == null)
                return;

            var cParams = conn.GetConnectionParameters();

            var nonce = new Nonce();
            nonce.Read(reader);

            var serverNonce = new Nonce();
            serverNonce.Read(reader);

            if (nonce != cParams.Nonce || serverNonce != cParams.ServerNonce)
                return;

            var decryptPos = reader.GetBytePosition();
            reader.SetBytePosition(decryptPos);

            if (cParams.UsingCrypto)
            {
                var theCipher = new SymmetricCipher(cParams.SharedSecret);

                if (!reader.DecryptAndCheckHash(NetConnection.MessageSignatureBytes, decryptPos, theCipher))
                    return;
            }

            reader.ReadString(out string reason);

            conn.ConnectionState = NetConnectionState.Disconnected;
            conn.OnConnectionTerminated(TerminationReason.ReasonRemoteDisconnectPacket, reason);

            RemoveConnection(conn);
        }

        public void HandleConnectionError(NetConnection conn, string reasonString)
        {
            Disconnect(conn, TerminationReason.ReasonError, reasonString);
        }

        protected class DelaySendPacket
        {
            public IPEndPoint RemoteAddress { get; set; }
            public int SendTime { get; set; }
            public byte[] PacketData { get; set; }
            public uint PacketSize { get; set; }
        }
    }
}
