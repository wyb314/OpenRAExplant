using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Maps;
using Engine.Network;
using Engine.Network.Defaults;
using Engine.Network.Interfaces;
using Engine.Support;

namespace Engine
{
    public enum PowerState { Normal, Low, Critical }
    public enum WinState { Undefined, Won, Lost }

    public class Player
    {
        public readonly string InternalName;

        public readonly Actor PlayerActor;

        public World World { get; private set; }

        public readonly int ClientIndex;
        public Player(World world, IClient client)
        {
            this.World = world;

            if (client != null)
            {
                this.ClientIndex = client.Index;
            }
           
            //this.InternalName = pr.Name;

        }

        public void ProcessOrder(IOrder order)
        {
            //Order _order = order.OrderString;
            Order _order = order as Order;
            E_AttackType attackType = (E_AttackType)_order.OpCode;

            Log.Write("ClientIdx ->",
                "Client:{0} opCode->{1} opData->{2} attackType->{3}".
                F(this.ClientIndex,_order.OpCode,_order.OpData,attackType));
            switch (attackType)
            {
                case E_AttackType.X:
                    break;
                case E_AttackType.O:
                    break;
                case E_AttackType.BossBash:
                    break;
                case E_AttackType.Fatality:
                    break;
                case E_AttackType.Counter:
                    break;
                case E_AttackType.Berserk:
                    break;
                case E_AttackType.Max:
                    break;
                default:
                    break;
            }

        }
    }
}
