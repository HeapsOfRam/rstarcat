using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeShift
{
    class Matrix : Shape
    {
        private Texture2D[] tileTextures;
        private SpriteSheetAnimation[,] gridAnimations;

        private int matrixHeight    = 4;
        private int matrixWidth     = 4;
        private int offset          = 26;
        private int currentTexture  = 0;
        private int frameCounter;
        private int switchFrame;

        private float rotateSpeed = 12.0f;

        private const int NUM_FRAMES = 20;

        private Boolean playback = false;

        private ContentManager content;
        private Vector2 matrixCenter;

        public Matrix(ContentManager content)
        {
            this.content = content;

            frameCounter = 0;
            switchFrame = 2000;

            animations = new List<SpriteSheetAnimation>();

            tileTextures = new Texture2D[NUM_FRAMES];

            gridAnimations = new SpriteSheetAnimation[matrixWidth, matrixHeight];

            for (int i = 0; i < NUM_FRAMES; i++)
                tileTextures[i] = content.Load<Texture2D>("Matrix/MatrixSpriteSheet" + i); 

            for (int i = 0; i < matrixWidth; i++)
            {
                for (int j = 0; j < matrixHeight; j++)
                {
                    gridAnimations[i,j] = new SpriteSheetAnimation(this, true, 23, new Vector2(12.5f,12.5f));
                    gridAnimations[i,j].LoadContent(content, tileTextures[0], "", new Vector2(0, 0));
                    gridAnimations[i,j].IsEnabled = true;
                    animations.Add(gridAnimations[i,j]);
                }
            }

            matrixCenter = new Vector2(matrixWidth * offset/2, matrixHeight * offset/2);
        }

        public void attack()
        {
        }

        public void makeMatrix()
        {
            for (int i = 0; i < matrixWidth; i++)
            {
                for (int j = 0; j < matrixHeight; j++)
                {
                    gridAnimations[i, j].position.Y += j * offset;
                    gridAnimations[i, j].position.X += i * offset;
                }
            }
        }

        public void PreformRotate()
        {
            for (int i = 0; i < matrixWidth; i++)
            {
                for (int j = 0; j < matrixHeight; j++)
                {
                    gridAnimations[i, j].PreformRotate(rotateSpeed);
                    gridAnimations[i, j].setAnimationCenter (new Vector2(13f,13f));
                }

            }

        }
        public void PreformRotate(int type)
        {
            for (int i = 0; i < matrixWidth; i++)
            {
                for (int j = 0; j < matrixHeight; j++)
                {
                    gridAnimations[i, j].PreformRotate(rotateSpeed);
                    gridAnimations[i, j].setAnimationCenter(new Vector2( matrixCenter.X - 9.5f*i , matrixCenter.Y- 9.5f*j ));
                }

            }


            

        }

        public override Texture2D getTexture()
        {
            return tileTextures[0];
        }

        public void Update(GameTime gameTime)
        {

            frameCounter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (frameCounter >= switchFrame)
            {
                frameCounter = 0;

                for (int i = 0; i < matrixWidth; i++)
                    for (int j = 0; j < matrixHeight; j++)
                        gridAnimations[i, j].image = tileTextures[currentTexture];

                if (playback)
                    currentTexture--;
                else
                    currentTexture++;

                if (currentTexture >= NUM_FRAMES)
                {
                    playback = true;
                    currentTexture--;
                }

                if (currentTexture < 0)
                {
                    playback = false;
                    currentTexture = 0;
                }
            }
        }

        public override bool Collides(Vector2 position, Rectangle rectangle)
        {

            if (position.X + rectangle.Width * 2 < rectangle.X ||
                position.X > rectangle.X + rectangle.Width ||
                position.Y + rectangle.Height * 2 < rectangle.Y ||
                position.Y > rectangle.Y + rectangle.Height)
                return false;
            
            else
                return true;
        }
    }
}
