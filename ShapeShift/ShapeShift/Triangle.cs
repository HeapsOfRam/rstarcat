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
        
        private SpriteSheetAnimation idleAnimation;

        private SpriteSheetAnimation triangleShadowUpAnimation;
        private SpriteSheetAnimation triangleShadowDownAnimation;
        private SpriteSheetAnimation triangleShadowLeftAnimation;
        private SpriteSheetAnimation triangleShadowRightAnimation;

        private SpriteSheetAnimation triangleHitAnimation;

        private int shadowCount = 0;
        public Triangle(ContentManager content)
        {

            animations = new List<SpriteSheetAnimation>();


            triangleTexture = content.Load<Texture2D>("Triangle/TriangleIdleSpriteSheet");
            triangleShadowUpTexture = content.Load<Texture2D>("Triangle/TriangleShadowUp");
            triangleShadowDownTexture = content.Load<Texture2D>("Triangle/TriangleShadowDown");
            triangleShadowLeftTexture = content.Load<Texture2D>("Triangle/TriangleShadowLeft");
            triangleShadowRightTexture = content.Load<Texture2D>("Triangle/TriangleShadowRight");
            triangleHitTexture = content.Load<Texture2D>("Triangle/TrianglehitSpriteSheet");

            triangleShadowCurrentTexture = triangleShadowUpTexture;

            idleAnimation = new SpriteSheetAnimation(this,true, 92, new Vector2 (45.6667f,53.6667f));
            idleAnimation.LoadContent(content, triangleTexture, "", new Vector2(0, 0));
            idleAnimation.IsEnabled = true;

            triangleShadowUpAnimation = new SpriteSheetAnimation(this, true, 92, new Vector2(45.6667f, 53.6667f));
            triangleShadowUpAnimation.LoadContent(content, triangleShadowUpTexture, "", new Vector2(0, 0));
            triangleShadowUpAnimation.IsEnabled = false;
            triangleShadowDownAnimation = new SpriteSheetAnimation(this, true, 92, new Vector2(45.6667f, 53.6667f));
            triangleShadowDownAnimation.LoadContent(content, triangleShadowDownTexture, "", new Vector2(0, 0));
            triangleShadowDownAnimation.IsEnabled = false;
            triangleShadowLeftAnimation = new SpriteSheetAnimation(this, true, 92, new Vector2(45.6667f, 53.6667f));
            triangleShadowLeftAnimation.LoadContent(content, triangleShadowLeftTexture, "", new Vector2(0, 0));
            triangleShadowLeftAnimation.IsEnabled = false;
            triangleShadowRightAnimation = new SpriteSheetAnimation(this, true, 92, new Vector2(45.6667f, 53.6667f));
            triangleShadowRightAnimation.LoadContent(content, triangleShadowRightTexture, "", new Vector2(0, 0));
            triangleShadowRightAnimation.IsEnabled = false;

            triangleHitAnimation = new SpriteSheetAnimation(this, false, 92, new Vector2(45.6667f, 53.6667f));
            triangleHitAnimation.LoadContent(content, triangleHitTexture, "", new Vector2(0, 0));
            triangleHitAnimation.IsEnabled = false;


            animations.Add(idleAnimation);
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
            if (!idleAnimation.rotate)
            {
               
      
                idleAnimation.PreformRotate(6.0f, false);
                triangleHitAnimation.preformRotateNoAnimation(6.0f);
                
                idleAnimation.origin = new Vector2(45.6667f, 53.6667f);
                triangleHitAnimation.origin = new Vector2(45.6667f, 53.6667f);
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
                            break;
                    case 5: shadowCount = 0;
                            break;
                }

            }
        }

        //Checks to see if there is a collision 
        public override bool Collides(Vector2 position, Rectangle rectangleB, Color[] dataB)
        {
            Rectangle rectangleA = new Rectangle((int)position.X, (int)position.Y, 92, 92);
            Color[] dataA = new Color[92 * 92];
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

    }
}
