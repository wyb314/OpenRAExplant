using UnityEngine;
public class AnimSetEnemyBossOrochi : AnimSet
    protected AnimAttackData AnimAttackX;
    protected AnimAttackData AnimAttackBerserk;
    protected AnimAttackData AnimAttackInjury;
        AnimAttackBerserk = new AnimAttackData("attackO", null, 0, 0.9f, 3.5f, 30, 30, 3.5f, E_CriticalHitType.None, false);
        AnimAttackInjury = new AnimAttackData("attackInjury", null, 0, 0.5f, 1.5f, 30, 180, 4, E_CriticalHitType.None, false);

    public override string GetIdleActionAnim(E_WeaponType weapon, E_WeaponState weaponState)
    {
        return "tount";
    }

    public override string GetMoveAnim(E_MotionType motion, E_MoveType move, E_WeaponType weapon, E_WeaponState weaponState)
        return "walk";

    public override string GetRotateAnim(E_MotionType motionType, E_RotationType rotationType)
    {
        if (rotationType == E_RotationType.Left)
            return "rotateL";

        return "rotateR";
    }



    public override AnimAttackData GetFirstAttackAnim(E_WeaponType weapon, E_AttackType attackType)
    {
        if (attackType == E_AttackType.X)
            return AnimAttackX;
        else if (attackType == E_AttackType.O)
            return AnimAttackInjury;
        else if (attackType == E_AttackType.Berserk)
            return AnimAttackBerserk;

        return null;
    }

    public override AnimAttackData GetChainedAttackAnim(AnimAttackData parent, E_AttackType attackType) {return null; }


    public override string GetInjuryPhaseAnim(int phase) {
        string[] s = { "injury1", "injury2", "injury3", "injuryEnd" };

        return s[phase]; 
    }

    public override string GetInjuryAnim(E_WeaponType weapon, E_DamageType type) { return "null"; }

    public override string GetDeathAnim(E_WeaponType weapon, E_DamageType type)
    {
        return "death";
    }

    public override string GetKnockdowAnim(E_KnockdownState state, E_WeaponType weapon)  { return ""; }