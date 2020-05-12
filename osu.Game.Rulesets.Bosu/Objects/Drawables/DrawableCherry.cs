using osu.Framework.Graphics;
using osuTK;
using osuTK.Graphics;
using System.Collections.Generic;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Game.Rulesets.Bosu.Objects.Drawables.Pieces;
using osu.Framework.Allocation;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Bosu.UI;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Bosu.UI.Objects.Playfield.Player;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Bosu.Configuration;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace osu.Game.Rulesets.Bosu.Objects.Drawables
{
    public abstract class DrawableCherry : DrawableBosuHitObject
    {
        private const int hidden_distance = 70;
        private const int hidden_distance_buffer = 50;

        public bool HiddenApplied;

        protected override Color4 GetComboColour(IReadOnlyList<Color4> comboColours) =>
            comboColours[(HitObject.IndexInBeatmap + 1) % comboColours.Count];

        protected virtual float GetBaseSize() => 25;

        protected virtual bool AffectPlayer() => false;

        protected virtual float GetWallCheckOffset() => 0;

        private readonly Bindable<bool> hitboxEnabed = new Bindable<bool>(false);

        [Resolved(canBeNull: true)]
        private BosuRulesetConfigManager config { get; set; }

        private readonly CherryPiece cherry;
        private readonly Container hitbox;
        private readonly KiaiCherryPiece kiaiCherry;
        private readonly float finalSize;
        private double missTime;

        protected DrawableCherry(Cherry h)
            : base(h)
        {
            Origin = Anchor.Centre;
            Size = new Vector2(GetBaseSize() * MathExtensions.Map(h.CircleSize, 0, 10, 0.2f, 1));
            Position = h.Position;
            Scale = Vector2.Zero;

            finalSize = Size.X;

            if (h.IsKiai)
                AddInternal(kiaiCherry = new KiaiCherryPiece
                {
                    Scale = new Vector2(1.8f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                });

            AddInternal(cherry = new CherryPiece
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
            });

            AddInternal(hitbox = new Container
            {
                Size = new Vector2(finalSize),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = new Circle
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Red,
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            AccentColour.BindValueChanged(accent =>
            {
                if (kiaiCherry != null)
                    kiaiCherry.Colour = accent.NewValue;

                cherry.Colour = accent.NewValue;
            }, true);

            config?.BindWith(BosuRulesetSetting.EnableHitboxes, hitboxEnabed);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            hitboxEnabed.BindValueChanged(enabled => hitbox.Alpha = enabled.NewValue ? 1 : 0, true);
        }

        protected override void UpdateInitialTransforms()
        {
            this.ScaleTo(Vector2.One, HitObject.TimePreempt);

            cherry.Sprite.Delay(HitObject.TimePreempt).Then().FlashColour(Color4.White, 300);
        }

        protected override void Update()
        {
            base.Update();

            if (HiddenApplied)
            {
                var distance = MathExtensions.Distance(Player.PlayerPosition(), Position);

                if (distance > hidden_distance + hidden_distance_buffer)
                {
                    Alpha = 1;
                    return;
                }

                if (distance < hidden_distance)
                {
                    Alpha = 0;
                    return;
                }

                Alpha = MathExtensions.Map((float)distance - hidden_distance, 0, hidden_distance_buffer, 0, 1);
            }
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (timeOffset > 0)
            {
                if (AffectPlayer())
                {
                    if (collidedWithPlayer(Player))
                    {
                        Player.PlayMissAnimation();
                        missTime = timeOffset;
                        ApplyResult(r => r.Type = HitResult.Miss);
                        return;
                    }
                }

                if (timeOffset > GetWallCheckOffset())
                {
                    if (Position.X > BosuPlayfield.BASE_SIZE.X + DrawSize.X / 2f
                    || Position.X < -DrawSize.X / 2f
                    || Position.Y > BosuPlayfield.BASE_SIZE.Y + DrawSize.Y / 2f
                    || Position.Y < -DrawSize.Y / 2f)
                        ApplyResult(r => r.Type = HitResult.Perfect);
                }
            }
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            base.UpdateStateTransforms(state);

            switch (state)
            {
                case ArmedState.Miss:
                    // Check DrawableHitCircle L#168
                    this.Delay(missTime).FadeOut();
                    break;
            }
        }

        private bool collidedWithPlayer(BosuPlayer player)
        {
            var playerPosition = player.PlayerPosition();
            var playerSize = player.PlayerSize();

            var adjustedX = playerPosition.X - playerSize.X / 2;
            var adjustedY = playerPosition.Y - playerSize.Y / 2;

            var adjustedPlayerPosition = new Vector2(adjustedX, adjustedY);

            return MathExtensions.Collided(finalSize / 2, Position, adjustedPlayerPosition, playerSize);
        }
    }
}
