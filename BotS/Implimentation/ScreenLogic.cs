using BotS.Models;
using Core;
using Core.Models;
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
            Console.WriteLine("Players:");
            DrawPlayerList();

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

        public void DrawKillScreen(CauseOfDeath? _CauseOfDeath)
        {
            Console.Clear();
            CauseOfDeath CauseOfDeath;
            if (_CauseOfDeath == null)
            {
                Console.WriteLine("Select Cause of Death: ");
                var KillTypeMenu = Enum.GetValues(typeof(CauseOfDeath)).Cast<CauseOfDeath>().Select(x => new MenuItem { Display = x.ToString(), ReturnValue = x.ToString() });
                CauseOfDeath = (CauseOfDeath)Enum.Parse(typeof(CauseOfDeath), DrawMenu(KillTypeMenu));
            }
            else
            {
                CauseOfDeath = _CauseOfDeath ?? CauseOfDeath.Execution;
            }
            
            Console.WriteLine("");
            Console.WriteLine("Select which player you wish to kill by {0}", _CauseOfDeath.ToString());
            var KillablePlayers = Program.GameLogic.Players.PlayersList.Where(x => x.IsAlive).Select(x => new MenuItem()
            {
                Display = " (" + x.Role.Name + ") " + x.Name,
                ReturnValue = x.Id.ToString()
            });
            var retunredValue = DrawMenu(KillablePlayers);
            var KilledPlayer = Program.GameLogic.Players.PlayersList.Where(x => x.Id.ToString() == retunredValue).First();

            KilledPlayer.KillPlayer(CauseOfDeath);           

            Console.Clear();
        }

        public string DrawMenu(IEnumerable<MenuItem> InputValues)
        {
            var Values = InputValues.ToList();

            char PlayerInput;
            for (int i = 0; i < Values.Count(); i++)
            {
                Values[i].SeclectorValue = i + 1;
            }

            do
            {
                Console.Clear();
                Console.WriteLine("Select an Item Below");
                foreach (var item in Values)
                {
                    Console.WriteLine("{0} - {1}", item.SeclectorValue, item.Display);
                }
                PlayerInput = Console.ReadKey(true).KeyChar;

            } while (!Values.Any(x => x.SeclectorValue.ToString() == PlayerInput.ToString()));

            return Values.Where(x => x.SeclectorValue.ToString() == PlayerInput.ToString()).First().ReturnValue;
        }
    }
}
