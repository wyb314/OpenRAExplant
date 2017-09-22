

using System.Security.Cryptography.X509Certificates;

namespace Engine.Interfaces
{
    public interface IRender
    {
        void Render(Actor self, IWorldRenderer wr);
    }
}
