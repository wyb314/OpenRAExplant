using System;
using System.Collections.Generic;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;
using Engine.Primitives;
using OAEngine.Engine.ComponentsAI;
using TrueSync;

namespace Engine.ComponentAnim._AniStates
{
    public class AnimStateIdle : AnimState
    {
        int TimeToFinishWeaponAction;
        AgentAction WeaponAction;

        public AnimStateIdle(Animation anims, Agent owner)
            : base(anims, owner)
        {

        }

        override public void Release()
        {
            //if (m_Human.PlayerProperty != null)
            //Debug.Log(Time.timeSinceLevelLoad + " " + this.ToString() + " - release");

            SetFinished(true);
        }

        override public void OnActivate(AgentAction action)
        {
            base.OnActivate(action);

        }

        override public void OnDeactivate()
        {

            base.OnDeactivate();
        }


        override public bool HandleNewAction(AgentAction action)
        {
            //if (action is AgentActionWeaponShow)
            //{
            //    if ((action as AgentActionWeaponShow).Show == true)
            //    {
            //        //swhow weapon anim
            //        string s = Owner.AnimSet.GetShowWeaponAnim(Owner.BlackBoard.WeaponSelected);
            //        TimeToFinishWeaponAction = Time.timeSinceLevelLoad + AnimEngine[s].length * 0.8f;
            //        AnimEngine.CrossFade(s, 100);
            //        //                Owner.ShowWeapon(true, 0.1f);
            //    }
            //    else
            //    {
            //        //hide weapon anim
            //        string s = Owner.AnimSet.GetHideWeaponAnim(Owner.BlackBoard.WeaponSelected);
            //        TimeToFinishWeaponAction = Time.timeSinceLevelLoad + (AnimEngine[s].length * 0.9f);
            //        AnimEngine.CrossFade(s, 100);
            //        //              Owner.ShowWeapon(false, 2.3f);
            //    }
            //    WeaponAction = action;
            //    return true;
            //}
            return false;
        }

        override public void Update()
        {
            //if (WeaponAction != null && TimeToFinishWeaponAction < Time.timeSinceLevelLoad)
            //{
            //    WeaponAction.SetSuccess();
            //    WeaponAction = null;
            //    CrossFade(Owner.AnimSet.GetIdleAnim(Owner.BlackBoard.WeaponSelected, Owner.BlackBoard.WeaponState), 0.2f);
            //}
        }

        void PlayIdleAnim()
        {
            string s = Owner.AnimSet.GetIdleAnim(Owner.BlackBoard.WeaponSelected, Owner.BlackBoard.WeaponState);
            CrossFade(s,0.2f);
        }

        protected override void Initialize(AgentAction action)
        {
            base.Initialize(action);

            Owner.BlackBoard.MotionType = E_MotionType.None;
            Owner.BlackBoard.MoveDir = TSVector2.up;
            Owner.BlackBoard.Speed = 0;

            if (WeaponAction == null)
                PlayIdleAnim();
        }
    }
}
