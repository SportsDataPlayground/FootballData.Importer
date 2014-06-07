using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballData.Importer.DTOs
{
    public class Competition
    {
        public string Name { get; set; }
        public int Year { get; set; }
        public int NumberOfTeams { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Year, Name);
        }
    }
}
