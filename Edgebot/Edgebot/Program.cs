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
            
            _client = new IrcClient(EdgeData.Host, new IrcUser(EdgeData.Nickname, EdgeData.Username));
            _client.ConnectionComplete += (s, e) => _client.JoinChannel(EdgeData.Channel);

            _client.NetworkError += (s, e) => EdgeUtils.Log("Error: " + e.SocketError);

            _client.RawMessageRecieved += (s, e) => EdgeUtils.Log("RAWRCV {0}", e.Message);
            _client.RawMessageSent += (s, e) => EdgeUtils.Log("RAWSNT {0}", e.Message);

            _client.PrivateMessageRecieved += (sender, args) =>
            {
                // Only listen to !commands
                if (!args.PrivateMessage.Message.StartsWith("!")) return;
                if (Debug && EdgeUtils.IsDev(args.PrivateMessage.User.Nick))
                {
                    var message = args.PrivateMessage.Message;
                    // handle everything with !dev first to avoid conflict with edgebot
                    if (message.StartsWith("!dev"))
                    {
                        var paramList = message.Split(' ');
                        switch (paramList[1])
                        {
                            case "tps":
                                if (EdgeUtils.IsOp(_client, args.PrivateMessage.User.Nick))
                                {
                                    TpsHandler(paramList);
                                }
                                else
                                {
                                    EdgeUtils.SendChannel(_client, "This command is restricted to ops only.");
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
                            case "update":
                                UpdateHandler(paramList);
                                break;

                            // !dev minecheck | !dev minestatus
                            case "minecheck":
                            case "minestatus":
                                MineCheckHandler();
                                break;

                            default:
                                EdgeUtils.SendChannel(_client, "Dev command not found.");
                                break;
                        }
                    }
                }

                EdgeUtils.Log("RCVPRIV <{0}> {1}", args.PrivateMessage.User.Nick, args.PrivateMessage.Message);
            };

            _client.ChannelMessageRecieved += (sender, args) => EdgeUtils.Log("<{0}> {1}", args.PrivateMessage.User.Nick, args.PrivateMessage.Message);
            _client.UserJoinedChannel += (sender, args) =>
            {
                if (Debug && EdgeUtils.IsDev(args.User.Nick))
                {
                   EdgeUtils.SendNotice(_client, String.Format(EdgeData.JoinMessage, args.User.Nick, "2.7.6.1", "1.1.4" ), args.User.Nick);
                }
            };

            _announceTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            _client.ConnectAsync();
            while (true)
            {
            }
        }

        private static void MineCheckHandler()
        {
            EdgeConn.GetData(EdgeData.UrlMojangStatus, "get", jObject =>
            {
                var jArray = jObject.ToObject<Dictionary<string, string>>();
                foreach (var row in jArray)
                {
                    EdgeUtils.Log("{0}:{1}", row.Key, row.Value);
                }
            }, EdgeUtils.HandleException);
        }

        private static void TpsHandler(IList<string> paramList)
        {
            // Use api to retrieve data from the tps url
            EdgeConn.GetData(EdgeData.UrlTps, "get", jObject =>
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
                outputString = String.IsNullOrEmpty(filter) ? jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonTps>(row.ToString())).Aggregate(outputString, (current, tps) => current + (tps.Server.ToUpper() + ":" + tps.Tps + "-" + EdgeUtils.FormatColor(tps.Count, EdgeColors.Green) + delimiter)) : jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonTps>(row.ToString())).Where(tps => tps.Server.Contains(paramList[2])).Aggregate(outputString, (current, tps) => current + (tps.Server.ToUpper() + ":" + tps.Tps + "-" + EdgeUtils.FormatColor(tps.Count, EdgeColors.Green) + delimiter));
                if (!String.IsNullOrEmpty(outputString))
                {
                    // output to channel
                    EdgeUtils.SendChannel(_client, outputString.Substring(0, outputString.Length - delimiter.Length));
                }
            }, EdgeUtils.HandleException);
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

            var url = !String.IsNullOrEmpty(filter) ? EdgeData.UrlWiki + "/" + filter : EdgeData.UrlWiki + "/all";
            EdgeConn.GetData(url, "get", jObject =>
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

                    EdgeUtils.SendNotice(_client, outputString, nickname);
                }
                else
                {
                    EdgeUtils.SendNotice(_client, (string)jObject["message"], nickname);
                }
            }, EdgeUtils.HandleException);
        }

        private static void FishHandler(IList<string> paramList, string nick)
        {
            var url = EdgeData.UrlFish + paramList[2];
            // Use api to retrieve data from the tps url
            EdgeUtils.Log(url);
            EdgeConn.GetData(url, "get", jObject =>
            {
                var outputString = "";
                // parse the output  
                outputString = string.Concat(EdgeUtils.FormatText("Username: ",EdgeColors.Bold) , (string)jObject["stats"].SelectToken("username"), EdgeUtils.FormatText(" Total Bans: ",EdgeColors.Bold) , (string)jObject["stats"].SelectToken("totalbans"), EdgeUtils.FormatText(" URL: ",EdgeColors.Bold) , string.Concat(EdgeData.UrlFishLink + paramList[2]));
                if (!String.IsNullOrEmpty(outputString))
                {
                    // output to channel
                   EdgeUtils.SendNotice(_client, outputString, nick);
                }
            }, EdgeUtils.HandleException);
        }

        private static void EndPortalHandler()
        {
            EdgeUtils.SendChannel(_client, String.Format(EdgeData.EndPortal, "-855", "29", "-4"));
        }

        private static void AnnounceHandler(IList<string> paramList, string nick) 
        {
            if (paramList.Count <= 3)
            {
                EdgeUtils.SendNotice(_client, "Usage: !announce <time in seconds> <repeats> <message>", nick);
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
                EdgeData.AnnounceMsg = msg;
                EdgeData.AnnounceTimes = timeCount;
                _announceTimer.Enabled = true;
            }
            
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {                   
            if(Convert.ToInt32(EdgeData.AnnounceTimes) == 0)
            {
                _announceTimer.Enabled = false;
            }
            else 
            {
                var count = Convert.ToInt32(EdgeData.AnnounceTimes);
                count--;
                EdgeData.AnnounceTimes = count;
                EdgeUtils.SendChannel(_client, EdgeData.AnnounceMsg.ToString());
            }
        }
        private static void UpdateHandler(IList<string> paramList)
        {
            if(paramList.Count <= 2)
            {
                EdgeUtils.SendChannel(_client, EdgeData.rrUpdate);
            } else { 
                switch (paramList[2])
                {
                    case "rr":
                        EdgeUtils.SendChannel(_client, EdgeData.rrUpdate);
                        break;
                    case "ftb":
                        EdgeUtils.SendChannel(_client, EdgeData.ftbUpdate);
                        break;
                    case "px":
                        EdgeUtils.SendChannel(_client, EdgeData.pxUpdate);
                        break;
                    default:
                        EdgeUtils.SendChannel(_client, EdgeData.rrUpdate);
                        break;
                }
            }

        }

    }
}
