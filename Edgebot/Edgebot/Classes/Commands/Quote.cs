using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("quote", "Usage: !quote add <quote> to add quote, !quote <number> for a specific quote, !quote for random quote")]
    public class Quote : CommandHandler
    {
        public Quote()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            int num;
            if (paramList.Count == 1)
            {
                // display random quote
                //Connection.GetData(Data.UrlQuote, "get", jObject =>
                //{
                //    if ((bool)jObject["success"])
                //    {
                //        Utils.SendChannel((string)jObject["result"].SelectToken("id") + ": " +
                //                          (string)jObject["result"].SelectToken("quote"));
                //    }
                //    else
                //    {
                //        Utils.SendChannel("No quotes found.");
                //    }
                //}, Utils.HandleException);

                GetResult("SELECT id, quote FROM edge_quote WHERE status = '1' ORDER BY RAND() LIMIT 1", reader =>
                {
                    while (reader.Read())
                    {
                        Utils.SendChannel(reader.GetUInt32(0) + ": " + reader.GetString(1));
                    }
                    reader.Close();
                });
            }
            else if (paramList[1] == "add")
            {
                if (isIngameCommand == false)
                {
                    var quote = "";
                    for (var l = 2; l < paramList.Count; l++)
                    {
                        quote = quote + paramList[l] + " ";
                    }

                    Connection.GetData(string.Format(Data.UrlQuoteAdd, user.Nick, user.Hostmask, quote.Trim()),
                        "get", jObject => Utils.SendChannel(string.Format("Quote #{0} has been added.", jObject["result"].SelectToken("id"))), Utils.HandleException);
                }
                else
                {
                    Utils.SendChannel("This command is restricted to the IRC channel only.");
                }
            }
            else if (int.TryParse(paramList[1], out num))
            {
                Connection.GetData(string.Format(Data.UrlQuoteSpecific, num), "get", jObject =>
                {
                    if ((bool)jObject["success"])
                    {
                        Utils.SendChannel((string)jObject["result"].SelectToken("quote"));
                    }
                    else
                    {
                        Utils.SendChannel("Specific quote not found.");
                    }
                }, Utils.HandleException);
            }
            else
            {
                Utils.SendChannel("Usage: !quote add <message>");
            }
        }
    }
}
