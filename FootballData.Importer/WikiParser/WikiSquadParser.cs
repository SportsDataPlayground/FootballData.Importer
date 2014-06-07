using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FootballData.Importer.DTOs;
using FootballData.Importer.Interfaces;
using NLog;

namespace FootballData.Importer.WikiParser
{
    /// <summary>
    /// {{nat fs player|no=1|pos=GK|name=[[Moeneeb Josephs]]|age={{Birth date and age2|2010|6|11|1980|5|19|df=y}}|caps=17|club=[[Orlando Pirates FC|Orlando Pirates]]|clubnat=RSA}}
    /// </summary>
    public class WikiSquadParser : IPlayerParser
    {
        Logger logger = LogManager.GetLogger("WikiSquadParser");
        private Competition _competition;

        public WikiSquadParser(Competition competition)
        {
            _competition = competition;
        }

        public List<Player> Parse(string text)
        {
            text = new string(text.Where(c => !char.IsControl(c)).ToArray());
            text = NormalizeTemplateNames(text);

            var teams = Regex.Matches(text, "==([^=]+)==")
                .Cast<Match>()
                .Where(x => !string.IsNullOrWhiteSpace(x.Value.Replace("=", ""))).ToList();

            var coaches = Regex.Matches(text, "Coach:", RegexOptions.IgnoreCase).Cast<Match>().ToList();

            var start = "{{nat fs start}}";
            var end = "{{nat fs end}}";

            var actualTeams = new List<string>();
            var allPlayers = new List<Player>();
            foreach (Match m in teams)
            {
                var startIndex = text.IndexOf(start, m.Index);

                if (startIndex > 0)
                {
                    var endIndex = text.IndexOf(end, startIndex);
                    var innerData = text
                        .Substring(startIndex, endIndex - startIndex)
                        .Replace(start, "").Replace(end, "");

                    var teamName = GetTeamName(m.Value);
                    actualTeams.Add(teamName);
                    var players = Parse(innerData, teamName);
                    allPlayers.AddRange(players);
                }
                else
                {
                    logger.Info("Skipping match " + m.Value);
                }
            }

            var invalids = allPlayers.Where(x => !x.IsValid()).ToList();
            //invalids.ForEach(logger.Info);

            logger.Info("");
            logger.Info(" -- " + _competition.ToString());

            if (_competition.NumberOfTeams != actualTeams.Count)
                logger.Info("**********MISMATCH for " + _competition.ToString());

            logger.Info(string.Join(",", actualTeams));
            logger.Info("{0} teams and {1} coaches", actualTeams.Count, coaches.Count);
            logger.Info("{0} players imported vs {1} estimated with {2} invalids and {3} captains.",
                allPlayers.Count, actualTeams.Count * 23, invalids.Count, allPlayers.Count(x => x.IsCaptain));

            allPlayers.ForEach(p => { p.Year = _competition.Year; p.Competition = "World Cup"; });
            return allPlayers;
        }

