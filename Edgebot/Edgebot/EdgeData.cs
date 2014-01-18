using System;
using System.Collections.Generic;

namespace Edgebot
{
    public class EdgeData
    {
        public const String JoinMessage = "{0}, Welcome to OTEGamers IRC: RR1 and RR2 Version: 2.6.7.1 || Unleashed Version: Unknown";
        public static readonly List<string> Developers = new List<string> { "Helkarakse", "Citidel", "Citidel_","djdarkstorm"};

        public const String Host = "irc.esper.net:5555";
        public const String Nickname = "EdgeSharp";
        public const String Username = "EdgeSharp";
        public const String Channel = "#otegamers";

        public const String UrlTps = "http://dev.otegamers.com/api/v1/edge/tps";
        public const String UrlFish = "http://api.fishbans.com/stats/";
        public const String UrlFishLink = "http://www.fishbans.com/u/";
    }
}
