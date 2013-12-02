using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    class MatrixEnemy : Enemy
    {
        private Random rand;
        private GameTime gameTime;
        private MatrixTileEnemy[,] tiles;

        private Boolean grouped = false;

        private int matrixWidth, matrixHeight;

        public MatrixEnemy(Vector2 position)
        {
            this.position = position;
        }

        public void group(Boolean grouped)
        {
            this.grouped = grouped;
        }

        public override void LoadContent(ContentManager content, int matrixWidth, int matrixHeight)
        {
            this.content = content;
            this.matrixWidth = matrixWidth;
            this.matrixHeight = matrixHeight;

            base.LoadContent(content, matrixWidth, matrixHeight);
            rand = new Random();

            tiles = new MatrixTileEnemy[matrixWidth, matrixHeight];

            for (int i = 0; i < matrixWidth; i++)
            {
                for (int j = 0; j < matrixHeight; j++)
                {
                    tiles[i,j] = new MatrixTileEnemy(new Point((matrixWidth * 28)/2,(matrixHeight * 28)/2), new Vector2 (position.X + (i * 28), position.Y + (j* 28)));
                    tiles[i, j].LoadContent(content, matrixWidth, matrixHeight);
                }
            }

            this.content = content;
            moveSpeed = 150f; 

            moveAnimation = new SpriteSheetAnimation();
            gameTime = new GameTime();

            
            moveAnimation.position = position;

          

            enemyShape = tiles[0,0].getShape();

        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            moveAnimation.UnloadContent();
        }

        public override Shape getShape()
        {
            return enemyShape;
        }

        public override void Update(GameTime gameTime, Collision col, Layers layer, Entity player)
        {
            
            position = tiles[0, 0].getPosition();

                for (int i = 0; i < matrixWidth; i++)
                {
                    for (int j = 0; j < matrixHeight; j++)
                    {
                        tiles[i, j].Update(gameTime, col, layer, player);
                        
                        if (grouped)
                            tiles[i, j].getShape().setPosition(new Vector2(position.X + (i * 28), position.Y + (j * 28)));
                    }
                }
           


        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            for (int i = 0; i < matrixWidth; i++)
            {
                for (int j = 0; j < matrixHeight; j++)
                {
                    tiles[i, j].Draw(spriteBatch);
                }
            }
        }

        public List<Enemy> getTileEnemies()
        {
            List<Enemy> returnList = new List<Enemy>();


            for (int i = 0; i < matrixWidth; i++)
            {
                for (int j = 0; j < matrixHeight; j++)
                {
                    returnList.Add(tiles[i, j]);
                }
            }

            return returnList;
        }
    }
}
