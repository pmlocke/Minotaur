using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minotaur
{
    public class MazeTile
    {
        public bool isWall { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool isValid { 
            get { 
            if (this.X >= 0 && this.Y >= 0) 
            return true; 
            return false; 
             } 
        }

        public MazeTile(int x = -1, int y = -1, bool isWall = true)
        {
            X = x;
            Y = y;
            this.isWall = isWall;
        }

        public string getCoordinateKey()
        {
            return X.ToString() + Y.ToString();
        } 
    }
}