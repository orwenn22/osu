// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps.ControlPoints;

namespace osu.Game.Screens.Edit.Timing.RowAttributes
{
    public partial class EffectRowAttribute : RowAttribute
    {
        private readonly Bindable<bool> kiaiMode;
        private readonly BindableNumber<double> scrollSpeed;
        private readonly Bindable<EffectControlPointScrollMode> scrollMode;

        private AttributeText kiaiModeBubble = null!;
        private AttributeText text = null!;
        private AttributeProgressBar progressBar = null!;
        private AttributeText scrollModeText = null!;

        [Resolved]
        protected EditorBeatmap Beatmap { get; private set; } = null!;

        public EffectRowAttribute(EffectControlPoint effect)
            : base(effect, "effect")
        {
            kiaiMode = effect.KiaiModeBindable.GetBoundCopy();
            scrollSpeed = effect.ScrollSpeedBindable.GetBoundCopy();
            scrollMode = effect.ScrollModeBindable.GetBoundCopy();
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Content.AddRange(new Drawable[]
            {
                progressBar = new AttributeProgressBar(Point)
                {
                    Current = scrollSpeed,
                },
                text = new AttributeText(Point) { Width = 45 },
                kiaiModeBubble = new AttributeText(Point) { Text = "kiai" },
                scrollModeText = new AttributeText(Point) { Text = "Overlapping" },
            });

            if (!Beatmap.BeatmapInfo.Ruleset.CreateInstance().EditorShowScrollSpeed)
            {
                text.Hide();
                progressBar.Hide();
            }

            kiaiMode.BindValueChanged(enabled => kiaiModeBubble.FadeTo(enabled.NewValue ? 1 : 0), true);
            scrollSpeed.BindValueChanged(_ => updateText(), true);
            scrollMode.BindValueChanged(_ => updateScrollModeText(), true);
        }

        private void updateText() => text.Text = $"{scrollSpeed.Value:n2}x";

        private void updateScrollModeText()
        {
            if ((Beatmap.BeatmapInfo.Ruleset.OnlineID == 1 && scrollMode.Value == EffectControlPointScrollMode.Overlapping)
                || (Beatmap.BeatmapInfo.Ruleset.OnlineID == 3 && scrollMode.Value == EffectControlPointScrollMode.Sequential)
                || (Beatmap.BeatmapInfo.Ruleset.OnlineID != 1 && Beatmap.BeatmapInfo.Ruleset.OnlineID != 3))
            {
                scrollModeText.Hide();
            }
            else
            {
                scrollModeText.Text = scrollMode.Value.ToString();
                scrollModeText.Show();
            }
        }
    }
}
