﻿using System;
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

        private const int FULL = 3, MID = 2, LOW = 1, EMPTY = 0, SIZE = 60, MOVE = 5;
       // private Rectangle rectangle;
        private Shape playerShape, nextShape;
        private Square pSquare;
        private Circle pCircle;
        private Triangle pTriangle;
        private Diamond pDiamond;
        
        private Random rand;

        private int r;

        private Matrix pMatrix;

        private Rectangle lastCheckedRectangle; //basically, it's the last tile on the map checked for collision killa bee

        private const int START_X = 200, START_Y = 200;

        public override void LoadContent(ContentManager content, InputManager input)
        {
            base.LoadContent(content, input);
            rand = new Random();

            this.content  = content;
            moveSpeed     = 150f; //Set the move speed

            moveAnimation = new SpriteSheetAnimation();
            gameTime      = new GameTime();

            //declare the shapes
            pSquare   = new Square(content);
            pCircle   = new Circle(content);
            pTriangle = new Triangle(content);
            pDiamond  = new Diamond(content);
            pMatrix   = new Matrix(content);

            maxHealth = FULL;
            health = maxHealth;

            playerShape = pTriangle;            

      


            //Queue up the next shape
            queueOne();
            position = new Vector2(START_X, START_Y);

            //each shape should have silhouette shape of its own
            //moveAnimation.LoadContent(content, playerShape.getTexture(), "", position);
            
            playerShape.setPosition(position);

            pMatrix.makeMatrix();

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

            moveSpeed = 150f;
            pSquare.stopDashing();

            if (playerShape == pCircle && pCircle.shielded)
                pCircle.removeShield();
            else if (playerShape == pSquare && pSquare.dashing)
                pSquare.stopDashing();
            else if (playerShape == pDiamond)
                pDiamond.clearMines();


            playerShape = nextShape;
            nextShape   = null;
            
            queueOne();

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
       /* public void fixCollision(Vector2 position, Rectangle lastCheckedRectangle)
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
        }*/

        public override void Update(GameTime gameTime, InputManager input, Collision col, Layers layer)
        {
            previousPosition = position;

            Boolean[] directions = new Boolean[4];

            for (int i = 0; i < 4; i++)        
                directions[i] = false;
            
            //MOVEMENT
            if (input.KeyDown(Keys.Right, Keys.D))
            { //MOVE RIGHT
                position.X += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                directions[0] = true;
            }

            if (input.KeyDown(Keys.Left, Keys.A))
            { //MOVE LEFT
                position.X -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                directions[1] = true;
            }
            if (input.KeyDown(Keys.Down, Keys.S))
            { //MOVE DOWN
                position.Y += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                directions[2] = true;
            }
            if (input.KeyDown(Keys.Up, Keys.W))
            { //MOVE UP
                position.Y -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                directions[3] = true;
            }

            if (playerShape == pSquare)
            {
                pSquare.setDirectionMap(directions);
            }

         

            
            //Used to check deploy sheild in circle
            if (input.KeyDown(Keys.R))
            {
                if (playerShape == pCircle && !pCircle.shielded)
                    pCircle.deployShield();

                if (playerShape == pSquare)
                    pSquare.dash(this);

                if (playerShape == pTriangle)
                    pTriangle.PreformRotate();

                if (playerShape == pDiamond && !pDiamond.mineDeployed())
                    pDiamond.deployMine();

                if (playerShape == pMatrix)
                    pMatrix.PreformRotate();
            
            }

            if (input.KeyDown(Keys.E))
            {
                if (playerShape == pCircle && pCircle.shielded)
                    pCircle.removeShield();

                if (playerShape == pDiamond)
                    pDiamond.dropMine();

                if (playerShape == pMatrix)
                    pMatrix.PreformRotate(3);
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
                        if (playerShape.Collides(position, lastCheckedRectangle, layer.getColorData(i, j, col.CollisionMap[i].Count)))
                            position = moveAnimation.Position;
      
                    }
                }
            }


            // moveAnimation is used to check collisions, it is not drawn and is the same for each shape 
            // (just a rectangle corresponding to the image)
            moveAnimation.Position = position;

            // Update all of the enabled animations
            List<SpriteSheetAnimation> Animations = playerShape.getActiveTextures();
            foreach (SpriteSheetAnimation animation in Animations)
            {
                if (playerShape == pMatrix)
                {
                    pMatrix.Update(gameTime);
                    animation.Update(gameTime);
                }
                else if (animation.IsEnabled)
                    animation.Update(gameTime);

                if (!(playerShape == pDiamond && pDiamond.mineDropped() && pDiamond.isMineAnimation(animation)))
                    animation.Position = position;
            }

            // Update the next shape animation in the upper right of the screen
            Animations = nextShape.getActiveTextures();
            foreach (SpriteSheetAnimation animation in Animations)
            {
                if (animation.IsEnabled)
                    animation.Update(gameTime);
            }


            if (playerShape == pMatrix)
            {
                pMatrix.makeMatrix();
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
            enabledAnimations[0].Draw(spriteBatch);
        }
    }
}
