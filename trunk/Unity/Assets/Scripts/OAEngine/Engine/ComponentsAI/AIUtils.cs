using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueSync;

namespace Engine.ComponentsAI
{
    public static class AIUtils
    {
        public static int TickFacing(int facing, int desiredFacing, int rot)
        {
            var leftTurn = (facing - desiredFacing) & 0xFF;
            var rightTurn = (desiredFacing - facing) & 0xFF;
            if (Math.Min(leftTurn, rightTurn) < rot)
                return desiredFacing & 0xFF;
            else if (rightTurn < leftTurn)
                return (facing + rot) & 0xFF;
            else
                return (facing - rot) & 0xFF;
        }


        public static int RoundFacing(int facing)
        {
            facing = facing % 256;

            if (facing < 0)
            {
                facing += 256;
            }
            return facing;
        }

        public static FP RoundFacing(FP facing)
        {
            facing = facing % 256;

            if (facing < 0)
            {
                facing += 256;
            }
            return facing;
        }

        public static int AngleBetween(int facing0 , int facing1)
        {
            facing0 = RoundFacing(facing0);
            facing1 = RoundFacing(facing1);

            if (facing0 > facing1 && facing0 - facing1 > 128)
            {
                return facing1 + 256 - facing0;
            }

            if (facing1 > facing0 && facing1 - facing0 > 128)
            {
                return facing0 + 256 - facing1;
            }

            return MathUtils.Abs(facing0 - facing1);
        }
    }
}
