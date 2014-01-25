using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Instances;
using EdgeBot.Classes.JSON;
using Newtonsoft.Json;

namespace EdgeBot.Classes
{
    public static class Handler
    {
        public static void CommandHelp(IList<string> paramList)
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
                    Utils.SendChannel("Usage: !help");
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

                    Utils.SendChannel(outputString);
                }
                else
                {
                    Utils.SendChannel((string)jObject["message"]);
                }
            }, Utils.HandleException);
        }

        public static void CommandDice(IList<string> paramList)
        {
            int i;
            // check if the params number 4, that the number/sides are integers, and that number and sides are both greater than 0
            if (paramList.Count() == 3 && Int32.TryParse(paramList[1], out i) && Int32.TryParse(paramList[2], out i) && (Int32.Parse(paramList[1]) > 0) && (Int32.Parse(paramList[2]) > 0))
            {
                var dice = Int32.Parse(paramList[1]);
                var sides = Int32.Parse(paramList[2]);
                var random = new Random();

                var diceList = new List<int>();
                for (var j = 0; j < dice; j++)
                {
                    diceList.Add(random.Next(1, sides));
                }

                var outputString = String.Format("Rolling a {0} sided die, {1} time{2}: {3}", sides, dice, (dice > 1) ? "s" : "", diceList.Aggregate("", (current, roll) => current + roll + " ").Trim());
                Utils.SendChannel(outputString);
            }
            else
            {
                Utils.SendChannel("Usage: !dice <number> <sides>");
            }
        }

        public static void CommandEight(ICollection<string> paramList)
        {
            if (paramList.Count > 1)
            {
                var response = Data.EightBallResponses[new Random().Next(0, Data.EightBallResponses.Count)];
                Utils.SendChannel("The magic 8 ball responds with: " + response);
            }
            else
            {
                Utils.SendChannel("No question was asked of the magic 8 ball!");
            }
        }

        public static void CommandLog(IList<string> paramList)
        {
            int i;
            // check if params number more than 4, if the pack length is less than 5 and the server is a number
            if (paramList.Count == 3 && paramList[1].Length < 5 && Int32.TryParse(paramList[2], out i))
            {
                Connection.GetData(String.Format(Data.UrlCrashLog, paramList[1], paramList[2]), "get", jObject =>
                {
                    if ((bool)jObject["success"])
                    {
                        Utils.SendChannel((string)jObject["result"]["response"]);
                    }
                    else
                    {
                        Utils.SendChannel("Failed to push crash log to pastebin. Please try again later.");
                    }
                }, Utils.HandleException);
            }
            else
            {
                Utils.SendChannel("Usage: !log <pack> <server_id>");
            }
        }

        public static void CommandMineCheck()
        {
            Connection.GetServerStatus(status =>
            {
                var message = String.Concat("MCStatus: ", Utils.FormatStatus("Accounts", status.Account), ", ", Utils.FormatStatus("Session", status.Session), ", ", Utils.FormatStatus("Auth", status.Authentication), ", ", Utils.FormatStatus("Site", status.Website), ", ", Utils.FormatStatus("Login", status.Login));
                Utils.SendChannel(message);
            }, Utils.HandleException);
        }

        public static void CommandTps(IList<string> paramList)
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
                    Utils.SendChannel(outputString.Substring(0, outputString.Length - delimiter.Length));
                }
            }, Utils.HandleException);
        }

        public static void CommandWiki(IList<string> paramList)
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
                    Utils.SendChannel("Usage: !wiki");
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

                    Utils.SendChannel(outputString);
                }
                else
                {
                    Utils.SendChannel((string)jObject["message"]);
                }
            }, Utils.HandleException);
        }

        public static void CommandCheck(IList<string> paramList, string nick)
        {
            if (!(paramList.Count() <= 1))
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

                        outputString = String.Concat(Utils.FormatText("Username: ", Colors.Bold), fishBans.Username,
                            Utils.FormatText(" Total Bans: ", Colors.Bold), Utils.FormatColor(fishBans.TotalBans, Utils.GetColorCode(fishBans.TotalBans)),
                            Utils.FormatText(" URL: ", Colors.Bold), fishBans.Url);
                    }
                    else
                    {
                        outputString = Utils.FormatText("Error: ", Colors.Bold) + (string)jObject["error"];
                    }

                    if (!String.IsNullOrEmpty(outputString))
                    {
                        Utils.SendNotice(outputString, nick);
                    }
                }, Utils.HandleException);

                Connection.GetPlayerLookup(paramList[1], bans =>
                {
                    // only report mcbans if there are bans to report
                    if (bans == null || bans.Total <= 0) return;

                    var localBans = Utils.FormatColor(bans.Local.Count, Utils.GetColorCode(bans.Local.Count));
                    var globalBans = Utils.FormatColor(bans.Global.Count, Utils.GetColorCode(bans.Global.Count));
                    var reputation = Utils.FormatColor(bans.Reputation, Utils.GetColorCode(bans.Reputation));

                    var outputString = String.Concat(Utils.FormatText("MCBans: ", Colors.Bold),
                        Utils.FormatText("Total: ", Colors.Bold), Utils.FormatColor(bans.Total, Utils.GetColorCode(bans.Total)), " (Local: ", localBans, ", Global: ", globalBans, ") ", Utils.FormatText("Rep: ", Colors.Bold), reputation,
                        Utils.FormatText(" URL: ", Colors.Bold), "http://www.mcbans.com/player/", paramList[1]);

                    Utils.SendNotice(outputString, nick);
                }, Utils.HandleException);
            }
            else { Utils.SendNotice("Usage: !check <username>", nick); }
        }

        public static void CommandMcb(IList<string> paramList, string nick)
        {
            if (paramList.Count() == 1)
            {
                Utils.SendNotice("Usage: !mcb <name> <optional:number>", nick);
            }
            else
            {
                Connection.GetPlayerLookup(paramList[1], bans =>
                {
                    if (bans.Local != null && bans.Local.Any())
                    {
                        var limit = 1;
                        int i;
                        if (paramList.Count() == 3 && Int32.TryParse(paramList[2], out i))
                        {
                            // a count is given
                            limit = int.Parse(paramList[2]);
                        }

                        for (var j = 0; j < limit; j++)
                        {
                            var localBan =
                                bans.Local[j].Replace("\r\n", "")
                                    .Replace("\r", "")
                                    .Replace("\n", "")
                                    .Replace("\0", "")
                                    .Split(' ');

                            var banReason = "";
                            for (var k = 4; k < localBan.Count(); k++)
                            {
                                banReason += localBan[k] + " ";
                            }

                            Utils.SendNotice(
                                string.Concat("Server: ", localBan[2], ", Reason: ",
                                    Utils.Truncate(banReason.Trim(), 25), " URL: ", "http://www.mcbans.com/ban/" + localBan[1]), nick);
                        }
                    }
                    else
                    {
                        Utils.SendNotice("This player does not have any local bans.", nick);
                    }
                }, Utils.HandleException);
            }
        }

        public static void CommandAnnounce(IList<string> paramList, string nick)
        {
            if (paramList.Count < 3)
            {
                Utils.SendNotice("Usage: !announce <time in seconds> <repeats> <message>", nick);
            }
            else
            {
                var msg = "";
                var timeTick = Convert.ToInt32(paramList[1]) * 1000;
                var timeCount = Convert.ToInt32(paramList[2]);
                if (timeTick == 0) return;
                Program.AnnounceTimer.Interval = timeTick;
                GC.KeepAlive(Program.AnnounceTimer);
                for (var i = 3; i < paramList.Count; i++)
                {
                    msg = msg + paramList[i] + " ";
                }
                Announcement.AnnounceMsg = msg;
                Announcement.AnnounceTimes = timeCount;
                Program.AnnounceTimer.Enabled = true;
            }
        }

        public static void CommandAuric()
        {
            var serverList = new List<string> { "RR1", "RR2", "Unleashed", "Pixelmon", "MagicFarm" };
            Utils.SendChannel(serverList[new Random().Next(0, 5)] + ": Server appears to have stopped responding");
        }

        public static void CommandUpdate(IList<string> paramList, string nick)
        {
            if (paramList.Count < 2)
            {
                Utils.SendChannel("Current Version: " + Utils.GetVersion("rr", "1"));
                Utils.SendChannel(Data.RrUpdate);
            }
            else
            {
                switch (paramList[1])
                {
                    case "rr":
                        Utils.SendChannel("Current Version: " + Utils.GetVersion("rr", "1"));
                        Utils.SendChannel(Data.RrUpdate);
                        break;

                    case "ftb":
                        Utils.SendChannel("Current Version: " + Utils.GetVersion("fu", "1"));
                        Utils.SendChannel(Data.FtbUpdate);
                        break;

                    case "px":
                        Utils.SendChannel("Current Version: " + Utils.GetVersion("px", "1"));
                        Utils.SendChannel(Data.PxUpdate);
                        break;

                    case "reload":
                        if (Utils.IsOp(nick))
                        {
                            Program.PopulateServers();
                        }
                        else
                        {
                            Utils.SendChannel(Data.MessageRestricted);
                        }
                        break;

                    default:
                        Utils.SendChannel("Current Version: " + Utils.GetVersion("rr", "1"));
                        Utils.SendChannel(Data.RrUpdate);
                        break;
                }
            }
        }

        public static void CommandDev()
        {
            // Placeholder method for any future dev related commands
        }

        public static void CommandQuote(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (paramList.Count() == 1)
            {
                // display random quote
                Connection.GetData(Data.UrlQuote, "get", jObject =>
                {
                    if ((bool)jObject["success"])
                    {
                        var quote = (string)jObject["result"].SelectToken("quote");
                        Utils.SendChannel(string.Concat("Random Quote: ", quote));
                    }
                    else
                    {
                        Utils.SendChannel("No quotes found.");
                    }
                }, Utils.HandleException);
            }
            else
            {
                if (paramList[1] == "add")
                {
                    if (isIngameCommand == false)
                    {
                        var quote = "";
                        for (var l = 2; l < paramList.Count(); l++)
                        {
                            quote = quote + paramList[l] + " ";
                        }

                        Connection.GetData(string.Format(Data.UrlQuoteAdd, user.Nick, user.Hostmask, quote.Trim()),
                            "get", jObject => Utils.SendChannel("Quote successfully added."), Utils.HandleException);
                    }
                    else
                    {
                        Utils.SendChannel("This command is restricted to the IRC channel only.");
                    }
                }
                else
                {
                    Utils.SendChannel("Usage: !quote add <message>");
                }
            }
        }

        public static void CommandSmug()
        {
            var random = new Random().Next(0, 2);
            Utils.SendChannel(Data.SmugResponses[random]);
        }

        public static void CommandSlap(IList<string> paramList, string nick)
        {
            if (paramList.Count() == 1)
            {
                Utils.SendChannel("Usage: !slap <target>");
            }
            else
            {
                var target = paramList[1];
                var random = new Random();
                Utils.SendChannel(string.Format(Data.MessageSlap, nick, Data.SlapActions[random.Next(0, Data.SlapLocations.Count)], target, Data.SlapLocations[random.Next(0, Data.SlapLocations.Count)], Data.SlapItems[random.Next(0, Data.SlapItems.Count)]));
            }
        }

        public static void CommandEdgebot(IEnumerable<string> paramList, string nick)
        {
            if (paramList.Count() == 1)
            {
                Utils.SendChannel("Usage: !edgebot update");
            }
            else
            {
                // git pull origin feature/auto-update
                var gitProc = new System.Diagnostics.Process
                {
                    EnableRaisingEvents = false,
                    StartInfo = {FileName = "git", Arguments = "pull origin feature/auto-update"}
                };

                gitProc.Start();
                Utils.SendNotice("Starting git pull process.", nick);
                gitProc.WaitForExit();
                Utils.SendNotice("Git pull process complete.", nick);

                // rm -r bin/Debug/*
                var rmProc = new System.Diagnostics.Process
                {
                    EnableRaisingEvents = false,
                    StartInfo = { FileName = "rm", Arguments = "-r /home/edgebot/Edgebot/Edgebot/bin/Debug/*" }
                };

                rmProc.Start();
                rmProc.WaitForExit();
                Utils.SendNotice("Old build files removed.", nick);

                // xbuild project
                var xbuildProc = new System.Diagnostics.Process()
                {
                    EnableRaisingEvents = true,
                    StartInfo = { FileName = "xbuild", Arguments = "/home/edgebot/Edgebot/Edgebot/Edgebot.csproj"}
                };

                xbuildProc.Start();
                Utils.SendNotice("Starting xbuild.", nick);
                xbuildProc.WaitForExit();
                Utils.SendNotice("Xbuild completed.", nick);

                xbuildProc.Exited += (sender, args) =>
                {
                    // cp bin/Debug/* bin/Release/*
                    var cpProc = new System.Diagnostics.Process
                    {
                        EnableRaisingEvents = false,
                        StartInfo = { FileName = "cp", Arguments = "/home/edgebot/Edgebot/Edgebot/bin/Debug/* /home/edgebot/Edgebot/Edgebot/bin/Release" }
                    };

                    cpProc.Start();
                    cpProc.WaitForExit();
                    Utils.SendNotice("Build files copied to Release", nick);
                };
            }
        }
    }
}
