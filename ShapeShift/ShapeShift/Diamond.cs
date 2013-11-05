using System;
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
        private Texture2D diamondTexture;
        private Texture2D diamondDeployMineTexture;
        private Texture2D diamondMineIdleTexture;

        private SpriteSheetAnimation idleAnimation;
        private SpriteSheetAnimation deployMineAnimation;
        private SpriteSheetAnimation diamondMineIdleAnimation;

        private Boolean dropped = false;

        public Diamond(ContentManager content)
        {

            //Distances between image boundries and actual shape
            X_OFFSET = 24;
            Y_OFFSET = 20;

            animations = new List<SpriteSheetAnimation>();
            
            diamondTexture = content.Load<Texture2D>("DiamondIdleSpriteSheet");
            diamondDeployMineTexture = content.Load<Texture2D>("DiamondMineSpriteSheet");
            diamondMineIdleTexture = content.Load<Texture2D>("DiamondMineIdleSpriteSheet");
            
            idleAnimation = new SpriteSheetAnimation(this,true);
            idleAnimation.LoadContent(content, diamondTexture, "", new Vector2(0, 0));
            idleAnimation.IsEnabled = true;

            deployMineAnimation = new SpriteSheetAnimation(this, false);
            deployMineAnimation.LoadContent(content, diamondDeployMineTexture, "", new Vector2(0, 0));
            deployMineAnimation.IsEnabled = false;

            diamondMineIdleAnimation = new SpriteSheetAnimation(this, true);
            diamondMineIdleAnimation.LoadContent(content, diamondMineIdleTexture, "", new Vector2(0, 0));
            diamondMineIdleAnimation.IsEnabled = false;


            animations.Add(idleAnimation);
            animations.Add(deployMineAnimation);
            animations.Add(diamondMineIdleAnimation);
        }

        public void attack()
        {
        }

        public void deployMine()
        {
            deployMineAnimation.IsEnabled = true;
            idleAnimation.IsEnabled = false;

        }
        public void dropMine()
        {
            dropped = true;
        }
        //Disables the animation, preforms a check to see if that animation was the deploy shield animation
        // in which case the idle shield animation now needs to be enabled
        public override void disableAnimation(SpriteSheetAnimation spriteSheetAnimation)
        {
            spriteSheetAnimation.IsEnabled = false;

            if (spriteSheetAnimation == deployMineAnimation)
            {
                idleAnimation.IsEnabled = true;
                diamondMineIdleAnimation.IsEnabled = true;
            }
        }

        public override Texture2D getTexture()
        {
            return diamondTexture;
        }



        //Checks to see if there is a collision 
        public override bool Collides(Vector2 position, Rectangle rectangleA)
        {
            return false;
            /*Color[] dataB = idleAnimation.getCurrentImage();

            Rectangle rectangleB = new Rectangle((int)position.X, (int)position.Y, (int)position.X + 92, (int)position.Y + 92);



            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    if ((x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width < dataB.Length)
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
            }

            // No intersection found
            return false;*/



        }

        internal bool mineDeployed()
        {
            return diamondMineIdleAnimation.IsEnabled; 
        }
        internal bool mineDropped()
        {
            return dropped;
        }


        internal bool isMineAnimation(SpriteSheetAnimation animation)
        {
            if (animation == diamondMineIdleAnimation)
                return true;
            return false;
        }

        internal void clearMines()
        {
            diamondMineIdleAnimation.IsEnabled = false;
            dropped = false;
        }
    }
}
