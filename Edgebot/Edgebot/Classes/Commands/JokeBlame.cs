using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [Command("blame")]
    public class JokeBlame : CommandHandler
    {
        public JokeBlame()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (isIngameCommand == false)
            {
                var random = new Random();
                Utils.SendChannel(paramList.Count() == 1
                    ? string.Format(Data.BlameResponses[random.Next(0, Data.BlameResponses.Count)], user.Nick)
                    : string.Format(Data.BlameTargetResponses[random.Next(0, Data.BlameTargetResponses.Count)],
                        paramList[1]));
            }
            else
            {
                Utils.SendChannel("This command is restricted to the IRC channel only.");
            }
        }
    }
}
