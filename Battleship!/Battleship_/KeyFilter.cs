using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;

namespace Battleship_
{
    class KeyFilter
    {
        Keys[] lastKeysDown;

        internal KeyFilter()
        {
            lastKeysDown = new Keys[1];
        }

        internal List<Keys> GetPressedKeys(KeyboardState _keyboardState)
        {
            List<Keys> pressedKeys = new List<Keys>();
            Keys[] currentDownKeys = _keyboardState.GetPressedKeys();
            foreach (Keys key in lastKeysDown)
            {
                if (!currentDownKeys.Contains(key))
                {
                    pressedKeys.Add(key);
                }
            }
            lastKeysDown = currentDownKeys;
            return pressedKeys;
        }

        internal List<Keys> GetHeldDownKeys(KeyboardState _keyboardState)
        {
            List<Keys> heldDownKeys = new List<Keys>();
            Keys[] currentDownKeys = _keyboardState.GetPressedKeys();
            foreach(Keys key in lastKeysDown)
            {
                if (currentDownKeys.Contains(key))
                {
                    heldDownKeys.Add(key);
                }
            }
            lastKeysDown = currentDownKeys;
            return heldDownKeys;
        }
    }
}
