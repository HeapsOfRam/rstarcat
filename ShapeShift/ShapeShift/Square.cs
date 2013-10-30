using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeShift
{
    class Square : Shape
    {
        private Texture2D squareTexture;
        
        private SpriteSheetAnimation idleAnimation;

        public Square(ContentManager content)
        {
            X_OFFSET = 24;
            Y_OFFSET = 24;
            animations = new List<SpriteSheetAnimation>();

            squareTexture = content.Load<Texture2D>("SquareIdleSpriteSheet");

            idleAnimation = new SpriteSheetAnimation(this,true);
            idleAnimation.LoadContent(content, squareTexture, "", new Vector2(0, 0));
            idleAnimation.IsEnabled = true;

            animations.Add(idleAnimation);
        }

        public void attack()
        {
        }

        public override Texture2D getTexture()
        {
            return squareTexture;
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
