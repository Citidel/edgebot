using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Instances;
using EdgeBot.Classes.JSON;
using Newtonsoft.Json;

namespace EdgeBot.Classes
{
    class Program
    {
        public const bool Debug = true;
        private static Timer _announceTimer;
        private static IrcClient _client;
        private static readonly List<Server> ServerList = new List<Server>();

        static void Main()
        {
            _announceTimer = new Timer();

            _client = new IrcClient(Config.Host, new IrcUser(Config.Nickname, Config.Username));
            _client.ConnectionComplete += (s, e) =>
            {
                _client.JoinChannel(Config.Channel);

                // pull addresses from api
                Connection.GetData(Data.UrlAddress, "get", jObject =>
                {
                    if ((bool)jObject["success"])
                    {
                        foreach (var server in jObject["result"].Select(row => new Server
                        {
                            ShortCode = (string)row["short_code"],
                            Id = (string)row["server"],
                            Address = (string)row["address"]
                        }))
                        {
                            ServerList.Add(server);
                        }

                        if (ServerList.Count > 0)
                        {
                            Utils.Log("Server addresses retrieved from API");
                        }
                    }
                    else
                    {
                        Utils.Log("Failed to query for servers.");
                    }
                }, Utils.HandleException);
            };
            _client.NetworkError += (s, e) => Utils.Log("Error: " + e.SocketError);
            _client.RawMessageRecieved += (s, e) => Utils.Log("RAWRCV {0}", e.Message);
            _client.RawMessageSent += (s, e) => Utils.Log("RAWSNT {0}", e.Message);

            _client.PrivateMessageRecieved += (sender, args) =>
            {
                // Only listen to !commands
                if (Utils.IsDev(args.PrivateMessage.User.Nick) && args.PrivateMessage.Message.StartsWith("!"))
                {
                    var message = args.PrivateMessage.Message;
                    var paramList = message.Split(' ');
                    switch (paramList[0].Substring(1))
                    {
                        // !tps
                        case "tps":
                            if (Utils.IsOp(_client, args.PrivateMessage.User.Nick))
                            {
                                TpsHandler(paramList);
                            }
                            else
                            {
                                Utils.SendChannel(_client, "This command is restricted to ops only.");
                            }
                            break;

                        // !wiki <keyword>
                        case "wiki":
                            WikiHandler(paramList);
                            break;

                        // !check <username>
                        case "check":
                            FishHandler(paramList, args.PrivateMessage.User.Nick);
                            break;

                        // !announce <time in seconds> <repeats> <message>
                        case "announce":
                            AnnounceHandler(paramList, args.PrivateMessage.User.Nick);
                            break;

                        // !update
                        case "update":
                            UpdateHandler(paramList);
                            break;

                        // !minecheck | !minestatus
                        case "minecheck":
                        case "minestatus":
                            MineCheckHandler();
                            break;

                        // !log <pack> <server>
                        case "log":
                            if (Utils.IsOp(_client, args.PrivateMessage.User.Nick))
                            {
                                LogHandler(paramList);
                            }
                            else
                            {
                                Utils.SendChannel(_client, "This command is restricted to ops only.");
                            }
                            break;

                        // !8 <question>
                        case "8":
                            EightBallHandler(paramList);
                            break;

                        // !dice <number> <sides>
                        case "dice":
                            DiceHandler(paramList);
                            break;

                        // !help, !help <keyword>
                        case "help":
                            HelpHandler(paramList);
                            break;

                        default:
                            Utils.SendChannel(_client, "Dev command not found.");
                            break;
                    }

                    //listen for www or http(s)
                    if (args.PrivateMessage.Message.Contains("http://") | args.PrivateMessage.Message.Contains("https://") | args.PrivateMessage.Message.Contains("www."))
                    {
                        var url = "";
                        for (var i = 0; i < paramList.Count(); i++)
                        {
                            if (paramList[i].Contains("http://") | paramList[i].Contains("https://"))
                            {
                                url = paramList[i];
                            }
                            else if (paramList[i].Contains("www."))
                            {
                                url = string.Concat("http://", paramList[i]);
                            }

                            Connection.GetLinkTitle(url, title =>
                            {
                                if (!string.IsNullOrEmpty(title)) { Utils.SendChannel(_client, "URL TITLE: " + title); } else { Utils.Log("Connection: Result is null"); }
                            }, Utils.HandleException);
                        }
                    }
                }

                Utils.Log("RCVPRIV <{0}> {1}", args.PrivateMessage.User.Nick, args.PrivateMessage.Message);
            };

            _client.ChannelMessageRecieved += (sender, args) => Utils.Log("<{0}> {1}", args.PrivateMessage.User.Nick, args.PrivateMessage.Message);
            _client.UserJoinedChannel += (sender, args) =>
            {
                if (Utils.IsDev(args.User.Nick))
                {
                    Utils.SendNotice(_client, String.Format(Data.JoinMessage, args.User.Nick, "2.7.6.1", "1.1.4"), args.User.Nick);
                }
            };

            _announceTimer.Elapsed += OnTimedEvent;

            _client.ConnectAsync();
            while (true)
            {
            }
        }

