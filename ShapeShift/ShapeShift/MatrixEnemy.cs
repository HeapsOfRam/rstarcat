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
        private Matrix eMatrix;

        private const int START_X = 500, START_Y = 500;

        private int matrixWidth, matrixHeight;

        public override void LoadContent(ContentManager content, int matrixWidth, int matrixHeight)
        {
            this.content = content;
            this.matrixWidth = matrixWidth;
            this.matrixHeight = matrixHeight;

            base.LoadContent(content, matrixWidth, matrixHeight);
            rand = new Random();

            eMatrix = new Matrix(content, matrixWidth, matrixHeight);

            this.content = content;
            moveSpeed = 150f; 

            moveAnimation = new SpriteSheetAnimation();
            gameTime = new GameTime();

            position = new Vector2(START_X, START_Y);
            moveAnimation.position = position;

            eMatrix.setPosition(position);

            enemyShape = eMatrix;

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
            base.Update(gameTime, col, layer, player);
            eMatrix.setPosition(position);
            eMatrix.Update(gameTime);
    
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            eMatrix.Draw(spriteBatch);
        }
    }
}
