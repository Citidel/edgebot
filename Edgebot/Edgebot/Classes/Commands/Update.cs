using System.Collections.Generic;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [Command("update")]
    public class Update : CommandHandler
    {
        public Update()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
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
                        if (Utils.IsOp(user.Nick))
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
    }
}
