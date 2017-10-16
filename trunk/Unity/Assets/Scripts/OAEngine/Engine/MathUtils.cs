using System;
using System.Collections.Generic;
using Engine.ComponentsAI;
using TrueSync;

namespace Engine
{
    public static class MathUtils
    {
        public const float Deg2Rad = 0.01745329f;

        public const float Rad2Deg = 57.29578f;

        public const float PI = 3.141593f;

        public const float Half_PI = PI*0.5f;

        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2((double)y, (double)x);
        }

        public static int Abs(int v)
        {
            return Math.Abs(v);
        }

        /// <summary>
        /// 
        /// <para>
        /// Returns the smallest integer greater to or equal to f.
        /// </para>
        /// 
        /// </summary>
        /// <param name="f"/>
        public static float Ceil(float f)
        {
            return (float)Math.Ceiling((double)f);
        }

        /// <summary>
        /// 
        /// <para>
        /// Returns the largest integer smaller to or equal to f.
        /// </para>
        /// 
        /// </summary>
        /// <param name="f"/>
        public static float Floor(float f)
        {
            return (float)Math.Floor((double)f);
        }

        /// <summary>
        /// 
        /// <para>
        /// Returns f rounded to the nearest integer.
        /// </para>
        /// 
        /// </summary>
        /// <param name="f"/>
        public static float Round(float f)
        {
            return (float)Math.Round((double)f);
        }

        /// <summary>
        /// 
        /// <para>
        /// Returns the smallest integer greater to or equal to f.
        /// </para>
        /// 
        /// </summary>
        /// <param name="f"/>
        public static int CeilToInt(float f)
        {
            return (int)Math.Ceiling((double)f);
        }

        /// <summary>
        /// 
        /// <para>
        /// Returns the largest integer smaller to or equal to f.
        /// </para>
        /// 
        /// </summary>
        /// <param name="f"/>
        public static int FloorToInt(float f)
        {
            return (int)Math.Floor((double)f);
        }

        /// <summary>
        /// 
        /// <para>
        /// Returns f rounded to the nearest integer.
        /// </para>
        /// 
        /// </summary>
        /// <param name="f"/>
        public static int RoundToInt(float f)
        {
            return (int)Math.Round((double)f);
        }

        public static float Max(float a, float b)
        {
            return (double)a <= (double)b ? b : a;
        }
        
        public static float Min(float a, float b)
        {
            return (double)a >= (double)b ? b : a;
        }

        public static FP Min(FP a, FP b)
        {
            return (double)a >= (double)b ? b : a;
        }

        public static int Max(int a, int b)
        {
            return a <= b ? b : a;
        }
        public static int Min(int a, int b)
        {
            return a >= b ? b : a;
        }

        public static float Sqrt(float f)
        {
            return (float) Math.Sqrt(f);
        }


        public static FP DeltaAngle(FP from ,FP to)
        {
            from = RoundFacing(from);
            to = RoundFacing(to);
            if (from > to && from - to > 180)
            {
                from -= 360;
            }

            if (from < to && to - from > 180)
            {
                to -= 360;
            }

            return TSMath.Abs(from - to);
        }

        public static FP RoundFacing(FP facing)
        {
            facing = facing % 360;

            if (facing < 0)
            {
                facing += 360;
            }
            return facing;
        }


        public static TSVector2 FacingToTSVector2(FP facing)
        {
            facing = RoundFacing(facing);
            FP rad = facing*TSMath.Deg2Rad;
            FP cos = TSMath.Cos(rad);
            FP sin = TSMath.Sin(rad);

            return new TSVector2(cos, sin);
        }

        public static FP TSVector2ToFacing(TSVector2 dir, FP TwoPi2Period)
        {
            FP facing = TSMath.Atan2(dir.y, dir.x);
            facing = facing * TwoPi2Period * 0.5f / TSMath.Pi;

            if (dir.x >= 0)
            {
                if (dir.y >= 0) // in quadrant 1
                {

                }
                else// in quadrant 4
                {
                    facing = TwoPi2Period + facing;
                }
            }
            else
            {
                if (dir.y >= 0)// in quadrant 2
                {
                }
                else // in quadrant 3
                {
                    facing = TwoPi2Period + facing;
                }
            }

            return facing;
        }


        public static FP Hermite(FP start, FP end, FP value)
        {
            return TSMath.Lerp(start, end, value * value * (3.0f - 2.0f * value));
        }

        public static TSVector2 Hermite(TSVector2 start, TSVector2 end, FP value)
        {
            return new TSVector2(Hermite(start.x, end.x, value), Hermite(start.y, end.y, value));
        }
        

    }

}
