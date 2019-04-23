using System;
using System.IO;
using System.Reflection;
using System.Linq;

namespace BotS
{
    class Program
    {
        private static readonly string Resources = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\");
        private static readonly Core.GameLogic GameLogic = new Core.GameLogic(Resources);

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


            //List players & roles
            Console.WriteLine("Player & Role List:");
            foreach (var Player in GameLogic.Players.PlayersList)
            {
                Console.WriteLine(" ({0}) {1}", Player.Role.Name, Player.Name);
            }

            Console.WriteLine("");
            Console.WriteLine("Setting up before the first night:");
            
            //Get roles you need to set up before the first night & all roles that need visiting on the first night
            GameLogic.NightVisitLogic.AddFirstNightReminders();
            foreach (var Player in GameLogic.Players.GetPlayersWithNightVisitsToPreSetup().OrderBy(x => x.Role.FirstNightPriority))
            {
                Console.WriteLine(" ({0}) {1}: {2}", Player.Role.Name, Player.Name, Player.Role.RoleText);
            }


            Console.WriteLine("");
            Console.WriteLine("First night visits:");
            foreach (var Player in GameLogic.Players.GetPlayersWithNightVisits().OrderBy(x => x.Role.FirstNightPriority))
            {
                Console.WriteLine(" ({0}) {1}: {2}", Player.Role.Name, Player.Name, Player.Role.RoleText);
            }

            //Wipe reminders to start next night
            GameLogic.NightVisitLogic.ClearNightVisits();
            Console.WriteLine("");
            Console.WriteLine("Press any key when first night is complete");
            Console.ReadKey(false);
            Console.Clear();

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
                Console.WriteLine("Day {0}", DayNumber);

                Console.WriteLine("");
                Console.WriteLine("Tonights Setup");
                GameLogic.NightVisitLogic.AddNightReminders();
                foreach (var Player in GameLogic.Players.GetPlayersWithNightVisitsToPreSetup().OrderBy(x => x.Role.NightPriority))
                {
                    Console.WriteLine(" ({0}) {1}: {2}", Player.Role.Name, Player.Name, Player.Role.RoleText);
                }

                Console.WriteLine("");
                Console.WriteLine("Was a Player Executed Today?");

                Console.WriteLine("");
                Console.WriteLine("Tonights Visits");
                GameLogic.NightVisitLogic.AddNightReminders();
                foreach (var Player in GameLogic.Players.GetPlayersWithNightVisits().OrderBy(x => x.Role.NightPriority))
                {
                    Console.WriteLine(" ({0}) {1}: {2}", Player.Role.Name, Player.Name, Player.Role.RoleText);
                }

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
                


                Console.Clear();
            }            
        }

    }
}
