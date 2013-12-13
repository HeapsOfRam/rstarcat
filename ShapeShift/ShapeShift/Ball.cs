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

        private Effect effect;

        private Vector2 velocity;

        private Random rand;

        private Texture2D ballShadowTexture;

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
            bCircle.scaleShape(.75f);
            bCircle.setOrigin(new Vector2(46 / 2, 46 / 2));
            entityShape = bCircle;
            moveAnimation = new SpriteSheetAnimation();
            moveAnimation.position = position;
            rand = new Random();           
            velocity.X = randomVelocity();
            velocity.Y = randomVelocity();
            effect = content.Load<Effect>("normalmap");

            ballShadowTexture = content.Load<Texture2D>("Circle/CircleBallShadow");

            bCircle.setShadowTexture(ballShadowTexture);
        }

        public void lockToOwner()
        {
            position.X = owner.position.X + 6;
            position.Y = owner.position.Y + 6;
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

        public override void die()
        {
            expired = true;
            deployed = false;
            fired = false;
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
            this.gameTime = gameTime;
            this.input = input;
            this.col = col;
            this.layer = layer;

            if (!expired)
            {
                //base.Update(gameTime, input, col, layer);                

                bCircle.Update(gameTime);

                //previousPosition = position;

                if (deployed)
                    lockToOwner();
                if (fired)
                {
                    //base.Update(gameTime, input, col, layer);

                    currTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //moveLeft(gameTime);
                    /*if (xCollide && yCollide)
                    {
                        velocity.X = -velocity.X;
                        velocity.Y = -velocity.Y;
                        position.X += velocity.X;
                        position.Y += velocity.Y;
                    }
                    else
                    {*/
                        /*if (yCollide)// && previousPosition.X == position.X)
                        {
                            velocity.Y = -velocity.Y;
                        }
                        if(xCollide)
                        {
                            velocity.X = -velocity.X;
                        }*/
                        
                        position.X += velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (detectCollision())
                        {
                            velocity.X = -velocity.X;
                            position.X += velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        position.Y += velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (detectCollision())
                            velocity.Y = -velocity.Y;


                        //position.X += velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        //position.Y += velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        //velocity.X = randomVelocity();
                        //velocity.Y = randomVelocity();
                    //}

                    //moveBall();
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

            //Console.WriteLine("Ball" + updateCount);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            effect.CurrentTechnique.Passes[0].Apply();

            if (!expired)
                entityShape.Draw(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin();
            base.Draw(spriteBatch);
        }
    }
}
