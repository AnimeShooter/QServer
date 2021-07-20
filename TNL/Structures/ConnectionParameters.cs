using System.Collections.Generic;
using System.Net;

namespace TNL.Structures
{
    using Entities;
    using Utils;

    public class ConnectionParameters
    {
        public bool IsArranged;
        public bool UsingCrypto;
        public bool PuzzleRetried;
        public Nonce Nonce;
        public Nonce ServerNonce;
        public uint PuzzleDifficulty;
        public uint PuzzleSolution;
        public uint ClientIdentity;
        public AsymmetricKey PublicKey;
        public AsymmetricKey PrivateKey;
        public Certificate Certificate;
        public ByteBuffer SharedSecret;
        public bool RequestKeyExchange;
        public bool RequestCertificate;
        public byte[] SymmetricKey = new byte[SymmetricCipher.KeySize];
        public byte[] InitVector = new byte[SymmetricCipher.KeySize];
        public List<IPEndPoint> PossibleAddresses;
        public bool IsInitiator;
        public bool IsLocal;
        public ByteBuffer ArrangedSecret;
        public bool DebugObjectSizes;

        public ConnectionParameters()
        {
            IsInitiator = false;
            PuzzleRetried = false;
            UsingCrypto = false;
            IsArranged = false;
            DebugObjectSizes = false;
            IsLocal = false;

            Nonce = new();
            ServerNonce = new();
        }
    }
}
