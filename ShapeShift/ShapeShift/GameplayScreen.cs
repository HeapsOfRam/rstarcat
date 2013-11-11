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
        Layers layer;
        Map map;
        Rectangle rectangle;
        Rectangle[] healthRectangle;
        Texture2D healthFillTexture, healthUnfillTexture;

        private SpriteFont font;
        int screenWidth, screenHeight, counter, maxCount = 10;
        int timeRemaining;
        const int HUDHEIGHT = 50, ABSZERO = 0, HEALTHSIZEX = 25, HEALTHSIZEY = 25, HEALTHOFFSETX = 150, HEALTHOFFSETY = 60, DISPLACEHEALTH = 5;

        float countDuration = 1f, currentTime = 0f;

        public override void LoadContent(ContentManager content, InputManager input)
        {
            base.LoadContent(content, input);
            player = new Player();
            layer = new Layers();
            map = new Map();
            rectangle = new Rectangle();
            font = this.content.Load<SpriteFont>("Fonts/Font1");
            map.LoadContent(content, "Map1");
            //layer.LoadContent(content, "Map1");
            player.LoadContent(content, input);
            healthRectangle = new Rectangle[player.getMaxHealth()];
            healthFillTexture = content.Load<Texture2D>("Lain");
            healthUnfillTexture = content.Load<Texture2D>("lainbackground");
        }


        public override void UnloadContent()
        {
            base.UnloadContent();
            player.UnloadContent();
            map.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            inputManager.Update();
            player.Update(gameTime, inputManager, map.collision, map.layer);
            map.Update(gameTime);

            if (inputManager.KeyDown(Keys.Up, Keys.W))
                player.moveUp(gameTime);
            if (inputManager.KeyDown(Keys.Down, Keys.S))
                player.moveDown(gameTime);
            if (inputManager.KeyDown(Keys.Left, Keys.A))
                player.moveLeft(gameTime);
            if (inputManager.KeyDown(Keys.Right, Keys.D))
                player.moveRight(gameTime);
            if (inputManager.KeyDown(Keys.R))
                player.rAction();
            if (inputManager.KeyDown(Keys.E))
                player.eAction();


            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

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

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            map.Draw(spriteBatch);
            player.Draw(spriteBatch);
            spriteBatch.DrawString(font, timeRemaining.ToString(), new Vector2(175, 5), Color.White);
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
