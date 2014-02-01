using System.Collections.Generic;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [Command("nospam")]
    public class noSpam : CommandHandler
    {
        public noSpam()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (Utils.IsOp(user.Nick) == true)
            {
                if (Program.isLocked == false)
                {
                    Program.isLocked = true;
                    Utils.SendChannel("Bot command are now locked to Ops Only.");
                }
                else
                {
                    Program.isLocked = false;
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
