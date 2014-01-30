using System.Collections.Generic;

namespace EdgeBot.Classes.Common
{
    /// <summary>
    /// EdgeBot data
    /// </summary>
    public static class Data
    {
        // Messages
        public const string MessageJoinChannel = "{0}, Welcome to OTEGamers IRC: use !update to get Modpack Update Info!";
        public const string MessageIdentified = ":NickServ!NickServ@services.esper.net NOTICE EdgeBot :You are now identified for EdgeBot.";
        public const string MessageRestricted = "This command is restricted to ops only.";
        public const string MessageSlap = "{0} {1} {2}'s {3} with {4}.";
        public const string MessageUpdate = "Server Version: {0}, Update information: {1}";

        public const string McBansApiKey = "83855ea895268ec47f2e7ea0e8a25323f11e189c";

        // Devs
        public static readonly List<string> Developers = new List<string>
        {
            "Helkarakse",
            "Citidel",
            "Citidel_",
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
        public const string UrlBlacklist = "http://dev.otegamers.com/api/v1/edge/blacklist";
        public const string UrlBlacklistAdd = "http://dev.otegamers.com/api/v1/edge/add-blacklist/{0}/{1}";

        // TS3 API
        public const string UrlTs3NowPlaying = "http://otegamers.com:8080/getnowPlaying";
        public const string UrlTs3Next = "http://otegamers.com:8080/next";
        public const string UrlTs3Prev = "http://otegamers.com:8080/prev";
        public const string UrlTs3Stop = "http://otegamers.com:8080/stop";
        public const string UrlTs3Pause = "http://otegamers.com:8080/pause";
        public const string UrlTs3Vol = "http://otegamers.com:8080/setvolume?vol={0}";
        public const string UrlStation1 = "http://otegamers.com:8080/play?folder=radio&file=1.fm%20Classic%20Rock%20Replay.pls";
        public const string UrlStation2 = "http://otegamers.com:8080/play?folder=radio&file=netgameradio.pls";
        public const string UrlYoutube = "http://otegamers.com:8080/playyoutube?link={0}";

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

        public static readonly Dictionary<KeyValuePair<string, string>, KeyValuePair<string, string>> UpdateDict = new Dictionary<KeyValuePair<string, string>, KeyValuePair<string, string>>
        {
            {new KeyValuePair<string, string>("rr","1"), new KeyValuePair<string, string>("rr","http://www.otegamers.com/topic/6945-")},
            {new KeyValuePair<string, string>("fu","1"), new KeyValuePair<string, string>("unleashed","http://www.otegamers.com/topic/6383-")},
            {new KeyValuePair<string, string>("px","1"), new KeyValuePair<string, string>("pixelmon","http://www.otegamers.com/topic/7683-")},
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

        public static readonly List<string> BlameResponses = new List<string>
        {
            "{0} looks around for someone to blame...",
            "Hide! {0} is looking for a scapegoat.",
            "{0} needs a scapegoat to blame!",
            "{0} is looking for someone to blame!",
            "{0} casts around for something to blame..."
        };

        public static readonly List<string> BlameTargetResponses = new List<string>
        {
            "It's all {0}'s fault!",
            "Blame {0} for everything!",
            "Why {0}? Why...",
            "{0}... please... why?",
            "This is a fine predicament you've got us into, {0}!",
            "Great job {0}! Great job...",
            "You had one job, {0}, one job!",
            "Well played {0}... well played."
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
            "conks",
            "clones",
            "dessicates"
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
            "cheek",
            "spine",
            "fingers"
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
            "a TARDIS",
            "a Golden Crash",
            "a giant spear",
            "a tiny dagger",
            "a massive fireball"
        };
    }
}