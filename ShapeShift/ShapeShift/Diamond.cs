using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeShift
{
    class Diamond : Shape
    {
        private Texture2D diamondTexture;
        private SpriteSheetAnimation idleAnimation;

        public Diamond(ContentManager content)
        {

            //Distances between image boundries and actual shape
            X_OFFSET = 24;
            Y_OFFSET = 20;

            animations = new List<SpriteSheetAnimation>();
            
            diamondTexture = content.Load<Texture2D>("DiamondIdleSpriteSheet");
            
            idleAnimation = new SpriteSheetAnimation(this,true);
            idleAnimation.LoadContent(content, diamondTexture, "", new Vector2(0, 0));
            idleAnimation.IsEnabled = true;

            animations.Add(idleAnimation);
        }

        public void attack()
        {
        }

        public override Texture2D getTexture()
        {
            return diamondTexture;
        }


        //Checks to see if there is a collision 
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
