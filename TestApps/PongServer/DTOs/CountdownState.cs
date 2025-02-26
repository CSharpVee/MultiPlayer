using MPClientBase.DTO;
using PongServer.DTOs.Duplicates;
using PongServer.Utilities;

namespace PongServer.DTOs
{
    public class CountdownState : IDataPackTyped<CountdownState>
    {
        public short PointsL;
        public short PointsR;
        public RectangleF BallBounds;
        public float Countdown;

        public byte[] GetBytes()
        {
            var ptsLBytes = BitConverter.GetBytes(PointsL);
            var ptsRBytes = BitConverter.GetBytes(PointsR);
            var ballBoundsBts = ByteHelper.GetRectFBytes(BallBounds);
            var countdownBts = BitConverter.GetBytes((Half)Countdown);

            return ByteHelper.ConcatBytes(ptsLBytes, ptsRBytes, ballBoundsBts, countdownBts);
        }

        public CountdownState ParseIn(byte[] data)
        {
            PointsL = BitConverter.ToInt16(data, 0);
            PointsR = BitConverter.ToInt16(data, 2);
            BallBounds = ByteHelper.ConstructRectF(data, 4);
            Countdown = (float)BitConverter.ToHalf(data, 12);
            return this;
        }
    }
}