        private static void HelpHandler(IList<string> paramList)
        {
            var filter = "";
            switch (paramList.Count())
            {
                case 1:
                    break;
                case 2:
                    filter = paramList[1];
                    break;

                default:
                    Utils.SendChannel(_client, "Usage: !help");
                    return;
            }

            var url = !String.IsNullOrEmpty(filter) ? Data.UrlHelp + "/" + filter : Data.UrlHelp + "/all";
            Connection.GetData(url, "get", jObject =>
            {
                if ((bool)jObject["success"])
                {
                    string outputString;
                    const string delimiter = ", ";
                    if (!String.IsNullOrEmpty(filter))
                    {
                        var help = JsonConvert.DeserializeObject<JsonHelp>(jObject["result"].ToString());
                        outputString = help.Value;
                    }
                    else
                    {
                        outputString = jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonHelp>(row.ToString())).Aggregate("The following keywords are valid: ", (current, help) => current + (help.Keyword + delimiter));
                        outputString = outputString.Substring(0, outputString.Length - delimiter.Length);
                    }

                    Utils.SendChannel(_client, outputString);
                }
                else
                {
                    Utils.SendChannel(_client, (string)jObject["message"]);
                }
            }, Utils.HandleException);
        }

        private static void DiceHandler(IList<string> paramList)
        {
            int i;
            // check if the params number 4, that the number/sides are integers, and that number and sides are both greater than 0
            if (paramList.Count() == 3 && int.TryParse(paramList[1], out i) && int.TryParse(paramList[2], out i) && (int.Parse(paramList[1]) > 0) && (int.Parse(paramList[2]) > 0))
            {
                var dice = int.Parse(paramList[1]);
                var sides = int.Parse(paramList[2]);
                var random = new Random();

                var diceList = new List<int>();
                for (var j = 0; j < dice; j++)
                {
                    diceList.Add(random.Next(1, sides));
                }

                var outputString = string.Format("Rolling a {0} sided die, {1} time{2}: {3}", sides, dice, (dice > 1) ? "s" : "", diceList.Aggregate("", (current, roll) => current + roll + " ").Trim());
                Utils.SendChannel(_client, outputString);
            }
            else
            {
                Utils.SendChannel(_client, "Usage: !dice <number> <sides>");
            }
        }

        private static void EightBallHandler(ICollection<string> paramList)
        {
            if (paramList.Count > 1)
            {
                var response = Data.EightBallResponses[new Random().Next(0, Data.EightBallResponses.Count)];
                Utils.SendChannel(_client, "The magic 8 ball responds with: " + response);
            }
            else
            {
                Utils.SendChannel(_client, "No question was asked of the magic 8 ball!");
            }
        }

        private static void LogHandler(IList<string> paramList)
        {
            int i;
            // check if params number more than 4, if the pack length is less than 5 and the server is a number
            if (paramList.Count == 3 && paramList[1].Length < 5 && int.TryParse(paramList[2], out i))
            {
                Connection.GetData(string.Format(Data.UrlCrashLog, paramList[1], paramList[2]), "get", jObject =>
                {
                    if ((bool)jObject["success"])
                    {
                        Utils.SendChannel(_client, (string)jObject["result"]["response"]);
                    }
                    else
                    {
                        Utils.SendChannel(_client, "Failed to push crash log to pastebin. Please try again later.");
                    }
                }, Utils.HandleException);
            }
            else
            {
                Utils.SendChannel(_client, "Usage: !log <pack> <server_id>");
            }
        }

        private static void MineCheckHandler()
        {
            Connection.GetServerStatus(status =>
            {
                var message = string.Concat("MCStatus: ", Utils.FormatStatus("Accounts", status.Account), ", ", Utils.FormatStatus("Session", status.Session), ", ", Utils.FormatStatus("Auth", status.Authentication), ", ", Utils.FormatStatus("Site", status.Website), ", ", Utils.FormatStatus("Login", status.Login));
                Utils.SendChannel(_client, message);
            }, Utils.HandleException);
        }

