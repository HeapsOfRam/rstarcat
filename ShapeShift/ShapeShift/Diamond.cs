﻿using System;
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
        #region Textures
        private Texture2D diamondTexture;
        private Texture2D diamondDeployMineTexture;
        private Texture2D diamondMineIdleTexture;
        private Texture2D diamondShadowTexture;
        private Texture2D diamondDeployTurretTexture;
        private Texture2D diamondTurretIdleTexture;
        private Texture2D diamondHitTexture;
        private Texture2D diamondMineShadowTexture;
        #endregion

        #region Animations
        private SpriteSheetAnimation idleAnimation;
        private SpriteSheetAnimation deployMineAnimation;
        private SpriteSheetAnimation diamondMineIdleAnimation;
        private SpriteSheetAnimation diamondShadowAnimation;
        private SpriteSheetAnimation diamondTurretAnimation;
        private SpriteSheetAnimation diamondTurretIdleAnimation;
        private SpriteSheetAnimation diamondHitAnimation;
        private SpriteSheetAnimation diamondMineShadowAnimation;
        #endregion

        private Boolean droppedMine   = false;
        private Boolean droppedTurret = false;

        protected int frameCounter;



        public Boolean firing = false;

        private const int WIDTH  = 92;
        private const int HEIGHT = 92;
        private ContentManager content;

        public Diamond(ContentManager content)
        {
            this.content = content; 
            animations = new List<SpriteSheetAnimation>();
            activeBullets = new List<Shape>();

            heartTextures = new Texture2D[2];
            heartTextures[0] = content.Load<Texture2D>("Diamond/heart");
            heartTextures[1] = content.Load<Texture2D>("Diamond/heartEmpty");


            #region Load Textures
            diamondTexture = content.Load<Texture2D>("Diamond/DiamondIdleSpriteSheet");
            diamondDeployMineTexture = content.Load<Texture2D>("Diamond/DiamondMineSpriteSheet");
            diamondMineIdleTexture = content.Load<Texture2D>("Diamond/DiamondMineIdleSpriteSheet");
            diamondShadowTexture = content.Load<Texture2D>("Diamond/DiamondShadow");
            diamondTurretIdleTexture = content.Load<Texture2D>("Diamond/DiamondTurretIdleSpriteSheet");
            diamondDeployTurretTexture = content.Load<Texture2D>("Diamond/DiamondTurretSpriteSheet");
            diamondHitTexture = content.Load<Texture2D>("Diamond/DiamondHitSpriteSheet");
            #endregion

            #region Create Animations
            idleAnimation = new SpriteSheetAnimation(this,true);
            idleAnimation.LoadContent(content, diamondTexture, "", new Vector2(0, 0));
            idleAnimation.IsEnabled = true;

            diamondMineShadowAnimation = new SpriteSheetAnimation(this, false);
            diamondMineShadowAnimation.LoadContent(content, diamondShadowTexture, "", new Vector2(0, 0));
            diamondMineShadowAnimation.IsEnabled = true;

            deployMineAnimation = new SpriteSheetAnimation(this, false);
            deployMineAnimation.LoadContent(content, diamondDeployMineTexture, "", new Vector2(0, 0));
            deployMineAnimation.IsEnabled = false;

            diamondMineIdleAnimation = new SpriteSheetAnimation(this, true);
            diamondMineIdleAnimation.LoadContent(content, diamondMineIdleTexture, "", new Vector2(0, 0));
            diamondMineIdleAnimation.IsEnabled = false;

            diamondShadowAnimation = new SpriteSheetAnimation(this, true);
            diamondShadowAnimation.LoadContent(content, diamondShadowTexture, "", new Vector2(0, 0));
            diamondShadowAnimation.IsEnabled = false;

            diamondTurretAnimation = new SpriteSheetAnimation(this, false);
            diamondTurretAnimation.LoadContent(content, diamondDeployTurretTexture, "", new Vector2(0, 0));
            diamondTurretAnimation.IsEnabled = false;

            diamondTurretIdleAnimation = new SpriteSheetAnimation(this, true);
            diamondTurretIdleAnimation.LoadContent(content, diamondTurretIdleTexture, "", new Vector2(0, 0));
            diamondTurretIdleAnimation.IsEnabled = false;

            diamondHitAnimation = new SpriteSheetAnimation(this, false);
            diamondHitAnimation.LoadContent(content, diamondHitTexture, "", new Vector2(0, 0));
            diamondHitAnimation.IsEnabled = false;
            #endregion

            animations.Add(idleAnimation);
            animations.Add(deployMineAnimation);
            animations.Add(diamondMineIdleAnimation);
            animations.Add(diamondShadowAnimation);
            animations.Add(diamondTurretIdleAnimation);
            animations.Add(diamondTurretAnimation);
            animations.Add(diamondHitAnimation);

            colorData = new Color[WIDTH * HEIGHT];
            diamondShadowTexture.GetData(colorData);

            GraphicsDevice myDevice = GameServices.GetService<GraphicsDevice>();

            RenderTarget2D renderTarget = new RenderTarget2D(myDevice, WIDTH, HEIGHT);
            SpriteBatch spriteBatch = new SpriteBatch(myDevice);

            // Set the render target on the device.
            myDevice.SetRenderTarget(renderTarget);
            myDevice.Clear(Color.Transparent);

            diamondMineShadowAnimation.scale = 2.0f;
            diamondMineShadowAnimation.origin = new Vector2(WIDTH / 2, HEIGHT / 2);

            spriteBatch.Begin();

            diamondMineShadowAnimation.Draw(spriteBatch);

            spriteBatch.End();


            // Call GetTexture to retrieve the render target data and save it to a texture.
            myDevice.SetRenderTarget((RenderTarget2D)null);
            diamondMineShadowTexture = new Texture2D(myDevice, WIDTH, HEIGHT);
            Color[] con = new Color[WIDTH * HEIGHT];
            renderTarget.GetData<Color>(con);
            diamondMineShadowTexture.SetData<Color>(con);
        }

        public void attack()
        {
        }
        public void shoot(int direction)
        {

            if (frameCounter >= SWITCH_FRAME)
            {
                frameCounter = 0;
                Bullet b = new Bullet(content, direction, "diamond");

                b.setPosition(idleAnimation.Position);
                activeBullets.Add(b);

                


            }


        }

        public override void hit()
        {
            diamondHitAnimation.IsEnabled = true;
        }

        public void deployMine()
        {
            deployMineAnimation.IsEnabled = true;
            idleAnimation.IsEnabled = false;
        }

        public void deployTurret()
        {
            diamondTurretAnimation.IsEnabled = true;
            idleAnimation.IsEnabled = false;
        }

        public void dropMine()
        {
            droppedMine = true;
        }

        public void dropTurret()
        {
            droppedTurret = true;
        }

        public void turretGetDeployed()
        {
        }

       

        public void turretGetDropped()
        {
        }

        public void mineGetDeployed()
        {
        }

        public void mineGetDropped()
        {
        }

        public Boolean mineGoBoom(float boomTime)
        {
            SpriteSheetAnimation idleAnimation = getActiveTextures()[0];

            if (boomTime >= 30)
            {
                boomTime = 0;


                Console.WriteLine(idleAnimation.scale);

                float newScale = idleAnimation.scale + 0;


                if (newScale < 2.0f)
                {
                    newScale = newScale + 0.1f;
                }
                else
                    return true;

                idleAnimation.scale = newScale;
                idleAnimation.origin = new Vector2(46, 46);

            }

            return false;

        }

        public void switchShadowTexture()
        {
            diamondShadowTexture = diamondMineShadowTexture;
        }

        public Boolean mineDropped()
        {
            return droppedMine;
        }

        public Boolean turretDropped()
        {
            return droppedTurret;
        }

        public Boolean mineDeployed()
        {
            return diamondMineIdleAnimation.IsEnabled;
        }

        public Boolean turretDeployed()
        {
            return diamondTurretIdleAnimation.IsEnabled;
        }

        public override void disableAnimation(SpriteSheetAnimation spriteSheetAnimation)
        {
            spriteSheetAnimation.IsEnabled = false;

            if (spriteSheetAnimation == deployMineAnimation)
            {
                idleAnimation.IsEnabled = true;
                diamondMineIdleAnimation.IsEnabled = true;
            }

            if (spriteSheetAnimation == diamondTurretAnimation)
            {
                idleAnimation.IsEnabled = true;
                diamondTurretIdleAnimation.IsEnabled = true;
            }
        }

        public override Texture2D getTexture()
        {
            return diamondTexture;
        }

        //Checks to see if there is a collision 
        public override bool collides(Vector2 position, Rectangle rectangleB, Color[] dataB)
        {

            foreach (Bullet b in activeBullets)
            {
                if (b.collides(position, rectangleB, dataB))
                    b.hit();
            }


            Rectangle rectangleA = new Rectangle((int)position.X, (int)position.Y, WIDTH, HEIGHT);
            Color[] dataA = new Color[WIDTH * HEIGHT];
            diamondShadowTexture.GetData(dataA);

            return (IntersectPixels(rectangleA, dataA, rectangleB,dataB));
        }

        public void checkBulletCollision(Vector2 position, Rectangle rectangle, Color[] data)
        {
            foreach (Bullet b in activeBullets)
            {
                if (b.collides(position, rectangle, data))
                    b.hit();
            }
        }

        public bool isMineAnimation(SpriteSheetAnimation animation)
        {
            if (animation == diamondMineIdleAnimation || animation == diamondTurretIdleAnimation)
                return true;
            return false;
        }
        public override void Update(GameTime gameTime)
        {
            frameCounter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;


            foreach (Bullet b in activeBullets)
            {
                if (!b.dispose())
                    b.Update(gameTime);
            }
        }

        public void clearMines()
        {
            diamondMineIdleAnimation.IsEnabled = false;
            droppedMine = false;
        }

        public void clearTurrets()
        {
            diamondTurretIdleAnimation.IsEnabled = false;
            droppedTurret = false;
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

            foreach (Bullet b in activeBullets)
                b.Draw(spriteBatch);
        }

        public override void DrawOnlyIdle(SpriteBatch spriteBatch)
        {
            base.DrawOnlyIdle(spriteBatch);

            idleAnimation.Draw(spriteBatch);
        }

        public void clearBullets()
        {
            activeBullets = new List<Shape>();
        }

        internal bool isReady()
        {
            return (frameCounter >= SWITCH_FRAME);
        }
    }
}
