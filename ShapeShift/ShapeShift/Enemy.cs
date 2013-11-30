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
        private const int WANDER = 1, CHASE = 2, ATTACK = 3, REELING = 4, FIND = 5;
        private int state = WANDER;
        private float currentTime = 0, countDuration = 10f, knockCurr = 0;
        private const float KNOCKDURATION = .5f;
        private Random rand;
        private int direction = 1;
        private const int WANDERSWITCH = 5, UP = 1, RIGHTUP = 2, RIGHT = 3, RIGHTDOWN = 4, DOWN = 5, LEFTDOWN = 6, LEFT = 7, LEFTUP = 8;
        protected Shape enemyShape;
        protected Boolean reeling, findX = false, findY = true;
        

        public override void LoadContent(ContentManager content, int matrixWidth, int matrixHeight)
        {
            base.LoadContent(content, matrixWidth, matrixHeight);
            rand = new Random();
        }


        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public Shape getEnemyShape() { return enemyShape; }

        public void wander(GameTime gameTime)
        {
            if (colliding)
                direction = rand.Next(UP, LEFTUP);
            if (currentTime > WANDERSWITCH)
            {
                currentTime = 0;
                direction = rand.Next(UP, LEFTUP);
            }

            switch (direction)
            {
                case UP:
                    moveUp(gameTime);
                    break;
                case RIGHTUP:
                    moveRightUp(gameTime);
                    break;
                case RIGHT:
                    moveRight(gameTime);
                    break;
                case RIGHTDOWN:
                    moveRightDown(gameTime);
                    break;
                case DOWN:
                    moveDown(gameTime);
                    break;
                case LEFTDOWN:
                    moveLeftDown(gameTime);
                    break;
                case LEFT:
                    moveLeft(gameTime);
                    break;
                case LEFTUP:
                    moveLeftUp(gameTime);
                    break;
                default:
                    moveUp(gameTime);
                    break;

            }
        }

        public Boolean isReeling()
        { return reeling; }

        public void makeReel()
        { reeling = true; }

        public void knockedAway(GameTime gameTime, Entity player)
        {
            reeling = true;
            if (player.getPositionX() < position.X)
                moveRight(gameTime);
            if (player.getPositionX() > position.X)
                moveLeft(gameTime);
            if (player.getPositionY() < position.Y)
                moveDown(gameTime);
            if (player.getPositionY() > position.Y)
                moveUp(gameTime);
        }

        private void chaseX(GameTime gameTime, Entity player)
        {
            if (player.getPositionX() < position.X)
                moveLeft(gameTime);
            if (player.getPositionX() > position.X)
                moveRight(gameTime);
        }

        private void chaseY(GameTime gameTime, Entity player)
        {
            if (player.getPositionY() < position.Y)
                moveUp(gameTime);
            if (player.getPositionY() > position.Y)
                moveDown(gameTime);
        }

        public void chase(GameTime gameTime, Entity player)
        {
            chaseX(gameTime, player);
            chaseY(gameTime, player);
        }

        private void findChase(GameTime gameTime, Entity player)
        {
            if (player.getPositionY() != position.Y)
            {
                findY = true;
                findX = false;
            }
            if (player.getPositionX() != position.X)
            {
                findX = true;
                findY = false;
            }
            if (findX)
                moveRight(gameTime);
            else
                chaseX(gameTime, player);
            if (findY)
                moveDown(gameTime);
            else
                chaseY(gameTime, player);
        }

        public void standStill()
        { }

        public override void Update(GameTime gameTime, Collision col, Layers layer, Entity player)
        {
            base.Update(gameTime, col, layer, player);

            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (reeling)
                state = REELING;

            switch (state)
            {
                case WANDER:
                    spotDist = 300;
                    wander(gameTime);
                    if (spot(player))
                        state = CHASE;
                    break;
                case CHASE:
                    spotDist = 800;
                    chase(gameTime, player);
                    if (!spot(player))
                        state = WANDER;
                    if(enemyShape.collides(position, player.getRectangle(), player.getShape().getColorData()))
                        state = ATTACK;
                    if (colliding)
                        state = FIND;
                    break;
                case ATTACK:
                    //standStill();
                    if(!enemyShape.collides(position, player.getRectangle(), player.getShape().getColorData()))
                        state = CHASE;
                    break;
                case REELING:
                    knockCurr += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    knockedAway(gameTime, player);
                    if (knockCurr > KNOCKDURATION)
                    {
                        state = CHASE;
                        reeling = false;
                        knockCurr = 0;
                    }
                    break;
                case FIND:
                    findChase(gameTime, player);
                    if (!colliding)
                        state = CHASE;
                    break;
                default:
                    wander(gameTime);
                    break;
            }

            /*if (!reeling)
            {
                if (spot(player))
                    chase(gameTime, player);
                else
                    wander(gameTime);
            }
            else
            {
                knockCurr += (float)gameTime.ElapsedGameTime.TotalSeconds;
                //position.X = position.Y = 250;
                knockedAway(gameTime, player);
                if (knockCurr > KNOCKDURATION)
                {
                    reeling = false;
                    knockCurr = 0;
                }
            }*/
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

    }
}
