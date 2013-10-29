using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace ShapeShift
{
    public class Shape
    {
        //Texture2D texture;
        private Rectangle rectangle;

        protected bool collision = false;
        private const int X = 0, Y = 0, SIZE = 50;

        protected int X_OFFSET;
        protected int Y_OFFSET;

        // List of textures/spritesheet images used to animate the shape
        protected List<SpriteSheetAnimation> animations;

        public Shape()
        {
            rectangle = new Rectangle(X, Y, SIZE, SIZE);
        }

        public virtual Texture2D getTexture()
        {
            return null;
        }

        public Rectangle getRectangle()
        { return rectangle; }

        public List<SpriteSheetAnimation> getActiveTextures()
        {
            return animations;
        }


        public virtual void disableAnimation(SpriteSheetAnimation spriteSheetAnimation) { }

        


        internal void setPosition(Vector2 position)
        {
            foreach (SpriteSheetAnimation animation in animations)
                animation.Position = position;
        }

        internal float getXOffSet()
        {
            return X_OFFSET;
        }

        internal float getYOffSet()
        {
            return Y_OFFSET;
        }

        public virtual bool Collides(Vector2 position, Rectangle rect)
        {
            return collision;
        }


    }
}
