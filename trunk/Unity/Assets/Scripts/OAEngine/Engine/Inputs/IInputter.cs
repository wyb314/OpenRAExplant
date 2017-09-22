using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Inputs
{
    public interface IInputter
    {
        bool AllowGetInput { set; get; }

        IInputsGetter inputGetter { set; get; }

        IInputPoster inputPoster { get; }

        void Update();
    }
}
