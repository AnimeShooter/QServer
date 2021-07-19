using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Qserver.GameServer;

namespace Qserver.External.Websocket
{
    // NOTE: takes care of all web socket connections and turns them into WebUser
    public class NetServer
    {
        public static List<WebUser> Users = new List<WebUser>();
        private Task acceptClientsTask;
        private bool acceptClientsTaskRun = true;

        private Task DisconnectingClientsTask;
        private bool disconnectingClientsTaskRun = true;

        private TcpListener Listener { get; }
        internal List<NetClient> ConnectedClients { get; private set; }
        private object ConnectedClientsLockObj = new object();

        private DateTime LastCleanup = DateTime.UtcNow;

        public static NetServer Instance { private set; get; }

        internal ConcurrentQueue<NetClient> DisconnectingClients { get; }

        public NetServer()
        {
            this.ConnectedClients = new List<NetClient>();
            this.DisconnectingClients = new ConcurrentQueue<NetClient>();
            this.DisconnectingClientsTask = Task.Run(HandleDisconnectingClientsAsync);
            this.Listener = new TcpListener(IPAddress.Any, Settings.WS_PORT);
            Instance = this;
        }

        public static void CleanupUsers()
        {
            lock (Users)
            {
                List<WebUser> OldBots = new List<WebUser>();
                for (int i = 0; i < Users.Count; i++)
                    if (Users[i].LastHeartbeat.AddSeconds(30) < DateTime.UtcNow || !Users[i].NetClient.Client.Connected || !Users[i].NetClient.Client.Client.Connected) // remove 60sec inactive ones
                        OldBots.Add(Users[i]);
                foreach (var b in OldBots)
                    Users.Remove(b);
            }
        }

        public void UpdateStats()
        {
            try
            {
                lock (Users)
                {
                    foreach (var user in Users)
                        user.SendStats();
                }
            }
            catch { }
            
        }

        private async Task HandleDisconnectingClientsAsync()
        {
            while (this.disconnectingClientsTaskRun)
            {
                if (this.DisconnectingClients.TryDequeue(out NetClient client))
                {
                    lock (this.ConnectedClientsLockObj)
                        this.ConnectedClients.Remove(client);
                }
                else
                    await Task.Delay(250);
            }
        }

        public void Start()
        {
            this.Listener.Start();
            this.acceptClientsTask = Task.Run(AcceptClientsAsync);
        }

        public void Stop()
        {
            this.acceptClientsTaskRun = false;

            List<NetClient> copy;

            lock (this.ConnectedClientsLockObj)
                copy = new List<NetClient>(this.ConnectedClients);

            foreach (var client in copy)
            {
                client.DisconnectAndDispose();
            }

            Thread.Sleep(1000);
            this.disconnectingClientsTaskRun = false;

            lock (NetServer.Users)
                Users.Clear();
        }

        private async Task AcceptClientsAsync()
        {
            while (this.acceptClientsTaskRun)
            {
                try
                {
                    // quick cleanup?
                    if (LastCleanup.AddSeconds(30) < DateTime.UtcNow)
                    {
                        LastCleanup = DateTime.UtcNow;
                        CleanupUsers();
                    }

                    var client = await this.Listener.AcceptTcpClientAsync();
#if DEBUG
                    Console.WriteLine($"Accepted new client [{client?.Client?.RemoteEndPoint?.ToString() ?? "unknown ip"}].");
#endif
                    var netclient = new NetClient(client, this);
                    lock (this.ConnectedClientsLockObj)
                        this.ConnectedClients.Add(netclient);

                    lock (NetServer.Users)
                    {
                        Users.Add(new WebUser(netclient)
                        {
                            RemoteAddress = client?.Client?.RemoteEndPoint?.ToString(),
                            ConnectedAt = DateTime.UtcNow,
                            LastHeartbeat = DateTime.UtcNow
                        });

#if DEBUG
                        foreach (var b in NetServer.Users)
                            Console.WriteLine(b.ToString());
#endif
                    }
                }
                catch (Exception ex)
                {
                    if (!(ex is ObjectDisposedException && !this.acceptClientsTaskRun)) // if it wasnt triggered by the AcceptTcpClientAsync
                    {
                        Console.WriteLine($"Uncaught exception!\n{ex.ToString()}\nInnerException: ({ex.InnerException})\nStactrace:\n{ex.StackTrace} "/*, LogLevel.Error*/);
                    }
                }
            }
        }
    }
}
