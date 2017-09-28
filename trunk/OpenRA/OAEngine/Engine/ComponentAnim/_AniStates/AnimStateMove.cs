using System;
using System.Collections.Generic;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;
using Engine.Primitives;
using OAEngine.Engine.ComponentsAI;

namespace Engine.ComponentAnim._AniStates
{
    public class AnimStateMove : AnimState
    {
        AgentActionMove Action;
        float MaxSpeed;

        private int FinalRotation;
        private int StartRotation;
        float RotationProgress;


        public AnimStateMove(Animation anims, Agent owner)
            : base(anims, owner)
        {
        }


        override public void OnActivate(AgentAction action)
        {
            // Time.timeScale = 0.1f;
            base.OnActivate(action);
            //PlayAnim(GetMotionType());
        }

        override public void OnDeactivate()
        {
            if (Action != null)
            {
                Action.SetSuccess();
                Action = null;
            }

            Owner.BlackBoard.Speed = 0;

            base.OnDeactivate();

            // Time.timeScale = 1;
        }

        override public void Update()
        {
            //Debug.DrawLine(OwnerTransform.position + new Vector3(0, 1, 0), Action.FinalPosition + new Vector3(0, 1, 0));

            //if (Owner.debugAnims) Debug.Log(Time.timeSinceLevelLoad + " " + "Speed " + Owner.BlackBoard.Speed + " Max Speed " + Owner.BlackBoard.MaxWalkSpeed);
            if (Action.IsActive() == false)
            {
                Release();
                return;
            }

            RotationProgress += Owner.BlackBoard.RotationSmooth * Game.Timestep;
            RotationProgress = MathUtils.Min(RotationProgress, 1000);

            this.Owner.Facing = AIUtils.TickFacing(this.Owner.Facing, FinalRotation, this.Owner.TurnSpeed);
            

            //if (MathUtils.Abs(FinalRotation - this.Owner.Facing) > 29)
            //    return;

            MaxSpeed = MathUtils.Max(Owner.BlackBoard.MaxWalkSpeed, (int)((Owner.BlackBoard.MaxRunSpeed * Owner.BlackBoard.MoveSpeedModifier)/100));

            // Smooth the speed based on the current target direction
            int curSmooth = (int)Owner.BlackBoard.SpeedSmooth * Game.Timestep / 100;

            Owner.BlackBoard.Speed = Owner.BlackBoard.Speed +(int)(MaxSpeed - Owner.BlackBoard.Speed) * curSmooth /1000;
            Owner.BlackBoard.MoveDir = Owner.BlackBoard.DesiredDirection;

            var dir = new WVec(0, -1024, 0).Rotate(WRot.FromFacing(this.Owner.Facing));
            WVec v = Owner.BlackBoard.Speed * dir * Game.Timestep / (1024 * 1024); 

            this.Owner.Position = this.Owner.Position + v;
            //// MOVE
            //if (Move(Owner.BlackBoard.MoveDir * Owner.BlackBoard.Speed * Time.deltaTime) == false)
            //    Release();

            //E_MotionType motion = GetMotionType();
            //if (motion != Owner.BlackBoard.MotionType)
            //    PlayAnim(motion);

        }
        


        override public bool HandleNewAction(AgentAction action)
        {
            if (action is AgentActionMove)
            {
                if (Action != null)
                    Action.SetSuccess();

                SetFinished(false); // just for sure, if we already finish in same tick

                Initialize(action);

                return true;
            }

            if (action is AgentActionIdle)
            {
                action.SetSuccess();

                SetFinished(true);
            }

            if (action is AgentActionWeaponShow)
            {
                action.SetSuccess();

                //            Owner.ShowWeapon((action as AgentActionWeaponShow).Show, 0);

                PlayAnim(GetMotionType());
                return true;
            }
            return false;
        }

        private void PlayAnim(E_MotionType motion)
        {
            Owner.BlackBoard.MotionType = motion;

            CrossFade(Owner.AnimSet.GetMoveAnim(Owner.BlackBoard.MotionType, E_MoveType.Forward, Owner.BlackBoard.WeaponSelected, Owner.BlackBoard.WeaponState), 0.2f);
        }

        private E_MotionType GetMotionType()
        {
            if (Owner.BlackBoard.Speed > Owner.BlackBoard.MaxRunSpeed * 1.5f)
                return E_MotionType.Sprint;
            else if (Owner.BlackBoard.Speed > Owner.BlackBoard.MaxWalkSpeed * 1.5f)
                return E_MotionType.Run;

            return E_MotionType.Walk;
        }

        protected override void Initialize(AgentAction action)
        {
            base.Initialize(action);

            Action = action as AgentActionMove;

            FinalRotation = Owner.BlackBoard.DesiredDirection;

            StartRotation = Owner.Facing;

            Owner.BlackBoard.MotionType = GetMotionType();

            RotationProgress = 0;
        }
    }
}
