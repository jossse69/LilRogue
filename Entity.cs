using SFML.System;
using SFML.Graphics;

namespace LilRogue
{
    public class Entity
    {
        public char Symbol { get; set; }
        public Vector2i Position { get; set; }
        public Color FillColor { get; set; }

        public Color BackgroundColor { get; set; }
        public Color OutlineColor { get; set; }
        public float OutlineThickness { get; set; }
        
        public Grid<char> grid;
        private SchedulingSystem schedulingSystem;
        public Entity(Grid<char> grid, char symbol, Vector2i position, Color fillColor, Color backgroundColor, Color outlineColor, float outlineThickness = 0, Map? map = null)
        {
            this.grid = grid;
            Symbol = symbol;
            Position = position;
            FillColor = fillColor;
            BackgroundColor = backgroundColor;
            OutlineColor = outlineColor;
            OutlineThickness = outlineThickness;
        }

        public virtual void draw()
        {
            grid.SetCell(Position.X, Position.Y, Symbol, FillColor, BackgroundColor, OutlineColor, OutlineThickness);
        }

    }
}

