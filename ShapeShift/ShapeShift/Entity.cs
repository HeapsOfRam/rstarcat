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

        protected ContentManager content;
        protected FileManager fileManager;

       // protected Texture2D image;

        protected List<List<string>> attributes, contents;

        protected Vector2 position;
        protected Vector2 previousPosition;

        protected Boolean collision = false;

        public virtual void LoadContent(ContentManager content, InputManager input)
        {
            this.content = new ContentManager(content.ServiceProvider, "Content");
            attributes = new List<List<string>>();
            contents = new List<List<string>>();
        }

        public virtual void UnloadContent()
        {
            content.Unload();
        }

        public virtual void Update(GameTime gameTime, InputManager input, Collision col, Layers layer) //May need to be adjusted, as enemies don't need input
        { 
        
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

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

    }
}
