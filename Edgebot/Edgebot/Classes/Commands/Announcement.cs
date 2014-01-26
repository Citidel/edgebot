using System;
using System.Collections.Generic;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [Command("announce")]
    public class Announcement : CommandHandler
    {
        public Announcement()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (Utils.IsOp(user.Nick))
            {
                if (paramList.Count < 3)
                {
                    Utils.SendNotice("Usage: !announce <time in seconds> <repeats> <message>", user.Nick);
                }
                else
                {
                    var msg = "";
                    var timeTick = Convert.ToInt32(paramList[1])*1000;
                    var timeCount = Convert.ToInt32(paramList[2]);
                    if (timeTick == 0) return;
                    Program.AnnounceTimer.Interval = timeTick;
                    GC.KeepAlive(Program.AnnounceTimer);
                    for (var i = 3; i < paramList.Count; i++)
                    {
                        msg = msg + paramList[i] + " ";
                    }
                    Instances.Announcement.AnnounceMsg = msg;
                    Instances.Announcement.AnnounceTimes = timeCount;
                    Program.AnnounceTimer.Enabled = true;
                }
            }
            else
            {
                Utils.SendChannel(Data.MessageRestricted);
            }
        }
    }
}
