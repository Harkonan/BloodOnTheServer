using Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Core
{
    public class GameLogic
    {
        public Players Players { get; set; }
        public NightVisitLogic NightVisitLogic;
        public IEnumerable<XElement> Roles { get; set; }
        public int CurrentDay { get; set; }



        public GameLogic()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Core.Data.Roles.xml");
            Players = new Players();
            CurrentDay = 0;
            NightVisitLogic = new NightVisitLogic(this);
            

            Roles = XElement.Load(stream).Descendants("role");
        }


        /// <summary>
        /// Looks in the XML file to see if there is a Role that matches the string inputed.
        /// if it finds a match, it returns a Role Item with that matching roles data.
        /// </summary>
        /// <param name="Role"></param>
        /// <returns>Role</returns>
        public Role GetRole(string Role)
        {
            var RoleExists = Roles.Any(x => x.Element("name").Value.Trim().ToLower() == Role.Trim().ToLower());
            if (RoleExists) {

                //Select the role from the Roles XML data by mathcing its name
                var RoleData = Roles.Where(x => x.Element("name").Value.Trim().ToLower() == Role.Trim().ToLower()).First();

                //conver the role into a Role Object converting it to the correct format where needed
                return new Role {
                    Name = RoleData.Element("name").Value,
                    NightPriority = Convert.ToInt32(RoleData.Element("night_priority").Value),
                    FirstNightPriority = Convert.ToInt32(RoleData.Element("first_night_priority").Value),
                    Team = (Team)Enum.Parse(typeof(Team), RoleData.Element("team").Value),
                    Type = (RoleType)Enum.Parse(typeof(RoleType), RoleData.Element("type").Value),
                    RoleText = RoleData.Element("role_text").Value
                };
            }
            else
            {
                //if we cannot find a role, throw an error (this needs to be made user friendly)
                throw new FileNotFoundException("Cannot find Role: "+Role);
            }
        }

      
        public GameResult CheckForWin()
        {
            if (Players.PlayersList.Where(x => x.Role.Type == RoleType.Demon).Count() == 0)
            {
                return new GameResult
                {
                    GameWon = true,
                    WinText = "Demon has been killed",
                    WonBy = Team.Good
                };

            } else if (false) {
                //TODO: Mayor win clause
            } else if (Players.PlayersList.Where(x => x.IsAlive).Count() <= 2)
            {
                return new GameResult {
                    GameWon = true,
                    WinText = "Only 2 players left alive",
                    WonBy = Team.Evil
                };
            }

            return new GameResult { GameWon = false };
        }


    }
}
