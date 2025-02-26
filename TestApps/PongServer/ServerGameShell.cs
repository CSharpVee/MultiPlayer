using PongServer.DTOs;
using PongServer.DTOs.Duplicates;
using PongServer.Game;

namespace PongServer
{

    internal class ServerGameShell : IGameCore
    {
        public bool IsPrecount => _loop.GameState == GameStates.Precount;//TERRIBLE. Not-scalable. non-understandable. works for the purpose. couldn't be bothered.

        public int PointsL => _loop.PointsL;
        public int PointsR => _loop.PointsR;
        public int ChargeL => _loop.ChargeL;
        public int ChargeR => _loop.ChargeR;
        public bool LCharging => _loop.LCharging;
        public bool RCharging => _loop.RCharging;
        public RectangleF LeftPaddleBounds => _loop.LeftPaddleBounds;
        public RectangleF RightPaddleBounds => _loop.RightPaddleBounds;
        public RectangleF BallBounds => _loop.BallBounds;
        public float? StartupTimeLeft => _loop.StartupTimeLeft;
        public PosDir Chargeup => _charge;
        public PosDir Powershot => _powershot;
        public PosDir Goal => _goal;
        public IEnumerable<Vector2> BouncesHappenned => _bounces;


        private GameLoop _loop;

        private Player _playerStateL;
        private Player _playerStateR;

        //event storage
        private List<Vector2> _bounces;
        private PosDir _goal;
        private PosDir _charge;
        private PosDir _powershot;


        public ServerGameShell(RectangleF gameArea)
        {
            _bounces = new List<Vector2>();

            _loop = new GameLoop();
            _loop.Initialize(gameArea);
            _loop.OnBounce += (Vector2 pos) => _bounces.Add(pos);
            _loop.OnGoal += (Vector2 pos, int dir) => _goal = new PosDir(pos, (short)dir);
            _loop.ChargeAdd += (Vector2 pos, int dir) => _charge = new PosDir(pos, (short)dir);
            _loop.PowerShotFired += (Vector2 pos, int dir) => _powershot = new PosDir(pos, (short)dir);

            _loop.Start();
        }

        public void ClearEventStorage()
        {
            _charge = _goal = _powershot = null;
            _bounces.Clear();
        }

        public CountdownState ExtractPrecount()
        {
            var countdown = new CountdownState()
            {
                PointsL = (short)_loop.PointsL,
                PointsR = (short)_loop.PointsR,
                Countdown = _loop.StartupTimeLeft.HasValue ? _loop.StartupTimeLeft.Value : -3,
                BallBounds = _loop.BallBounds,
            };

            return countdown;
        }

        public GameState ExtractGameplay()
        {
            var gamestate = new GameState()
            {
                PointsL = (short)_loop.PointsL,
                PointsR = (short)_loop.PointsR,
                ChargeL = (short)_loop.ChargeL,
                ChargeR = (short)_loop.ChargeR,

                IsChargingL = _loop.LCharging,
                IsChargingR = _loop.RCharging,

                BallBounds = _loop.BallBounds,
                LeftPaddleBounds = _loop.LeftPaddleBounds,
                RightPaddleBounds = _loop.RightPaddleBounds,

                Chargeup = _charge,
                Goal = _goal,
                Powershot = _powershot,
                BouncesHappenned = _bounces.ToArray()
            };

            ClearEventStorage();

            return gamestate;
        }

        public void Update(double dTime)
        {
            //saved input or default
            _loop.Update((float)dTime, _playerStateL.LastInput, _playerStateR.LastInput);
        }

        internal void SetInput(Player playerL, Player playerR)
        {
            _playerStateL = playerL;
            _playerStateR = playerR;
        }
    }
}
