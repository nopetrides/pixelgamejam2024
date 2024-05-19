using System;

namespace Multiplayer
{
    public static class GameConstants
    {
        public enum GameStateData
        {
            Score = 0,
            MapSeed = 1,
        }

        public enum PlayerStateData
        {
            Health = 0,
            Attack = 1,
            MoveSpeed = 2,
            Icon = 3,
            CarryingCapacity = 4,
            CharacterType = 5,
            Position = 6,
            IsCarrying = 7,
        }

        public enum CharacterTypes
        {
            Alpha = 0,
            Beta = 1,
            Gamma = 2,
            Delta = 3
        }

        [Flags]
        public enum DragonStats
        {
            Heat = 1, 
            Temper = 2, 
            Energy = 4,
            Chewing = 8, 
        }
    }
    public static class Extensions
    {

        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length==j) ? Arr[0] : Arr[j];            
        }
        
        public static T Previous<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) - 1;
            return (j<0) ? Arr[^1] : Arr[j];
        }
    }
}