using System;
using System.Collections.Generic;
using Engine.ComponentAnim;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.Factories;
using Engine.ComponentsAI.GOAP;
using Engine.ComponentsAI.GOAP.Core;
using Engine.Support;
using OAEngine.Engine.ComponentsAI;
using TrueSync;

namespace Engine.ComponentsAI.ComponentPlayer
{

    public class ComponentPlayer: IActionHandler
    {

        public class ComboStep
        {
            public E_AttackType AttackType;
            public E_ComboLevel ComboLevel;
            public AnimAttackData Data;
        }

        public class Combo
        {
            public E_SwordLevel SwordLevel;
            public ComboStep[] ComboSteps;
        }

        public Combo[] PlayerComboAttacks = new Combo[6];
        private List<E_AttackType> ComboProgress = new List<E_AttackType>();

        private Agent Owner;

        private AgentActionAttack CurrentAttackAction;
        private AnimSetPlayer AnimSet;

        private Agent LastAttacketTarget;

        public int force;

        private Queue<AgentOrder> BufferedOrders = new Queue<AgentOrder>();

        public ComponentPlayer(Agent Owner)
        {
            this.Owner = Owner;
        }


        public void Init(AnimSetPlayer animSet)
        {
            this.AnimSet = animSet;
            PlayerComboAttacks[0] = new Combo() // FAST   Raisin Wave
            {
                SwordLevel = E_SwordLevel.One,
                ComboSteps = new ComboStep[]{new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[0]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[1]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[2]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Two, Data = AnimSet.AttackData[3]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.Three, Data = AnimSet.AttackData[4]},
            }
            };
            PlayerComboAttacks[1] = new Combo() // BREAK BLOCK  half moon
            {
                SwordLevel = E_SwordLevel.One,
                ComboSteps = new ComboStep[]{new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[5]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[6]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[7]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Two, Data = AnimSet.AttackData[8]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Three, Data = AnimSet.AttackData[9]},
            }
            };
            PlayerComboAttacks[2] = new Combo() // CRITICAL  cloud cuttin
            {
                SwordLevel = E_SwordLevel.Two,
                ComboSteps = new ComboStep[]{new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[5]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[6]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[17]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Two, Data = AnimSet.AttackData[18]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Three, Data = AnimSet.AttackData[19]},
            }
            };

            PlayerComboAttacks[3] = new Combo()  // flying dragon
            {
                SwordLevel = E_SwordLevel.Three,
                ComboSteps = new ComboStep[]{new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[0]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[10]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[11]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Two, Data = AnimSet.AttackData[12]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Three, Data = AnimSet.AttackData[13]},
            }
            };
            PlayerComboAttacks[4] = new Combo() // KNCOK //walking death
            {
                SwordLevel = E_SwordLevel.Four,
                ComboSteps = new ComboStep[]{new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[0]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[1]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[14]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Two, Data = AnimSet.AttackData[15]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.Three, Data = AnimSet.AttackData[16]},
            }
            };

            PlayerComboAttacks[5] = new Combo() // HEAVY, AREA  shogun death
            {
                SwordLevel = E_SwordLevel.Five,
                ComboSteps = new ComboStep[]{new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[5]},
                                         new ComboStep(){AttackType = E_AttackType.X, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[20]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.One, Data = AnimSet.AttackData[21]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.Two, Data = AnimSet.AttackData[22]},
                                         new ComboStep(){AttackType = E_AttackType.O, ComboLevel = E_ComboLevel.Three, Data = AnimSet.AttackData[23]},
            }
            };

            //Owner.BlackBoard.IsPlayer = true;
            Owner.BlackBoard.Rage = 0;
            Owner.BlackBoard.Dodge = 0;
            Owner.BlackBoard.Fear = 0;

            this.Owner.AddGOAPAction(E_GOAPAction.gotoPos);
            this.Owner.AddGOAPAction(E_GOAPAction.move);
            this.Owner.AddGOAPAction(E_GOAPAction.gotoMeleeRange);
            this.Owner.AddGOAPAction(E_GOAPAction.weaponShow);
            this.Owner.AddGOAPAction(E_GOAPAction.weaponHide);
            this.Owner.AddGOAPAction(E_GOAPAction.orderAttack);
            //Agent.AddGOAPAction(E_GOAPAction.orderAttackJump);
            this.Owner.AddGOAPAction(E_GOAPAction.orderDodge);
            this.Owner.AddGOAPAction(E_GOAPAction.rollToTarget);
            this.Owner.AddGOAPAction(E_GOAPAction.useLever);
            this.Owner.AddGOAPAction(E_GOAPAction.playAnim);
            this.Owner.AddGOAPAction(E_GOAPAction.teleport);
            this.Owner.AddGOAPAction(E_GOAPAction.injury);
            this.Owner.AddGOAPAction(E_GOAPAction.death);

            this.Owner.AddGOAPGoal(E_GOAPGoals.E_GOTO);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_ORDER_ATTACK);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_ORDER_DODGE);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_ORDER_USE);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_ALERT);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_CALM);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_USE_WORLD_OBJECT);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_PLAY_ANIM);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_TELEPORT);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_REACT_TO_DAMAGE);

            this.Owner.InitializeGOAP();

            Owner.BlackBoard.ActionHandlerAdd(this);
        }



        public bool CouldBufferNewOrder()
        {
            return BufferedOrders.Count <= 0 && CurrentAttackAction != null;
        }

        public bool CouldAddnewOrder()
        {
            AgentOrder.E_OrderType order = Owner.WorldState.GetWSProperty(E_PropKey.E_ORDER).GetOrder();

            if (order == AgentOrder.E_OrderType.E_DODGE || order == AgentOrder.E_OrderType.E_ATTACK || order == AgentOrder.E_OrderType.E_USE)
                return false;

            AgentAction action;

            for (int i = 0; i < Owner.BlackBoard.ActionCount(); i++)
            {
                action = Owner.BlackBoard.ActionGet(i);
                if (action is AgentActionAttack && (action as AgentActionAttack).AttackPhaseDone == false)
                    return false;
                else if (action is AgentActionRoll)
                    return false;
                //else if (action is AgentActionUseLever)
                //    return false;
                else if (action is AgentActionGoTo && (action as AgentActionGoTo).Motion == E_MotionType.Sprint)
                    return false;
            }
            return true;
        }

        public void CreateOrderAttack(E_AttackType type)
        {
            if (CouldBufferNewOrder() == false && CouldAddnewOrder() == false)
            {
                return;
            }
            AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_ATTACK);

            if (this.Owner.CurJoystickDir != TSVector2.zero)
                order.Direction = this.Owner.CurJoystickDir;
            else
                order.Direction = this.Owner.Forward;


            order.AnimAttackData = ProcessCombo(type);

            order.Target = GetBestTarget(false);

            if (CouldAddnewOrder())
            {
                Owner.BlackBoard.OrderAdd(order);
            }
            else
            {
                BufferedOrders.Enqueue(order);
            }
        }

        private AnimAttackData ProcessCombo(E_AttackType attackType)
        {
            if (attackType != E_AttackType.O && attackType != E_AttackType.X)
                return null;

            ComboProgress.Add(attackType);

            for (int i = 0; i < PlayerComboAttacks.Length; i++)
            {// projedem vsechny attacky

                Combo combo = PlayerComboAttacks[i];

                if (combo.SwordLevel > (E_SwordLevel)4)
                    continue; // nema combo...

                bool valid = ComboProgress.Count <= combo.ComboSteps.Length; // 
                for (int ii = 0; ii < ComboProgress.Count && ii < combo.ComboSteps.Length; ii++)
                {
                    if (ComboProgress[ii] != combo.ComboSteps[ii].AttackType || combo.ComboSteps[ii].ComboLevel > this.Owner.ComboLevel[i])
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    combo.ComboSteps[ComboProgress.Count - 1].Data.LastAttackInCombo = NextAttackIsAvailable(E_AttackType.X) == false && NextAttackIsAvailable(E_AttackType.O) == false;
                    combo.ComboSteps[ComboProgress.Count - 1].Data.FirstAttackInCombo = false;
                    combo.ComboSteps[ComboProgress.Count - 1].Data.ComboIndex = i;
                    combo.ComboSteps[ComboProgress.Count - 1].Data.FullCombo = ComboProgress.Count == combo.ComboSteps.Length;
                    combo.ComboSteps[ComboProgress.Count - 1].Data.ComboStep = ComboProgress.Count;

                    //if (ComboProgress.Count == 3)
                    //FlurryrBinding.FlurryLogPerformedCombo(ComboNames[i]);

                    //GuiManager.Instance.ShowComboProgress(ComboProgress);
                    return combo.ComboSteps[ComboProgress.Count - 1].Data;
                }
            }

            // takze zadny uspech

            //pokud ale je nabuferovano,tak nezacinat nove combo ?


            //je treba zacit od zacatku
            ComboProgress.Clear();
            ComboProgress.Add(attackType);

            for (int i = 0; i < PlayerComboAttacks.Length; i++)
            {// projedem vsechny prvni stepy
                if (PlayerComboAttacks[i].ComboSteps[0].AttackType == attackType)
                {
                    // Debug.Log(Time.timeSinceLevelLoad + " New combo " + i + " step " + ComboProgress.Count);
                    PlayerComboAttacks[i].ComboSteps[0].Data.FirstAttackInCombo = true;
                    PlayerComboAttacks[i].ComboSteps[0].Data.LastAttackInCombo = false;
                    PlayerComboAttacks[i].ComboSteps[0].Data.ComboIndex = i;
                    PlayerComboAttacks[i].ComboSteps[0].Data.FullCombo = false;
                    PlayerComboAttacks[i].ComboSteps[0].Data.ComboStep = 0;

                    //GuiManager.Instance.ShowComboProgress(ComboProgress);
                    return PlayerComboAttacks[i].ComboSteps[0].Data;
                }
            }
            
            return null;
        }


        private bool NextAttackIsAvailable(E_AttackType attackType)
        {
            if (attackType != E_AttackType.O && attackType != E_AttackType.X)
                return false;

            if (ComboProgress.Count == 5) // ehmm. proste je jich ted sest, tak bacha na to...
                return false;

            List<E_AttackType> progress = new List<E_AttackType>(ComboProgress);

            progress.Add(attackType);

            for (int i = 0; i < PlayerComboAttacks.Length; i++)
            {// projedem vsechny attacky

                Combo combo = PlayerComboAttacks[i];

                if (combo.SwordLevel >this.Owner.SwordLevel)
                    continue;

                bool valid = true;
                for (int ii = 0; ii < progress.Count; ii++)
                {
                    if (progress[ii] != combo.ComboSteps[ii].AttackType || combo.ComboSteps[ii].ComboLevel > this.Owner.ComboLevel[i])
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                    return true;
            }
            return false;
        }

        public Agent GetBestTarget(bool hasToBeKnockdown)
        {
            //if (Mission.Instance.CurrentGameZone == null)
            //    return null;

            //List<Agent> enemies = Mission.Instance.CurrentGameZone.Enemies;

            //float[] EnemyCoeficient = new float[enemies.Count];
            //Agent enemy;
            //Vector3 dirToEnemy;

            //for (int i = 0; i < enemies.Count; i++)
            //{
            //    EnemyCoeficient[i] = 0;
            //    enemy = enemies[i];

            //    if (hasToBeKnockdown && enemy.BlackBoard.MotionType != E_MotionType.Knockdown)
            //        continue;

            //    if (enemy.BlackBoard.Invulnerable)
            //        continue;

            //    dirToEnemy = (enemy.Position - Owner.Position);

            //    float distance = dirToEnemy.magnitude;

            //    if (distance > 5.0f)
            //        continue;

            //    dirToEnemy.Normalize();

            //    float angle = Vector3.Angle(dirToEnemy, Owner.Forward);

            //    if (enemy == LastAttacketTarget)
            //        EnemyCoeficient[i] += 0.1f;

            //    //Debug.Log("LastTarget " + Mission.Instance.CurrentGameZone.GetEnemy(i).name + " : " + EnemyCoeficient[i]); 

            //    EnemyCoeficient[i] += 0.2f - ((angle / 180.0f) * 0.2f);

            //    //  Debug.Log("angle " + Mission.Instance.CurrentGameZone.GetEnemy(i).name + " : " + EnemyCoeficient[i]);

            //    if (Controls.Joystick.Direction != Vector3.zero)
            //    {
            //        angle = Vector3.Angle(dirToEnemy, Controls.Joystick.Direction);
            //        EnemyCoeficient[i] += 0.5f - ((angle / 180.0f) * 0.5f);
            //    }
            //    //    Debug.Log(" joy " + Mission.Instance.CurrentGameZone.GetEnemy(i).name + " : " + EnemyCoeficient[i]); 

            //    EnemyCoeficient[i] += 0.2f - ((distance / 5) * 0.2f);

            //    //      Debug.Log(" dist " + Mission.Instance.CurrentGameZone.GetEnemy(i).name + " : " + EnemyCoeficient[i]); 
            //}

            //float bestValue = 0;
            //int best = -1;
            //for (int i = 0; i < enemies.Count; i++)
            //{
            //    //     Debug.Log(Mission.Instance.CurrentGameZone.GetEnemy(i).name + " : " + EnemyCoeficient[i]); 
            //    if (EnemyCoeficient[i] <= bestValue)
            //        continue;

            //    best = i;
            //    bestValue = EnemyCoeficient[i];
            //}

            //if (best >= 0)
            //    return enemies[best];

            return null;
        }


        public void CreateOrderDodge()
        {
            if (Owner.BlackBoard.IsOrderAddPossible(AgentOrder.E_OrderType.E_DODGE) == false)
                return;

            TSVector2 rollDir;

            if (this.Owner.CurJoystickDir != TSVector2.zero)
            {
                rollDir = this.Owner.CurJoystickDir;
            }
            else
            {
                rollDir = this.Owner.Forward;
            }
               
            rollDir.Normalize();
            AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_DODGE);
            order.Direction = rollDir;

            Owner.BlackBoard.OrderAdd(order);
            ComboProgress.Clear();
            ClearBufferedOrder();
        }

        void ClearBufferedOrder()
        {
            while (BufferedOrders.Count > 0)
                AgentOrderFactory.Return(BufferedOrders.Dequeue());
        }


        public void CreateOrderGoTo(FP MoveSpeedModifier , FP facing)
        {
            this.force = force;

            if (CouldAddnewOrder() == false)
                return;
            
            AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_GOTO);
            order.Direction = this.Owner.CurJoystickDir;
            order.MoveSpeedModifier = MoveSpeedModifier;
            order.Facing = facing;
            Owner.BlackBoard.OrderAdd(order);
        }

        public void CreateOrderStop()
        {
            this.force = 0;
            AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_STOPMOVE);
            Owner.BlackBoard.OrderAdd(order);
        }

        

        public void Update()
        {
            if (Owner.BlackBoard.Stop)
            {
                LastAttacketTarget = null;
                //ComboProgress.Clear();
                ClearBufferedOrder();
                CreateOrderStop();
                return;
            }

            if (BufferedOrders.Count > 0)
            {
                if (CouldAddnewOrder())
                    Owner.BlackBoard.OrderAdd(BufferedOrders.Dequeue());
            }
        }

        public void HandleAction(AgentAction a)
        {
            if (a is AgentActionAttack)
            {
                CurrentAttackAction = a as AgentActionAttack;
                Owner.WorldState.SetWSProperty(E_PropKey.E_ALERTED, true);
            }
            //else if (a is AgentActionInjury)
            //{
            //    Owner.WorldState.SetWSProperty(E_PropKey.E_ORDER, AgentOrder.E_OrderType.E_NONE);
            //    ComboProgress.Clear();
            //    ClearBufferedOrder();
            //    GuiManager.Instance.ShowComboProgress(ComboProgress);
            //    Game.Instance.Hits = 0;
            //    Game.Instance.NumberOfInjuries++;

            //}
            //else if (a is AgentActionDeath)
            //{
            //    Owner.WorldState.SetWSProperty(E_PropKey.E_ORDER, AgentOrder.E_OrderType.E_NONE);
            //    ComboProgress.Clear();
            //    ClearBufferedOrder();
            //    GuiManager.Instance.ShowComboProgress(ComboProgress);
            //    Game.Instance.Hits = 0;
            //    Game.Instance.NumberOfDeath++;
            //    //Game.Instance.Score -= 50;
            //    Mission.Instance.EndOfMission(false);
            //    // of	unlockAchievement
            //    if (Game.Instance.NumberOfDeath >= 100)
            //    {
            //        Achievements.UnlockAchievement(2);
            //    }
            //    else if (Game.Instance.NumberOfDeath >= 50)
            //    {
            //        Achievements.UnlockAchievement(1);
            //    }
            //}
        }
    }
}
