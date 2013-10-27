using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    class Diamond : Shape
    {
        Texture2D diamondTexture;

        public Diamond(ContentManager content)
        {
            diamondTexture = content.Load<Texture2D>("PlayerImage4");
        }

        public void attack()
        {
        }

        public override Texture2D getTexture()
        {
            return diamondTexture;
        }
    }
}
