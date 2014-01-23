using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Timers;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Instances;

namespace EdgeBot.Classes
{
    static class Program
    {
        public static Timer AnnounceTimer;
        public static IrcClient Client;
        public static readonly List<Server> ServerList = new List<Server>();
        public static string McBansApiUrl = "";
        private static string _nickServAuth = "";

        static void Main(string[] argArray)
        {
            try
            {
                _nickServAuth = argArray[0];
            }
            catch (Exception)
            {
                _nickServAuth = "";
            }

            AnnounceTimer = new Timer();

            Client = (!string.IsNullOrEmpty(_nickServAuth)) ? new IrcClient(Config.Host, new IrcUser(Config.Nickname, Config.Username)) : new IrcClient(Config.Host, new IrcUser(Config.NickTest, Config.UserTest));
            Client.NetworkError += (s, e) => Utils.Log("Error: " + e.SocketError);
            Client.ConnectionComplete += (s, e) =>
            {
                Utils.Log("Connection complete.");
                if (!string.IsNullOrEmpty(_nickServAuth))
                {
                    Utils.Log("Sending ident message to NickServ");
                    Utils.SendPm(string.Format("IDENTIFY EdgeBot {0}", _nickServAuth), "NickServ");
                }
                else
                {
                    Utils.Log("No NickServ authentication detected.");
                    JoinChannel();
                }

                Client.RawMessageRecieved += (sender, args) =>
                {
                    if (args.Message != Data.MessageIdentified)
                        return;
                    Utils.Log("NickServ authentication was successful.");
                    JoinChannel();
                };

                PopulateServers();
                McBansApiUrl = GetApiServer();
            };

            Client.ChannelMessageRecieved += (sender, args) =>
            {
                // Only listen to !commands
                //if (!Utils.IsDev(args.PrivateMessage.User.Nick) || !args.PrivateMessage.Message.StartsWith("!")) return;
                var message = args.PrivateMessage.Message;
                var paramList = message.Split(' ');
                if (args.PrivateMessage.Message.StartsWith("!"))
                {
                    switch (paramList[0].Substring(1))
                    {
                        // !tps
                        case "tps":
                            if (Utils.IsOp(args.PrivateMessage.User.Nick))
                            {
                                Handler.CommandTps(paramList);
                            }
                            else
                            {
                                Utils.SendChannel(Data.MessageRestricted);
                            }
                            break;

                        // !wiki <keyword>
                        case "wiki":
                            Handler.CommandWiki(paramList);
                            break;

                        // !check <username>
                        case "check":
                            if (Utils.IsOp(args.PrivateMessage.User.Nick))
                            {
                                Handler.CommandCheck(paramList, args.PrivateMessage.User.Nick);
                            }
                            else
                            {
                                Utils.SendChannel(Data.MessageRestricted);
                            }
                            break;

                        // !announce <time in seconds> <repeats> <message>
                        case "announce":
                            if (Utils.IsOp(args.PrivateMessage.User.Nick))
                            {
                            Handler.CommandAnnounce(paramList, args.PrivateMessage.User.Nick);
                            }
                            else
                            {
                                Utils.SendChannel(Data.MessageRestricted);
                            }
                            break;

                        // !update
                        case "update":
                            Handler.CommandUpdate(paramList, args.PrivateMessage.User.Nick);
                            break;

                        // !minecheck | !minestatus
                        case "minecheck":
                        case "minestatus":
                            Handler.CommandMineCheck();
                            break;

                        // !log <pack> <server>
                        case "log":
                            if (Utils.IsOp(args.PrivateMessage.User.Nick))
                            {
                                Handler.CommandLog(paramList);
                            }
                            else
                            {
                                Utils.SendChannel(Data.MessageRestricted);
                            }
                            break;

                        // !8 <question>
                        case "8":
                            Handler.CommandEight(paramList);
                            break;

                        // !dice <number> <sides>
                        case "dice":
                            Handler.CommandDice(paramList);
                            break;

                        // !help, !help <keyword>
                        case "help":
                            Handler.CommandHelp(paramList);
                            break;

                        // !dev
                        case "dev":
                            if (Utils.IsDev(args.PrivateMessage.User.Nick) ||
                                Utils.IsAdmin(args.PrivateMessage.User.Nick))
                            {
                                Handler.CommandDev();
                            }
                            else
                            {
                                Utils.SendChannel("This command is restricted to developers or server admins only.");
                            }
                            break;

                        // !smug
                        case "smug":
                            if (Utils.IsOp(args.PrivateMessage.User.Nick) || args.PrivateMessage.User.Nick == "DrSmugleaf" || args.PrivateMessage.User.Nick == "DrSmugleaf_")
                            {
                                Handler.CommandSmug();
                            }
                            else
                            {
                                Utils.SendChannel("This command is useless.");
                            }
                            break;

                        // !slap
                        case "slap":
                            Handler.CommandSlap(paramList, args.PrivateMessage.User.Nick);
                            break;

                        // !quote add <quote> | !quote
                        case "quote":
                            Handler.CommandQuote(paramList, args.PrivateMessage.User);
                            break;
                    }
                }

                //listen for www or http(s)
                if (args.PrivateMessage.Message.Contains("http://") || args.PrivateMessage.Message.Contains("https://") || args.PrivateMessage.Message.Contains("www."))
                {
                    for (var i = 0; i < paramList.Count(); i++)
                    {
                        var url = "";
                        if (paramList[i].Contains("http://") || paramList[i].Contains("https://"))
                        {
                            url = paramList[i];
                        }
                        else if (paramList[i].Contains("www."))
                        {
                            url = string.Concat("http://", paramList[i]);
                        }

                        if (!string.IsNullOrEmpty(url))
                        {
                            Connection.GetLinkTitle(url, title =>
                            {
                                if (!string.IsNullOrEmpty(title))
                                {
                                    Utils.SendChannel("URL TITLE: " + title);
                                }
                                else
                                {
                                    Utils.Log("Connection: Result is null");
                                }
                            }, Utils.HandleException);
                        }
                    }
                }

                if (args.PrivateMessage.Message.StartsWith("!"))
                {
                    Utils.Log("<{0}> {1}", args.PrivateMessage.User.Nick, args.PrivateMessage.Message);
                }

                //Utils.Log("<{0}> {1}", args.PrivateMessage.User.Nick, args.PrivateMessage.Message);
            };

            //_client.ChannelMessageRecieved += (sender, args) => Utils.Log("<{0}> {1}", args.PrivateMessage.User.Nick, args.PrivateMessage.Message);
            Client.UserJoinedChannel += (sender, args) => Utils.SendNotice(String.Format(Data.MessageJoinChannel, args.User.Nick, Utils.GetVersion("rr", "1"), Utils.GetVersion("fu", "1")), args.User.Nick);

            AnnounceTimer.Elapsed += OnTimedEvent;

            if (string.IsNullOrEmpty(_nickServAuth))
            {
                Utils.Log("Warning, nick serv authentication password is empty.");
            }

            Utils.Log("Connecting to IRC...");
            Client.ConnectAsync();
            while (true)
            {
            }
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (Convert.ToInt32(Announcement.AnnounceTimes) == 0)
            {
                AnnounceTimer.Enabled = false;
            }
            else
            {
                var count = Convert.ToInt32(Announcement.AnnounceTimes);
                count--;
                Announcement.AnnounceTimes = count;
                Utils.SendChannel(Announcement.AnnounceMsg.ToString());
            }
        }

        public static void PopulateServers()
        {
            // pull addresses from api
            Connection.GetData(Data.UrlAddress, "get", jObject =>
            {
                if ((bool)jObject["success"])
                {
                    foreach (var row in jObject["result"])
                    {
                        ServerList.Add(new Server { Address = (string)row["address"], ShortCode = (string)row["short_code"], Id = (string)row["server"], Version = (string)row["version"] });
                    }
                    
                    Utils.Log("Server addresses retrieved from API");
                }
                else
                {
                    Utils.Log("Failed to query for servers.");
                }
            }, Utils.HandleException);
        }

        private static string GetApiServer()
        {
            var ping = new Ping();
            var pingList = new Dictionary<string, long>();
            foreach (var server in Data.McBansApiServerList)
            {
                var pingReturn = ping.Send(server);
                if (pingReturn != null) pingList.Add(server, pingReturn.RoundtripTime);
            }

            return pingList.Where(pair => pair.Value == pingList.Values.Min()).Select(pair => pair.Key).First();
        }

        private static void JoinChannel()
        {
            Client.JoinChannel(Config.Channel);
            Utils.Log("Joining channel: {0}", Config.Channel);
        }
    }
}
