using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minotaur
{
    public enum Direction {North = -1,East = 1,South = 1,West = -1};
    
    public class PathFinder
    {
        private Dictionary<MazeTile,int> _Visited;
        public MazeTile CurrentTile;
        public MazeTile PreviousTile;
        private MazeTile _Exit;
        private bool IKnowExitCoordinates;
        public EventHandler Moved;

        public PathFinder(bool knowsExitCoordinates = true)
        {
            IKnowExitCoordinates = knowsExitCoordinates;
            _Visited = new Dictionary<MazeTile,int>();
        }

        public bool Solve(Maze theMaze)
        {
            InitVisitedList(theMaze);

            this.CurrentTile = theMaze.Start;
            this._Exit = IKnowExitCoordinates ? theMaze.Finish : null;
            this.Move(theMaze);

            return true;
        }

        private void InitVisitedList(Maze theMaze)
        {
            foreach (MazeTile mt in theMaze.Layout)
            {
                _Visited.Add(mt, 0);
            }
        }

        private void Move(Maze theMaze)
        {
            //From Current Location
            MazeTile Next = this.ChooseNextLocation(theMaze);

            if (!this.MoveToNextLocation(Next))
            {
                throw new Exception("No path exists between start and finish.");
            }

            if (Moved != null){ Moved(this, new EventArgs()); }
            if(!CurrentTile.Equals(theMaze.Finish)){ this.Move(theMaze); }
        }

        private bool MoveToNextLocation(MazeTile Next)
        {
            if (!Next.isValid) { return false; }
            PreviousTile = CurrentTile;
            CurrentTile = Next;
            _Visited[CurrentTile]++;
            return true;
        }

        /// <summary>
        /// Move to an ajacent tile following this priority:
        /// - You have no other option
        /// - You have visited it less than another option
        /// - It is closer to the exit than an equally visited option
        /// - Randomly
        /// </summary>
        /// <param name="theMaze"></param>
        /// <returns></returns>
        private MazeTile ChooseNextLocation(Maze theMaze)
        {
           var nextTile = new MazeTile();
           var choices =  this.LookAround(theMaze);
          
           choices.RemoveAll(c => c.isWall);

           //Stuck - return an invalid tile to kill the maze.
           if (choices.Count == 0) { return nextTile; }

           //Dead End - kill this tile
           if (choices.Count == 1) {
               CurrentTile.isWall = true;
               return choices[0]; 
           }

           FilterByLeastVisited(choices);
           FilterByLeastDistance(choices);
           nextTile = ChooseRandom(choices);

           return nextTile;
        }

        private void FilterByLeastVisited(List<MazeTile> choices)
        {
            var visitChoices = (from v in _Visited
                    where choices.Contains(v.Key)
                    select new { v.Key, v.Value }).ToDictionary(kv => kv.Key,kv=> kv.Value);

            var exclude = from vc in visitChoices
                          where vc.Value != visitChoices.Values.Min()
                          select vc;

            exclude.ToList().ForEach(c => choices.RemoveAll(m => m.getCoordinateKey() == c.Key.getCoordinateKey()));
        }

        private void FilterByLeastDistance(List<MazeTile> choices)
        {
            var MoveScore = CalculateDistanceIfExitIsKnown(choices);
            if (MoveScore != null)
            {
                var exclude = from e in MoveScore
                              where e.Value != MoveScore.Values.Min()
                              select e;

                exclude.ToList().ForEach(kv => choices.RemoveAll(m => m.getCoordinateKey() == kv.Key));
            }
        }

        /// <summary>
        /// Get the  ajacent tiles of the current location
        /// </summary>
        /// <param name="theMaze"></param>
        /// <returns>List of MazeTiles</returns>
        private List<MazeTile> LookAround(Maze theMaze)
        {
            var North = theMaze.GetTile(CurrentTile.X, CurrentTile.Y + (int)Direction.North);
            var South = theMaze.GetTile(CurrentTile.X, CurrentTile.Y + (int)Direction.South);
            var East = theMaze.GetTile(CurrentTile.X + (int)Direction.East, CurrentTile.Y);
            var West = theMaze.GetTile(CurrentTile.X + (int)Direction.West, CurrentTile.Y);

            return new List<MazeTile>() { North, South, East, West };
        }

        /// <summary>
        /// Calculate the distance (in moves) between a tile and the exit.
        /// Returns null if the exit is unknown. 
        /// </summary>
        /// <param name="choices"></param>
        /// <returns></returns>
        private Dictionary<string, int> CalculateDistanceIfExitIsKnown(List<MazeTile> choices)
        {
            if (!this.IKnowExitCoordinates) { return null; }
            var MoveScore = new Dictionary<string, int>();
            
            foreach (MazeTile choice in choices)
            {
                int score = Math.Abs(choice.Y - _Exit.Y) + Math.Abs(choice.X - _Exit.X);
                MoveScore.Add(choice.getCoordinateKey(), score);
            }

            return MoveScore;
        }

        private MazeTile ChooseRandom(List<MazeTile> choices)
        {
            if (choices.Count == 1) { return choices[0];}
            Random rand = new Random();
            return choices[rand.Next(0,choices.Count - 1)];
        }
    }
}
