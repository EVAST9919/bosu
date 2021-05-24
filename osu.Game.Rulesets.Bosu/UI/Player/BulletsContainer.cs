using osu.Framework.Allocation;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics;
using System;
using osu.Framework.Graphics.OpenGL.Vertices;
using osuTK;
using System.Collections.Generic;
using osu.Game.Rulesets.Bosu.Extensions;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Primitives;

namespace osu.Game.Rulesets.Bosu.UI.Player
{
    public class BulletsContainer : Sprite
    {
        private readonly List<Bullet> bullets = new List<Bullet>();

        private double currentTime;

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            RelativeSizeAxes = Axes.Both;
            Texture = textures.Get("bullet");
        }

        public void Add(Vector2 position, bool rightwards)
        {
            foreach (var b in bullets)
            {
                if (!b.IsValid)
                {
                    b.Reuse(position, rightwards, Time.Current);
                    return;
                }
            }

            bullets.Add(new Bullet(position, rightwards, Time.Current));
        }

        protected override void Update()
        {
            base.Update();

            currentTime = Time.Current;

            foreach (var b in bullets)
            {
                if (currentTime < b.StartTime || currentTime > b.EndTime)
                    b.IsValid = false;
            }

            Invalidate(Invalidation.DrawNode);
        }

        protected override DrawNode CreateDrawNode() => new BulletsDrawNote(this);

        private class BulletsDrawNote : SpriteDrawNode
        {
            protected new BulletsContainer Source => (BulletsContainer)base.Source;

            private readonly List<Bullet> bullets = new List<Bullet>();
            private double currentTime;

            public BulletsDrawNote(BulletsContainer source)
                : base(source)
            {
            }

            public override void ApplyState()
            {
                base.ApplyState();

                currentTime = Source.currentTime;

                bullets.Clear();
                bullets.AddRange(Source.bullets);
            }

            protected override void Blit(Action<TexturedVertex2D> vertexAction)
            {
                foreach (var b in bullets)
                {
                    if (!b.IsValid)
                        continue;

                    var position = Vector2.Lerp(b.InitialPosition, b.EndPosition, (float)((currentTime - b.StartTime) / (b.EndTime - b.StartTime)));
                    var rect = getBulletRectangle(position, IWannaExtensions.BULLET_SIZE);
                    var quad = getQuad(rect);

                    DrawQuad(
                        Texture,
                        quad,
                        DrawColourInfo.Colour,
                        null,
                        vertexAction);
                }
            }

            private Quad getQuad(RectangleF rect) => new Quad(
                Vector2Extensions.Transform(rect.TopLeft, DrawInfo.Matrix),
                Vector2Extensions.Transform(rect.TopRight, DrawInfo.Matrix),
                Vector2Extensions.Transform(rect.BottomLeft, DrawInfo.Matrix),
                Vector2Extensions.Transform(rect.BottomRight, DrawInfo.Matrix)
            );

            private static RectangleF getBulletRectangle(Vector2 position, float size) => new RectangleF(
                position.X - size / 2,
                position.Y - size / 2,
                size,
                size);
        }

        private class Bullet
        {
            public Vector2 InitialPosition;
            public Vector2 EndPosition;
            public double StartTime;
            public double EndTime;
            public bool IsValid;

            public Bullet(Vector2 initialPosition, bool rightwards, double startTime)
            {
                Reuse(initialPosition, rightwards, startTime);
            }

            public void Reuse(Vector2 initialPosition, bool rightwards, double startTime)
            {
                InitialPosition = initialPosition;
                EndPosition = new Vector2(rightwards ? BosuPlayfield.BASE_SIZE.X - IWannaExtensions.TILE_SIZE : IWannaExtensions.TILE_SIZE, initialPosition.Y);
                StartTime = startTime;

                var dst = Math.Abs(initialPosition.X - EndPosition.X);
                var duration = IWannaExtensions.BULLET_SPEED / 20f * dst;
                EndTime = startTime + duration;

                IsValid = true;
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            bullets.Clear();
            base.Dispose(isDisposing);
        }
    }
}
