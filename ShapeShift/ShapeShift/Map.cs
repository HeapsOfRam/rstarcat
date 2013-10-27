using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    public class Map
    {
        //we will change their visibility later
        public Layers layer;
        public Collision collision;

        public void LoadContent(ContentManager content, string mapID)
        {
        layer = new Layers();
        collision = new Collision();

        layer.LoadContent(content, mapID);
        collision.LoadContent(content, mapID);


        }

        public void UnloadContent()
        {
        layer.UnloadContent();
        
        }
        public void Update(GameTime gameTime)
        { 
        
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            layer.Draw(spriteBatch);
        }






    }
}
