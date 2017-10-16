using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.ComponentAnim.Core
{
    public enum E_WeaponType
    {
        None = -1,
        Katana = 0,
        Body,
        Bow,
        Max,
    }

    public enum E_WeaponState
    {
        NotInHands,
        Ready,
        Attacking,
        Reloading,
        Empty,
    }

    public enum E_MoveType
    {
        None,
        Forward,
        Backward,
        StrafeLeft,
        StrafeRight,
    }

    public enum E_RotationType
    {
        Left,
        Right
    }

    public enum E_BlockState
    {
        None = -1,
        Start = 0,
        Loop,
        End,
        HitBlocked,
        Failed
    }

    public enum E_KnockdownState
    {
        None = -1,
        Down = 0,
        Loop,
        Up,
        Fatality,
    }

    public enum E_DamageType
    {
        Front,
        Back,
        BreakBlock,
        InKnockdown,
        Enviroment,
    }

    public enum E_CriticalHitType
    {
        None,
        Vertical,
        Horizontal,
    }

    public enum E_InteractionObjects
    {
        None,
        UseLever,
        Trigger,
        UseExperience,
        TriggerAnim,
    }

    public enum E_InteractionType
    {
        None,
        On,
        Off
    }

    public enum E_LookType
    {
        None,
        TrackTarget,
    }

    public enum E_AttackType
    {
        None = -1,
        X = 0,
        O = 1,
        BossBash = 2,
        Fatality = 3,
        Counter = 4,
        Berserk = 5,
        Max = 6,
    }

    public enum E_ComboLevel
    {
        One = 1,
        Two = 2,
        Three = 3,
        Max = 3
    }

    public enum E_SwordLevel
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Max = 5
    }
}
