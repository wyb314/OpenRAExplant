using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public enum E_AttackType : byte
    {
        X = 0,
        O = 1,
        BossBash = 2,
        Fatality = 3,
        Counter = 4,
        Berserk = 5,
        Max = 6,
    }
}
