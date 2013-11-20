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
            moveSpeed = 150f; //Set the move speed

            moveAnimation = new SpriteSheetAnimation();
            gameTime = new GameTime();

            position = new Vector2(START_X, START_Y);

            //each shape should have silhouette shape of its own
            //moveAnimation.LoadContent(content, playerShape.getTexture(), "", position);

            eMatrix.setPosition(position);

            eMatrix.makeMatrix();

        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            moveAnimation.UnloadContent();
        }

        public override void Update(GameTime gameTime, Collision col, Layers layer, Entity player)
        {
            base.Update(gameTime, col, layer, player);
            colliding = false;

            for (int i = 0; i < col.CollisionMap.Count; i++)
            {
                for (int j = 0; j < col.CollisionMap[i].Count; j++)
                {
                    if (col.CollisionMap[i][j] == "x")
                    {

                        //Creates a rectangle that is the current tiles postion and size
                        lastCheckedRectangle = new Rectangle((int)(j * layer.TileDimensions.X), (int)(i * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));
                        
                        //Calls Collides method in shape class, in which each shape will check collisions uniquely 
                        if (eMatrix.Collides(position, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                        {
                            position = moveAnimation.Position;
                            colliding = true;
                            //eMatrix.hit();
                        }
                    }
                }
            }

            // moveAnimation is used to check collisions, it is not drawn and is the same for each shape 
            // (just a rectangle corresponding to the image)
            moveAnimation.Position = position;

            // Update all of the enabled animations
            List<SpriteSheetAnimation> Animations = eMatrix.getActiveTextures();

            eMatrix.Update(gameTime);
            foreach (SpriteSheetAnimation animation in Animations)
            {
                if (animation.IsEnabled)
                {
                    animation.Update(gameTime);
                    animation.position = position;
                }
            }
            
            eMatrix.makeMatrix();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Draws each of the enabled animations for the current shape
            eMatrix.Draw(spriteBatch);
        }
    }
}
