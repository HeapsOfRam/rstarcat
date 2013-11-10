using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeShift
{
    class Octomon : Shape
    {
        private Texture2D octomonTexture;

        private SpriteSheetAnimation idleAnimation;

        public Octomon(ContentManager content)
        {

            X_OFFSET = 24;
            Y_OFFSET = 24;



            animations = new List<SpriteSheetAnimation>();


            octomonTexture = content.Load<Texture2D>("OctomonIdleSpriteSheet");

            idleAnimation = new SpriteSheetAnimation(this, true);
            idleAnimation.LoadContent(content, octomonTexture, "", new Vector2(0, 0));
            idleAnimation.IsEnabled = true;


            animations.Add(idleAnimation);
        }

        public override Texture2D getTexture()
        {
            return octomonTexture;
        }

        public void PreformRotate()
        {
            idleAnimation.PreformRotate();

        }

        public override bool Collides(Vector2 position, Rectangle rectangle)
        {

            if (position.X - X_OFFSET + rectangle.Width * 2 < rectangle.X ||
                position.X + X_OFFSET > rectangle.X + rectangle.Width ||
                position.Y - Y_OFFSET + rectangle.Height * 2 < rectangle.Y ||
                position.Y + Y_OFFSET > rectangle.Y + rectangle.Height)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
