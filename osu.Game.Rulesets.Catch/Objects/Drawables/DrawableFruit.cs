// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Catch.Skinning.Default;
using osu.Game.Rulesets.Scoring;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Catch.Objects.Drawables
{
    public partial class DrawableFruit : DrawablePalpableCatchHitObject, IKeyBindingHandler<CatchAction>
    {
        public DrawableFruit()
            : this(null)
        {
        }

        public DrawableFruit(Fruit? h)
            : base(h)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            ScalingContainer.Child = new SkinnableDrawable(
                new CatchSkinComponentLookup(CatchSkinComponents.Fruit),
                _ => new FruitPiece());
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (CheckPosition == null) return;

            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyMinResult();
                return;
            }

            if (Result != null)
            {
                var result = HitObject.HitWindows.ResultFor(timeOffset);
                if (result == HitResult.None)
                    return;

                if (CheckPosition.Invoke(HitObject))
                {
                    ApplyResult(result);
                }
                else
                    ApplyMinResult(); // Maybe do nothing? right now that means that if we press in the time window without having the catcher at the correct X position, we count as a miss
            }
        }

        public bool OnPressed(KeyBindingPressEvent<CatchAction> e)
        {
            if (e.Action != CatchAction.HitFruit1 && e.Action != CatchAction.HitFruit2) return false;

            if (Judged)
                return false;

            // Only count this as handled if the new judgement is a hit
            bool result = UpdateResult(true);
            return result;
        }

        public void OnReleased(KeyBindingReleaseEvent<CatchAction> e)
        {
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            // Important to have this in UpdateInitialTransforms() to it is re-triggered by RefreshStateTransforms().
            ScalingContainer.Rotation = (RandomSingle(1) - 0.5f) * 40;
        }
    }
}
