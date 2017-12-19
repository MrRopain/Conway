using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Conway
{
    public class Grid
    {
        private static Grid _grid;
        
        public delegate void UpdateHandler();
        public event UpdateHandler OnUpdate;
        
        public Cell[,] Cells;
        
        public List<Cell> AliveCells;
        public List<Cell> CellsOfInterest;
        public List<Cell> MarkedCells;
        
        public Grid(int width, int height, int cellSize)
        {
            _grid = this;
            Width = width;
            Height = height;
            CellSize = cellSize;
            Setup();
        }
        
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int CellSize { get; private set; }

        public bool IsSetup { get; private set; }
        public bool IsRunning { get; set; }

        public int Iteration { get; private set; }
        public int AliveCellCount { get; private set; }
        
        private void Setup()
        {
            var maxX = Width / CellSize;
            var maxY = Height / CellSize;
            var centerX = maxX / 2;
            var centerY = maxY / 2;

            Cells = new Cell[maxX, maxY];
            for (var x = 0; x < maxX; x++)
            {
                for (var y = 0; y < maxY; y++)
                {
                    var cell = CreateCell(x, y);

                    // hard code ahead, fuuuckkk (maybe add functionality to import presets)
                    if (x == centerX && y == centerY)
                    {
                        cell.IsAlive = true;
                    }
                    if (x == centerX - 2 && y == centerY - 1)
                    {
                        cell.IsAlive = true;
                    }
                    if (y == centerY + 1)
                    {
                        if (x == centerX - 3 || x == centerX - 2 || x == centerX + 1 || x == centerX + 2 || x == centerX + 3)
                        {
                            cell.IsAlive = true;
                        }
                    }
                }
            }

            IsSetup = true;
        }

        public void Update()
        {
            PopulateAliveCellsIfNeeded();

            if (!IsRunning)
            {
                OnUpdate?.Invoke();
                return;
            }

            PopulateCellsOfInterest();
            MarkedCells = new List<Cell>();
            AliveCellCount = AliveCells.Count;

            foreach (var cell in CellsOfInterest)
            {
                cell.Update();
                if (cell.IsMarked)
                {
                    MarkedCells.Add(cell);
                }
            }

            foreach (var cell in MarkedCells)
            {
                if (cell.IsAliveNext)
                {
                    AliveCells.Add(cell);
                }
                else
                {
                    AliveCells.Remove(cell);
                }
                
                cell.UpdateLife();
            }

            Iteration++;
            OnUpdate?.Invoke();
        }

        private Cell CreateCell(int x, int y)
        {
            var cell = new Cell(x, y, CellSize);
            Cells[x, y] = cell;
            return cell;
        }

        private void PopulateAliveCellsIfNeeded()
        {
            if (AliveCells != null)
            {
                return;
            }
            
            AliveCells = new List<Cell>();
            foreach (var cell in Cells)
            {
                if (!cell.IsAlive)
                {
                    continue;
                }
                
                AliveCells.Add(cell);
            }
        }

        private void PopulateCellsOfInterest()
        {
            CellsOfInterest = new List<Cell>();
            foreach (var cell in AliveCells)
            {
                AddCellIfNeeded(CellsOfInterest, cell);
                AddCellsIfNeeded(CellsOfInterest, cell.GetNeighbours());
            }
        }
        
        private void AddCellIfNeeded(List<Cell> cells, Cell cell)
        {
            if (!cells.Contains(cell))
            {
                cells.Add(cell);
            }
        }

        private void AddCellsIfNeeded(List<Cell> cells, List<Cell> cellsToBeAdded)
        {
            foreach (var cell in cellsToBeAdded)
            {
                AddCellIfNeeded(cells, cell);
            }
        }

        public static void ToggleCell(Cell cell)
        {
            cell.IsAlive = !cell.IsAlive;
            if (cell.IsAlive)
            {
                _grid.AliveCells.Add(cell);
            }
            else
            {
                _grid.AliveCells.Remove(cell);
            }
        }

        public static Cell GetCellAtAbsolute(int x, int y)
        {
            foreach (var cell in _grid.Cells)
            {
                var minX = cell.AbsoluteX;
                var maxX = cell.AbsoluteX + _grid.CellSize;
                var minY = cell.AbsoluteY;
                var maxY = cell.AbsoluteY + _grid.CellSize;

                if (minX <= x && maxX >= x && minY <= y && maxY >= y)
                {
                    return cell;
                }
            }

            // todo return Cell.None
            return null;
        }

        public static List<Cell> GetCellsAround(int x, int y)
        {
            var maxX = _grid.Width / _grid.CellSize;
            var maxY = _grid.Height / _grid.CellSize;

            // use array? unsafe in this version.
            var cells = new List<Cell>();
            for (var relativeX = -1; relativeX <= 1; relativeX++)
            {
                for (var relativeY = -1; relativeY <= 1; relativeY++)
                {
                    if (relativeX == 0 && relativeY == 0)
                    {
                        continue;
                    }

                    var absoluteX = x + relativeX;
                    var absoluteY = y + relativeY;

                    if (absoluteX < 0 || absoluteX >= maxX || absoluteY < 0 || absoluteY >= maxY)
                    {
                        continue;
                    }

                    cells.Add(_grid.Cells[absoluteX, absoluteY]);
                }
            }

            return cells;
        }
    }
}