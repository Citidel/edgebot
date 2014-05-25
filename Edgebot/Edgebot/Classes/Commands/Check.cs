using System;
using System.Collections.Generic;
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
                        var detailedBanInfo = "";
                        if ((bool)jObject["success"])
                        {
                            var fishBans = new FishBans
                            {
                                TotalBans = (int)jObject["stats"].SelectToken("totalbans"),
                                Url = Data.UrlFishLink + paramList[1],
                                Username = (string)jObject["stats"].SelectToken("username")
                            };

                            outputString = String.Concat(Utils.FormatText("Username: ", Colors.Bold), fishBans.Username,
                                Utils.FormatText(" Total Bans: ", Colors.Bold),
                                Utils.FormatColor(fishBans.TotalBans, Utils.GetColorCode(fishBans.TotalBans)),
                                Utils.FormatText(" URL: ", Colors.Bold), fishBans.Url);

                            Connection.GetData(string.Format(Data.UrlBanCheck, paramList[1]), "get", apiObject =>
                            {
                                if ((string)apiObject["result"].SelectToken("bans") == "true")
                                {
                                    detailedBanInfo = "OTEAPI: Bans found - http://dev.otegamers.com/edge/lookup/" +
                                                      fishBans.Username;
                                }
                                else
                                {
                                    detailedBanInfo = "OTEAPI: No bans found.";
                                }

                                Utils.SendNotice(detailedBanInfo, user.Nick);
                            }, Utils.HandleException);


                        }
                        else
                        {
                            outputString = Utils.FormatText("Error: ", Colors.Bold) + (string)jObject["error"];
                        }

                        if (String.IsNullOrEmpty(outputString)) return;
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
