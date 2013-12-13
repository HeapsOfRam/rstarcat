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
        String record = "";

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
            {
                return true;
            }
            return false;        
        }


        public bool KeyPressed(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (keyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key))
                {
                    return true;
                }
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

        public String getKeyPress() //change return to String
        {
            String currentKeyString ="";
           
            if(keyState.IsKeyDown(Keys.A) &&
                prevKeyState.IsKeyUp(Keys.A))
            {
            currentKeyString = "A";
            }
            else if (keyState.IsKeyDown(Keys.B) &&
               prevKeyState.IsKeyUp(Keys.B))
            {
                currentKeyString = "B";
            }
            else if (keyState.IsKeyDown(Keys.C) &&
               prevKeyState.IsKeyUp(Keys.C))
            {
                currentKeyString = "C";
            }
            else if (keyState.IsKeyDown(Keys.D) &&
              prevKeyState.IsKeyUp(Keys.D))
            {
                currentKeyString = "D";
            }
            else if (keyState.IsKeyDown(Keys.E) &&
              prevKeyState.IsKeyUp(Keys.E))
            {
                currentKeyString = "E";
            }
            else if (keyState.IsKeyDown(Keys.F) &&
              prevKeyState.IsKeyUp(Keys.F))
            {
                currentKeyString = "F";
            }
            else if (keyState.IsKeyDown(Keys.G) &&
              prevKeyState.IsKeyUp(Keys.G))
            {
                currentKeyString = "G";
            }
            else if (keyState.IsKeyDown(Keys.H) &&
              prevKeyState.IsKeyUp(Keys.H))
            {
                currentKeyString = "H";
            }
            else if (keyState.IsKeyDown(Keys.I) &&
              prevKeyState.IsKeyUp(Keys.I))
            {
                currentKeyString = "I";
            }
            else if (keyState.IsKeyDown(Keys.J) &&
              prevKeyState.IsKeyUp(Keys.J))
            {
                currentKeyString = "J";
            }
            else if (keyState.IsKeyDown(Keys.K) &&
              prevKeyState.IsKeyUp(Keys.K))
            {
                currentKeyString = "K";
            }
            else if (keyState.IsKeyDown(Keys.L) &&
              prevKeyState.IsKeyUp(Keys.L))
            {
                currentKeyString = "L";
            }
            else if (keyState.IsKeyDown(Keys.M) &&
              prevKeyState.IsKeyUp(Keys.M))
            {
                currentKeyString = "M";
            }
            else if (keyState.IsKeyDown(Keys.N) &&
              prevKeyState.IsKeyUp(Keys.N))
            {
                currentKeyString = "N";
            }
            else if (keyState.IsKeyDown(Keys.O) &&
              prevKeyState.IsKeyUp(Keys.O))
            {
                currentKeyString = "O";
            }
            else if (keyState.IsKeyDown(Keys.P) &&
              prevKeyState.IsKeyUp(Keys.P))
            {
                currentKeyString = "P";
            }
            else if (keyState.IsKeyDown(Keys.Q) &&
              prevKeyState.IsKeyUp(Keys.Q))
            {
                currentKeyString = "Q";
            }
            else if (keyState.IsKeyDown(Keys.R) &&
              prevKeyState.IsKeyUp(Keys.R))
            {
                currentKeyString = "R";
            }
            else if (keyState.IsKeyDown(Keys.S) &&
              prevKeyState.IsKeyUp(Keys.S))
            {
                currentKeyString = "S";
            }
            else if (keyState.IsKeyDown(Keys.T) &&
              prevKeyState.IsKeyUp(Keys.T))
            {
                currentKeyString = "T";
            }
            else if (keyState.IsKeyDown(Keys.U) &&
              prevKeyState.IsKeyUp(Keys.U))
            {
                currentKeyString = "U";
            }
            else if (keyState.IsKeyDown(Keys.V) &&
              prevKeyState.IsKeyUp(Keys.V))
            {
                currentKeyString = "V";
            }
            else if (keyState.IsKeyDown(Keys.W) &&
              prevKeyState.IsKeyUp(Keys.W))
            {
                currentKeyString = "W";
            }
            else if (keyState.IsKeyDown(Keys.X) &&
              prevKeyState.IsKeyUp(Keys.X))
            {
                currentKeyString = "X";
            }
            else if (keyState.IsKeyDown(Keys.Y) &&
              prevKeyState.IsKeyUp(Keys.Y))
            {
                currentKeyString = "Y";
            }
            else if (keyState.IsKeyDown(Keys.Z) &&
              prevKeyState.IsKeyUp(Keys.Z))
            {
                currentKeyString = "Z";
            }


            return currentKeyString;

        }

    }
}
