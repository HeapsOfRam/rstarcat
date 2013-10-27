using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    //similar to an abstract class
    //virtual methods for other classes to distribute

    public class GameScreen
    {
        protected ContentManager content;
        protected List<List<string>> attributes, contents; //two seperate declarations

        protected InputManager inputManager;

        public virtual void LoadContent(ContentManager Content, InputManager inputManager) 
        {
            //HE SAYS IN TUTORIAL 13 THAT HE WILL LATER ADD INPUTMANAGER TO THE PARAMETERS
            
            //remember, we will have different content managers for different screens
                //easier for unloading
            content = new ContentManager(Content.ServiceProvider, "Content");
            attributes = new List<List<string>>();
            contents = new List<List<string>>();
            this.inputManager = new InputManager();

        }
        public virtual void UnloadContent() 
        {
            content.Unload();
            inputManager = null;
            attributes.Clear();
            contents.Clear();
        }
        public virtual void Update(GameTime gameTime) {}
        public virtual void Draw(SpriteBatch spriteBatch){}

    }
}
