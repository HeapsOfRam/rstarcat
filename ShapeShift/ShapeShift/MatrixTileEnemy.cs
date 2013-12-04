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
            moveSpeed = 120f;

            moveAnimation = new SpriteSheetAnimation();
            moveAnimation.position = position;
            eMatrixTile.setPosition(position);

            enemyShape = eMatrixTile;
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
    }
}
