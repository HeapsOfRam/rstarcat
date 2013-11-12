﻿using System;
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

        private int matrixHeight    = 2;
        private int matrixWidth     = 2;
        private int offset          = 28;
        private int currentTexture  = 0;
        private int frameCounter;
        private int switchFrame;

        public Boolean collapse = false;

        private float rotateSpeed = 15.0f;

        private const int NUM_FRAMES = 36;

        private Vector2 TILE_ROTATE_CENTER = new Vector2(11.5f, 11.5f);

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
                    gridAnimations[i, j] = new SpriteSheetAnimation(this, true, 23, TILE_ROTATE_CENTER);
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

        // Method accepts a boolean transformRotate, which determines whether the matrix will preform either of the two:
        // 1. (false) rotate in place for each tile
        // 2. (true) rotate in or out of a collapsed state
        public void PreformRotate(Boolean transformRotate)
        {
            

            if (!gridAnimations[0, 0].rotate)
            {
                if (transformRotate)
                    collapse = !collapse;
                
                for (int i = 0; i < matrixWidth; i++)
                {
                    for (int j = 0; j < matrixHeight; j++)
                    {

                        // If the rotate requested was a transform (collapse or uncollapse):
                        // Then set the rotationSpeed of the rotate and pass false ( false = do NOT reset rotation to 0.0f following animation
                        // becuase if we are now collapsed, we would like to resume from that position when we uncollapse)
                        // And lastly, we change the animation center of each tile to a variable distance (which is decreasing) away from center of the matrix while each
                        // tile is rotating. This makes it look like the tiles are coming together in a spiral motion 
             
                        if (transformRotate)
                        {
                            gridAnimations[i, j].PreformRotate(rotateSpeed, false);
                            gridAnimations[i, j].setAnimationCenter(new Vector2(matrixCenter.X - 9.5f * i, matrixCenter.Y - 9.5f * j));

                        }

                        // If the rotate requested was an individual tile rotate and the matrix is not currently collapsed:
                        // Then pass the rotationSpeed of the rotate and true (true = reset rotation to 0 after completing animation)
                        // and change the animation center for each tile (point around which each tile rotates) to the center of that tile.
                        if (!transformRotate && !collapse)
                        {
                            gridAnimations[i, j].PreformRotate(rotateSpeed, true);
                            gridAnimations[i, j].setAnimationCenter(TILE_ROTATE_CENTER);
                        }
                    }
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
