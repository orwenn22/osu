// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace osu.Game.Beatmaps.Legacy
{
    [Flags]
    internal enum LegacyEffectFlags
    {
        None = 0,
        Kiai = 1,
        OmitFirstBarLine = 8,
        AlternativeScrolling = 16 //if this bit is set : for taiko it enables sequential scrolling, for mania it enables overlapping scrolling
    }
}
