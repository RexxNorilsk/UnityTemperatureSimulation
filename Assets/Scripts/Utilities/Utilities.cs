
using UnityEngine;

namespace TemperatureSimulator
{
    public static class Utilities
    {
        public static float Functions(int x, int y, int z, float valueInPos, int funcId, float H)
        {
            switch (funcId)
            {
                case 0:
                    return 200;
                case 1:
                    return 0;
                case 2:
                    return 200 * (x * H);
                case 3:
                    return 200 * (y * H);
                case 4:
                    return 200 * (z * H);
                case 5:
                    return 200 * x * y;
            }
            return 0;
        }

        
    }
}