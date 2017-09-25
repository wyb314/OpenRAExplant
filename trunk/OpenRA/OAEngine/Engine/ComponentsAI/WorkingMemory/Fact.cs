using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.GOAP.Core;
using Engine.Primitives;

namespace Engine.ComponentsAI.WorkingMemory
{
    public class Fact
    {
        public enum E_FactType
        {
            E_FACT_INVALID = -1,
            E_EVENT,
            E_ENEMY,
            E_COUNT
        }

        public enum E_DataTypes
        {
            E_EVENT,
            E_POS,
            E_DIR,
            E_AGENT,
            E_COUNT
        }


        public static int LiveTime = 200;

        private BitArray m_DataTypesSet = new BitArray((int)E_DataTypes.E_COUNT);

        public Agent Causer;
        public E_FactType FactType;// { get { return FactType; } private set { FactType = value; } }
                                   //private float m_TimeCreated;
        public int Belief;

        private E_EventTypes _Event;
        public E_EventTypes EventType { get { return _Event; } set { _Event = value; m_DataTypesSet.Set((int)E_DataTypes.E_EVENT, true); } }
        private WPos _Pos;
        public WPos Position { get { return _Pos; } set { _Pos = value; m_DataTypesSet.Set((int)E_DataTypes.E_POS, true); } }
        private WVec _Dir;
        public WVec Direction { get { return _Dir; } set { _Dir = value; m_DataTypesSet.Set((int)E_DataTypes.E_DIR, true); } }
        private Agent _Agent;
        public Agent Agent { get { return _Agent; } set { _Agent = value; m_DataTypesSet.Set((int)E_DataTypes.E_AGENT, true); } }
        public bool Deleted;


        static private int m_TotalNumberOfFacts;

        public Fact(E_FactType type) { FactType = type; m_TotalNumberOfFacts++; }

        public void Reset(E_FactType newType)
        {
            FactType = newType;
            //m_TimeCreated = Time.timeSinceLevelLoad;
            Belief = 0;
            Position = WPos.Zero;
            Direction = WVec.Zero;
            Agent = null;
            Deleted = false;
            m_DataTypesSet.SetAll(false);
        }


        public bool MatchesQuery(Fact other)
        {
            if (Deleted) return false;

            if (other.m_DataTypesSet.Get((int)E_DataTypes.E_EVENT) == true)
                if (EventType != other.EventType)
                    return false;

            return true;
        }

        public void DecreaseBelief()
        {
            if (Belief <= 0.0f)
                return;
            Belief -= (1000 / LiveTime) * Game.Timestep;
            Belief = MathUtils.Max(0, Belief);
        }

        public override string ToString()
        {
            string s = base.ToString() + " : " + FactType.ToString();

            if (m_DataTypesSet.Get((int)E_DataTypes.E_EVENT) == true)
                s += " " + EventType.ToString();

            return s;
        }
    }
}
