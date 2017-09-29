using System;
using System.Collections.Generic;
using Engine.ComponentAnim.Core;
using OAEngine.Engine.ComponentsAI;

namespace Engine.ComponentAnim
{
    public class AnimSetPlayer : AnimSet
    {
        public override string GetBlockAnim(E_BlockState block, E_WeaponType weapon)
        {
            return null;
        }

        public override string GetDeathAnim(E_WeaponType weapon, E_DamageType type)
        {
            if (type == E_DamageType.Back)
                return "deathBack";

            return "deathFront";
        }

        public override string GetHideWeaponAnim(E_WeaponType weapon)
        {
            return "hidSwordRun";
        }

        public override string GetIdleActionAnim(E_WeaponType weapon, E_WeaponState weaponState)
        {
            return "idle";
        }

        public override string GetIdleAnim(E_WeaponType weapon, E_WeaponState weaponState)
        {
            if (weaponState == E_WeaponState.NotInHands)
                return "idle";

            return "idleSword";
        }

        public override string GetInjuryAnim(E_WeaponType weapon, E_DamageType type)
        {
            if (type == E_DamageType.Back)
                return "injuryBackSword";

            return "injuryFrontSword";
        }

        public override string GetKnockdowAnim(E_KnockdownState block, E_WeaponType weapon)
        {
            return null;
        }

        public override string GetMoveAnim(E_MotionType motion, E_MoveType move, E_WeaponType weapon, E_WeaponState weaponState)
        {
            if (weaponState == E_WeaponState.NotInHands)
            {
                if (motion != E_MotionType.Walk)
                    return "run";
                else
                    return "walk";
            }

            if (motion != E_MotionType.Walk)
                return "runSword";

            return "walkSword";
        }

        public override string GetRollAnim(E_WeaponType weapon, E_WeaponState weaponState)
        {
            return "evadeSword";
        }

        public override string GetRotateAnim(E_MotionType motionType, E_RotationType rotationType)
        {
            return null;
        }

        public override string GetShowWeaponAnim(E_WeaponType weapon)
        {
            return "showSwordRun";
        }

        public override string GetUseAnim(E_InteractionObjects objectType, E_InteractionType interaction)
        {
            if (objectType == E_InteractionObjects.UseLever)
                return "useLever";

            if (objectType == E_InteractionObjects.Trigger)
                return "idle";

            if (objectType == E_InteractionObjects.UseExperience)
                return "attackJump";

            return "idle";
        }
    }
}
