using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    class MatrixTileEnemy : Enemy
    {
        
        private GameTime gameTime;
        public Boolean collided = false;
        private MatrixTile eMatrixTile;

        private const int START_X = 500, START_Y = 500;

        private int matrixWidth, matrixHeight;
        
        private Point center;

        private const int POSITION_OFFSET = 28;

        public MatrixTileEnemy(Point center, Vector2 position)
        {
            this.center = center;
            this.position = position;
        }

        public override void LoadContent(ContentManager content, int matrixWidth, int matrixHeight)
        {
            this.content = content;
            this.matrixWidth = matrixWidth;
            this.matrixHeight = matrixHeight;


            
            gameTime = new GameTime();

            base.LoadContent(content, matrixWidth, matrixHeight);
            rand = new Random();

            eMatrixTile = new MatrixTile(content, matrixWidth, matrixHeight, POSITION_OFFSET, center);

            this.content = content;
            moveSpeed = 100f;

            moveAnimation = new SpriteSheetAnimation();
            moveAnimation.position = position;
            eMatrixTile.setPosition(position);

            entityShape = eMatrixTile;
        }

        public void setPosition(Vector2 position)
        {
            this.position = position;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            moveAnimation.UnloadContent();
        }

        public override Shape getShape()
        {
            return eMatrixTile;
        }


        public override void die()
        {
            eMatrixTile.die();
        }

        public override void Update(GameTime gameTime, Collision col, Layers layer, Entity player, List<Shape> bullets)
        {
        
            base.Update(gameTime, col, layer, player, bullets);
            eMatrixTile.setPosition(position);
            eMatrixTile.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            eMatrixTile.Draw(spriteBatch);
        }

        public void group()
        {
            eMatrixTile.group();
        }

        protected override void detectCollision()
        {
            exitsLevel = false;   //resets the signal to switch levels to false
            for (int i = 0; i < col.CollisionMap.Count; i++)
            {
                for (int j = 0; j < col.CollisionMap[i].Count; j++)
                {

                    if (col.CollisionMap[i][j] == "x") //Collision against solid objects (ex: Tiles)
                    {

                        //Creates a rectangle that is the current tiles postion and size
                        lastCheckedRectangle = new Rectangle((int)(j * layer.TileDimensions.X), (int)(i * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));

                        if (Math.Abs(position.X - lastCheckedRectangle.X) < 46 || Math.Abs(position.Y - lastCheckedRectangle.Y) < 46)
                        {

                            Vector2 xPosition = new Vector2(position.X, moveAnimation.Position.Y);
                            Vector2 yPosition = new Vector2(moveAnimation.Position.X, position.Y);



                            if (getShape().collides(yPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                            {
                                position.Y = moveAnimation.Position.Y;
                                colliding = true;
                            }

                            if (getShape().collides(xPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                            {
                                position.X = moveAnimation.Position.X;
                                colliding = true;
                            }
                        }

                    }

                    if (col.CollisionMap[i][j] == "*") //Marks a level transition (ex: Tiles)
                    {

                        //Creates a rectangle that is the current tiles postion and size
                        lastCheckedRectangle = new Rectangle((int)(j * layer.TileDimensions.X), (int)(i * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));



                        if (getShape().collides(position, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                        {
                            exitsLevel = true; //Boolean sent to GamePlayScreen. Update method will detect this, and then call map.loadContent

                            //Going through Right Door
                            if (position.X > 500)
                                position = leftSpawnPosition;
                            //Going through Left door
                            else if (position.X < 500)
                                position = rightSpawnPosition;

                        }

                        //Calls Collides method in shape class, in which each shape will check collisions uniquely 

                    }
                }
            }

        }

        public void ungroup()
        {
            eMatrixTile.ungroup();
        }
    }
}
