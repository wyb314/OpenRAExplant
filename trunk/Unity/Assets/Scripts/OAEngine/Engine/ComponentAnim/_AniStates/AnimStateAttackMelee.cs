using System;
using System.Collections.Generic;
using System.Diagnostics;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;
using Engine.Support;
using OAEngine.Engine.ComponentsAI;
using TrueSync;

namespace Engine.ComponentAnim._AniStates
{
    public class AnimStateAttackMelee : AnimState
    {
        enum E_State
        {
            E_PREPARING,
            E_ATTACKING,
            E_FINISHED,
        }


        AgentActionAttack Action;
        AnimAttackData AnimAttackData;

        FP FinalRotation;
        FP StartRotation;
        TSVector2 StartPosition;
        TSVector2 FinalPosition;
        FP CurrentRotationTime;
        FP RotationTime;
        FP MoveTime;
        FP CurrentMoveTime;
        FP EndOfStateTime;
        FP HitTime;
        FP AttackPhaseTime;

        bool RotationOk = false;
        bool PositionOK = false;
        // bool MovingToAttackPos;

        bool Critical = false;
        bool Knockdown = false;

        E_State State;

        public AnimStateAttackMelee(Animation anims, Agent owner)
            : base(anims, owner)
        {

        }

        override public void OnActivate(AgentAction action)
        {
            /*if(Owner.IsPlayer == false)
                Time.timeScale = 0.2f;*/

            base.OnActivate(action);

        }

        override public void OnDeactivate()
        {
            Action.SetSuccess();
            Action = null;

            base.OnDeactivate();
        }


        override public bool HandleNewAction(AgentAction action)
        {
            if (action as AgentActionAttack != null)
            {
                if (Action != null)
                {
                    Action.AttackPhaseDone = true;
                    Action.SetSuccess();
                }

                Initialize(action);
                return true;
            }
            return false;
        }

        override public void Update()
        {
            FP deltaTime = Game.Timestep / 1000f;

            if (State == E_State.E_PREPARING)
            {
                bool dontMove = false;
                if (RotationOk == false)
                {
                    //Debug.Log("rotate");
                    CurrentRotationTime += deltaTime;

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
                    

                    if (MathUtils.DeltaAngle(facing, FinalRotation) > 20.0f)
                        dontMove = true;
                }

                if (dontMove == false && PositionOK == false)
                {
                    CurrentMoveTime += deltaTime;
                    if (CurrentMoveTime >= MoveTime)
                    {
                        CurrentMoveTime = MoveTime;
                        PositionOK = true;
                    }

                    if (CurrentMoveTime > 0)
                    {
                        FP progress = CurrentMoveTime / MoveTime;
                        TSVector2 finalPos = MathUtils.Hermite(StartPosition, FinalPosition, progress);
                        //if (MoveToCollideWithEnemy(finalPos, Transform.forward) == false)
                        //if (Move(finalPos - this.Owner.Position) == false)
                        //{
                        //    PositionOK = true;
                        //}

                        this.Owner.Position = finalPos;

                        //PositionOK = true;

                    }
                }

                if (RotationOk && PositionOK)
                {
                    State = E_State.E_ATTACKING;
                    PlayAnim();
                }
            }
            else if (State == E_State.E_ATTACKING)
            {
                CurrentMoveTime += deltaTime;

                if (AttackPhaseTime < Game.Time)
                {
                    //Debug.Log(Time.timeSinceLevelLoad + " attack phase done");
                    Action.AttackPhaseDone = true;
                    State = E_State.E_FINISHED;
                }

                if (CurrentMoveTime >= MoveTime)
                    CurrentMoveTime = MoveTime;

                if (CurrentMoveTime > 0 && CurrentMoveTime <= MoveTime)
                {
                    FP progress = TSMath.Min(1.0f, CurrentMoveTime / MoveTime);
                    TSVector2 finalPos = MathUtils.Hermite(StartPosition, FinalPosition, progress);
                    //if (MoveToCollideWithEnemy(finalPos, Transform.forward) == false)
                    //if (Move(finalPos - Transform.position, false) == false)
                    //{
                    //    CurrentMoveTime = MoveTime;
                    //}

                    this.Owner.Position = finalPos;
                }

                if (Action.Hit == false && HitTime <= Game.Time)
                {
                    Action.Hit = true;

                    //if (Owner.IsPlayer && AnimAttackData.FullCombo)
                    //    GuiManager.Instance.ShowComboMessage(AnimAttackData.ComboIndex);

                    //Mission.Instance.CurrentGameZone.BreakBreakableObjects(Owner);

                    //if (Action.AttackType == E_AttackType.Fatality)
                    //    Mission.Instance.CurrentGameZone.DoDamageFatality(Owner, Action.Target, Owner.BlackBoard.WeaponSelected, AnimAttackData);
                    //else
                    //    Mission.Instance.CurrentGameZone.DoMeleeDamage(Owner, Action.Target, Owner.BlackBoard.WeaponSelected, AnimAttackData, Critical, Knockdown);

                    //if (AnimAttackData.LastAttackInCombo || AnimAttackData.ComboStep == 3)
                    //    CameraBehaviour.Instance.ComboShake(AnimAttackData.ComboStep - 3);

                    //if (AnimAttackData.LastAttackInCombo)
                    //    Owner.StartCoroutine(ShowTrail(AnimAttackData, 1, 0.3f, Critical, MoveTime - Time.timeSinceLevelLoad));
                    //else
                    //    Owner.StartCoroutine(ShowTrail(AnimAttackData, 2, 0.1f, Critical, MoveTime - Time.timeSinceLevelLoad));

                    //Debug.Log("DoMeleeDamage  " + (Action.AttackTarget != null ? Action.AttackTarget.name : "no target"));
                }
            }
            else if (State == E_State.E_FINISHED && EndOfStateTime <= Game.Time)
            {
                Action.AttackPhaseDone = true;
                //Debug.Log(Time.timeSinceLevelLoad + " attack finished");
                Release();
            }
        }

