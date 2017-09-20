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
                TryToIssureOrder(world, E_OpType.X);
            }
            if (this.inputer.GetButtonDown("O"))
            {
                TryToIssureOrder(world, E_OpType.O);
            }
            if (this.inputer.GetButtonDown("Dodge"))
            {
                TryToIssureOrder(world, E_OpType.Dodge);
            }

            this.GetJoystickInput(world);
            
        }


        
        private void TryToIssureOrder(IWorld world, E_OpType opType  , byte opData = 0)
        {
            //TODO:
            
            world.IssureOrder(ORDER.ButtonDown((byte)opType,opData));
        }

        private void GetJoystickInput(IWorld world)
        {
            float h = this.inputer.GetAxis("Horizontal");
            float v = this.inputer.GetAxis("Vertical");

            float sqrLength = h*h + v*v;
            if (sqrLength > 0)
            {
                float angle = MathUtils.Atan2(v,h) * MathUtils.Rad2Deg;

                byte opData = (byte)(MathUtils.Min(MathUtils.RoundToInt(angle / ControllerConst.DEG_PER_BYTE) - 1,byte.MaxValue));

                this.TryToIssureOrder(world,E_OpType.Joystick, opData);
            }
        }

    }
}
