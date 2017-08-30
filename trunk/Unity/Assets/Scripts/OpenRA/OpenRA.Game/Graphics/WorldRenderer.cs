using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Effects;
using OpenRA.Traits;

namespace OpenRA.Graphics
{
    public sealed class WorldRenderer : IDisposable
    {
        public readonly World World;

        public Viewport Viewport { get; private set; }

        internal WorldRenderer(ModData modData, World world)
        {
            World = world;
            //TileSize = World.Map.Grid.TileSize;
            //TileScale = World.Map.Grid.Type == MapGridType.RectangularIsometric ? 1448 : 1024;
            //Viewport = new Viewport(this, world.Map);

            //createPaletteReference = CreatePaletteReference;

            //var mapGrid = modData.Manifest.Get<MapGrid>();
            //enableDepthBuffer = mapGrid.EnableDepthBuffer;

            //foreach (var pal in world.TraitDict.ActorsWithTrait<ILoadsPalettes>())
            //    pal.Trait.LoadPalettes(this);

            //foreach (var p in world.Players)
            //    UpdatePalettesForPlayer(p.InternalName, p.Color, false);

            //palette.Initialize();

            //Theater = new Theater(world.Map.Rules.TileSet);
            //terrainRenderer = new TerrainRenderer(world, this);

            //debugVis = Exts.Lazy(() => world.WorldActor.TraitOrDefault<DebugVisualizations>());
        }

        public void Draw()
        {
        }


        public readonly Size TileSize;
        public readonly int TileScale;
        public float2 ScreenPosition(WPos pos)
        {
            return new float2((float)TileSize.Width * pos.X / TileScale, (float)TileSize.Height * (pos.Y - pos.Z) / TileScale);
        }

        public int2 ScreenPxPosition(WPos pos)
        {
            // Round to nearest pixel
            var px = ScreenPosition(pos);
            return new int2((int)Math.Round(px.X), (int)Math.Round(px.Y));
        }


        public void Dispose()
        {
            // HACK: Disposing the world from here violates ownership
            // but the WorldRenderer lifetime matches the disposal
            // behavior we want for the world, and the root object setup
            // is so horrible that doing it properly would be a giant mess.
            //World.Dispose();

            //palette.Dispose();
            //Theater.Dispose();
            //terrainRenderer.Dispose();
        }
    }
}
