using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballData.Importer.Interfaces
{
    public interface IPlayerParser
    {
        List<DTOs.Player> Parse(string text);
    }
}
