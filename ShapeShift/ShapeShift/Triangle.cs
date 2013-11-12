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
        private Texture2D triangleShadowTexture;
        private Texture2D triangleHitTexture;
        
        private SpriteSheetAnimation idleAnimation;
        private SpriteSheetAnimation triangleShadowAnimation;
        private SpriteSheetAnimation triangleHitAnimation;

        public Triangle(ContentManager content)
        {

            X_OFFSET = 24; 
            Y_OFFSET = 24;



            animations = new List<SpriteSheetAnimation>();


            triangleTexture = content.Load<Texture2D>("Triangle/TriangleIdleSpriteSheet");
            triangleShadowTexture = content.Load<Texture2D>("Triangle/TriangleShadow");
            triangleHitTexture = content.Load<Texture2D>("Triangle/TrianglehitSpriteSheet");


            idleAnimation = new SpriteSheetAnimation(this,true, 92, new Vector2 (45,54));
            idleAnimation.LoadContent(content, triangleTexture, "", new Vector2(0, 0));
            idleAnimation.IsEnabled = true;

            triangleShadowAnimation = new SpriteSheetAnimation(this, true, 92, new Vector2(45, 54));
            triangleShadowAnimation.LoadContent(content, triangleShadowTexture, "", new Vector2(0, 0));
            triangleShadowAnimation.IsEnabled = false;

            triangleHitAnimation = new SpriteSheetAnimation(this, false, 92, new Vector2(45, 54));
            triangleHitAnimation.LoadContent(content, triangleHitTexture, "", new Vector2(0, 0));
            triangleHitAnimation.IsEnabled = false;


            animations.Add(idleAnimation);
            animations.Add(triangleShadowAnimation);
            animations.Add(triangleHitAnimation);
        }

        public override Texture2D getTexture()
        {
            return triangleTexture;
        }

        public void PreformRotate()
        {
            idleAnimation.PreformRotate(6.0f);
            triangleHitAnimation.PreformRotate(6.0f);
            triangleShadowAnimation.PreformRotate(6.0f);
            idleAnimation.origin = new Vector2(45, 54);

        }

        //Checks to see if there is a collision 
        public override bool Collides(Vector2 position, Rectangle rectangleB, Color[] dataB)
        {
            Rectangle rectangleA = new Rectangle((int)position.X, (int)position.Y, 92, 92);
            Color[] dataA = new Color[92 * 92];
            triangleShadowTexture.GetData(dataA);

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
