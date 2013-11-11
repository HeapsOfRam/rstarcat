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

        private Texture2D shadowTexture;

        private int matrixHeight    = 4;
        private int matrixWidth     = 8;
        private int offset          = 28;
        private int currentTexture  = 0;
        private int frameCounter;
        private int switchFrame;

        public Boolean collapse = false;

        private float rotateSpeed = 15.0f;

        private const int NUM_FRAMES = 36;

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
                    gridAnimations[i,j] = new SpriteSheetAnimation(this, true, 23, new Vector2(14.0f,14.0f));
                    gridAnimations[i,j].LoadContent(content, tileTextures[0], "", new Vector2(0, 0));
                    gridAnimations[i,j].IsEnabled = true;
                    animations.Add(gridAnimations[i,j]);
                }
            }

            matrixCenter = new Vector2(matrixWidth * offset/2, matrixHeight * offset/2);


            shadowTexture = content.Load<Texture2D>("Matrix/MatrixShadow");

          
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
            if (!collapse)
            {
                for (int i = 0; i < matrixWidth; i++)
                {
                    for (int j = 0; j < matrixHeight; j++)
                    {
                        gridAnimations[i, j].PreformRotate(rotateSpeed);
                        gridAnimations[i, j].setAnimationCenter (new Vector2(11.5f,11.5f));
                    }

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

            collapse = !collapse;
            

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

        //Checks to see if there is a collision 
        public override bool Collides(Vector2 position, Rectangle rectangleB, Color[] dataB)
        {

            if (!collapse)
            {
                Rectangle rectangleA;

                Color[] dataA = new Color[28 * 28];
                shadowTexture.GetData(dataA);
                Boolean collision = false;

                for (int i = 0; i < matrixWidth; i++)
                {
                    for (int j = 0; j < matrixHeight; j++)
                    {
                        rectangleA = new Rectangle((int)position.X + i * offset, (int)position.Y + j * offset, 28, 28);
                        if (IntersectPixels(rectangleA, dataA, rectangleB, dataB))
                            collision = true;
                    }
                }

                return collision;
            }
            else
            {

                Rectangle rectangleA;

                Color[] dataA = new Color[28 * 28];
                shadowTexture.GetData(dataA);
                Boolean collision = false;

                for (int i = 0; i < matrixWidth; i++)
                {
                    for (int j = 0; j < matrixHeight; j++)
                    {
                        rectangleA = new Rectangle((int)(position.X + (matrixWidth * offset) + (i*10.0)) - 23 , (int)(position.Y + (matrixHeight * offset) +  (j * 10.0)) - 23, 28, 28);
                        if (IntersectPixels(rectangleA, dataA, rectangleB, dataB))
                            collision = true;
                    }
                }

                return collision;
            }



        }
    }
}
