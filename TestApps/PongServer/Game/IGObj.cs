using PongServer.DTOs.Duplicates;

namespace PongServer.Game
{
    public abstract class AGObj
    {
        public Vector2 Position { get; set; }
        public Size2D Size { get; set; }


        public Vector2 Center => Position + Size.AsVector2() / 2;
        public RectangleF Bounds => new RectangleF(Position, Size);
        public bool Contains(Vector2 point) => Bounds.Contains(point);
    }
}
