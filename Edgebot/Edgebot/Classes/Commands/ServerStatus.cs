using System;
using System.Collections.Generic;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("minecheck", "Displays Minecraft-related status information")]
    public class ServerStatus : CommandHandler
    {
        public ServerStatus()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            Connection.GetServerStatus(status =>
            {
                var message = "MCStatus: Accounts[";
                switch (status.Account)
                {
                    case true:
                        message = string.Concat(message, Utils.FormatStatus("U", true));
                        break;
                    case false:
                        message = string.Concat(message, Utils.FormatStatus("D", false));
                        break;

                }
                message = string.Concat(message, "] Session[");
                switch (status.Session)
                {
                    case true:
                        message = string.Concat(message, Utils.FormatStatus("U", true));
                        break;
                    case false:
                        message = string.Concat(message, Utils.FormatStatus("D", false));
                        break;

                }
                message = string.Concat(message, "] Auth[");
                switch (status.Authentication)
                {
                    case true:
                        message = string.Concat(message, Utils.FormatStatus("U", true));
                        break;
                    case false:
                        message = string.Concat(message, Utils.FormatStatus("D", false));
                        break;

                }
                message = string.Concat(message, "] Site[");
                switch (status.Website)
                {
                    case true:
                        message = string.Concat(message, Utils.FormatStatus("U", true));
                        break;
                    case false:
                        message = string.Concat(message, Utils.FormatStatus("D", false));
                        break;

                }
                message = string.Concat(message, "] Login[");
                switch (status.Login)
                {
                    case true:
                        message = string.Concat(message, Utils.FormatStatus("U", true));
                        break;
                    case false:
                        message = string.Concat(message, Utils.FormatStatus("D", false));
                        break;

                }
                message = string.Concat(message, "]");

                Utils.SendChannel(message);
            }, Utils.HandleException);
        }
    }
}