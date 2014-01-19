using System;
using System.Collections.Generic;

namespace Edgebot
{
    /// <summary>
    /// Class for EdgeBot containing constants
    /// </summary>
    public class EdgeData
    {
        public const string JoinMessage = "{0}, Welcome to OTEGamers IRC: RR1 and RR2 Version: {1} || Unleashed Version: {2}";
        public static readonly List<string> Developers = new List<string> { "Helkarakse", "Citidel", "Citidel_","djdarkstorm"};
        public const string EndPortal = "The End Portal is Located at: {0} X, {1} Y, {2} Z";
        public static Object AnnounceMsg { get; set; }
        public static Object AnnounceTimes { get; set; }

        public const string Host = "irc.esper.net:5555";
        public const string Nickname = "EdgeSharp";
        public const string Username = "EdgeSharp";
        public const string Channel = "#OTEGamers";
        public const string UrlTps = "http://dev.otegamers.com/api/v1/edge/tps";
        public const string UrlFish = "http://api.fishbans.com/stats/";
        public const string UrlFishLink = "http://www.fishbans.com/u/";
        public const string UrlVersion = "http://dev.otegamers.com/api/v1/edge/version";
        public const string UrlWiki = "http://dev.otegamers.com/api/v1/edge/wiki";

    }

}
