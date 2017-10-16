using System;
using System.Collections.Generic;
using Engine.ComponentsAI.ComponentPlayer;
using TrueSync;

namespace Engine.ComponentAnim.Core
{
    [Serializable]
    public class AnimAttackData
    {
        public string AnimName;
        public AnimAttackData[] NextAttacks = new AnimAttackData[(int)E_AttackType.Max];

        // best attack distance
        public FP MoveDistance;

        //timers
        public FP AttackMoveStartTime;
        public FP AttackMoveEndTime;
        public FP AttackEndTime;
        public FP ZanshinEndTime;

        // hit parameters
        public FP HitTime;
        public FP HitDamage;
        public FP HitAngle;
        public FP HitMomentum;
        public E_CriticalHitType HitCriticalType;
        public bool HitAreaKnockdown;
        public bool BreakBlock;
        public bool UseImpuls;
        public FP CriticalModificator = 1;
        public bool SloMotion;

        //meni se v realtime, pro playera
        public bool FirstAttackInCombo = true;
        public bool LastAttackInCombo = false;
        public int ComboIndex;
        public bool FullCombo;
        public int ComboStep;


        public AnimAttackData(string animName, FP moveDistance, FP hitTime,
            FP attackEndTime, FP hitDamage, FP hitAngle, FP hitMomentum,
                    E_CriticalHitType criticalType, bool areaKnockDown)
        {
            AnimName = animName;
            

            MoveDistance = moveDistance;

            AttackEndTime = attackEndTime;
            AttackMoveStartTime = 0;
            AttackMoveEndTime = AttackEndTime * 0.7f;

            HitTime = hitTime;
            HitDamage = hitDamage;
            HitAngle = hitAngle;
            HitMomentum = hitMomentum;
            HitCriticalType = criticalType;
            HitAreaKnockdown = areaKnockDown;
            BreakBlock = false;
            UseImpuls = false;
            CriticalModificator = 1;
        }


        public AnimAttackData(string animName,FP moveDistance, FP hitTime,
            FP moveStartTime, FP moveEndTime, FP attackEndTime, FP hitDamage,
            FP hitAngle, FP hitMomentum,
                    E_CriticalHitType criticalType, FP criticalMod, bool areaKnockDown,
                    bool breakBlock, bool useImpuls, bool sloMotion)
        {
            AnimName = animName;
            

            MoveDistance = moveDistance;

            AttackMoveStartTime = moveStartTime;
            AttackMoveEndTime = moveEndTime;
            AttackEndTime = attackEndTime;

            HitTime = hitTime;
            HitDamage = hitDamage;
            HitAngle = hitAngle;
            HitMomentum = hitMomentum;
            HitCriticalType = criticalType;
            HitAreaKnockdown = areaKnockDown;
            BreakBlock = breakBlock;
            UseImpuls = useImpuls;
            CriticalModificator = criticalMod;
            SloMotion = sloMotion;

        }


    }
}
