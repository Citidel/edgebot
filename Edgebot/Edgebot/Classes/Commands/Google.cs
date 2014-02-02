using System.Collections.Generic;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [Command("google")]
    public class Google : CommandHandler
    {
        public Google()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (paramList.Count > 1)
            {
                var queryString = "http://www.google.com/search?q=";
                for (var i = 1; i < paramList.Count; i++)
                {
                    queryString = queryString + (paramList[i] + "+");
                }
                Utils.SendChannel(queryString.Substring(0, queryString.Length - 1));
            }
            else
            {
                Utils.SendChannel("Usage: !google <search terms>");
            }
        }
    }
}
