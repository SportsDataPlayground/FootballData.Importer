using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net;
using FootballData.Importer.DTOs;
using FootballData.Importer.WikiParser;
using System.Reflection;
using FootballData.Importer.Interfaces;
using NLog;

namespace FootballData.Importer
{
    class Program
    {
        static Logger logger = LogManager.GetLogger("Program");

        static string OutputFullPath()
        {
            var bin = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var datetimeFolder = DateTime.Now.ToString("yyyy-mm-dd hh-mm-ss");
            var fullPath = Path.Combine(bin, datetimeFolder);
            var dir = new DirectoryInfo(fullPath);

            if (!dir.Exists)
            {
                logger.Info("Creating dir " + dir.FullName);
                dir.Create();
            }

            return dir.FullName;
        }

        static void Main(string[] args)
        {
            var importers = new List<IImporter> { new WikiWorldCupSquadImporter() };

            var path = OutputFullPath();

            foreach (var i in importers)
            {
                try
                {
                    var players = i.Import();
                    var json = JsonConvert.SerializeObject(players, Formatting.Indented);

                    File.WriteAllText(Path.Combine(OutputFullPath(), i.Name() + ".json"), json);
                }
                catch(Exception e)
                {
                    logger.Error("failed to import", e);
                }
            }


            Console.WriteLine("Program Done !! Press any key to close");
            Console.Read();
        }
    }
}
