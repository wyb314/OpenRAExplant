﻿using System;
using System.Collections.Generic;

namespace Engine.ComponentsAI.AStarMachine
{
    abstract class AStarGoal
    {
        public abstract void SetDestNode(AStarNode destNode);
        public abstract float GetHeuristicDistance(Agent ai, AStarNode pAStarNode, bool firstRun);
        public abstract float GetActualCost(AStarNode nodeOne, AStarNode nodeTwo);
        public abstract bool IsAStarFinished(AStarNode currNode);
        public abstract bool IsAStarNodePassable(int node);
        public abstract void Cleanup();
    }
}