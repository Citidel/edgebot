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
                            case "check":
                                FishHandler(paramList, args.PrivateMessage.User.Nick);
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
                // parse the output string using linq
                outputString = String.IsNullOrEmpty(filter) ? jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonTps>(row.ToString())).Aggregate(outputString, (current, tps) => current + (tps.Server.ToUpper() + ":" + tps.Tps + "-" + tps.Count + " || ")) : jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonTps>(row.ToString())).Where(tps => tps.Server.Contains(paramList[2])).Aggregate(outputString, (current, tps) => current + (tps.Server.ToUpper() + ":" + tps.Tps + "-" + tps.Count + " || "));
                if (!String.IsNullOrEmpty(outputString))
                {
                    // output to channel
                    EdgeUtils.SendChannel(_client, outputString.Substring(0, outputString.Length - 4));
                }
            }, EdgeUtils.HandleException);
        }
        private static void FishHandler(IList<string> paramList, string nick)
        {
            // Use api to retrieve data from the tps url
            EdgeUtils.Log(string.Concat(EdgeData.UrlFish + paramList[2]));
            EdgeConn.GetData(string.Concat(EdgeData.UrlFish + paramList[2]), "get", jObject =>
            {
                EdgeUtils.Log("<0>", jObject.ToString());
                var outputString = "";
                // parse the output string using linq   
                outputString = string.Concat("Username: ", (string)jObject["stats"].SelectToken("username"), " Total Bans: ", (string)jObject["stats"].SelectToken("totalbans"), " URL: ", string.Concat(EdgeData.UrlFishLink + paramList[2]));
                if (!String.IsNullOrEmpty(outputString))
                {
                    // output to channel
                   // _client.SendRawMessage("NOTICE {0} :{1}", nick, outputString.ToString());
                   EdgeUtils.SendNotice(_client, outputString, nick);
                }
            }, EdgeUtils.HandleException);
        }
    }
}
