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

        public void group()
        {

            grouped = !grouped;

            for (int i = 0; i < matrixWidth; i++)
            {
                for (int j = 0; j < matrixHeight; j++)
                {
                    tiles[i, j].group();

                    if (grouped)
                        tiles[i, j].setPosition(new Vector2 (tiles[0, 0].getPosition().X + (i * 28),tiles[0, 0].getPosition().Y + (j *28)));
                }
            }
            
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

        public override void Update(GameTime gameTime, Collision col, Layers layer, Entity player, List<Shape> bullets)
        {
            
            position = tiles[0, 0].getPosition();

                for (int i = 0; i < matrixWidth; i++)
                {
                    for (int j = 0; j < matrixHeight; j++)
                    {
                        tiles[i, j].Update(gameTime, col, layer, player,bullets);
                        
                        if (grouped)
                            tiles[i, j].getShape().setPosition(new Vector2(position.X + (i * 28), position.Y + (j * 28)));

                        foreach (Shape bullet in bullets)
                        {
                            if (!bullet.isDead()){
                                if (tiles[i, j].getShape().collides(tiles[i, j].getPosition(), bullet.getRectangle(), bullet.getColorData()))
                                {
                                    if (!tiles[i, j].isDead())
                                    {
                                        tiles[i, j].die();
                                        tiles[i, j].Update(gameTime, input, col, layer);

                                        if (!bullet.isDead())
                                        {
                                            bullet.hit();

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
        }

        public override Boolean isDead()
        {
           
            foreach (MatrixTileEnemy tile in tiles)
            {
                if (!tile.getShape().isDead())
                {
                    return false;
                }
            }

            return true;
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

        public override bool collides(Vector2 vector2, Rectangle rectangle, Color[] color)
        {
            foreach (MatrixTileEnemy e in tiles){
                if (e.getEnemyShape().collides(vector2, rectangle, color))
                    return true;
            }

            return false;
        }
    }
}
