using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    class FadeAnimation : Animation
    {
        bool increase;
        float fadeSpeed;
        TimeSpan defaultTime, timer;
        float activateValue;
        bool stopUpdating;
        float defaultAlpha;

        public TimeSpan Timer
        {
            get { return timer; }
            set { defaultTime = value; timer = defaultTime; }
        }

        public float FadeSpeed
        {
            get { return fadeSpeed; }
            set { fadeSpeed = value; }
        }

        public override float Alpha
        {
            get
            {
                return alpha;
            }
            set
            {
                alpha = value;

                
                if (alpha == 1.0f)
                    increase = false;
                else if (alpha == 0.0f)
                    increase = true;
            }
        }

        public float ActivateValue
        {
            get { return activateValue; }
            set { activateValue = value; }
        }

        public bool Increase
        {
            get { return increase; }
            set { increase = value; }
        }

        public override void LoadContent(ContentManager Content, Texture2D image, string text, Vector2 position)
        {
            base.LoadContent(Content, image, text, position);
            increase = false;
            fadeSpeed = 1.0f;
            defaultTime = new TimeSpan(0, 0, 1);
            timer = defaultTime;
            activateValue = 0.0f;
            stopUpdating = false;
            defaultAlpha = alpha;
            //the way we are doing fading may be different for otherimages
            // we will have a black image, and we will change transparency based on fading In/Out
            
            //TEXT
            //when alpha is 255 -- we fully show something
            // alpha is 0 -- you cannot see the text
            
            //BLACK IMAGE
            //Transparency is zero - see everything
            //Transparency is 255 - Cant see

        }

        public override void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (!stopUpdating)
                {
                    if (!increase)
                        alpha -=  fadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    else
                        alpha += fadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (alpha <= 0.0f)
                    {
                        alpha = 0.0f;
                        increase = true;
                    }
                    else if (alpha >= 1.0f)  //1.0 being opaque, and 0.0 being fully transparent
                    {
                        alpha = 1.0f;
                        increase = false;
                    }
                }
                if (alpha == activateValue)
                {
                    stopUpdating = true;
                    timer -= gameTime.ElapsedGameTime;
                    if (timer.TotalSeconds <= 0)
                    {
                        timer = defaultTime;
                        stopUpdating = false;
                        //we want to wait a brief amount of time before the fade
                    }
                }

            }
            else 
            {
                alpha = defaultAlpha;
            }
        
        
        }

    }
}
