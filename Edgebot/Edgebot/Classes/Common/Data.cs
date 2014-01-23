using System.Collections.Generic;

namespace EdgeBot.Classes.Common
{
    /// <summary>
    /// EdgeBot data
    /// </summary>
    public static class Data
    {
        // Messages
        public const string MessageJoinChannel = "{0}, Welcome to OTEGamers IRC: RR1 and RR2 Version: {1} || Unleashed Version: {2}";
        public const string MessageIdentified = ":NickServ!NickServ@services.esper.net NOTICE EdgeBot :You are now identified for EdgeBot.";
        public const string MessageRestricted = "This command is restricted to ops only.";
        public const string MessageSlap = "{0} {1} {2}'s {3} with {4}";

        public const string RrUpdate = "Please go to: http://www.otegamers.com/topic/6945- for Resonant Rise update info";
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

        // Server Admin
        public static readonly List<string> Admin = new List<string>
        {
            "Cozza38",
            "Cozza38_",
            "Ryahn"
        }; 

        // OTE API
        public const string UrlTps = "http://dev.otegamers.com/api/v1/edge/tps";
        public const string UrlAddress = "http://dev.otegamers.com/api/v1/edge/address";
        public const string UrlWiki = "http://dev.otegamers.com/api/v1/edge/wiki";
        public const string UrlHelp = "http://dev.otegamers.com/api/v1/edge/help";
        public const string UrlCrashLog = "http://dev.otegamers.com/api/v1/edge/last-crash-log/{0}/{1}";
        public const string UrlQuote = "http://dev.otegamers.com/api/v1/edge/quote";
        public const string UrlQuoteAdd = "http://dev.otegamers.com/api/v1/edge/add-quote/{0}/{1}/{2}";

        // URLs
        public const string UrlFish = "http://api.fishbans.com/stats/";
        public const string UrlFishLink = "http://www.fishbans.com/u/";
        public const string UrlMojangStatus = "http://status.mojang.com/check";

        public static readonly List<string> McBansApiServerList = new List<string>
        {
            "api01.cluster.mcbans.com",
            "api02.cluster.mcbans.com",
            "api03.cluster.mcbans.com",
            "api.mcbans.com"
        }; 

        // Responses
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

        public static readonly List<string> SmugResponses = new List<string>
        {
            "U WOT!?!",
            "HUEHUEHUEHUEHUEHUEHUEHUEHUE!!"
        };

        // Slap related
        public static readonly List<string> SlapActions = new List<string>
        {
            "slaps",
            "pokes",
            "wallops",
            "stings",
            "burninates",
            "deforms",
            "macerates",
            "nerfs",
            "chops",
            "confounds",
            "prods",
            "decorates",
            "draws on",
            "clones",
            "pinches"
        };

        public static readonly List<string> SlapLocations = new List<string>
        {
            "head",
            "ribs",
            "face",
            "arms",
            "legs",
            "feet",
            "nose",
            "ear",
            "forehead",
            "chin",
            "cheek"
        };

        public static readonly List<string> SlapItems = new List<string>
        {
            "a live trout",
            "a dead trout",
            "a baseball",
            "a cicada",
            "a rubber chicken",
            "a magic missile",
            "a smuggled wonton",
            "a storm of hubris",
            "a +2 Vajra",
            "a macerator",
            "a hastily crafted dart of force",
            "a blue screen of death",
            "a bag of smug-looking leaves",
            "a ball of fried dough",
            "a can of moon cheese",
            "a TARDIS"
        };
    }
}