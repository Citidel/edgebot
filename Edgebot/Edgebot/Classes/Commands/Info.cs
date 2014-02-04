using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;
using Newtonsoft.Json;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("info", "Usage: !info list to list keywords, !info <keyword> to see info")]
    public class Info : CommandHandler
    {
        public Info()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            var filter = "";
            if (paramList.Count == 2)
            {
                filter = paramList[1];
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
                        outputString =
                            jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonHelp>(row.ToString()))
                                .Aggregate("Keywords: ",
                                    (current, help) => current + (help.Keyword + delimiter));
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

    public class JsonHelp
    {
        public string Keyword { get; set; }
        public string Value { get; set; }
    }
}
