using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;
using RogueSharp;
using System.Diagnostics;

namespace LilRogue
{
    public class Player : Entity
    {
        public Random RNG = new Random();

        private Stopwatch movementTimer = new Stopwatch();
        private const int MovementDelayMilliseconds = 100; // Adjust this value to control movement speed
        public int HP = 100;
        public int MaxHP = 100;
        public int mana = 100;
        public int MaxMana = 100;
        public int armor = 0;
        public int Gold = 0;
        private SchedulingSystem _schedulingSystem;
        private List<Mob> mobs = new List<Mob>();

        public static SoundBuffer DMGsoundBuffer = new SoundBuffer("hitHurt.wav");
        public Sound DMGsound = new Sound(DMGsoundBuffer);
        public static SoundBuffer attacksoundBuffer = new SoundBuffer("attack.wav");
        public Sound attacksound = new Sound(attacksoundBuffer);
        public Player(Grid<char> grid, Vector2i position, Color fillColor, Color backgroundColor, Color outlineColor, SchedulingSystem schedulingSystem, float outlineThickness = 0, Map map = null, List<Mob> mobs = null)
        : base(grid, '@', position, fillColor, backgroundColor, outlineColor, outlineThickness)
        {
            movementTimer.Start();
            _schedulingSystem = schedulingSystem;
            this.mobs = mobs;
            if (map != null)
            {
                map.ComputeFov(Position.X, Position.Y, 10, true);
            }
        }

        // Add player-specific logic here, such as handling input and movement
         public void HandleInput(Map map)
        {
            // Handle player input and update the player's position
            if (movementTimer.ElapsedMilliseconds >= MovementDelayMilliseconds)
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad8)) // Numpad 8 for moving up
                {
                    tryToMove(0, -1, map);
                    
                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad2)) // Numpad 2 for moving down
                {
                    tryToMove(0, 1, map);

                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad4)) // Numpad 4 for moving left
                {
                    tryToMove(-1, 0, map);
                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad6)) // Numpad 6 for moving right
                {
                    tryToMove(1, 0, map);

                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad7)) // Numpad 7 for moving up-left
                {
                    tryToMove(-1, -1, map);

                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad9)) // Numpad 9 for moving up-right
                {
                    tryToMove(1, -1, map);
                    
                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad1)) // Numpad 1 for moving down-left
                {
                    tryToMove(-1, 1, map);
                    
                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad3)) // Numpad 3 for moving down-right
                {
                    tryToMove(1, 1, map);
                    
                }

                // Reset the movement timer
                movementTimer.Restart();

            }
        }

        public void tryToMove(int x, int y, Map map)
        {
            int newX = Position.X + x;
            int newY = Position.Y + y;

            // Check if the new position is within map bounds
            if (map.IsWalkable(newX, newY))
            {
                // if its gona run into a mob, attack it instead
                foreach (Mob mob in mobs)
                {
                    if (mob.Position.X == newX && mob.Position.Y == newY)
                    {
                        if (mob.HP >= 0)
                        {
                         
                            mob.TakeDamage(10);
                            // play damage sound using SFML
                            attacksound.Play();
                            // Update the scheduling system's time
                            _schedulingSystem.Update(_schedulingSystem.time + 6);

                            // Update the mobs
                            UpdateMobs(mobs, map);    
                            return;
                        }
                        else
                        {
                            Console.WriteLine("already dead, moving on...");
                        }   
                    }
                }

                // Update visibility of cells based on player's field of view
                map.ComputeFov(newX, newY, 10, true);

                // Move the player if walkable
                Position = new Vector2i(newX, newY);

                // Update the scheduling system's time
                _schedulingSystem.Update(_schedulingSystem.time + 6);

                // Update the mobs
                UpdateMobs(mobs, map);                

            }
        }

        public void UpdateMobs(List<Mob> mobs, Map map)
        {
            if (mobs == null)
            {
                // Handle the null case
                return;
            }

            foreach (Mob mob in mobs)
            {
                if (mob != null)
                {
                    mob.Update(map, this, _schedulingSystem, mobs);
                }
            }
        }

        public void TakeDamage(int damage)
        {
            var DMGmuti = Math.Clamp(RNG.NextSingle(), 0.7, 1.3);
            var finalDMG = (int)(damage * (DMGmuti / (armor + 1)));

            HP -=  Math.Clamp(finalDMG, 0, 999999);

            DMGsound.Play();

            Console.WriteLine(finalDMG);
            Console.WriteLine(HP);
        }

    }
}
