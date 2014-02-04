using System;
using System.Collections.Generic;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("8")]
    public class JokeEight : CommandHandler
    {
        public JokeEight()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (paramList.Count > 1)
            {
                var response = Data.EightBallResponses[new Random().Next(0, Data.EightBallResponses.Count)];
                Utils.SendChannel("The magic 8 ball responds with: " + response);
            }
            else
            {
                Utils.SendChannel("No question was asked of the magic 8 ball!");
            }
        }
    }
}
