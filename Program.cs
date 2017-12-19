using System;
using System.Threading;
using System.Windows.Forms;

namespace Conway
{
    public static class Program
    {
        private static Grid _grid;
        private static Display _display;

        public static void Main(string[] args)
        {
            const int zoom = 1;
            const int width = 1920 * zoom;
            const int height = 1080 * zoom;
            const int cellSize = 1 * zoom;
            _grid = new Grid(width, height, cellSize);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            _display = new Display(_grid);
            Application.Run(_display);
        }
    }
}