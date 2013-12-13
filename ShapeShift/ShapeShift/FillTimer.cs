using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeShift
{
    class FillTimer
    {
        protected Texture2D[] fillTextures;

        protected const int HEIGHT = 46;
        protected const int WIDTH = 46;
        protected Vector2 position;

        protected int fillCount = 14;

        protected Texture2D drawTexture;

        private ContentManager content;

        public FillTimer(ContentManager content, String type)
        {
            this.content = content;
            // Create list that will hold animations
            fillTextures = new Texture2D [15];


            for (int i = 0; i < 15; i++)
            {
                fillTextures[i] = content.Load<Texture2D>("Fill_" + type + "/" + (15-i));
            }

            if (type.Equals("R"))
                position = new Vector2(5 * 46, -21);
            else
                position = new Vector2(5 * 46, 25);

            drawTexture = fillTextures[fillCount];
            
        }

     
        public  int getHeight() { return HEIGHT; }

        public  int getWidth() { return WIDTH; }

        public void incrementFill ()
        {
            drawTexture = fillTextures[fillCount];

            fillCount ++;

            if (fillCount >= 15)
                fillCount = 0;

        }
     

        public void Draw(SpriteBatch spriteBatch)
        {
            Color c = new Color(0, 0, 0);


            spriteBatch.Draw(drawTexture, position, Color.White * 1.0f);
        }

        public void fillPercentage(double percentage)
        {
            if (percentage < 100)
            {
                int x = (int)((percentage * 15) / 100);


                drawTexture = fillTextures[x];

              
            }

        }

        public void reset()
        {
            drawTexture = fillTextures[14];
        }
    }
}
