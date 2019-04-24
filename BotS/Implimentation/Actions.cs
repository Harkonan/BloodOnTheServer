using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BotS.Implimentation
{
    static class Actions
    {

        public static void KillPlayer()
        {
            Console.Clear();

            String Name;
            foreach (var Player in Program.GameLogic.Players.PlayersList)
            {
                Console.WriteLine("({0}) {1}", Player.Role.Name, Player.Name);
            }

            do
            {
                Console.WriteLine("Type the name of the character killed");
                Name = Console.ReadLine();

            } while (Program.GameLogic.Players.PlayersList.Any(x => x.Name == Name));
        }
    }
}
