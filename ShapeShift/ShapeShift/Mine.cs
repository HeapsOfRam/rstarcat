using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    class Mine : Entity
    {
        
        private Diamond mDiamond;
        private Entity owner;

        private Effect effect;

        private Boolean deployed, dropped, exploded = false, awaitingReset = false, collidable = false;
        private float currTime, boomTime = 0;

        private const float FUSE_TIME = 2f;

        private Boolean lightFuse = false;
        private Boolean gone = true;

        public Mine(ContentManager content, InputManager input, Entity owner)
        {
            LoadContent(content, input);
            this.owner = owner;
        }

        public override void LoadContent(ContentManager content, InputManager input)
        {
            base.LoadContent(content, input);
            currTime = 0;
            mDiamond = new Diamond(content);
            entityShape = mDiamond;
            moveAnimation = new SpriteSheetAnimation();
            moveAnimation.position = position;

            effect = content.Load<Effect>("ColorChange");
            mDiamond.scaleShape(.75f);
        }

        public void lockToOwner()
        {
            position = owner.position;
            entityShape.setPosition(position);
            moveAnimation.Position = position;
        }

        public void setOwner(Entity owner)
        { this.owner = owner; }

        public Entity getOwner()
        { return owner; }

        public Boolean isDeployed()
        { return deployed; }

        public Boolean isDropped()
        { return dropped; }

        public Boolean isDetonated()
        { return exploded; }

        public Boolean isCollidable()
        { return collidable; }

        public void goBoom()
        {
            exploded = true;
            gone = false;
            mDiamond.switchShadowTexture();
            collidable = true;
            //mDiamond.mineGoBoom();
        }

        public override Rectangle getRectangle()
        {
            return new Rectangle((int)position.X, (int)position.Y, entityShape.getWidth(), entityShape.getHeight());
        }

        public override Boolean isEnemy()
        { return false; }

        public Boolean isAwaitingReset()
        { return awaitingReset; }

        public void reset()
        {
            awaitingReset = false;
        }

        public void deploySelf()
        {
            exploded = false;
            deployed = true;
            gone = false;
            mDiamond.mineGetDeployed();
        }

        public override void die()
        {
            //Console.WriteLine("KILL ME");
            gone = true;
            deployed = false;
            dropped = false;
        }
        public void dropSelf()
        {
            dropped = true;
            deployed = false;
            mDiamond.mineGetDropped();
        }



        public void trigger()
        {
            lightFuse = true;
           
        }

        public override void Update(GameTime gameTime, InputManager input, Collision col, Layers layer)
        {
           // base.Update(gameTime, input, col, layer);

            this.gameTime = gameTime;
            this.input    = input;
            this.col      = col;
            this.layer    = layer;

            if (!exploded)
            {

                mDiamond.Update(gameTime);

                if (deployed)
                    lockToOwner();
                if (dropped && lightFuse)
                    currTime += (float)gameTime.ElapsedGameTime.TotalSeconds;



                if (currTime > FUSE_TIME)
                {
                    currTime = 0;
                    goBoom();
                }

                if (exploded)
                {
                    dropped = false;
                    awaitingReset = true;

                }

                // Update all of the enabled animations
                List<SpriteSheetAnimation> Animations = entityShape.getActiveTextures();
                foreach (SpriteSheetAnimation animation in Animations)
                {
                    if (animation.IsEnabled)
                        animation.Update(gameTime);
                }

            }
            else
            {
                boomTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                gone = mDiamond.mineGoBoom(boomTime);
                
                if (gone)
                    collidable = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            
            effect.CurrentTechnique.Passes[0].Apply();

            if (!gone)
                entityShape.Draw(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin();
            base.Draw(spriteBatch);
        }

        public bool isGone()
        {
            return gone;
        }
    }
}
