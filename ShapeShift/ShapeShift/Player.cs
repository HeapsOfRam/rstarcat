using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    class Player
    {
        private int  health;
        private const int FULL = 3, MID = 2, LOW = 1, EMPTY = 0, SIZE = 60, MOVE = 5;
        private Rectangle rectangle;
        private Shape playerShape, nextShape;
        private Square pSquare;
        private Circle pCircle;
        private Triangle pTriangle;
        private Diamond pDiamond;
        private ContentManager content;
        private Random rand;
        private int r;
        
        public Player(int x, int y, ContentManager content)
        {
            this.content = content;
            pSquare = new Square(content);
            pCircle = new Circle(content);
            pTriangle = new Triangle(content);
            pDiamond = new Diamond(content);
            health = FULL;
            rectangle = new Rectangle(x, y, SIZE, SIZE);
            playerShape = pSquare;
            rand = new Random();
            queueOne();
        }

        public int getX()
        { return rectangle.X; }

        public int getY()
        { return rectangle.Y; }

        public int getWidth()
        { return SIZE; }

        public int getHeight()
        { return SIZE; }

        public Rectangle getRect()
        { return rectangle; }

        public Texture2D getTexture()
        { return playerShape.getTexture(); }

        public Shape getNextShape()
        { return nextShape; }

        public Rectangle getNextRectangle()
        { return nextShape.getRectangle(); }

        public void setX(int x)
        { rectangle.X = x; }

        public void setY(int y)
        { rectangle.Y = y; }

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

        public void moveLeft()
        {
            rectangle.X -= MOVE;
        }

        public void moveRight()
        {
            rectangle.X += MOVE;
        }

        public void moveUp()
        {
            rectangle.Y -= MOVE;
        }

        public void moveDown()
        {
            rectangle.Y += MOVE;
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

 /*       private void changeToCircle()
        {
            playerShape = pCircle;
        }

        private void changeToSquare()
        {
            playerShape = pSquare;
        }

        private void changeToTriangle()
        {
            playerShape = pTriangle;
        }

        private void changeToDiamond()
        {
            playerShape = pDiamond;
        } */

        public void shiftShape()
        {
            playerShape = nextShape;
            nextShape = null;
            queueOne();
        }

    }
}
