using FootballData.Importer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballData.Importer.Interfaces
{
    public interface IImporter
    {
        List<Player> Import();

        string Name();
    }
}
