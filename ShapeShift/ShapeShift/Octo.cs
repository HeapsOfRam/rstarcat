using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeShift
{
    class Octo : Shape
    {
        private Texture2D octoIdleTexture;
        private Texture2D octoShadowTexture;

        private SpriteSheetAnimation octoIdleAnimation;
        private SpriteSheetAnimation octoShadowUpAnimation;


        public Octo(ContentManager content)
        {
            animations = new List<SpriteSheetAnimation>();

            octoIdleTexture = content.Load<Texture2D>("Triangle/TriangleIdleSpriteSheet");
            //octoShadowTexture = content.Load<Texture2D>("Triangle/TriangleShadowUp");

            octoIdleAnimation = new SpriteSheetAnimation(this, true, 92);
            octoIdleAnimation.LoadContent(content, octoIdleTexture, "", new Vector2(0, 0));
            octoIdleAnimation.IsEnabled = true;

           /* octoShadowUpAnimation = new SpriteSheetAnimation(this, true, 92);
            octoShadowUpAnimation.LoadContent(content, octoShadowTexture, "", new Vector2(0, 0));
            octoShadowUpAnimation.IsEnabled = false;*/

            animations.Add(octoIdleAnimation);
           
        }

        public override Texture2D getTexture()
        {
            return octoIdleTexture;
        }

        public override void hit()
        {
            //triangleHitAnimation.IsEnabled = true;
        }

        public override void disableAnimation(SpriteSheetAnimation spriteSheetAnimation)
        {
            spriteSheetAnimation.IsEnabled = false;

          
        }


    }

}
