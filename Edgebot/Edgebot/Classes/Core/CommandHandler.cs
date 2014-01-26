using System.Collections.Generic;
using ChatSharp;

namespace EdgeBot.Classes.Core
{
    public abstract class CommandHandler
    {
        public abstract void HandleCommand(IList<string> paramList, IrcUser user);
    }
}
