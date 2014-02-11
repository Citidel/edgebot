using System.Collections.Generic;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;
using System;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("poop", "This command is useless.")]
    public class JokeVelo : CommandHandler
    {
        public JokeVelo()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (Utils.IsOp(user.Nick) ||
                user.Nick == "Velotican" ||
                user.Nick == "Velo|PackDev" ||
                user.Nick == "Velo|Food")
            {
                var random = new Random().Next(0, Data.GamerPoop.Count);
                Utils.SendChannel("http://www.youtube.com/watch?v="+Data.GamerPoop[random]);
            }
            else
            {
                Utils.SendChannel("This command is useless.");
            }
        }
    }
}
