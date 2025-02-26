namespace PongServer.DTOs.Duplicates
{
    public struct Size2D
    {
        public float Width;
        public float Height;

        public int WidthI => (int)MathF.Round(Width);
        public int HeightI => (int)MathF.Round(Height);

        public static Size2D Zero => new Size2D(0);

        public Size2D(float inDimensions)
        {
            Width = inDimensions;
            Height = inDimensions;
        }

        public Size2D(float inWidth, float inHeight)
        {
            Width = inWidth;
            Height = inHeight;
        }

        public Size2D(Vector2 inSize)
        {
            Width = inSize.X;
            Height = inSize.Y;
        }

        //public Size2D(Point inSize)
        //{
        //    Width = inSize.X;
        //    Height = inSize.Y;
        //}

        //public Size2D(Rectangle rect)
        //{
        //    Width = rect.Width;
        //    Height = rect.Height;
        //}

        public static Size2D operator +(Size2D size, Size2D addition)
        {
            return new Size2D(size.Width + addition.Width, size.Height + addition.Height);
        }

        public static Size2D operator -(Size2D size, Size2D addition)
        {
            return new Size2D(size.Width - addition.Width, size.Height - addition.Height);
        }

        public static Size2D operator *(Size2D size, Size2D multiplier)
        {
            return new Size2D(size.Width * multiplier.Width, size.Height * multiplier.Height);
        }

        public static Size2D operator *(Size2D size, float multiplier)
        {
            return new Size2D(size.Width * multiplier, size.Height * multiplier);
        }

        public static Size2D operator /(Size2D size, float multiplier)
        {
            return new Size2D(size.Width / multiplier, size.Height / multiplier);
        }

        public static Size2D operator *(Size2D size, Vector2 multiplier)
        {
            return new Size2D(size.Width * multiplier.X, size.Height * multiplier.Y);
        }

        public static Vector2 operator *(Vector2 pos, Size2D multiplier)
        {
            return new Vector2(pos.X * multiplier.Width, pos.Y * multiplier.Height);
        }

        public Vector2 AsVector2()
        {
            return new Vector2(Width, Height);
        }

        public override string ToString()
        {
            return $"W : {Width}; H : {Height}";
        }

        //public static Size2D FromGD(GraphicsDevice gd)
        //{
        //    return new Size2D(gd.Viewport.Width, gd.Viewport.Height);
        //}

        //public static Size2D FromTex(Texture2D texture)
        //{
        //    return new Size2D(texture.Width, texture.Height);
        //}

        //public static Size2D TexDimensionRatio(Texture2D texture)
        //{
        //    var ratio = texture.Height / (float)texture.Width;
        //    return new Size2D(1, ratio);
        //}

        //internal static Size2D FromRect(Rectangle rect)
        //{
        //    return new Size2D(rect.Width, rect.Height);
        //}

        public static Size2D Max(Size2D val1, Size2D val2)
        {
            var maxWidth = val1.Width > val2.Width ? val1.Width : val2.Width;
            var maxHeight = val1.Height > val2.Height ? val1.Height : val2.Height;

            return new Size2D(maxWidth, maxHeight);
        }

        public static Size2D Min(Size2D val1, Size2D val2)
        {
            var minWidth = val1.Width < val2.Width ? val1.Width : val2.Width;
            var minHeight = val1.Height < val2.Height ? val1.Height : val2.Height;

            return new Size2D(minWidth, minHeight);
        }
    }
}
