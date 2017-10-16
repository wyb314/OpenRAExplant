using System;
using System.Collections.Generic;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;
using OAEngine.Engine.ComponentsAI;
using TrueSync;

namespace Engine.ComponentAnim._AniStates
{
    public class AnimStateRoll : AnimState
    {
        AgentActionRoll Action;

        FP FinalRotation;
        FP StartRotation;
        TSVector2 StartPosition;
        TSVector2 FinalPosition;
        FP CurrentRotationTime;
        FP RotationTime;
        FP CurrentMoveTime;
        FP MoveTime;
        FP EndOfStateTime;
        FP BlockEndTime;

        bool RotationOk = false;
        bool PositionOK = false;

        //ParticleEmitter Effect;


        public AnimStateRoll(Animation anims, Agent owner)
            : base(anims, owner)
        {
            //Effect = owner.Transform.Find("sust").GetComponent<ParticleEmitter>();
        }


        override public void OnActivate(AgentAction action)
        {
            base.OnActivate(action);



            //     Time.timeScale = .7f;
        }

        override public void OnDeactivate()
        {
            //      Time.timeScale = 1;

            Action.SetSuccess();
            Action = null;
            base.OnDeactivate();
        }

        override public void Update()
        {
            if (RotationOk == false)
            {
                CurrentRotationTime += Game.DeltaTime;

                if (CurrentRotationTime >= RotationTime)
                {
                    CurrentRotationTime = RotationTime;
                    RotationOk = true;
                }

                FP progress = CurrentRotationTime / RotationTime;
                FP curFacing = AIUtils.RoundFacing(StartRotation);
                FP targetFacing = AIUtils.RoundFacing(FinalRotation);
                if (curFacing > targetFacing && curFacing - targetFacing > 180)
                {
                    curFacing -= 360;
                }

                if (curFacing < targetFacing && targetFacing - curFacing > 180)
                {
                    targetFacing -= 360;
                }
                FP facing = TSMath.Lerp(curFacing, targetFacing, progress);
                Owner.Facing = facing;
                
            }

            if (PositionOK == false)// && (RotationOk || (Quaternion.Angle(Owner.Transform.rotation, FinalRotation) > 40.0f))
            {
                CurrentMoveTime += Game.DeltaTime;
                if (CurrentMoveTime >= MoveTime)
                {
                    CurrentMoveTime = MoveTime;
                    PositionOK = true;
                }

                FP progress = CurrentMoveTime / MoveTime;
                TSVector2 finalPos = MathUtils.Hermite(StartPosition, FinalPosition, progress);
                //MoveTo(finalPos);
                //if (Move(finalPos - Transform.position) == false)
                //    PositionOK = true;

                //TODO: 之后有必要考虑物理计算
                this.Owner.Position = finalPos;
                //PositionOK = true;
            }

            if (EndOfStateTime <= Game.WorldTime)
                Release();
        }

        override public bool HandleNewAction(AgentAction action)
        {
            if (action is AgentActionRoll)
            {
                if (Action != null)
                    Action.SetSuccess();


                Initialize(action);

                return true;
            }
            return false;
        }


        protected override void Initialize(AgentAction action)
        {
            base.Initialize(action);

            Action = action as AgentActionRoll;

            CurrentMoveTime = 0;
            CurrentRotationTime = 0;

            StartRotation = this.Owner.Facing;
            StartPosition = this.Owner.Position;

            TSVector2 finalDir;
            if (Action.ToTarget != null)
            {
                finalDir = Action.ToTarget.Position - this.Owner.Position;
                finalDir.Normalize();

                FinalPosition = Action.ToTarget.Position - finalDir * Owner.BlackBoard.WeaponRange;
            }
            else
            {
                finalDir = Action.Direction;
                FinalPosition = StartPosition + Action.Direction * Owner.BlackBoard.RollDistance;
            }

            string AnimName = Owner.AnimSet.GetRollAnim(Owner.BlackBoard.WeaponSelected, Owner.BlackBoard.WeaponState);
            CrossFade(AnimName, 0.1f);

            FinalRotation = MathUtils.TSVector2ToFacing(finalDir, 360);
           

            RotationTime = TSVector2.Angle(this.Owner.Forward, finalDir) / 1000.0f;
            MoveTime = this.AnimEngine.GetAnimLength(AnimName) * 0.85f;
            EndOfStateTime = this.AnimEngine.GetAnimLength(AnimName) * 0.9f + Game.WorldTime;

            RotationOk = RotationTime == 0;
            PositionOK = false;

            Owner.BlackBoard.MotionType = E_MotionType.Roll;

            //if (Effect)
            //    CombatEffectsManager.Instance.StartCoroutine(CombatEffectsManager.Instance.PlayAndStop(Effect, 0.1f));
        }
    }
}
