using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;
using Newtonsoft.Json;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("wiki", "Usage: !wiki")]
    public class Wiki : CommandHandler
    {
        public Wiki()
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
                    Utils.SendChannel("Usage: !wiki");
                    return;
            }

            var url = !String.IsNullOrEmpty(filter) ? Data.UrlWiki + "/" + filter : Data.UrlWiki + "/all";
            Connection.GetData(url, "get", jObject =>
            {
                if ((bool)jObject["success"])
                {
                    string outputString;
                    const string delimiter = ", ";
                    if (!String.IsNullOrEmpty(filter))
                    {
                        var wiki = JsonConvert.DeserializeObject<JsonWiki>(jObject["result"].ToString());
                        outputString = "Wiki URL for '" + wiki.Keyword + "' - " + wiki.Url;
                    }
                    else
                    {
                        outputString = jObject["result"].Select(row => JsonConvert.DeserializeObject<JsonWiki>(row.ToString())).Aggregate("The following keywords are valid: ", (current, wiki) => current + (wiki.Keyword + delimiter));
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

    public class JsonWiki
    {
        public string Keyword { get; set; }
        public string Url { get; set; }
    }
}
