// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Rulesets.Catch.Objects;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Catch.Mods
{
    public class CatchModBarrelRoll : ModBarrelRoll<CatchHitObject>
    {
        public override void Update(Playfield playfield)
        {
            //TODO : it is not necessary to do this on every frames
            playfield.Parent.X = 0;
            playfield.Parent.Y = 0;
            playfield.Parent.Origin = Anchor.Centre;
            playfield.Parent.Anchor = Anchor.Centre;

            base.Update(playfield);
        }
    }
}
