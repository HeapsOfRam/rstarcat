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
    public class Player : Entity
    {
        GameTime gameTime;

        
       // private int  health;
        private const int FULL = 3, MID = 2, LOW = 1, EMPTY = 0, SIZE = 60, MOVE = 5;
       // private Rectangle rectangle;
        private Shape playerShape, nextShape;
       // Texture2D image1, image2, image3, image4;
        Texture2D tempImage;
        List<Texture2D> imageList;
        private Square pSquare;
        private Circle pCircle;
        private Triangle pTriangle;
        private Diamond pDiamond;
       // private ContentManager content;
        private Random rand;
        private int r;
        ContentManager contentGlobal;



        private Rectangle lastCheckedRectangle;
         

        public Texture2D PlayerTexture
        {
            get { return playerShape.getTexture(); }
        }

        public Vector2 Position
        {
            get {return position;}
            set {position = value;}
        }


        public override void LoadContent(ContentManager content, InputManager input)
        {
            base.LoadContent(content, input);

            this.content  = content;
            contentGlobal = content;
            moveSpeed     = 150f; //Set the move speed

            fileManager   = new FileManager();
            moveAnimation = new SpriteSheetAnimation();
            gameTime      = new GameTime();
            imageList     = new List<Texture2D>();

            Vector2 tempFrames = Vector2.Zero;
            
            pSquare   = new Square(content);
            pCircle   = new Circle(content);
            pTriangle = new Triangle(content);
            pDiamond  = new Diamond(content);

            health = FULL;
            playerShape = pCircle;

            rand = new Random();

            //Queue up the next shape
            queueOne();

            fileManager.LoadContent("Load/Player.starcat", attributes, contents);
            for (int i = 0; i < attributes.Count; i++)
            {
                for (int j = 0; j < attributes[i].Count; j++)
                {
                    switch (attributes[i][j])
                    {
                        case "Health":
                            health = int.Parse(contents[i][j]);
                            break;
                        case "Frames":
                            string[] frames = contents[i][j].Split(' ');
                            tempFrames = new Vector2(int.Parse(frames[0]), int.Parse(frames[1]));
                            break;
                        case "Image":
                            tempImage = this.content.Load<Texture2D>(contents[i][j]);
                            imageList.Add(tempImage);
                            //Post-integration: This is not really needed anymore, as the individual classes have textures loaded
                            break;
                        case "Position":
                            frames = contents[i][j].Split(' ');
                            position = new Vector2(int.Parse(frames[0]), int.Parse(frames[1]));
                            break;

                    }
                }
            }

            moveAnimation.LoadContent(content, playerShape.getTexture(), "", position);
            playerShape.setPosition(position);

        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            moveAnimation.UnloadContent();
        }

        #region MovementMethods

        private void queueOne()
        {
           do
            {
                r = rand.Next(4) + 1;
                if (r == 1)
                    nextShape = pCircle;
                if (r == 2)
                    nextShape = pSquare;
                if (r == 3)
                    nextShape = pDiamond;
                if (r == 4)
                    nextShape = pTriangle;
            } while (nextShape == playerShape);          
        }

        public void takeDamage()
        {
            health--;
            if (health == EMPTY)
                die();
        }

        public void die()
        {
        }

        public void shiftShape()
        {
            playerShape = nextShape;
            nextShape   = null;
            
            queueOne();

            fixCollision(position, lastCheckedRectangle);
            

            //Resets the locaiton of the next shape to the upper right corner
            List<SpriteSheetAnimation> Animations = nextShape.getActiveTextures();
            foreach (SpriteSheetAnimation animation in Animations)
            {
                if (animation.IsEnabled)
                    animation.position = new Vector2(0, 0); 
            }
        }

        #endregion

        // Checks to see if the next shape is colliding with anything before switching 
        // If it is...it incremently trys moving the shape backward until it isn't colliding
        public void fixCollision(Vector2 position, Rectangle lastCheckedRectangle)
        {
            int count = 1;
            while (playerShape.Collides(position, lastCheckedRectangle))
            {
                if (!(playerShape.Collides(new Vector2(position.X + count, position.Y + count), lastCheckedRectangle)))
                    position = new Vector2(position.X + count, position.Y + count);
                else if (!(playerShape.Collides(new Vector2(position.X - count, position.Y - count), lastCheckedRectangle)))
                    position = new Vector2(position.X + count, position.Y + count);
                else if (!(playerShape.Collides(new Vector2(position.X - count, position.Y + count), lastCheckedRectangle)))
                    position = new Vector2(position.X + count, position.Y + count);
                else if (!(playerShape.Collides(new Vector2(position.X + count, position.Y - count), lastCheckedRectangle)))
                    position = new Vector2(position.X + count, position.Y + count);

                count++;
            }
        }

        public override void Update(GameTime gameTime, InputManager input, Collision col, Layers layer)
        {

            //MOVEMENT
            if (input.KeyDown(Keys.Right, Keys.D)) //MOVE RIGHT
                position.X += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (input.KeyDown(Keys.Left, Keys.A)) //MOVE LEFT
                position.X -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (input.KeyDown(Keys.Down, Keys.S)) //MOVE DOWN
                position.Y += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (input.KeyDown(Keys.Up, Keys.W)) //MOVE UP
                position.Y -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            
            //Used to check deploy sheild in circle
            if (input.KeyDown(Keys.R))
            {
                if (playerShape == pCircle)
                    pCircle.deployShield();

             fixCollision(position, lastCheckedRectangle);
            }

            if (input.KeyDown(Keys.E))
            {
                if (playerShape == pCircle)
                    pCircle.removeShield();
            }

            

            for (int i = 0; i < col.CollisionMap.Count; i++)
            {
                for (int j = 0; j < col.CollisionMap[i].Count; j++)
                {
                    if (col.CollisionMap[i][j] == "x")
                    {
                                
                        //Creates a rectangle that is the current tiles postion and size
                        lastCheckedRectangle = new Rectangle((int)(j * layer.TileDimensions.X), (int)(i * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));
                       
                        //Calls Collides method in shape class, in which each shape will check collisions uniquely 
                        if (playerShape.Collides(position, lastCheckedRectangle)) 
                            position = moveAnimation.Position;
      
                    }
                }
            }

            moveAnimation.Position = position;

            // Update all of the enabled animations
            List<SpriteSheetAnimation> Animations = playerShape.getActiveTextures();
            foreach (SpriteSheetAnimation animation in Animations)
            {
                if (animation.IsEnabled)
                    animation.Update(gameTime);

                animation.Position = position;
            }

            // Update the next shape animation in the upper right of the screen
            Animations = nextShape.getActiveTextures();
            foreach (SpriteSheetAnimation animation in Animations)
            {
                if (animation.IsEnabled)
                    animation.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
           
            // Draws each of the enabled animations for the current shape
            List<SpriteSheetAnimation> enabledAnimations = playerShape.getActiveTextures();
            foreach (SpriteSheetAnimation animation in enabledAnimations)
            {
                if (animation.IsEnabled)
                    animation.Draw(spriteBatch);
            }

            // Draws each of the enabled animations for the current shape in the upper right hand corner. 
            enabledAnimations = nextShape.getActiveTextures();
            foreach (SpriteSheetAnimation animation in enabledAnimations)
            {
                if (animation.IsEnabled)
                    animation.Draw(spriteBatch);
            }
        }
    }
}
