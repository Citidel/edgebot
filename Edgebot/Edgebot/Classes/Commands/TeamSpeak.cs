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
                    var regex = new Regex(@"^\'*.*\'|\'\'", RegexOptions.IgnoreCase);
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
            else
            {
                     switch (paramList[1])
                    {
                        case "next":
                             if (Utils.IsOp(user.Nick)|Utils.IsAdmin(user.Nick))
                                {
                                    Connection.GetTs3(Data.UrlTs3Next, s =>
                                    {
                                        Utils.SendChannel("Music Bot: Next Track");

                                    }, Utils.HandleException);
                                 }  else
                                    {
                                        Utils.SendChannel("Only Ops can control the bot from IRC");
                                    }
                            break;
                        case "prev":
                            if (Utils.IsOp(user.Nick) | Utils.IsAdmin(user.Nick))
                                {
                            Connection.GetTs3(Data.UrlTs3Prev, s =>
                            {
                                Utils.SendChannel("Music Bot: Previous Track");

                            }, Utils.HandleException);
                             }  else
                                {
                                    Utils.SendChannel("Only Ops can control the bot from IRC");
                                }
                            break;
                        case "vol":
                            if (Utils.IsAdmin(user.Nick) || Utils.IsDev(user.Nick))
                            {
                                if (paramList.Count < 3)
                                {
                                    Utils.SendNotice("To set the Volume use: !ts vol 0 to 100", user.Nick);
                                }
                                else
                                {
                                    Connection.GetTs3(string.Format(Data.UrlTs3Vol, paramList[2]), s =>
                                    {
                                        Utils.SendChannel("Music Bot Volume set to: " + paramList[2]);

                                    }, Utils.HandleException);

                                }
                            }
                            else
                            {
                                Utils.SendNotice(user.Nick + "  Only Admins can set the Bot Volume from IRC", user.Nick);
                            }
                            break;
                        case "stop":
                            if (Utils.IsOp(user.Nick) | Utils.IsAdmin(user.Nick))
                                {
                            Connection.GetTs3(Data.UrlTs3Stop, s =>
                            {
                                Utils.SendChannel("Music Bot Stoppig Music");

                            }, Utils.HandleException);
                             }  else
                            {
                                Utils.SendChannel("Only Ops can control the bot from IRC");
                            }
                            break;
                        case "play":
                            if (Utils.IsOp(user.Nick) | Utils.IsAdmin(user.Nick))
                                {
                            Connection.GetTs3(Data.UrlTs3Pause, s =>
                            {
                                Utils.SendChannel("Music Bot: Play\\Pause button Pressed");

                            }, Utils.HandleException);
                             }  else
                            {
                                Utils.SendChannel("Only Ops can control the bot from IRC");
                            }
                            break;
                        case "classic":
                            if (Utils.IsOp(user.Nick) | Utils.IsAdmin(user.Nick))
                                {
                            Connection.GetTs3(Data.UrlStation1, s =>
                            {
                                Utils.SendChannel("Music Bot Station changed to: Classic Rock FM1");

                            }, Utils.HandleException);
                             }  else
                            {
                                Utils.SendChannel("Only Ops can control the bot from IRC");
                            }
                                break;

                            case "ngr":
                                if (Utils.IsOp(user.Nick) | Utils.IsAdmin(user.Nick))
                                    {
                                Connection.GetTs3(Data.UrlStation2, s =>
                                {
                                    Utils.SendChannel("Music Bot Station changed to: NetGamesRadio");

                                }, Utils.HandleException);
                                }  else
                                {
                                    Utils.SendChannel("Only Ops can control the bot from IRC");
                                }
                                break;
                            case "youtube":
                                if (Utils.IsOp(user.Nick) | Utils.IsAdmin(user.Nick))
                                    {
                                        Connection.GetTs3(string.Format(Data.UrlYoutube,paramList[2]), s =>
                                        {
                                            Utils.SendChannel("Music Bot Playing Youtube: " + paramList[2]);

                                        }, Utils.HandleException);

                                   }
                                break;
                                

                    }
                }
              

            }
        }

}



