namespace PongServer.DTOs.Duplicates
{
    public struct RectangleF
    {

        public float X;
        public float Y;
        public float Width;
        public float Height;

        public Vector2 Position => new Vector2(X, Y);
        public Size2D Size => new Size2D(Width, Height);
        public Vector2 Center => new Vector2(X, Y) + Size.AsVector2() / 2;

        public float Top => Y;
        public float Bottom => Y + Height;
        public float Left => X;
        public float Right => X + Width;

        public RectangleF(float inX, float inY, float inWidth, float inHeight)
        {
            X = inX;
            Y = inY;
            Width = inWidth;
            Height = inHeight;
        }

        //public RectangleF(Rectangle r)
        //{
        //    X = r.X;
        //    Y = r.Y;
        //    Width = r.Width;
        //    Height = r.Height;
        //}

        public RectangleF(Vector2 pos, Size2D size)
        {
            X = pos.X;
            Y = pos.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public bool Contains(Vector2 pt)
        {
            return Contains(pt.X, pt.Y);
        }

        public bool Contains(float X, float Y)
        {
            return X >= Left && X < Right && Y >= Top && Y < Bottom;
        }

        //public Rectangle AsRectangle()
        //{
        //    return new Rectangle((int)MathF.Round(X), (int)MathF.Round(Y), (int)MathF.Round(Width), (int)MathF.Round(Height));
        //}

    }
}
