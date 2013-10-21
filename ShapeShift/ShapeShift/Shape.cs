using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace ShapeShift
{
    class Shape
    {
        //Texture2D texture;
        private Rectangle rectangle;
        private const int X = 0, Y = 0, SIZE = 50;

        public Shape()
        {
            rectangle = new Rectangle(X, Y, SIZE, SIZE);
        }

        public virtual Texture2D getTexture()
        {
            return null;
        }

        public Rectangle getRectangle()
        { return rectangle; }
    }
}
