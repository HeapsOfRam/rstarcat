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
        private const int DEFAULT_WIDTH = 92, DEFAULT_HEIGHT = 92;

        // List of textures/spritesheet images used to animate the shape
        protected List<SpriteSheetAnimation> animations;
        protected bool collision = false;

        protected Color[] colorData;
        protected List<Shape> activeBullets;
        protected Texture2D[] heartTextures = new Texture2D[2];
        protected int coolDown;
        protected float coolDownTimer;
        protected int abilityDuration;
        protected float abilityTimer;
        protected bool coolDownStarted;


        protected const int SWITCH_FRAME = 300;
        // Constructor 
        public Shape(){}

        public virtual bool isDead()
        {
            return false;
        }
        public List<SpriteSheetAnimation> getActiveTextures()
        {
            return animations;
        }

        public virtual Rectangle getRectangle() {

            Vector2 position = getActiveTextures()[0].Position;

            return new Rectangle ((int)position.X,(int)position.Y,getWidth(),getHeight());
        }
      
        public virtual void setPosition(Vector2 position)
        {
            foreach (SpriteSheetAnimation animation in animations)
                animation.Position = position;
        }

        public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top    = Math.Max(rectangleA.Top,    rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left   = Math.Max(rectangleA.Left,   rectangleB.Left);
            int right  = Math.Min(rectangleA.Right,  rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) + (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) + (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                        return true; // then an intersection has been found
                }
            }

            // No intersection found
            return false;
        }

        public virtual void Draw(SpriteBatch spriteBatch){}

        public virtual void DrawOnlyIdle(SpriteBatch spriteBatch){}

        public virtual void disableAnimation(SpriteSheetAnimation spriteSheetAnimation) { }

        public virtual void hit() { }

        public virtual bool collides(Vector2 position, Rectangle rect, Color[] data){return collision;}

        public virtual Texture2D getTexture(){return null;}

        public Color[] getColorData(){return colorData;}

        public virtual int getHeight() { return DEFAULT_HEIGHT; }

        public virtual int getWidth() { return DEFAULT_WIDTH; }

        public virtual int getCoolDown() { return coolDown; }

        public virtual float getCoolDownTimer() { return coolDownTimer; }

        public virtual int getAbilityDuration() { return abilityDuration; }


        public void setOrigin(Vector2 origin)
        {
            List <SpriteSheetAnimation> animations = getActiveTextures();

            foreach (SpriteSheetAnimation animation in animations){

                animation.origin = origin;

            }
        }




        public List<Shape> getActiveBullets()
        {
            return activeBullets;
        }


        public virtual void Update(GameTime gameTime)
        {
        }

        public Texture2D[] getHearts()
        {
            return heartTextures;
        }

        public virtual void scaleShape(float scale)
        {
            List<SpriteSheetAnimation> animation = getActiveTextures();

            foreach (SpriteSheetAnimation s in animation)
            {
                s.scale = scale;
                s.origin = new Vector2(46, 46);
            }

          /*  if (getActiveBullets() != null)
            {
                List<Shape> bullets = getActiveBullets();

                foreach (Shape b in bullets)
                {
                    animation = getActiveTextures();
                    animation[0].scale = scale;
                }
            }*/

            
        }
    }
}
