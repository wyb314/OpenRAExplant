using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentsAI.AStarMachine;
using TrueSyncPhysics;

namespace OAEngine.Engine.Physics
{
    public interface IPhysObjectAgent
    {

        IRegidbodyWrapObject RendererObject { get; }
        T GetComponent<T>() where T : class, IAgentComponent;


    }
}
