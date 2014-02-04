using System.Collections.Generic;
using System.Text.RegularExpressions;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("ts", "")]
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
                    var regex = new Regex(@"^\'*.*\'|\'\'", RegexOptions.IgnoreCase);
                    var returnString = regex.Match(s).Value.Trim();
                    if (string.IsNullOrEmpty(returnString))
                    {
                        Utils.SendChannel("Music Bot: No track detected");
                    }
                    else
                    {
                        Utils.SendChannel("Music Bot: Now playing - " + returnString);
                    }
                }, Utils.HandleException);
            }
            else
            {
                switch (paramList[1])
                {
                    case "next":
                        if (Utils.IsOp(user.Nick) | Utils.IsAdmin(user.Nick))
                        {
                            Connection.GetTs3(Data.UrlTs3Next, s => Utils.SendChannel("Music Bot: Next track"), Utils.HandleException);
                        }
                        else
                        {
                            Utils.SendChannel(Data.MessageRestricted);
                        }
                        break;
                    case "prev":
                        if (Utils.IsOp(user.Nick) | Utils.IsAdmin(user.Nick))
                        {
                            Connection.GetTs3(Data.UrlTs3Prev, s => Utils.SendChannel("Music Bot: Previous track"), Utils.HandleException);
                        }
                        else
                        {
                            Utils.SendChannel(Data.MessageRestricted);
                        }
                        break;
                    case "vol":
                        if (Utils.IsAdmin(user.Nick) || Utils.IsDev(user.Nick))
                        {
                            if (paramList.Count < 3)
                            {
                                Utils.SendNotice("To set the volume use: !ts vol 0 to 100", user.Nick);
                            }
                            else
                            {
                                Connection.GetTs3(string.Format(Data.UrlTs3Vol, paramList[2]), s => Utils.SendChannel("Music Bot volume set to: " + paramList[2]), Utils.HandleException);

                            }
                        }
                        else
                        {
                            Utils.SendNotice(Data.MessageRestricted, user.Nick);
                        }
                        break;
                    case "stop":
                        if (Utils.IsOp(user.Nick) | Utils.IsAdmin(user.Nick))
                        {
                            Connection.GetTs3(Data.UrlTs3Stop, s => Utils.SendChannel("Music Bot： Playback stopped"), Utils.HandleException);
                        }
                        else
                        {
                            Utils.SendChannel(Data.MessageRestricted);
                        }
                        break;
                    case "play":
                        if (Utils.IsOp(user.Nick) | Utils.IsAdmin(user.Nick))
                        {
                            Connection.GetTs3(Data.UrlTs3Pause, s => Utils.SendChannel("Music Bot: Playback paused"), Utils.HandleException);
                        }
                        else
                        {
                            Utils.SendChannel(Data.MessageRestricted);
                        }
                        break;
                    case "classic":
                        if (Utils.IsOp(user.Nick) | Utils.IsAdmin(user.Nick))
                        {
                            Connection.GetTs3(Data.UrlStation1, s => Utils.SendChannel("Music Bot: Station changed to Classic Rock FM1"), Utils.HandleException);
                        }
                        else
                        {
                            Utils.SendChannel(Data.MessageRestricted);
                        }
                        break;

                    case "ngr":
                        if (Utils.IsOp(user.Nick) | Utils.IsAdmin(user.Nick))
                        {
                            Connection.GetTs3(Data.UrlStation2, s => Utils.SendChannel("Music Bot: Station changed to NetGamesRadio"), Utils.HandleException);
                        }
                        else
                        {
                            Utils.SendChannel(Data.MessageRestricted);
                        }
                        break;
                    case "youtube":
                        if (Utils.IsOp(user.Nick) | Utils.IsAdmin(user.Nick))
                        {
                            Connection.GetTs3(string.Format(Data.UrlYoutube, paramList[2]), s => Utils.SendChannel("Music Bot: Playing YT Link - " + paramList[2]), Utils.HandleException);

                        }
                        else
                        {
                            Utils.SendChannel(Data.MessageRestricted);
                        }
                        break;


                }
            }


        }
    }

}



