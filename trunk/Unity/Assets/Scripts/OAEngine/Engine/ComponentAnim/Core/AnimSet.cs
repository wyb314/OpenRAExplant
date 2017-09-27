using System;
using System.Collections.Generic;
using Engine.ComponentsAI.ComponentPlayer;
using OAEngine.Engine.ComponentsAI;

namespace Engine.ComponentAnim.Core
{
    public abstract class AnimSet
    {
        public abstract string GetIdleAnim(E_WeaponType weapon, E_WeaponState weaponState);

        public abstract string GetIdleActionAnim(E_WeaponType weapon, E_WeaponState weaponState);

        public abstract string GetMoveAnim(E_MotionType motion, E_MoveType move, E_WeaponType weapon, E_WeaponState weaponState);

        public abstract string GetRotateAnim(E_MotionType motionType, E_RotationType rotationType);

        public abstract string GetRollAnim(E_WeaponType weapon, E_WeaponState weaponState);

        public abstract string GetBlockAnim(E_BlockState block, E_WeaponType weapon);

        public abstract string GetKnockdowAnim(E_KnockdownState block, E_WeaponType weapon);

        public abstract string GetShowWeaponAnim(E_WeaponType weapon);

        public abstract string GetHideWeaponAnim(E_WeaponType weapon);

        public abstract string GetUseAnim(E_InteractionObjects objectType, E_InteractionType interaction);

        //public abstract AnimAttackData GetFirstAttackAnim(E_WeaponType weapon, E_AttackType attackType);
        //public abstract AnimAttackData GetChainedAttackAnim(AnimAttackData parent, E_AttackType attackType);
        //public virtual AnimAttackData GetRollAttackAnim() { return null; }
        //public virtual AnimAttackData GetWhirlAttackAnim() { return null; }
        public virtual string GetInjuryPhaseAnim(int phase) { return null; }

        public abstract string GetInjuryAnim(E_WeaponType weapon, E_DamageType type);

        public abstract string GetDeathAnim(E_WeaponType weapon, E_DamageType type);
    }
}
