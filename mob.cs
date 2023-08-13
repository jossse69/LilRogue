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
        public Grid<char> grid1 { get; set; }
        public Mob(Grid<char> grid, char symbol, Vector2i position, Color fillColor, Color backgroundColor, Color outlineColor, int maxHP = 100, float outlineThickness = 0)
            : base(grid, symbol, position, fillColor, backgroundColor, outlineColor, outlineThickness)
        {
            MaxHP = maxHP;
            HP = MaxHP;
            grid1 = grid;
        }

        public Vector2i GoToLocation(int x, int y, Map map)
        {
            if (map == null)
            {
                return new Vector2i(x, y);
            }
        
            PathFinder pathFinder = new PathFinder(map.getMap(),0);
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
                // set char to a bloody red "&"
                this.Symbol = '&';
                this.BackgroundColor = Color.Black;
                this.FillColor = Color.Red;

                return;
            }

            //update state
            if (CanbeSeen(map))
            {
                lastSeenPlayerPosition = new Vector2i(player.Position.X, player.Position.Y);
                state = "chase";
            }
            else if (lastSeenPlayerPosition.X != -1 && lastSeenPlayerPosition.Y != -1)
            {
                state = "wander";
            }
         
            if (state == "chase"){
                
                for (int i = 0; i < schedulingSystem.calculateTimeSteps(schedulingSystem.time, schedulingSystem.time + 1, 2); i++)
                {
                    
                    ToGoalPosition = GoToLocation(lastSeenPlayerPosition.X, lastSeenPlayerPosition.Y, map);
                    //check if the mob has reached the goal position
                    foreach (var mob in mobs)
                        if (ToGoalPosition.X == player.Position.X && ToGoalPosition.Y == player.Position.Y)
                        {
                            Console.WriteLine("player reached! damaging player...");
                            player.TakeDamage(4.25);
                            break;
                        }
                        else if (ToGoalPosition.X == mob.Position.X && ToGoalPosition.Y == mob.Position.Y) //blocked by another mob
                        {
                            Console.WriteLine("blocked by another mob...");
                            break;
                        }
                        else if (ToGoalPosition.X == mob.Position.X && ToGoalPosition.Y == mob.Position.Y)
                        {
                            Console.WriteLine("cant see player...");
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

                for (int i = 0; i < schedulingSystem.calculateTimeSteps(schedulingSystem.time, schedulingSystem.time + 1, 1); i++)
                {     
                    ToGoalPosition = GoToLocation(wanderpos.X, wanderpos.Y, map);

                      foreach (var mob in mobs)
                        if (ToGoalPosition.X == player.Position.X && ToGoalPosition.Y == player.Position.Y)
                        {
                            Console.WriteLine("blocked by player...");
                            break;
                        }
                        else if (ToGoalPosition.X == mob.Position.X && ToGoalPosition.Y == mob.Position.Y) //blocked by another mob
                        {
                            Console.WriteLine("blocked by another mob...");
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

    }
}
