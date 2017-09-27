using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Interfaces;

namespace Engine.Inputs
{
    public class NormalInputPoster : IInputPoster
    {
        private bool allowAcceptInput = true;

        public bool AllowAcceptInput
        {
            set { this.allowAcceptInput = value; }
            get { return this.allowAcceptInput; }
        }

        public IInputEvtDispatcher dispatcher { private set; get; }
        
        public NormalInputPoster()
        {
            this.dispatcher = new NormalInputEvtDispatcher();
        }

        public NormalInputPoster(IInputEvtDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public void PostInput(E_OpType type, int val)
        {
            if (!this.allowAcceptInput)
            {
                return;
            }
            this.dispatcher.HandInput(new ControllerEvt(type, val));
        }
        
    }
}
