using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;

namespace EdgeBot.Classes.Core
{
    public abstract class CommandHandler
    {
        public abstract void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand);

        protected static void HandleHelp(IList<string> paramList)
        {
            var exists = false;
            foreach (var help in Program.Commands.Where(cmd => cmd.Value.Listener == paramList[1]).Select(cmd => cmd.Value.Help))
            {
                Utils.SendChannel(!string.IsNullOrEmpty(help) ? help : Data.MessageRestricted);
                exists = true;
            }

            if (!exists)
            {
                Utils.SendChannel("Command not found.");
            }
        }
    }
}
