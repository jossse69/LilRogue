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
                state = "chase";
            }
            else
            {
                state = "wander";
            }
         
            if (state == "chase"){
                
                for (int i = 0; i < schedulingSystem.calculateTimeSteps(schedulingSystem.time, schedulingSystem.time + 4, 1); i++)
                {
                    ToGoalPosition = GoToLocation(player.Position.X, player.Position.Y, map);
                    //check if the mob has reached the goal position
                    foreach (var mob in mobs)
                        if (ToGoalPosition.X == player.Position.X && ToGoalPosition.Y == player.Position.Y)
                        {
                            Console.WriteLine("Goal reached! damaging player...");
                            player.TakeDamage(5);
                        }
                        else if (ToGoalPosition.X == mob.Position.X && ToGoalPosition.Y == mob.Position.Y) //blocked by another mob
                        {
                            Console.WriteLine("blocked by another mob...");;
                        }
                        else
                        {
                            Position = new Vector2i(ToGoalPosition.X, ToGoalPosition.Y);
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
                        }
                        else if (ToGoalPosition.X == mob.Position.X && ToGoalPosition.Y == mob.Position.Y) //blocked by another mob
                        {
                            Console.WriteLine("blocked by another mob...");
                        }
                        else
                        {
                            Position = new Vector2i(ToGoalPosition.X, ToGoalPosition.Y);
                        }
                }
            }
         }


         public void TakeDamage(int damage)
        {
            var DMGmuti = Math.Clamp(RNG.NextSingle(), 0.7, 1.3);
            var finalDMG = (int)(damage * (DMGmuti / (armor + 1)));

            HP -=  Math.Clamp(finalDMG, 0, 999999);

            Console.WriteLine(finalDMG);
            Console.WriteLine(HP);
        }

    }
}
