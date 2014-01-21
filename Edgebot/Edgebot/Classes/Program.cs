using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Instances;

namespace EdgeBot.Classes
{
    class Program
    {
        public static Timer AnnounceTimer;
        public static IrcClient Client;
        public static readonly List<Server> ServerList = new List<Server>();
        public static string NickServAuth = "";

        static void Main(string[] argArray)
        {
            if (argArray.Any()) NickServAuth = argArray[0];
            AnnounceTimer = new Timer();

            Client = (!string.IsNullOrEmpty(NickServAuth)) ? new IrcClient(Config.Host, new IrcUser(Config.Nickname, Config.Username)) : new IrcClient(Config.Host, new IrcUser(Config.NickTest, Config.UserTest));
            Client.NetworkError += (s, e) => Utils.Log("Error: " + e.SocketError);
            Client.ConnectionComplete += (s, e) =>
            {
                Utils.Log("Connection complete.");
                if (!string.IsNullOrEmpty(NickServAuth))
                {
                    Utils.Log("Sending ident message to NickServ");
                    Utils.SendPm(string.Format("IDENTIFY EdgeBot {0}", NickServAuth), "NickServ");
                }
                else
                {
                    Utils.Log("No NickServ authentication detected.");
                    Client.JoinChannel(Config.Channel);
                }

                Client.RawMessageRecieved += (sender, args) =>
                {
                    if (args.Message != Data.IdentifiedMessage)
                        return;
                    Utils.Log("NickServ authentication was successful.");
                    Client.JoinChannel(Config.Channel);
                    Utils.Log("Joining channel: {0}", Config.Channel);
                };

                // pull addresses from api
                Connection.GetData(Data.UrlAddress, "get", jObject =>
                {
                    if ((bool)jObject["success"])
                    {
                        foreach (var server in jObject["result"].Select(row => new Server
                        {
                            ShortCode = (string)row["short_code"],
                            Id = (string)row["server"],
                            Address = (string)row["address"],
                            Version = (string)row["version"]
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

            Client.ChannelMessageRecieved += (sender, args) =>
            {
                // Only listen to !commands
                //if (!Utils.IsDev(args.PrivateMessage.User.Nick) || !args.PrivateMessage.Message.StartsWith("!")) return;
                if (!args.PrivateMessage.Message.StartsWith("!")) return;
                var message = args.PrivateMessage.Message;
                var paramList = message.Split(' ');
                switch (paramList[0].Substring(1))
                {
                    // !tps
                    case "tps":
                        if (Utils.IsOp(args.PrivateMessage.User.Nick))
                        {
                            Handler.TpsHandler(paramList);
                        }
                        else
                        {
                            Utils.SendChannel("This command is restricted to ops only.");
                        }
                        break;

                    // !wiki <keyword>
                    case "wiki":
                        Handler.WikiHandler(paramList);
                        break;

                    // !check <username>
                    case "check":
                        if (Utils.IsOp(args.PrivateMessage.User.Nick))
                        {
                            Handler.FishHandler(paramList, args.PrivateMessage.User.Nick);
                        }
                        else
                        {
                            Utils.SendChannel("This command is restricted to ops only.");
                        }
                        break;

                    // !announce <time in seconds> <repeats> <message>
                    case "announce":
                        Handler.AnnounceHandler(paramList, args.PrivateMessage.User.Nick);
                        break;

                    // !update
                    case "update":
                        Handler.UpdateHandler(paramList);
                        break;

                    // !minecheck | !minestatus
                    case "minecheck":
                    case "minestatus":
                        Handler.MineCheckHandler();
                        break;

                    // !log <pack> <server>
                    case "log":
                        if (Utils.IsOp(args.PrivateMessage.User.Nick))
                        {
                            Handler.LogHandler(paramList);
                        }
                        else
                        {
                            Utils.SendChannel("This command is restricted to ops only.");
                        }
                        break;

                    // !8 <question>
                    case "8":
                        Handler.EightBallHandler(paramList);
                        break;

                    // !dice <number> <sides>
                    case "dice":
                        Handler.DiceHandler(paramList);
                        break;

                    // !help, !help <keyword>
                    case "help":
                        Handler.HelpHandler(paramList);
                        break;

                    case "dev":
                        Handler.DevHandler(paramList);
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
                            if (!string.IsNullOrEmpty(title)) { Utils.SendChannel("URL TITLE: " + title); } else { Utils.Log("Connection: Result is null"); }
                        }, Utils.HandleException);
                    }
                }

                if (args.PrivateMessage.Message.StartsWith("!"))
                {
                    Utils.Log("<{0}> {1}", args.PrivateMessage.User.Nick, args.PrivateMessage.Message);
                }
            };

            //_client.ChannelMessageRecieved += (sender, args) => Utils.Log("<{0}> {1}", args.PrivateMessage.User.Nick, args.PrivateMessage.Message);
            Client.UserJoinedChannel += (sender, args) =>
            {
                if (Utils.IsDev(args.User.Nick))
                {
                    Utils.SendNotice(String.Format(Data.JoinMessage, args.User.Nick, "2.7.6.1", "1.1.4"), args.User.Nick);
                }
            };

            AnnounceTimer.Elapsed += OnTimedEvent;

            if (string.IsNullOrEmpty(NickServAuth))
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
            if (Convert.ToInt32(Data.AnnounceTimes) == 0)
            {
                AnnounceTimer.Enabled = false;
            }
            else
            {
                var count = Convert.ToInt32(Data.AnnounceTimes);
                count--;
                Data.AnnounceTimes = count;
                Utils.SendChannel(Data.AnnounceMsg.ToString());
            }
        }
    }
}
