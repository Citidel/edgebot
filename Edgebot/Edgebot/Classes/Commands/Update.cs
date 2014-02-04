using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("update", "")]
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
                        case "magicfarm":
                        case "mf2":
                            keyword = "mf2";
                            break;
                        case "px":
                        case "potemon":
                            keyword = "pixelmon";
                            break;
                        case "direwolf20":
                        case "dw20":
                        case "dw":
                            keyword = "dw20";
                            break;
                        case "horizons":
                        case "hz":
                            keyword = "fh1";
                            break;
                        case "unleashed":
                        case "un":
                            keyword = "fu1";
                            break;
                        case "tppi":
                            keyword = "tp1";
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
