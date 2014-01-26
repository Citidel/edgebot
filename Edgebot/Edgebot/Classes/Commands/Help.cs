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
    [Command("help")]
    public class Help : CommandHandler
    {
        public Help()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            var filter = "";
            switch (paramList.Count())
            {
                case 1:
                    break;
                case 2:
                    filter = paramList[1];
                    break;

                default:
                    Utils.SendChannel("Usage: !help");
                    return;
            }

            var url = !String.IsNullOrEmpty(filter) ? Data.UrlHelp + "/" + filter : Data.UrlHelp + "/all";
            Connection.GetData(url, "get", jObject =>
            {
                if ((bool)jObject["success"])
                {
                    string outputString;
                    const string delimiter = ", ";
                    if (!String.IsNullOrEmpty(filter))
                    {
                        var help = JsonConvert.DeserializeObject<JsonHelp>(jObject["result"].ToString());
                        outputString = help.Value;
                    }
                    else
                    {
                        outputString = jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonHelp>(row.ToString())).Aggregate("The following keywords are valid: ", (current, help) => current + (help.Keyword + delimiter));
                        outputString = outputString.Substring(0, outputString.Length - delimiter.Length);
                    }

                    Utils.SendChannel(outputString);
                }
                else
                {
                    Utils.SendChannel((string)jObject["message"]);
                }
            }, Utils.HandleException);
        }
    }
}
