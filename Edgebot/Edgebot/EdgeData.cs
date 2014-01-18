using System;
using System.Collections.Generic;

namespace Edgebot
{
    public class EdgeData
    {
        public const String JoinMessage = "{0}, Welcome to OTEGamers IRC: RR1 and RR2 Version: {1} || Unleashed Version: {2}";
        public static readonly List<string> Developers = new List<string> { "Helkarakse", "Citidel", "Citidel_","djdarkstorm"};

        public const String Host = "irc.esper.net:5555";
        public const String Nickname = "EdgeSharp";
        public const String Username = "EdgeSharp";
        public const String Channel = "#otegamers";

        public const String UrlTps = "http://dev.otegamers.com/api/v1/edge/tps";
        public const String UrlFish = "http://api.fishbans.com/stats/";
        public const String UrlFishLink = "http://www.fishbans.com/u/";
        public const String UrlVersion = "http://dev.otegamers.com/api/v1/edge/version";

        //Wiki Pages OTE wiki - Ote<PageName>
        //Wiki pages Other Wiki - Wiki<Mod>
        public const String UrlOteOre = "http://wiki.otegamers.com/view/Ore_Heightmap";
        public const String UrlOtePower = "http://wiki.otegamers.com/view/Power_Conversion";
        public const String UrlOteMyTown = "http://wiki.otegamers.com/view/MyTown";
        public const String UrlOteVoting = "http://wiki.otegamers.com/view/Voting";
        public const String UrlOteBugs = "http://wiki.otegamers.com/view/Bug_Reporting";
        public const String UrlOteRules = "http://wiki.otegamers.com/view/Server_Rules";
    }
}
