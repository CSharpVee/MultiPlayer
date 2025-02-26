using System;

namespace PongServer.DTOs.Duplicates
{
    public static class HelpRand
    {
        //RANDOM RANGES
        public static float RandRangeF(this Random iR, double iMin, double iMax)
        {
            double iRange = iMax - iMin;
            return (float)(iR.NextDouble() * iRange + iMin);
        }

        public static double RandRangeD(this Random iR, double iMin, double iMax)
        {
            double iRange = iMax - iMin;
            return iR.NextDouble() * iRange + iMin;
        }

        public static int RandRangeI(this Random iR, int iMin, int iMax)
        {
            int iRange = iMax - iMin;
            return iR.Next() % (iRange + 1) + iMin;
        }
    }
}
