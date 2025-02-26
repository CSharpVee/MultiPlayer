using MPClientBase.DTO;
using PongServer.DTOs.Duplicates;
using PongServer.Utilities;

namespace PongServer.DTOs
{
    public class GameState : IDataPackTyped<GameState>
    {
        public short PointsL;
        public short PointsR;
        public short ChargeL;
        public short ChargeR;

        public bool IsChargingL;
        public bool IsChargingR;

        public RectangleF BallBounds;
        public RectangleF LeftPaddleBounds;
        public RectangleF RightPaddleBounds;

        //
        public PosDir Chargeup;
        public PosDir Powershot;
        public PosDir Goal;
        public Vector2[] BouncesHappenned;

        public byte[] GetBytes()
        {
            var byteSeq = new List<byte[]>();
            byteSeq.Add(BitConverter.GetBytes(PointsL));
            byteSeq.Add(BitConverter.GetBytes(PointsR));
            byteSeq.Add(BitConverter.GetBytes(ChargeL));
            byteSeq.Add(BitConverter.GetBytes(ChargeR));

            byteSeq.Add(new byte[] { (byte)((IsChargingL ? 1 : 0) << 1 | (IsChargingR ? 1 : 0)) });

            byteSeq.Add(ByteHelper.GetRectFBytes(BallBounds));
            byteSeq.Add(ByteHelper.GetRectFBytes(LeftPaddleBounds));
            byteSeq.Add(ByteHelper.GetRectFBytes(RightPaddleBounds));


            byteSeq.Add(ByteHelper.GetPosDirBytes(Chargeup));
            byteSeq.Add(ByteHelper.GetPosDirBytes(Powershot));
            byteSeq.Add(ByteHelper.GetPosDirBytes(Goal));

            byteSeq.Add(new byte[] { (byte)BouncesHappenned.Length });
            foreach (var v in BouncesHappenned)
                byteSeq.Add(ByteHelper.GetVectorBytes(v));

            return ByteHelper.ConcatByteList(byteSeq);
        }

        public GameState ParseIn(byte[] data)
        {

            int ofst = 0;
            PointsL = BitConverter.ToInt16(data, 0);
            PointsR = BitConverter.ToInt16(data, 2);
            ChargeL = BitConverter.ToInt16(data, 4);
            ChargeR = BitConverter.ToInt16(data, 6);
            IsChargingL = (data[8] >> 1 & 1) != 0;
            IsChargingR = (data[8] & 1) != 0;

            BallBounds = ByteHelper.ConstructRectF(data, 9);
            LeftPaddleBounds = ByteHelper.ConstructRectF(data, 17);
            RightPaddleBounds = ByteHelper.ConstructRectF(data, 25);

            Chargeup = ByteHelper.ConstructPosdir(data, 33);
            if (Chargeup != null)
                ofst += 4;

            Powershot = ByteHelper.ConstructPosdir(data, 34 + ofst);
            if (Powershot != null)
                ofst += 4;

            Goal = ByteHelper.ConstructPosdir(data, 35 + ofst);
            if (Goal != null)
                ofst += 4;

            var bounces = data[36 + ofst];
            var bounceLst = new List<Vector2>();
            for (int i = 0; i < bounces; i++)
            {
                var bnc = ByteHelper.ConstructVector(data, 37 + ofst + i * 4);
                bounceLst.Add(bnc);
            }
            BouncesHappenned = bounceLst.ToArray();

            return this;
        }
    }
}
