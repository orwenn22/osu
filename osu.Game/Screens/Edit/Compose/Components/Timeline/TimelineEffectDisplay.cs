// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Caching;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Screens.Edit.Components.Timelines.Summary.Parts;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.Edit.Compose.Components.Timeline
{
    public partial class TimelineEffectDisplay : TimelinePart<TimelineEffectDisplay.EffectPiece>
    {
        [Resolved]
        private Timeline timeline { get; set; } = null!;

        private (float min, float max) visibleRange = (float.MinValue, float.MaxValue);

        private readonly Cached groupCache = new Cached();

        private ControlPointInfo controlPointInfo = null!;

        protected override void LoadBeatmap(EditorBeatmap beatmap)
        {
            base.LoadBeatmap(beatmap);

            beatmap.ControlPointInfo.ControlPointsChanged += () => groupCache.Invalidate();
            controlPointInfo = beatmap.ControlPointInfo;
        }

        protected override void Update()
        {
            base.Update();

            if (DrawWidth <= 0) return;

            (float, float) newRange = (
                //TODO : replace "5" by actual width
                (ToLocalSpace(timeline.ScreenSpaceDrawQuad.TopLeft).X - 5) / DrawWidth * Content.RelativeChildSize.X,
                (ToLocalSpace(timeline.ScreenSpaceDrawQuad.TopRight).X + 5) / DrawWidth * Content.RelativeChildSize.X);

            if (visibleRange != newRange)
            {
                visibleRange = newRange;
                groupCache.Invalidate();
            }

            if (!groupCache.IsValid)
            {
                recreateDrawableGroups();
                groupCache.Validate();
            }
        }

        private void recreateDrawableGroups()
        {
            // Remove groups outside the visible range (or timing points which have since been removed from the beatmap).
            foreach (EffectPiece drawableGroup in this)
            {
                if (!controlPointInfo.EffectPoints.Contains(drawableGroup.Effect) || !shouldBeVisible(drawableGroup.Effect))
                    drawableGroup.Expire();
            }

            // Add remaining / new ones.
            foreach (EffectControlPoint e in controlPointInfo.EffectPoints)
                attemptAddEffectPoint(e);
        }

        private void attemptAddEffectPoint(EffectControlPoint effect)
        {
            if (!shouldBeVisible(effect))
                return;

            foreach (var child in this)
            {
                if (ReferenceEquals(child.Effect, effect))
                    return;
            }

            Add(new EffectPiece(effect));
        }

        private bool shouldBeVisible(EffectControlPoint point) => point.Time >= visibleRange.min && point.Time <= visibleRange.max;

        public partial class EffectPiece : HitObjectPointPiece, IHasPopover
        {
            [Resolved]
            private Editor? editor { get; set; }

            public EffectControlPoint Effect;

            private readonly Bindable<bool> contracted = new Bindable<bool>();

            public EffectPiece(EffectControlPoint effect)
            {
                RelativePositionAxes = Axes.X; //required for zooming to work

                Origin = Anchor.TopCentre;

                Effect = effect;
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                Label.AllowMultiline = false;
                LabelContainer.AutoSizeAxes = Axes.None;
                UpdateText();

                //if (editor != null)
                //    editor.ShowSampleEditPopoverRequested += onShowSampleEditPopoverRequested;
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                contracted.BindValueChanged(v =>
                {
                    if (v.NewValue)
                    {
                        Label.FadeOut(200, Easing.OutQuint);
                        LabelContainer.ResizeTo(new Vector2(12), 200, Easing.OutQuint);
                        LabelContainer.CornerRadius = 6;
                    }
                    else
                    {
                        Label.FadeIn(200, Easing.OutQuint);
                        LabelContainer.ResizeTo(new Vector2(Label.Width, 16), 200, Easing.OutQuint);
                        LabelContainer.CornerRadius = 8;
                    }
                }, true);

                FinishTransforms();
            }

            public void UpdateText()
            {
                if (Effect.KiaiMode) { Label.Text = $"K x{Effect.ScrollSpeed}"; }
                else { Label.Text = $" x{Effect.ScrollSpeed}"; }

                if (!contracted.Value)
                    LabelContainer.ResizeWidthTo(Label.Width, 200, Easing.OutQuint);
            }

            protected override Color4 GetRepresentingColour(OsuColour colours)
            {
                return colours.Yellow;
            }

            protected override void Update()
            {
                base.Update();
                X = (float)Effect.Time;
            }

            protected override bool OnClick(ClickEvent e)
            {
                this.ShowPopover();
                return true;
            }

            public virtual Popover GetPopover() => new EffectControlPointEditPopover(Effect, this);
        }

        public partial class EffectControlPointEditPopover : OsuPopover
        {
            private bool isRebinding;
            private readonly EffectControlPoint point;
            private readonly EffectPiece effectPiece;
            private LabelledSwitchButton kiai = null!;
            private SliderWithTextBoxInput<double> scrollSpeedSlider = null!;

            [Resolved]
            protected EditorBeatmap Beatmap { get; private set; } = null!;

            [Resolved]
            protected IEditorChangeHandler? ChangeHandler { get; private set; }

            public EffectControlPointEditPopover(EffectControlPoint point, EffectPiece effectPiece)
            {
                this.point = point;
                this.effectPiece = effectPiece;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        Width = 200,
                        Direction = FillDirection.Vertical,
                        AutoSizeAxes = Axes.Y,
                        Spacing = new Vector2(0, 10),
                        Children = new Drawable[]
                        {
                            new OsuTextFlowContainer(s => s.Font = OsuFont.GetFont(size: 20, weight: FontWeight.Bold, italics: true))
                            {
                                Text = "Effect",
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                            },
                            kiai = new LabelledSwitchButton
                            {
                                Label = "Kiai Time",
                                Current = point.KiaiModeBindable,
                            },
                            scrollSpeedSlider = new SliderWithTextBoxInput<double>("Scroll Speed")
                            {
                                Current = new EffectControlPoint().ScrollSpeedBindable,
                                KeyboardStep = 0.1f
                            }
                        }
                    }
                };
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                //This is done to imitate the behaviour of EffectSection
                SetEffectControlPoint();

                kiai.Current.BindValueChanged(_ => saveChanges());
                scrollSpeedSlider.Current.BindValueChanged(_ => saveChanges());

                if (!Beatmap.BeatmapInfo.Ruleset.CreateInstance().EditorShowScrollSpeed)
                    scrollSpeedSlider.Hide();

                void saveChanges()
                {
                    effectPiece.UpdateText();
                    if (!isRebinding) ChangeHandler?.SaveState();
                }
            }

            protected void SetEffectControlPoint()
            {
                scrollSpeedSlider.Current.ValueChanged -= updateControlPointFromSlider;

                isRebinding = true;

                kiai.Current = point.KiaiModeBindable;
                scrollSpeedSlider.Current = new BindableDouble(1)
                {
                    MinValue = 0.01,
                    MaxValue = 10,
                    Precision = 0.01,
                    Value = point.ScrollSpeedBindable.Value
                };
                scrollSpeedSlider.Current.ValueChanged += updateControlPointFromSlider;
                // at this point in time the above is enough to keep the slider control in sync with reality,
                // since undo/redo causes `OnControlPointChanged()` to fire.
                // whenever that stops being the case, or there is a possibility that the scroll speed could be changed
                // by something else other than this control, this code should probably be revisited to have a binding in the other direction, too.

                isRebinding = false;
            }

            private void updateControlPointFromSlider(ValueChangedEvent<double> scrollSpeed)
            {
                if (isRebinding)
                    return;

                point.ScrollSpeedBindable.Value = scrollSpeed.NewValue;
            }
        }
    }
}
