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

        int spriteWidth = 92; //With of the overall image, not just the shape (which would be half)

        Vector2 frames;
        Vector2 currentFrame;

        Vector2 animationCenter;

        private Shape shape;
        private Boolean isRepeatable = true;

        private Boolean rotate = false;

        private int rotateCounter = 0;

        private GraphicsDeviceManager graphics;

        private float rotationSpeed;


        // Shape is pass a an argument in order to disable animations that are not repeatable
        // This kind of links the shape and its animations (animations can call methods in the shape)
        public SpriteSheetAnimation(Shape shape, Boolean isRepeatable)
        {
            this.shape = shape;
            this.isRepeatable = isRepeatable;
            this.animationCenter = origin;
        }

        public SpriteSheetAnimation(Shape shape)
        {
            this.shape = shape;
            this.isRepeatable = false;
            this.animationCenter = origin;
        }
        public SpriteSheetAnimation(Shape shape, Boolean isRepeatable, int spriteWidth)
        {
            this.shape = shape;
            this.isRepeatable = isRepeatable;
            this.spriteWidth = spriteWidth;
            this.animationCenter = origin;
        }

        public SpriteSheetAnimation(Shape shape, Boolean isRepeatable, int spriteWidth, Vector2 center)
        {
            this.animationCenter = center;
            this.shape = shape;
            this.isRepeatable = isRepeatable;
            this.spriteWidth = spriteWidth;
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

            int numFrames = image.Width / spriteWidth;

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

                if (rotate && rotateCounter < 15)
                {
                    origin = animationCenter;

                  
                    rotation += (float)Math.PI / rotationSpeed;
                    rotateCounter++;
                }

                else if (rotateCounter >= 15)
                {
                    rotateCounter = 0;
                    rotate = false;

                }


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

        public Color[] getCurrentImage(){
            
            Rectangle newBounds = new Rectangle((int)currentFrame.X * FrameWidth, (int)currentFrame.Y * FrameHeight, FrameWidth, FrameHeight);

           

            // Copy the data from the cropped region into a buffer, then into the new texture
            Color[] data = new Color[newBounds.Width * newBounds.Height];

            image.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);

            return data;

        }

        internal void PreformRotate(float rotationSpeed)
        {
            rotate = true;
            this.rotationSpeed = rotationSpeed;
        }
    }
}
