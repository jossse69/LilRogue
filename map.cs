using System;
using SFML.System;
using RogueSharp;
using RogueSharp.MapCreation;

namespace LilRogue
{
    public class Map
    {
        public IMap _rogueSharpMap;
        private bool[,] _isCellVisible; // Add the visibility array

        public int Width => _rogueSharpMap.Width;
        public int Height => _rogueSharpMap.Height;

        public Map(int width, int height)
        {
            var mapCreationStrategy = new RandomRoomsMapCreationStrategy<RogueSharp.Map>(width, height, 100, 7, 3);
            _rogueSharpMap = RogueSharp.Map.Create(mapCreationStrategy);
            _isCellVisible = new bool[width, height]; // Initialize the visibility array
        }

        public bool IsWalkable(int x, int y)
        {
            return _rogueSharpMap.GetCell(x, y).IsWalkable;
        }

        public ICell GetCell(int x, int y)
        {
            return _rogueSharpMap.GetCell(x, y);
        }

        public ICell GetRandomCell()
        {
            var cell = _rogueSharpMap.GetCell(Random.Shared.Next(Width), Random.Shared.Next(Height));
            if (cell != null)
            {
                return cell;
            }
            return null;
        }

        public void ComputeFov(int x, int y, int radius, bool lightWalls)
        {
            var fov = new FieldOfView(_rogueSharpMap);
            fov.ComputeFov(x, y, radius, lightWalls);

            // clean up visibility map cells
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    _isCellVisible[i, j] = false;
                }
            }

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    int mapX = x + dx;
                    int mapY = y + dy;

                    if (IsValidPosition(mapX, mapY))
                    {
                        // Update
                        _isCellVisible[mapX, mapY] = fov.IsInFov(mapX, mapY); // Update visibility array
                    }
                }
            }
        }

        public bool IsTransparent(int x, int y)
        {
            return _rogueSharpMap.GetCell(x, y).IsTransparent;
        }

        public bool IsCellVisible(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return _isCellVisible[x, y];
            }
            return false;
        }


        private bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public IMap getMap()
        {
            return _rogueSharpMap;
        }
    }
}
