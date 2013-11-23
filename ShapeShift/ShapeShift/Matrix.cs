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
        protected MatrixTile[,] tiles;

        protected Texture2D shadowTexture;

        protected const int POSITION_OFFSET   = 28;
        protected const int SWITCH_FRAME      = 150;
        protected const int TILE_WIDTH        = 23; //width of each square component of the matrix; dog

        protected int matrixHeight = 1;
        protected int matrixWidth = 1;

        protected Stack<MatrixTile> collidingTiles;

        
        protected Boolean collapse = false;

        protected Vector2 matrixCenter;

        protected Vector2 position;

        protected ContentManager content;     

        public Matrix(ContentManager content, int matrixWidth, int matrixHeight)
        {
            this.content = content;
            this.matrixWidth = matrixWidth;
            this.matrixHeight = matrixHeight;
            
            matrixCenter = new Vector2(matrixWidth * POSITION_OFFSET / 2, matrixHeight * POSITION_OFFSET / 2);


            animations = new List<SpriteSheetAnimation>();


            tiles = new MatrixTile[matrixWidth,matrixHeight];

            for (int i = 0; i < matrixWidth; i++)
            {
                for (int j = 0; j < matrixHeight; j++)
                {
                    tiles[i, j] = new MatrixTile(content, this, matrixWidth, matrixHeight, POSITION_OFFSET, new Point(i, j));

                    foreach (SpriteSheetAnimation s in tiles[i, j].getActiveTextures())
                        animations.Add(s);
                }
           }

           

            collidingTiles = new Stack<MatrixTile>();
        }

        public void attack()
        {
        }

        public override void setPosition(Vector2 position){
            this.position = position;
        }

        public void makeMatrix(Vector2 position)
        {

   
            for (int i = 0; i < matrixWidth; i++)
            {
                for (int j = 0; j < matrixHeight; j++)
                {
                    Vector2 newPos = position;

                    newPos.Y += j * POSITION_OFFSET;
                    newPos.X += i * POSITION_OFFSET;

                    tiles[i, j].setPosition(newPos);
                }
            }
        }

        public void makeMatrix()
        {


            for (int i = 0; i < matrixWidth; i++)
            {
                for (int j = 0; j < matrixHeight; j++)
                {
                    Vector2 newPos = tiles[i,j].getPosition();

                    newPos.Y += j * POSITION_OFFSET;
                    newPos.X += i * POSITION_OFFSET;

                    tiles[i, j].setPosition(newPos);
                }
            }
        }

        // Method accepts a boolean transformRotate, which determines whether the matrix will preform either of the two:
        // 1. (false) rotate in place for each tile
        // 2. (true) rotate in or out of a collapsed state
        public void PreformRotate(Boolean transformRotate)
        {
            foreach (MatrixTile tile in tiles)
                tile.ProformRotate(transformRotate);
        }

        public override Texture2D getTexture()
        {
            return tiles[0,0].getTexture();
        }

        public void Update(GameTime gameTime)
        {

            makeMatrix(position);

            foreach (MatrixTile tile in tiles)
                tile.Update(gameTime);
        }

        public override int getWidth()
        {
            return matrixWidth * TILE_WIDTH;
        }

        public override int getHeight()
        {
            return matrixHeight * TILE_WIDTH;
        }

        //Checks to see if there is a collision 
        public override bool collides(Vector2 position, Rectangle rectangleB, Color[] dataB)
        {

            Boolean collision = false;

            foreach (MatrixTile tile in tiles)
            {
                if (tile.collides(position,rectangleB,dataB)){
                    collision = true;
                    if (!collidingTiles.Contains(tile))
                        collidingTiles.Push(tile);
                }
                    
            }

            return collision;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (MatrixTile tile in tiles)
                tile.Draw(spriteBatch);
            
        }
    }
}