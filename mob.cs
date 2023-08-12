using SFML.Graphics;
using SFML.System;
using RogueSharp;

namespace LilRogue
{
    public class Mob : Entity
    {
        public int HP { get; set; }
        public int MaxHP { get; set; }

        public Mob(Grid<char> grid, char symbol, Vector2i position, Color fillColor, Color backgroundColor, Color outlineColor, int maxHP = 100, float outlineThickness = 0)
            : base(grid, symbol, position, fillColor, backgroundColor, outlineColor, outlineThickness)
        {
            HP = MaxHP = maxHP; // Set initial HP values
        }

        // Inside the Mob class
        public void GoToLocation(int x, int y, Map map, SchedulingSystem schedulingSystem)
        {
            // Get the path from the mob's current position to the target location (x, y)
            PathFinder pathFinder = new PathFinder(map.getMap());
            RogueSharp.Path path = pathFinder.ShortestPath(map.GetCell(Position.X, Position.Y), map.GetCell(x, y));

            
            try
            {
                if (path != null)
                {
                    // Move the mob along the path
                    foreach (Cell cell in path.Steps)
                    {
                        // Move the mob to the next cell and use the SchedulingSystem to schedule an action to happen after 1 turn
                        schedulingSystem.Schedule(schedulingSystem.time + 1, () =>
                        {
                            Position = new Vector2i(cell.X, cell.Y);
                        });

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }
         public void Update(Map map, Player player, SchedulingSystem schedulingSystem)
         {
            // Implement AI logic for the TestMob's behavior
            // For example, call GoToLocation to move towards the player's position
            GoToLocation(player.Position.X, player.Position.Y, map, schedulingSystem);
         }
    }
}
