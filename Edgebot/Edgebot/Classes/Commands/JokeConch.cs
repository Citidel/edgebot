using System.Collections.Generic;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("conch", "Humorous command referring to TV show")]
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
