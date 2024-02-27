
using UnityEngine;

namespace TemperatureSimulator
{
    public static class Utilities
    {

        public static float Functions(int x, int y, int z, float valueInPos, int funcId, float H, float time)
        {
            switch (funcId)
            {
                case 0:
                    return 200;
                case 1:
                    return 0;
                case 2:
                    return 20 * (x * H);
                case 3:
                    return 20 * (y * H);
                case 4:
                    return 20 * (z * H);
                case 5:
                    return 20 * x * y;
                case 6:
                    return 5 * time;
            }
            return 0;
        }

        
    }
}