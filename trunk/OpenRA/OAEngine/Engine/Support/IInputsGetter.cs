using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Support
{
    public interface IInputsGetter
    {
        void GetInput(byte opCode);
    }
}
