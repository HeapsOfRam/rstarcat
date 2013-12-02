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

        protected Texture2D idleTexture; // Contains all the different color animation textures
        protected Texture2D shadowTexture; // Shadow Texture of the tile
        protected Texture2D hitTexture;

        protected SpriteSheetAnimation idleAnimation; // idle animation ( image texture of animation changes as it cycles through colors)
        protected SpriteSheetAnimation hitAnimation;

        protected Boolean collapse = false; // True if the matrix is in collapsed state, flase otherwise
       
        protected float rotateSpeed = 15.0f;
        protected Vector2 TILE_ROTATE_CENTER = new Vector2(11.5f, 11.5f);

        protected Vector2 matrixCenter;
        protected ContentManager content;
        protected int matrixWidth;
        protected int matrixHeight;
        protected int positionOffset;

        protected Point matrixPosition;

        public MatrixTile(ContentManager content, int matrixWidth, int matrixHeight, int positionOffset, Point matrixPosition)
        {
            this.content = content;
            this.matrixWidth = matrixWidth;
            this.matrixHeight = matrixHeight;
            this.positionOffset = positionOffset;
            this.matrixPosition = matrixPosition;
            
            animations = new List<SpriteSheetAnimation>();

            matrixCenter = new Vector2(matrixWidth * positionOffset / 2, matrixHeight * positionOffset / 2);

            #region Create Textures & Animations
            idleTexture = content.Load<Texture2D>("Matrix/MatrixIdleSpriteSheet");
            hitTexture = content.Load<Texture2D>("Matrix/MatrixHitSpriteSheet");
            shadowTexture = content.Load<Texture2D>("Matrix/MatrixShadow");

            idleAnimation = new SpriteSheetAnimation(this, true, WIDTH, TILE_ROTATE_CENTER);
            idleAnimation.LoadContent(content, idleTexture, "", new Vector2(0, 0));
            idleAnimation.IsEnabled = true;

            hitAnimation = new SpriteSheetAnimation(this, false, WIDTH, TILE_ROTATE_CENTER);
            hitAnimation.LoadContent(content, hitTexture, "", new Vector2(0, 0));
            hitAnimation.IsEnabled = false;

            animations.Add(idleAnimation);
            animations.Add(hitAnimation);

            #endregion

            colorData = new Color[28 * 28];
            shadowTexture.GetData(colorData);
        }

        public void attack()
        {
        }

        public override void hit()
        {
            hitAnimation.IsEnabled = true;
        }

        public void PreformRotate(Boolean transformRotate)
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
            return idleTexture;
        }

        public override int getWidth()
        {
            return WIDTH;
        }

        public override int getHeight()
        {
            return HEIGHT;
        }

        public override Rectangle getRectangle()
        {
            return new Rectangle((int)idleAnimation.position.X, (int)idleAnimation.position.Y, WIDTH, HEIGHT);
        }

        public override bool collides(Vector2 position, Rectangle rectangleB, Color[] dataB)
        {
            
            Color[] dataA = new Color[28 * 28];
            shadowTexture.GetData(dataA);

            Rectangle rectangleA = new Rectangle((int)position.X, (int)position.Y, WIDTH, HEIGHT);
           
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

        public void Update(GameTime gameTime)
        {
            foreach (SpriteSheetAnimation s in animations)
            {
                if (s.IsEnabled)
                    s.Update(gameTime);
            }
        }
    }
}
