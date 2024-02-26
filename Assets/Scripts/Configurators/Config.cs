
using UnityEngine;

namespace TemperatureSimulator
{
    public class Config
    {
        public float MaxTemperature;
        public Texture2D XTarget;
        public Texture2D YTarget;
        public Texture2D ZTarget;
        public int[] TargetSlice;
        public int[] Border—onditions;
        public int Size;
        public float Alfa;
        public float Tau;
        public float H;
        public float TimeMax;
        public Config()
        {
            TargetSlice = new int[3];
            for (int i = 0; i < TargetSlice.Length; i++)
            {
                TargetSlice[i] = 1;
            }
            Border—onditions = new int[6];
        }
    }
}
