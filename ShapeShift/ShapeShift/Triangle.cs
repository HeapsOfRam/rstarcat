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
        private Texture2D triangleShadowUpTexture;
        private Texture2D triangleShadowDownTexture;
        private Texture2D triangleShadowLeftTexture;
        private Texture2D triangleShadowRightTexture;
        private Texture2D triangleHitTexture;
        private Texture2D triangleShadowCurrentTexture;
        
        private const int HEIGHT = 92;
        private const int WIDTH = 92;
        private const float ROTATION_SPEED = 6.0f;

        private int shadowCount = 0;

        private Vector2 rotatationCenter = new Vector2(45.6667f, 53.6667f);

        private SpriteSheetAnimation triangleIdleAnimation;
        private SpriteSheetAnimation triangleShadowUpAnimation;
        private SpriteSheetAnimation triangleShadowDownAnimation;
        private SpriteSheetAnimation triangleShadowLeftAnimation;
        private SpriteSheetAnimation triangleShadowRightAnimation;
        private SpriteSheetAnimation triangleHitAnimation;

        public Triangle(ContentManager content)
        {
            // Create list that will hold animations
            animations = new List<SpriteSheetAnimation>();

            #region Loading Textures

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
        }

        public override Texture2D getTexture()
        {
            return triangleTexture;
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
            }
        }


        public override bool Collides(Vector2 position, Rectangle rectangleB, Color[] dataB)
        {
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
        }

        public override void DrawOnlyIdle(SpriteBatch spriteBatch)
        {
            base.DrawOnlyIdle(spriteBatch);
            triangleIdleAnimation.Draw(spriteBatch);
        }
    }
}
