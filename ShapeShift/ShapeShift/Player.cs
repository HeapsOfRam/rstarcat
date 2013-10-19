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
        private Shape playerShape;
        private ContentManager content;
        
        public Player(int x, int y, ContentManager content)
        {
            this.content = content;
            health = FULL;
            rectangle = new Rectangle(x, y, SIZE, SIZE);
            playerShape = new Square(content);
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

        public void setX(int x)
        { rectangle.X = x; }

        public void setY(int y)
        { rectangle.Y = y; }

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

        private void changeToCircle()
        {
            playerShape = new Circle(content);
        }

        private void changeToSquare()
        {
            playerShape = new Square(content);
        }

        private void changeToTriangle()
        {
            playerShape = new Triangle(content);
        }

        private void changeToDiamond()
        {
            playerShape = new Diamond(content);
        }

        public void shiftShape()
        {
            changeToDiamond();
        }

    }
}
