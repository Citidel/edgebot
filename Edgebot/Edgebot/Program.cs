using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using System;
using Edgebot.JSON;
using Newtonsoft.Json;

namespace Edgebot
{
    class Program
    {
        public const bool Debug = true;
        private static IrcClient _client;

        static void Main()
        {
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
                                TpsHandler(paramList);
                                break;

                            // !dev wiki <keyword>
                            case "wiki":
                                WikiHandler(paramList, args.PrivateMessage.User.Nick);
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
                    EdgeUtils.SendNotice(_client, String.Format(EdgeData.JoinMessage, args.User.Nick), args.User.Nick);
                }
            };

            _client.ConnectAsync();
            while (true)
            {
            }
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
                outputString = String.IsNullOrEmpty(filter) ? jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonTps>(row.ToString())).Aggregate(outputString, (current, tps) => current + (tps.Server.ToUpper() + ":" + tps.Tps + "-" + tps.Count + delimiter)) : jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonTps>(row.ToString())).Where(tps => tps.Server.Contains(paramList[2])).Aggregate(outputString, (current, tps) => current + (tps.Server.ToUpper() + ":" + tps.Tps + "-" + tps.Count + delimiter));
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
    }
}
