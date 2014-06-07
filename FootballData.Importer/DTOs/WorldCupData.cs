using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballData.Importer.DTOs
{
    public static class WorldCupData
    {
        /// <summary>
        /// Past and Current World Cups
        /// </summary>
        public static List<Competition> WorldCups
        {
            get
            {
                return TeamTotalsByYear.Select(x =>
                    new Competition { Name = "World Cup", Year = x.Key, NumberOfTeams = x.Value }).ToList();
            }
        }

        public static Dictionary<int, int> TeamTotalsByYear
        {
            get
            {
                return new Dictionary<int, int>() {
                    {1930, 13},
                    {1934, 16},
                    {1938, 15},
                    {1942, 0},
                    {1946, 0},
                    {1950, 13},
                    {1954, 16},
                    {1958, 16},
                    {1962, 16},
                    {1966, 16},
                    {1970, 16},
                    {1974, 16},
                    {1978, 16},
                    {1982, 24},
                    {1986, 24},
                    {1990, 24},
                    {1994, 24},
                    {1998, 32},
                    {2002, 32},
                    {2006, 32},
                    {2010, 32},
                    {2014, 32},
                };
            }
        }
    }
}
