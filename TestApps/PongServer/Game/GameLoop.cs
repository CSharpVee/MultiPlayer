using PongServer.DTOs;
using PongServer.DTOs.Duplicates;

namespace PongServer.Game
{
    public enum GameStates
    {
        Paused = 0,
        Precount,
        NormalPlay,
        Powershot,
        Goal
    }

    public delegate void PositionalEvent(Vector2 pos);
    public delegate void PosDirEvent(Vector2 pos, int dir);

    public class GameLoop
    {
        public int PointsL = 0;
        public int PointsR = 0;

        public int ChargeL = 0;
        public int ChargeR = 0;

        public bool LCharging { get; private set; }
        public bool RCharging { get; private set; }


        public RectangleF BallBounds => _ball.Bounds;
        public RectangleF LeftPaddleBounds => _paddleL.Bounds;
        public RectangleF RightPaddleBounds => _paddleR.Bounds;

        public float? StartupTimeLeft => _playState == GameStates.Precount? KPrecountTime - _elapse : null;


        public event PositionalEvent OnBounce;
        public event PosDirEvent ChargeAdd;
        public event PosDirEvent OnGoal;
        public event PosDirEvent PowerShotFired;

        public GameStates GameState => _playState;

        //Constants
        //Timeouts
        private const float KPrecountTime = 2.5f;
        private const float KGoalTime = 2.5f;

        //charge things
        private float KChargeHitArea = 0.25f;//the amount of area to hit to get a charge
        private int KChargeCost = 3;
        private float KPSTravelTime = 1.5f;
        private float KPSMaxAmplitude = 280f;
        private float KPSMinFreq = 2;
        private float KPSMaxFreq = 5;



        private const int KPaddleSpeedY = 1000;
        private const int KPaddleGap = 20;
        private const int KBallSize = 45;
        private const float KVelocityIncrease = 1.1f;
        private readonly Size2D KPaddleSize = new Size2D(30, 140);


        private GameStates _playState;
        private float _elapse;

        private RectangleF _gameArea;
        private Random _rnd = new Random();
        private Ball _ball;
        private Paddle _paddleL;
        private Paddle _paddleR;

        public GameLoop()
        {
            
        }

        public void Initialize(RectangleF bounds)
        {
            _playState = GameStates.Paused;
            _gameArea = bounds;

            _ball = new Ball()
            {
                Size = new Size2D(KBallSize)
            };

            _paddleL = new Paddle()
            {
                Position = new Vector2(KPaddleGap, _gameArea.Height / 2),
                Size = KPaddleSize
            };
            _paddleR = new Paddle()
            {
                Position = new Vector2(_gameArea.Width - KPaddleGap - KPaddleSize.Width, _gameArea.Height / 2),
                Size = KPaddleSize
            };
        }

        public void Start()
        {
            RandomizeStart();
            _playState = GameStates.Precount;
            _elapse = 0;
        }

        public void Update(float dTime, PlayerInputDTO playerL, PlayerInputDTO playerR)
        {
            _elapse += dTime;

            switch (_playState)
            {
                case GameStates.Paused: break;
                case GameStates.Precount:
                    if (_elapse > KPrecountTime)
                        _playState = GameStates.NormalPlay;
                    break;
                case GameStates.NormalPlay:
                case GameStates.Powershot:
                    _ball.Update(dTime);
                    ProcessNormalPlay(dTime, playerL, playerR);
                    break;

                case GameStates.Goal:
                    _ball.Update(dTime);
                    UpdateInput(dTime, playerL.Move, playerR.Move);
                    if (_elapse > KGoalTime)
                        Start();
                    break;
            }
        }

