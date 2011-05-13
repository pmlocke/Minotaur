using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Minotaur
{
    public class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            Maze maze = new Maze(di.Parent.Parent.FullName + @"\trickMaze.maz");
            PathFinder pf = new PathFinder();
            pf.Moved += DrawOnMove;
            pf.Solve(maze);
            
        }

        public static void DrawOnMove(Object sender, EventArgs e)
        {
            PathFinder pf = sender as PathFinder;
            
        }

      
    }
}
