﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    //The basis for Players, enemies, beings, etc.
    class Entity
    {
        protected int health, maxHealth;
        protected SpriteSheetAnimation moveAnimation;
        protected float moveSpeed;
        protected Boolean colliding = false;

        protected ContentManager content;
        protected FileManager fileManager;

        protected int updateCount = 0;

       // protected Texture2D image;

        protected List<List<string>> attributes, contents;

        public Vector2 position;
        protected Vector2 previousPosition;
        protected Vector2 spawnPosition = new Vector2(55, 320);
        protected Vector2 leftSpawnPosition = new Vector2(50, 320);
        protected Vector2 rightSpawnPosition = new Vector2(750, 320);
        protected Vector2 topSpawnPosition;
        protected Vector2 bottomSpawnPosition;
        protected Shape entityShape;        

        public int spotRadius = 5, spotDist = 300;

        protected float damageTime = 3, invulnPeriod = 1f;

        protected Rectangle lastCheckedRectangle;

        protected Boolean xCollide = false, yCollide = false, stuck = false;

        protected Boolean collision = false;
        protected Boolean[] directions = new Boolean[4];
        protected bool exitsLevel = false;

        private const int EMPTY = 0;

        protected GameTime gameTime;
        protected InputManager input;
        protected Collision col;
        protected Layers layer;
        protected bool dead = false;

        public virtual void LoadContent(ContentManager content, InputManager input)
        {
            this.content = new ContentManager(content.ServiceProvider, "Content");
            attributes = new List<List<string>>();
            contents = new List<List<string>>();

        }

        public virtual void LoadContent(ContentManager content, int matrixWidth, int matrixHeight)
        {
            this.content = new ContentManager(content.ServiceProvider, "Content");
            attributes = new List<List<string>>();
            contents = new List<List<string>>();
        }

        public virtual void UnloadContent()
        {
            content.Unload();
        }


        public void moveRight(GameTime gameTime)
        {
            position.X += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            directions[0] = true;
        }

        public void moveLeft(GameTime gameTime)
        {
            position.X -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            directions[1] = true;
        }

        public void moveDown(GameTime gameTime)
        {
            position.Y += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            directions[2] = true;
        }

        public void moveUp(GameTime gameTime)
        {
            position.Y -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            directions[3] = true;
        }

        public void moveLeftUp(GameTime gameTime)
        {
            moveLeft(gameTime);
            moveUp(gameTime);
        }

        public void moveRightUp(GameTime gameTime)
        {
            moveRight(gameTime);
            moveUp(gameTime);
        }

        public void moveLeftDown(GameTime gameTime)
        {
            moveLeft(gameTime);
            moveDown(gameTime);
        }

        public void moveRightDown(GameTime gameTime)
        {
            moveRight(gameTime);
            moveDown(gameTime);
        }

        public Boolean checkLeft(GameTime gameTime, Entity entity)
        {
            Vector2 checkPosition = new Vector2(position.X + 0, position.Y + 0);
            checkPosition.X -= 10;
            return Vector2.Distance(checkPosition, entity.position) < Vector2.Distance(position, entity.position);
        }

        public Boolean checkRight(GameTime gameTime, Entity entity)
        {
            Vector2 checkPosition = new Vector2(position.X + 0, position.Y + 0);
            checkPosition.X += 10;
            return Vector2.Distance(checkPosition, entity.position) < Vector2.Distance(position, entity.position);
        }

        public Boolean checkDown(GameTime gameTime, Entity entity)
        {
            Vector2 checkPosition = new Vector2(position.X + 0, position.Y + 0);
            checkPosition.Y += 10;
            return Vector2.Distance(checkPosition, entity.position) < Vector2.Distance(position, entity.position);
        }

        public Boolean checkUp(GameTime gameTime, Entity entity)
        {
            Vector2 checkPosition = new Vector2(position.X + 0, position.Y + 0);
            checkPosition.X -= 10;
            return Vector2.Distance(checkPosition, entity.position) < Vector2.Distance(position, entity.position);
        }

        public virtual Boolean takeDamage()
        {
            //Console.WriteLine("Health = " + health);

            damageTime = 0;
            getShape().hit();
            health--;
            if (health <= EMPTY)
                die();

            return true;         
        }

        public virtual void die()
        {
            dead = true;
        }

        public virtual bool isDead()
        {
            return dead;
        }

        public virtual Boolean isEnemy()
        { return false;  }

        public virtual Entity getTurret(){
            return null;
        }

        public virtual Boolean hasTurretDropped()
        {
            return false;
        }

        public virtual Mine getMine()
        {
            return null;
        }

        public virtual Boolean hasMineDropped()
        {
            return false;
        }

        public Boolean spot(Entity e)
        {          
            float distanceFromEntity = Vector2.Distance(e.getPosition(), position);
            return distanceFromEntity < spotDist;
        }

        public bool ExitsLevel
        {
            get { return exitsLevel; }
        }


        public float getPositionX()
        { return position.X; }

        public float getPositionY()
        { return position.Y; }

        public Vector2 getPosition()
        { return position; }

        protected virtual Boolean detectCollision()
        {
            /*int yIndex = (int) position.Y / 46;
            int xIndex = (int) position.X / 46;

            Point[] surroundingTiles = new Point[9];

            surroundingTiles[0] = new Point(xIndex, yIndex);
            surroundingTiles[1] = new Point(xIndex - 1, yIndex - 1);
            surroundingTiles[2] = new Point(xIndex, yIndex - 1);
            surroundingTiles[3] = new Point(xIndex + 1, yIndex - 1);
            surroundingTiles[4] = new Point(xIndex + 1, yIndex);
            surroundingTiles[5] = new Point(xIndex + 1, yIndex + 1);
            surroundingTiles[6] = new Point(xIndex, yIndex + 1);
            surroundingTiles[7] = new Point(xIndex - 1, yIndex + 1);
            surroundingTiles[8] = new Point(xIndex - 1, yIndex);

            //if (!isEnemy())
            //{
                foreach (Point p in surroundingTiles)
                {
                    //Console.WriteLine(p);
                    if (p.X >= 0 && p.Y >= 0 && p.X < 15 && p.Y < 20)
                    {
                        if (col.CollisionMap[p.X][p.Y] == "x") //Collision against solid objects (ex: Tiles)
                        {

                            //Creates a rectangle that is the current tiles postion and size
                            lastCheckedRectangle = new Rectangle((int)(p.Y * layer.TileDimensions.X), (int)(p.X * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));

                            /*String type = this.GetType() + "";
                            int limit;

                            if (type.Equals("ShapeShift.MatrixEnemy"))
                        
                                limit = 70;
                        
                            else
                                limit = 20000;


                            //if (Math.Abs(position.X - lastCheckedRectangle.X) < limit || Math.Abs(position.Y - lastCheckedRectangle.Y) < limit)
                            //{
                            Vector2 xPosition = new Vector2(position.X, moveAnimation.Position.Y);
                            Vector2 yPosition = new Vector2(moveAnimation.Position.X, position.Y);

                            if (getShape().collides(yPosition, lastCheckedRectangle, layer.getColorData(p.X, p.Y, col.CollisionMap[p.X].Count)))
                            {
                                position.Y = moveAnimation.Position.Y;
                                colliding = true;
                                yCollide = true;
                                Console.WriteLine("Y" + position.Y);
                            }

                            if (getShape().collides(xPosition, lastCheckedRectangle, layer.getColorData(p.X, p.Y, col.CollisionMap[p.X].Count)))
                            {
                                position.X = moveAnimation.Position.X;
                                colliding = true;
                                xCollide = true;
                                Console.WriteLine("X" + position.X);
                            }

                            //}

                        }

                        if (col.CollisionMap[p.X][p.Y] == "*") //Marks a level transition (ex: Tiles)
                        {

                            //Creates a rectangle that is the current tiles postion and size
                            lastCheckedRectangle = new Rectangle((int)(p.Y * layer.TileDimensions.X), (int)(p.X * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));


                            if (getShape().collides(position, lastCheckedRectangle, layer.getColorData(p.X, p.Y, col.CollisionMap[p.X].Count)))
                            {
                                exitsLevel = true; //Boolean sent to GamePlayScreen. Update method will detect this, and then call map.loadContent

                                //Going through Right Door
                                if (position.X > 500)
                                    position = leftSpawnPosition;
                                //Going through Left door
                                else if (position.X < 500)
                                    position = rightSpawnPosition;

                            }
                        }
                    //}
                }
            }*/

            xCollide = false;
            yCollide = false;
            stuck = false;
            colliding = false;
            //Console.WriteLine(this.GetType());
            exitsLevel = false;   //resets the signal to switch levels to false
            for (int i = 0; i < col.CollisionMap.Count; i++)
            {
                for (int j = 0; j < col.CollisionMap[i].Count; j++)
                {

                    if (col.CollisionMap[i][j] == "x") //Collision against solid objects (ex: Tiles)
                    {
                        
                        //Creates a rectangle that is the current tiles postion and size
                        lastCheckedRectangle = new Rectangle((int)(j * layer.TileDimensions.X), (int)(i * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));

                        String type = this.GetType() + "";
                        int limit;

                        if (type.Equals("ShapeShift.Ball") || type.Equals("ShapeShift.MatrixEnemy"))

                            limit = 70;

                        else if (type.Equals("ShapeShift.MatrixTileEnemy"))
                            limit = 46;
                        else
                            limit = 20000;


                        if (Math.Abs(position.X - lastCheckedRectangle.X) < limit || Math.Abs(position.Y - lastCheckedRectangle.Y) < limit)
                        {
                            Vector2 xPosition = new Vector2(position.X, moveAnimation.Position.Y);
                            Vector2 yPosition = new Vector2(moveAnimation.Position.X, position.Y);

                            Vector2 yPrevious = new Vector2(position.X, previousPosition.Y);
                            Vector2 xPrevious = new Vector2(previousPosition.X, position.Y);

                            if (getShape().collides(yPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                            {
                                position.Y = moveAnimation.Position.Y;
                                colliding = true;
                                yCollide = true;

                                //Console.WriteLine("Y" + position.Y);
                                /*if (getShape().collides(previousPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                                    stuck = true;*/
                            }
                            else
                                previousPosition.Y = position.Y + 0;

                            if (getShape().collides(xPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                            {
                                position.X = moveAnimation.Position.X;
                                colliding = true;
                                xCollide = true;

                                if (getShape().collides(previousPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                                    stuck = true;
                                //Console.WriteLine("X" + position.X);
                            }
                            else
                                previousPosition.X = position.X + 0;

                        }

                    }

                    if (col.CollisionMap[i][j] == "*") //Marks a level transition (ex: Tiles)
                    {

                        //Creates a rectangle that is the current tiles postion and size
                        lastCheckedRectangle = new Rectangle((int)(j * layer.TileDimensions.X), (int)(i * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));


                            if (getShape().collides(position, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                            {
                                exitsLevel = true; //Boolean sent to GamePlayScreen. Update method will detect this, and then call map.loadContent

                                //Going through Right Door
                                if (position.X > 500)
                                    position = leftSpawnPosition;
                                //Going through Left door
                                else if (position.X < 500)
                                    position = rightSpawnPosition;

                            }
                        
                        //Calls Collides method in shape class, in which each shape will check collisions uniquely 
                        
                    }
                }
            }

            return colliding;
        
        }

        public virtual void Update(GameTime gameTime, InputManager input, Collision col, Layers layer) //May need to be adjusted, as enemies don't need input
        {
            updateCount++;
            //previousPosition = position;
            colliding = false;

            this.gameTime = gameTime;
            this.input = input;
            this.col = col;
            this.layer = layer;
            detectCollision();

            moveAnimation.Position = position;
        }

        public virtual void Update(GameTime gameTime, Collision col, Layers layer, Entity entity, List<Shape> bullets)
        {
            updateCount++;

            //previousPosition = position;
            //colliding = false;

            this.gameTime = gameTime;
            this.col = col;
            this.layer = layer;
            detectCollision();

            moveAnimation.Position = position;
            
            /*
            for (int i = 0; i < col.CollisionMap.Count; i++)
            {
                for (int j = 0; j < col.CollisionMap[i].Count; j++)
                {
                    if (col.CollisionMap[i][j] == "x" || col.CollisionMap[i][j] == "*")
                    {

                        lastCheckedRectangle = new Rectangle((int)(j * layer.TileDimensions.X), (int)(i * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));


                        Vector2 xPosition = new Vector2(position.X, moveAnimation.Position.Y);
                        Vector2 yPosition = new Vector2(moveAnimation.Position.X, position.Y);



                        if (getShape().collides(yPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                        {
                            position.Y = moveAnimation.Position.Y;
                            colliding = true;
                        }

                        if (getShape().collides(xPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                        {
                            position.X = moveAnimation.Position.X;
                            colliding = true;
                        }

                    }
                }
            }*/
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        { 
        
        }

        public virtual Boolean collides(Color[] data2, Rectangle rectangle2)
        {
            return collision;
        }

        public virtual Rectangle getRectangle() { return new Rectangle(); }

        public virtual Shape getShape() { return entityShape; }//return new Shape(); }


        public void setMoveSpeed(float moveSpeed)
        {
            this.moveSpeed = moveSpeed;
        }

        public int getHealth()
        { return health; }

        public int getMaxHealth()
        { return maxHealth; }

        public int takeDamage(int damage)
        {
            health -= damage;
            return health;
        }

        /*public void setPositionY(float y)
        { position.Y = y; }

        public void setPositionX(float x)
        { position.X = x; }

        public float getPositionY()
        { return position.Y; }

        public float getPositionX()
        { return position.X; }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }*/

    }
}
