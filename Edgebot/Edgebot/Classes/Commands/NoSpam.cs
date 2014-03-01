using System.Collections.Generic;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("lock", "")]
    public class NoSpam : CommandHandler
    {
        public NoSpam()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (Utils.IsOp(user.Nick) || Utils.IsAdmin(user.Nick))
            {
                if (Program.IsLocked == false)
                {
                    Program.IsLocked = true;
                    Utils.SendChannel("Bot command are now locked to Ops Only.");
                }
                else
                {
                    Program.IsLocked = false;
                    Utils.SendChannel("Bot commands are unlocked.");
                }
            }
            else
            {
                Utils.SendChannel(Data.MessageRestricted);
            }
        }
    }
}
