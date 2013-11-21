using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeShift
{
    class Bullet : Shape
    {
        #region Textures
        protected Texture2D shotTexture;
        protected Texture2D shotHitTexture;
        #endregion

        #region Animations
        protected SpriteSheetAnimation shotAnimation;
        protected SpriteSheetAnimation shotHitAnimation;
        #endregion

        protected float fireAngle;
        protected int frameCounter;

        protected int X_OFFSET;
        protected int Y_OFFSET;

        protected const int WIDTH = 92;
        protected const int HEIGHT = 92;

        protected const int PROJECTILE_SPEED = 10;
        protected const int SWITCH_FRAME = 10;

        public Bullet(ContentManager content, float fireAngle, String shape)
        {

            X_OFFSET = 24;
            Y_OFFSET = 24;

            animations = new List<SpriteSheetAnimation>();
            this.fireAngle = fireAngle; 

            #region Load Textures

            if (shape.Equals("square"))
            {
                shotTexture = content.Load<Texture2D>("Square/SquareShotSpriteSheet");
                shotHitTexture = content.Load<Texture2D>("Square/SquareShotHitSpriteSheet");
            }
            else if (shape.Equals("triangle"))
            {
                shotTexture = content.Load<Texture2D>("Triangle/TriangleShotSpriteSheet");
                shotHitTexture = content.Load<Texture2D>("Triangle/TriangleShotHitSpriteSheet");
            }

            else if (shape.Equals("circle"))
            {
                shotTexture = content.Load<Texture2D>("Circle/CircleShotSpriteSheet");
                shotHitTexture = content.Load<Texture2D>("Circle/CircleShotHitSpriteSheet");
            }

            else
            {
                shotTexture = content.Load<Texture2D>("Diamond/DiamondShotSpriteSheet");
                shotHitTexture = content.Load<Texture2D>("Diamond/DiamondShotHitSpriteSheet");
            }
            #endregion

            #region Create Animations
            shotAnimation = new SpriteSheetAnimation(this, true);
            shotAnimation.LoadContent(content, shotTexture, "", new Vector2(0, 0));
            shotAnimation.IsEnabled = true;

            shotHitAnimation = new SpriteSheetAnimation(this, false);
            shotHitAnimation.LoadContent(content, shotHitTexture, "", new Vector2(0, 0));
            shotHitAnimation.IsEnabled = false;
            #endregion


            animations.Add(shotAnimation);
            animations.Add(shotHitAnimation);

            rotateTowardsFiringAngle(fireAngle);
        }

        public void rotateTowardsFiringAngle(float fireAngle)
        {

            float rotation = (fireAngle * MathHelper.Pi) / 180;

            foreach (SpriteSheetAnimation s in animations)
            {
                s.origin = new Vector2(WIDTH / 2, HEIGHT / 2);
                s.rotation = rotation;
            }
        }



        public void attack()
        {
        }

        public override void hit()
        {
            shotAnimation.IsEnabled = false;
            shotHitAnimation.position.X = shotAnimation.position.X;
            shotHitAnimation.position.Y = shotAnimation.position.Y;
            shotHitAnimation.IsEnabled = true;
                   

        }

        public override Texture2D getTexture()
        {
            return shotTexture;
        }

        
        public override bool collides(Vector2 position, Rectangle rectangle, Color[] Data)
        {
            if (!collision)
            {

                if (!(shotAnimation.position.X - 40 + rectangle.Width * 2 < rectangle.X ||
                   shotAnimation.position.X + 40 > rectangle.X + rectangle.Width ||
                   shotAnimation.position.Y - Y_OFFSET + rectangle.Height * 2 < rectangle.Y ||
                   shotAnimation.position.Y + Y_OFFSET + 10 > rectangle.Y + rectangle.Height))
                {
                    collision = true;
                    return true;
                }

                else
                    return false;
            }
            return false;
        }
        
       
        public override void disableAnimation(SpriteSheetAnimation spriteSheetAnimation)
        {
            spriteSheetAnimation.IsEnabled = false;
        }

        public void Update(GameTime gameTime)
        {
            shotAnimation.Update(gameTime);
            
            if (collision)
                shotHitAnimation.Update(gameTime);

            frameCounter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (frameCounter >= SWITCH_FRAME)
                {
                    frameCounter = 0;

                    if (!shotHitAnimation.IsEnabled)
                    {
                        float newX = (float)(shotAnimation.position.X + PROJECTILE_SPEED * (Math.Cos(MathHelper.ToRadians(fireAngle-90))));
                        float newY = (float)(shotAnimation.position.Y + PROJECTILE_SPEED * (Math.Sin(MathHelper.ToRadians(fireAngle-90))));

                        shotAnimation.position.X = newX;
                        shotAnimation.position.Y = newY;
                    }
                }
            }
  

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            List<SpriteSheetAnimation> animations = getActiveTextures();

            foreach (SpriteSheetAnimation s in animations)
            {
                if (s.IsEnabled)
                    s.Draw(spriteBatch);
            }

        }


    }
}
