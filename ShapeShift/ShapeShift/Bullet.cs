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
        protected int readyCounter;
        protected int X_OFFSET;
        protected int Y_OFFSET;

        protected Boolean isReady = false;


        protected const int WIDTH = 92;
        protected const int HEIGHT = 92;

        protected const int PROJECTILE_SPEED = 10;
        protected const int SWITCH_FRAME = 5;
        private Texture2D shotShadowTexture;

        private Color[] data;
        private bool dead = false;
        private bool gone = false;

        private Vector2 originalPostion; 

        public Bullet(ContentManager content, float fireAngle, String shape)
        {

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
            readyCounter = 0;

            animations.Add(shotAnimation);
            animations.Add(shotHitAnimation);

            rotateTowardsFiringAngle(fireAngle);

           GraphicsDevice myDevice = GameServices.GetService<GraphicsDevice>();

           RenderTarget2D renderTarget = new RenderTarget2D(myDevice, WIDTH, HEIGHT);
           SpriteBatch spriteBatch = new SpriteBatch(myDevice);

            // Set the render target on the device.
            myDevice.SetRenderTarget(renderTarget);
            myDevice.Clear(Color.Transparent);


         
            spriteBatch.Begin();
            
            shotAnimation.Draw(spriteBatch);
            
            spriteBatch.End();

           
            // Call GetTexture to retrieve the render target data and save it to a texture.
            myDevice.SetRenderTarget((RenderTarget2D)null);
            shotShadowTexture = new Texture2D(myDevice, WIDTH, HEIGHT);
            Color[] con = new Color[WIDTH * HEIGHT];
            renderTarget.GetData<Color>(con);
            shotShadowTexture.SetData<Color>(con);
           
            data = new Color[WIDTH * HEIGHT];
            shotShadowTexture.GetData(data);


            this.colorData = data;
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

        public override bool isDead()
        {
            return dead;
        }

        public Boolean dispose()
        {
            if (!shotHitAnimation.IsEnabled && !shotAnimation.IsEnabled)
                return true;

            return false;
        }

        public void attack()
        {
        }

        public override void hit()
        {
            if (!dead)
            {
                shotAnimation.IsEnabled = false;
                shotHitAnimation.position.X = shotAnimation.position.X;
                shotHitAnimation.position.Y = shotAnimation.position.Y;
                shotHitAnimation.IsEnabled = true;
                dead = true;

            }
        }

        public override Texture2D getTexture()
        {
            return shotTexture;
        }

        
        public override bool collides(Vector2 position, Rectangle rectangle, Color[] Data)
        {
            
                if (!collision && isReady)
                {
                    Rectangle rectangleA = new Rectangle((int)shotAnimation.Position.X, (int)shotAnimation.Position.Y, WIDTH, HEIGHT);


                    if (IntersectPixels(rectangleA, data, rectangle, Data))
                    {
                        collision = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }

            return false;

        }

        public Boolean outOfBounds()
        {
            return shotAnimation.position.X > 690 || shotAnimation.position.X < 0 || shotAnimation.position.Y > 920 || shotAnimation.position.Y < 92;
        }
        
       
        public override void disableAnimation(SpriteSheetAnimation spriteSheetAnimation)
        {
            spriteSheetAnimation.IsEnabled = false;

            if (spriteSheetAnimation == shotHitAnimation)
            {
                gone = true;
                
            }
        }

        public override void Update(GameTime gameTime)
        {
            shotAnimation.Update(gameTime);
            
            if (dead)
                shotHitAnimation.Update(gameTime);

            frameCounter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            readyCounter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (readyCounter > 10)
            {
                isReady = true;
            }

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
            if (!gone)
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
}
