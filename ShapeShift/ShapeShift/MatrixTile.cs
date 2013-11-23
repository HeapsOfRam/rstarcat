using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace ShapeShift
{
    class MatrixTile : Shape
    {

        protected const int WIDTH        = 23;
        protected const int HEIGHT       = 23;
        protected const int NUM_FRAMES   = 26; // Corresponds to the number of color changes there are
        protected const int SWITCH_FRAME = 150; // Speed at which the color changes ( FAST < SLOW )

        protected int currentTexture = 0; // Current color texture being used 
        protected int frameCounter   = 0; // Time elapsed between frames

        protected Texture2D[] tileTextures; // Contains all the different color animation textures
        protected Texture2D shadowTexture; // Shadow Texture of the tile

        protected SpriteSheetAnimation idleAnimation; // idle animation ( image texture of animation changes as it cycles through colors)

        protected Boolean collapse = false; // True if the matrix is in collapsed state, flase otherwise
        protected Boolean playback = false; // True if the end of the color textures has been reached (loop backward to make smooother color flow)

        protected float rotateSpeed = 15.0f;
        protected Vector2 TILE_ROTATE_CENTER = new Vector2(11.5f, 11.5f);

        

        protected Vector2 matrixCenter;
        protected ContentManager content;
        protected Matrix matrix;
        protected int matrixWidth;
        protected int matrixHeight;
        protected int positionOffset;

        protected Point matrixPosition;


        public MatrixTile(ContentManager content, Matrix matrix, int matrixWidth, int matrixHeight, int positionOffset, Point matrixPosition)
        {

            this.content = content;
            this.matrix = matrix;
            this.matrixWidth = matrixWidth;
            this.matrixHeight = matrixHeight;
            this.positionOffset = positionOffset;
            this.matrixPosition = matrixPosition;
            animations = new List<SpriteSheetAnimation>();

            matrixCenter = new Vector2(matrixWidth * positionOffset / 2, matrixHeight * positionOffset / 2);

            tileTextures = new Texture2D[NUM_FRAMES];

            for (int i = 0; i < NUM_FRAMES; i++)
                tileTextures[i] = content.Load<Texture2D>("Matrix/MatrixSpriteSheet" + i);


            idleAnimation = new SpriteSheetAnimation(this, true, WIDTH, TILE_ROTATE_CENTER);
            idleAnimation.LoadContent(content, tileTextures[0], "", new Vector2(0, 0));
            idleAnimation.IsEnabled = true;

            animations.Add(idleAnimation);

            shadowTexture = content.Load<Texture2D>("Matrix/MatrixShadow");
        }

        public void attack()
        {
        }

        public void ProformRotate(Boolean transformRotate)
        {
            if (!idleAnimation.rotate)
            {
                if (transformRotate)
                {
                    collapse = !collapse;
                    idleAnimation.PreformRotate(rotateSpeed, false);
                    idleAnimation.setAnimationCenter(new Vector2(matrixCenter.X - 9.5f * matrixPosition.X, matrixCenter.Y - 9.5f * matrixPosition.Y));
                }

                if (!transformRotate && !collapse)
                {
                    idleAnimation.PreformRotate(rotateSpeed, true);
                    idleAnimation.setAnimationCenter(TILE_ROTATE_CENTER);
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

                
                idleAnimation.image = tileTextures[currentTexture];

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
        public override int getWidth()
        {
            return WIDTH;
        }

        public override int getHeight()
        {
            return HEIGHT;
        }

        public override bool collides(Vector2 position, Rectangle rectangleB, Color[] dataB)
        {
            Rectangle rectangleA;
            Color[] dataA = new Color[positionOffset * positionOffset];
            shadowTexture.GetData(dataA);

            if (!collapse)
                rectangleA = new Rectangle((int)position.X + matrixPosition.X * positionOffset, (int)position.Y + matrixPosition.Y * positionOffset, 28, 28);
            else
                rectangleA = new Rectangle((int)(position.X + (matrixWidth * positionOffset) + (matrixPosition.X * 10.0)) - 23, (int)(position.Y + (matrixHeight * positionOffset) + (matrixPosition.Y * 10.0)) - 23, 28, 28);

            return (IntersectPixels(rectangleA, dataA, rectangleB, dataB));
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

        public Vector2 getPosition()
        {
            return idleAnimation.Position;
        }
    }
}
