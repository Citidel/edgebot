using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [Command("mcb")]
    public class McBans : CommandHandler
    {
        public McBans()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (Utils.IsOp(user.Nick))
            {
                if (paramList.Count() == 1)
                {
                    Utils.SendNotice("Usage: !mcb <name> <optional:number>", user.Nick);
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
                                        Utils.Truncate(banReason.Trim(), 25), " URL: ", "http://www.mcbans.com/ban/" + localBan[1]), user.Nick);
                            }
                        }
                        else
                        {
                            Utils.SendNotice("This player does not have any local bans.", user.Nick);
                        }
                    }, Utils.HandleException);
                }
            }
            else
            {
                Utils.SendChannel(Data.MessageRestricted);
            }
        }
    }
}
