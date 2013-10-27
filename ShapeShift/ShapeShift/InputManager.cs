using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;

namespace ShapeShift
{

    //some people may think to have input manager be singleton
    //however, we may need to do 2 players, so we don't want that
    public class InputManager
    {
        KeyboardState prevKeyState, keyState;

        public KeyboardState PrevKeyState
        {
            get { return prevKeyState; }
            set { prevKeyState = value; }

        }

        public KeyboardState KeyState
        {
            get { return keyState; }
            set { keyState = value; }

        }

        public void Update()
        {
            prevKeyState = keyState;
            keyState = Keyboard.GetState();
        }

        public bool KeyPressed(Keys key)
        {
            //Checks for SINGLE KEY PRESSES. If you hold a key down, it will not register, as it sees that
            //the key had already been held down.
            if (keyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key))
                return true;
            return false;        
        }


        public bool KeyPressed(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (keyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key))
                    return true;
            }
            return false; 
        }

        public bool KeyReleased(Keys key)
        {
            //The opposite of KeyPressed
            if (keyState.IsKeyUp(key) && prevKeyState.IsKeyDown(key))
                return true;
            return false;
        }


        public bool KeyReleased(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (keyState.IsKeyUp(key) && prevKeyState.IsKeyDown(key))
                    return true;
            }
            return false;
        }

        public bool KeyDown(Keys key)//check for MULTIPLE KEY PRESSES
        {
            if (keyState.IsKeyDown(key))
                return true;
            return false;
        
        }

        public bool KeyDown(params Keys[] keys) // for multiple things
        {

            foreach (Keys key in keys)
            {
                if (keyState.IsKeyDown(key))
                    return true;
            }
            return false;
        }

    }
}
