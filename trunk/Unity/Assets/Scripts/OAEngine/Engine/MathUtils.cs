using System;
using System.Collections.Generic;

namespace Engine
{
    public static class MathUtils
    {
        public const float Deg2Rad = 0.01745329f;

        public const float Rad2Deg = 57.29578f;

        public const float PI = 3.141593f;

        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2((double)y, (double)x);
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

        

    }

}
