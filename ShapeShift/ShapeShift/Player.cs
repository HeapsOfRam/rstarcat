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
    class Player : Entity
    {
        GameTime gameTime;

        private FillTimer rActionFillTimer;
        private FillTimer eActionFillTimer;


        private List<Vector2> previousPositions = new List<Vector2>();
        private const int FULL = 3, MID = 2, LOW = 1, EMPTY = 0, SIZE = 60, MOVE = 5;
        private const float BASEMOVESPEED = 150f;
        private Shape nextShape;
        private Square pSquare;
        private Circle pCircle;
        private Triangle pTriangle;
        private Diamond pDiamond;
        private ContentManager content;

        private Turret turret;
        private Mine mine;
        private Ball ball;
        private float abilityTimer;
        private float cooldownTimer;
        private float cooldownTimer2;
        private bool startAbilityTimer;

        private double coolDownPrecentageR;
        private double coolDownPrecentageE;
        private int currentCooldownE;
        private int currentCooldownR;


        //Square: Ability Durations and Cooldowns
        private int squareDashDuration;
        private int squareDashCooldown;
        private bool squareDashCooldownStarted;

        //Circle: Ability Durations and Cooldowns
        private const float SHIELD_TIME = 5f;
        private float shieldDuration = 0;
        private int circleShieldCooldown;
        private int circleBallCooldown;
        private bool circleShieldCooldownStarted;
        private bool shieldReady;
        private bool ballReady;
        private bool shieldExpired;
        private bool turretReadyToDeploy;
        private int turretDeploymentCooldown;
        private bool turretDeploymentCooldownStarted;
        private bool turretExpired;

        private bool mineReadyToDeploy;
        private int mineDeploymentCooldown;
        private bool mineDeploymentCooldownStarted;
        private bool mineExpired;
       
        
        private Random rand;

        private int r;

        private Matrix pMatrix;

        private const int START_X = 200, START_Y = 200;

        private  Collision col;
        private  Layers layer;
        private InputManager input;
        private SpriteFont font;
        private Boolean circleBallCooldownStarted = false;
        private Boolean gameEnd;
        private bool dead;

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

            eActionFillTimer = new FillTimer(content, "E");
            rActionFillTimer = new FillTimer(content, "R");

            this.content  = content;
            moveSpeed     = BASEMOVESPEED; //Set the move speed equal to the baseMoveSpeed(baseMoveSpeed is constant)

            turret = new Turret(content, input, this);
            mine = new Mine(content, input, this);
            ball = new Ball(content, input, this);

            moveAnimation = new SpriteSheetAnimation();
            moveAnimation.position = new Vector2(START_X, START_Y);

            gameTime      = new GameTime();

            //declare the shapes
            pSquare   = new Square(content);
            pCircle   = new Circle(content);
            pTriangle = new Triangle(content);
            pDiamond  = new Diamond(content);

            font = content.Load<SpriteFont>("Fonts/AbilityStatusFont");
            maxHealth = FULL;
            health    = maxHealth;
            dead = false;

            entityShape = pSquare;   //********STARTING SHAPE*******         

            //Queue up the next shape
            queueOne();
            position = new Vector2(START_X, START_Y);

            //each shape should have silhouette shape of its own
            //moveAnimation.LoadContent(content, playerShape.getTexture(), "", position);
            
            entityShape.setPosition(position);
            abilityTimer = 0;
            cooldownTimer = 0;
            cooldownTimer2 = 0;
            startAbilityTimer = false;

            #region Cooldown Management
            //**Cooldown Management**
            //SQUARE
            squareDashDuration = 4;
            squareDashCooldown = 2;
            squareDashCooldownStarted = false;

            //CIRCLE
            circleShieldCooldown = 6;
            circleShieldCooldownStarted = false;
            shieldReady = true;
            shieldExpired = false;

            circleBallCooldown = 6;
            circleBallCooldownStarted = false;
            ballReady = true;


            //DIAMOND
            //diamond turret
            turretDeploymentCooldown = 5;
            turretDeploymentCooldownStarted = false;
            turretReadyToDeploy = true;
            turretExpired = false;

           //diamond mine
            mineDeploymentCooldown = 5;
            mineDeploymentCooldownStarted = false;
            mineReadyToDeploy = true;
            mineExpired = false;
            
            #endregion



            // spawnPosition = new Vector2(55, 320); //player spawns handled in entity
           // leftSpawnPosition = new Vector2(55, 320);
           // rightSpawnPosition = new Vector2(680, 320);
            currentCooldownE = 0;
            currentCooldownR = 0;


        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            moveAnimation.UnloadContent();
        }

       

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
            gameEnd = true;
        }

        public void shiftShape()
        {
            //NOTE: make sure to set the ready booleans to true for cooldowns
            //that way the ability is available when you shift to the new shape
            moveSpeed = BASEMOVESPEED;
            pSquare.stopDashing(this);
            clearBullets();
            currentCooldownE = 0;
            currentCooldownR = 0;
            
            

            if (entityShape == pCircle && pCircle.shielded)
            {
                pCircle.removeShield();
                shieldReady = true;
                ballReady = true;
                ball.die();
            }
            else if (entityShape == pSquare && pSquare.dashing)
                pSquare.stopDashing(this);
            else if (entityShape == pSquare)
                pSquare.dashReady = true;
            else if (entityShape == pDiamond)
            {
                //pDiamond.clearMines();
                if (turret.isDeployed())
                    forceTurretExpire();

                if (mine.isDeployed())
                    forceMineExpire();

                turretReadyToDeploy = true;
                mineReadyToDeploy = true;
            }

            if (entityShape == pCircle && !ball.isFired())
            {
                ball.die();
            }

            entityShape = nextShape;

            if (entityShape == pTriangle)
            
                entityShape.setOrigin(new Vector2(45.6667f, 53.6667f));

            
            else
                entityShape.setOrigin(new Vector2(46, 46));


            if (entityShape == pDiamond)
            {
                turretReadyToDeploy = true;
                mineReadyToDeploy = true;
            }

        
            

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
            pushOut ();


            resetFillTimers();

        }

        private void pushOut()
        {
            int count;
            if (previousPositions.Count == 0)
            {
                count = 0;
            }
            else
                count = previousPositions.Count - 1;

            while (detectCollision())
            {
                this.position = previousPositions[count];
                this.moveAnimation.position = previousPositions[count];
                
                count--;
            }
            
        }

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
            {
                if (!shielded() && shieldReady)
                {
                    
                    pdeployShield();

                    //detectCollision();
                    pushOut();
                    

                }
                //else
                  //  premoveShield();

                currentCooldownR = circleShieldCooldown;

            }
            if (entityShape == pSquare)
            {
                pDash();
               // entityShape.Update(gameTime);

                currentCooldownR = squareDashCooldown;
                
            }
            if (entityShape == pTriangle)
            {
                pRotate();
                //detectCollision();
                pushOut();
                currentCooldownR = 0;
            }
            if (entityShape == pDiamond)
            {
                currentCooldownR = mineDeploymentCooldown;

                if (mine.isDeployed())
                {
                    mine.dropSelf();
                    pdropMine();
                }
                else
                {
                    if (mineReadyToDeploy && !turret.isDeployed() && !turret.isDropped())
                    {
                        mine = new Mine(content, input, this);
                        mine.deploySelf();
                        mineReadyToDeploy = false;
                        
                        //pdeployMine();
                    }

                    
                }

                
            }
            
        }

        public void eAction()
        {
            if (entityShape == pCircle)
            {
                currentCooldownE = circleBallCooldown;
                if (ball.isDeployed())
                {
                    ball.fireSelf();
                }
                else if (ballReady)
                {
                    ball = new Ball(content, input, this);
                    updateCount = 0;
                    ball.deploySelf();
                }

            }
            if (entityShape == pDiamond)
            {
                currentCooldownE = 0;
                if (turret.isDeployed())
                {
                    turret.dropSelf();
                    pdropTurret();
                    turretReadyToDeploy = false;
                }
                else
                {
                    if (turretReadyToDeploy && !mine.isDeployed() && !mine.isDropped())
                    {
                        turret = new Turret(content, input, this);
                        turret.deploySelf();
                        turretReadyToDeploy = false;
                    }
                    //pdeployTurret();
                }
                //pDiamond.deployTurret();
                //pDiamond.hit();
            }

            if (entityShape == pTriangle)
            {
                pTriangle.hit();
                currentCooldownE = 0;
            }

            if (entityShape == pSquare)
            {
                currentCooldownE = 0;
            }

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

        
        public override bool isDead()
        {
            return dead;
        }

        public Boolean isTurretDropped()
        { return turret.isDropped(); }

        public void fireTurret(GameTime gameTime, MatrixTileEnemy enemy)
        {
            turret.shoot(gameTime, enemy);
        }

        public void turretSpot(Entity enemy)
        {
            if (enemy.GetType().ToString().Equals("ShapeShift.MatrixEnemy"))
            {
                MatrixEnemy e = (MatrixEnemy)enemy;
                foreach (MatrixTileEnemy tile in e.getTileEnemies())
                {
                    if (!tile.getShape().isDead())
                    {
                        if (isTurretDropped() && turret.spot(tile))
                        {
                            fireTurret(gameTime, tile);
                            //e.Update(gameTime, map.collision, map.layer, player, player.getTurretBullets());
                        }
                    }
                }
           
            }
                
       
        
        }

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

            if (entityShape == pTriangle && !rotating())
            {
                int facing = pTriangle.rotationIteration(); 
                switch (direction)
                {
                    case 1:
                        if (facing == 1)
                            pTriangle.shoot(25);
                        if (facing == 2)
                            pTriangle.shoot(0);
                        if (facing == 3)
                            pTriangle.shoot(335);
                        break;
                    case 4:
                        if (facing == 0)
                            pTriangle.shoot(65);
                        if (facing == 2)
                            pTriangle.shoot(115);
                        if (facing == 3)
                            pTriangle.shoot(90);
                        break;
                    case 2:
                        //if(facing != 2)
                        if (facing == 0)
                            pTriangle.shoot(180);
                        if (facing == 1)
                            pTriangle.shoot(155);
                        if (facing == 3)
                            pTriangle.shoot(205);
                        break;
                    case 3:
                        //if(facing != 3)
                        if (facing == 0)
                            pTriangle.shoot(300);
                        if (facing == 1)
                            pTriangle.shoot(270);
                        if (facing == 2)
                            pTriangle.shoot(245);
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

        private void pStopDash()
        {
            pSquare.stopDashing(this);
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
            if (turretReadyToDeploy)
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
            resetFillTimers();
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

        private void resetFillTimers()
        {

            rActionFillTimer.reset();
            eActionFillTimer.reset();
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
            moveSpeed = BASEMOVESPEED;
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
            pSquare.stopDashing(this);
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

        public void forceMineExpire()
        {
            mine.die();
        }

        public override Mine getMine()
        {
            return mine;
        }

        public override Boolean hasMineDropped()
        {
            return mine.isDropped();
        }

        public Ball getBall()
        {
            return ball;
        }

        public Boolean hasBallThrown()
        {
            return ball.isFired();
        }

        private void updateShield(GameTime gameTime)
        {
            if (shielded())
            {
                //Console.WriteLine("shieldDuration: " + shieldDuration);
                shieldDuration += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (shieldDuration > SHIELD_TIME)
                {
                    premoveShield();
                    shieldExpired = true;
                    circleShieldCooldownStarted = true;

                }
            }
        }

        public override void Update(GameTime gameTime, InputManager input, Collision col, Layers layer)
        {
            base.Update(gameTime, input, col, layer);

            this.col = col;
            this.layer = layer;
            this.input = input;

            if (!colliding)
            {
                Vector2 newPos = new Vector2(position.X, position.Y);

                   if (!previousPositions.Contains(newPos))
                        previousPositions.Add(newPos);
                



            }

            if (!turret.isExpired())
                turret.Update(gameTime, input, col, layer);
            if (!mine.isGone())
                mine.Update(gameTime, input, col, layer);
            if (!ball.isExpired())
                ball.Update(gameTime, input, col, layer);

            if (mine.isCollidable() && entityShape == pDiamond)
                mineDeploymentCooldownStarted = true;
            
            //previousPosition = position;

            updateShield(gameTime);

            coolDownPrecentageR = (cooldownTimer / currentCooldownR) * 100;
            coolDownPrecentageE = (cooldownTimer2 / currentCooldownE) * 100;

            if (currentCooldownR != 0)
                rActionFillTimer.fillPercentage(coolDownPrecentageR);
            else
                rActionFillTimer.reset();

            if (currentCooldownE != 0)
                eActionFillTimer.fillPercentage(coolDownPrecentageE);
            else
                eActionFillTimer.reset();

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

        
            if (entityShape == pSquare)  //<<<<<SQUARE>>>>>
            {
                pSquare.setDirectionMap(directions);
                pSquare.Update(gameTime);


                 if (pSquare.dashing)
                    {
                        pSquare.dashReady = false;
                        abilityTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }

                 if (abilityTimer > squareDashDuration)
                 {
                     pSquare.dashExpired = true;
                     squareDashCooldownStarted = true;
                     abilityTimer = 0;
                 }

                 if (squareDashCooldownStarted)
                 {
                     cooldownTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                     //Console.WriteLine("The value of coolDownTimer is: " + coolDownTimer + "Seconds");
                 }

                 if (cooldownTimer > squareDashCooldown)
                 {
                     squareDashCooldownStarted = false;
                     cooldownTimer = 0;
                     pSquare.dashExpired = false;
                     pSquare.dashReady = true;

                     currentCooldownR = 0;
                 }

                if (pSquare.dashExpired)
                {
                    pSquare.stopDashing(this);
                }


            }

            if (entityShape == pDiamond) //<<<<<DIAMOND>>>>>
            {
                pDiamond.Update(gameTime);

                //if (pDiamond.turretDeployed())
                //{
                  //  turretReadyToDeploy = false;
                    //abilityTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                //}

                if (turret.isExpired())
                {
                    turretDeploymentCooldownStarted = true;
                    //abilityTimer = 0;
                }

                if (turretDeploymentCooldownStarted)
                {
                    cooldownTimer2 += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //Console.WriteLine("The value of coolDownTimer is: " + coolDownTimer + "Seconds");
                }

                if (mineDeploymentCooldownStarted)
                {
                    cooldownTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }


                if (cooldownTimer2 > turretDeploymentCooldown)
                {
                    turretDeploymentCooldownStarted = false;
                    cooldownTimer2 = 0;
                    turret.setExpired(false);

                    //turretReadyToDeploy = true;
                }

                if (cooldownTimer > mineDeploymentCooldown)
                {
                    mineDeploymentCooldownStarted = false;
                    cooldownTimer = 0;
                    mineReadyToDeploy = true;
                    currentCooldownR = 0;
                }


            }
            if (entityShape == pTriangle)//<<<<<TRIANGLE>>>>>
                pTriangle.Update(gameTime);

            if (entityShape == pCircle) //<<<<<CIRCLE>>>>>
            {
                pCircle.Update(gameTime);

                if (pCircle.isShielded())
                {
                    shieldReady = false;
                }

                if (ball.isFired() && !ball.isExpired())
                {
                    ballReady = false;
                }
                
                if (ball.isExpired() && !ballReady)
                    circleBallCooldownStarted = true;
               

                if (circleShieldCooldownStarted)
                {
                    cooldownTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    
                }

                if (circleBallCooldownStarted)
                {
                    cooldownTimer2 += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    
                }

                if (cooldownTimer > circleShieldCooldown)
                {
                    circleShieldCooldownStarted = false;
                    cooldownTimer = 0;
                    shieldExpired = false;
                    currentCooldownR = 0;
                    shieldReady = true;
                }

                if (cooldownTimer2 > circleBallCooldown)
                {
                    circleBallCooldownStarted = false;
                    cooldownTimer2 = 0;
                    currentCooldownE = 0;
                    ballReady = true;
                }

                

            }

            //Console.WriteLine("Player" + updateCount);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            rActionFillTimer.Draw(spriteBatch);
            eActionFillTimer.Draw(spriteBatch);
            /*
            //Square Dash - Status
            if (entityShape == pSquare && pSquare.dashReady)
            {
                spriteBatch.DrawString(font, "dash is Ready!!!: ", new Vector2(475, 5), Color.White);
            }
            else if (entityShape == pSquare && pSquare.dashing) 
            {
                spriteBatch.DrawString(font, "dashing!!: ", new Vector2(475, 5), Color.White);
            }
            else if (entityShape == pSquare && !pSquare.dashReady)
            {
                spriteBatch.DrawString(font, "dash cooldown: " + (squareDashCooldown - cooldownTimer), new Vector2(475, 5), Color.White);
            }

            //Circle Shield - Status
            if (entityShape == pCircle && shieldReady)
            {
                spriteBatch.DrawString(font, "Shield is Ready!!!: ", new Vector2(475, 5), Color.White);
            }
            else if (entityShape == pCircle && pCircle.isShielded())
            {
                spriteBatch.DrawString(font, "Shielded!", new Vector2(475, 5), Color.White);
            }
            else if (entityShape == pCircle && !shieldReady)
            {
                spriteBatch.DrawString(font, "shield cooldown: " + (circleShieldCooldown - cooldownTimer), new Vector2(475, 5), Color.White);
            }

            //Diamond Turret - Status
            if (entityShape == pDiamond && turretReadyToDeploy)
            {
                spriteBatch.DrawString(font, "Turret Ready!", new Vector2(440, 5), Color.White);
            }
            else if (entityShape == pDiamond && !turretReadyToDeploy && !turret.isExpired())
            {
                spriteBatch.DrawString(font, "TurretDeployed!", new Vector2(475, 5), Color.White);
            }
            else if (entityShape == pDiamond && !turretReadyToDeploy)
            {
                spriteBatch.DrawString(font, "turret cooldown: " + (turretDeploymentCooldown - cooldownTimer), new Vector2(475, 5), Color.White);
            }

            //Diamond Mine - Status
            if (entityShape == pDiamond && mineReadyToDeploy)
            {
                spriteBatch.DrawString(font, "Mine Ready! ", new Vector2(720, 5), Color.White);
            }
            else if (entityShape == pDiamond && !mineReadyToDeploy && !mine.isGone())
            {
                spriteBatch.DrawString(font, "MineDeployed!", new Vector2(475, 5), Color.White);
            }
            else if (entityShape == pDiamond && !mineReadyToDeploy)
            {
                spriteBatch.DrawString(font, "Mine cooldown: " + (mineDeploymentCooldown - cooldownTimer2), new Vector2(475, 5), Color.White);
                //uses cooldownTimer2, as coolDownTimer  is used for the turret
            }


            */
            // Draws each of the enabled animations for the current shape
            entityShape.Draw(spriteBatch);

            // Draws each of the enabled animations for the current shape in the upper right hand corner. 
            nextShape.DrawOnlyIdle(spriteBatch);

            if(!turret.isExpired() && (turret.isDeployed() || turret.isDropped()))
                turret.Draw(spriteBatch);

            if (((mine.isDeployed() || mine.isDropped()) || mine.isDetonated()))
                mine.Draw(spriteBatch);

            if (!ball.isExpired() && (ball.isDeployed() || ball.isFired()))
                ball.Draw(spriteBatch);
        }

        public List<Shape> getActiveBullets()
        {
            List<Shape> list = new List<Shape>();
            list.AddRange(entityShape.getActiveBullets());
            if (turret.isDropped() && !turret.isExpired())
            {
                list.AddRange(getTurretBullets());
            }
            if (ball.isFired() && !ball.isExpired())
                list.Add(ball.getShape());
            if (mine.isCollidable())
                list.Add(mine.getShape());
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
            return turret.getShape().getActiveBullets();
        }

        public void resetMoveSpeed()
        {
            moveSpeed = BASEMOVESPEED;
        }

        public Texture2D[] getHearts()
        {
            return entityShape.getHearts();
        }

        public bool isMineDropped()
        { return mine.isDropped(); }

        public void forceBallExpire()
        {
            ball.die();
        }
    }
}
