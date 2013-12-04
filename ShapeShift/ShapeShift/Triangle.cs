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
        protected Texture2D triangleTexture;
        protected Texture2D triangleShadowUpTexture;
        protected Texture2D triangleShadowDownTexture;
        protected Texture2D triangleShadowLeftTexture;
        protected Texture2D triangleShadowRightTexture;
        protected Texture2D triangleHitTexture;
        protected Texture2D triangleShadowCurrentTexture;
        protected SpriteSheetAnimation triangleIdleAnimation;
        protected SpriteSheetAnimation triangleShadowUpAnimation;
        protected SpriteSheetAnimation triangleShadowDownAnimation;
        protected SpriteSheetAnimation triangleShadowLeftAnimation;
        protected SpriteSheetAnimation triangleShadowRightAnimation;
        protected SpriteSheetAnimation triangleHitAnimation;

        protected const int HEIGHT = 92;
        protected const int WIDTH = 92;
        protected const float ROTATION_SPEED = 6.0f;

        protected int shadowCount = 0;

        protected Vector2 rotatationCenter = new Vector2(45.6667f, 53.6667f);

        public Boolean firing = false;
        protected int frameCounter;
     
        private ContentManager content;

        public Triangle(ContentManager content)
        {
            this.content = content;
            // Create list that will hold animations
            animations = new List<SpriteSheetAnimation>();
            activeBullets = new List<Shape>();

            #region Loading Textures

            heartTextures = new Texture2D[2];
            heartTextures[0] = content.Load<Texture2D>("Triangle/heart");
            heartTextures[1] = content.Load<Texture2D>("Triangle/heartEmpty");

            triangleTexture = content.Load<Texture2D>("Triangle/TriangleIdleSpriteSheet");
            triangleShadowUpTexture = content.Load<Texture2D>("Triangle/TriangleShadowUp");
            triangleShadowDownTexture = content.Load<Texture2D>("Triangle/TriangleShadowDown");
            triangleShadowLeftTexture = content.Load<Texture2D>("Triangle/TriangleShadowLeft");
            triangleShadowRightTexture = content.Load<Texture2D>("Triangle/TriangleShadowRight");
            triangleHitTexture = content.Load<Texture2D>("Triangle/TrianglehitSpriteSheet");

            #endregion 

            // Triangle starts facing up 
            triangleShadowCurrentTexture = triangleShadowUpTexture;

            #region Create Animations

            triangleIdleAnimation = new SpriteSheetAnimation(this, true, WIDTH, rotatationCenter);
            triangleIdleAnimation.LoadContent(content, triangleTexture, "", new Vector2(0, 0));
            triangleIdleAnimation.IsEnabled = true;

            triangleShadowUpAnimation = new SpriteSheetAnimation(this, true, WIDTH, rotatationCenter);
            triangleShadowUpAnimation.LoadContent(content, triangleShadowUpTexture, "", new Vector2(0, 0));
            triangleShadowUpAnimation.IsEnabled = false;

            triangleShadowDownAnimation = new SpriteSheetAnimation(this, true, WIDTH, rotatationCenter);
            triangleShadowDownAnimation.LoadContent(content, triangleShadowDownTexture, "", new Vector2(0, 0));
            triangleShadowDownAnimation.IsEnabled = false;

            triangleShadowLeftAnimation = new SpriteSheetAnimation(this, true, WIDTH, rotatationCenter);
            triangleShadowLeftAnimation.LoadContent(content, triangleShadowLeftTexture, "", new Vector2(0, 0));
            triangleShadowLeftAnimation.IsEnabled = false;

            triangleShadowRightAnimation = new SpriteSheetAnimation(this, true, WIDTH, rotatationCenter);
            triangleShadowRightAnimation.LoadContent(content, triangleShadowRightTexture, "", new Vector2(0, 0));
            triangleShadowRightAnimation.IsEnabled = false;

            triangleHitAnimation = new SpriteSheetAnimation(this, false, WIDTH, rotatationCenter);
            triangleHitAnimation.LoadContent(content, triangleHitTexture, "", new Vector2(0, 0));
            triangleHitAnimation.IsEnabled = false;

            #endregion

            animations.Add(triangleIdleAnimation);
            animations.Add(triangleShadowUpAnimation);
            animations.Add(triangleShadowDownAnimation);
            animations.Add(triangleShadowLeftAnimation);
            animations.Add(triangleShadowRightAnimation);
            animations.Add(triangleHitAnimation);

            colorData = new Color[WIDTH * HEIGHT];
            triangleShadowCurrentTexture.GetData(colorData);
        }

        public override Texture2D getTexture()
        { return triangleTexture; }

        public void shoot(int direction)
        {

            if (frameCounter >= SWITCH_FRAME)
            {
                frameCounter = 0;
                Bullet b = new Bullet(content, direction, "triangle");

                b.setPosition(triangleIdleAnimation.Position);
                activeBullets.Add(b);


            }


        }
        public override int getHeight() { return HEIGHT; }

        public override int getWidth() { return WIDTH; }

        public Boolean rotating()
        {
            return triangleIdleAnimation.rotate;
        }

        public void PreformRotate()
        {
            // If there isn't a rotation already taking place:
            if (!triangleIdleAnimation.rotate)
            {
                // If you are transitioning back to the first shadow (up):
                if (shadowCount == 4)
                {
                    // Reset the rotation variable in the animation (prevents accumulated error in collision mapping)
                    triangleIdleAnimation.rotation = (0.0f);
                    triangleHitAnimation.rotation = (0.0f);
                }


                triangleIdleAnimation.PreformRotate(ROTATION_SPEED, false);
                triangleHitAnimation.preformRotateNoAnimation(ROTATION_SPEED);

                triangleIdleAnimation.origin = rotatationCenter;
                triangleHitAnimation.origin = rotatationCenter;
               
                shadowCount++;

                switch (shadowCount)
                {
                    case 1: triangleShadowCurrentTexture = triangleShadowRightTexture;
                            break;
                    case 2: triangleShadowCurrentTexture = triangleShadowDownTexture;
                            break;
                    case 3: triangleShadowCurrentTexture = triangleShadowLeftTexture;
                            break;
                    case 4: triangleShadowCurrentTexture = triangleShadowUpTexture;
                            shadowCount = 0;
                            break;
                }


                colorData = new Color[WIDTH * HEIGHT];
                triangleShadowCurrentTexture.GetData(colorData);
            }
        }

        public void Update(GameTime gameTime)
        {
            frameCounter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;


            foreach (Bullet b in activeBullets)
            {
                if (!b.dispose())
                    b.Update(gameTime);

            }
        }

        public override bool collides(Vector2 position, Rectangle rectangleB, Color[] dataB)
        {
            foreach (Bullet b in activeBullets)
            {
                if (b.collides(position, rectangleB, dataB))
                    b.hit();
            }

            Rectangle rectangleA = new Rectangle((int)position.X, (int)position.Y, WIDTH, HEIGHT);
            
            //gets the color data for the current shadow texture
            Color[] dataA = new Color[WIDTH * HEIGHT];
            triangleShadowCurrentTexture.GetData(dataA);

            return (IntersectPixels(rectangleA, dataA, rectangleB, dataB));
        }

        public override void hit()
        {
            triangleHitAnimation.IsEnabled = true;
        }

        public override void disableAnimation(SpriteSheetAnimation spriteSheetAnimation)
        {
            spriteSheetAnimation.IsEnabled = false;
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
            triangleIdleAnimation.Draw(spriteBatch);
        }

        public void clearBullets()
        {
            activeBullets = new List<Shape>();
        }
    }
}
