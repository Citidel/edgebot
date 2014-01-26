using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [Command("slap")]
    public class JokeSlap : CommandHandler
    {
        public JokeSlap()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (isIngameCommand == false)
            {
                if (paramList.Count() == 1)
                {
                    Utils.SendChannel("Usage: !slap <target>");
                }
                else
                {
                    var target = paramList[1];
                    var random = new Random();
                    Utils.SendChannel(string.Format(Data.MessageSlap, user.Nick, Data.SlapActions[random.Next(0, Data.SlapLocations.Count)], target, Data.SlapLocations[random.Next(0, Data.SlapLocations.Count)], Data.SlapItems[random.Next(0, Data.SlapItems.Count)]));
                }
            }
            else
            {
                Utils.SendChannel("This command is restricted to the IRC channel only.");
            }
        }
    }
}
