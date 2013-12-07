﻿using System;
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
        private Shape nextShape;
        private Square pSquare;
        private Circle pCircle;
        private Triangle pTriangle;
        private Diamond pDiamond;
        private ContentManager content;

        private Turret turret;
        private Mine mine;
        
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

            turret = new Turret(content, input, this);
            mine = new Mine(content, input, this);

            moveAnimation = new SpriteSheetAnimation();
            moveAnimation.position = new Vector2(START_X, START_Y);

            gameTime      = new GameTime();

            //declare the shapes
            pSquare   = new Square(content);
            pCircle   = new Circle(content);
            pTriangle = new Triangle(content);
            pDiamond  = new Diamond(content);
            

            maxHealth = FULL;
            health    = maxHealth;

            entityShape = pSquare;   //********STARTING SHAPE*******         

            //Queue up the next shape
            queueOne();
            position = new Vector2(START_X, START_Y);

            //each shape should have silhouette shape of its own
            //moveAnimation.LoadContent(content, playerShape.getTexture(), "", position);
            
            entityShape.setPosition(position);
            
         
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
            } while (nextShape == entityShape);        
  

        }

        public Boolean rotating()
        {
            return entityShape == pTriangle && pTriangle.rotating();
        }

        public Boolean shielded()
        {
            return entityShape == pCircle && pCircle.isShielded();
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
            clearBullets();
            if (entityShape == pCircle && pCircle.shielded)
                pCircle.removeShield();     
            else if (entityShape == pSquare && pSquare.dashing)
                pSquare.stopDashing();
            else if (entityShape == pDiamond)
                pDiamond.clearMines();

            entityShape = nextShape;

            if (entityShape == pTriangle)
                entityShape.setOrigin(new Vector2(45.6667f, 53.6667f));
            else
                entityShape.setOrigin(new Vector2(46, 46));

            pushOut (entityShape);
            

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

        public override Shape getShape() { return entityShape; }

        public override Rectangle getRectangle() { return new Rectangle((int) position.X, (int) position.Y, entityShape.getWidth(), entityShape.getHeight()); }

        public void rAction()
        {
            if (entityShape == pCircle)
                pdeployShield();
            if (entityShape == pSquare)
                pDash();
            if (entityShape == pTriangle)
                pRotate();
            if (entityShape == pDiamond)
            {
                if (mine.isDeployed())
                {
                    mine.dropSelf();
                    pdropMine();
                }
                else
                {
                    mine = new Mine(content, input, this);
                    mine.deploySelf();
                    pdeployMine();
                }
            }
            
        }

        public void eAction()
        {
            if (entityShape == pCircle)
            {
                premoveShield();
                pCircle.hit();
            }
            if (entityShape == pDiamond)
            {
                if (turret.isDeployed())
                {
                    turret.dropSelf();
                    pdropTurret();
                }
                else
                {
                    turret = new Turret(content, input, this);
                    turret.deploySelf();
                    //pdeployTurret();
                }
                //pDiamond.deployTurret();
                //pDiamond.hit();
            }
           
            if (entityShape == pTriangle)
                pTriangle.hit();

        }

        public void clearBullets()
        {
           
                pSquare.clearBullets();
        
                pDiamond.clearBullets();
         
                pTriangle.clearBullets();
    
                pCircle.clearBullets();
        }

        public override Boolean isEnemy()
        { return false; }

        public Boolean isTurretDropped()
        { return turret.isDropped(); }

        public void fireTurret(GameTime gameTime, Entity enemy)
        {
            turret.shoot(gameTime, enemy);
        }

        public Boolean turretSpot(Entity enemy)
        { return turret.spot(enemy);  }

        public void shoot(GameTime gametime, int direction)
        {

            if (entityShape == pSquare)
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


            if (entityShape == pDiamond)
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

            if (entityShape == pTriangle)
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

            if (entityShape == pCircle)
            {
                switch (direction)
                {
                    case 1:
                        pCircle.shoot(45);
                        break;
                    case 4:
                        pCircle.shoot(135);
                        break;
                    case 2:
                        pCircle.shoot(225);
                        break;
                    case 3:
                        pCircle.shoot(315);
                        break;
                    default:
                        pCircle.shoot(45);
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
            if(entityShape == pTriangle)
                pTriangle.PreformRotate();
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
        private void pdropTurret()
        {
            pDiamond.dropTurret();
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
            pClearSquare();
            pClearTriangle();
        }

        private void changeToCircle()
        {
            premoveShield();
            entityShape = pCircle;
        }

        private void changeToSquare()
        {
            pSquare.stopDashing();
            entityShape = pSquare;
        }

        private void changeToDiamond()
        {
            pDiamond.clearMines();
            entityShape = pDiamond;
        }

        private void changeToTriangle()
        {
            entityShape = pTriangle;
        }

        public override Boolean hasTurretDropped()
        {
            return turret.isDropped();
        }

        public void forceTurretExpire()
        {
            turret.die();
        }

        public Entity getMine()
        {
            return mine;
        }

        public override Boolean hasMineDropped()
        {
            return mine.isDropped();
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

            if (!turret.isExpired())
                turret.Update(gameTime, input, col, layer);
            if (!mine.isDetonated())
                mine.Update(gameTime, input, col, layer);

            previousPosition = position;

            updateShield(gameTime);

            // moveAnimation is used to check collisions, it is not drawn and is the same for each shape 
            // (just a rectangle corresponding to the image)
            moveAnimation.Position = position;

            // Update all of the enabled animations
            List<SpriteSheetAnimation> Animations = entityShape.getActiveTextures();
            foreach (SpriteSheetAnimation animation in Animations)
            {
               
                if (animation.IsEnabled)
                    animation.Update(gameTime);

                if (!(entityShape == pDiamond && pDiamond.mineDropped() && pDiamond.isMineAnimation(animation)))
                    animation.Position = position;
            }

          

            // Update the next shape animation in the upper right of the screen
            Animations = nextShape.getActiveTextures();
            foreach (SpriteSheetAnimation animation in Animations)
            {
                if (animation.IsEnabled)
                    animation.Update(gameTime);
            }

        
            if (entityShape == pSquare)
            {
                pSquare.setDirectionMap(directions);
                pSquare.Update(gameTime);
            }

            if (entityShape == pDiamond)
                pDiamond.Update(gameTime);

            if (entityShape == pTriangle)
                pTriangle.Update(gameTime);

            if (entityShape == pCircle)
                pCircle.Update(gameTime);

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
           
            // Draws each of the enabled animations for the current shape
            entityShape.Draw(spriteBatch);

            // Draws each of the enabled animations for the current shape in the upper right hand corner. 
            nextShape.DrawOnlyIdle(spriteBatch);

            if(turret != null && (!turret.isExpired()) && (turret.isDeployed() || turret.isDropped()))
                turret.Draw(spriteBatch);

            if (!mine.isDetonated() && (mine.isDeployed() || mine.isDropped()))
                mine.Draw(spriteBatch);
        }

        public List<Shape> getActiveBullets()
        {
            List<Shape> list = entityShape.getActiveBullets();
            /*if(!turret.isExpired())
                list.AddRange(getTurretBullets());*/
            return list;
        }

        public override Entity getTurret()
        {
            return turret;
        }

        public Rectangle getTurretRectangle()
        {
            return turret.getRectangle();
        }

        public Color[] getTurretColor()
        {
            return turret.getShape().getColorData();
        }

        public void turretTakeDamage()
        {
            turret.takeDamage();
        }

        public List<Shape> getTurretBullets()
        {
            return turret.getActiveBullets();
        }

        public Texture2D[] getHearts()
        {
            return entityShape.getHearts();
        }
    }
}
