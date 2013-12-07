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
        
        private Diamond tDiamond;
        private Entity owner;

        private Boolean deployed, dropped, exploded = true, awaitingReset = false;
        private float currTime;

        private const float FUSE_TIME = 6f;

        public Mine(ContentManager content, InputManager input, Entity owner)
        {
            LoadContent(content, input);
            this.owner = owner;
        }

        public override void LoadContent(ContentManager content, InputManager input)
        {
            base.LoadContent(content, input);
            currTime = 0;
            tDiamond = new Diamond(content);
            entityShape = tDiamond;
            moveAnimation = new SpriteSheetAnimation();
            moveAnimation.position = position;
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

        public void goBoom()
        {
            exploded = true;
            tDiamond.mineGoBoom();
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
            tDiamond.mineGetDeployed();
        }

        public void dropSelf()
        {
            dropped = true;
            deployed = false;
            tDiamond.mineGetDropped();
        }

        public override void Update(GameTime gameTime, InputManager input, Collision col, Layers layer)
        {
            base.Update(gameTime, input, col, layer);

            if (!exploded)
            {
                
                tDiamond.Update(gameTime);
                
                if (deployed)
                    lockToOwner();
                if (dropped)
                    currTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (currTime > FUSE_TIME)
                {
                    currTime = 0;
                    exploded = true;
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
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if(!exploded)
                entityShape.Draw(spriteBatch);
        }
    }
}
