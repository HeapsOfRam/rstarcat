using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    class Square : Shape
    {
        Texture2D squareTexture;

        public Square(ContentManager content)
        {
            squareTexture = content.Load<Texture2D>("PlayerImage1");
        }

        public void attack()
        {
        }

        public override Texture2D getTexture()
        {
            return squareTexture;
        }
    }
}
