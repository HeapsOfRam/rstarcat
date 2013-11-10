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
        private Texture2D diamondShadowTexture;

        private SpriteSheetAnimation idleAnimation;
        private SpriteSheetAnimation deployMineAnimation;
        private SpriteSheetAnimation diamondMineIdleAnimation;
        private SpriteSheetAnimation diamondShadowAnimation;

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
            diamondShadowTexture = content.Load<Texture2D>("DiamondShadow");
            
            idleAnimation = new SpriteSheetAnimation(this,true);
            idleAnimation.LoadContent(content, diamondTexture, "", new Vector2(0, 0));
            idleAnimation.IsEnabled = true;

            deployMineAnimation = new SpriteSheetAnimation(this, false);
            deployMineAnimation.LoadContent(content, diamondDeployMineTexture, "", new Vector2(0, 0));
            deployMineAnimation.IsEnabled = false;

            diamondMineIdleAnimation = new SpriteSheetAnimation(this, true);
            diamondMineIdleAnimation.LoadContent(content, diamondMineIdleTexture, "", new Vector2(0, 0));
            diamondMineIdleAnimation.IsEnabled = false;

            diamondShadowAnimation = new SpriteSheetAnimation(this, true);
            diamondShadowAnimation.LoadContent(content, diamondShadowTexture, "", new Vector2(0, 0));
            diamondShadowAnimation.IsEnabled = false;


            animations.Add(idleAnimation);
            animations.Add(deployMineAnimation);
            animations.Add(diamondMineIdleAnimation);
            animations.Add(diamondShadowAnimation);

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
        public override bool Collides(Vector2 position, Rectangle rectangleB, Color[] dataB)
        {
            Rectangle rectangleA = new Rectangle((int)position.X, (int)position.Y, 92, 92);
            Color[] dataA = new Color[92 * 92];
            diamondShadowTexture.GetData(dataA);

            return (IntersectPixels(rectangleA, dataA, rectangleB,dataB));


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
