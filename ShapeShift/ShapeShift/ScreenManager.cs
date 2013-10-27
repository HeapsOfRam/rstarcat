using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{

    //handles all the screens and the modules

    //split into REGIONS: easier for editing

    //This class is Singleton

    public class ScreenManager
    {

        #region Variables
        //the string is the key/reference. If the ID string is title, it can be used to locate the particular screen.
            //(seems similar to a HashMap)
        //The concept: load things when you need to load them. unload when you no longer need them.
            //This is important for a larger scale game
        private static ScreenManager instance;

        //**Screen Stack**
            //stack of screens. lets us know which screens will open, and in what order.
            //ex: shifting from title screen to options screen and back
        Stack<GameScreen> screenStack = new Stack<GameScreen>();

        Vector2 dimensions;

        //creating custom contentManager
        ContentManager content;

        GameScreen currentScreen;
        GameScreen newScreen;

        bool transition;

        FadeAnimation fade;
        Texture2D fadeTexture;

        Texture2D nullImage;

        InputManager inputManager;

        #endregion

        #region Properties


        public static ScreenManager Instance
        {
            get 
            {
                if (instance == null)
                    instance = new ScreenManager();
                return instance;
            }
        
        }

        public Vector2 Dimensions
        {
            get { return dimensions; }
            set { dimensions = value; }

        }

        public Texture2D NullImage
        {
            get { return nullImage; }
        }

        #endregion

        #region Main Methods

        //the Initialize method can be called whenever you want, as opposed to just initializing in a constructor
        public void AddScreen(GameScreen screen, InputManager inputManager)
        { 
        //when we add a new screen, we want to add it to the top of our screen stack
            transition = true;
            newScreen = screen;
            fade.IsActive = true;
            fade.Alpha = 0.0f;
            fade.ActivateValue = 1.0f;
            //0.0f -- full transparency. increase until it reaches activateValue

            this.inputManager = inputManager;


        }

        public void AddScreen(GameScreen screen, InputManager inputManager, float alpha)
        {
            transition = true;
            newScreen = screen;
            fade.IsActive = true;
            fade.ActivateValue = 1.0f;
            if (alpha != 1.0f)
                fade.Alpha = 1.0f - alpha;
            else
                fade.Alpha = alpha;
            fade.Increase = true;

            this.inputManager = inputManager;
        
        }


        public virtual void Initialize() 
        {
            currentScreen = new SplashScreen();
            fade = new FadeAnimation();
            inputManager = new InputManager();
        }
        public virtual void LoadContent(ContentManager Content) 
        {
            content = new ContentManager(Content.ServiceProvider, "Content");
            currentScreen.LoadContent(Content, inputManager);


            nullImage = this.content.Load<Texture2D>("null");
            fadeTexture = this.content.Load<Texture2D>("fade");
            fade.LoadContent(content, fadeTexture, "", Vector2.Zero);
            fade.Scale = dimensions.X; //if the width is higher then the height of the screen, set it to the width, and vice versa
        }
        public virtual void Update(GameTime gameTime) 
        {
            if (!transition)//mmost likely dont want to update while transitioning screens
                currentScreen.Update(gameTime);
            else
                Transition(gameTime);
        }
        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            currentScreen.Draw(spriteBatch);
            if (transition)
                fade.Draw(spriteBatch);
        }


        #endregion

        #region Private Methods

        private void Transition(GameTime gameTime)
        {

            //Explained at the end of Tutorial 7 -Animation[Part4]]

            fade.Update(gameTime);
            if (fade.Alpha == 1.0f && fade.Timer.TotalSeconds == 1.0f)
            {
                //We push the screen onto the stack
                screenStack.Push(newScreen);
                currentScreen.UnloadContent();
                currentScreen = newScreen;
                currentScreen.LoadContent(content, this.inputManager); //load the new screen's content
                //copies the input over from the last screen
            }
            else if (fade.Alpha == 0.0f)
            {
                transition = false;
                fade.IsActive = false;
            }
        
        }

        #endregion


    }
}
