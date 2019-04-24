using BotS.Models;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BotS.Implimentation
{
    class ScreenLogic
    {
        public void DrawFirstNightScreen()
        {

            DrawPlayerList();
            Console.WriteLine("");
            Console.WriteLine("Setting up before the first night:");

            //Get roles you need to set up before the first night & all roles that need visiting on the first night
            
            foreach (var Player in Program.GameLogic.Players.GetPlayersWithNightVisitsToPreSetup().OrderBy(x => x.Role.FirstNightPriority))
            {
                Console.WriteLine(" ({0}) {1}: {2}", Player.Role.Name, Player.Name, Player.Role.RoleText);
            }


            Console.WriteLine("");
            Console.WriteLine("First night visits:");
            foreach (var Player in Program.GameLogic.Players.GetPlayersWithNightVisits().OrderBy(x => x.Role.FirstNightPriority))
            {
                Console.WriteLine(" ({0}) {1}: {2}", Player.Role.Name, Player.Name, Player.Role.RoleText);
            }

            Console.WriteLine("");
            Console.WriteLine("Press any key when first night is complete");
            Console.ReadKey(false); 
            
            Console.Clear();
        }

        public void DrawPlayerList()
        {
            //List players & roles
            Console.WriteLine("Player & Role List:");
            foreach (var Player in Program.GameLogic.Players.PlayersList)
            {
                string VoteStatus = "";
                if (!Player.IsAlive)
                {
                    VoteStatus = (Player.HasVote ? "[Has Vote]" : "");
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                
                Console.WriteLine(" - ({0}) {1} {2}", Player.Name, Player.Role.Name, VoteStatus);
                Console.ResetColor();
            }
        }



        public void DrawDayScreen(int _DayNumber)
        {
            Console.WriteLine("Day {0}", _DayNumber);

            Console.WriteLine("");
            Console.WriteLine("Tonights Setup");
            
            foreach (var Player in Program.GameLogic.Players.GetPlayersWithNightVisitsToPreSetup().OrderBy(x => x.Role.NightPriority))
            {
                Console.WriteLine(" ({0}) {1}: {2}", Player.Role.Name, Player.Name, Player.Role.RoleText);
            }

            
        }

        public void DrawNightScreen()
        {
            Console.WriteLine("");
            Console.WriteLine("Tonights Visits");
            Program.GameLogic.NightVisitLogic.AddNightVisits();
            foreach (var Player in Program.GameLogic.Players.GetPlayersWithNightVisits().OrderBy(x => x.Role.NightPriority))
            {
                Console.WriteLine(" ({0}) {1}: {2}", Player.Role.Name, Player.Name, Player.Role.RoleText);
            }

        }

        public string DrawMenu(IEnumerable<MenuItem> InputValues)
        {
            int i = 1;
            InputValues = InputValues.Select(x => { x.SeclectorValue = i++; return x; });

            Console.WriteLine("Select an Item Below");
            foreach (var item in InputValues)
            {
                Console.WriteLine("{0} - {1}", item.SeclectorValue, item.Display);
            }
            var PlayerInput = Console.ReadKey(false).KeyChar;

            return "";
        }
    }
}
