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

        public WPos Pos;

        public int MoveSpeed = 3000;

        public int Facing;

        public int TurnSpeed = 4;

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
            
        }

        public void Init()
        {
            this.Agent = new PlayerAgent(this,this.render);
            this.Agent.Init();
            Animation anim = new Animation(this.render);
            this.AnimComponent = new AnimComponent(this.Agent,anim);
            this.AnimComponent.Init();
            this.ComponentPlayer = new ComponentPlayer(this.Agent);
            this.ComponentPlayer.Init();
        }


        public void Tick()
        {
            this.Agent.Tick();
            this.AnimComponent.Update();
            this.ComponentPlayer.Update();
        }

        public void SetMoveDir(byte angle)
        {
            this.Facing = angle;
            //this.Pos += this.MoveSpeed*new CVec();
        }

        public void ProcessOrder(IOrder order)
        {
            Order _order = order as Order;
            E_OpType opType = (E_OpType)_order.OpCode;
            
            switch (opType)
            {
                case E_OpType.X:
                    //this.ComponentPlayer.CreateOrderAttack(E_AttackType.X);
                    break;
                case E_OpType.O:
                    //this.ComponentPlayer.CreateOrderAttack(E_AttackType.O);
                    break;
                case E_OpType.Dodge:
                    //this.ComponentPlayer.CreateOrderDodge();
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
