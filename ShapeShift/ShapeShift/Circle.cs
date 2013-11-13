using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeShift
{
    public class Circle : Shape
    {

        private const int RADIUS_NO_SHIELD = 21; // gap between the image boundry and the actual shape
        private const int RADIUS_SHIELD = 27;    // gap between the image boundry and the shield
        private const int WIDTH = 46;            // width of the shape, not the image
        private const int HEIGHT = 46;           // height of the shape

        private int radius;  // holds the current radius (shield or no shield)

        #region Textures
        private Texture2D idleCircleTexture;        // Texture containing the circle idle spritesheet image
        private Texture2D shieldDeployCircleTexture;// Texture containing the deploy sheild spritesheet image
        private Texture2D shieldIdleTexture;        // Texture containing the shield idle spritesheet image
        private Texture2D shieldFadeTexture;        // Texture containing the shield fade spritesheet image
        private Texture2D circleHitTexture;
        #endregion

        #region Animations
        private SpriteSheetAnimation idleAnimation; 
        private SpriteSheetAnimation deployAnimation;
        private SpriteSheetAnimation shieldIdleAnimation;
        private SpriteSheetAnimation shieldFadeAnimation;
        private SpriteSheetAnimation circleHitAnimation;
        #endregion

        public bool shielded;

        public Circle(ContentManager content)
        {

            animations = new List<SpriteSheetAnimation>();
            radius = RADIUS_NO_SHIELD;

            #region Load Textures & Create Animations
            //Load in the specific spritesheets used for animating the Circle
            idleCircleTexture = content.Load<Texture2D>("Circle/CircleIdleSpriteSheet");
            shieldDeployCircleTexture = content.Load<Texture2D>("Circle/CircleDeployShieldSpriteSheet");
            shieldIdleTexture = content.Load<Texture2D>("Circle/CircleShieldIdleSpriteSheet");
            shieldFadeTexture = content.Load<Texture2D>("Circle/CircleFadeSpriteSheet");
            circleHitTexture = content.Load<Texture2D>("Circle/CircleHitSpriteSheet");

            // Create new SpriteSheetAnimation objects for each texture and add them to the animation list
            idleAnimation = new SpriteSheetAnimation(this, true);
            idleAnimation.LoadContent(content, idleCircleTexture, "", new Vector2(0, 0));
            idleAnimation.IsEnabled = true;

            deployAnimation = new SpriteSheetAnimation(this, false);
            deployAnimation.LoadContent(content, shieldDeployCircleTexture, "", new Vector2(0, 0));
            deployAnimation.IsEnabled = false;

            shieldIdleAnimation = new SpriteSheetAnimation(this, true);
            shieldIdleAnimation.LoadContent(content, shieldIdleTexture, "", new Vector2(0, 0));
            shieldIdleAnimation.IsEnabled = false;

            shieldFadeAnimation = new SpriteSheetAnimation(this, false);
            shieldFadeAnimation.LoadContent(content, shieldFadeTexture, "", new Vector2(0, 0));
            shieldFadeAnimation.IsEnabled = false;

            circleHitAnimation = new SpriteSheetAnimation(this, false);
            circleHitAnimation.LoadContent(content, circleHitTexture, "", new Vector2(0, 0));
            circleHitAnimation.IsEnabled = false;
            #endregion

            animations.Add(idleAnimation);
            animations.Add(deployAnimation);
            animations.Add(shieldIdleAnimation);
            animations.Add(shieldFadeAnimation);
            animations.Add(circleHitAnimation);
        }

        public override Texture2D getTexture()
        {
            return idleCircleTexture;
        }


        // Deploys shield by enabling the deployAnimation
        // When deployAnimation finishes it will disable itself (calls disableAnimation(deployAnimation) on this shape)
        public void deployShield()
        {
            deployAnimation.IsEnabled = true;
            shielded = true;
            //Increase the radius to account for the shield being displayed 
            radius = RADIUS_SHIELD;
        }

        public void removeShield()
        {

            shieldIdleAnimation.IsEnabled = false;

            shieldFadeAnimation.IsEnabled = true;

            //Decrease the radius to account for the shield no longer being displayed
            radius = RADIUS_NO_SHIELD;

            shielded = false;
        }

        //Disables the animation, preforms a check to see if that animation was the deploy shield animation
        // in which case the idle shield animation now needs to be enabled
        public override void disableAnimation(SpriteSheetAnimation spriteSheetAnimation)
        {
            spriteSheetAnimation.IsEnabled = false;

            if (spriteSheetAnimation == deployAnimation)
                shieldIdleAnimation.IsEnabled = true;
        }


        // Overrides abstract collides method in shape.
        // Takes in a rectangle corresponding to the tile we are checking a 
        // collision with and a Vector2 that represents the center of the circle.
        // Using the radius, this method returns true of the circle intersects 
        // any potion of the rectangle and false if not.
        public override bool Collides(Vector2 vect, Rectangle rectangle, Color[] Data)
        {
            Point circle = new Point((int)vect.X + WIDTH, (int)vect.Y + WIDTH);

            var rectangleCenter = new Point((rectangle.X + rectangle.Width / 2),
                                            (rectangle.Y + rectangle.Height / 2));

            var w = rectangle.Width / 2;
            var h = rectangle.Height / 2;

            var dx = Math.Abs(circle.X - rectangleCenter.X);
            var dy = Math.Abs(circle.Y - rectangleCenter.Y);

            if ((dx > radius + w) || (dy > radius + h))
                return false;

            var circleDistance = new Point
            {
                X = Math.Abs(circle.X - rectangle.X - w),
                Y = Math.Abs(circle.Y - rectangle.Y - h)
            };

            if (circleDistance.X <= w)
                return true;

            if (circleDistance.Y <= h)
                return true;

            var cornerDistanceSq = Math.Pow(circleDistance.X - w, 2) +
                                   Math.Pow(circleDistance.Y - h, 2);

            return (cornerDistanceSq <= (Math.Pow(radius, 2)));

        }

        public override void hit()
        {
            circleHitAnimation.IsEnabled = true;
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

            idleAnimation.Draw(spriteBatch);
        }
    }
}
