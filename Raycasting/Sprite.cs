using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raycasting
{
    public class Sprite
    {
        // x, y position
        public Vector2 position;

        // what texture this sprite uses
        public int texture;


        // constructor
        public Sprite()
        {
        }

        public Sprite(Vector2 pos, int texNum = 0)
        {
            position = pos;
            texture = texNum;
        }
    }
}