        private void ProcessNormalPlay(float dTime, PlayerInputDTO playerL, PlayerInputDTO playerR)
        {
            UpdateInput(dTime, playerL.Move, playerR.Move);
            var ctr = _ball.Center;

            LCharging = ChargeL >= KChargeCost && playerL.UseSupershot;
            RCharging = ChargeR >= KChargeCost && playerR.UseSupershot;


            if (_ball.Position.Y < _gameArea.Top)
            {
                _ball.SetBouncePosition(new Vector2(_ball.Position.X, _gameArea.Top));
                SwitchVelocity(new Vector2(1, -1), false);
            }

            if (_ball.Position.Y + _ball.Size.Height > _gameArea.Bottom)
            {
                _ball.SetBouncePosition(new Vector2(_ball.Position.X, _gameArea.Bottom - _ball.Size.Height));
                SwitchVelocity(new Vector2(1, -1), false);
            }

            //bounces
            if (_paddleL.Contains(ctr))
            {
                _ball.Position = new Vector2(_paddleL.Bounds.Right, _ball.Position.Y);
                SwitchVelocity(new Vector2(-1, 1), true);
                if (ChargeL >= KChargeCost && playerL.UseSupershot)
                {
                    ChargeL = 0;
                    StartPowershot(1);
                }
                else _ball.StartNormalMode();
                if (ChargeCalc(_paddleL, _ball.Center))
                {
                    ChargeL++;
                    ChargeAdd.Invoke(_ball.Center, -1);
                }
            }

            if (_paddleR.Contains(ctr))
            {
                _ball.Position = new Vector2(_paddleR.Bounds.Left - _ball.Size.Width, _ball.Position.Y);
                SwitchVelocity(new Vector2(-1, 1), true);
                if (ChargeR >= KChargeCost && playerR.UseSupershot)
                {
                    ChargeR = 0;
                    StartPowershot(-1);
                }
                else _ball.StartNormalMode();
                if (ChargeCalc(_paddleR, _ball.Center))
                {
                    ChargeR++;
                    ChargeAdd.Invoke(_ball.Center, 1);
                }
            }

            //scoring
            if (_ball.Position.X < _gameArea.Left)
                Score(-1);

            if (_ball.Position.X + _ball.Size.Width > _gameArea.Right)
                Score(1);
        }

        private void UpdateInput(float dTime, int leftMove, int rightMove)
        {
            //move paddles

            _paddleL.Position += new Vector2(0, KPaddleSpeedY * leftMove) * dTime;
            _paddleR.Position += new Vector2(0, KPaddleSpeedY * rightMove) * dTime;
        }

        private void SwitchVelocity(Vector2 xy, bool speedUp)
        {
            var mult = speedUp ? KVelocityIncrease : 1;
            _ball.Velocity = _ball.Velocity * xy * mult;

            if (!speedUp)
                OnBounce?.Invoke(_ball.Center);
        }

        private bool ChargeCalc(Paddle paddle, Vector2 ballCtrPos)
        {
            var thresh = _paddleL.Size.Height * KChargeHitArea;
            var diff1 = ballCtrPos.Y - _paddleL.Position.Y;
            var diff2 = (_paddleL.Position.Y + _paddleL.Size.Height) - ballCtrPos.Y;

            return diff1 >= 0 && diff1 < thresh || diff2 >= 0 && diff2 < thresh;
            //return diff1 < thresh || diff2 >= 0;//the ↑ other bounds are already accounted for in the Contains calcuclation and might not register on the very edge, when it should.
        }

        private void Score(int side)
        {
            if (side < 0)
                PointsL++;
            else PointsR++;

            _elapse = 0;
            _playState = GameStates.Goal;

            OnGoal?.Invoke(_ball.Center, side);
            LCharging = RCharging = false;
        }

        private void StartPowershot(int dir)
        {
            var prms = new PowershotParams()
            {
                TravelTime = KPSTravelTime,
                TravelDistance = _gameArea.Width * 1.05f,
                Phasing = _rnd.Next() % 2 == 0 ? 1 : -1,
                Frequency = _rnd.RandRangeF(KPSMinFreq, KPSMaxFreq),
                Amplitude = _rnd.RandRangeF(KPSMaxAmplitude * 0.8f, KPSMaxAmplitude),
                XDirecton = dir
            };

            _ball.StartPowershot(prms);
            PowerShotFired.Invoke(_ball.Center, dir);
        }

        private void RandomizeStart()
        {
            _ball.StartNormalMode();
            _ball.Position = _gameArea.Center + _rnd.RandomV2(-200, 200);

            var velocity = 450 + _rnd.RandomF(0, 200);
            var angl = _rnd.RandomF(-0.87, 0.87);
            var dir = (_rnd.Next() % 2 == 0 ? 1 : -1);
            _ball.Velocity = new Vector2(MathF.Cos(angl) * dir, MathF.Sin(angl)) * velocity;
        }
    }
}
