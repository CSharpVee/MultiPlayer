namespace PongServer.DTOs.Duplicates
{
    public struct Vector2
    {
        public float X;
        public float Y;

        public Vector2() : this(0,0)
        {
        }

        public Vector2(float inX, float inY)
        {
            X = inX;
            Y = inY;
        }


        public static Vector2 operator +(Vector2 one, Vector2 addition)
        {
            return new Vector2(one.X + addition.X, one.Y + addition.Y);
        }

        public static Vector2 operator *(Vector2 one, Vector2 two)
        {
            return new Vector2(one.X * two.X, one.Y * two.Y);
        }

        public static Vector2 operator *(Vector2 one, float multiplier)
        {
            return new Vector2(one.X * multiplier, one.Y * multiplier);
        }

        public static Vector2 operator /(Vector2 one, float divisor)
        {
            return new Vector2(one.X / divisor, one.Y / divisor);
        }
    }
}
