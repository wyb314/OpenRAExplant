using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Interfaces;
using Engine.Network.Defaults;
using Engine.Support;

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
            }
            if (this.inputer.GetButtonDown("O"))
            {
            }
            if (this.inputer.GetButtonDown("Doge"))
            {
            }
        }
    }
}
