using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TrueSync;

namespace Engine
{
    public sealed class Time
    {
        public static FP deltaTime { private set;get; }

        private static int timestep;
        public static int Timestep
        {
            set
            {
                deltaTime = new FP(value) / new FP(1000);
                timestep = value;
            }
            get { return timestep; }
        }

        private static FP mTime;
        public static FP time
        {
            set { mTime = value; }
            get { return mTime; }
        }

        //public static FP timeSinceLevelLoad { get; }

        public static void Reset()
        {
            timestep = 0;
            deltaTime = 0;
            mTime = 0;
        }
    }
}
