using System;
using System.Collections.Generic;
using SFML.System;
using SFML.Graphics;
using SFML.Window;

namespace LilRogue
{
    public class Grid<T>
    {
        private struct CellData
        {
            public T Value;
            public Color BackgroundColor;
            public Color FillColor;
            public Color OutlineColor;
            public float OutlineThickness;
        }

        private CellData[,] gridArray;

        public int Width { get; }
        public int Height { get; }

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;
            gridArray = new CellData[width, height];
        }

        public void SetCell(int x, int y, T value, Color backgroundColor, Color fillColor, Color outlineColor, float outlineThickness)
        {
            if (IsValidPosition(x, y))
            {
                gridArray[x, y] = new CellData { Value = value, BackgroundColor = backgroundColor, FillColor = fillColor, OutlineColor = outlineColor, OutlineThickness = outlineThickness };
            }
            else
            {
                throw new ArgumentOutOfRangeException("Position is out of grid bounds.");
            }
        }

        public T GetCellValue(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return gridArray[x, y].Value;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Position is out of grid bounds.");
            }
        }

        public Color GetCellBackgroundColor(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return gridArray[x, y].BackgroundColor;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Position is out of grid bounds.");
            }
        }

        public Color GetCellFillColor(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return gridArray[x, y].FillColor;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Position is out of grid bounds.");
            }
        }

        public float GetCellOutlineThickness(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return gridArray[x, y].OutlineThickness;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Position is out of grid bounds.");
            }
        }

        public void WriteString(int x, int y, string text, Color backgroundColor, Color fillColor, Color outlineColor, float outlineThickness, Func<char, T> charToValue)
        {
            if (IsValidPosition(x, y))
            {
                int currentX = x;

                foreach (char c in text)
                {
                    if (IsValidPosition(currentX, y))
                    {
                        SetCell(currentX, y, charToValue(c), backgroundColor, fillColor, outlineColor, outlineThickness);
                        currentX++;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Text got out of bounds.");
                    }
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("Position is out of grid bounds.");
            }
        }

        public void Draw(RenderWindow window, Font font, Map map, int Xoffset, int Yoffset)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    T cellValue = GetCellValue(x, y);
                    if (!EqualityComparer<T>.Default.Equals(cellValue, default(T)))
                    {
                        Color fillColor = GetCellFillColor(x, y);
                        Color outlineColor = GetCellBackgroundColor(x, y);
                        float outlineThickness = GetCellOutlineThickness(x, y);


                        if (map != null)
                        {
                            if (!map.IsCellVisible(x, y))
                            {
                                fillColor = Color.Black;
                                outlineColor = Color.Black;
                                outlineThickness = 0;
                            }
                        }
                        var characterText = new Text(cellValue.ToString(), font, 16)
                        {
                            Position = new Vector2f((x * 16) + Xoffset * 16, (y * 16) + Yoffset * 16),
                            FillColor = fillColor,
                            OutlineColor = outlineColor,
                            OutlineThickness = outlineThickness,
                        };
                        //draw square behind text for background color
                        var square = new RectangleShape(new Vector2f(16, 16));
                        square.FillColor = outlineColor;
                        square.Position = new Vector2f((x * 16) + Xoffset * 16, (y * 16) + Yoffset * 16);
                        window.Draw(square);

                        window.Draw(characterText);
                    }
                }
            }
        }

        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
    }
}
