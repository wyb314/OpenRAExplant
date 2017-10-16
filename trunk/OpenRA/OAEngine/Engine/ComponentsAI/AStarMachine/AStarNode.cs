using System;
using System.Collections.Generic;
using TrueSync;

namespace Engine.ComponentsAI.AStarMachine
{
    class AStarNode
    {
        public enum E_AStarFlags
        {
            Unchecked = 0,
            Open = 1,
            Closed = 2,
            NotPassable = 3,
        }

        public AStarNode()
        {
            NodeID = -1;
            G = 0;
            H = 0;
            F = FP.MaxValue;

            Flag = E_AStarFlags.Unchecked;
        }

        public short NodeID;
        public FP G;
        public FP H;
        public FP F;
        public AStarNode Next;
        public AStarNode Previous;
        public AStarNode Parent;
        public E_AStarFlags Flag;
    }
}
