using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("hat")]
    public class JokeHat : CommandHandler
    {
        public JokeHat()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (isIngameCommand == false)
            {
                var random = new Random();
                Utils.SendChannel(string.Format("{0} their hand into a giant top hat and produces {1}", Data.HatActions[random.Next(0, Data.HatActions.Count())], Data.HatItems[random.Next(0, Data.HatItems.Count())]));
            }
            else
            {
                Utils.SendChannel("This command is restricted to the IRC channel only.");
            }
        }
    }
}
