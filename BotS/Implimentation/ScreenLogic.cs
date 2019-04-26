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
            DrawTitle("Player & Role List:");
            DrawPlayerList();


            //Get roles you need to set up before the first night & all roles that need visiting on the first night
            var FirstNightSetup = Program.GameLogic.Players.GetPlayersWithNightVisitsToPreSetup().OrderBy(x => x.Role.FirstNightPriority);
            if (FirstNightSetup.Count() > 0)
            {
                DrawTitle("Setting up before the first night");

                foreach (var Player in FirstNightSetup)
                {
                    DrawPlayerDescription(Player);
                }
            }


            var FirstNightVisits = Program.GameLogic.Players.GetPlayersWithNightVisits().OrderBy(x => x.Role.FirstNightPriority);
            if (FirstNightVisits.Count() > 0)
            {
                DrawTitle("First night visits");
                foreach (var Player in FirstNightVisits)
                {
                    DrawPlayerDescription(Player);
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Press Space when first night is complete");
            do{} while (Console.ReadKey(true).Key != ConsoleKey.Spacebar);
            

            Console.Clear();
        }

        public void DrawPlayerList()
        {
            //List players & roles
            foreach (var Player in Program.GameLogic.Players.PlayersList)
            {
                string DeathStatus = "";
                if (!Player.IsAlive)
                {
                    DeathStatus = "[D]";
                }
                SetColorByPlayer(Player);
                Console.WriteLine(" - ({0}) {1} {2}", Player.Name, Player.Role.Name, DeathStatus);
                Console.ResetColor();
            }
        }

        internal void SetColorByPlayer(Player Player)
        {
            if (!Player.IsAlive)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                if (Player.Role.Team == Team.Good)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                else if (Player.Role.Team == Team.Evil)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
            }
        }

        public void ErrorScreen(string Title, string Message)
        {
            Console.Clear();
            DrawTitle(Title);
            Console.WriteLine(Message);
            Console.WriteLine("");
            Console.WriteLine("Press Space Key to quit");
            do { } while (Console.ReadKey(true).Key != ConsoleKey.Spacebar);
            Environment.Exit(0);
        }


        public void DrawDayScreen()
        {
            Console.WriteLine("Day time on Day {0}", Program.GameLogic.CurrentDay);

            DrawTitle("Players");
            DrawPlayerList();

            var NightSetupVisits = Program.GameLogic.Players.GetPlayersWithNightVisitsToPreSetup().OrderBy(x => x.Role.NightPriority);

            if (NightSetupVisits.Count() > 0)
            {
                DrawTitle("Tonights Setup");
                foreach (var Player in NightSetupVisits)
                {
                    DrawPlayerDescription(Player);
                }
            }
        }

        public void DrawNightScreen()
        {
            Console.WriteLine("Nightime on Day {0} ", Program.GameLogic.CurrentDay);

            DrawTitle("Tonights Visits");
            Program.GameLogic.NightVisitLogic.AddNightVisits();
            foreach (var Player in Program.GameLogic.Players.GetPlayersWithNightVisits().OrderBy(x => x.Role.NightPriority))
            {
                DrawPlayerDescription(Player);
            }

        }

        public void DrawRoleChange(Role role)
        {
            

            Console.Clear();
            DrawTitle("Select a player to become the " + role.Name);

        }

        public void DrawKillScreen(CauseOfDeath? causeOfDeath = null)
        {
            Console.Clear();
            CauseOfDeath CauseOfDeath;
            if (causeOfDeath == null)
            {
                DrawTitle("Select Cause of Death: ");
                var KillTypeMenu = Enum.GetValues(typeof(CauseOfDeath)).Cast<CauseOfDeath>().Where(x =>x != CauseOfDeath.NotDead).Select(x => new MenuItem { Display = x.ToString(), ReturnValue = x.ToString() });
                CauseOfDeath = (CauseOfDeath)Enum.Parse(typeof(CauseOfDeath), DrawMenu(KillTypeMenu));
            }
            else
            {
                CauseOfDeath = causeOfDeath ?? CauseOfDeath.Execution;
            }

            
            DrawTitle("Select which player you wish to kill by "+ causeOfDeath.ToString());
            var KillablePlayers = Program.GameLogic.Players.PlayersList.Where(x => x.IsAlive).Select(x => new MenuItem()
            {
                Display = " (" + x.Role.Name + ") " + x.Name,
                ReturnValue = x.Id.ToString()
            });
            var retunredValue = DrawMenu(KillablePlayers);
            var KilledPlayer = Program.GameLogic.Players.PlayersList.Where(x => x.Id.ToString() == retunredValue).First();

            //Select a minion to become the Imp if it suicides
            
            if (KilledPlayer.Role.Name == "Imp" && CauseOfDeath == CauseOfDeath.Demon
                && Program.GameLogic.Players.PlayersList.Any(x => x.IsAlive && x.Role.Type == RoleType.Minion))
            {
                if (Program.GameLogic.Players.PlayersList.Where(x => x.IsAlive && x.Role.Type == RoleType.Minion).Count() > 1)
                {
                    DrawTitle("Select a Minion to become the Imp");
                    var NewImpId = DrawMenu(Program.GameLogic.Players.PlayersList.Where(x => x.IsAlive && x.Role.Type == RoleType.Minion)
                            .Select(x => new MenuItem
                            {
                                Display = " (" + x.Role.Name + ") " + x.Name,
                                ReturnValue = x.Id.ToString()
                            }));
                    Player NewImpPlayer = Program.GameLogic.Players.PlayersList.Where(x => x.Id.ToString() == NewImpId).First();

                    NewImpPlayer.ChangeRole(KilledPlayer.Role);
                }
                else
                {
                    Program.GameLogic.Players.PlayersList.Where(x => x.IsAlive && x.Role.Type == RoleType.Minion).First().ChangeRole(KilledPlayer.Role);
                }
                
                
                
            }

            Program.GameLogic.KillPlayer(KilledPlayer, CauseOfDeath);
            Console.Clear();
        }

        internal void DrawTitle(string title)
        {
            Console.WriteLine("");
            Console.WriteLine(title);
            string Underline = "";
            foreach (char Char in title)
            {
                Underline += "#";
            }
            Console.WriteLine(Underline);
        }

        internal void DrawPlayerDescription(Player player)
        {
            SetColorByPlayer(player);

            Console.WriteLine("");
            Console.Write(" ({0}) {1}", player.Role.Name, player.Name);
            Console.ResetColor();
            Console.Write(": {0}", player.Role.RoleText);
            Console.ResetColor();
            Console.WriteLine("");
        }

        public string DrawMenu(IEnumerable<MenuItem> InputValues)
        {
            var Values = InputValues.ToList();

            string PlayerInput;
            for (int i = 0; i < Values.Count(); i++)
            {
                Values[i].SeclectorValue = i + 1;
            }

            do
            {
                Console.Clear();
                DrawTitle("Select an Item Below");
                foreach (var item in Values)
                {
                    Console.WriteLine("{0} - {1}", item.SeclectorValue, item.Display);
                }
                PlayerInput = Console.ReadLine();

            } while (!Values.Any(x => x.SeclectorValue.ToString() == PlayerInput.ToString()));

            return Values.Where(x => x.SeclectorValue.ToString() == PlayerInput.ToString()).First().ReturnValue;
        }
    }
}
