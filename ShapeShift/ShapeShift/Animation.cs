﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{

    //a base class for all animation classes
    public class Animation
    {
    
        protected string text;
        protected SpriteFont font;
        protected Color color;
        protected Rectangle sourceRect;
        public float rotation, scale, axis;
        public Vector2 origin, position;
        protected ContentManager content;
        protected bool isActive;
        protected float alpha;

        public Texture2D image;

        protected bool isEnabled;

        public virtual float Alpha //we will override, hence the virtual
        {
            get { return alpha; }
            set { alpha = value; }
        }

        public bool IsActive
        {
            set { isActive = value; }
            get { return isActive; }
        
        }

        public bool IsEnabled
        {
            set { isEnabled = value; }
            get { return isEnabled; }

        }
        //scale is the width/height of the screen
        public float Scale
        {
            set { scale = value; }

        }

        public void setImage(Texture2D image)
        {
            this.image = image;
        }

        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        // If you use a constructor then you'll have to create a new instance of the class in order to reset the values of the class.
        //if you need to reload the content or reset the content then you can do so by calling the LoadContent method. 
        public virtual void LoadContent(ContentManager Content, Texture2D image, string text, Vector2 position)
        {
            content = new ContentManager(Content.ServiceProvider, "Content"); //he says if we don't need this in the future he might change it
            
            this.image    = image;
            this.text     = text;
            this.position = position;

            rotation  = 0.0f;
            axis      = 0.0f;
            scale     = 1.0f;
            isActive  = false;
            isEnabled = false;
            alpha     = 1.0f;
            
            if (text != String.Empty)
            {
                font  = this.content.Load<SpriteFont>("Fonts/Font1");
                color = new Color(114,77,255);
            }
            if (image != null) //so, if you have an image (animations can be image or text animations)
                sourceRect = new Rectangle(0,0,image.Width, image.Height);

            origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);

            

        }

        public virtual void UnloadContent()
        {
           // content.Unload(); 
            //^^^^Not sure if problematic. Due to a null pointer, I commented this out so that GameOver would load a titleScreen.
            //I believe it was not unloading because there was nothing left to unload.
           
            text = String.Empty;
            position = Vector2.Zero;
            sourceRect = Rectangle.Empty;
            image = null;
            
            
        }

        public virtual void Update(GameTime gameTime)
        {
        //Each animation will update things differently
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (image != null)
            {  
                spriteBatch.Draw(image, position + origin, sourceRect, Color.White * alpha, rotation, origin, scale, SpriteEffects.None,0.0f);
            }

            if (text != String.Empty)
            { 
                origin = new Vector2(font.MeasureString(text).X / 2, font.MeasureString(text).Y/2);
                spriteBatch.DrawString(font, text, position + origin, color * alpha, rotation, origin,scale, SpriteEffects.None, 0.0f);
            }

        }


    }
}
