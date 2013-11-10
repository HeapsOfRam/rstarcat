using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    public class GameplayScreen : GameScreen
    {
        Player player;
        Layers layer;
        Map map;
        Rectangle rectangle;

        private SpriteFont font;
        int screenWidth, screenHeight, counter, maxCount = 10;
        int timeRemaining;
        const int HUDHEIGHT = 50, ABSZERO = 0;

        float countDuration = 1f, currentTime = 0f;

        public override void LoadContent(ContentManager content, InputManager input)
        {
            base.LoadContent(content, input);
            player = new Player();
            layer = new Layers();
            map = new Map();
            rectangle = new Rectangle();
            font = this.content.Load<SpriteFont>("Font1");
            map.LoadContent(content, "Map1");
            //layer.LoadContent(content, "Map1");
            player.LoadContent(content, input);
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
            spriteBatch.DrawString(font, timeRemaining.ToString(), new Vector2(150, 0), Color.White);
           


        }
    }
}
