using System;
using System.Collections.Generic;
using Engine.Interfaces;
using Engine.OrderGenerators;
using Engine.Support;
using YamlDotNet.Serialization.ValueDeserializers;

namespace Engine.Inputs
{
    public class GameInputter : IInputter
    {
        public IInputsGetter inputGetter { set; get; }

        public IInputPoster inputPoster { private set; get; }

        public bool AllowGetInput
        {
            set; get;
        }

        public GameInputter()
        {
            this.inputPoster = new NormalInputPoster();
        }

        public void Update()
        {
            if (!this.AllowGetInput)
            {
                return;
            }
            if (this.inputGetter.GetButtonDown("X"))
            {
                this.inputPoster.PostInput(E_OpType.X);
            }
            if (this.inputGetter.GetButtonDown("O"))
            {
                this.inputPoster.PostInput(E_OpType.O);
            }
            if (this.inputGetter.GetButtonDown("Dodge"))
            {
                this.inputPoster.PostInput(E_OpType.Dodge);
            }

            this.GetJoystickInput();
        }
        

        private void GetJoystickInput()
        {
            float h = this.inputGetter.GetAxis("Horizontal");
            float v = this.inputGetter.GetAxis("Vertical");

            float sqrLength = h * h + v * v;
            if (sqrLength > 0)
            {
                float rad = MathUtils.Atan2(v, h);

                if (rad < 0)
                {
                    rad = 2*MathUtils.PI + rad;
                }

                int val = (int) (rad*128/MathUtils.PI);

                val = MathUtils.Min(val, byte.MaxValue); //normaized to 0 - 255;
                ushort angle = (ushort) val;

                int length = (int) (256*MathUtils.Sqrt(sqrLength));
                length = MathUtils.Min(length, byte.MaxValue);
                ushort force = (ushort) length;

                ushort data = (ushort) ((angle << 8) | force);

                this.inputPoster.PostInput(E_OpType.Joystick, data);
            }
            else
            {
                this.inputPoster.PostInput(E_OpType.Joystick, 0);
            }
        }
    }

    
}
