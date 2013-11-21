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

        private bool loadNextLevel = false;

        private Vector2 spawnPosition;

        public bool LoadNextLevel
        {
            get { return loadNextLevel; }
        }


        public override void LoadContent(ContentManager content, InputManager input)
        {
            this.content = content;
            base.LoadContent(content, input);
            rand = new Random();

            this.content  = content;
            moveSpeed     = 150f; //Set the move speed

            moveAnimation = new SpriteSheetAnimation();
            gameTime      = new GameTime();

            //declare the shapes
            pSquare   = new Square(content);
            pCircle   = new Circle(content);
            pTriangle = new Triangle(content);
            pDiamond  = new Diamond(content);
            pMatrix   = new Matrix(content, 2, 2);

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
            spawnPosition = new Vector2(120, 200); //The location where the player spawns

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

        public void takeDamage()
        {
            health--;
            if (health == EMPTY)
                die();
        }

        public void die()
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
            nextShape   = null;
            
            queueOne();

            //Resets the locaiton of the next shape to the upper right corner
            List<SpriteSheetAnimation> Animations = nextShape.getActiveTextures();
            foreach (SpriteSheetAnimation animation in Animations)
            {
                if (animation.IsEnabled)
                    animation.position = new Vector2(0, 0); 
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

        public Shape getShape() { return playerShape; }

        public Rectangle getRectangle() { return new Rectangle((int) position.X, (int) position.Y, playerShape.getWidth(), playerShape.getHeight()); }

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
                        pDiamond.shoot(45);
                        break;
                    case 4:
                        pDiamond.shoot(135);
                        break;
                    case 2:
                        pDiamond.shoot(225);
                        break;
                    case 3:
                        pDiamond.shoot(315);
                        break;
                    default:
                        pDiamond.shoot(45);
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

        public override void Update(GameTime gameTime, InputManager input, Collision col, Layers layer)
        {
            previousPosition = position;
            //commented out; moved to GamePlayScreen; do we need?
            /*//MOVEMENT
            if (input.KeyDown(Keys.Right, Keys.D))
            { //MOVE RIGHT
                moveRight();
                //position.X += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                directions[0] = true;
            }

            if (input.KeyDown(Keys.Left, Keys.A))
            { //MOVE LEFT
                position.X -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                directions[1] = true;
            }
            if (input.KeyDown(Keys.Down, Keys.S))
            { //MOVE DOWN
                position.Y += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                directions[2] = true;
            }
            if (input.KeyDown(Keys.Up, Keys.W))
            { //MOVE UP
                position.Y -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                directions[3] = true;
            }

            

            //also in gameplayscreen            
            //Used to check deploy sheild in circle
            /*if (input.KeyDown(Keys.R))
            {
                if (playerShape == pCircle && !pCircle.shielded)
                    pCircle.deployShield();

                if (playerShape == pSquare)
                    pSquare.dash(this);

                if (playerShape == pTriangle)
                    pTriangle.PreformRotate();

                if (playerShape == pDiamond && !pDiamond.mineDeployed())
                    pDiamond.deployMine();

                if (playerShape == pMatrix)
                    pMatrix.PreformRotate();
            
            }

            if (input.KeyDown(Keys.E))
            {
                if (playerShape == pCircle && pCircle.shielded)
                    pCircle.removeShield();

                if (playerShape == pDiamond)
                    pDiamond.dropMine();

                if (playerShape == pMatrix)
                    pMatrix.PreformRotate(3);
            } */


            loadNextLevel = false;   //resets the signal to switch levels to false
            for (int i = 0; i < col.CollisionMap.Count; i++)
            {
                for (int j = 0; j < col.CollisionMap[i].Count; j++)
                {                   

                    if (col.CollisionMap[i][j] == "x") //Collision against solid objects (ex: Tiles)
                    {
                                
                        //Creates a rectangle that is the current tiles postion and size
                        lastCheckedRectangle = new Rectangle((int)(j * layer.TileDimensions.X), (int)(i * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));
                        
                        

                        
                        //Calls Collides method in shape class, in which each shape will check collisions uniquely 
                        if (playerShape.collides(position, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                        {
                            position = moveAnimation.Position;
                            playerShape.hit();
                        }
                    }

                    if (col.CollisionMap[i][j] == "*") //Marks a level transition (ex: Tiles)
                    {

                        //Creates a rectangle that is the current tiles postion and size
                        lastCheckedRectangle = new Rectangle((int)(j * layer.TileDimensions.X), (int)(i * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));




                        //Calls Collides method in shape class, in which each shape will check collisions uniquely 
                        if (playerShape.collides(position, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                        {
                            loadNextLevel = true; //Boolean sent to GamePlayScreen. Update method will detect this, and then call map.loadContent
                            position = spawnPosition;
                        }
                    }

                }
            }

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
