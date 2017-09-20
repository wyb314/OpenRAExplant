using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Interfaces;
using Engine.Network.Defaults;
using Engine.Support;
using ORDER = Engine.Network.Defaults.Order;

namespace Engine.OrderGenerators
{
    public sealed class PlayerControllerOrderGenerator : IOrderGenerator
    {

        public IInputsGetter inputer { private set; get; }

        public PlayerControllerOrderGenerator(IInputsGetter inputer)
        {
            this.inputer = inputer;
        }

        public IEnumerable<Order> Order(World world, byte opCode, byte extCode)
        {

            yield break;
        }

        public void Tick(IWorld world)
        {
            if (this.inputer.GetButtonDown("X"))
            {
                CreateOrderAttack(world,E_AttackType.X);
            }
            if (this.inputer.GetButtonDown("O"))
            {
                CreateOrderAttack(world, E_AttackType.X);
            }
            if (this.inputer.GetButtonDown("Dodge"))
            {
                CreateOrderAttack(world, E_AttackType.X);
            }
        }

        private void CreateOrderAttack(IWorld world, E_AttackType type)
        {
            world.IssureOrder(ORDER.ButtonDown((byte)type));
        }
    }
}
