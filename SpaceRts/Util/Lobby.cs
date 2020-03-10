using System;
using System.Collections.Generic;

namespace SpaceRts.Util
{
    public class LobbyData
    {
        public string ID;
        public string Owner;
        public string[] Members;
        public List<ChatEntry> Chat;

        public LobbyData(string iD, string owner, string[] members, List<ChatEntry> chat)
        {
            ID = iD;
            Owner = owner;
            Members = members;
            Chat = chat;
        }
    }

    public class ChatEntry
    {
        public string UserName, Message;

        public ChatEntry(string userName, string message)
        {
            UserName = userName;
            Message = message;
        }
    }
}
