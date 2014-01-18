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
                            case "ore":
                                OreHandler(paramList);
                                break;
                            case "power":
                                PowerHandler(paramList);
                                break;
                            case "mytown":
                                MyTownHandler(paramList);
                                break;
                            case "voting":
                                VotingHandler(paramList);
                                break;
                            case "bugs":
                                BugRepoHandler(paramList);
                                break;
                            case "rules":
                                RulesHandler(paramList);
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
                    // add Json Parsing, Unable to get it to work... 
                   // EdgeConn.GetData(EdgeData.UrlVersion, "get", jObject =>
                    // {
                         //(string)jObject["result"].SelectToken("rr2"), (string)jObject["result"].SelectToken("ftb1")
                         EdgeUtils.SendNotice(_client, String.Format(EdgeData.JoinMessage, args.User.Nick, "2.7.6.1", "1.1.4" ), args.User.Nick);
                    // }, EdgeUtils.HandleException);
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
                var outputString = "";
                // parse the output  
                outputString = string.Concat("Username: ", (string)jObject["stats"].SelectToken("username"), " Total Bans: ", (string)jObject["stats"].SelectToken("totalbans"), " URL: ", string.Concat(EdgeData.UrlFishLink + paramList[2]));
                if (!String.IsNullOrEmpty(outputString))
                {
                    // output to channel
                   // _client.SendRawMessage("NOTICE {0} :{1}", nick, outputString.ToString());
                   EdgeUtils.SendNotice(_client, outputString, nick);
                }
            }, EdgeUtils.HandleException);
        }
        private static void OreHandler(IList<string> paramList)
        {
            EdgeUtils.SendChannel(_client, string.Concat("Ore Heightmap: " ,EdgeData.UrlOteOre));
        }

        private static void PowerHandler(IList<string> paramList)
        {
            EdgeUtils.SendChannel(_client, string.Concat("Power Conversions: ", EdgeData.UrlOtePower));
        }

        private static void VotingHandler(IList<string> paramList)
        {
            EdgeUtils.SendChannel(_client, string.Concat("Voting Information: ", EdgeData.UrlOteVoting));
        }

        private static void MyTownHandler(IList<string> paramList)
        {
            EdgeUtils.SendChannel(_client, string.Concat("My Town Info: ", EdgeData.UrlOteMyTown));
        }

        private static void BugRepoHandler(IList<string> paramList)
        {
            EdgeUtils.SendChannel(_client, string.Concat("Bug Report Info: ", EdgeData.UrlOteBugs));
        }

        private static void RulesHandler(IList<string> paramList)
        {
            EdgeUtils.SendChannel(_client, string.Concat("OTE Server Rules: ", EdgeData.UrlOteRules));
        }
        
    }
}
