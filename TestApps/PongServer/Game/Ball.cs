using PongServer.DTOs.Duplicates;

namespace PongServer.Game
{
    public enum MovementMode
    {
        Normal,
        Powershot
    }

    internal class Ball : AGObj
    {
        public Vector2 Velocity { get; set; }
        public MovementMode Movement { get; private set; }

        private float _elapse;
        private PowershotParams _pshotParams;

        public Ball()
        {
        }

        public void Update(float dTime)
        {
            _elapse += dTime;
            if (Movement == MovementMode.Normal)
                Position += Velocity * dTime;
            else
            {
                var ratio = _elapse / _pshotParams.TravelTime;
                var positionY = MathF.Sin(ratio * _pshotParams.Frequency * 2 * MathF.PI * _pshotParams.Phasing) * _pshotParams.Amplitude * HelpEasing.EaseValue(0, 1, ratio, EaseType.CubicInvIn); ;
                var positionX = _pshotParams.TravelDistance * _pshotParams.XDirecton * ratio;

                Position = _pshotParams.StartPoint + new Vector2(positionX, positionY);
            }
        }

        public void StartPowershot(PowershotParams inPrms)
        {
            Movement = MovementMode.Powershot;
            _elapse = 0;

            _pshotParams = inPrms;
            _pshotParams.StartPoint = Position;
        }

        public void StartNormalMode()
        {
            Movement = MovementMode.Normal;
        }

        public void SetBouncePosition(Vector2 newPos)
        {
            if (Movement == MovementMode.Normal)
                Position = newPos;
            else
            {
                var posDiff = Position.Y - newPos.Y;
                Position = new Vector2(Position.X, newPos.Y - posDiff);
            }
        }
    }

    public class PowershotParams
    {
        public Vector2 StartPoint { get; set; }
        public float TravelTime { get; set; }
        public float TravelDistance { get; set; }
        public float Phasing { get; set; }
        public float Frequency { get; set; }
        public float Amplitude { get; set; }
        public float XDirecton { get; set; }
    }
}
