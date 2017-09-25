using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.GOAP.Core;

namespace Engine.ComponentsAI.Factories
{
    public enum E_GOAPGoals
    {
        E_INVALID = -1,
        E_ORDER_ATTACK,
        E_ORDER_DODGE,
        E_ORDER_USE,
        E_GOTO,
        E_COMBAT_MOVE_LEFT,
        E_COMBAT_MOVE_RIGHT,
        E_COMBAT_MOVE_FORWARD,
        E_COMBAT_MOVE_BACKWARD,
        E_LOOK_AT_TARGET,
        E_KILL_TARGET,
        E_DODGE,
        E_DO_BLOCK,
        E_ALERT,
        E_CALM,
        E_USE_WORLD_OBJECT,
        E_PLAY_ANIM,
        E_IDLE_ANIM,
        E_REACT_TO_DAMAGE,
        E_BOSS_ATTACK,
        E_TELEPORT,
        E_COUNT,
    }

    class GOAPGoalFactory
    {
        public static GOAPGoal Create(E_GOAPGoals type, Agent owner)
        {
            GOAPGoal g = null;
            switch (type)
            {

                default:
                    break;
            }

            return g;
        }
    }
}
