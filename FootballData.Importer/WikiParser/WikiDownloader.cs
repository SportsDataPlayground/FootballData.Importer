using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FootballData.Importer.WikiParser
{
    public class WikiDownloader
    {
        Logger logger = LogManager.GetLogger("WikiDownloader");

        public string Api = "http://en.wikipedia.org/w/api.php";

        public string QueryFormat = "?format=txt&action=query&titles={0}&prop=revisions&rvprop=content";
        public string SquadTitleFormat = "{0} FIFA World Cup squads";

        public string DownloadWorldCupSquad(int year)
        {
            using (var wc = new WebClient())
            {
                var title = string.Format("{0} FIFA World Cup squads", year);
                var url = Api + string.Format(QueryFormat, title);

                logger.Info("Downloading {0}", url);
                return wc.DownloadString(url);
            }
        }
    }
}
