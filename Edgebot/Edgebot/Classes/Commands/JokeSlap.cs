using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("slap", "Usage: !slap <target> to show your ire!")]
    public class JokeSlap : CommandHandler
    {
        public JokeSlap()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (isIngameCommand == false)
            {
                if (paramList.Count == 1)
                {
                    var actionString = Data.SlapActions[GenerateRandom(0, Data.SlapActions.Count)];
                    Utils.SendChannel(string.Format("{0} looks around for someone to {1}.", user.Nick, actionString.Substring(0, actionString.Length - 1)));
                }
                else
                {
                    Utils.SendChannel(string.Format(Data.MessageSlap, user.Nick, Data.SlapActions[GenerateRandom(0, Data.SlapLocations.Count)], paramList[1], Data.SlapLocations[GenerateRandom(0, Data.SlapLocations.Count)], Data.SlapItems[GenerateRandom(0, Data.SlapItems.Count)]));
                }
            }
            else
            {
                Utils.SendChannel(Data.MessageRestrictedIrc);
            }
        }
    }
}