        public List<Player> Parse(string data, string team)
        {
            var list = new List<Player>();

            var players = data.Split(new[] { "{{nat fs player" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in players)
            {
                var player = new Player
                {
                    FullName = GetName(p),
                    Number = GetNumber(p),
                    Position = GetPosition(p),
                    DateOfBirth = GetBirthdate(p),
                    Club = GetClub(p),
                    ClubCountry = GetClubNationality(p),
                    Team = team,
                    IsCaptain = GetIsCaptain(p),
                };
                list.Add(player);
            }


            return list;
        }

        // age={{Birth date and age2|2010|6|11|1977|11|25|df=y}}
        // {{Birth date and age2|specified year|specified month|specified day|year of birth|month of birth|day of birth}}
        // {{Birth date and age2|yyyy|mm|dd|yyyy|mm|dd|df=y}}
        public string GetBirthdate(string s)
        {
            var pattern = @"\|\d{4}\|\d+\|\d+\|\d{4}\|\d+\|\d+";
            var match = Regex.Match(s, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var x = match.Value.Replace("df=y", "").Replace("df=yes", "").Replace("2010", "");
                var matches = Regex.Matches(x, @"\d{4}\|\d+\|\d+").Cast<Match>().ToList();
                if (matches.Count > 0)
                    return matches.Last().Value.Replace("|", "-");
            }

            return string.Empty;
        }

        public string GetTeamName(string s)
        {
            var names = s.Split(new[] { "|", "{{", "}}", "[", "]", "==", "flagicon" },
                StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .Where(x => !string.IsNullOrWhiteSpace(x) && x != "fb")
                .ToList();

            if (names.Any()) return names.First();

            return string.Empty;
        }

        public bool GetIsCaptain(string s)
        {
            var match = Regex.Match(s, @"\[\[Captain", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                // not vice captain
                if (!Regex.IsMatch(s, @"\|vc\]\]"))
                    return true;
            }
            return false;
        }

        public string GetNumber(string s)
        {
            var match = Regex.Match(s, @"\|no=[ \d]+\|", RegexOptions.IgnoreCase);
            if (match.Success)
                return Regex.Match(match.Value, @"\d+").Value;

            return string.Empty;
        }

        public string GetClubNationality(string s)
        {
            var match = Regex.Match(s, @"\|clubnat=[A-Za-z ]+", RegexOptions.IgnoreCase);
            if (match.Success)
                return match.Value.Replace("|clubnat=", "").Replace("|", "").Replace("}}", "");

            return string.Empty;
        }

        // club=[[Orlando Pirates FC|Orlando Pirates]]|clubnat=RSA
        public string GetClub(string s)
        {
            try
            {
                if (s.Contains("Free agent"))
                    return "Free agent";

                var tag = "|club=";
                var index = s.IndexOf(tag);

                if (index < 0)
                    return string.Empty;

                index += tag.Length;
                var end = s.IndexOf("]]", index);

                var diff = 2;
                if (end < 0)
                {
                    end = s.IndexOf("|", index);
                    diff = 0;

                    if (end < 0)
                    {
                        end = s.IndexOf("}}", index);
                        diff = 2;
                    }
                }

                var team = s.Substring(index, end - index - diff);
                team = team.Replace("[[", "").Replace("=", "");
                var names = team.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (names.Any())
                    return names.First();

                return string.Empty;
            }
            catch (Exception e)
            {
                logger.Info(s);
            }
            return "";
        }

        public string GetPosition(string s)
        {
            var match = Regex.Match(s, @"\|pos=[A-Z ]+\|", RegexOptions.IgnoreCase);
            if (match.Success)
                return match.Value.Replace("|pos=", "").Replace("|", "");

            return string.Empty;
        }

        // |name=[[Moeneeb Josephs]]|
        // |name=[[Aaron Mokoena]] ([[Captain (association football)|c]])|
        // |name=[[Matthew Booth (soccer)|Matthew Booth]]|
        public string GetName(string s)
        {
            var match = Regex.Match(s, @"\|name=\[\[[\w -\]]+\|", RegexOptions.IgnoreCase);
            if (match.Success)
                return match.Value.Replace("|name=", "").Replace("[", "").Replace("]", "")
                    .Replace("|", "")
                    .Replace("(soccer)", "")
                    .Replace("(Captain (association football)", "").Trim();

            return string.Empty;
        }

        public string NormalizeTemplateNames(string data)
        {
            var newData = data.Replace("{{National football squad", "{{nat fs");
            newData = newData.Replace("==Reserves in stan by==", "");
            newData = newData.Replace("==References==", "");
            newData = newData.Replace("==Notes==", "");
            newData = Regex.Replace(newData, "==External links==", "", RegexOptions.IgnoreCase);
            newData = Regex.Replace(newData, "Group [1-9]", "", RegexOptions.IgnoreCase);
            newData = Regex.Replace(newData, "Group [a-z]", "", RegexOptions.IgnoreCase);
            return newData;
        }
    }
}
