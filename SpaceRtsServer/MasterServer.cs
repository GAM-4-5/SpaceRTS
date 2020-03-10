using System;
using System.Collections.Generic;
using Lidgren.Network;
using SpaceRts;
using SpaceRts.Channels;
using SpaceRts.Util;
using SpaceRtsServer.Util;
using static SpaceRts.Channels.Channel;

namespace SpaceRtsServer
{
    public class MasterServer
    {
        public static Dictionary<int, List<Tuple<Action<NetIncomingMessage, object>, bool>>> Hooks = new Dictionary<int, List<Tuple<Action<NetIncomingMessage, object>, bool>>>(TOTAL_CHANNELS);

        public static Dictionary<string, Lobby> Lobbies = new Dictionary<string, Lobby>();

        public static Database Database;

        public void Start()
        {
            Database = new Database();

            NetPeerConfiguration config = new NetPeerConfiguration("SpaceRts")
            {
                Port = 42424,

            };
            //config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            NetServer server = new NetServer(config);
            server.Start();

            NetIncomingMessage message;

            Console.WriteLine("Server lisening");

            InitilizeHooks();

            while (true)
            {
                while ((message = server.ReadMessage()) != null)
                {
                    Console.WriteLine("RECIEVED");
                    Console.WriteLine(message.MessageType.ToString());

                    object target = new object();

                    if (message.SenderConnection != null)
                    {
                        Console.WriteLine("REEEEEEEEE:: " + message.SenderConnection.RemoteUniqueIdentifier);
                    }


                    switch (message.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            HandleRequest(message);
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.WriteLine(message.SenderConnection.Status);
                            Console.ResetColor();
                            switch (message.SenderConnection.Status)
                            {
                                case NetConnectionStatus.Connected:
                                    Connections.ActiveConnections.Add(message.SenderConnection.RemoteUniqueIdentifier.ToString(), new Connection(message.SenderConnection.RemoteHailMessage.ReadString(), message.SenderConnection.RemoteUniqueIdentifier.ToString(), message.SenderConnection));
                                    break;

                            }
                            break;

                        case NetIncomingMessageType.DebugMessage:
                            // handle debug messages
                            // (only received when compiled in DEBUG mode)
                            Console.WriteLine(message.ReadString());
                            break;

                        /* .. */
                        default:
                            Console.WriteLine("unhandled message with type: "
                                + message.MessageType);
                            break;
                    }

                    server.Recycle(message);
                }
            }
        }

        private void InitilizeHooks()
        {
            AddHook(Channels.Lobby, LobbyChannel.Create, CreateLobby, false);
        }

        private void CreateLobby(NetIncomingMessage message, object obj)
        {
            switch (obj)
            {
                case string str:
                    string guid = Guid.NewGuid().ToString();
                    Console.WriteLine("Creating lobby: " + guid);
                    //Lobbies.Add(guid, new Lobby(guid, message.SenderConnection, ));
                    break;
                default:
                    break;
            }
        }

        private void JoinLobby(NetIncomingMessage message, object obj)
        {
            switch (obj)
            {
                case string str:
                    Console.WriteLine(str);
                    break;
                default:
                    break;
            }
        }

        private void SendChat(NetIncomingMessage message, object obj)
        {
            switch (obj)
            {
                case string str:
                    Console.WriteLine(str);
                    break;
                default:
                    break;
            }
        }


        private void AddHook(int channel, int subChannel, Action<NetIncomingMessage, object> hook, bool oneTime)
        {
            int key = channel * CHANNEL_SIZE + subChannel;
            Hooks.TryGetValue(key, out List<Tuple<Action<NetIncomingMessage, object>, bool>> hooks);
            if (hooks != null)
            {
                hooks.Add(new Tuple<Action<NetIncomingMessage, object>, bool>(hook, oneTime));
            }
            else
            {
                var newHooks = new List<Tuple<Action<NetIncomingMessage, object>, bool>>
                {
                    new Tuple<Action<NetIncomingMessage, object>, bool>(hook, oneTime)
                };
                Hooks.Add(key, newHooks);
            }
        }

        #region AddHook
        public void AddHook(Channels channel, GameChannel subChannel, Action<NetIncomingMessage, object> hook, bool oneTime)
        {
            AddHook((int)channel, (int)subChannel, hook, oneTime);
        }

        public void AddHook(Channels channel, LobbyChannel subChannel, Action<NetIncomingMessage, object> hook, bool oneTime)
        {
            AddHook((int)channel, (int)subChannel, hook, oneTime);
        }

        public void AddHook(Channels channel, AuthChannel subChannel, Action<NetIncomingMessage, object> hook, bool oneTime)
        {
            AddHook((int)channel, (int)subChannel, hook, oneTime);
        }
        #endregion


        public static void HandleRequest(NetIncomingMessage message)
        {
            int channel = message.SequenceChannel;
            int subChannel = message.ReadRangedInteger(0, TOTAL_CHANNELS);
            int key = channel * CHANNEL_SIZE + subChannel;
            Hooks.TryGetValue(key, out List<Tuple<Action<NetIncomingMessage, object>, bool>> hooks);
            if (hooks != null)
            {
                message.SkipPadBits();
                int numberOfBytes = message.ReadInt32();
                message.SkipPadBits();

                byte[] data = message.ReadBytes(numberOfBytes);

                object obj = ObjectSerialization.ByteArrayToObject(data);
                for (int i = 0; i < hooks.Count; i++)
                {
                    hooks[i]?.Item1?.Invoke(message, ObjectSerialization.ByteArrayToObject(data));
                }
            }
            else
            {
                Console.WriteLine("Channel not handeled: {0}, {1}", channel, subChannel);
            }
        }
    }
}
