using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("slap")]
    public class JokeSlap : CommandHandler
    {
        public JokeSlap()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (isIngameCommand == false)
            {
                var random = new Random();
                if (paramList.Count() == 1)
                {
                    var actionString = Data.SlapActions[random.Next(0, Data.SlapActions.Count)];
                    Utils.SendChannel(string.Format("{0} looks around for someone to {1}.", user.Nick, actionString.Substring(0, actionString.Length - 1)));
                }
                else
                {
                    Utils.SendChannel(string.Format(Data.MessageSlap, user.Nick, Data.SlapActions[random.Next(0, Data.SlapLocations.Count)], paramList[1], Data.SlapLocations[random.Next(0, Data.SlapLocations.Count)], Data.SlapItems[random.Next(0, Data.SlapItems.Count)]));
                }
            }
            else
            {
                Utils.SendChannel("This command is restricted to the IRC channel only.");
            }
        }
    }
}
