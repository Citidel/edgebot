using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;
using EdgeBot.Classes.JSON;
using Newtonsoft.Json;

namespace EdgeBot.Classes.Commands
{
    [Command("nowplaying")]
    public class NowPlaying : CommandHandler
    {
        public NowPlaying()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            var url = "http://otegamers.com:8080/getnowPlaying";
            WebClient client = new WebClient();
            var returnString = client.DownloadString(url);
            var regex = new Regex(@"(\[url\]*.*)", RegexOptions.IgnoreCase);
            returnString = regex.Match(returnString).Value.Trim();
            Utils.SendNotice(returnString, "Citidel");
        }
    }
}
