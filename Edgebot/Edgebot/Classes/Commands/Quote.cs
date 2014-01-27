using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [Command("quote")]
    public class Quote : CommandHandler
    {
        public Quote()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (paramList.Count() == 1)
            {
                // display random quote
                Connection.GetData(Data.UrlQuote, "get", jObject =>
                {
                    if ((bool)jObject["success"])
                    {
                        Utils.SendChannel((string)jObject["result"].SelectToken("quote"));
                    }
                    else
                    {
                        Utils.SendChannel("No quotes found.");
                    }
                }, Utils.HandleException);
            }
            else
            {
                if (paramList[1] == "add")
                {
                    if (isIngameCommand == false)
                    {
                        var quote = "";
                        for (var l = 2; l < paramList.Count(); l++)
                        {
                            quote = quote + paramList[l] + " ";
                        }

                        Connection.GetData(string.Format(Data.UrlQuoteAdd, user.Nick, user.Hostmask, quote.Trim()),
                            "get", jObject => Utils.SendChannel("Quote successfully added."), Utils.HandleException);
                    }
                    else
                    {
                        Utils.SendChannel("This command is restricted to the IRC channel only.");
                    }
                }
                else
                {
                    Utils.SendChannel("Usage: !quote add <message>");
                }
            }
        }
    }
}
