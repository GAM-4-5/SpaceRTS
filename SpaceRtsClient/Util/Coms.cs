using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Lidgren.Network;
using SpaceRts.Channels;
using SpaceRts.Util;
using static SpaceRts.Channels.Channel;

namespace SpaceRtsClient.Util
{
    public static class Coms
    {
        public static string UUID;
        public static NetClient Connection;

        /// <summary>
        /// Dictionary of hooks for channel sockets.
        /// </summary>
        public static Dictionary<int, List<Tuple<Action<object>, bool>>> Hooks = new Dictionary<int, List<Tuple<Action<object>, bool>>>(TOTAL_CHANNELS);
        public static void Initilize()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("SpaceRts")
            {
                Port = 42424
            };

            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            Connection = new NetClient(config);
            Connection.Start();
            Connection.Connect("127.0.0.1", 42424);

            NetIncomingMessage message;
            while (true)
            {
                while ((message = Connection.ReadMessage()) != null)
                {
                    Console.WriteLine("RECIEVED");
                    Console.WriteLine(message.MessageType.ToString());

                    object target = new object();

                    if (message.SenderConnection != null)
                    {

                    }


                    switch (message.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            HandleRequest(message);
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            // handle connection status messages
                            switch (message.SenderConnection.Status)
                            {
                                case NetConnectionStatus.InitiatedConnect:
                                    Console.WriteLine("Initial Connect");
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

                    Connection.Recycle(message);
                }
            }

        }


        #region LobbyMethods

        public static void RequestLobbyes(Action<object> response)
        {
            SendMessageAndGetResponse(Channels.Lobby, LobbyChannel.Create, "0", response);
        }

        public static void CreateLobby(Action<object> response)
        {
            SendMessageAndGetResponse(Channels.Lobby, LobbyChannel.Create, "0", response);
        }

        public static void JoinLobby(string id, Action<object> response)
        {
            SendMessageAndGetResponse(Channels.Lobby, LobbyChannel.Create, id, response);
        }

        public static void SendChat(string message)
        {
            SendMessage(Channels.Lobby, LobbyChannel.Chat, message, NetDeliveryMethod.ReliableOrdered);
        }

        public static void KickPlayer(string username)
        {
            SendMessage(Channels.Lobby, LobbyChannel.Kick, username, NetDeliveryMethod.ReliableOrdered);
        }

        //public static void SetLeader(string username)
        //{
        //    SendMessage(Channels.Lobby, LobbyChannel., username, NetDeliveryMethod.ReliableOrdered);
        //}

        #endregion

        public static void HandleRequest(NetIncomingMessage message)
        {
            int channel = message.SequenceChannel;
            int subChannel = message.ReadRangedInteger(0, TOTAL_CHANNELS);
            int key = channel * CHANNEL_SIZE + subChannel;
            Hooks.TryGetValue(key, out List<Tuple<Action<object>, bool>> hooks);
            if (hooks != null)
            {
                int numberOfBytes = message.ReadInt32();
                message.SkipPadBits();
                byte[] data = message.ReadBytes(numberOfBytes);
                for (int i = 0; i < hooks.Count; i++)
                {
                    hooks[i]?.Item1?.Invoke(ObjectSerialization.ByteArrayToObject(data));
                }
            }
            else
            {
                Console.WriteLine("Channel not handeled: {0}, {1}", channel, subChannel);
            }
        }
        /// <summary>
        /// Adds the hook to hooks dict.
        /// </summary>
        /// <param name="channel">Channel of hook.</param>
        /// <param name="hook">Hook action to call.</param>
        /// <param name="oneTime">If set to <c>true</c> hook shall be called just one time.</param>
        public static void AddHook(int channel, int subchannel, Action<object> hook, bool oneTime)
        {
            int key = channel * CHANNEL_SIZE + subchannel;
            Hooks.TryGetValue(key, out List<Tuple<Action<object>, bool>> hooks);
            if (hooks != null)
            {
                hooks.Add(new Tuple<Action<object>, bool>(hook, oneTime));
            }
            else
            {
                var newHooks = new List<Tuple<Action<object>, bool>>
                {
                    new Tuple<Action<object>, bool>(hook, oneTime)
                };
                Hooks.Add(key, newHooks);
            }
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="channel">Channel socket to use.</param>
        /// <param name="subchannel">Subchannel v-socket to use.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="method">Method to use for socket transport.</param>
        public static void SendMessage(int channel, int subchannel, object message, NetDeliveryMethod method)
        {
            NetOutgoingMessage msg = Connection.CreateMessage();

            msg.WriteRangedInteger(0, TOTAL_CHANNELS, channel * CHANNEL_SIZE + subchannel);
            msg.WritePadBits();

            byte[] serialized = ObjectSerialization.ObjectToByteArray(message);
            msg.Write(serialized.Length);
            msg.WritePadBits();
            msg.Write(serialized);

            Connection.SendMessage(msg, method);
        }

        public static void SendMessageAndGetResponse(int channel, int subchannel, object message, Action<object> response)
        {
            AddHook(channel, subchannel, response, true);
            SendMessage(channel, subchannel, message, NetDeliveryMethod.ReliableOrdered);
        }

        #region SendMessage
        public static void SendMessage(Channels channel, GameChannel subChannel, object message, NetDeliveryMethod method)
        {
            SendMessage((int)channel, (int)subChannel, message, method);
        }

        public static void SendMessage(Channels channel, LobbyChannel subChannel, object message, NetDeliveryMethod method)
        {
            SendMessage((int)channel, (int)subChannel, message, method);
        }

        public static void SendMessage(Channels channel, AuthChannel subChannel, object message, NetDeliveryMethod method)
        {
            SendMessage((int)channel, (int)subChannel, message, method);
        }
        #endregion

        #region AddHook
        public static void AddHook(Channels channel, GameChannel subChannel, Action<object> hook, bool oneTime)
        {
            AddHook((int)channel, (int)subChannel, hook, oneTime);
        }

        public static void AddHook(Channels channel, LobbyChannel subChannel, Action<object> hook, bool oneTime)
        {
            AddHook((int)channel, (int)subChannel, hook, oneTime);
        }

        public static void AddHook(Channels channel, AuthChannel subChannel, Action<object> hook, bool oneTime)
        {
            AddHook((int)channel, (int)subChannel, hook, oneTime);
        }
        #endregion

        #region SendMessageAndGetResponse
        public static void SendMessageAndGetResponse(Channels channel, GameChannel subChannel, object message, Action<object> response)
        {
            SendMessageAndGetResponse((int)channel, (int)subChannel, message, response);
        }

        public static void SendMessageAndGetResponse(Channels channel, LobbyChannel subChannel, object message, Action<object> response)
        {
            SendMessageAndGetResponse((int)channel, (int)subChannel, message, response);
        }

        public static void SendMessageAndGetResponse(Channels channel, AuthChannel subChannel, object message, Action<object> response)
        {
            SendMessageAndGetResponse((int)channel, (int)subChannel, message, response);
        }
        #endregion
    }
}