        private static void TpsHandler(IList<string> paramList)
        {
            // Use api to retrieve data from the tps url
            Connection.GetData(Data.UrlTps, "get", jObject =>
            {
                // if filter throws the exception then no filter
                string filter;
                try
                {
                    filter = paramList[1];
                }
                catch (ArgumentOutOfRangeException)
                {
                    filter = "";
                }

                var outputString = "";
                const string delimiter = ", ";
                // parse the output string using linq
                outputString = String.IsNullOrEmpty(filter) ? jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonTps>(row.ToString())).Aggregate(outputString, (current, tps) => current + (Utils.FormatText(tps.Server.ToUpper(), Colors.Bold) + ":" + Utils.FormatTps(tps.Tps) + "-" + Utils.FormatColor(tps.Count, Colors.DarkGreen) + delimiter)) : jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonTps>(row.ToString())).Where(tps => tps.Server.Contains(paramList[1])).Aggregate(outputString, (current, tps) => current + (Utils.FormatText(tps.Server.ToUpper(), Colors.Bold) + ":" + Utils.FormatTps(tps.Tps) + "-" + Utils.FormatColor(tps.Count, Colors.DarkGreen) + delimiter));
                if (!String.IsNullOrEmpty(outputString))
                {
                    // output to channel
                    Utils.SendChannel(_client, outputString.Substring(0, outputString.Length - delimiter.Length));
                }
            }, Utils.HandleException);
        }

        private static void WikiHandler(IList<string> paramList)
        {
            var filter = "";
            switch (paramList.Count())
            {
                case 1:
                    break;
                case 2:
                    filter = paramList[1];
                    break;

                default:
                    Utils.SendChannel(_client, "Usage: !wiki");
                    return;
            }

            var url = !String.IsNullOrEmpty(filter) ? Data.UrlWiki + "/" + filter : Data.UrlWiki + "/all";
            Connection.GetData(url, "get", jObject =>
            {
                if ((bool)jObject["success"])
                {
                    string outputString;
                    const string delimiter = ", ";
                    if (!String.IsNullOrEmpty(filter))
                    {
                        var wiki = JsonConvert.DeserializeObject<JsonWiki>(jObject["result"].ToString());
                        outputString = "Wiki URL for '" + wiki.Keyword + "' - " + wiki.Url;
                    }
                    else
                    {
                        outputString = jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonWiki>(row.ToString())).Aggregate("The following keywords are valid: ", (current, wiki) => current + (wiki.Keyword + delimiter));
                        outputString = outputString.Substring(0, outputString.Length - delimiter.Length);
                    }

                    Utils.SendChannel(_client, outputString);
                }
                else
                {
                    Utils.SendChannel(_client, (string)jObject["message"]);
                }
            }, Utils.HandleException);
        }

        private static void FishHandler(IList<string> paramList, string nick)
        {
            var url = Data.UrlFish + paramList[1];
            Connection.GetData(url, "get", jObject =>
            {
                string outputString;
                if ((bool)jObject["success"])
                {
                    var fishBans = new FishBans
                    {
                        TotalBans = (int)jObject["stats"].SelectToken("totalbans"),
                        Url = Data.UrlFishLink + paramList[1],
                        Username = (string)jObject["stats"].SelectToken("username")
                    };

                    var colorCode = "";
                    if (fishBans.TotalBans == 0)
                    {
                        colorCode = Colors.DarkGreen;
                    }
                    else if (fishBans.TotalBans >= 1 && fishBans.TotalBans < 5)
                    {
                        colorCode = Colors.Yellow;
                    }
                    else if (fishBans.TotalBans > 5)
                    {
                        colorCode = Colors.Red;
                    }

                    outputString = string.Concat(Utils.FormatText("Username: ", Colors.Bold), fishBans.Username,
                        Utils.FormatText(" Total Bans: ", Colors.Bold), Utils.FormatColor(fishBans.TotalBans, colorCode),
                        Utils.FormatText(" URL: ", Colors.Bold), fishBans.Url);
                }
                else
                {
                    outputString = Utils.FormatText("Error: ", Colors.Bold) + (string)jObject["error"];
                }

                if (!String.IsNullOrEmpty(outputString))
                {
                    Utils.SendNotice(_client, outputString, nick);
                }
            }, Utils.HandleException);
        }

        private static void AnnounceHandler(IList<string> paramList, string nick)
        {
            if (paramList.Count < 3)
            {
                Utils.SendNotice(_client, "Usage: !announce <time in seconds> <repeats> <message>", nick);
            }
            else
            {
                var msg = "";
                var timeTick = Convert.ToInt32(paramList[1]) * 1000;
                var timeCount = Convert.ToInt32(paramList[2]);
                if (timeTick == 0) return;
                _announceTimer.Interval = timeTick;
                GC.KeepAlive(_announceTimer);
                for (var i = 3; i < paramList.Count; i++)
                {
                    msg = msg + paramList[i] + " ";
                }
                Data.AnnounceMsg = msg;
                Data.AnnounceTimes = timeCount;
                _announceTimer.Enabled = true;
            }

        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (Convert.ToInt32(Data.AnnounceTimes) == 0)
            {
                _announceTimer.Enabled = false;
            }
            else
            {
                var count = Convert.ToInt32(Data.AnnounceTimes);
                count--;
                Data.AnnounceTimes = count;
                Utils.SendChannel(_client, Data.AnnounceMsg.ToString());
            }
        }
        private static void UpdateHandler(IList<string> paramList)
        {
            if (paramList.Count < 2)
            {
                Utils.SendChannel(_client, Data.RrUpdate);
            }
            else
            {
                switch (paramList[1])
                {
                    case "rr":
                        Utils.SendChannel(_client, Data.RrUpdate);
                        break;
                    case "ftb":
                        Utils.SendChannel(_client, Data.FtbUpdate);
                        break;
                    case "px":
                        Utils.SendChannel(_client, Data.PxUpdate);
                        break;
                    default:
                        Utils.SendChannel(_client, Data.RrUpdate);
                        break;
                }
            }

        }

    }
}
