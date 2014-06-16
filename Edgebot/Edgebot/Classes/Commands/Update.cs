using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("update", "Usage: !update <server>, !update list or !update to view valid servers")]
    public class Update : CommandHandler
    {
        public Update()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (paramList.Count == 1 || paramList.Count > 3)
            {
                var outputString = Data.UpdateDict.Aggregate("Valid servers: ", (current, item) => current + (item.Value.Key + " "));
                Utils.SendChannel("Usage: !update <server>. " + outputString.Trim());
                return;
            }

            var keyword = paramList[1];
            switch (keyword)
            {
                case "reload":
                    if (Utils.IsOp(user.Nick))
                    {
                        Utils.SendNotice("Reloading server list.", user.Nick);
                        Program.PopulateServers();
                    }
                    else
                    {
                        Utils.SendChannel(Data.MessageRestricted);
                    }
                    break;

                case "list":
                    var outputString = Data.UpdateDict.Aggregate("Valid servers: ", (current, item) => current + (item.Value.Key + " "));
                    Utils.SendChannel(outputString.Trim());
                    break;

                default:
                    switch (keyword.ToLower())
                    {
                        case "resonant":
                        case "rr1":
                        case "rr2":
                            keyword = "rr";
                            break;

                        case "sky":
                        case "skyblock":
                        case "skyfactory":
                            keyword = "sky";
                            break;

                        case "yogs":
                        case "yogscomplete":
                            keyword = "yogs";
                            break;

                        case "pvp":
                        case "crackpack":
                            keyword = "pvp";
                            break;
                    }
                    var exists = false;
                    foreach (var item in Data.UpdateDict.Where(item => item.Value.Key == keyword))
                    {
                        Utils.SendChannel(string.Format(Data.MessageUpdate, Utils.GetVersion(item.Key.Key, item.Key.Value), item.Value.Value));
                        exists = true;
                    }

                    if (!exists)
                    {
                        Utils.SendChannel("Invalid server. Type !update list for a list of valid servers.");
                    }
                    break;
            }
        }
    }
}
