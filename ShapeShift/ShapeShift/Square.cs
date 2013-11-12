using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeShift
{
    class Square : Shape
    {
        private Texture2D squareTexture;
        private Texture2D dashIdleTexture;
        private Texture2D dashEastTexture;
        private Texture2D dashWestTexture;
        private Texture2D dashSouthTexture;
        private Texture2D dashSouthEastTexture;
        private Texture2D dashNorthTexture;
        private Texture2D dashSouthWestTexture;
        private Texture2D dashNorthEastTexture;
        private Texture2D dashNorthWestTexture;
        private Texture2D squareHitTexture;
        
        private SpriteSheetAnimation idleAnimation;
        private SpriteSheetAnimation dashIdleAnimation;
        private SpriteSheetAnimation dashEastAnimation;
        private SpriteSheetAnimation dashWestAnimation;
        private SpriteSheetAnimation dashNorthAnimation;
        private SpriteSheetAnimation dashSouthAnimation;
        private SpriteSheetAnimation dashSouthEastAnimation;
        private SpriteSheetAnimation dashSouthWestAnimation;
        private SpriteSheetAnimation dashNorthEastAnimation;
        private SpriteSheetAnimation dashNorthWestAnimation;
        private SpriteSheetAnimation squareHitAnimation;

        private const float DASH_DISTANCE = 30;

        public Boolean dashing = false;

        private List<SpriteSheetAnimation> dashAnimations;

        public Square(ContentManager content)
        {
            X_OFFSET = 24;
            Y_OFFSET = 24;
            animations = new List<SpriteSheetAnimation>();
            dashAnimations = new List<SpriteSheetAnimation>();

            squareTexture = content.Load<Texture2D>("Square/SquareIdleSpriteSheet");
            dashIdleTexture = content.Load<Texture2D>("Square/SquareDashIdleSpriteSheet");
            dashEastTexture = content.Load<Texture2D>("Square/SquareDashRightSpriteSheet");
            dashWestTexture = content.Load<Texture2D>("Square/SquareDashLeftSpriteSheet");
            dashSouthEastTexture = content.Load<Texture2D>("Square/SquareDashSouthEastSpriteSheet");
            dashSouthTexture = content.Load<Texture2D>("Square/SquareDashSouthSpriteSheet");
            dashNorthTexture = content.Load<Texture2D>("Square/SquareDashNorthSpriteSheet");
            dashNorthWestTexture = content.Load<Texture2D>("Square/SquareDashNorthWestSpriteSheet");
            dashNorthEastTexture = content.Load<Texture2D>("Square/SquareDashNorthEastSpriteSheet");
            dashSouthWestTexture = content.Load<Texture2D>("Square/SquareDashSouthWestSpriteSheet");
            squareHitTexture = content.Load<Texture2D>("Square/SquareHitSpriteSheet");

            idleAnimation = new SpriteSheetAnimation(this,true);
            idleAnimation.LoadContent(content, squareTexture, "", new Vector2(0, 0));
            idleAnimation.IsEnabled = true;

            dashIdleAnimation = new SpriteSheetAnimation(this, true);
            dashIdleAnimation.LoadContent(content, dashIdleTexture, "", new Vector2(0, 0));
            dashIdleAnimation.IsEnabled = false;

            dashEastAnimation = new SpriteSheetAnimation(this, true);
            dashEastAnimation.LoadContent(content, dashEastTexture, "", new Vector2(0, 0));
            dashEastAnimation.IsEnabled = false;

            dashWestAnimation = new SpriteSheetAnimation(this, true);
            dashWestAnimation.LoadContent(content, dashWestTexture, "", new Vector2(0, 0));
            dashWestAnimation.IsEnabled = false;

            dashNorthAnimation = new SpriteSheetAnimation(this, true);
            dashNorthAnimation.LoadContent(content, dashNorthTexture, "", new Vector2(0, 0));
            dashNorthAnimation.IsEnabled = false;

            dashSouthEastAnimation = new SpriteSheetAnimation(this, true);
            dashSouthEastAnimation.LoadContent(content, dashSouthEastTexture, "", new Vector2(0, 0));
            dashSouthEastAnimation.IsEnabled = false;

            dashSouthAnimation = new SpriteSheetAnimation(this, true);
            dashSouthAnimation.LoadContent(content, dashSouthTexture, "", new Vector2(0, 0));
            dashSouthAnimation.IsEnabled = false;

            dashSouthWestAnimation = new SpriteSheetAnimation(this, true);
            dashSouthWestAnimation.LoadContent(content, dashSouthWestTexture, "", new Vector2(0, 0));
            dashSouthWestAnimation.IsEnabled = false;


            dashNorthEastAnimation = new SpriteSheetAnimation(this, true);
            dashNorthEastAnimation.LoadContent(content, dashNorthEastTexture, "", new Vector2(0, 0));
            dashNorthEastAnimation.IsEnabled = false;

            dashNorthWestAnimation = new SpriteSheetAnimation(this, true);
            dashNorthWestAnimation.LoadContent(content, dashNorthWestTexture, "", new Vector2(0, 0));
            dashNorthWestAnimation.IsEnabled = false;

            squareHitAnimation = new SpriteSheetAnimation(this, false);
            squareHitAnimation.LoadContent(content, squareHitTexture, "", new Vector2(0, 0));
            squareHitAnimation.IsEnabled = false;

            animations.Add(idleAnimation);
            animations.Add(dashIdleAnimation);
            animations.Add(dashEastAnimation);
            animations.Add(dashWestAnimation);
            animations.Add(dashSouthEastAnimation);
            animations.Add(dashSouthAnimation);
            animations.Add(dashNorthAnimation);
            animations.Add(dashSouthWestAnimation);
            animations.Add(dashNorthEastAnimation);
            animations.Add(dashNorthWestAnimation);
            animations.Add(squareHitAnimation);

            dashAnimations.Add(dashEastAnimation);
            dashAnimations.Add(dashWestAnimation);      
            dashAnimations.Add(dashSouthEastAnimation);
            dashAnimations.Add(dashSouthAnimation);
            dashAnimations.Add(dashNorthAnimation);
            dashAnimations.Add(dashSouthWestAnimation);
            dashAnimations.Add(dashNorthEastAnimation);
            dashAnimations.Add(dashNorthWestAnimation);

        }

        public void attack()
        {
        }

        public override void hit (){
            squareHitAnimation.IsEnabled = true; 
        }

        public override Texture2D getTexture()
        {
            return squareTexture;
        }


        public override bool Collides(Vector2 position, Rectangle rectangle, Color[] Data)
        {

            if (position.X - X_OFFSET + rectangle.Width * 2 < rectangle.X ||
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


        }

        public void dash(Player player)
        {
            player.setMoveSpeed(300f);

            dashing = true;

            dashIdleAnimation.IsEnabled = true;
        }

        public void stopDashing()
        {
            dashing = false;

            dashIdleAnimation.IsEnabled = false;

            resetDashAnimations();
        }

        public override void disableAnimation(SpriteSheetAnimation spriteSheetAnimation)
        {
            spriteSheetAnimation.IsEnabled = false;


        }


        internal void setDirectionMap(Boolean[] directions)
        {

            if (dashing)
            {
                SpriteSheetAnimation currentDashAnimation = dashIdleAnimation;

                resetDashAnimations();

                if (directions[0])
                {
                    if (directions[2])
                        currentDashAnimation = dashSouthEastAnimation;
                    else if (directions[3])
                        currentDashAnimation = dashNorthEastAnimation;
                    else
                        currentDashAnimation = dashEastAnimation;
                }
                if (directions[1])
                {
                    if (directions[2])
                        currentDashAnimation = dashSouthWestAnimation;
                    else if (directions[3])
                        currentDashAnimation = dashNorthWestAnimation;
                    else
                        currentDashAnimation = dashWestAnimation;
                }

                if (directions[2])
                {
                    if (directions[1])
                        currentDashAnimation = dashSouthWestAnimation;
                    else if (directions[0])
                        currentDashAnimation = dashSouthEastAnimation;
                    else
                        currentDashAnimation = dashSouthAnimation;
                }
                if (directions[3])
                {
                    if (directions[1])
                        currentDashAnimation = dashNorthWestAnimation;
                    else if (directions[0])
                        currentDashAnimation = dashNorthEastAnimation;
                    else
                        currentDashAnimation = dashNorthAnimation;
                }

                currentDashAnimation.IsEnabled = true;
                currentDashAnimation.CurrentFrame = dashIdleAnimation.CurrentFrame;

            }

        }

        public void resetDashAnimations()
        {
            foreach (SpriteSheetAnimation s in dashAnimations)
                s.IsEnabled = false;
        }

   

    }
}
