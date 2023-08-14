using SFML.Graphics;
using SFML.System;

namespace LilRogue
{
    public class UI : Grid<char>
    {
        private Grid<char> grid;

        public UI(Grid<char> grid, int width, int height)
        : base(width, height)
        {
            this.grid = grid;
            this.grid.Width = width;
            this.grid.Height = height;
        }

        public void DrawBorderedBox(int x, int y, int width, int height, Color borderColor, Color fillColor)
        {
            // Draw top border
            grid.SetCell(x, y, '/', borderColor, fillColor, Color.Black, 1);
            for (int i = 1; i < width - 1; i++)
            {
                grid.SetCell(x + i, y, '-', borderColor, fillColor, Color.Black, 1);
            }
            grid.SetCell(x + width - 1, y, '\\', borderColor, fillColor, Color.Black, 1);

            // Draw middle rows
            for (int j = 1; j < height - 1; j++)
            {
                grid.SetCell(x, y + j, '|', borderColor, fillColor, Color.Black, 1);
                for (int i = 1; i < width - 1; i++)
                {
                    grid.SetCell(x + i, y + j, ' ', borderColor, fillColor, Color.Black, 1);
                }
                grid.SetCell(x + width - 1, y + j, '|', borderColor, fillColor, Color.Black, 1);
            }

            // Draw bottom border
            grid.SetCell(x, y + height - 1, '\\', borderColor, fillColor, Color.Black, 1);
            for (int i = 1; i < width - 1; i++)
            {
                grid.SetCell(x + i, y + height - 1, '-', borderColor, fillColor, Color.Black, 1);
            }
            grid.SetCell(x + width - 1, y + height - 1, '/', borderColor, fillColor, Color.Black, 1);
        }
    }
}
