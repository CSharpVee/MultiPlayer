using PongServer.DTOs;
using PongServer.DTOs.Duplicates;

namespace PongServer.Utilities
{
    internal class ByteHelper
    {
        //serialization
        public static byte[] ConcatBytes(params byte[][] bytes)
        {
            return ConcatByteList(bytes);
        }

        public static byte[] ConcatByteList(IEnumerable<byte[]> bytes)
        {
            var count = 0;
            foreach (var item in bytes)
                count += item.Length;

            var buffer = new byte[count];
            var start = 0;
            foreach (var arr in bytes)
            {
                for (var i = 0; i < arr.Length; i++)
                    buffer[start + i] = arr[i];
                start += arr.Length;
            }
            return buffer;
        }

        public static byte[] GetRectFBytes(RectangleF rf)
        {
            var btsX = BitConverter.GetBytes((Half)rf.X);
            var btsY = BitConverter.GetBytes((Half)rf.Y);
            var btsWidth = BitConverter.GetBytes((Half)rf.Width);
            var btsHeight = BitConverter.GetBytes((Half)rf.Height);

            var bts = new byte[8] { btsX[0], btsX[1], btsY[0], btsY[1], btsWidth[0], btsWidth[1], btsHeight[0], btsHeight[1] };

            var reconstituted = ConstructRectF(bts);
            return bts;
        }

        public static byte[] GetVectorBytes(Vector2 v)
        {
            var btsX = BitConverter.GetBytes((Half)v.X);
            var btsY = BitConverter.GetBytes((Half)v.Y);

            var bts = new byte[4] { btsX[0], btsX[1], btsY[0], btsY[1] };

            var reconstituted = ConstructVector(bts);
            return bts;
        }

        public static byte[] GetPosDirBytes(PosDir posdir)
        {
            if (posdir == null || posdir.Dir == 0)
                return new byte[] { 0 };

            var dir = posdir.Dir;
            var btsX = BitConverter.GetBytes((Half)posdir.Position.X);
            var btsY = BitConverter.GetBytes((Half)posdir.Position.Y);

            return new byte[5] { DirToByte(dir), btsX[0], btsX[1], btsY[0], btsY[1] };
        }


        //deserialization

        public static RectangleF ConstructRectF(byte[] data)
        {
            return ConstructRectF(data, 0);
        }

        public static RectangleF ConstructRectF(byte[] data, int startIndex)
        {
            var x = BitConverter.ToHalf(data, startIndex + 0);
            var y = BitConverter.ToHalf(data, startIndex + 2);
            var width = BitConverter.ToHalf(data, startIndex + 4);
            var height = BitConverter.ToHalf(data, startIndex + 6);

            return new RectangleF((float)x, (float)y, (float)width, (float)height);
        }


        public static Vector2 ConstructVector(byte[] data)
        {
            return ConstructVector(data, 0);
        }

        public static Vector2 ConstructVector(byte[] data, int startIndex)
        {
            var x = BitConverter.ToHalf(data, startIndex + 0);
            var y = BitConverter.ToHalf(data, startIndex + 2);

            return new Vector2((float)x, (float)y);
        }


        public static PosDir ConstructPosdir(byte[] data, int startIndex)
        {
            var dir = ByteToDir(data[startIndex + 0]);
            if (dir == 0)
                return null;

            var x = BitConverter.ToHalf(data, startIndex + 1);
            var y = BitConverter.ToHalf(data, startIndex + 3);

            return new PosDir() { Dir = dir, Position = new Vector2((float)x, (float)y) };
        }


        private static byte DirToByte(short dir)
        {
            if (dir < 0)
                return 1;
            if (dir > 0)
                return 2;
            return 0;
        }

        private static short ByteToDir(byte dir)
        {
            if (dir == 1)
                return -1;
            if (dir == 2)
                return 1;
            return 0;
        }
    }
}
