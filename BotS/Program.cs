using System;
using System.IO;
using System.Reflection;
using System.Linq;
using BotS.Implimentation;

namespace BotS
{
    class Program
    {
        private static readonly string Resources = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\");
        public static readonly Core.GameLogic GameLogic = new Core.GameLogic();
        private static readonly ScreenLogic ScreenLogic = new ScreenLogic();

        static void Main(string[] args)
        {
            Gui.Initalise();


            /* DG - TEMPORARY COMMENTING OUT TO TEST GUI
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

            DayNightLoop();

            Console.WriteLine("");
            Console.WriteLine("Fin");
            Console.Read();
            */

        }

        

        private static void DayNightLoop()
        {
            bool looper = true;
            int DayNumber = 1;

            while (looper)
            {
                //Day Phase
                GameLogic.NightVisitLogic.AddNightVisits();
                ScreenLogic.DrawDayScreen(DayNumber);
                Console.WriteLine("");
                Console.WriteLine("Was a Player Executed Today? (Y/N)");
                if (Console.ReadKey(false).Key == ConsoleKey.Y)
                {
                    ScreenLogic.DrawKillScreen(CauseOfDeath.Execution);
                    GameLogic.NightVisitLogic.RefreshNightVisits();
                    ScreenLogic.DrawDayScreen(DayNumber);

                }
                else
                {
                    Console.Clear();
                    ScreenLogic.DrawDayScreen(DayNumber);
                }
                
                Console.WriteLine("Press any Key to move to night Phase");
                Console.ReadKey();
                Console.Clear();

                //Move into night phase
                ScreenLogic.DrawNightScreen();

                Console.WriteLine("");
                Console.WriteLine("Continue to Next Turn? (Y/N)");
                if (Console.ReadKey(false).Key == ConsoleKey.N)
                {
                    looper = false;
                }
                
                GameLogic.NightVisitLogic.ClearNightVisits();
                Console.Clear();
            }
        }

    }
}
