using System;
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

        protected Rectangle lastCheckedRectangle;

        protected Boolean collision = false;

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
            //directions[0] = true;
        }

        public void moveLeft(GameTime gameTime)
        {
            position.X -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //directions[1] = true;
        }

        public void moveDown(GameTime gameTime)
        {
            position.Y += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //directions[2] = true;
        }

        public void moveUp(GameTime gameTime)
        {
            position.Y -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //directions[3] = true;
        }

        public virtual void Update(GameTime gameTime, InputManager input, Collision col, Layers layer) //May need to be adjusted, as enemies don't need input
        { 
        
        }

        public virtual void Update(GameTime gameTime, Collision col, Layers layer)
        {
            previousPosition = position;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        { 
        
        }

        public virtual Boolean collides(Color[] data2, Rectangle rectangle2)
        {
            return collision;
        }

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
