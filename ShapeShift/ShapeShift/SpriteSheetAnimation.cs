using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    public class SpriteSheetAnimation : Animation
    {
        //Some Private Variables
        int frameCounter;
        int switchFrame; //Is essentially the speed or however many frame counts we would like to wait before switching the frame.

        const int SPRITE_WIDTH = 92; //With of the overall image, not just the shape (which would be half)

        Vector2 frames;
        Vector2 currentFrame; 

        private Shape shape;
        private Boolean isRepeatable = true;


        // Shape is pass a an argument in order to disable animations that are not repeatable
        // This kind of links the shape and its animations (animations can call methods in the shape)
        public SpriteSheetAnimation(Shape shape, Boolean isRepeatable)
        {
            this.shape = shape;
            this.isRepeatable = isRepeatable;
        }

        public SpriteSheetAnimation()
        {
           
        }

        public Vector2 Frames
        {
            set { frames = value; }
        }

        public Vector2 CurrentFrame
        {
            set { currentFrame = value; }
            get { return currentFrame; }
        }

        public int FrameWidth
        {
            get { return image.Width / (int)frames.X; }
        }

        public int FrameHeight
        {
            get { return image.Height / (int)frames.Y; }
        }

      
        public override void LoadContent(ContentManager Content, Texture2D image, string text, Vector2 position)
        {
            base.LoadContent(Content, image, text, position);
            frameCounter = 0;

            switchFrame = 40;

            int numFrames = image.Width / SPRITE_WIDTH;

            frames = new Vector2(numFrames,1);        
            currentFrame = new Vector2(0, 0);

            sourceRect = new Rectangle((int)currentFrame.X * FrameWidth, (int)currentFrame.Y * FrameHeight, FrameWidth, FrameHeight);
        }

        public void switchContent (ContentManager Content, Texture2D image, string text, Vector2 position){
            base.LoadContent(Content, image, text, position);

           
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            
                //let's us know that we need to switch the frame
                frameCounter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (frameCounter >= switchFrame)
                {
                    frameCounter = 0;
                    currentFrame.X++;

                    if (currentFrame.X * FrameWidth >= image.Width)
                    {
                       currentFrame.X = 0;
                       
                        // If this animation is not supposed to be repeated, disabled it by calling shape.disableAnimation
                        if (!isRepeatable)
                        {
                            shape.disableAnimation(this);
                            this.Update(gameTime);
                        }
                            
                    }
                }
          
            sourceRect = new Rectangle((int)currentFrame.X * FrameWidth, (int)currentFrame.Y * FrameHeight, FrameWidth, FrameHeight);          
        }
    }
}
