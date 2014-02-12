using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("dev", "")]
    public class Dev : CommandHandler
    {
        public Dev()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand, string truncatedUser)
        {
            if (!Utils.IsDev(user.Nick)) return;
        }
    }
}
