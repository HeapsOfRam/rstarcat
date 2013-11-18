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
                direction = rand.Next(1, 8);
            if (currentTime > 5)
            {
                currentTime = 0;
                direction = rand.Next(1, 8);
            }
            if (direction == 1)
            {
                moveUp(gameTime);
            }
            if (direction == 2)
            {
                moveUp(gameTime);
                moveRight(gameTime);
            }
            if (direction == 3)
            {
                moveRight(gameTime);
            }
            if (direction == 4)
            {
                moveRight(gameTime);
                moveDown(gameTime);
            }
            if (direction == 5)
            {
                moveDown(gameTime);
            }
            if (direction == 6)
            {
                moveDown(gameTime);
                moveLeft(gameTime);
            }
            if (direction == 7)
            {
                moveLeft(gameTime);
            }
            if (direction == 8)
            {
                moveUp(gameTime);
                moveLeft(gameTime);
            }
            /*if(currentTime < 5)
                moveRight(gameTime);
            if (currentTime > 5)
                moveUp(gameTime);
            if (currentTime > 6)
                moveLeft(gameTime);*/
        }

        public override void Update(GameTime gameTime, Collision col, Layers layer)
        {
            base.Update(gameTime, col, layer);
            //previousPosition = position;
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            wander(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

    }
}
