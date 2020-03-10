using System;
using System.Collections.Generic;

namespace SpaceRtsServer.Util
{
    public class Database
    {
        public Dictionary<string, string> Usernames = new Dictionary<string, string> { {"test", "patrik"} };

        public Database()
        {
        }

        public int TimesCalled;
        public void GetUsername(string authToken, Action<string> response)
        {
            response("Patrik" + TimesCalled);
            TimesCalled++;
        }
    }
}
