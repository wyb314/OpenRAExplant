using System;
using OpenRA.FileSystem;

namespace OpenRA.Graphics
{

    public interface IModel
    {
        uint Frames { get; }
        uint Sections { get; }

        float[] TransformationMatrix(uint section, uint frame);
        float[] Size { get; }
        float[] Bounds(uint frame);
        ModelRenderData RenderData(uint section);
    }

    public struct ModelRenderData
    {
        public readonly int Start;
        public readonly int Count;
        //public readonly Sheet Sheet;

        public ModelRenderData(int start, int count)
        {
            Start = start;
            Count = count;
            //Sheet = sheet;
        }
    }

    public interface IModelCache : IDisposable
    {
        IModel GetModelSequence(string model, string sequence);
        bool HasModelSequence(string model, string sequence);
        //IVertexBuffer<Vertex> VertexBuffer { get; }
    }
}
