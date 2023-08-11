using SFML.System;
using SFML.Graphics;

namespace LilRogue
{
    public class Entity
    {
        public char Symbol { get; }
        public Vector2i Position { get; set; }
        public Color FillColor { get; }

        public Color BackgroundColor { get; }
        public Color OutlineColor { get; }
        public float OutlineThickness { get; }

        private readonly Grid<char> grid;
        public Entity(Grid<char> grid,char symbol, Vector2i position, Color fillColor, Color backgroundColor, Color outlineColor, float outlineThickness = 0)
        {
            this.grid = grid;
            Symbol = symbol;
            Position = position;
            FillColor = fillColor;
            BackgroundColor = backgroundColor;
            OutlineColor = outlineColor;
            OutlineThickness = outlineThickness;
        }

        public void draw()
        {
            grid.SetCell(Position.X, Position.Y, Symbol, FillColor, BackgroundColor, OutlineColor, OutlineThickness);
        }

    }
}

