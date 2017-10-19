using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueSync;

namespace Engine.Physics
{
    public class SampleWall
    {
        public TSTransform2D tran { private set; get; }

        public TSBoxCollider2D collider { private set; get; }


        public SampleWall(TSVector2 pos , TSVector2 size)
        {
            this.tran = new TSTransform2D();
            this.tran = new TSTransform2D();
            this.collider = new TSBoxCollider2D();

            this.tran.position = pos;
        }
    }
}
