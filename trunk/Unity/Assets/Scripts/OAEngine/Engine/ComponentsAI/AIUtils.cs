using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
