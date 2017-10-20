
using TrueSync;

namespace Engine.Physics
{
    public class TSMaterial : AbstractPhysicComponent
    {
        public FP friction = 0.25f;

        /**
         *  @brief Coeficient of restitution. 
         **/
        public FP restitution = 0;
    }
}
