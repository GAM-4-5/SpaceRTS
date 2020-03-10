using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Lidgren.Network;

namespace SpaceRts.Channels
{
    public static class Channel
    {
        public static void SetSubChannel(NetOutgoingMessage message, int subChannel)
        {
            message.WriteRangedInteger(0, 64, subChannel);
            message.WritePadBits();
        }

        public static int NUMBER_OF_CHANNELS = 3;
        public static int CHANNEL_SIZE = 32;
        public static int TOTAL_CHANNELS = CHANNEL_SIZE * NUMBER_OF_CHANNELS;

        public enum Channels
        {
            Auth,
            Lobby,
            Game,
        }

        public enum AuthChannel
        {
            Id,
            UUID,
        }

        public enum LobbyChannel
        {
            Create, 
            Join,
            Invite,
            Kick,
            Chat,
            Users,
            SendChat,
        }

        public enum GameChannel
        {

            SpaceGenerationSeed,

            PlaceStructure,
            RemoveStructure,
            AssignUnit,
            ConfirmResourceCount,
            ConfirmBuild,
        }

        public enum StructureList
        {
            Base,
            Mine,
            Smeltery,
            Conveyor,
            Turret,
            Lab,
            Frack,
            Raffinery,
            Farm,
            Housing,
            RecharginStation,
            Baracks,
            LaunchPad,
        }
    }

}
