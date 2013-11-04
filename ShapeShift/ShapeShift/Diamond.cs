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
        public override bool Collides(Vector2 position, Rectangle rectangle)
        {

            /*if (position.X - X_OFFSET + rectangle.Width * 2 < rectangle.X ||
                position.X + X_OFFSET > rectangle.X + rectangle.Width ||
                position.Y - Y_OFFSET + rectangle.Height * 2 < rectangle.Y ||
                position.Y + Y_OFFSET > rectangle.Y + rectangle.Height)
            {
                return false;
            }
            else
            {
                return true;
            }
            */

            double m1 = (((position.Y + 17) - (position.Y + 46)) / ((position.X + 46) - (position.X + 23)));
            double b1 = (position.Y + 17) - (m1 * (position.X + 46));
     

          //  Console.WriteLine(m1);

            int numPoints = rectangle.Width * 2 + rectangle.Height * 2;
            Point[] rectPoints = new Point[numPoints];

            for (int i = 0; i < numPoints; i++)
            {

                if (i < rectangle.Width)
                    rectPoints[i] = new Point(rectangle.X + i, rectangle.Y);

                
                else if (i < rectangle.Width + rectangle.Height)
                    rectPoints[i] = new Point(rectangle.X + rectangle.Width, rectangle.Y + i - rectangle.Width);

                else if (i < rectangle.Width * 2 + rectangle.Height )
                    rectPoints[i] = new Point(rectangle.X  + i - rectangle.Height - rectangle.Width, rectangle.Y + rectangle.Height);

                else
                    rectPoints[i] = new Point(rectangle.X, rectangle.Y + i - rectangle.Width*2 - rectangle.Height);

            }
            int quad;
            foreach (Point p in rectPoints)
            {

                quad = getQuad(p, position);

                switch (quad)
                {
                    case 0: return false;

                    case 2: if (p.Y < m1 * p.X + b1)
                                return false;
                            return true;
                }

            }


            return false;









        }

        private int getQuad(Point p, Vector2 position)
        {
            if (p.X > position.X - 46)
            {
                if (p.Y > position.Y - 46)
                    return 4;
                else
                    return 1;
            }
            else if (p.X < position.X - 46)
            {
                if (p.Y < position.Y - 46)
                    return 2;
                else
                    return 3;
            }
            else
                return 0;
                
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
