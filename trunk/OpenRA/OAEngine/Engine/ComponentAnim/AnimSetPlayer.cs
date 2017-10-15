using System;
using System.Collections.Generic;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI.ComponentPlayer;
using OAEngine.Engine.ComponentsAI;

namespace Engine.ComponentAnim
{
    public class AnimSetPlayer : AnimSet
    {

        public AnimAttackData[] AttackData = new AnimAttackData[24];
        private AnimAttackData AttackKnockdown;

        public override void Init()
        {
            AttackKnockdown = new AnimAttackData("attackKnockdown", 1.5f, 0.65f, 0.2f, 0.6f, 1.0f, 20, 0, 0, E_CriticalHitType.None, 0, false, false, false, true);

            // AAAAA, critical later
            AttackData[0] = new AnimAttackData("attackX", 0.6f, 0.23f, 0.05f, 0.366f, 0.366f, 3, 15, 0.6f, E_CriticalHitType.Vertical, 0.2f, false, false, false, false);
            AttackData[1] = new AnimAttackData("attackXX", 0.6f, 0.22f, 0.15f, 0.35f, 0.4f, 5, 15, 0.6f, E_CriticalHitType.Vertical, 0.25f, false, false, false, false);
            /*1*/
            AttackData[2] = new AnimAttackData("attackXXX", 0.7f, 0.25f, 0.20f, 0.30f, 0.366f, 10, 45, 0.75f, E_CriticalHitType.Horizontal, 0.3f, false, false, false, false);
            AttackData[3] = new AnimAttackData("attackXXXX",  0.8f, 0.28f, 0.22f, 0.35f, 0.366f, 12, 90, 0.8f, E_CriticalHitType.Horizontal, 0.5f, false, false, true, false);
            AttackData[4] = new AnimAttackData("attackXXXXX", 0.8f, 0.3f, 0.15f, 0.35f, 0.366f, 16, 45, 1.5f, E_CriticalHitType.Vertical, 0.6f, false, false, true, true);
            // ANTI BLOCK , no critical
            AttackData[5] = new AnimAttackData("attackO", 0.7f, 0.38f, 0.30f, 0.40f, 0.45f, 6, 45, 0.8f, E_CriticalHitType.Horizontal, 0.1f, false, true, false, false);
            AttackData[6] = new AnimAttackData("attackOO", 0.7f, 0.34f, 0.10f, 0.35f, 0.4f, 10, 45, 0.8f, E_CriticalHitType.Vertical, 0.15f, false, true, false, false);
            /*2*/
            AttackData[7] = new AnimAttackData("attackOOO",  1.0f, 0.5f, 0.36f, 0.55f, 0.6f, 15, 15, 1.0f, E_CriticalHitType.None, 0, false, true, false, false);
            AttackData[8] = new AnimAttackData("attackOOOX", 1.0f, 0.45f, 0.15f, 0.45f, 0.533f, 20, 45, 0.5f, E_CriticalHitType.Vertical, 0.8f, false, true, false, false);
            AttackData[9] = new AnimAttackData("attackOOOXX",  1.0f, 0.51f, 0.15f, 0.6f, 0.7f, 25, 20, 1.8f, E_CriticalHitType.Vertical, 1, false, true, true, true);
            // 
            AttackData[10] = new AnimAttackData("attackXO",  0.85f, 0.45f, 0.10f, 0.8f, 0.833f, 15, 25, 0.8f, E_CriticalHitType.None, 0, false, false, false, false);
            /*4*/
            AttackData[11] = new AnimAttackData("attackXOO",0.8f, 0.45f, 0.20f, 0.50f, 0.55f, 20, 15, 0.8f, E_CriticalHitType.None, 0, false, false, false, false);
            AttackData[12] = new AnimAttackData("attackXOOX", 0.7f, 0.4f, 0.3f, 0.7f, 0.8f, 20, 180, 1, E_CriticalHitType.Vertical, 0.8f, false, false, true, false);
            AttackData[13] = new AnimAttackData("attackXOOXX", 1.5f, 0.35f, 0.10f, 0.30f, 0.48f, 25, 20, 1.5f, E_CriticalHitType.Vertical, 1.0f, false, false, true, true);
            //KNOCK
            AttackData[14] = new AnimAttackData("attackXXO", 0.7f, 0.30f, 0.15f, 0.4f, 0.6f, 15, 90, 1.0f, E_CriticalHitType.None, 0, true, false, true, true);
            /*5*/
            AttackData[15] = new AnimAttackData("attackXXOX", 1.0f, 0.41f, 0.11f, 0.55f, 0.60f, 20, 15, 0.7f, E_CriticalHitType.Vertical, 1, false, false, true, false);
            AttackData[16] = new AnimAttackData("attackXXOXX", 1.0f, 0.5f, 0.1f, 0.4f, 0.6f, 20, 180, 1.2f, E_CriticalHitType.None, 0, true, false, true, true);
            //CRITICAL
            AttackData[17] = new AnimAttackData("attackOOX", 0.8f, 0.45f, 0.25f, 0.5f, 0.66f, 15, 25, 0.7f, E_CriticalHitType.Vertical, 1.2f, false, false, true, true);
            /*3*/
            AttackData[18] = new AnimAttackData("attackOOXO",  0.7f, 0.6f, 0.2f, 0.6f, 0.7f, 20, 45, 0.7f, E_CriticalHitType.Vertical, 1.4f, false, false, false, false);
            AttackData[19] = new AnimAttackData("attackOOXOO",  1.0f, 0.45f, 0.05f, 0.65f, 1.03f, 25, 30, 1.5f, E_CriticalHitType.Vertical, 1.6f, false, false, true, true);
            //area dmage 
            AttackData[20] = new AnimAttackData("attackOX", 0.7f, 0.25f, 0.15f, 0.44f, 0.45f, 20, 25, 0.8f, E_CriticalHitType.Vertical, 0.4f, false, false, true, true);
            /*6*/
            AttackData[21] = new AnimAttackData("attackOXO", 1.0f, 0.35f, 0.25f, 0.4f, 0.55f, 20, 90, 1, E_CriticalHitType.Horizontal, 0.5f, false, false, true, false);
            AttackData[22] = new AnimAttackData("attackOXOX", 1.0f, 0.35f, 0.15f, 0.3f, 0.5f, 20, 180, 1.2f, E_CriticalHitType.Horizontal, .7f, false, false, true, false);
            AttackData[23] = new AnimAttackData("attackOXOXO", 1.0f, 0.35f, 0.15f, 0.5f, 1.1f, 25, 180, 1.8f, E_CriticalHitType.Horizontal, 0.9f, false, false, true, true);

        }

