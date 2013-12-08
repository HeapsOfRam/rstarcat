using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeShift
{
    class Circle : Shape
    {
        private const int WIDTH = 92;            // width of the shape, not the image
        private const int HEIGHT = 92;           // height of the shape

        private int radius;  // holds the current radius (shield or no shield)

        #region Textures
        private Texture2D idleCircleTexture;        // Texture containing the circle idle spritesheet image
        private Texture2D shieldDeployCircleTexture;// Texture containing the deploy sheild spritesheet image
        private Texture2D shieldIdleTexture;        // Texture containing the shield idle spritesheet image
        private Texture2D shieldFadeTexture;        // Texture containing the shield fade spritesheet image
        private Texture2D circleHitTexture;
        private Texture2D circleShadowTexture;
        private Texture2D circleShieldedShadowTexture;

        #endregion

        #region Animations
        private SpriteSheetAnimation idleAnimation; 
        private SpriteSheetAnimation deployAnimation;
        private SpriteSheetAnimation shieldIdleAnimation;
        private SpriteSheetAnimation shieldFadeAnimation;
        private SpriteSheetAnimation circleHitAnimation;
        #endregion
        
        public bool shielded;


        public Boolean firing = false;
      
        public int frameCounter;
        public ContentManager content;
    

        public Circle(ContentManager content)
        {
            this.content = content;
            animations = new List<SpriteSheetAnimation>();

            activeBullets = new List<Shape>();

            heartTextures = new Texture2D[2];
            heartTextures[0] = content.Load<Texture2D>("Circle/heart");
            heartTextures[1] = content.Load<Texture2D>("Circle/heartEmpty");


            #region Load Textures & Create Animations
            //Load in the specific spritesheets used for animating the Circle
            idleCircleTexture = content.Load<Texture2D>("Circle/CircleIdleSpriteSheet");
            shieldDeployCircleTexture = content.Load<Texture2D>("Circle/CircleDeployShieldSpriteSheet");
            shieldIdleTexture = content.Load<Texture2D>("Circle/CircleShieldIdleSpriteSheet");
            shieldFadeTexture = content.Load<Texture2D>("Circle/CircleFadeSpriteSheet");
            circleHitTexture = content.Load<Texture2D>("Circle/CircleHitSpriteSheet");
            circleShadowTexture = content.Load<Texture2D>("Circle/CircleShadow");
            circleShieldedShadowTexture = content.Load<Texture2D>("Circle/CircleShieldShadow");

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

            //FOR FUN USE LATER ON
           /* foreach (SpriteSheetAnimation s in animations)
            {
                s.scale = 2.0f;
                s.origin = new Vector2(WIDTH / 2, HEIGHT / 2);
            }*/

            colorData = new Color[WIDTH * HEIGHT];
            circleShadowTexture.GetData(colorData);
        }

        public override Texture2D getTexture()
        {
            return idleCircleTexture;
        }

        public void shoot(int direction)
        {

            if (frameCounter >= SWITCH_FRAME)
            {
                frameCounter = 0;
                Bullet b = new Bullet(content, direction, "circle");

                b.setPosition(idleAnimation.Position);
                activeBullets.Add(b);


            }


        }

        // Deploys shield by enabling the deployAnimation
        // When deployAnimation finishes it will disable itself (calls disableAnimation(deployAnimation) on this shape)
        public void deployShield()
        {
            deployAnimation.IsEnabled = true;
            shielded = true;
        }

        public void removeShield()
        {

            shieldIdleAnimation.IsEnabled = false;

            shieldFadeAnimation.IsEnabled = true;

            shielded = false;
        }

        public Boolean isShielded()
        {
            return shielded;
        }

        public void ballExpire()
        {
        }

        public void ballDeploy()
        {
        }

        public void ballFire()
        {
        }

        //Disables the animation, preforms a check to see if that animation was the deploy shield animation
        // in which case the idle shield animation now needs to be enabled
        public override void disableAnimation(SpriteSheetAnimation spriteSheetAnimation)
        {
            spriteSheetAnimation.IsEnabled = false;

            if (spriteSheetAnimation == deployAnimation)
                shieldIdleAnimation.IsEnabled = true;
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


        // Overrides abstract collides method in shape.
        // Takes in a rectangle corresponding to the tile we are checking a 
        // collision with and a Vector2 that represents the center of the circle.
        // Using the radius, this method returns true of the circle intersects 
        // any potion of the rectangle and false if not.
        public override bool collides(Vector2 vect, Rectangle rectangle, Color[] Data)
        {

            foreach (Bullet b in activeBullets)
            {
                if (b.collides(vect, rectangle, Data))
                    b.hit();
            }

            Rectangle rectangleA = new Rectangle((int)vect.X, (int)vect.Y, WIDTH, HEIGHT);
            Color[] dataA = new Color[WIDTH * HEIGHT];

            if (!shielded)
                circleShadowTexture.GetData(dataA);
            else
                circleShieldedShadowTexture.GetData(dataA);

            return (IntersectPixels(rectangleA, dataA, rectangle, Data));

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

             foreach (Bullet b in activeBullets)
                b.Draw(spriteBatch);
        }

        public override void DrawOnlyIdle(SpriteBatch spriteBatch)
        {
            base.DrawOnlyIdle(spriteBatch);

            idleAnimation.Draw(spriteBatch);
        }

        public void clearBullets()
        {
            activeBullets = new List<Shape>();
        }
    }
}
