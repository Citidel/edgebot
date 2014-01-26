using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;
using EdgeBot.Classes.JSON;
using Newtonsoft.Json;

namespace EdgeBot.Classes.Commands
{
    [Command("tps")]
    public class TicksPerSecond : CommandHandler
    {
        public TicksPerSecond()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (Utils.IsOp(user.Nick))
            {
                Connection.GetData(Data.UrlTps, "get", jObject =>
                {
                    string filter;
                    try
                    {
                        filter = paramList[1];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        filter = "";
                    }

                    var outputString = "";
                    const string delimiter = ", ";
                    outputString = String.IsNullOrEmpty(filter) ? jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonTps>(row.ToString())).Aggregate(outputString, (current, tps) => current + (Utils.FormatText(tps.Server.ToUpper(), Colors.Bold) + ":" + Utils.FormatTps(tps.Tps) + "-" + Utils.FormatColor(tps.Count, Colors.DarkGreen) + delimiter)) : jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonTps>(row.ToString())).Where(tps => tps.Server.Contains(paramList[1])).Aggregate(outputString, (current, tps) => current + (Utils.FormatText(tps.Server.ToUpper(), Colors.Bold) + ":" + Utils.FormatTps(tps.Tps) + "-" + Utils.FormatColor(tps.Count, Colors.DarkGreen) + delimiter));
                    if (!String.IsNullOrEmpty(outputString))
                    {
                        Utils.SendChannel(outputString.Substring(0, outputString.Length - delimiter.Length));
                    }
                }, Utils.HandleException);
            }
            else
            {
                Utils.SendChannel(Data.MessageRestricted);
            }
        }
    }
}
