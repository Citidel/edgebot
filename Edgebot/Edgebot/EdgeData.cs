using System;
using System.Collections.Generic;

namespace Edgebot
{
    /// <summary>
    /// Class for EdgeBot containing constants
    /// </summary>
    public class EdgeData
    {
        public const String JoinMessage = "{0}, Welcome to OTEGamers IRC: RR1 and RR2 Version: {1} || Unleashed Version: {2}";
        public static readonly List<string> Developers = new List<string> { "Helkarakse", "Citidel", "Citidel_","djdarkstorm"};

        public const String Host = "irc.esper.net:5555";
        public const String Nickname = "EdgeSharp";
        public const String Username = "EdgeSharp";
        public const String Channel = "#OTEGamers";
        public const String UrlTps = "http://dev.otegamers.com/api/v1/edge/tps";
        public const String UrlFish = "http://api.fishbans.com/stats/";
        public const String UrlFishLink = "http://www.fishbans.com/u/";
        public const String UrlVersion = "http://dev.otegamers.com/api/v1/edge/version";
        public const String UrlWiki = "http://dev.otegamers.com/api/v1/edge/wiki";
    }
}
