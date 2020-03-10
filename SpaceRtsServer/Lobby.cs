using System;
using System.Collections.Generic;
using Lidgren.Network;
using SpaceRts;
using SpaceRts.Channels;
using SpaceRts.Util;
using static SpaceRts.Channels.Channel;

namespace SpaceRtsServer
{
    public class Lobby
    {
        public Dictionary<string, NetConnection> Members;
        public NetServer MasterServer;

        LobbyData LobbyData;

        public Lobby(string id, NetConnection nc, string owner, NetServer masterServer)
        {
            MasterServer = masterServer;

            LobbyData = new LobbyData(id, owner, new string[] { owner }, new List<ChatEntry>
            {
                new ChatEntry("Server", owner + "joined to lobby.")
            }
            );
            Members = new Dictionary<string, NetConnection>
            {
                { owner, nc }
            };
        }

        /// <summary>
        /// Adds the user to lobby.
        /// </summary>
        /// <param name="netConnection">NetConnection of user.</param>
        /// <param name="username">Username of user.</param>
        public void AddUser(NetConnection netConnection, string username)
        {
            Members.Add(username, netConnection);
        }

        /// <summary>
        /// Kicks the user from lobby.
        /// </summary>
        /// <param name="username">Username of user</param>
        public void KickUser(string username)
        {
            Members.TryGetValue(username, out NetConnection nc);

            if (nc != null)
            {
                NetOutgoingMessage m = MasterServer.CreateMessage();
                Channel.SetSubChannel(m, (int)LobbyChannel.Kick);
                MasterServer.SendMessage(m, nc, NetDeliveryMethod.ReliableOrdered, (int)Channels.Lobby);

                Members.Remove(username);
            }
        }

        public void SendChat()
        {

        }
    }
}
