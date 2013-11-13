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
        protected Texture2D[] tileTextures;
        protected SpriteSheetAnimation[,] gridAnimations;

        protected Texture2D shadowTexture;

        protected const int MATRIX_HEIGHT     = 2;
        protected const int MATRIX_WIDTH      = 2;
        protected const int POSITION_OFFSET   = 28;
        protected const int NUM_FRAMES        = 36;
        protected const int SWITCH_FRAME      = 2000;

        protected int currentTexture = 0;
        protected int frameCounter = 0;
        
        protected Boolean collapse = false;
        protected Boolean playback = false;

        protected float rotateSpeed = 15.0f;

        protected Vector2 TILE_ROTATE_CENTER = new Vector2(11.5f, 11.5f);
        protected Vector2 matrixCenter;

        protected ContentManager content;
        
        public Matrix(ContentManager content)
        {
            this.content = content;

            matrixCenter = new Vector2(MATRIX_WIDTH * POSITION_OFFSET / 2, MATRIX_HEIGHT * POSITION_OFFSET / 2);

            #region Load Textures & Create Animations

            animations = new List<SpriteSheetAnimation>();

            tileTextures = new Texture2D[NUM_FRAMES];
            gridAnimations = new SpriteSheetAnimation[MATRIX_WIDTH, MATRIX_HEIGHT];

            for (int i = 0; i < NUM_FRAMES; i++)
                tileTextures[i] = content.Load<Texture2D>("Matrix/MatrixSpriteSheet" + i); 


            for (int i = 0; i < MATRIX_WIDTH; i++)
            {
                for (int j = 0; j < MATRIX_HEIGHT; j++)
                {
                    gridAnimations[i, j] = new SpriteSheetAnimation(this, true, 23, TILE_ROTATE_CENTER);
                    gridAnimations[i,j].LoadContent(content, tileTextures[0], "", new Vector2(0, 0));
                    gridAnimations[i,j].IsEnabled = true;
                    animations.Add(gridAnimations[i,j]);
                }
            }
            shadowTexture = content.Load<Texture2D>("Matrix/MatrixShadow");

            #endregion               
        }

        public void attack()
        {
        }

        public void makeMatrix()
        {
            for (int i = 0; i < MATRIX_WIDTH; i++)
            {
                for (int j = 0; j < MATRIX_HEIGHT; j++)
                {
                    gridAnimations[i, j].position.Y += j * POSITION_OFFSET;
                    gridAnimations[i, j].position.X += i * POSITION_OFFSET;
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
                
                for (int i = 0; i < MATRIX_WIDTH; i++)
                {
                    for (int j = 0; j < MATRIX_HEIGHT; j++)
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

            if (frameCounter >= SWITCH_FRAME)
            {
                frameCounter = 0;

                for (int i = 0; i < MATRIX_WIDTH; i++)
                    for (int j = 0; j < MATRIX_HEIGHT; j++)
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
            #region Messy shit
            if (!collapse)
            {
                Rectangle rectangleA;

                Color[] dataA = new Color[28 * 28];
                shadowTexture.GetData(dataA);
                Boolean collision = false;

                for (int i = 0; i < MATRIX_WIDTH; i++)
                {
                    for (int j = 0; j < MATRIX_HEIGHT; j++)
                    {
                        rectangleA = new Rectangle((int)position.X + i * POSITION_OFFSET, (int)position.Y + j * POSITION_OFFSET, 28, 28);
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

                for (int i = 0; i < MATRIX_WIDTH; i++)
                {
                    for (int j = 0; j < MATRIX_HEIGHT; j++)
                    {
                        rectangleA = new Rectangle((int)(position.X + (MATRIX_WIDTH * POSITION_OFFSET) + (i*10.0)) - 23 , (int)(position.Y + (MATRIX_HEIGHT * POSITION_OFFSET) +  (j * 10.0)) - 23, 28, 28);
                        if (IntersectPixels(rectangleA, dataA, rectangleB, dataB))
                            collision = true;
                    }
                }
                return collision;
            }
#endregion
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            List<SpriteSheetAnimation> animations = getActiveTextures();

            foreach (SpriteSheetAnimation s in animations)
            {
                if (s.IsEnabled)
                    s.Draw(spriteBatch);
            }
        }
    }
}
