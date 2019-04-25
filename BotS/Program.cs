using System;
using System.IO;
using System.Reflection;
using System.Linq;
using BotS.Implimentation;
using Core.Models;

namespace BotS
{
    class Program
    {
        private static readonly string Resources = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\");
        public static readonly Core.GameLogic GameLogic = new Core.GameLogic();
        private static readonly ScreenLogic ScreenLogic = new ScreenLogic();

        static void Main(string[] args)
        {
            
            Console.WriteLine("Starting Demo Game");
            Console.WriteLine("");
            

            //load the demo game values from the csv then create players & roles based on this file
            using (var Reader = new StreamReader(Resources + @"\DemoGame.csv"))
            {
                while (!Reader.EndOfStream)
                {
                    var line = Reader.ReadLine();
                    var values = line.Split(',');

                    GameLogic.Players.PlayersList.Add(new Core.Models.Player
                    {
                        Name = values[0],
                        Role = GameLogic.GetRole(values[1])
                    });

                }
            }

            GameLogic.NightVisitLogic.AddFirstNightVisits();
            ScreenLogic.DrawFirstNightScreen();
            //Wipe reminders to start next night
            GameLogic.NightVisitLogic.ClearNightVisits();

            GameLogic.CurrentDay++;

            DayNightLoop();

            Console.WriteLine("");
            Console.WriteLine("Fin");
            Console.Read();
            

        }

        private static void DayNightLoop()
        {
            bool looper = true;

            while (looper)
            {
                //Day Phase
                GameLogic.NightVisitLogic.RefreshNightVisits();
                ScreenLogic.DrawDayScreen();
                Console.WriteLine("");
                Console.WriteLine("Was a Player Executed Today? (Y/N)");
                if (Console.ReadKey(false).Key == ConsoleKey.Y)
                {
                    ScreenLogic.DrawKillScreen(CauseOfDeath.Execution);
                    GameLogic.NightVisitLogic.RefreshNightVisits();
                }
               
                Console.Clear();
                ScreenLogic.DrawDayScreen();

                Console.WriteLine("");
                Console.WriteLine("Press Any key to continue to Night Phase");
                Console.ReadKey(false);


                //Move into night phase
                Console.Clear();
                ScreenLogic.DrawNightScreen();


                Console.WriteLine("");
                Console.WriteLine("Has a player been killed? (Y/N)");
                var Key = Console.ReadKey(false).Key;
                if (Key == ConsoleKey.Y)
                {
                    ScreenLogic.DrawKillScreen();
                    GameLogic.NightVisitLogic.RefreshNightVisits();
                    ScreenLogic.DrawNightScreen();

                    while (Key == ConsoleKey.Y)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Has another player been killed? (Y/N)");
                        Key = Console.ReadKey(false).Key;
                        if (Key == ConsoleKey.Y)
                        {
                            ScreenLogic.DrawKillScreen();
                            GameLogic.NightVisitLogic.RefreshNightVisits();
                            ScreenLogic.DrawNightScreen();
                        }
                    }
                }
               
                Console.Clear();
                ScreenLogic.DrawNightScreen();
                Console.WriteLine("");
                Console.WriteLine("Press any key to continue to next day (or press 'Q' to end the game)");
                if (Console.ReadKey(false).Key == ConsoleKey.Q)
                {
                    looper = false;
                }
                
                GameLogic.NightVisitLogic.ClearNightVisits();
                GameLogic.CurrentDay++;
                Console.Clear();
            }
        }

    }
}
