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
        internal static readonly Core.GameLogic GameLogic = new Core.GameLogic();
        internal static readonly ScreenLogic ScreenLogic = new ScreenLogic();

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
            GameLogic.Players.PlayersList.Where(x => x.Name == "Ex").First().KillPlayer(Core.Models.CauseOfDeath.Demon);

            GameLogic.NightVisitLogic.AddFirstNightVisits();
            ScreenLogic.DrawFirstNightScreen();
            //Wipe reminders to start next night
            GameLogic.NightVisitLogic.ClearNightVisits();

            DayNightLoop();

            Console.WriteLine("");
            Console.WriteLine("Fin");
            Console.Read();

        }

        

        private static void DayNightLoop()
        {
            bool looper = true;
            int DayNumber = 1;

            while (looper)
            {
                GameLogic.NightVisitLogic.AddNightVisits();
                ScreenLogic.DrawDayScreen(DayNumber);
                Console.WriteLine("");
                Console.WriteLine("Was a Player Executed Today? (Y/N)");
                switch (Console.ReadKey(false).Key)
                {
                    case ConsoleKey.Y:
                        ScreenLogic.DrawKillScreen(CauseOfDeath.Execution);
                        GameLogic.NightVisitLogic.RefreshNightVisits();
                        break;
                    default:
                        break;
                }

                Console.Clear();

                ScreenLogic.DrawNightScreen();
                Console.WriteLine("");
                Console.WriteLine("Continue to Next Turn? (Y/N)");
                switch (Console.ReadKey(false).Key)
                {
                    case ConsoleKey.N:
                        looper = false;
                        break;
                    default:
                        break;
                }
                GameLogic.NightVisitLogic.ClearNightVisits();
                Console.Clear();
            }
        }

    }
}
