﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRA.Traits
{
    public static class SelectableExts
    {
        public static int SelectionPriority(this ActorInfo a)
        {
            var selectableInfo = a.TraitInfoOrDefault<SelectableInfo>();
            return selectableInfo != null ? selectableInfo.Priority : int.MinValue;
        }

        const int PriorityRange = 30;

        public static int SelectionPriority(this Actor a)
        {
            var basePriority = a.Info.TraitInfo<SelectableInfo>().Priority;
            var lp = a.World.LocalPlayer;

            if (a.Owner == lp || lp == null)
                return basePriority;

            switch (lp.Stances[a.Owner])
            {
                case Stance.Ally: return basePriority - PriorityRange;
                case Stance.Neutral: return basePriority - 2 * PriorityRange;
                case Stance.Enemy: return basePriority - 3 * PriorityRange;

                default:
                    throw new InvalidOperationException();
            }
        }

        public static Actor WithHighestSelectionPriority(this IEnumerable<Actor> actors, int2 selectionPixel)
        {
            return actors.MaxByOrDefault(a => CalculateActorSelectionPriority(a.Info, a.Bounds, selectionPixel));
        }

        public static FrozenActor WithHighestSelectionPriority(this IEnumerable<FrozenActor> actors, int2 selectionPixel)
        {
            return actors.MaxByOrDefault(a => CalculateActorSelectionPriority(a.Info, a.Bounds, selectionPixel));
        }

        static long CalculateActorSelectionPriority(ActorInfo info, Rectangle bounds, int2 selectionPixel)
        {
            var centerPixel = new int2(bounds.X, bounds.Y);
            var pixelDistance = (centerPixel - selectionPixel).Length;

            return ((long)-pixelDistance << 32) + info.SelectionPriority();
        }

        static readonly Actor[] NoActors = { };

        public static IEnumerable<Actor> SubsetWithHighestSelectionPriority(this IEnumerable<Actor> actors)
        {
            return actors.GroupBy(x => x.SelectionPriority())
                .OrderByDescending(g => g.Key)
                .Select(g => g.AsEnumerable())
                .DefaultIfEmpty(NoActors)
                .FirstOrDefault();
        }
    }
}