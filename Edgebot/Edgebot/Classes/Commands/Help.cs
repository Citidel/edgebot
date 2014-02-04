using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("help", "Usage: !help to list commands, !help <command> for more information.")]
    public class Help : CommandHandler
    {
        public Help()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            switch (paramList.Count)
            {
                case 1:
                    var commands = Program.Commands.OrderBy(pair => pair.Value.Listener).Aggregate("", (current, item) => current + (item.Value.Listener + " "));
                    Utils.SendChannel("Available commands: " + commands.Trim());
                    break;

                case 2:
                    HandleHelp(paramList);
                    break;
            }
        }
    }
}
