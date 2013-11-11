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
    public class TitleScreen : GameScreen
    {
        SpriteFont font;
        MenuManager menu;

        public override void LoadContent(ContentManager Content, InputManager inputManager)
        {
            base.LoadContent(Content, inputManager);
            if (font == null)
                font = this.content.Load<SpriteFont>("TitleFont");
            menu = new MenuManager();
            menu.LoadContent(content, "Title");

            Song song = Content.Load<Song>("White Denim - D - At The Farm");  // Put the name of your song in instead of "song_title"
            //MediaPlayer.Play(song);
            MediaPlayer.Volume = .5f;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            menu.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            inputManager.Update();
            menu.Update(gameTime, inputManager);

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            
            menu.Draw(spriteBatch);
        }
    }
}
