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
        SpriteSheetAnimation nextShapeAnimation;
         

        public Texture2D PlayerTexture
        {
            get { return playerShape.getTexture(); }
        }

        public Vector2 Position
        {
            get { return position;}
            set {position = value;}
        }


        public override void LoadContent(ContentManager content, InputManager input)
        {
            base.LoadContent(content, input);
            contentGlobal = content;
            fileManager = new FileManager();
            moveAnimation = new SpriteSheetAnimation();
            nextShapeAnimation = new SpriteSheetAnimation();
            Vector2 tempFrames = Vector2.Zero;
            moveSpeed = 150f; //Set the move speed
            gameTime = new GameTime();
            imageList = new List<Texture2D>();

            this.content = content;
            pSquare = new Square(content);
            pCircle = new Circle(content);
            pTriangle = new Triangle(content);
            pDiamond = new Diamond(content);
            health = FULL;
      //      rectangle = new Rectangle(x, y, SIZE, SIZE);
            playerShape = pSquare;
            rand = new Random();
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
                            tempFrames = new Vector2(int.Parse(frames[0]),int.Parse(frames[1]));
                            break;
                        case "Image":
                            tempImage = this.content.Load<Texture2D>(contents[i][j]);
                            imageList.Add(tempImage);
                            //Post-integration: This is not really needed anymore, as the individual classes have textures loaded
                            break;
                        case "Position":
                            frames = contents[i][j].Split(' ');
                            position = new Vector2(int.Parse(frames[0]),int.Parse(frames[1]));
                            break;
                    
                    }
                }
            }

            nextShapeAnimation.LoadContent(contentGlobal, nextShape.getTexture(), "", new Vector2(0, 0));
            moveAnimation.LoadContent(content, playerShape.getTexture(), "", position);


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


        /*
        private void moveLeft()
        {
            moveAnimation.CurrentFrame = new Vector2(moveAnimation.CurrentFrame.X, 1);
            position.X -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void moveRight()
        {
            moveAnimation.CurrentFrame = new Vector2(moveAnimation.CurrentFrame.X, 2);
            position.X += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void moveUp()
        {
            moveAnimation.CurrentFrame = new Vector2(moveAnimation.CurrentFrame.X, 3);
            position.Y -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void moveDown()
        {
            moveAnimation.CurrentFrame = new Vector2(moveAnimation.CurrentFrame.X, 0);
            position.Y += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        */

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
            nextShape = null;
            queueOne();
            moveAnimation.LoadContent(contentGlobal, playerShape.getTexture(), "", position);   //This is in the update method, so the shape shifts when shapeshift is called
        }


        #endregion


        public override void Update(GameTime gameTime, InputManager input, Collision col, Layers layer)
        {
            //MOVEMENT
            if (input.KeyDown(Keys.Right, Keys.D)) //MOVE RIGHT
            {
                moveAnimation.CurrentFrame = new Vector2(moveAnimation.CurrentFrame.X, 2);
                position.X += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (input.KeyDown(Keys.Left, Keys.A)) //MOVE LEFT
            {
                moveAnimation.CurrentFrame = new Vector2(moveAnimation.CurrentFrame.X, 1);
                position.X -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (input.KeyDown(Keys.Down, Keys.S)) //MOVE DOWN
            {
                moveAnimation.CurrentFrame = new Vector2(moveAnimation.CurrentFrame.X, 0);
                position.Y += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            else if (input.KeyDown(Keys.Up, Keys.W)) //MOVE UP
            {
                moveAnimation.CurrentFrame = new Vector2(moveAnimation.CurrentFrame.X, 3);
                position.Y -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else moveAnimation.IsActive = false;
            
            //Console.WriteLine("col.CollisionMap.Count: " + col.CollisionMap.Count);
            
            
            for (int i = 0; i < col.CollisionMap.Count; i++)
            {
                for (int j = 0; j < col.CollisionMap[i].Count; j++)
                {
                    if (col.CollisionMap[i][j] == "x")
                    {
                        //checks to see if it has collided with map elements that are loaded in
                        if (position.X + moveAnimation.FrameWidth < j * layer.TileDimensions.X ||
                            position.X > j * layer.TileDimensions.X + layer.TileDimensions.X ||
                            position.Y + moveAnimation.FrameHeight < i * layer.TileDimensions.Y ||
                            position.Y > i * layer.TileDimensions.Y + layer.TileDimensions.Y)
                        {
                            //no collision
                        }
                        else
                        {
                            //there is a collision
                            position = moveAnimation.Position; //sets player position to last frame before contact (does not pass through)
                        }
                    }

                }

            }

          //  moveAnimation.LoadContent(contentGlobal, playerShape.getTexture(), "", position);   //This is in the update method, so the shape shifts when shapeshift is called
            nextShapeAnimation.LoadContent(contentGlobal, nextShape.getTexture(), "", new Vector2(0,0)); //this keeps track of the next shape at the top
            moveAnimation.Position = position;
            moveAnimation.Update(gameTime);
           
        
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            moveAnimation.Draw(spriteBatch);
            nextShapeAnimation.Draw(spriteBatch);
        }


    }
}
