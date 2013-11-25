using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ShapeShift
{
    public class Player : Entity
    {
        GameTime gameTime;

        private const int FULL = 3, MID = 2, LOW = 1, EMPTY = 0, SIZE = 60, MOVE = 5;
        private const float SHIELD_TIME = 5f;
        private float shieldDuration = 0;
       // private Rectangle rectangle;
        private Shape playerShape, nextShape;
        private Square pSquare;
        private Circle pCircle;
        private Triangle pTriangle;
        private Diamond pDiamond;
        private ContentManager content;
        
        private Random rand;

        private int r;

        private Matrix pMatrix;

        private const int START_X = 200, START_Y = 200;

        private  Collision col;
        private  Layers layer;
        private InputManager input;

        /* Player Spawns are currently handled in entity
        private Vector2 leftSpawnPosition;
        private Vector2 rightSpawnPosition;
        private Vector2 topSpawnPosition;
        private Vector2 bottomSpawnPosition;
        */

        public override void LoadContent(ContentManager content, InputManager input)
        {
            this.content = content;
            base.LoadContent(content, input);
            rand = new Random();

            this.content  = content;
            moveSpeed     = 150f; //Set the move speed

            moveAnimation = new SpriteSheetAnimation();
            moveAnimation.position = new Vector2(START_X, START_Y);

            gameTime      = new GameTime();

            //declare the shapes
            pSquare   = new Square(content);
            pCircle   = new Circle(content);
            pTriangle = new Triangle(content);
            pDiamond  = new Diamond(content);
            pMatrix   = new Matrix(content, 6, 6);

            maxHealth = FULL;
            health    = maxHealth;

            playerShape = pSquare;   //********STARTING SHAPE*******         

            //Queue up the next shape
            queueOne();
            position = new Vector2(START_X, START_Y);

            //each shape should have silhouette shape of its own
            //moveAnimation.LoadContent(content, playerShape.getTexture(), "", position);
            
            playerShape.setPosition(position);
            
            pMatrix.makeMatrix();
           // spawnPosition = new Vector2(55, 320); //player spawns handled in entity
           // leftSpawnPosition = new Vector2(55, 320);
           // rightSpawnPosition = new Vector2(680, 320);



        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            moveAnimation.UnloadContent();
        }

        #region MovementMethods

        private void queueOne()
        {
           do
            {
                r = rand.Next(4) + 1;
                if (r == 1)
                    nextShape = pCircle;
                if (r == 2)
                    nextShape = pSquare;
                if (r == 3)
                    nextShape = pDiamond;
                if (r == 4)
                    nextShape = pTriangle;
            } while (nextShape == playerShape);        
  

        }

        public Boolean rotating()
        {
            return playerShape == pTriangle && pTriangle.rotating();
        }

        public Boolean shielded()
        {
            return playerShape == pCircle && pCircle.isShielded();
        }

        public override Boolean takeDamage()
        {
            if (!(rotating() || shielded()))
                return base.takeDamage();
            else
                return false;
        }

        public void restoreHealth()
        {
            health = maxHealth;
        }

        public override void die()
        {
        }

        public void shiftShape()
        {

            moveSpeed = 150f;
            pSquare.stopDashing();

            if (playerShape == pCircle && pCircle.shielded)
                pCircle.removeShield();     
            else if (playerShape == pSquare && pSquare.dashing)
                pSquare.stopDashing();
            else if (playerShape == pDiamond)
                pDiamond.clearMines();

            playerShape = nextShape;

            if (playerShape == pTriangle)
                playerShape.setOrigin(new Vector2(45.6667f, 53.6667f));
            else
                playerShape.setOrigin(new Vector2(46, 46));

            pushOut (playerShape);
            

            nextShape   = null;
            
            queueOne();

            //Resets the locaiton of the next shape to the upper right corner
            List<SpriteSheetAnimation> Animations = nextShape.getActiveTextures();
            foreach (SpriteSheetAnimation animation in Animations)
            {
                if (animation.IsEnabled)
                    animation.position = new Vector2(0, 0); 
            }

            Update(gameTime,input,col,layer);
        }

        private void pushOut(Shape playerShape)
        {

             for (int i = 0; i < col.CollisionMap.Count; i++)
            {
                for (int j = 0; j < col.CollisionMap[i].Count; j++)
                {                   

                    if (col.CollisionMap[i][j] == "x") //Collision against solid objects (ex: Tiles)
                    {
                                
                        //Creates a rectangle that is the current tiles postion and size
                        lastCheckedRectangle = new Rectangle((int)(j * layer.TileDimensions.X), (int)(i * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));


                        Vector2 xPosition = new Vector2(position.X, position.Y);
                        Vector2 yPosition = new Vector2(position.X, position.Y);      

                        int count = 0;

                        while (playerShape.collides(xPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                        {

                            xPosition.X += count;

                            if (!playerShape.collides(xPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                                break;

                            xPosition.X -= count*2;

                            if (!playerShape.collides(xPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                                break;

                            xPosition.X += count;

                            count ++;

                        }
                        count = 0;

                        while (playerShape.collides(yPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                        {

                            yPosition.Y += count;

                            if (!playerShape.collides(yPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                                break;

                            yPosition.Y -= count*2;

                            if (!playerShape.collides(yPosition, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                                break;

                            yPosition.Y += count;

                            count ++;

                        }

                        position = new Vector2(xPosition.X, yPosition.Y);
                        playerShape.setPosition( new Vector2 (xPosition.X,yPosition.Y));


                    }
                }
             }

        }

        #endregion

        // Checks to see if the next shape is colliding with anything before switching 
        // If it is...it incremently trys moving the shape backward until it isn't colliding
       /* public void fixCollision(Vector2 position, Rectangle lastCheckedRectangle)
        {
            int count = 1;
            while (playerShape.Collides(position, lastCheckedRectangle))
            {
                if (!(playerShape.Collides(new Vector2(position.X + count, position.Y + count), lastCheckedRectangle)))
                    position = new Vector2(position.X + count, position.Y + count);
                else if (!(playerShape.Collides(new Vector2(position.X - count, position.Y - count), lastCheckedRectangle)))
                    position = new Vector2(position.X + count, position.Y + count);
                else if (!(playerShape.Collides(new Vector2(position.X - count, position.Y + count), lastCheckedRectangle)))
                    position = new Vector2(position.X + count, position.Y + count);
                else if (!(playerShape.Collides(new Vector2(position.X + count, position.Y - count), lastCheckedRectangle)))
                    position = new Vector2(position.X + count, position.Y + count);

                count++;
            }
        }*/

        /*public void moveRight(GameTime gameTime)
        {
            position.X += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            directions[0] = true;
        }

        public void moveLeft(GameTime gameTime)
        {
            position.X -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            directions[1] = true;
        }

        public void moveDown(GameTime gameTime)
        {
            position.Y += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            directions[2] = true;
        }

        public void moveUp(GameTime gameTime)
        {
            position.Y -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            directions[3] = true;
        }*/

        public override Shape getShape() { return playerShape; }

        public override Rectangle getRectangle() { return new Rectangle((int) position.X, (int) position.Y, playerShape.getWidth(), playerShape.getHeight()); }

        public void rAction()
        {
            if (playerShape == pCircle)
                pdeployShield();
            if (playerShape == pSquare)
                pDash();
            if (playerShape == pMatrix || playerShape == pTriangle)
                pRotate();
            if (playerShape == pDiamond)
                pDiamond.deployMine();
        }

        public void eAction()
        {
            if (playerShape == pCircle)
            {
                premoveShield();
                pCircle.hit();
            }
            if (playerShape == pDiamond)
            {
                //pDiamond.deployTurret();
                //pDiamond.hit();
            }
            if (playerShape == pMatrix)
                pMRotate();
            if (playerShape == pTriangle)
                pTriangle.hit();

        }


     

        public void shoot(GameTime gametime, int direction)
        {

            if (playerShape == pSquare)
            {
                switch (direction)
                {
                    case 1: 
                        pSquare.shoot(0);
                        break;
                    case 4: 
                        pSquare.shoot(90);
                        break;
                    case 2: 
                        pSquare.shoot(180);
                        break;
                    case 3: 
                        pSquare.shoot(270);
                        break;
                    default:
                        pSquare.shoot(0);
                        break;
                }
            }


            if (playerShape == pDiamond)
            {
                switch (direction)
                {
                    case 1:
                        pDiamond.shoot(39);
                        break;
                    case 4:
                        pDiamond.shoot(141);
                        break;
                    case 2:
                        pDiamond.shoot(219);
                        break;
                    case 3:
                        pDiamond.shoot(321);
                        break;
                    default:
                        pDiamond.shoot(39);
                        break;
                }
            }

            if (playerShape == pTriangle)
            {
                switch (direction)
                {
                    case 1:
                        pTriangle.shoot(0);
                        break;
                    case 4:
                        pTriangle.shoot(90);
                        break;
                    case 2:
                        pTriangle.shoot(180);
                        break;
                    case 3:
                        pTriangle.shoot(270);
                        break;
                    default:
                        pTriangle.shoot(0);
                        break;
                }
            }

            if (playerShape == pCircle)
            {
                switch (direction)
                {
                    case 1:
                        pCircle.shoot(0);
                        break;
                    case 4:
                        pCircle.shoot(90);
                        break;
                    case 2:
                        pCircle.shoot(180);
                        break;
                    case 3:
                        pCircle.shoot(270);
                        break;
                    default:
                        pCircle.shoot(0);
                        break;
                }
            }
        }


        private void pdeployShield()
        {
            if(!pCircle.shielded)
                pCircle.deployShield();
        }

        private void premoveShield()
        {
            if(pCircle.shielded)
                pCircle.removeShield();
            shieldDuration = 0;
        }

        private void pDash()
        {
            pSquare.dash(this);
        }

        private void pSquareShoot(int angle)
        {
            pSquare.shoot(angle);
        }

        private void pDiamondShoot(int angle)
        {
            pDiamond.shoot(angle);
        }

        private void pTriangleShoot(int angle)
        {
            pTriangle.shoot(angle);
        }

        private void pRotate()
        {
            if(playerShape == pTriangle)
                pTriangle.PreformRotate();
            if(playerShape == pMatrix)
                pMatrix.PreformRotate(false);
        }

        private void pMRotate()
        {
                pMatrix.PreformRotate(true);
        }

        private void pdeployMine()
        {
            pDiamond.deployMine();
        }
        private void pdeployTurret()
        {
            pDiamond.deployTurret();
        }

        private void pdropMine()
        {
            pDiamond.dropMine();
        }

        public void manualChange(int n)
        {
            pClearAll();
            if (n == 1)
                changeToCircle();
            if (n == 2)
                changeToSquare();
            if (n == 3)
                changeToTriangle();
            if (n == 4)
                changeToDiamond();
            if (n == 5)
                changeToMatrix();
        }

        private void pClearCircle()
        {
            pCircle = new Circle(content);
        }

        private void pClearDiamond()
        {
            pDiamond = new Diamond(content);
        }

        private void pClearSquare()
        {
            pSquare = new Square(content);
        }

        private void pClearTriangle()
        {
            pTriangle = new Triangle(content);
        }

        private void pClearMatrix()
        {
            pMatrix.makeMatrix();
        }

        public void pSquareResetDirections()
        {
            for (int i = 0; i < 4; i++)
                directions[i] = false;
        }


        private void pClearAll()
        {
            moveSpeed = 150f;
            pClearCircle();
            pClearDiamond();
            pClearMatrix();
            pClearSquare();
            pClearTriangle();
        }

        private void changeToCircle()
        {
            premoveShield();
            playerShape = pCircle;
        }

        private void changeToSquare()
        {
            pSquare.stopDashing();
            playerShape = pSquare;
        }

        private void changeToDiamond()
        {
            pDiamond.clearMines();
            playerShape = pDiamond;
        }

        private void changeToTriangle()
        {
            playerShape = pTriangle;
        }

        private void changeToMatrix()
        {
            playerShape = pMatrix;
        }

        private void updateShield(GameTime gameTime)
        {
            if (shielded())
            {
                shieldDuration += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (shieldDuration > SHIELD_TIME)
                    premoveShield();
            }
        }

        public override void Update(GameTime gameTime, InputManager input, Collision col, Layers layer)
        {
            base.Update(gameTime, input, col, layer);

            this.col = col;
            this.layer = layer;
            this.input = input;

            previousPosition = position;

            updateShield(gameTime);

            // moveAnimation is used to check collisions, it is not drawn and is the same for each shape 
            // (just a rectangle corresponding to the image)
            moveAnimation.Position = position;

            // Update all of the enabled animations
            List<SpriteSheetAnimation> Animations = playerShape.getActiveTextures();
            foreach (SpriteSheetAnimation animation in Animations)
            {
                if (playerShape == pMatrix)
                {
                    animation.Update(gameTime);
                }
                else if (animation.IsEnabled)
                    animation.Update(gameTime);

                if (!(playerShape == pDiamond && pDiamond.mineDropped() && pDiamond.isMineAnimation(animation)))
                    animation.Position = position;
            }

            if(playerShape == pMatrix)
                pMatrix.Update(gameTime);

            // Update the next shape animation in the upper right of the screen
            Animations = nextShape.getActiveTextures();
            foreach (SpriteSheetAnimation animation in Animations)
            {
                if (animation.IsEnabled)
                    animation.Update(gameTime);
            }

            if (playerShape == pMatrix)
                pMatrix.makeMatrix();

            if (playerShape == pSquare)
            {
                pSquare.setDirectionMap(directions);
                pSquare.Update(gameTime);
            }

            if (playerShape == pDiamond)
                pDiamond.Update(gameTime);

            if (playerShape == pTriangle)
                pTriangle.Update(gameTime);

            if (playerShape == pCircle)
                pCircle.Update(gameTime);

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
           
            // Draws each of the enabled animations for the current shape
            playerShape.Draw(spriteBatch);

            // Draws each of the enabled animations for the current shape in the upper right hand corner. 
            nextShape.DrawOnlyIdle(spriteBatch);
        }
    }
}
