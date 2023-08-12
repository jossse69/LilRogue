using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using RogueSharp;
using RogueSharp.MapCreation;

namespace LilRogue
{
    class Program
    {
        static void Main(string[] args)
        {
            const int screenWidth = 800;
            const int screenHeight = 600;
            const int mapWidth = 50;
            const int mapHeight = 30;
            const int UIWidth = 50;
            const int UIHeight = 7;

            var window = new RenderWindow(new VideoMode((uint)screenWidth, (uint)screenHeight), "Lil Rogue");
            
            //set limit FPS
            window.SetFramerateLimit(60);

            var gameMap = new Map(mapWidth, mapHeight);

            window.Closed += (s, e) => window.Close();
            var font = new Font("PublicPixel.ttf");


            var grid = new Grid<char> (mapWidth, mapHeight);

            SchedulingSystem schedulingSystem = new SchedulingSystem();

            //UI and ui grid
            var UIgrid = new Grid<char> (UIWidth, UIHeight);
            var ui = new UI(UIgrid);

            var upStairsPosition = FindWalkableCell(gameMap);
            var downStairsPosition = FindWalkableCell(gameMap);

            var upStairs = new Entity(grid, '<', new Vector2i(upStairsPosition.X, upStairsPosition.Y), Color.Black, Color.Yellow, Color.White, 1, gameMap);
            var downStairs = new Entity(grid, '>', new Vector2i(downStairsPosition.X, downStairsPosition.Y), Color.Black, Color.Yellow, Color.White, 1, gameMap);

            var player = new Player(grid,  new Vector2i(upStairsPosition.X, upStairsPosition.Y), Color.Black, Color.Green, Color.White, schedulingSystem, 1, gameMap);

            var testMob = new Mob(grid, '!', new Vector2i(downStairsPosition.X, downStairsPosition.Y), Color.Black, Color.Red, Color.Black, 1, 1);

            // Schedule an action to happen after 12 turns
            schedulingSystem.Schedule(schedulingSystem.time + 12, () =>
            {
                Console.WriteLine("Scheduled action executed!");
            });
                
            while (window.IsOpen)
            {
                window.DispatchEvents();
                
                // Clear the window
                window.Clear();

                //map render
                for (int x = 0; x < mapWidth; x++)
                {
                    for (int y = 0; y < mapHeight; y++)
                    {
                        if (gameMap.IsWalkable(x, y))
                        {
                            grid.SetCell(x, y, '.', Color.Black, Color.White, Color.Black, 1);
                        }
                        else
                        {
                            grid.SetCell(x, y, '#', Color.Black, Color.White, Color.Black, 1);
                        }
                    }
                }

                // Render entities
                upStairs.draw();
                downStairs.draw();
                player.draw();
                testMob.draw();
                // Render UI
                ui.DrawBorderedBox(0, 0, UIWidth, UIHeight, Color.Black, Color.White);

                // Draw HP, Mana and armor values
                UIgrid.WriteString(1, 1, "HP: " + player.HP + "/" + player.MaxHP, Color.Black, Color.Green, Color.Black, 1, c =>  c);
                UIgrid.WriteString(1, 3, "Mana: " + player.mana + "/" + player.MaxMana, Color.Black, new Color(153, 204, 255, 255), Color.Black, 1, c =>  c);
                UIgrid.WriteString(1, 5, "Armor: " + player.armor, Color.Black, new Color(255, 153, 0, 255), Color.Black, 1, c =>  c);
                // Render the grid
                grid.Draw(window, font, gameMap, 0, 0);
                UIgrid.Draw(window, font, null, 0, 30);


                testMob.Update(gameMap, player, schedulingSystem);
                player.HandleInput(gameMap); // Handle player input and movement

                window.Display();
            }

            static Vector2i FindWalkableCell(Map gameMap)
            {
                var random = new Random();
                while (true)
                {
                    var randomCell = gameMap.GetRandomCell();
                    if (gameMap.IsWalkable(randomCell.X, randomCell.Y))
                    {
                        return new Vector2i(randomCell.X, randomCell.Y);
                    }
                }
            }
        }
    }
}
