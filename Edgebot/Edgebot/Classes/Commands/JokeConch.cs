using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;
namespace EdgeBot.Classes.Commands
{
    [Command("conch")]
    public class JokeConch : CommandHandler
    {
        public JokeConch()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            Utils.SendChannel("All hail the magic conch!");
        }
    }
}
