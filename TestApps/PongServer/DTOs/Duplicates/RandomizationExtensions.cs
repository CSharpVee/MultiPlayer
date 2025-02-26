namespace PongServer.DTOs.Duplicates
{
    public static class RandomizationExtensions
    {
        public static float NextFloat(this Random rand)
        {
            return (float)rand.NextDouble();
        }

        public static float NextFloat(this Random rand, float max)
        {
            return (float)rand.NextDouble() * max;
        }

        public static float RandomF(this Random rand, double min, double max)
        {
            return HelpRand.RandRangeF(rand, min, max);
        }

        public static double RandomD(this Random rand, double min, double max)
        {
            return HelpRand.RandRangeD(rand, min, max);
        }

        public static int RandomI(this Random rand, int min, int max)
        {
            return HelpRand.RandRangeI(rand, min, max);
        }

        public static Size2D RandomSize(this Random rand, double min, double max, bool WHSame = false)
        {
            var width = rand.RandRangeF(min, max);
            var height = WHSame ? width : rand.RandRangeF(min, max);

            return new Size2D(width, height);
        }

        public static Vector2 RandomV2(this Random rand, double min, double max, bool XYSame = false)
        {
            var X = rand.RandRangeF(min, max);
            var Y = XYSame ? X : rand.RandRangeF(min, max);

            return new Vector2(X, Y);
        }

        //public static Vector3 RandomV3(this Random rand, int min, int max, bool XYZSame = false)
        //{
        //    var X = rand.RandRangeF(min, max);
        //    var Y = XYZSame ? X : rand.RandRangeF(min, max);
        //    var Z = XYZSame ? X : rand.RandRangeF(min, max);

        //    return new Vector3(X, Y, Z);
        //}

        //public static Color RandomColor(this Random rand)
        //{
        //    return new Color(rand.Next(256), rand.Next(256), rand.Next(256));
        //}

        public static T RandomItem<T>(this Random rand, T[] array)
        {
            return array[rand.Next(array.Length)];
        }
    }
}
