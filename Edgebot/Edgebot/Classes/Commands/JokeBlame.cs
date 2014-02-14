using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("blame", "Usage: !blame <target> to show your ire!")]
    public class JokeBlame : CommandHandler
    {
        public JokeBlame()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (isIngameCommand == false)
            {
                Utils.SendChannel(paramList.Count == 1
                    ? string.Format(Data.BlameResponses[GenerateRandom(0, Data.BlameResponses.Count)], user.Nick)
                    : string.Format(Data.BlameTargetResponses[GenerateRandom(0, Data.BlameResponses.Count)],
                        paramList[1]));
            }
            else
            {
                Utils.SendChannel(Data.MessageRestrictedIrc);
            }
        }
    }
}
