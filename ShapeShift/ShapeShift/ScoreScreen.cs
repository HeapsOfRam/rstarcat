using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace ShapeShift
{
    public class ScoreScreen : GameScreen
    {
        SpriteFont font;
        //MenuManager menu;
        HighScores scoreScreen;

        public override void LoadContent(ContentManager Content, InputManager inputManager)
        {
            base.LoadContent(Content, inputManager);
            if (font == null)
                font = this.content.Load<SpriteFont>("Fonts/TitleFont");
            //menu = new MenuManager();
            //menu.LoadContent(content, "Title");
            scoreScreen = new HighScores();
            scoreScreen.LoadContent(content, "Scores");

        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            //menu.UnloadContent();
            scoreScreen.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            inputManager.Update();
            //menu.Update(gameTime, inputManager);
            scoreScreen.Update(gameTime, inputManager);



        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            //menu.Draw(spriteBatch);
            scoreScreen.Draw(spriteBatch);
        }
    }
}
