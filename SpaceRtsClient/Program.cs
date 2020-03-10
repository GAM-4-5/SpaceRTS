using System;
using System.Threading;
using SpaceRtsClient.Util;

namespace SpaceRtsClient.Desktop
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread serverThread = new Thread(() => Coms.Initilize());
            serverThread.Start();


            using (var game = new Game())
                game.Run();
        }
    }
}
