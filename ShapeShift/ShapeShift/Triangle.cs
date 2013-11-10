using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeShift
{
    class Triangle : Shape
    {
        private Texture2D triangleTexture;
        
        private SpriteSheetAnimation idleAnimation;

        public Triangle(ContentManager content)
        {

            X_OFFSET = 24; 
            Y_OFFSET = 24;



            animations = new List<SpriteSheetAnimation>();


            triangleTexture = content.Load<Texture2D>("TriangleIdleSpriteSheet");

            idleAnimation = new SpriteSheetAnimation(this,true, 92, new Vector2 (45,54));
            idleAnimation.LoadContent(content, triangleTexture, "", new Vector2(0, 0));
            idleAnimation.IsEnabled = true;


            animations.Add(idleAnimation);
        }

        public override Texture2D getTexture()
        {
            return triangleTexture;
        }

        public void PreformRotate()
        {
            idleAnimation.PreformRotate(6.0f);
            idleAnimation.origin = new Vector2(45, 54);

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
