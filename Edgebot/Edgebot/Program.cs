using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using System;
using Edgebot.JSON;
using Newtonsoft.Json;
using System.Timers;

namespace Edgebot
{
    class Program
    {
        public const bool Debug = true;
        private static Timer _announceTimer;
        private static IrcClient _client;

        static void Main()
        {
            _announceTimer = new Timer();

            _client = new IrcClient(Data.Host, new IrcUser(Data.Nickname, Data.Username));
            _client.ConnectionComplete += (s, e) => _client.JoinChannel(Data.Channel);

            _client.NetworkError += (s, e) => Utils.Log("Error: " + e.SocketError);

            _client.RawMessageRecieved += (s, e) => Utils.Log("RAWRCV {0}", e.Message);
            _client.RawMessageSent += (s, e) => Utils.Log("RAWSNT {0}", e.Message);

            _client.PrivateMessageRecieved += (sender, args) =>
            {
                // Only listen to !commands
                if (!args.PrivateMessage.Message.StartsWith("!")) return;
                if (Debug && Utils.IsDev(args.PrivateMessage.User.Nick))
                {
                    var message = args.PrivateMessage.Message;
                    // handle everything with !dev first to avoid conflict with edgebot
                    if (message.StartsWith("!dev"))
                    {
                        var paramList = message.Split(' ');
                        switch (paramList[1])
                        {
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

                            // !dev wiki <keyword>
                            case "wiki":
                                WikiHandler(paramList, args.PrivateMessage.User.Nick);
                                break;

                            // !dev check <username>
                            case "check":
                                FishHandler(paramList, args.PrivateMessage.User.Nick);
                                break;

                            // !dev endportal
                            case "endportal":
                                EndPortalHandler();
                                break;

                            // !dev announce <time in seconds> <repeates> <message>
                            case "announce":
                                AnnounceHandler(paramList, args.PrivateMessage.User.Nick);
                                break;

                            // !dev update
                            case "update":
                                UpdateHandler(paramList);
                                break;

                            // !dev minecheck | !dev minestatus
                            case "minecheck":
                            case "minestatus":
                                MineCheckHandler();
                                break;

                            default:
                                Utils.SendChannel(_client, "Dev command not found.");
                                break;
                        }
                    }
                }

                Utils.Log("RCVPRIV <{0}> {1}", args.PrivateMessage.User.Nick, args.PrivateMessage.Message);
            };

            _client.ChannelMessageRecieved += (sender, args) => Utils.Log("<{0}> {1}", args.PrivateMessage.User.Nick, args.PrivateMessage.Message);
            _client.UserJoinedChannel += (sender, args) =>
            {
                if (Debug && Utils.IsDev(args.User.Nick))
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
                    filter = paramList[2];
                }
                catch (ArgumentOutOfRangeException)
                {
                    filter = "";
                }

                var outputString = "";
                const string delimiter = " || ";
                // parse the output string using linq
                outputString = String.IsNullOrEmpty(filter) ? jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonTps>(row.ToString())).Aggregate(outputString, (current, tps) => current + (tps.Server.ToUpper() + ":" + tps.Tps + "-" + Utils.FormatColor(tps.Count, EdgeColors.Green) + delimiter)) : jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonTps>(row.ToString())).Where(tps => tps.Server.Contains(paramList[2])).Aggregate(outputString, (current, tps) => current + (tps.Server.ToUpper() + ":" + tps.Tps + "-" + Utils.FormatColor(tps.Count, EdgeColors.Green) + delimiter));
                if (!String.IsNullOrEmpty(outputString))
                {
                    // output to channel
                    Utils.SendChannel(_client, outputString.Substring(0, outputString.Length - delimiter.Length));
                }
            }, Utils.HandleException);
        }

        private static void WikiHandler(IList<string> paramList, string nickname)
        {
            var filter = "";
            try
            {
                filter = paramList[2];
            }
            catch (ArgumentOutOfRangeException)
            {
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

                    Utils.SendNotice(_client, outputString, nickname);
                }
                else
                {
                    Utils.SendNotice(_client, (string)jObject["message"], nickname);
                }
            }, Utils.HandleException);
        }

        private static void FishHandler(IList<string> paramList, string nick)
        {
            var url = Data.UrlFish + paramList[2];
            // Use api to retrieve data from the tps url
            Utils.Log(url);
            Connection.GetData(url, "get", jObject =>
            {
                // parse the output  
                var outputString = string.Concat(Utils.FormatText("Username: ", EdgeColors.Bold), (string)jObject["stats"].SelectToken("username"), Utils.FormatText(" Total Bans: ", EdgeColors.Bold), (string)jObject["stats"].SelectToken("totalbans"), Utils.FormatText(" URL: ", EdgeColors.Bold), Data.UrlFishLink, paramList[2]);
                if (!String.IsNullOrEmpty(outputString))
                {
                    // output to channel
                    Utils.SendNotice(_client, outputString, nick);
                }
            }, Utils.HandleException);
        }

        private static void EndPortalHandler()
        {
            Utils.SendChannel(_client, String.Format(Data.EndPortal, "-855", "29", "-4"));
        }

        private static void AnnounceHandler(IList<string> paramList, string nick)
        {
            if (paramList.Count <= 3)
            {
                Utils.SendNotice(_client, "Usage: !announce <time in seconds> <repeats> <message>", nick);
            }
            else
            {
                var msg = "";
                var timeTick = Convert.ToInt32(paramList[2]) * 1000;
                var timeCount = Convert.ToInt32(paramList[3]);
                if (timeTick == 0) return;
                _announceTimer.Interval = timeTick;
                GC.KeepAlive(_announceTimer);
                for (var i = 4; i < paramList.Count; i++)
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
            if (paramList.Count <= 2)
            {
                Utils.SendChannel(_client, Data.RrUpdate);
            }
            else
            {
                switch (paramList[2])
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
