using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    class Triangle : Shape
    {
        Texture2D triangleTexture;

        public Triangle(ContentManager content)
        {
            triangleTexture = content.Load<Texture2D>("PlayerImage3");
        }

        public override Texture2D getTexture()
        {
            return triangleTexture;
        }
    }
}
