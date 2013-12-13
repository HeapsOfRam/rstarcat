using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ShapeShift
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int currentSong = 0;
      
        List<Song> bgMusicList;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ScreenManager.Instance.Initialize();
            GameServices.AddService<GraphicsDevice>(graphics.GraphicsDevice);
            GameServices.AddService<ContentManager>(Content);
            GameServices.AddService<int>(0);
            

            //we can access any of the public methods from the SingletonClass (ScreenManager)
            ScreenManager.Instance.Dimensions = new Vector2(920,690); //640 x 480 in tutorial
            graphics.PreferredBackBufferWidth = (int)ScreenManager.Instance.Dimensions.X;
            graphics.PreferredBackBufferHeight = (int)ScreenManager.Instance.Dimensions.Y;
            graphics.ApplyChanges();
            base.Initialize();

           // Song song = Content.Load<Song>("Music/White Denim - D - At The Farm");  // Put the name of your song in instead of "song_title"
            //MediaPlayer.Play(song);
            MediaPlayer.Volume = .5f;

            bgMusicList = new List<Song>();

            Song song = Content.Load<Song>("Music/GrooveBox");
            bgMusicList.Add(song);

            

            song = Content.Load<Song>("Music/Waking Up");
            bgMusicList.Add(song);

            song = Content.Load<Song>("Music/Star Death");
            bgMusicList.Add(song);

            song = Content.Load<Song>("Music/Feed The Moon");
            bgMusicList.Add(song);

            song = Content.Load<Song>("Music/Curse The Galaxey");
            bgMusicList.Add(song);

            Random rand = new Random();
            MediaPlayer.Play(bgMusicList[rand.Next(5)]);


        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GameServices.AddService<SpriteBatch>(spriteBatch);

            // TODO: use this.Content to load your game content here
            ScreenManager.Instance.LoadContent(Content);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            //**we need to create seperate content managers for each screen or whaterver we load in
            //Because we do not want to unload everything, only the stuff that we are not using****




        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            
// have a global boolean;   
 
// in the initialize function;  
bool doonce = true;  
//  
 
if (MediaPlayer.State != MediaState.Playing) {  
    if(doonce) {  
        doonce = false;  
        currentSong++;  
        if(currentSong > bgMusicList.Count - 1)  
            currentSong = 0;  
        }  
        MediaPlayer.Play(bgMusicList[currentSong]);  
    }  

    if(MediaPlayer.State == MediaState.Playing) {  
        doonce = true;  
    } 

            // TODO: Add your update logic here
            ScreenManager.Instance.Update(gameTime);
            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            ScreenManager.Instance.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
