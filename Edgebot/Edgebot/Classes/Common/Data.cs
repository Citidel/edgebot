using System;
using System.Collections.Generic;

namespace EdgeBot.Classes.Common
{
    /// <summary>
    /// EdgeBot data
    /// </summary>
    public class Data
    {
        // Messages
        public const string JoinMessage = "{0}, Welcome to OTEGamers IRC: RR1 and RR2 Version: {1} || Unleashed Version: {2}";
        public const string EndPortal = "The End Portal is Located at: {0} X, {1} Y, {2} Z";
        public static Object AnnounceMsg { get; set; }
        public static Object AnnounceTimes { get; set; }

        public const string RrUpdate = "Please go to: www.otegamers.com/topic/6945- for Resonant Rise update info";
        public const string FtbUpdate = "Please go to: http://www.otegamers.com/topic/6383- for Unleashed update info";
        public const string PxUpdate = "Please go to: http://www.otegamers.com/topic/7683- for Pixelmon update info";

        // Devs
        public static readonly List<string> Developers = new List<string> { "Helkarakse", "Citidel", "Citidel_","djdarkstorm"};

        // URLs
        public const string UrlTps = "http://dev.otegamers.com/api/v1/edge/tps";
        public const string UrlFish = "http://api.fishbans.com/stats/";
        public const string UrlFishLink = "http://www.fishbans.com/u/";
        public const string UrlVersion = "http://dev.otegamers.com/api/v1/edge/version";
        public const string UrlWiki = "http://dev.otegamers.com/api/v1/edge/wiki";
        public const string UrlMojangStatus = "http://status.mojang.com/check";
    }
}
