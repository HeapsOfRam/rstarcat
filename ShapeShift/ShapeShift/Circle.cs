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

        private const int RADIUS_NO_SHIELD = 16;
        private const int RADIUS_SHIELD = 21;
        private const int WIDTH = 32;
        private const int HEIGHT = 32;

        private int radius = RADIUS_NO_SHIELD;

        private Texture2D idleCircleTexture;
        private Texture2D shieldDeployCircleTexture;
        private Texture2D shieldIdleTexture;

        private SpriteSheetAnimation idleAnimation;
        private SpriteSheetAnimation deployAnimation;
        private SpriteSheetAnimation sheildIdleAnimation;

        public Circle(ContentManager content)
        {

            animations = new List<SpriteSheetAnimation>();

            //Load in the specific spritesheets used for animating the Circle
            idleCircleTexture = content.Load<Texture2D>("CircleIdleSpriteSheet");
            shieldDeployCircleTexture = content.Load<Texture2D>("CircleDeployShieldSpriteSheet");
            shieldIdleTexture = content.Load<Texture2D>("CircleShieldIdleSpriteSheet");

            // Create new SpriteSheetAnimation objects for each texture and add them to the animation list
            idleAnimation = new SpriteSheetAnimation(this, true);
            idleAnimation.LoadContent(content, idleCircleTexture, "", new Vector2(0, 0));
            idleAnimation.IsEnabled = true;

            deployAnimation = new SpriteSheetAnimation(this, false);
            deployAnimation.LoadContent(content, shieldDeployCircleTexture, "", new Vector2(0, 0));
            deployAnimation.IsEnabled = false;

            sheildIdleAnimation = new SpriteSheetAnimation(this, true);
            sheildIdleAnimation.LoadContent(content, shieldIdleTexture, "", new Vector2(0, 0));
            sheildIdleAnimation.IsEnabled = false;

            animations.Add(idleAnimation);
            animations.Add(deployAnimation);
            animations.Add(sheildIdleAnimation);
        }

        public override Texture2D getTexture()
        {
            return idleCircleTexture;
        }


        public void deployShield()
        {
            deployAnimation.IsEnabled = true;

            radius = RADIUS_SHIELD;
        }

        public override void disableAnimation(SpriteSheetAnimation spriteSheetAnimation)
        {

            spriteSheetAnimation.IsEnabled = false;

            if (spriteSheetAnimation == deployAnimation)
                sheildIdleAnimation.IsEnabled = true;
            
 
            
        }

        public override bool Collides(Vector2 vect, Rectangle rectangle)
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
    }
}
