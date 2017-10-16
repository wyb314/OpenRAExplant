using System;
using System.Collections.Generic;
using TrueSync;

namespace Engine.ComponentsAI.AStarMachine
{
    abstract class AStarGoal
    {
        public abstract void SetDestNode(AStarNode destNode);
        public abstract FP GetHeuristicDistance(Agent ai, AStarNode pAStarNode, bool firstRun);
        public abstract FP GetActualCost(AStarNode nodeOne, AStarNode nodeTwo);
        public abstract bool IsAStarFinished(AStarNode currNode);
        public abstract bool IsAStarNodePassable(int node);
        public abstract void Cleanup();
    }
}
