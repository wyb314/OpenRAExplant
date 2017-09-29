using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;

namespace Engine.ComponentAnim.Core
{
    public abstract class AnimState
    {
        protected Animation AnimEngine;
        private bool m_Finished = true;
        protected Agent Owner;

        public AnimState(Animation anims, Agent owner)
        {
            AnimEngine = anims;
            Owner = owner;
        }


        virtual public void OnActivate(AgentAction action) // state is being activated
        {
            SetFinished(false);

            Initialize(action);
        }

        virtual public void OnDeactivate() //..............deactivated
        {
        }

        virtual public void Release() { SetFinished(true); } // finish currrent action and then finish state

        virtual public bool HandleNewAction(AgentAction action) { return false; } // new action is comming..

        virtual public void Update() { } // update current state

        virtual public bool IsFinished() { return m_Finished; }

        public virtual void SetFinished(bool finished) { m_Finished = finished; } // state is finished or not



        //

        virtual protected void Initialize(AgentAction action)
        {
            //if (Owner.debugAnims) Debug.Log(Time.timeSinceLevelLoad + " " + this.ToString() + " Initialize " + " by " + (action != null ? action.ToString() : "nothing"));
        }


        protected void CrossFade(string anim, float fadeInTime)
        {
            //if (Owner.debugAnims) Debug.Log(Time.timeSinceLevelLoad + " " + this.ToString() + " cross fade anim: " + anim + " in " + fadeInTime + "s.");

            if (AnimEngine.IsPlaying(anim))
            {
                AnimEngine.CrossFadeQueued(anim, fadeInTime, QueueMode.PlayNow);
            }
            else
            {
                AnimEngine.CrossFade(anim, fadeInTime);
            }
                
        }

        //protected bool Move(Vector3 velocity, bool slide = true)
        //{
        //    Vector3 old = Transform.position;

        //    Transform.position += Vector3.up * Time.deltaTime;

        //    velocity.y -= 9 * Time.deltaTime;
        //    CollisionFlags flags = Owner.CharacterController.Move(velocity);

        //    //Debug.Log("move " + flags.ToString());

        //    if (slide == false && (flags & CollisionFlags.Sides) != 0)
        //    {
        //        Transform.position = old;
        //        return false;
        //    }

        //    if ((flags & CollisionFlags.Below) == 0)
        //    {
        //        Transform.position = old;
        //        return false;
        //    }

        //    return true;
        //}

        //protected bool MoveEx(Vector3 velocity)
        //{
        //    Vector3 old = Transform.position;

        //    Transform.position += Vector3.up * Time.deltaTime;

        //    velocity.y -= 9 * Time.deltaTime;
        //    CollisionFlags flags = Owner.CharacterController.Move(velocity);

        //    if (flags == CollisionFlags.None)
        //    {
        //        RaycastHit hit;
        //        if (Physics.Raycast(Transform.position, -Vector3.up, out hit, 3, 1 << 10) == false)
        //        {
        //            Transform.position = old;
        //            return false;
        //        }
        //    }

        //    return true;
        //}
    }
}
