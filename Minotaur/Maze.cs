using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Minotaur
{
    public class Maze
    {
        public MazeTile Start { get; set; }
        public MazeTile Finish { get; set; }
        public List<MazeTile> Layout {get;set;}
       
        public MazeTile GetTile(int X, int Y)
        {
            MazeTile mt = new MazeTile();
            mt = Layout.Single(t => t.X == X && t.Y == Y);
            return mt;
        }

        public Maze(string mazeFilePath)
        {
            Layout = new List<MazeTile>();
            Start = new MazeTile();
            Finish = new MazeTile();

            ParseMazeFile(mazeFilePath);
            
            if (!this.Start.isValid || !this.Finish.isValid )
            {
                throw new Exception("Maze must contain a Start and Finish tile.");
            }
        }

        private void ParseMazeFile(string mazeFilePath)
        {
            StreamReader sr = new StreamReader(mazeFilePath);
            int x = 0;
            int y = 0;
            string line = string.Empty;

            while ((line = sr.ReadLine()) != null)
            {
                foreach (char c in line)
                {
                    Debug.Write(c);
                    MazeTile mt = new MazeTile(x, y, Maze.ParseTile("wall", c));
                    if (Maze.ParseTile("start", c)) { Start = mt; }
                    if (Maze.ParseTile("finish", c)) { Finish = mt; }
                    Layout.Add(mt);
                    x++;
                }
                y++;
                x = 0;
                Debug.Write("\n");
            }
        }

        public static bool ParseTile(string key, char tile)
        {
            Dictionary<string, char> legend = new Dictionary<string, char>();
            legend.Add("wall",'*');
            legend.Add("empty", ' ');
            legend.Add("start", 's');
            legend.Add("finish", 'f');

            if (!legend.ContainsValue(tile))
            {
                throw new Exception(tile + "is not a valid maze tile");
            }

            return legend[key] == tile;
        }

    }
}