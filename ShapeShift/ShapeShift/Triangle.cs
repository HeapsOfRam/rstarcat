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
        
        private SpriteSheetAnimation idleAnimation;
        private SpriteSheetAnimation triangleShadowAnimation;

        public Triangle(ContentManager content)
        {

            X_OFFSET = 24; 
            Y_OFFSET = 24;



            animations = new List<SpriteSheetAnimation>();


            triangleTexture = content.Load<Texture2D>("TriangleIdleSpriteSheet");

            triangleShadowTexture = content.Load<Texture2D>("TriangleShadow");


            idleAnimation = new SpriteSheetAnimation(this,true, 92, new Vector2 (45,54));
            idleAnimation.LoadContent(content, triangleTexture, "", new Vector2(0, 0));
            idleAnimation.IsEnabled = true;

            triangleShadowAnimation = new SpriteSheetAnimation(this, true, 92, new Vector2(45, 54));
            triangleShadowAnimation.LoadContent(content, triangleShadowTexture, "", new Vector2(0, 0));
            triangleShadowAnimation.IsEnabled = false;


            animations.Add(idleAnimation);
            animations.Add(triangleShadowAnimation);
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

        //Checks to see if there is a collision 
        public override bool Collides(Vector2 position, Rectangle rectangleB, Color[] dataB)
        {
            Rectangle rectangleA = new Rectangle((int)position.X, (int)position.Y, 92, 92);
            Color[] dataA = new Color[92 * 92];
            triangleShadowTexture.GetData(dataA);

            return (IntersectPixels(rectangleA, dataA, rectangleB, dataB));


        }


        static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                                    Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }
    }
}
