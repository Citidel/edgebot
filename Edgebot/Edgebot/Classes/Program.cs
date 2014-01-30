using ChatSharp;
using ChatSharp.Events;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;
using EdgeBot.Classes.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Timers;

namespace EdgeBot.Classes
{
    static class Program
    {
        public static Timer AnnounceTimer;
        public static IrcClient Client;

        public static string McBansApiUrl = "";
        private static string _nickServAuth = "";
        private static string _commandPrefix = "";

        public static readonly List<Server> ServerList = new List<Server>();
        public static readonly Dictionary<string, Command> Commands = new Dictionary<string, Command>();
        private static readonly List<Blacklist> BlackList = new List<Blacklist>();

        static void Main(string[] argArray)
        {
            InitClasses();

            if (argArray.Any()) _nickServAuth = argArray[0];

            //set the command prefix to $ if debug mode
            _commandPrefix = string.IsNullOrEmpty(_nickServAuth) ? "$" : "!";

            Client = (!string.IsNullOrEmpty(_nickServAuth)) ? new IrcClient(Config.Host, new IrcUser(Config.Nickname, Config.Username)) : new IrcClient(Config.Host, new IrcUser(Config.NickTest, Config.UserTest));
            Client.NetworkError += OnNetworkError;
            Client.ConnectionComplete += OnConnectionComplete;
            Client.UserMessageRecieved += OnUserMessageRecieved;
            Client.ChannelMessageRecieved += OnChannelMessageRecieved;
            Client.UserJoinedChannel += OnUserJoinedChannel;

            AnnounceTimer = new Timer();
            AnnounceTimer.Elapsed += OnTimedEvent;

            if (string.IsNullOrEmpty(_nickServAuth))
            {
                Utils.Log("Warning, nick serv authentication password is empty.");
            }

            Utils.Log("Connecting to IRC...");
            Client.ConnectAsync();
            Console.ReadLine();
        }

        private static void InitClasses()
        {
            var classes =
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(type => type.IsClass && type.BaseType.Name == "CommandHandler");

            foreach (var cls in classes)
            {
                var attrib = cls.GetCustomAttributes(typeof(Command), true).FirstOrDefault() as Command;
                Commands.Add(cls.FullName, attrib);
            }
        }

        private static void OnNetworkError(object s, SocketErrorEventArgs e)
        {
            Utils.Log("Error: " + e.SocketError);
        }

        private static void OnUserJoinedChannel(object sender, ChannelUserEventArgs args)
        {
            Utils.SendNotice(String.Format(Data.MessageJoinChannel, args.User.Nick), args.User.Nick);
        }

        private static void OnUserMessageRecieved(object s, PrivateMessageEventArgs e)
        {
            if (!Utils.IsDev(e.PrivateMessage.User.Nick)) return;
            if (e.PrivateMessage.Message.StartsWith(_commandPrefix + "msg "))
                Utils.SendChannel(e.PrivateMessage.Message.Substring(5));
        }

        private static void OnChannelMessageRecieved(object sender, PrivateMessageEventArgs args)
        {
            var isIngameCommand = false;
            var message = args.PrivateMessage.Message;
            var paramList = message.Split(' ');

            if (args.PrivateMessage.User.Nick == "RR1" || args.PrivateMessage.User.Nick == "RR2")
            {
                var ingameMessage = args.PrivateMessage.Message.Split(':');
                if (ingameMessage.Any())
                {
                    try
                    {
                        if (ingameMessage[1].StartsWith(" " + _commandPrefix))
                        {
                            paramList = ingameMessage[1].Trim().Split(' ');
                            isIngameCommand = true;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (args.PrivateMessage.Message.StartsWith(_commandPrefix) || paramList[0].StartsWith(_commandPrefix))
            {
                // Only listen to people who are not blacklisted
                if (BlackList.All(item => item.Ip != args.PrivateMessage.User.Hostname) || Utils.IsAdmin(args.PrivateMessage.User.Nick) || Utils.IsOp(args.PrivateMessage.User.Nick))
                {
                    foreach (var type in Commands.Where(cmd => cmd.Value.Listener == paramList[0].Substring(1)).Select(cmd => Type.GetType(cmd.Key)).Where(type => type != null))
                    {
                        ((CommandHandler)Activator.CreateInstance(type)).HandleCommand(paramList, args.PrivateMessage.User, isIngameCommand);
                    }
                }
                else
                {
                    Utils.SendNotice("You have been blacklisted from this bot. Refer to a staff member if you feel this is in error.", args.PrivateMessage.User.Nick);
                }
            }

            //listen for www or http(s)
            if (!string.IsNullOrEmpty(_nickServAuth))
            {
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
            }

            if (args.PrivateMessage.Message.StartsWith(_commandPrefix))
            {
                Utils.Log("<{0}> {1}", args.PrivateMessage.User.Nick, args.PrivateMessage.Message);
            }
        }

        private static void OnConnectionComplete(object s, EventArgs e)
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
            PopulateBlacklist();
            McBansApiUrl = GetApiServer();
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
            ServerList.Clear();
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

        public static void PopulateBlacklist()
        {
            BlackList.Clear();
            Connection.GetData(Data.UrlBlacklist, "get", jObject =>
            {
                if ((bool)jObject["success"])
                {
                    foreach (var row in jObject["result"])
                    {
                        BlackList.Add(new Blacklist { Ip = (string)row["ip"] });
                    }

                    Utils.Log("Bot Blacklist retrieved from API");
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
