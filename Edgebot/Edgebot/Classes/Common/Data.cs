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
        public const string MessageRestrictedIrc = "This command is restricted to the IRC channel only.";

        public const string MessageSlap = "{0} {1} {2}'s {3} with {4}!";
        public const string MessageUpdate = "Server Version: {0}, Update information: {1}";

        public const string McBansApiKey = "83855ea895268ec47f2e7ea0e8a25323f11e189c";

        // Devs
        public static readonly List<string> Developers = new List<string>
        {
            "Helkarakse",
            "Citidel",
            "Citidel_"
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
        public const string UrlQuoteAdd = "http://dev.otegamers.com/api/v1/edge/add-quote/{0}/{1}/{2}";
        public const string UrlBlacklist = "http://dev.otegamers.com/api/v1/edge/blacklist";
        public const string UrlBlacklistAdd = "http://dev.otegamers.com/api/v1/edge/add-blacklist/{0}/{1}";
        public const string UrlBanCheck = "http://dev.otegamers.com/api/v1/edge/bans/{0}";

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
            //{new KeyValuePair<string, string>("fu","1"), new KeyValuePair<string, string>("unleashed","http://www.otegamers.com/topic/6383-")},
            //{new KeyValuePair<string, string>("px","1"), new KeyValuePair<string, string>("pixelmon","http://www.otegamers.com/topic/7683-")},
            //{new KeyValuePair<string, string>("mf","1"), new KeyValuePair<string, string>("mf2","http://www.otegamers.com/topic/8174-")},
            //{new KeyValuePair<string, string>("fm","1"), new KeyValuePair<string, string>("monster","http://www.otegamers.com/topic/8173-")},
            //{new KeyValuePair<string, string>("dw","1"), new KeyValuePair<string, string>("dw20","http://www.otegamers.com/topic/8322-")},
            //{new KeyValuePair<string, string>("tp","1"), new KeyValuePair<string, string>("tppi","http://www.otegamers.com/topic/8321-")},
            {new KeyValuePair<string, string>("sky","1"), new KeyValuePair<string, string>("sky","http://www.otegamers.com/forum/228-")},
            {new KeyValuePair<string, string>("yogs","1"), new KeyValuePair<string, string>("yogs","http://www.otegamers.com/topic/9503-")},
            {new KeyValuePair<string, string>("pvp","1"), new KeyValuePair<string, string>("pvp","http://www.otegamers.com/topic/11115-")},
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
            "Don't count on it.",
            "It's totes gonna happen.",
            "It's totes never gonna happen.",
            "It's totes gonna never happen at all.",
            "It's totes gonna not not not not not not not not not not not not not not happen."
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
            "dessicates",
            "clouds",
            "pings",
            "annoys",
            "smooshes",
            "squeezes",
            "stabs",
            "slices",
            "fillets"
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
            "fingers",
            "ankles",
            "ankle",
            "finger",
            "ears"
        };

        public static readonly List<string> SlapItems = new List<string>
        {
            "a live trout",
            "a dead trout",
            "a baseball",
            "a cicada",
            "a rubber chicken",
            "a magic missile",
            "a spray of color",
            "a pair of burning hands",
            "a smuggled wonton",
            "a dark and stormy cloud of arbitrarily formed hubris",
            "a +2 Vajra",
            "a macerator",
            "a hastily crafted dart of force",
            "a blue screen of death",
            "a bag of smug-looking leaves",
            "a doughnut",
            "a can of moon cheese",
            "a TARDIS",
            "a Golden Crash",
            "a giant spear",
            "a tiny dagger",
            "a massive fireball",
            "a wiffle bat",
            "a Justin Bieber album",
            "a +14 enchanted hammer of ban",
            "a piece of Gregorian technology",
            "a tungstensteel hammer",
            "an Industrial Blarb Furnace",
            "a sacrifice to the ARB GARB",
            "a finely filigreed filleting fish finger",
            "ALDUIN! EATER OF WORLDS",
            "FUS RO DAH",
            "a mudkip",
            "an ancient dinosaur species known only as the Auricbertusaurus",
            "a tote of goats",
            "a cranberry souffle",
            "a beard-comb",
            "a bottle of Maker's Mark",
            "a tube of herbal turtle jelly",
            "a rolled up newspaper",
            "Zoidberg",
            "a can of Soylent Green",
            "Van Persie's magical header",
            "a gold star",
            "a supernova",
            "a bag of sausages"
        };

        public static readonly List<string> HatActions = new List<string>
        {
            "shoves",
            "thrusts", 
            "crams", 
            "jabs",
            "digs"
        };
        public static readonly List<string> HatItems = new List<string>
        {
            "a large ferret.",
            "a rabbit.",
            "a chicken.",
            "a raven.",
            "a massive tarantula.",
            "an adorable puppy.",
            "an adorable kitten.",
            "a wild ostrich.",
            "a relentless baby dragon.",
            "another hat.",
            "a wild Golden Crash!",
            "Pooky.",
            "Zot!",
            "a +6 Ban Hammer!",
            "a mudkip.",
            "ALDUIN! EATER OF WORLDS!",
            "a bottle of Maker's Mark!"
        };
        public static readonly List<string> GamerPoop = new List<string>
        {
            "umY9aO2dXsQ", "ZU2vIAryZU4", "Q4SXtMmcdUo", "6xtjLnyMyo4",
            "BCtsFtPs1fY", "LG46DboDfhk", "yOpdnWYZ_ic", "nR9gMg4hKfY",
            "ZPllpzxmM5k", "9qMdd96Dqko", "CpeRk1YFn8s", "jpJ7NihviVU",
            "DM8krWnU0uQ", "crgEIhI3y_o", "pSawGT5bgdM", "gawughIGjK0",
            "Rs_ty7I3X8w", "azI_RlDHTBE", "BUOw3JyPRlQ", "661dbFQYbgU",
            "60sn_8qXsTA", "qsIS17jJH7A", "voc9TrSVVOA", "7vo5xhRTOjU",
            "k5FRbZ1zZm4", "8WzT2fnkfPk", "V-tgSGYU9fw", "mM-bfn8vZ_w",
            "GQ1ISnkS5oY", "TTUrDkBUtfk", "EM1RXZGWiqo", "SjIeo5cOGQs",
            "YmqlrhCNiMU", "HycTcUN6H0M", "HqQMS4vYSkM", "4xQLjIDjcIs",
        };
    }
}