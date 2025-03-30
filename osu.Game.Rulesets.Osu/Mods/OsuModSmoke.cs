// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Osu.Mods
{
    public partial class OsuModSmoke : Mod, IUpdatableByPlayfield, IApplicableToDrawableRuleset<OsuHitObject>
    {
        public override string Name => "Smoke";
        public override string Acronym => "SM";
        public override ModType Type => ModType.Fun;
        public override LocalisableString Description => "Smoke is always enabled";
        public override double ScoreMultiplier => 1;
        public override bool Ranked => false;

        private bool needSegmentEnd = false;

        public void Update(Playfield playfield)
        {
            SmokeContainer smoke = ((OsuPlayfield)playfield).Smoke;

            if (needSegmentEnd)
            {
                smoke.EndSegment();
                needSegmentEnd = false;
            }

            smoke.AddSegment();
        }

        public void ApplyToDrawableRuleset(DrawableRuleset<OsuHitObject> drawableRuleset)
        {
            ((DrawableOsuRuleset)drawableRuleset).KeyBindingInputManager.Add(new InputInterceptor(this));
        }

        public bool OnPressed(KeyBindingPressEvent<OsuAction> e)
        {
            if(e.Action == OsuAction.Smoke) return true; //block smoke

            if (e.Action == OsuAction.LeftButton || e.Action == OsuAction.RightButton)
            {
                needSegmentEnd = true;
                return false;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<OsuAction> e)
        {
        }

        private partial class InputInterceptor : Component, IKeyBindingHandler<OsuAction>
        {
            private readonly OsuModSmoke mod;

            public InputInterceptor(OsuModSmoke mod)
            {
                this.mod = mod;
            }

            public bool OnPressed(KeyBindingPressEvent<OsuAction> e)
            {
                return mod.OnPressed(e);
            }

            public void OnReleased(KeyBindingReleaseEvent<OsuAction> e)
            {
                mod.OnReleased(e);
            }
        }
    }
}
