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

        private Vector2 velocity;

        private Random rand;

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
            rand = new Random();           
            velocity.X = randomVelocity();
            velocity.Y = randomVelocity();
        }

        public void lockToOwner()
        {
            position.X = owner.position.X + 0;
            position.Y = owner.position.Y + 0;
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

        public int randomVelocity()
        {
            int vel = rand.Next(50, 60);
            if (rand.Next(0, 2) == 1)
                return vel;
            return -vel;
        }

        public void moveBall()
        {
            position.X += velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            position.Y += velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Update(GameTime gameTime, InputManager input, Collision col, Layers layer)
        {
            /*this.gameTime = gameTime;
            this.input = input;
            this.col = col;
            this.layer = layer;

            detectCollision();*/

            if (!expired)
            {

                
                bCircle.Update(gameTime);

                previousPosition = position;

                if (deployed)
                    lockToOwner();
                if (fired)
                {
                    base.Update(gameTime, input, col, layer);

                    currTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //moveLeft(gameTime);
                    if (!colliding)
                    {
                        moveBall();
                    }
                    else
                    {
                        velocity.X = randomVelocity();
                        velocity.Y = randomVelocity();

                        moveBall();
                        //position.X += velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        //position.Y += velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        colliding = false;
                        //velocity.X = randomVelocity();
                        //velocity.Y = randomVelocity();
                    }
                }

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

                    animation.position = position;
                }

                moveAnimation.Position = position;                

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
