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
    public class GameplayScreen : GameScreen
    {
        private float damageTime = 3;
        private const float INVULN_TIME = 1f;

        Player player;
        MatrixEnemy dummyEnemy;
        Layers layer;
        Map map;
        Rectangle rectangle;
        Rectangle[] healthRectangle;
        Texture2D healthFillTexture, healthUnfillTexture;
        Boolean paused = false;
        Boolean pauseTimer = false;

        int count = 0;

        String Level = "";
        String Level1 = "1";
        String Level2 = "2";
        String Level3 = "3";
        String previousLevel = "";
        Random randomLevelGenerator;
        int levelNumber;
        int numLevelsCompleted;
        FileManager fileManager;
        List<Texture2D> images;

        List<MatrixEnemy> enemyList = new List<MatrixEnemy>();

        bool LevelCompleted;
        bool GameOver;
        int imageNumber;

        private SpriteFont font;
        int screenWidth, screenHeight, counter, maxCount = 15; //maxCount sets the duration between shapeShifts
        int timeRemaining;
        const int HUDHEIGHT = 50, ABSZERO = 0, HEALTHSIZEX = 25, HEALTHSIZEY = 25, HEALTHOFFSETX = 150, HEALTHOFFSETY = 60, DISPLACEHEALTH = 5;

        float countDuration = 1f, currentTime = 0f;

        int score;  //The score of the game
        String playerName = "";
        int currentHighScore; //Used for comparison
        private bool enemiesLoaded = true;

        #region ScoreManagement
        public int Score //Used to access the score of the game elsewhere
        {
            get { return score; }
        }

        public int IncreaseScore(int pointsAdded)
        {
            score += pointsAdded;
            return score;
        }
        
        private int DecreaseScore(int pointsSubtracted)
        {
            score -= pointsSubtracted;
            return score;
        }

        public int NumLevelsCompleted //Used to access the number of levels completed by the player elsewhere
        {
            get { return numLevelsCompleted; }
        }

        public String PlayerName
        {
            get { return playerName; }
        }

        public void setPlayerName(String newName)
        {
            playerName = newName;
        }

        #endregion

        public override void LoadContent(ContentManager content, InputManager input)
        {
            base.LoadContent(content, input);
            player = new Player();

            
            dummyEnemy = new MatrixEnemy(new Vector2 (500,500),this);
            //enemyList.Add(dummyEnemy);

            layer = new Layers();
            map = new Map();
            rectangle = new Rectangle();
            font = this.content.Load<SpriteFont>("Fonts/Font1");
            Level = Level1; //FIRST LEVEL
            map.LoadContent(content, "InProgressMap1");
            player.LoadContent(content, input);
            dummyEnemy.LoadContent(content, 2, 2);
            healthRectangle = new Rectangle[player.getMaxHealth()];
            
            levelNumber = 1;
            Level = "1";
            previousLevel = Level;
            randomLevelGenerator = new Random();
            LevelCompleted = false;
            GameOver = false;
            score = 0;
            numLevelsCompleted = 0;
            playerName = "Player1";

            healthFillTexture = content.Load<Texture2D>("Circle/heart"); ;
            healthUnfillTexture = content.Load<Texture2D>("Circle/heartEmpty");


            dummyEnemy = new MatrixEnemy(new Vector2(500, 500),this);
            dummyEnemy.LoadContent(content, 2, 2);
            enemyList.Add(dummyEnemy);


           
        }


        public override void UnloadContent()
        {
            base.UnloadContent();
            player.UnloadContent();
            map.UnloadContent();
            fileManager = null;
        
        }

        public override void Update(GameTime gameTime)
        {
            if (!paused)
            {
                if (inputManager.KeyPressed(Keys.P))
                    paused = true;

                inputManager.Update();
                player.Update(gameTime, inputManager, map.collision, map.layer);


                Texture2D[] hearts = player.getHearts();
                healthFillTexture = hearts[0];
                healthUnfillTexture = hearts[1];
               
                
                map.Update(gameTime);
                LevelCompleted = false;

                player.pSquareResetDirections();

                damageTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                
                
                foreach (MatrixEnemy e in enemyList)
                {
                   

                    e.Update(gameTime, map.collision, map.layer, player, player.getActiveBullets());
                   

                        if (!e.isDead())
                        {
                            player.turretSpot(e);

                            if (player.hasMineDropped())
                            {
                                if (e.collides(e.getPosition(), player.getMine().getRectangle(), player.getMine().getShape().getColorData()))
                                    player.getMine().trigger();
                            }

                            if (e.collides(e.getPosition(), player.getRectangle(), player.getShape().getColorData()))
                            {
                                e.makeReel();

                                if (damageTime > INVULN_TIME && player.takeDamage())
                                {
                                    DecreaseScore(1);
                                    damageTime = 0;
                                }
                            }

                            //TODO TO MAKE TURRET GET HIT AND DIE
                            /*if (e.collides(e.getPosition(), player.getTurretRectangle(), player.getTurretColor()))
                            {
                                Console.WriteLine("Collision Works");
                                player.turretTakeDamage();
                            }*/
                        }

                       
                }

                if (inputManager.KeyDown(Keys.W))
                    player.moveUp(gameTime);
                if (inputManager.KeyDown(Keys.S))
                    player.moveDown(gameTime);
                if (inputManager.KeyDown(Keys.A))
                    player.moveLeft(gameTime);
                if (inputManager.KeyDown(Keys.D))
                    player.moveRight(gameTime);

                if (inputManager.KeyPressed(Keys.R))
                    player.rAction();
                if (inputManager.KeyPressed(Keys.E))
                    player.eAction();

                if (inputManager.KeyDown(Keys.F))
                {
                    foreach (MatrixEnemy e in enemyList)
                    {
                        e.group();
                    }
                    
                }


                if (inputManager.KeyDown(Keys.G))
                {
                    foreach (MatrixEnemy e in enemyList)
                    {
                        e.ungroup();
                    }

                }

                

                if (inputManager.KeyDown(Keys.Up))
                    player.shoot(gameTime, 1);
                if (inputManager.KeyDown(Keys.Down))
                    player.shoot(gameTime, 2);
                if (inputManager.KeyDown(Keys.Left))
                    player.shoot(gameTime, 3);
                if (inputManager.KeyDown(Keys.Right))
                    player.shoot(gameTime, 4);

                //for debug, pause
                
                if (inputManager.KeyPressed(Keys.T))
                    pauseTimer = !pauseTimer;
                if (!pauseTimer)
                    currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (inputManager.KeyPressed(Keys.L))
                    player.restoreHealth();
                
                //SCORE and GameOver Testing STUFF

                if (inputManager.KeyPressed(Keys.C))  //A demonstration of IncreaseScore(). Increases score by 100 when 'c' is pressed.
                    IncreaseScore(100);

                if (inputManager.KeyPressed(Keys.Y))
                    GameOver = true;

                if (pauseTimer)
                {
                    int n = 0;
                    if (inputManager.KeyPressed(Keys.D1))
                        n = 1;
                    if (inputManager.KeyPressed(Keys.D2))
                        n = 2;
                    if (inputManager.KeyPressed(Keys.D3))
                        n = 3;
                    if (inputManager.KeyPressed(Keys.D4))
                        n = 4;
                    if (inputManager.KeyPressed(Keys.D5))
                        n = 5;

                    if (n > 0)
                        player.manualChange(n);
                    n = 0;
                }

                
                if (currentTime >= countDuration)
                {
                    counter++;
                    currentTime -= countDuration;
                }
                if (counter >= maxCount)
                {
                    // Console.WriteLine("Calling ShiftShape");
                    
                    player.shiftShape();
                    counter = 0;
                }

                timeRemaining = maxCount - counter;
                for (int i = 0; i < player.getMaxHealth(); i++)
                {
                    healthRectangle[i] = new Rectangle(HEALTHOFFSETX + ((HEALTHSIZEX + DISPLACEHEALTH) * i), HEALTHOFFSETY, HEALTHSIZEX, HEALTHSIZEY);
                }
                /* This code was used to create a UI Boundary at the top of the screen
                 * so that the player would not be able to move past it
                if (player1.getX() <= ABSZERO)
                    player1.setX(ABSZERO);
                if (player1.getX() >= screenWidth - player1.getWidth())
                    player1.setX(screenWidth - player1.getWidth());
                */

                /*if (player.isTurretDropped())
                {
                    foreach (Enemy e in enemyList)
                    {
                        if (player.turretSpot(e))
                        {
                            player.fireTurret(gameTime, e);
                        }
                    }
                }*/

                //LOADING IN THE NEXT LEVEL

                
                if (enemiesLoaded)
                {
               
                Boolean aliveEnemyFound = false;
                
                    foreach (MatrixEnemy e in enemyList)
                    {
                        if (!e.isDead())
                        {
                            aliveEnemyFound = true;

                        }
                    }

                    if (!aliveEnemyFound)
                    {
                        pauseTimer = true;
                        enemiesLoaded = false;
                        LevelCompleted = true;
                        player.forceTurretExpire();
                        //player.forceTurretExpire();
                        //player.shiftShape();
                    }

                   
                        if (LevelCompleted)
                        {
                            map.LoadContent(content, "CompletedMap" + previousLevel);
                            count++;
                        }


                        
                    }
                if (Level.Equals(previousLevel)) //IF you are already on the next randomly generated level, load again
                {
                    // previousLevel = Level;                          //Assign the current level to 'previous level'
                    levelNumber = randomLevelGenerator.Next(4) + 1; //randomly generate the next level number
                    
                    Level = levelNumber.ToString();                 //Assign the new level number to 'level'

                }
                else if (player.ExitsLevel)
                {
                    map.LoadContent(content, "InProgressMap" + Level);
                    previousLevel = Level;
                    loadEnemies(levelNumber);
                    
                }

                //IF THE GAME SESSION ENDS
                if (GameOver)
                {
                    List<int> listOfScores = new List<int>();

                    string[] highScoresList = System.IO.File.ReadAllLines(@".\Scores\Scores.txt");
                    //Reads in the existing high scores for comparison

                    // Display the file contents by using a foreach loop.


                    foreach (string highScore in highScoresList)
                    {
                        // Use a tab to indent each line of the file.

                        String[] temp = highScore.Split(',');

                        currentHighScore = int.Parse(temp[0]);
                        listOfScores.Add(currentHighScore);

                    }

                    //   Console.WriteLine("The Lowest Score: " + listOfScores.Min());
                    // Console.WriteLine("The Highest Score: " + listOfScores.Max());

                    if (score > listOfScores.Min()) //if the Game Score is higher than the lowest recorded High Score
                    {


                        //System.IO.File.AppendAllLines(@".\Scores\Scores.txt", scoreLine);
                        GameServices.RemoveService<int>();
                        GameServices.AddService<int>(score);

                        //WRITES THE SCORE AND SPECIFIES WHAT LEVEL THE SCORE WAS ACHIEVED ON
                        // string[] lines = { "Name: " + playerName + " ", "Score: " + score.ToString() + " ", "Levels Completed: " + numLevelsCompleted.ToString() + " " };
                        // WriteAllLines creates a file, writes a collection of strings to the file, 
                        // and then closes the file.
                        // System.IO.File.AppendAllLines(@"C:\Users\wildcat\Documents\Visual Studio 2010\Projects\ShapeShift\rstarcat\ShapeShift\ShapeShift\bin\x86\Debug\Scores\ScoreInfo.txt", lines);
                    }
                    else
                    {
                        GameServices.RemoveService<int>();
                        GameServices.AddService<int>(0);
                    }

                    //Loads the TitleScreen -----Later the GameOver Screen First
                    Type newClass = Type.GetType("ShapeShift.ScoreScreen"); //whatever your namespace is
                    ScreenManager.Instance.AddScreen((GameScreen)Activator.CreateInstance(newClass), inputManager);


                }
                 

                }


               



            else if (paused)
            {
                if (inputManager.KeyPressed(Keys.P))
                    paused = false;
                inputManager.Update();
            }

     }

        private void loadEnemies(int Level)
        {
            enemyList = new List<MatrixEnemy>();
            //Console.WriteLine(Level);
            enemiesLoaded = true;

            pauseTimer = false;

            switch (Level)
            {

                case 1: 
                    
                    dummyEnemy = new MatrixEnemy(new Vector2 (500,500),this);
                    dummyEnemy.LoadContent(content, 2, 2);
                    enemyList.Add(dummyEnemy);
                    break;

                case 2: 
                    dummyEnemy = new MatrixEnemy(new Vector2 (500,500),this);
                    dummyEnemy.LoadContent(content, 3, 3);
                    enemyList.Add(dummyEnemy);
                    

                    dummyEnemy = new MatrixEnemy(new Vector2 (200,200),this);
                    dummyEnemy.LoadContent(content, 2, 2);
                    enemyList.Add(dummyEnemy);
                    break;

                case 3:
                    dummyEnemy = new MatrixEnemy(new Vector2(500, 500), this);
                    dummyEnemy.LoadContent(content, 3, 3);
                    enemyList.Add(dummyEnemy);
                    break;
                case 4:
                    dummyEnemy = new MatrixEnemy(new Vector2(500, 450), this);
                    dummyEnemy.LoadContent(content, 5, 1);
                    enemyList.Add(dummyEnemy);


                    dummyEnemy = new MatrixEnemy(new Vector2(200, 200), this);
                    dummyEnemy.LoadContent(content, 2, 3);
                    enemyList.Add(dummyEnemy);
                    break;


                   
            }
           
        }

        

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            map.Draw(spriteBatch);
            player.Draw(spriteBatch);

            foreach (MatrixEnemy e in enemyList)
                e.Draw(spriteBatch);


            spriteBatch.DrawString(font, timeRemaining.ToString(), new Vector2(175, 5), Color.White);
            spriteBatch.DrawString(font, "score: " + score.ToString(), new Vector2(270, 5), Color.White);

            if(paused)
            spriteBatch.DrawString(font, "GAME PAUSED", new Vector2(525, 5), Color.White);

            if(GameOver)
            spriteBatch.DrawString(font, "<<GAME OVER!>>", new Vector2(500, 5), Color.Red);

            for (int i = 0; i < player.getMaxHealth(); i++)
            {
                fillHealth(i, spriteBatch);
            }

        }

        public void fillHealth(int step, SpriteBatch spriteBatch)
        {
            if (step > player.getHealth() - 1)
                spriteBatch.Draw(healthUnfillTexture, healthRectangle[step], Color.White);
            else
                spriteBatch.Draw(healthFillTexture, healthRectangle[step], Color.White);
        }
    }
}
