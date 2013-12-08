using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    class Ball : Entity
    {
        
        
        private Circle bCircle;
        private Entity owner;

        private Boolean deployed, fired, expired = true, awaitingReset = false;
        private float currTime;

        private const float BOUNCE_TIME = 6f;

        public Ball(ContentManager content, InputManager input, Entity owner)
        {
            LoadContent(content, input);
            this.owner = owner;
        }

        public override void LoadContent(ContentManager content, InputManager input)
        {
            base.LoadContent(content, input);
            currTime = 0;
            bCircle = new Circle(content);
            entityShape = bCircle;
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

        public Boolean isFired()
        { return fired; }

        public Boolean isExpired()
        { return expired; }

        public void expire()
        {
            expired = true;
            bCircle.ballExpire();
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
            expired = false;
            deployed = true;
            bCircle.ballDeploy();
        }

        public void fireSelf()
        {
            fired = true;
            deployed = false;
            bCircle.ballFire();
        }

        public override void Update(GameTime gameTime, InputManager input, Collision col, Layers layer)
        {
            base.Update(gameTime, input, col, layer);

            if (!expired)
            {
                
                bCircle.Update(gameTime);
                
                if (deployed)
                    lockToOwner();
                if (fired)
                    currTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (currTime > BOUNCE_TIME)
                {
                    currTime = 0;
                    expired = true;
                }

                if (expired)
                {
                    fired = false;
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

            if(!expired)
                entityShape.Draw(spriteBatch);
        }
    }
}
