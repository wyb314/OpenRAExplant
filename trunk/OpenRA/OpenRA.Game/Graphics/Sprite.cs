using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRA.Graphics
{
    public class Sprite
    {
        public readonly Rectangle Bounds;
        //public readonly Sheet Sheet;
        //public readonly BlendMode BlendMode;
        //public readonly TextureChannel Channel;
        public readonly float ZRamp;
        public readonly float3 Size;
        public readonly float3 Offset;
        public readonly float3 FractionalOffset;
        public readonly float Top, Left, Bottom, Right;

        //public Sprite(Sheet sheet, Rectangle bounds, TextureChannel channel)
        //    : this(sheet, bounds, 0, float2.Zero, channel) { }

        //public Sprite(Sheet sheet, Rectangle bounds, float zRamp, float3 offset, TextureChannel channel, BlendMode blendMode = BlendMode.Alpha)
        //{
        //    Sheet = sheet;
        //    Bounds = bounds;
        //    Offset = offset;
        //    ZRamp = zRamp;
        //    Channel = channel;
        //    Size = new float3(bounds.Size.Width, bounds.Size.Height, bounds.Size.Height * zRamp);
        //    BlendMode = blendMode;
        //    FractionalOffset = Size.Z != 0 ? offset / Size :
        //        new float3(offset.X / Size.X, offset.Y / Size.Y, 0);

        //    Left = (float)Math.Min(bounds.Left, bounds.Right) / sheet.Size.Width;
        //    Top = (float)Math.Min(bounds.Top, bounds.Bottom) / sheet.Size.Height;
        //    Right = (float)Math.Max(bounds.Left, bounds.Right) / sheet.Size.Width;
        //    Bottom = (float)Math.Max(bounds.Top, bounds.Bottom) / sheet.Size.Height;
        //}
    }
}
