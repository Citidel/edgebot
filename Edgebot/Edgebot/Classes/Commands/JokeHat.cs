using System.Collections.Generic;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("hat", "Humorous command to magically produce hats")]
    public class JokeHat : CommandHandler
    {
        public JokeHat()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            Utils.SendChannel(isIngameCommand == false
                ? string.Format("{0} their hand into a giant top hat and produces {1}",
                    Data.HatActions[GenerateRandom(0, Data.HatActions.Count)],
                    Data.HatItems[GenerateRandom(0, Data.HatItems.Count)])
                : Data.MessageRestrictedIrc);
        }
    }
}
