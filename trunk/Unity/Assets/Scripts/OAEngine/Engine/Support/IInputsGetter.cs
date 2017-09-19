using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Support
{
    public interface IInputsGetter
    {
        float GetAxis(string axisName);

        bool GetButtonDown(string buttonName);
    }
}
