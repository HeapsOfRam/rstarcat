﻿using System;
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
        protected const int WANDER = 1, CHASE = 2, ATTACK = 3, REELING = 4, FIND = 5, CHASE_TURRET = 6, CHASE_MINE = 7, WRITHE = 8;
        public int state = WANDER;
        protected float currentTime = 0, countDuration = 10f, knockCurr = 0;
        protected const float KNOCKDURATION = .5f;
        protected Random rand;
        protected int direction = 1;
        protected const int WANDERSWITCH = 5, UP = 1, RIGHTUP = 2, RIGHT = 3, RIGHTDOWN = 4, DOWN = 5, LEFTDOWN = 6, LEFT = 7, LEFTUP = 8;
        protected Boolean reeling, findX = false, findY = true;

        protected const int TO_CENTER = 23;
        

        public override void LoadContent(ContentManager content, int matrixWidth, int matrixHeight)
        {
            base.LoadContent(content, matrixWidth, matrixHeight);
            rand = new Random();

            moveSpeed = 180f;
        }
        

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public Shape getEnemyShape() { return entityShape; }

        public void wander(GameTime gameTime)
        {
            if (colliding)
                direction = rand.Next(1,8);
            if (currentTime > rand.Next(4,8))
            {
                currentTime = 0;
                direction = rand.Next(1,8);
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

        public virtual void makeReel()
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
            if (player.getPositionX() + TO_CENTER < position.X)
                moveLeft(gameTime);
            if (player.getPositionX() + TO_CENTER> position.X)
                moveRight(gameTime);
        }

        private void chaseY(GameTime gameTime, Entity player)
        {
            if (player.getPositionY() + TO_CENTER< position.Y)
                moveUp(gameTime);
            if (player.getPositionY() + TO_CENTER > position.Y)
                moveDown(gameTime);
        }

        public void chase(GameTime gameTime, Entity player)
        {
            chaseX(gameTime, player);
            chaseY(gameTime, player);
        }

        private void findChase(GameTime gameTime, Entity player)
        {
            if (xCollide)
            {
                moveUp(gameTime);
                /*if (checkUp(gameTime, player))
                    moveUp(gameTime);
                if (checkDown(gameTime, player))
                    moveDown(gameTime);
                 */
            }
            else
                chaseY(gameTime, player);
                //chaseX(gameTime, player);
            if (yCollide)
            {
                moveRight(gameTime);
                /*
                if (checkRight(gameTime, player))
                    moveRight(gameTime);
                if (checkLeft(gameTime, player))
                    moveLeft(gameTime);
                 * */
            }
            else
                chaseX(gameTime, player);
                //chaseY(gameTime, player);

            /*if (xCollide)//player.getPositionY() != position.Y)
            {
                findX = true;
                findY = false;
            }
            if (yCollide)//player.getPositionX() != position.X)
            {
                findY = true;
                findX = false;
            }
            if (findX)
                moveRight(gameTime);
            else
                chaseX(gameTime, player);
            if (findY)
                moveUp(gameTime);
            else
                chaseY(gameTime, player);*/
        }

        public void standStill()
        { }

        public override Boolean isEnemy()
        { return true; }

        public override void Update(GameTime gameTime, Collision col, Layers layer, Entity entity, List<Shape> bullets)
        {
            base.Update(gameTime, col, layer, entity,  bullets);

            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (reeling)
                state = REELING;

            switch (state)
            {
                case WANDER:
                    spotDist = 300;
                    wander(gameTime);
                    if (entity.hasTurretDropped() && spot(entity.getTurret()))
                        state = CHASE_TURRET;
                    else if (entity.hasMineDropped() && spot(entity.getMine()))
                        state = CHASE_MINE;
                        else if (spot(entity))
                            state = CHASE;
                    break;
                case CHASE:
                    spotDist = 450;
                    chase(gameTime, entity);
                    if (!spot(entity))
                    {
                        direction = rand.Next(1, 8);
                        state = WANDER;
                    }
                    if (Math.Abs(position.X - entity.position.X) < 70 || Math.Abs(position.Y - lastCheckedRectangle.Y) < 70)
                    {
                        if (entityShape.collides(position, entity.getRectangle(), entity.getShape().getColorData()))
                            state = ATTACK;
                    }
                    if (colliding)
                        state = FIND;
                    if(entity.hasTurretDropped() && spot(entity.getTurret()))
                        state = CHASE_TURRET;
                    if (entity.hasMineDropped() && spot(entity.getMine()))
                        state = CHASE_MINE;
                    break;
                case ATTACK:
                    //standStill();
                    if (Math.Abs(position.X - entity.position.X) < 70 || Math.Abs(position.Y - lastCheckedRectangle.Y) < 70)
                    {
                        if (!entityShape.collides(position, entity.getRectangle(), entity.getShape().getColorData()))
                            state = CHASE;
                    }
                    else
                        state = CHASE;
                    break;
                case REELING:
                    knockCurr += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    knockedAway(gameTime, entity);
                    if (knockCurr > KNOCKDURATION)
                    {
                        state = CHASE;
                        reeling = false;
                        knockCurr = 0;
                    }
                    break;
                case FIND:
                    if (!spot(entity))
                        state = WANDER;
                    findChase(gameTime, entity);
                    if (!colliding)
                        state = CHASE;
                    break;
                case CHASE_TURRET:
                    if(!spot(entity.getTurret()) || !entity.hasTurretDropped())
                        state = WANDER;
                    chase(gameTime, entity.getTurret());
                    break;
                case CHASE_MINE:
                    if (!spot(entity.getMine()) || !entity.hasMineDropped())
                        state = WANDER;
                    chase(gameTime, entity.getMine());
                    break;
                case WRITHE:
                    wander(gameTime);
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


        public virtual bool collides(Vector2 vector2, Rectangle rectangle, Color[] color)
        {
            return false;
        }
    }
}
