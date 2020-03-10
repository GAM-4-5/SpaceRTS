using System;
using System.Threading;
using Lidgren.Network;

namespace SpaceRtsServer
{
    class MainClass
    {
        public static void Main(string[] args)
        {


            Console.WriteLine("Server start");

            MasterServer masterServer = new MasterServer();
            Thread serverThread = new Thread(new ThreadStart(masterServer.Start));
            serverThread.Start();


            Thread.Sleep(1000);

            Console.WriteLine("Client start");

            //NetOutgoingMessage sendMsg = server.CreateMessage();
            //sendMsg.Write("Hello");
            //sendMsg.Write(42);

            var config2 = new NetPeerConfiguration("SpaceRts");
            var client = new NetClient(config2);

            client.Start();

            var hailMessage1 = client.CreateMessage();
            hailMessage1.Write("AUTH1");

            client.Connect("127.0.0.1", 42424, hailMessage1);


            for (int i = 0; i < 5; i++)
            {
                var msg = client.CreateMessage();
                msg.Write("Test123");

                client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);

                Console.WriteLine("Client sent");

                Thread.Sleep(1000);
            }

            var config3 = new NetPeerConfiguration("SpaceRts");
            var client2 = new NetClient(config2);
            client2.Start();
            var hailMessage2 = client2.CreateMessage();
            hailMessage2.Write("AUTH_2");
            client2.Connect("127.0.0.1", 42424, hailMessage2);


            for (int i = 0; i < 5; i++)
            {
                var msg = client2.CreateMessage();
                msg.Write("Test13");

                client2.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);

                Console.WriteLine("Client sent");

                Thread.Sleep(1000);
            }
        }
    }
}
