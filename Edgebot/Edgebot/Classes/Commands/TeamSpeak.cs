using System.Collections.Generic;
using System.Text.RegularExpressions;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [Command("ts")]
    public class TeamSpeak : CommandHandler
    {
        public TeamSpeak()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (paramList.Count == 1)
            {
                Connection.GetTs3(Data.UrlTs3NowPlaying, s =>
                {
                    var regex = new Regex(@"^\'*.*\'", RegexOptions.IgnoreCase);
                    var returnString = regex.Match(s).Value.Trim();
                    if (string.IsNullOrEmpty(returnString))
                    {
                        Utils.SendChannel("No track detected");
                    }
                    else
                    {
                        Utils.SendChannel("Now playing: " + returnString);
                    }
                }, Utils.HandleException);
            }
        }
    }
}
