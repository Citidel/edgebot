using System;
using System.Collections.Generic;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [Command("log")]
    public class ServerLog : CommandHandler
    {
        public ServerLog()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (Utils.IsOp(user.Nick)|Utils.IsAdmin(user.Nick))
            {
                int i;
                // check if params number more than 4, if the pack length is less than 5 and the server is a number
                if (paramList.Count == 3 && paramList[1].Length < 5 && Int32.TryParse(paramList[2], out i))
                {
                    Connection.GetData(String.Format(Data.UrlCrashLog, paramList[1], paramList[2]), "get", jObject =>
                    {
                        if ((bool) jObject["success"])
                        {
                            Utils.SendChannel((string) jObject["result"]["response"]);
                        }
                        else
                        {
                            Utils.SendChannel("Failed to push crash log to pastebin. Please try again later.");
                        }
                    }, Utils.HandleException);
                }
                else
                {
                    Utils.SendChannel("Usage: !log <pack> <server_id>");
                }
            }
            else
            {
                Utils.SendChannel(Data.MessageRestricted);
            }
        }
    }
}
