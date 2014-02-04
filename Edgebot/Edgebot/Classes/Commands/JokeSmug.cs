using System;
using System.Collections.Generic;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("smug", "This command is useless")]
    public class JokeSmug : CommandHandler
    {
        public JokeSmug()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (Utils.IsOp(user.Nick) ||
                user.Nick == "DrSmugleaf" ||
                user.Nick == "DrSmugleaf_")
            {
                var random = new Random().Next(0, 2);
                Utils.SendChannel(Data.SmugResponses[random]);
            }
            else
            {
                Utils.SendChannel("This command is useless.");
            }
        }
    }
}
