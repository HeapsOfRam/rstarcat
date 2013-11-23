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
    public class Entity
    {
        protected int health, maxHealth;
        protected SpriteSheetAnimation moveAnimation;
        protected float moveSpeed;
        protected Boolean colliding = false;

        protected ContentManager content;
        protected FileManager fileManager;

       // protected Texture2D image;

        protected List<List<string>> attributes, contents;

        protected Vector2 position;
        protected Vector2 previousPosition;
        protected Vector2 spawnPosition = new Vector2(55, 320);
        protected Vector2 leftSpawnPosition = new Vector2(50, 320);
        protected Vector2 rightSpawnPosition = new Vector2(750, 320);
        protected Vector2 topSpawnPosition;
        protected Vector2 bottomSpawnPosition;

        // spawnPosition = new Vector2(55, 320); //player spawns handled in entity
        // leftSpawnPosition = new Vector2(55, 320);
        // rightSpawnPosition = new Vector2(680, 320);


        protected Rectangle lastCheckedRectangle;

        protected Boolean collision = false;
        protected Boolean[] directions = new Boolean[4];
        protected bool exitsLevel = false;        

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

        public virtual void Update(GameTime gameTime, InputManager input, Collision col, Layers layer) //May need to be adjusted, as enemies don't need input
        {
            exitsLevel = false;   //resets the signal to switch levels to false
            for (int i = 0; i < col.CollisionMap.Count; i++)
            {
                for (int j = 0; j < col.CollisionMap[i].Count; j++)
                {

                    if (col.CollisionMap[i][j] == "x") //Collision against solid objects (ex: Tiles)
                    {

                        //Creates a rectangle that is the current tiles postion and size
                        lastCheckedRectangle = new Rectangle((int)(j * layer.TileDimensions.X), (int)(i * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));


                        Vector2 xPosition = new Vector2(position.X, moveAnimation.Position.Y);
                        Vector2 yPosition = new Vector2(moveAnimation.Position.X, position.Y);



                        if (getShape().collides(yPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                        {
                            position.Y = moveAnimation.Position.Y;
                        }

                        if (getShape().collides(xPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                        {
                            position.X = moveAnimation.Position.X;
                        }


                    }

                    if (col.CollisionMap[i][j] == "*") //Marks a level transition (ex: Tiles)
                    {

                        //Creates a rectangle that is the current tiles postion and size
                        lastCheckedRectangle = new Rectangle((int)(j * layer.TileDimensions.X), (int)(i * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));




                        //Calls Collides method in shape class, in which each shape will check collisions uniquely 
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
                    }

                }
            }
        }

        public virtual void Update(GameTime gameTime, Collision col, Layers layer, Entity player)
        {
            previousPosition = position;
            colliding = false;

            for (int i = 0; i < col.CollisionMap.Count; i++)
            {
                for (int j = 0; j < col.CollisionMap[i].Count; j++)
                {
                    if (col.CollisionMap[i][j] == "x" || col.CollisionMap[i][j] == "*")
                    {

                        //Creates a rectangle that is the current tiles postion and size
                        lastCheckedRectangle = new Rectangle((int)(j * layer.TileDimensions.X), (int)(i * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));

                        //Calls Collides method in shape class, in which each shape will check collisions uniquely 
                        if (getShape().collides(position, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                        {
                            position = moveAnimation.Position;
                            colliding = true;
                        }
                    }
                }
            }

            moveAnimation.Position = position;

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        { 
        
        }

        public virtual Boolean collides(Color[] data2, Rectangle rectangle2)
        {
            return collision;
        }

        public virtual Rectangle getRectangle() { return new Rectangle(); }

        public virtual Shape getShape() { return new Shape(); }

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
