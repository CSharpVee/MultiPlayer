using PongServer.DTOs;

namespace PongServer
{
    public interface IGameCore
    {
        int PointsL { get; }
        int PointsR { get; }
        int ChargeL { get; }
        int ChargeR { get; }
        bool LCharging { get; }
        bool RCharging { get; }
        PongServer.DTOs.Duplicates.RectangleF LeftPaddleBounds { get; }
        PongServer.DTOs.Duplicates.RectangleF RightPaddleBounds { get; }
        PongServer.DTOs.Duplicates.RectangleF BallBounds { get; }
        float? StartupTimeLeft { get; }

        PosDir Chargeup { get; }
        PosDir Powershot { get; }
        PosDir Goal { get; }
        IEnumerable<PongServer.DTOs.Duplicates.Vector2> BouncesHappenned { get; }

        void ClearEventStorage();
        void Update(double dTime);
    }
}
