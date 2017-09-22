using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Inputs
{
    public interface IInputsGetter
    {
        float GetAxis(string axisName);

        bool GetButtonDown(string buttonName);
    }
}
