
namespace PongServer.DTOs.Duplicates
{
    public enum EaseType
    {
        /// <summary>
        /// /
        /// </summary>
        LinearIn,
        /// <summary>
        /// \
        /// </summary>
        LinearOut,

        SquareIn,
        SquareOut,
        SquareInvIn,
        SquareInvOut,
        CubicIn,
        CubicOut,
        CubicInvIn,
        CubicInvOut,
        QuarticIn,
        QuarticOut,
        QuarticInvIn,
        QuarticInvOut,

        LogarithmicIn,
        LogarithmicOut,

        ExponentialIn,
        ExponentialOut,

        SFormIn,
        SFormOut,
        SFormCubicIn,
        SFormCubicOut,
        SFormQuintIn,
        SFormQuintOut,

        ExponentialInOut,
        /// <summary>
        /// ???totalbullshit test easing
        /// </summary>
        MandrasInOutMinusZero
    };

    public class HelpEasing
    {
        /// <summary>
        /// Allows dampening (approaching 0) a value, while also limiting the dampening rate by time (as opposed to a direct [value = value * dampRatio], which happens faster the higher the FPS is)
        /// </summary>
        /// <param name="value">Value to dampen</param>
        /// <param name="dampeningRatio">Dampen rate per second</param>
        /// <param name="dTime">Time delta</param>
        /// <returns></returns>
        public static float DampenValuePerSecond(float startingValue, float dampeningRatio, float elapsedTime)
        {
            return (float)(startingValue * Math.Pow(dampeningRatio, elapsedTime));
        }

        public static Vector2 DampenValuePerSecond(Vector2 startingValue, float dampeningRatio, float elapsedTime)
        {
            var dampenMultiplier = (float)Math.Pow(dampeningRatio, elapsedTime);
            return startingValue * dampenMultiplier;
        }

        public static Vector2 EaseValue(Vector2 from, Vector2 to, float progress, EaseType easeType)
        {
            var x = EaseValue(from.X, to.X, progress, easeType);
            var y = EaseValue(from.Y, to.Y, progress, easeType);
            return new Vector2(x, y);
        }

        public static float EaseValue(float from, float to, float progress, EaseType easeType)
        {
            switch (easeType)
            {
                case EaseType.LinearIn:
                    return LinearEase(from, to, progress);
                case EaseType.LinearOut:
                    return LinearEase(from, to, 1 - progress);


                case EaseType.SquareIn:
                    return PowerLevelEase(from, to, progress, 2);
                case EaseType.SquareOut:
                    return PowerLevelEase(from, to, 1 - progress, 2);
                case EaseType.SquareInvIn:
                    return PowerLevelEase(from, to, progress, 0.5f);
                case EaseType.SquareInvOut:
                    return PowerLevelEase(from, to, 1 - progress, 0.5f);

                case EaseType.CubicIn:
                    return PowerLevelEase(from, to, progress, 3);
                case EaseType.CubicOut:
                    return PowerLevelEase(from, to, 1 - progress, 3);
                case EaseType.CubicInvIn:
                    return PowerLevelEase(from, to, progress, 1 / 3.0f);
                case EaseType.CubicInvOut:
                    return PowerLevelEase(from, to, 1 - progress, 1 / 3.0f);

                case EaseType.QuarticIn:
                    return PowerLevelEase(from, to, progress, 4);
                case EaseType.QuarticOut:
                    return PowerLevelEase(from, to, 1 - progress, 4);
                case EaseType.QuarticInvIn:
                    return PowerLevelEase(from, to, progress, 0.25f);
                case EaseType.QuarticInvOut:
                    return PowerLevelEase(from, to, 1 - progress, 0.25f);

                case EaseType.LogarithmicIn:
                    return LogarithmicEase(from, to, progress);
                case EaseType.LogarithmicOut:
                    return LogarithmicEase(from, to, 1 - progress);


                case EaseType.ExponentialIn:
                    return ExponentialEase(from, to, progress);
                case EaseType.ExponentialOut:
                    return ExponentialEase(from, to, 1 - progress);


                case EaseType.SFormIn:
                    return SFormEase(from, to, progress);
                case EaseType.SFormOut:
                    return SFormEase(from, to, 1 - progress);

                case EaseType.SFormCubicIn:
                    return SFormCubicEase(from, to, progress);
                case EaseType.SFormCubicOut:
                    return SFormCubicEase(from, to, 1 - progress);

                case EaseType.SFormQuintIn:
                    return SFormQuintEase(from, to, progress);
                case EaseType.SFormQuintOut:
                    return SFormQuintEase(from, to, 1 - progress);


                case EaseType.ExponentialInOut:
                    return ExponentialIOEase(from, to, progress);
                case EaseType.MandrasInOutMinusZero:
                    return MandrasInOutEase(from, to, progress);

                default:
                    throw new ArgumentException();
            }
        }

        private static float MandrasInOutEase(float from, float to, float progress)
        {
            //so-so
            var x = progress * 11.01;
            return from + (to - from) * (float)(
                -Math.Sin(x / 10 + 1.3) + Math.Cos(x / 2 + 6.2)
                ) / -19.333f;

            //var x = progress * 3;
            //return from + (to - from) * (float)(
            //    -Math.Pow(Math.Sin(x),4)*2
            //    );
        }

        private static float ExponentialIOEase(float from, float to, float progress)
        {
            var x = progress * 2.453 + 0.064;
            return from + (to - from) * (float)((Math.Pow(x, 2) / Math.Exp(x * 3) * 15 - 0.05) / 0.852);
        }

        private static float LogarithmicEase(float from, float to, float progress)
        {
            return from + (to - from) * (float)(Math.Log(progress + 1) / Math.Log(2));
        }

        private static float ExponentialEase(float from, float to, float progress)
        {
            return from + (to - from) * (float)((Math.Exp(progress) - 1) / (Math.Exp(1) - 1));
        }

        private static float PowerLevelEase(float from, float to, float progress, float power)
        {
            return from + (to - from) * (float)Math.Pow(progress, power);
        }

        private static float LinearEase(float from, float to, float progress)
        {
            return from + (to - from) * progress;
        }

        private static float SFormEase(float from, float to, float progress)
        {
            var part1 = (float)Math.Pow(progress, 2);
            var part2 = (float)Math.Pow(progress, 1 / 5.0);

            var diff = to - from;
            var combined = part1 * (1 - progress) + part2 * progress;

            return from + combined * diff;
        }


        private static float SFormCubicEase(float from, float to, float progress)
        {
            var val = progress < 0.5 ?
                4 * progress * progress * progress
                : 1 - MathF.Pow(-2 * progress + 2, 3) / 2;

            return from + val * (to - from);
        }

        private static float SFormQuintEase(float from, float to, float progress)
        {
            var val = progress < 0.5 ?
                16 * progress * progress * progress * progress * progress
                : 1 - MathF.Pow(-2 * progress + 2, 5) / 2;

            return from + val * (to - from);
        }

    }
}
