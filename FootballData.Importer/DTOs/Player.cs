using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballData.Importer.DTOs
{
    public class Player
    {
        public string Competition { get; set; }
        public int Year { get; set; }

        public string Team { get; set; }
        public string Number { get; set; }
        public string Position { get; set; }
        public string FullName { get; set; }
        public string Club { get; set; }
        public string ClubCountry { get; set; }
        public string DateOfBirth { get; set; }
        public bool IsCaptain { get; set; }

        public override string ToString()
        {
            return string.Format("{6} {3} {0} club={1} clubnat={5}; dob={2} Pos={4}", 
                FullName, Club, DateOfBirth, Number, Position, ClubCountry, Team);
        }
        public bool IsValid()
        {
            return
                !string.IsNullOrWhiteSpace(Team) &&
                //!string.IsNullOrWhiteSpace(Number) &&
                !string.IsNullOrWhiteSpace(Position) &&
                !string.IsNullOrWhiteSpace(Club) &&
                !string.IsNullOrWhiteSpace(DateOfBirth) &&
                ((Club == "Free agent" && ClubCountry == "") || !string.IsNullOrWhiteSpace(ClubCountry)) ;
        }
    }
}
