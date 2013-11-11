using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace ShapeShift
{
    //Screen that shows who made the game before it even loads the main menu

    //The splash screen can be a game screen

    class SplashScreen : GameScreen
    {

        SpriteFont font;
        List<FadeAnimation> fade;
        List<Texture2D> images;

        List<string> splashStrings;

        FileManager fileManager;

        int imageNumber;

        public override void LoadContent(ContentManager Content, InputManager inputManager)
        {
            base.LoadContent(Content, inputManager);
            if (font == null)
                font = this.content.Load<SpriteFont>("Fonts/Font1");
            //unloads the content when we dont need it. loads content when we do

            fileManager = new FileManager();
            fade = new List<FadeAnimation>();
            images = new List<Texture2D>();
            splashStrings = new List<string>();
            imageNumber = 0;

            fileManager.LoadContent("Load/Splash.starcat", attributes, contents);
                //Load all the files, and store them in attributes and contents
               //the file extension can be whatever you want. he does cme, for CodingMadeEasy

            for (int i = 0; i < attributes.Count; i++)
            { 
            for(int j = 0; j < attributes[i].Count; j++)
                {
                switch(attributes[i][j])
                {
                    case "Image":
                        images.Add(this.content.Load<Texture2D>(contents[i][j]));
                        fade.Add(new FadeAnimation());
                        //Console.WriteLine("image to be loaded: " + contents[i][j]);
                        break;

                    //you could make another case for 'sound' or whatever you need
                }
                
                }
            }

            for (int i = 0; i < fade.Count; i++)
            {
                fade[i].LoadContent(content, images[i], "", new Vector2(0, 0));
                //Formula: ImageWidth / 2 * scale - (imageWidth/2)
                //Formula: ImageHeight / 2 * scale - (imageHeight/2)
                fade[i].Scale = 1.0f;
                fade[i].IsActive = true;
            }

        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            fileManager = null;
        }

        public override void Update(GameTime gameTime)
        {

            inputManager.Update();

            fade[imageNumber].Update(gameTime);

            if (fade[imageNumber].Alpha == 0.0f)
                imageNumber++;

            if (imageNumber >= fade.Count - 1 || inputManager.KeyPressed(Keys.Z))
            {
                ScreenManager.Instance.AddScreen(new TitleScreen(), inputManager);
            
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            fade[imageNumber].Draw(spriteBatch);
    
        }

    }
}
