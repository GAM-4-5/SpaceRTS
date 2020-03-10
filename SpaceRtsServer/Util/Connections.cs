using System;
using System.Collections.Generic;
using Lidgren.Network;

namespace SpaceRtsServer.Util
{
    public static class Connections
    {
        public static Dictionary<string, Connection> ActiveConnections = new Dictionary<string, Connection>();
    }

    public class Connection
    {
        public string Username;
        public string GUID;
        public NetConnection NetConnection;
        public bool conneceted;

        private readonly string AuthToken;

        public Connection(string authToken, string guid, NetConnection connection)
        {
            AuthToken = authToken;
            GUID = guid;
            NetConnection = connection;

            MasterServer.Database.GetUsername(AuthToken, (string username) => Username = username);
        }
    }
}
