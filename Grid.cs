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

        public int Width { get; set; }
        public int Height { get; set; }

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;
            gridArray = new CellData[width, height];
        }

        public void SetCell(int x, int y, T value, Color backgroundColor, Color fillColor, Color outlineColor, float outlineThickness)
        {
        
                gridArray[x, y] = new CellData { Value = value, BackgroundColor = backgroundColor, FillColor = fillColor, OutlineColor = outlineColor, OutlineThickness = outlineThickness };
           
        }

        public T GetCellValue(int x, int y)
        {
            
                return gridArray[x, y].Value;

        }

        public Color GetCellBackgroundColor(int x, int y)
        {

                return gridArray[x, y].BackgroundColor;

        }

        public Color GetCellFillColor(int x, int y)
        {

                return gridArray[x, y].FillColor;

        }

        public float GetCellOutlineThickness(int x, int y)
        {
    
                return gridArray[x, y].OutlineThickness;
           
        }

        public void WriteString(int x, int y, string text, Color backgroundColor, Color fillColor, Color outlineColor, float outlineThickness, Func<char, T> charToValue)
        {
            
                int currentX = x;

                foreach (char c in text)
                {
                
                        SetCell(currentX, y, charToValue(c), backgroundColor, fillColor, outlineColor, outlineThickness);
                        currentX++;
                    
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
    }
}

