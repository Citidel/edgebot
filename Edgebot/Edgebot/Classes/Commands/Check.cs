using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;
using EdgeBot.Classes.Instances;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("check", "")]
    public class Fishbans : CommandHandler
    {
        public Fishbans()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (Utils.IsOp(user.Nick))
            {
                if (!(paramList.Count <= 1))
                {
                    var url = Data.UrlFish + paramList[1];
                    Connection.GetData(url, "get", jObject =>
                    {
                        string outputString;
                        if ((bool) jObject["success"])
                        {
                            var fishBans = new FishBans
                            {
                                TotalBans = (int) jObject["stats"].SelectToken("totalbans"),
                                Url = Data.UrlFishLink + paramList[1],
                                Username = (string) jObject["stats"].SelectToken("username")
                            };

                            outputString = String.Concat(Utils.FormatText("Username: ", Colors.Bold), fishBans.Username,
                                Utils.FormatText(" Total Bans: ", Colors.Bold),
                                Utils.FormatColor(fishBans.TotalBans, Utils.GetColorCode(fishBans.TotalBans)),
                                Utils.FormatText(" URL: ", Colors.Bold), fishBans.Url);
                        }
                        else
                        {
                            outputString = Utils.FormatText("Error: ", Colors.Bold) + (string) jObject["error"];
                        }

                        if (!String.IsNullOrEmpty(outputString))
                        {
                            Utils.SendNotice(outputString, user.Nick);
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
                            Utils.FormatText("Total: ", Colors.Bold),
                            Utils.FormatColor(bans.Total, Utils.GetColorCode(bans.Total)), " (Local: ", localBans,
                            ", Global: ", globalBans, ") ", Utils.FormatText("Rep: ", Colors.Bold), reputation,
                            Utils.FormatText(" URL: ", Colors.Bold), "http://www.mcbans.com/player/", paramList[1]);

                        Utils.SendNotice(outputString, user.Nick);
                    }, Utils.HandleException);
                }
                else
                {
                    Utils.SendNotice("Usage: !check <username>", user.Nick);
                }
            }
            else
            {
                Utils.SendChannel(Data.MessageRestricted);
            }
        }
    }
}
