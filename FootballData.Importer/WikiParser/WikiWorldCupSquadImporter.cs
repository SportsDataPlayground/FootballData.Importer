using FootballData.Importer.DTOs;
using FootballData.Importer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballData.Importer.WikiParser
{
    public class WikiWorldCupSquadImporter : IImporter
    {
        public List<Player> Import()
        {
            var allPlayers = new List<Player>();
            foreach (var wc in WorldCupData.WorldCups)
            {
                try
                {
                    if (wc.NumberOfTeams != 0)
                    {
                        var data = new WikiDownloader().DownloadWorldCupSquad(wc.Year);
                        var players = new WikiSquadParser(wc).Parse(data);
                        allPlayers.AddRange(players);
                    }

                    // throttle ourselves - be nice to wiki's api
                    System.Threading.Thread.Sleep(2 * 1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to parse " + wc.ToString());
                    Console.WriteLine(ex);
                }
            }
            return allPlayers;
        }

        public string Name()
        {
            return "world-cup-squads";
        }
    }
}
