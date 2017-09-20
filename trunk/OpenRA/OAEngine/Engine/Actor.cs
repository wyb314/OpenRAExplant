using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Interfaces;
using Engine.Primitives;

namespace Engine
{
    public class Actor
    {

        public CPos Pos;

        public int MoveSpeed = 3000;

        public int Rot;

        public readonly World World;

        public IRender render;

        public Actor(World world, string name, TypeDictionary initDict)
        {
            this.World = world;
            this.render = Platform.platformInfo.actorRendererFactory.CreateActorRenderer();
        }

        public void Tick()
        {
        }

        public void SetMoveDir(byte angle)
        {
            
            this.Pos += this.MoveSpeed*new CVec();
        }

        public void RenderSelf(IWorldRenderer worldRenderer)
        {
            if (this.render != null)
            {
                this.render.Render(this,worldRenderer);
            }
        }

    }
}
