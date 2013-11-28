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
        Player player;
        Enemy dummyEnemy;
        Layers layer;
        Map map;
        Rectangle rectangle;
        Rectangle[] healthRectangle;
        Texture2D healthFillTexture, healthUnfillTexture;
        Boolean paused = false;
        Boolean pauseTimer = false;
        String Level = "";
        String Level1 = "1";
        String Level2 = "2";
        String Level3 = "3";
        String previousLevel = "";
        Random randomLevelGenerator;
        int levelNumber;
        FileManager fileManager;
        List<Texture2D> images;

        bool LevelCompleted;
        int imageNumber;


        private SpriteFont font;
        int screenWidth, screenHeight, counter, maxCount = 10;
        int timeRemaining;
        const int HUDHEIGHT = 50, ABSZERO = 0, HEALTHSIZEX = 25, HEALTHSIZEY = 25, HEALTHOFFSETX = 150, HEALTHOFFSETY = 60, DISPLACEHEALTH = 5;

        float countDuration = 1f, currentTime = 0f;

        public override void LoadContent(ContentManager content, InputManager input)
        {
            base.LoadContent(content, input);
            player = new Player();
            dummyEnemy = new MatrixEnemy();
            layer = new Layers();
            map = new Map();
            rectangle = new Rectangle();
            font = this.content.Load<SpriteFont>("Fonts/Font1");
            Level = Level1; //FIRST LEVEL
            map.LoadContent(content, "InProgressMap1");
            player.LoadContent(content, input);
            dummyEnemy.LoadContent(content, 2, 2);
            healthRectangle = new Rectangle[player.getMaxHealth()];
            healthFillTexture = content.Load<Texture2D>("heart");
            healthUnfillTexture = content.Load<Texture2D>("lainbackground");
            levelNumber = 1;
            Level = "1";
            previousLevel = Level;
            randomLevelGenerator = new Random();
            LevelCompleted = false;

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
                dummyEnemy.Update(gameTime, map.collision, map.layer, player);
                map.Update(gameTime);
                LevelCompleted = false;

                player.pSquareResetDirections();

                if (dummyEnemy.getEnemyShape().collides(dummyEnemy.getPosition(), player.getRectangle(), player.getShape().getColorData()))
                {
                    if (!player.takeDamage())
                        dummyEnemy.makeReel();
                }

                if (inputManager.KeyDown(Keys.W))
                    player.moveUp(gameTime);
                if (inputManager.KeyDown(Keys.S))
                    player.moveDown(gameTime);
                if (inputManager.KeyDown(Keys.A))
                    player.moveLeft(gameTime);
                if (inputManager.KeyDown(Keys.D))
                    player.moveRight(gameTime);
                if (inputManager.KeyDown(Keys.R))
                    player.rAction();
                if (inputManager.KeyDown(Keys.E))
                    player.eAction();

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

                //LOADING IN THE NEXT LEVEL

                if (inputManager.KeyPressed(Keys.Space))
                {
                    LevelCompleted = true;
                }

                if (LevelCompleted)
                {
                    map.LoadContent(content, "CompletedMap" + previousLevel);
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

                }

            }//EndNotPause
            else if (paused)
            {
                if (inputManager.KeyPressed(Keys.P))
                    paused = false;
                inputManager.Update();
            }

            

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            map.Draw(spriteBatch);
            player.Draw(spriteBatch);
            dummyEnemy.Draw(spriteBatch);
            spriteBatch.DrawString(font, timeRemaining.ToString(), new Vector2(175, 5), Color.White);

            if(paused)
            spriteBatch.DrawString(font, "GAME PAUSED", new Vector2(300, 5), Color.White);

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
