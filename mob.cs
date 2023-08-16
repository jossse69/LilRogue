using SFML.Graphics;
using SFML.System;
using RogueSharp;

namespace LilRogue
{
    public class Mob : Entity
    {
        public Random RNG = new Random();
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int armor { get; set; }
        public int speed { get; set; }

        public int attack { get; set; }

        public char Symbol { get; set; }
        public int[] color { get; set; }
        public Color TrueColor { get; set; }
        public int[] BackgroundColor { get; set; }
        public Color TrueBackgroundColor { get; set; }
        public string Type { get; set; }
        public Grid<char> grid1 { get; set; }
        public Mob(Grid<char> grid, char symbol, Vector2i position, Color fillColor, Color backgroundColor, Color outlineColor, int maxHP = 100, float outlineThickness = 0, string type = "null", int Speed = 1, int Attack = 0, int Armor = 0, SchedulingSystem schedulingSystem = null, Map map = null)
            : base(grid, symbol, position, fillColor, backgroundColor, outlineColor, outlineThickness)
        {
            MaxHP = maxHP;
            HP = MaxHP;
            grid1 = grid;
            armor = Armor;
            speed = Speed;
            attack = Attack;
            Type = type;
            Symbol = symbol;
            color = new int[] { fillColor.R, fillColor.G, fillColor.B };
            TrueColor = new Color((byte)color[0], (byte)color[1], (byte)color[2]);
            BackgroundColor = new int[] { backgroundColor.R, backgroundColor.G, backgroundColor.B };
            TrueBackgroundColor = new Color((byte)BackgroundColor[0], (byte)BackgroundColor[1], (byte)BackgroundColor[2]);
        }

        public Vector2i GoToLocation(int x, int y, Map map)
        {
            if (map == null)
            {
                return new Vector2i(x, y);
            }
        
            PathFinder pathFinder = new PathFinder(map.getMap(),1);
            RogueSharp.Path path = pathFinder.ShortestPath(map.GetCell(Position.X, Position.Y), map.GetCell(x, y));
        
            if (path != null)
            {
                var cell = path.TryStepForward();
                if (cell != null)
                {
                    return new Vector2i(cell.X, cell.Y);
                }            
            }
        
            // Add a return statement here
            return new Vector2i(x, y);
        }


        public bool CanbeSeen(Map map)
        {
            return map.IsCellVisible(Position.X, Position.Y);
        }
        
        

        
         public void Update(Map map, Player player, SchedulingSystem schedulingSystem, List<Mob> mobs)
         {
            var state = "wander";
            var ToGoalPosition = new Vector2i(0,0);
            var lastSeenPlayerPosition = new Vector2i(-1, -1);
            if (HP <= 0 || HP > MaxHP)
            {

                return;
            }

            //update state
            if (CanbeSeen(map))
            {
                
                state = "chase";
            }
            else if (lastSeenPlayerPosition.X != -1 && lastSeenPlayerPosition.Y != -1)
            {
                state = "wander";
            }
         
            if (state == "chase"){
                
                for (int i = 0; i < schedulingSystem.calculateTimeSteps(schedulingSystem.time, schedulingSystem.time + 6, speed * 2); i++)
                {
                    if (CanbeSeen(map))
                    {
                        lastSeenPlayerPosition = new Vector2i(player.Position.X, player.Position.Y);
                    }
                    
                    ToGoalPosition = GoToLocation(lastSeenPlayerPosition.X, lastSeenPlayerPosition.Y, map);
                    //check if the mob has reached the goal position
                    foreach (var mob in mobs)
                        if (ToGoalPosition.X == player.Position.X && ToGoalPosition.Y == player.Position.Y)
                        {
                            player.TakeDamage(attack);
                            break;
                        }
                        else if (ToGoalPosition.X == mob.Position.X && ToGoalPosition.Y == mob.Position.Y) //blocked by another mob
                        {
                            
                            break;
                        }
                        else if (ToGoalPosition.X == mob.Position.X && ToGoalPosition.Y == mob.Position.Y)
                        {
                            
                            lastSeenPlayerPosition = new Vector2i(-1, -1);
                            break;
                        }
                        else
                        {
                            Position = new Vector2i(ToGoalPosition.X, ToGoalPosition.Y);
                            break;
                        }
                    }
                }
            else if (state == "wander"){
                var wanderpos = map.GetRandomCell();
                while (wanderpos.IsWalkable == false){
                    wanderpos = map.GetRandomCell();
                }

                for (int i = 0; i < schedulingSystem.calculateTimeSteps(schedulingSystem.time, schedulingSystem.time + 6, speed); i++)
                {     
                    ToGoalPosition = GoToLocation(wanderpos.X, wanderpos.Y, map);

                      foreach (var mob in mobs)
                        if (ToGoalPosition.X == player.Position.X && ToGoalPosition.Y == player.Position.Y)
                        {
                            
                            break;
                        }
                        else if (ToGoalPosition.X == mob.Position.X && ToGoalPosition.Y == mob.Position.Y) //blocked by another mob
                        {
                            
                            break;
                        }
                        else
                        {
                            Position = new Vector2i(ToGoalPosition.X, ToGoalPosition.Y);
                            break;
                        }
                }
            }
         }


         public void TakeDamage(double damage)
        {
            var DMGmuti = Math.Clamp(RNG.NextDouble(), 0.8, 1);
            var finalDMG = (double) damage * (DMGmuti * (armor + 1 - 0.6));

            this.HP -=  (int) Math.Round(Math.Clamp(finalDMG * 1.25, 0, 999999), 1);

        }
        public override void draw()
        {
            //if dead
            if (HP <= 0)
            {
                return;
            }

            grid1.SetCell(Position.X, Position.Y, Symbol, TrueColor, TrueBackgroundColor, Color.Black, 1);
        }

    }
}
