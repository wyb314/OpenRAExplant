﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentsAI.Factories;
using Engine.ComponentsAI.GOAP.Core;

namespace Engine.ComponentsAI.AStarMachine
{
    public abstract class Agent
    {
        public string name;

        public WorldState WorldState;

        private GOAPManager m_GoalManager;
        private Hashtable m_Actions = new Hashtable();

        public GOAPAction GetAction(E_GOAPAction type) { return (GOAPAction)m_Actions[type]; }
        public int GetNumberOfActions() { return m_Actions.Count; }

        public GOAPGoal CurrentGOAPGoal { get { return m_GoalManager.CurrentGoal; } }

        public void AddGOAPAction(E_GOAPAction action)
        {
            m_Actions.Add(action, GOAPActionFactory.Create(action, this));
        }

        public void AddGOAPGoal(E_GOAPGoals goal)
        {
            m_GoalManager.AddGoal(goal);
        }

        public void InitializeGOAP()
        {
            m_GoalManager.Initialize();
        }

    }
}