using System;
using System.Collections.Generic;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("auric", "")]
    public class JokeAuric : CommandHandler
    {
        public JokeAuric()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (Utils.IsOp(user.Nick) ||
                user.Nick == "Auric" ||
                user.Nick == "Auric_Polaris")
            {
                var serverList = new List<string> {"RR1", "RR2", "Unleashed", "Pixelmon", "MagicFarm", "Horizons", "Dire20", "TPPI"};
                Utils.SendChannel(serverList[new Random().Next(0, serverList.Count)] + ": Server appears to have stopped responding");
            }
            else
            {
                Utils.SendChannel("This command is useless.");
            }
        }
    }
}
