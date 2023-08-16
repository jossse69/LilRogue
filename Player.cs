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
        private Stopwatch deathwindowTimer = new Stopwatch();
        private const int MovementDelayMilliseconds = 100; // Adjust this value to control movement speed
        public int HP = 100;
        public int MaxHP = 100;
        public int mana = 100;
        public int MaxMana = 100;
        public int armor = 1;
        public int Gold = 0;
        public bool isDead = false;
        public bool showdeathwindow = false;
        private SchedulingSystem _schedulingSystem;
        private List<Mob> mobs = new List<Mob>();


        public static SoundBuffer DMGsoundBuffer = new SoundBuffer("hitHurt.wav");
        public Sound DMGsound = new Sound(DMGsoundBuffer);
        public static SoundBuffer attacksoundBuffer = new SoundBuffer("attack.wav");
        public Sound attacksound = new Sound(attacksoundBuffer);
        public Player(Grid<char> grid, Vector2i position, Color fillColor, Color backgroundColor, Color outlineColor, SchedulingSystem schedulingSystem, float outlineThickness = 0, Map map = null)
        : base(grid, '@', position, fillColor, backgroundColor, outlineColor, outlineThickness)
        {
            movementTimer.Start();
            _schedulingSystem = schedulingSystem;
            if (map != null)
            {
                map.ComputeFov(Position.X, Position.Y, 10, true);
            }
        }

        // Add player-specific logic here, such as handling input and movement
         public void HandleInput(Map map, List<Mob> mobs)
        {
            // Handle player input and update the player's position
            if (movementTimer.ElapsedMilliseconds >= MovementDelayMilliseconds)
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad8)) // Numpad 8 for moving up
                {
                    tryToMove(0, -1, map, mobs);
                    
                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad2)) // Numpad 2 for moving down
                {
                    tryToMove(0, 1, map, mobs);

                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad4)) // Numpad 4 for moving left
                {
                    tryToMove(-1, 0, map, mobs);
                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad6)) // Numpad 6 for moving right
                {
                    tryToMove(1, 0, map, mobs);

                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad7)) // Numpad 7 for moving up-left
                {
                    tryToMove(-1, -1, map, mobs);

                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad9)) // Numpad 9 for moving up-right
                {
                    tryToMove(1, -1, map, mobs);
                    
                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad1)) // Numpad 1 for moving down-left
                {
                    tryToMove(-1, 1, map, mobs);
                    
                }
                else if (Keyboard.IsKeyPressed(Keyboard.Key.Numpad3)) // Numpad 3 for moving down-right
                {
                    tryToMove(1, 1, map, mobs);
                    
                }

                // Reset the movement timer
                movementTimer.Restart();
                //update player
                Update();

            }
        }

        public void tryToMove(int x, int y, Map map, List<Mob> mobs)
        {
            //dont move if the player is dead
            if (isDead)
            {
                return;
            }

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
                         
                            mob.TakeDamage(15);
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

        public void TakeDamage(double damage)
        {
            var DMGmuti = Math.Clamp(RNG.NextDouble(), 0.8, 1);
            var finalDMG = (double) damage * (DMGmuti * (armor + 1 - 0.6));

            this.HP -=  (int) Math.Round(Math.Clamp(finalDMG * 0.75, 0, 999999), 1);

            DMGsound.Play();
        }

        public void Update()
        {
            //dead
            if (HP <= 0)
            {
                isDead = true;
                //wait a bit
                deathwindowTimer.Start();
                if (deathwindowTimer.ElapsedMilliseconds >= 1000)
                {
                    showdeathwindow = true;
                }
            }
        }

        public override void draw()
        {
            //if dead
            if (HP <= 0)
            {
                return;
            }

            grid.SetCell(Position.X, Position.Y, Symbol, FillColor, BackgroundColor, OutlineColor, OutlineThickness);
        }
    }
}
