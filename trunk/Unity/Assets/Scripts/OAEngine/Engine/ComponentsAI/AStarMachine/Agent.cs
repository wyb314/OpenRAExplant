using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI.Factories;
using Engine.ComponentsAI.GOAP.Core;
using Engine.ComponentsAI.WorkingMemory;
using Engine.Interfaces;
using Engine.Primitives;
using Engine.Support;
using OAEngine.Engine.ComponentsAI;
using TrueSyncPhysics;
using TrueSync;

namespace Engine.ComponentsAI.AStarMachine
{
    public abstract class Agent
    {
        public string name;

        public WorldState WorldState;

        protected Memory Memory;

        protected GOAPManager m_GoalManager;

        public BlackBoard BlackBoard;

        public AnimSet AnimSet;

        private Hashtable m_Actions = new Hashtable();
        
        private Dictionary<Type,IAgentComponent> components = new Dictionary<Type, IAgentComponent>(); 

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

        public void AddComponent(IAgentComponent component)
        {
            if (component == null)
            {
                return;
            }
            Type type = component.GetType();
            IAgentComponent _component;

            if (!this.components.TryGetValue(type,out _component))
            {
                component.SetAgent(this);
                this.components.Add(type, component);
            }
            else
            {
                throw new ArgumentException("Can't add the same type component multi times to a agent!");
            }
        }

        public T GetComponent<T>() where T : class,IAgentComponent
        {
            IAgentComponent result = null;

            foreach (var component in this.components)
            {
                if (component.Value is T)
                {
                    result = component.Value;

                    break;
                }
            }

            //this.components.TryGetValue(typeof (T), out result);

            return result as T;
        }

        public IAgentComponent GetComponent(Type type)
        {
            IAgentComponent result = null;
            foreach (var component in this.components)
            {
                if (component.Value.GetType() == type)
                {
                    result = component.Value;

                    break;
                }
            }
            //this.components.TryGetValue(type, out result);

            return result;
        }

        public abstract World world { get; }

        public abstract TSVector2 CurJoystickDir { get; }

        //public abstract WPos Position { set;get; }
        //public abstract FP Facing { set;get; }

        public abstract TSVector2 Position { set; get; }
        
        public abstract FP Facing { set; get; }

        public abstract TSVector2 Forward { get; }

        public abstract FP TurnSpeed { set;get; }
        //public abstract WVec Right { get; }

        public abstract MersenneTwister Random { get; }

        public abstract E_ComboLevel[] ComboLevel { get; }

        public abstract E_SwordLevel SwordLevel { get; }

        public abstract IRegidbodyWrapObject RendererObject { get; }
    }
}
