using System;
using SFML.Graphics;
using SFML.Audio;
using SFML.System;
using SFML.Window;
using RogueSharp;
using RogueSharp.MapCreation;
using Newtonsoft.Json;

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

            var window = new RenderWindow(new VideoMode((uint)screenWidth, (uint)screenHeight, 32), "Lil Rogue");
            
            var RNG = new Random();
            //set limit FPS
            window.SetFramerateLimit(60);

            var gameMap = new Map(mapWidth, mapHeight);

            window.Closed += (s, e) => window.Close();
            var font = new Font("PublicPixel.ttf");

            var grid = new Grid<char> (mapWidth, mapHeight);

            SchedulingSystem schedulingSystem = new SchedulingSystem();

            //UI and ui grid
            var UIgrid = new Grid<char> (UIWidth, UIHeight);
            var popupwindowgrid = new Grid<char> (mapWidth, mapHeight);
            var ui = new UI(UIgrid, UIWidth, UIHeight);
            var popupui = new UI(popupwindowgrid, UIWidth, UIHeight);
            var upStairsPosition = FindWalkableCell(gameMap);
            var downStairsPosition = FindWalkableCell(gameMap);

             //mob array
            var Mobs = new List<Mob>();

            string monstersjson = File.ReadAllText("data/monsters.json");
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ColorConverter());
            var monsters = JsonConvert.DeserializeObject<List<Mob>>(monstersjson, settings);
            
            for (int i = 0; i < 5; i++)
            {
                //get a random monster
                var monster = monsters[RNG.Next(0, monsters.Count)];
                var pos = FindWalkableCell(gameMap);
                var color = monster.color;
                var BackgroundColor = monster.BackgroundColor;
            
                Mob mob = new Mob(grid, monster.Symbol, pos, ConvertColor(BackgroundColor), ConvertColor(color), Color.Black, monster.HP, 0, monster.Type, monster.speed, monster.attack, monster.armor, schedulingSystem, gameMap);
                Mobs.Add(mob);
            }
            
            
            




            var upStairs = new Entity(grid, '<', new Vector2i(upStairsPosition.X, upStairsPosition.Y), Color.Black, Color.Yellow, Color.White, 1, gameMap);
            var downStairs = new Entity(grid, '>', new Vector2i(downStairsPosition.X, downStairsPosition.Y), Color.Black, Color.Yellow, Color.White, 1, gameMap);

            var player = new Player(grid,  new Vector2i(upStairsPosition.X, upStairsPosition.Y), Color.Black, Color.Green, Color.White, schedulingSystem, 1, gameMap);

           
            // Schedule an action to happen after 12 turns
            schedulingSystem.Schedule(12, () =>
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
                foreach (var mob in Mobs)
                {
                    mob.draw();
                }
                player.draw();
                // Render UI
                ui.DrawBorderedBox(0, 0, UIWidth, UIHeight, Color.Black, Color.White);

                // Draw HP, Mana and armor values
                UIgrid.WriteString(1, 1, "HP: " + player.HP + "/" + player.MaxHP, Color.Black, Color.Green, Color.Black, 1, c =>  c);
                UIgrid.WriteString(1, 3, "Mana: " + player.mana + "/" + player.MaxMana, Color.Black, new Color(153, 204, 255, 255), Color.Black, 1, c =>  c);
                UIgrid.WriteString(1, 5, "Armor: " + player.armor, Color.Black, new Color(255, 153, 0, 255), Color.Black, 1, c =>  c);

                // draw gold value
                UIgrid.WriteString(15, 1, "Gold: " + player.Gold, Color.Black, Color.Yellow, Color.Black, 1, c =>  c);

                //if player showdeathwindow is true, draw death window
                if (player.showdeathwindow  == true)
                {
                    popupui.DrawBorderedBox(0, 0, 40, 10, Color.Black, Color.White);
                    popupwindowgrid.WriteString(18, 0, ":(", Color.Black, Color.Red, Color.Black, 1, c => c);
                    popupwindowgrid.WriteString(3, 1, "well... looks like you died", Color.Black, Color.White, Color.Black, 1, c => c);
                    popupwindowgrid.WriteString(14, 9, "[press enter]", Color.Black, Color.White, Color.Black, 1, c => c);

                    //on enter, close game
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Enter))
                    {
                        window.Close();
                        break;
                    }
                }

                // Render the grid
                grid.Draw(window, font, gameMap, 0, 0);
                UIgrid.Draw(window, font, null, 0, 30);
                popupwindowgrid.Draw(window, font, null, 5, 15);


                
                player.HandleInput(gameMap, Mobs); // Handle player input and movement

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
            static Color ConvertColor(int[] ints)
            {
                return new Color((byte)ints[0], (byte)ints[1], (byte)ints[2]);
            }
        }
    }
}
