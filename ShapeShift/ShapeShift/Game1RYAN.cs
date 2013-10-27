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
/*
namespace ShapeShift
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1RYAN : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player1;

        int screenWidth, screenHeight, counter, maxCount = 15;
        const int HUDHEIGHT = 50, ABSZERO = 0;

        float countDuration = 1f, currentTime = 0f;

        public Game1RYAN()
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
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            player1 = new Player(240, 240, Content);

            screenHeight = GraphicsDevice.Viewport.Height;
            screenWidth = GraphicsDevice.Viewport.Width;
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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

            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (currentTime >= countDuration)
            {
                counter++;
                currentTime -= countDuration;
            }
            if (counter >= maxCount)
            {
                player1.shiftShape();
                counter = 0;
            }

            //for testing, to be removed
            if (Keyboard.GetState().IsKeyDown(Keys.H))
                player1.shiftShape();
            //************************************


            //Taken care of in the Update Method of the Player Class
            /*
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                player1.moveUp();
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                player1.moveDown();
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                player1.moveLeft();
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                player1.moveRight();
             

            if (player1.getX() <= ABSZERO)
                player1.setX(ABSZERO);
            if (player1.getX() >= screenWidth - player1.getWidth())
                player1.setX(screenWidth - player1.getWidth());

            if (player1.getY() <= HUDHEIGHT)
                player1.setY(HUDHEIGHT);
            if (player1.getY() >= screenHeight - player1.getHeight())
                player1.setY(screenHeight - player1.getHeight());

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(player1.getNextShape().getTexture(), player1.getNextRectangle(), Color.White);
            spriteBatch.Draw(player1.getTexture(), player1.getRect(), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
*/