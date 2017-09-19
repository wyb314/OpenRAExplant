using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Support;

namespace Engine.Interfaces
{
    public interface ILoadScreen : IDisposable
    {
        void Init(ModData m, Dictionary<string, string> info);
        
        void Display();
        
        bool BeforeLoad();

        void StartGame(Arguments args);
    }
}
