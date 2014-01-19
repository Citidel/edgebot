﻿using System;
using System.Collections.Generic;

namespace EdgeBot.Classes.Common
{
    /// <summary>
    /// EdgeBot data
    /// </summary>
    public class Data
    {
        // Messages
        public const string JoinMessage =
            "{0}, Welcome to OTEGamers IRC: RR1 and RR2 Version: {1} || Unleashed Version: {2}";

        public const string EndPortal = "The End Portal is Located at: {0} X, {1} Y, {2} Z";
        public static Object AnnounceMsg { get; set; }
        public static Object AnnounceTimes { get; set; }

        public const string RrUpdate = "Please go to: www.otegamers.com/topic/6945- for Resonant Rise update info";
        public const string FtbUpdate = "Please go to: http://www.otegamers.com/topic/6383- for Unleashed update info";
        public const string PxUpdate = "Please go to: http://www.otegamers.com/topic/7683- for Pixelmon update info";

        // Devs
        public static readonly List<string> Developers = new List<string>
        {
            "Helkarakse",
            "Citidel",
            "Citidel_",
            "djdarkstorm"
        };

        // URLs
        public const string UrlTps = "http://dev.otegamers.com/api/v1/edge/tps";
        public const string UrlFish = "http://api.fishbans.com/stats/";
        public const string UrlFishLink = "http://www.fishbans.com/u/";
        public const string UrlVersion = "http://dev.otegamers.com/api/v1/edge/version";
        public const string UrlWiki = "http://dev.otegamers.com/api/v1/edge/wiki";
        public const string UrlMojangStatus = "http://status.mojang.com/check";
        public const string UrlCrashLog = "http://dev.otegamers.com/api/v1/edge/last-crash-log/{0}/{1}";

        public static readonly List<string> EightBallResponses = new List<string>
        {
            "Signs point to yes.",
            "Yes.",
            "Reply is hazy, try again.",
            "Without a doubt.",
            "My sources say no.",
            "As I see it, yes.",
            "You may rely on it.",
            "Concentrate and ask again.",
            "Outlook is not so good.",
            "It is decidedly so.",
            "Better not to tell you now.",
            "Very doubtful.",
            "Yes, definitely.",
            "It is certain.",
            "Cannot predict now.",
            "Most likely.",
            "Ask again later.",
            "My reply is no.",
            "Outlook good.",
            "Don't count on it."
        };
    }
}