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

        static void Main()
        {
            Console.Clear();
            ScreenLogic.DrawTitle("Starting Demo Game");


            //load the demo game values from the csv then create players & roles based on this file
            using (var Reader = new StreamReader(Resources + @"\DemoGame.csv"))
            {

                try
                {
                    while (!Reader.EndOfStream)
                    {
                        var line = Reader.ReadLine();
                        var values = line.Split(',');

                        GameLogic.Players.PlayersList.Add(new Player
                        {
                            Name = values[0],
                            Role = GameLogic.GetRole(values[1])
                        });
                    }

                    if (GameLogic.Players.PlayersList.Count() < 5)
                    {
                        throw new PlayerCountException("Only " + GameLogic.Players.PlayersList.Count() + " players loaded, you need atleast 5 to play");
                    }
                    if (GameLogic.Players.PlayersList.Count() > 20)
                    {
                        throw new PlayerCountException("You have " + GameLogic.Players.PlayersList.Count() + " players loaded, The maximum players is 20");
                    }
                }
                catch (Exception ex)
                {
                    if (ex is RoleNotFoundError || ex is PlayerCountException)
                    {
                        ScreenLogic.ErrorScreen("Error Loading CSV", ex.Message);
                    }
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

                ConsoleKey Key;
                do
                {
                    Key = Console.ReadKey(true).Key;
                } while (Key != ConsoleKey.Y && Key != ConsoleKey.N);

                if (Key == ConsoleKey.Y)
                {
                    ScreenLogic.DrawKillScreen(CauseOfDeath.Execution);
                    GameLogic.NightVisitLogic.RefreshNightVisits();
                }

                Console.Clear();
                ScreenLogic.DrawDayScreen();

                Console.WriteLine("");
                Console.WriteLine("Press Space to continue to Night Phase");
                do { } while (Console.ReadKey(true).Key != ConsoleKey.Spacebar);

                //Move into night phase
                Console.Clear();
                ScreenLogic.DrawNightScreen();


                Console.WriteLine("");
                Console.WriteLine("Has a player been killed? (Y/N)");

                do
                {
                    Key = Console.ReadKey(true).Key;
                } while (Key != ConsoleKey.Y && Key != ConsoleKey.N);

                if (Key == ConsoleKey.Y)
                {
                    ScreenLogic.DrawKillScreen();
                    GameLogic.NightVisitLogic.RefreshNightVisits();
                    ScreenLogic.DrawNightScreen();

                    while (Key == ConsoleKey.Y)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Has another player been killed? (Y/N)");
                        
                        do
                        {
                            Key = Console.ReadKey(true).Key;
                        } while (Key != ConsoleKey.Y && Key != ConsoleKey.N);

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
                Console.WriteLine("Press Space to continue to next day (or press 'Q' to end the game)");
                
                do
                {
                    Key = Console.ReadKey(true).Key;
                } while (Key != ConsoleKey.Spacebar && Key != ConsoleKey.Q);

                if (Key == ConsoleKey.Q)
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
