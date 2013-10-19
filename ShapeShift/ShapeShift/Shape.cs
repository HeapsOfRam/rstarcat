using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ShapeShift
{
    class Shape
    {
        Texture2D texture;

        public Shape()
        {    
        }

        public virtual Texture2D getTexture()
        {
            return texture;
        }
    }
}
