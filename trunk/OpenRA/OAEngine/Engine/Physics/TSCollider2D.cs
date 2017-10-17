using System;
using System.Collections.Generic;
using TrueSync;
using TrueSync.Physics2D;

namespace Engine.Physics
{
    public abstract class TSCollider2D : ICollider
    {
        private Shape shape;
       
        public Shape Shape
        {
            get
            {
                if (shape == null)
                    shape = CreateShape();
                return shape;
            }
            protected set { shape = value; }
        }

        private bool _isTrigger;
        
        public bool isTrigger
        {

            get
            {
                if (_body != null)
                {
                    return _body.IsSensor;
                }

                return _isTrigger;
            }

            set
            {
                _isTrigger = value;

                if (_body != null)
                {
                    _body.IsSensor = value;
                }
            }

        }

        public TSMaterial tsMaterial;
        
        private TSVector2 center;

        private Vector3 scaledCenter;

        internal Body _body;


    }
}
