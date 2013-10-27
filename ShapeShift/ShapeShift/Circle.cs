using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    class Circle : Shape
    {
        private Texture2D circleTexture;

        public Circle(ContentManager content)
        {
            circleTexture = content.Load<Texture2D>("PlayerImage2");
        }

        public override Texture2D getTexture()
        {
            return circleTexture;
        }
    }
}
