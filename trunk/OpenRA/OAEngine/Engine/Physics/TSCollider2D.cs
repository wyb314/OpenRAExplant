using System;
using System.Collections.Generic;
using TrueSync;
using TrueSync.Physics2D;

namespace Engine.Physics
{
    using World = TrueSync.Physics2D.World;
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

        private Vector3 scaledCenter;


        protected TSVector lossyScale = TSVector.one;
        
        public TSVector2 ScaledCenter
        {
            get
            {
                return TSVector2.Scale(center, new TSVector2(lossyScale.x, lossyScale.y));
            }
        }

        internal Body _body;

        public IBody2D Body
        {
            get
            {
                if (_body == null)
                {
                    CheckPhysics();
                }

                return _body;
            }
        }

        private TSVector2 center;
        public TSVector2 Center
        {
            get
            {
                return center;
            }
            set
            {
                center = value;
            }
        }

        private void CheckPhysics()
        {
            if (_body == null && PhysicsManager.instance != null)
            {
                PhysicsManager.instance.AddBody(this);
            }
        }

        public virtual Shape CreateShape()
        {
            return null;
        }

        public virtual Shape[] CreateShapes()
        {
            return new Shape[0];
        }

        private TSRigidBody2D tsRigidBody;
        
        public TSTransform2D tsTransform;

        public TSRigidBody2D attachedRigidbody
        {
            get
            {
                return tsRigidBody;
            }
        }

        public void Awake()
        {

            
        }


        public virtual void Tick()
        {
        }

        public void Initialize(World world)
        {
            CreateBody(world);
        }


    }
}