        public override string GetBlockAnim(E_BlockState state, E_WeaponType weapon) { return null; }


        public override string GetIdleAnim(E_WeaponType weapon, E_WeaponState weaponState)
        {
            if (weaponState == E_WeaponState.NotInHands)
                return "idle";

            return "idleSword";
        }

        public override string GetIdleActionAnim(E_WeaponType weapon, E_WeaponState weaponState)
        {
            return "idle";
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

        public override string GetRotateAnim(E_MotionType motionType, E_RotationType rotationType)
        {
            return null;
        }


        public override string GetRollAnim(E_WeaponType weapon, E_WeaponState weaponState)
        {
            return "evadeSword";
        }

        public override string GetShowWeaponAnim(E_WeaponType weapon)
        {
            return "showSwordRun";
        }

        public override string GetHideWeaponAnim(E_WeaponType weapon)
        {
            return "hidSwordRun";
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

        public AnimAttackData GetFatalityAttack()
        {
            return AttackKnockdown;
        }

        public override AnimAttackData GetFirstAttackAnim(E_WeaponType weapon, E_AttackType attackType)
        {
            if (attackType == E_AttackType.Fatality)
                return AttackKnockdown;

            return null;
        }

        public override AnimAttackData GetChainedAttackAnim(AnimAttackData parent, E_AttackType attackType)
        {
            return null;
        }

        public override string GetInjuryAnim(E_WeaponType weapon, E_DamageType type)
        {
            if (type == E_DamageType.Back)
                return "injuryBackSword";

            return "injuryFrontSword";
        }


        public override string GetDeathAnim(E_WeaponType weapon, E_DamageType type)
        {
            if (type == E_DamageType.Back)
                return "deathBack";

            return "deathFront";
        }

        public override string GetKnockdowAnim(E_KnockdownState block, E_WeaponType weapon)
        {
            return null;
        }



    }
}
