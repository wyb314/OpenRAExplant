

namespace Engine.AI.GOAP.Enums
{
    public enum E_GOAPAction
    {
        invalid = -1,
        move,                 //0
        gotoPos,                 //70
        gotoMeleeRange,      //70
        combatMoveRight,    //0
        combatMoveLeft,     //0
        combatMoveForward,  //0
        combatMoveBackward, //0
        combatRunForward,   //0
        combatRunBackward,  //0
        lookatTarget,       //95
        weaponShow,          //90
        weaponHide,          //0
        attackMeleeOnce,    //0
        attackWhirl,        //0
        attackBerserk,        //0
        attackMeleeTwoSwords,    //0
        attackBow,          //0
        attackBoss,          //20
        attackRoll,          //20
        attackCounter,          //20
        orderAttack,         //0
        orderDodge,          //0
        rollToTarget,       //80
        useLever,            //0
        playAnim,            //0
        injury,               //100   
        injuryOrochi,               //100   
        death,                //100
        block,                //50
        tount,           //0
        knockdown,           //100
        teleport,             //90  
        count
    }
}
