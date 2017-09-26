using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI;
using Engine.ComponentsAI.ComponentPlayer;
using Engine.Interfaces;
using Engine.Network.Defaults;
using Engine.Network.Interfaces;
using Engine.Primitives;
using Engine.Support;

namespace Engine
{
    public class Actor
    {

        public CPos Pos;

        public int MoveSpeed = 3000;

        public int Rot;

        public readonly World World;

        public IRender render;

        public readonly uint ActorID;

        private ComponentPlayer ComponentPlayer;
        private AnimComponent AnimComponent;

        private PlayerAgent Agent;


        public Actor(World world, string name, TypeDictionary initDict)
        {
            this.World = world;
            this.render = Platform.platformInfo.actorRendererFactory.CreateActorRenderer();

            ActorID = world.NextAID();
            this.Agent = new PlayerAgent();
            this.Agent.Init();
            this.AnimComponent = new AnimComponent(this.Agent,null);
            this.AnimComponent.Init();
            this.ComponentPlayer = new ComponentPlayer();
        }

        public void Tick()
        {
        }

        public void SetMoveDir(byte angle)
        {
            this.Rot = angle;
            //this.Pos += this.MoveSpeed*new CVec();
        }

        public void ProcessOrder(IOrder order)
        {
            Order _order = order as Order;
            E_OpType opType = (E_OpType)_order.OpCode;
            
            switch (opType)
            {
                case E_OpType.X:
                    this.ComponentPlayer.CreateOrderAttack(E_AttackType.X);
                    break;
                case E_OpType.O:
                    this.ComponentPlayer.CreateOrderAttack(E_AttackType.O);
                    break;
                case E_OpType.Dodge:
                    this.ComponentPlayer.CreateOrderDodge();
                    break;
                case E_OpType.Joystick:
                    ushort data = _order.OpData;
                    int angle = ((data & 0xFF00) >> 8);
                    int force = data & 0x00ff;

                    if (force == 0)
                    {
                        this.ComponentPlayer.CreateOrderStop();
                        //Log.Write("wyb","Joystick end!");
                    }
                    else
                    {
                        this.ComponentPlayer.CreateOrderGoTo(angle,force);
                        //this.SetMoveDir(angle);
                        //Log.Write("wyb", "Joystick angle->" + angle+" force->"+force);
                    }
                    
                    break;
                default:
                    break;
            }
        }


        public void RenderSelf(IWorldRenderer worldRenderer)
        {
            if (this.render != null)
            {
                this.render.Render(this,worldRenderer);
            }
        }

    }
}
