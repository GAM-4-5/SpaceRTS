using System;
namespace SpaceRts.Util
{
    public class GameOptions
    {

        public NumberOfSolarSystems NumberOfSolarSystems;
        public NumberOfPlantes NumberOfPlantes;
        public GameSpeed GameSpeed;

        public GameOptions(NumberOfSolarSystems numberOfSolarSystems, NumberOfPlantes numberOfPlantes, GameSpeed gameSpeed)
        {
            NumberOfSolarSystems = numberOfSolarSystems;
            NumberOfPlantes = numberOfPlantes;
            GameSpeed = gameSpeed;
        }
    }

    public enum NumberOfSolarSystems
    {
        Low,
        Normal,
        High,
    }

    public enum NumberOfPlantes
    {
        Low,
        Normal,
        High,
    }

    public enum GameSpeed
    {
        Slow,
        Normal,
        Fast,
    }
}
