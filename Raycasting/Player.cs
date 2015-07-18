using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raycasting
{
    // Class representing the player, used to calculate the view
    public class Player
    {
        // VARIABLES
        /////

        // Vectors representing various parts of the player and view
        public Vector2 position; // x and y coordinate of the player's position
        public Vector2 direction; // what direction the player is facing 
        public Vector2 cameraPlane; // 2d raycaster version of camera plane  

        // Constructor
        public Player()
        {
            position = new Vector2(22f, 11.5f);
            direction = new Vector2(-1f, 0f);
            cameraPlane = new Vector2(0f, 0.66f);
        }
    }
}
