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
        private GameplayScreen gameplayScreen;
        
        

        private Boolean grouped = false;

        private int matrixWidth, matrixHeight;

        public MatrixEnemy(Vector2 position, GameplayScreen gameplayScreen)
        {
            this.gameplayScreen = gameplayScreen;
            this.position = position;
        }

       

        public void group()
        {

            grouped = true;

            for (int i = 0; i < matrixWidth; i++)
            {
                for (int j = 0; j < matrixHeight; j++)
                {
                    tiles[i, j].group();

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

          

            entityShape = tiles[0,0].getShape();

        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            moveAnimation.UnloadContent();
        }

        public override Shape getShape()
        {
            return new Shape();
            //return entityShape;
        }

        public override void Update(GameTime gameTime, Collision col, Layers layer, Entity player, List<Shape> bullets)
        {
            //base.Update(gameTime, col, layer, player, bullets);
            //position = tiles[0, 0].getPosition();

            
                //tiles[0, 0].setPosition(position);

                for (int i = 0; i < matrixWidth; i++)
                {
                    for (int j = 0; j < matrixHeight; j++)
                    {
                        if (!grouped)
                            tiles[i, j].Update(gameTime, col, layer, player, bullets);
                        else
                        {
                            Enemy b = new MatrixTileEnemy(new Point(0,0),tiles[0,0].getPosition());
                            b.LoadContent(content, 2, 2);
                            b.position = new Vector2(moveAnimation.position.X + (i * 28), moveAnimation.position.Y + (j * 28));
                            tiles[i, j].Update(gameTime, col, layer, b, bullets);
                            tiles[i, j].state = CHASE; 
                        }
                        

                        foreach (Shape bullet in bullets)
                        {
                            if (!bullet.isDead()){
                                if (Math.Abs(tiles[i, j].position.X - bullet.getActiveTextures()[0].position.X) < 70 || Math.Abs(tiles[i, j].position.Y - bullet.getActiveTextures()[0].position.Y) < 70)
                                {
                                    if (tiles[i, j].getShape().collides(tiles[i, j].getPosition(), bullet.getRectangle(), bullet.getColorData()))
                                    {
                                        if (!tiles[i, j].isDead())
                                        {
                                            gameplayScreen.IncreaseScore(100);
                                            tiles[i, j].die();


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
        }
        public override void makeReel()
        {
            foreach (MatrixTileEnemy e in tiles)
            {
                if (e.collided)
                    e.makeReel();
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
                if (!e.isDead())
                {
                    if (e.getEnemyShape().collides(e.getPosition(), rectangle, color))
                    {
                        e.collided = true;
                        return true;
                    }
                    e.collided = false;
                }
            }

            return false;
        }

        public void ungroup()
        {
            grouped = false;

            for (int i = 0; i < matrixWidth; i++)
            {
                for (int j = 0; j < matrixHeight; j++)
                {
                    tiles[i, j].ungroup();

                }
            }
        }
    }
}
