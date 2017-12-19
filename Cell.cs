using System;
using System.Collections.Generic;
using System.Linq;

namespace Conway
{
    public class Cell
    {
        public readonly int GridX;
        public readonly int GridY;
        public readonly int AbsoluteX;
        public readonly int AbsoluteY;
        
        public Cell(int x, int y, int cellSize)
        {
            GridX = x;
            GridY = y;
            AbsoluteX = x * cellSize;
            AbsoluteY = y * cellSize;
        }
        
        public bool IsMarked { get; private set; }
        public bool IsAliveNext { get; private set; }
        public bool IsAlive { get; set; }
        public bool WasVisited { get; private set; }
        public DateTime LastVisited { get; private set; }

        public List<Cell> Neighbours;

        public void Update()
        {
            if (Neighbours == null)
            {
                Neighbours = Grid.GetCellsAround(GridX, GridY);
            }
            
            var neighboursAlive = Neighbours.Count(cell => cell.IsAlive);
            
            if (!IsAlive)
            {
                IsAliveNext = false;

                if (neighboursAlive == 3)
                {
                    IsAliveNext = true;
                }
            }
            else
            {
                IsAliveNext = true;

                if (neighboursAlive > 3)
                {
                    IsAliveNext = false;
                }
                if (neighboursAlive < 2)
                {
                    IsAliveNext = false;
                }
            }

            IsMarked = IsAlive != IsAliveNext;
        }
        
        public void UpdateLife()
        {
            WasVisited = true;
            LastVisited = DateTime.Now;
            IsAlive = IsAliveNext;
        }

        public List<Cell> GetNeighbours()
        {
            return Neighbours ?? (Neighbours = Grid.GetCellsAround(GridX, GridY));
        }
    }
}