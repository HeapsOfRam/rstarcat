using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    class Enemy : Entity
    {
        private const String WANDER = "Wander";
        private float currentTime = 0, countDuration = 1f;
        private Random rand;
        private int direction = 1;
        private const int WANDERSWITCH = 5, UP = 1, RIGHTUP = 2, RIGHT = 3, RIGHTDOWN = 4, DOWN = 5, LEFTDOWN = 6, LEFT = 7, LEFTUP = 8;
        protected int spotRadius = 5, spotDist = 300;

        public virtual void LoadContent(ContentManager content, int matrixWidth, int matrixHeight)
        {
            base.LoadContent(content, matrixWidth, matrixHeight);
            rand = new Random();
        }


        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public void wander(GameTime gameTime)
        {
            if (colliding)
                direction = rand.Next(UP, LEFTUP);
            if (currentTime > WANDERSWITCH)
            {
                currentTime = 0;
                direction = rand.Next(UP, LEFTUP);
            }
            if (direction == UP)
                moveUp(gameTime);
            if (direction == RIGHTUP)
                moveRightUp(gameTime);
            if (direction == RIGHT)
                moveRight(gameTime);
            if (direction == RIGHTDOWN)
                moveRightDown(gameTime);;
            if (direction == DOWN)
                moveDown(gameTime);
            if (direction == LEFTDOWN)
                moveLeftDown(gameTime);
            if (direction == LEFT)
                moveLeft(gameTime);
            if (direction == LEFTUP)
                moveLeftUp(gameTime);
        }

        public void chase(GameTime gameTime, Entity player)
        {
            if (player.getPositionX() < position.X)
                moveLeft(gameTime);
            if (player.getPositionX() > position.X)
                moveRight(gameTime);
            if (player.getPositionY() < position.Y)
                moveUp(gameTime);
            if (player.getPositionY() > position.Y)
                moveDown(gameTime);
        }

        public Boolean spot(Entity e)
        {
            float distanceFromEntity = Vector2.Distance(e.getPosition(), position);
            return distanceFromEntity < spotDist;
        }

        public override void Update(GameTime gameTime, Collision col, Layers layer, Entity player)
        {
            base.Update(gameTime, col, layer, player);
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (spot(player))
                chase(gameTime, player);
            else
                wander(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

    }
}
