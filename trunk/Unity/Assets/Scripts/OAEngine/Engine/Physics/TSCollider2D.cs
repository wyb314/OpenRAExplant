using System;
using System.Collections.Generic;
using TrueSync;
using TrueSync.Physics2D;

namespace Engine.Physics
{
    using World = TrueSync.Physics2D.World;
    public abstract class TSCollider2D : AbstractPhysicComponent,ICollider
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

        /**
         *  @brief Simulated material. 
         **/
        public TSMaterial tsMaterial;
        

        private TSVector2 center;

        //private Vector3 scaledCenter;

        internal Body _body;

        /**
         *  @brief Returns {@link RigidBody} associated to this TSRigidBody.         
         **/
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

        /**
         *  @brief Center of the collider shape.
         **/
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

        /**
         *  @brief Returns a version of collider's center scaled by parent's transform.
         */
        public TSVector2 ScaledCenter
        {
            get
            {
                return TSVector2.Scale(center, new TSVector2(lossyScale.x, lossyScale.y));
            }
        }

        /**
         *  @brief Creates the shape related to a concrete implementation of TSCollider.
         **/
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

        /**
         *  @brief Returns the {@link TSRigidBody} attached.
         */
        public TSRigidBody2D attachedRigidbody
        {
            get
            {
                return tsRigidBody;
            }
        }

        /**
         *  @brief Holds an first value of the GameObject's lossy scale.
         **/
        protected TSVector lossyScale = TSVector.one;

        /**
         *  @brief Creates a new {@link TSRigidBody} when there is no one attached to this GameObject.
         **/
        public void Init()
        {
            tsTransform = this.GetComponent<TSTransform2D>();
            tsRigidBody = this.GetComponent<TSRigidBody2D>();

            //if (lossyScale == TSVector.one)//规定当前游戏中的刚体不缩放
            //{
            //    lossyScale = TSVector.Abs(transform.localScale.ToTSVector());
            //}
        }

        public void Update()
        {
            //if (!Application.isPlaying)
            //{
            //    lossyScale = TSVector.Abs(transform.lossyScale.ToTSVector());
            //}
        }

        private void CreateBodyFixture(Body body, Shape shape)
        {
            Fixture fixture = body.CreateFixture(shape);

            if (tsMaterial != null)
            {
                fixture.Friction = tsMaterial.friction;
                fixture.Restitution = tsMaterial.restitution;
            }
        }

        private void CreateBody(World world)
        {
            Body body = BodyFactory.CreateBody(world);

            if (tsMaterial == null)
            {
                tsMaterial = GetComponent<TSMaterial>();
            }

            Shape shape = Shape;
            if (shape != null)
            {
                CreateBodyFixture(body, shape);
            }
            else
            {
                Shape[] shapes = CreateShapes();
                for (int index = 0, length = shapes.Length; index < length; index++)
                {
                    CreateBodyFixture(body, shapes[index]);
                }
            }

            if (tsRigidBody == null)
            {
                body.BodyType = BodyType.Static;
            }
            else
            {
                if (tsRigidBody.isKinematic)
                {
                    body.BodyType = TrueSync.Physics2D.BodyType.Kinematic;
                }
                else
                {
                    body.BodyType = BodyType.Dynamic;
                    body.IgnoreGravity = !tsRigidBody.useGravity;
                }

                if (tsRigidBody.mass <= 0)
                {
                    tsRigidBody.mass = 1;
                }

                body.FixedRotation = tsRigidBody.freezeZAxis;
                body.Mass = tsRigidBody.mass;
                body.TSLinearDrag = tsRigidBody.drag;
                body.TSAngularDrag = tsRigidBody.angularDrag;
            }

            body.IsSensor = isTrigger;
            body.CollidesWith = Category.All;

            _body = body;
        }

        /**
         *  @brief Initializes Shape and RigidBody and sets initial values to position and orientation based on Unity's transform.
         **/
        public void Initialize(World world)
        {
            CreateBody(world);
        }

        private void CheckPhysics()
        {
            if (_body == null && PhysicsManager.instance != null)
            {
                PhysicsManager.instance.AddBody(this);
            }
        }

       
        /**
         *  @brief Draws the specific gizmos of concrete collider (for example "Gizmos.DrawWireCube" for a {@link TSBoxCollider}).
         **/
        protected abstract void DrawGizmos();

        /**
         *  @brief Returns true if the body was already initialized.
         **/
        public bool IsBodyInitialized
        {
            get
            {
                return _body != null;
            }
        }

    }
}