        private void PlayAnim()
        {
            CrossFade(AnimAttackData.AnimName, 0.2f);

            // when to do hit !!!
            HitTime = Game.Time + AnimAttackData.HitTime;

            StartPosition = this.Owner.Position;
            FinalPosition = StartPosition + this.Owner.Forward * AnimAttackData.MoveDistance;
            MoveTime = AnimAttackData.AttackMoveEndTime - AnimAttackData.AttackMoveStartTime;

            EndOfStateTime = Game.Time + AnimEngine.GetAnimLength(AnimAttackData.AnimName) * 0.9f;

            if (AnimAttackData.LastAttackInCombo)
                AttackPhaseTime = Game.Time + AnimEngine.GetAnimLength(AnimAttackData.AnimName) * 0.9f;
            else
                AttackPhaseTime =  Game.Time + AnimAttackData.AttackEndTime;

            CurrentMoveTime = -AnimAttackData.AttackMoveStartTime; // move a little bit later

            //if (Action.Target && Action.Target.IsAlive)
            //{
            //    if (Critical)
            //    {
            //        CameraBehaviour.Instance.InterpolateTimeScale(0.25f, 0.5f);
            //        CameraBehaviour.Instance.InterpolateFov(25, 0.5f);
            //        CameraBehaviour.Instance.Invoke("InterpolateScaleFovBack", 0.7f);
            //    }
            //    else if (Action.AttackType == E_AttackType.Fatality)
            //    {
            //        CameraBehaviour.Instance.InterpolateTimeScale(0.25f, 0.7f);
            //        CameraBehaviour.Instance.InterpolateFov(25, 0.65f);
            //        CameraBehaviour.Instance.Invoke("InterpolateScaleFovBack", 0.8f);
            //    }
            //}
        }

        override protected void Initialize(AgentAction action)
        {
            base.Initialize(action);
            SetFinished(false);

            State = E_State.E_PREPARING;
            Owner.BlackBoard.MotionType = E_MotionType.Attack;

            Action = action as AgentActionAttack;
            Action.AttackPhaseDone = false;
            Action.Hit = false;

            if (Action.Data == null)
                Action.Data = Owner.AnimSet.GetFirstAttackAnim(Owner.BlackBoard.WeaponSelected, Action.AttackType);

            AnimAttackData = Action.Data;

            if (AnimAttackData == null)
            {
                Log.LogError("AnimAttackData == null","wyb");
            }

            StartRotation = this.Owner.Facing;
            StartPosition = this.Owner.Position;

            FP angle = 0;

            bool backstab = false;

            FP distance = 0;
            if (Action.Target != null)
            {
                TSVector2 dir = Action.Target.Position - this.Owner.Position;
                distance = dir.magnitude;

                if (distance > 0.1f)
                {
                    dir.Normalize();
                    
                    angle = TSVector2.Angle(this.Owner.Forward,dir);

                    //attacker uhel k cili je mensi nez 40 and uhel enemace a utocnika je mensi nez 80 stupnu
                    if (angle < 40 && TSVector2.Angle(Owner.Forward, Action.Target.Forward) < 80)
                        backstab = true;
                }
                else
                {
                    dir = this.Owner.Forward;
                }

                FinalRotation = MathUtils.TSVector2ToFacing(dir,360);

                if (distance < Owner.BlackBoard.WeaponRange)
                    FinalPosition = StartPosition;
                else
                    FinalPosition = Action.Target.Position - dir * Owner.BlackBoard.WeaponRange;

                MoveTime = (FinalPosition - StartPosition).magnitude / 20.0f;
                RotationTime = angle / 720.0f;
            }
            else
            {

                FinalRotation = MathUtils.TSVector2ToFacing(Action.AttackDir, 360);
                //FinalRotation.SetLookRotation(Action.AttackDir);
                
                RotationTime = TSVector2.Angle(this.Owner.Forward, Action.AttackDir) / 720.0f;
                MoveTime = 0;
            }

            RotationOk = RotationTime == 0;
            PositionOK = MoveTime == 0;

            CurrentRotationTime = 0;
            CurrentMoveTime = 0;

            //if (Owner.IsPlayer && AnimAttackData.HitCriticalType != E_CriticalHitType.None && Action.Target && Action.Target.BlackBoard.CriticalAllowed && Action.Target.IsBlocking == false && Action.Target.IsInvulnerable == false && Action.Target.IsKnockedDown == false)
            //{
            //    if (backstab)
            //        Critical = true;
            //    else
            //    {
            //        //           Debug.Log("critical chance" + Owner.GetCriticalChance() * AnimAttackData.CriticalModificator * Action.Target.BlackBoard.CriticalHitModifier);
            //        Critical = Random.Range(0, 100) < Owner.GetCriticalChance() * AnimAttackData.CriticalModificator * Action.Target.BlackBoard.CriticalHitModifier;
            //    }
            //}
            //else
            //    Critical = false;

            //Knockdown = AnimAttackData.HitAreaKnockdown && Random.Range(0, 100) < 60 * Owner.GetCriticalChance();
        }
    }
}
