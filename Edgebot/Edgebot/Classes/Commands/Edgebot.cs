﻿using System;
using System.Collections.Generic;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("edgebot", "")]
    public class Edgebot : CommandHandler
    {
        public Edgebot()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (paramList.Count <= 1) return;
            switch (paramList[1])
            {
                case "shutdown":
                    if (Utils.IsDev(user.Nick) || Utils.IsAdmin(user.Nick))
                    {
                        Utils.SendNotice("Shutting down bot.", user.Nick);
                        Environment.Exit(0);
                    }
                    else
                    {
                        Utils.SendNotice("This is a restricted command.", user.Nick);
                    }
                    break;

                case "blacklist":
                    if (Utils.IsOp(user.Nick))
                    {
                        Program.Client.WhoIs(paramList[2], whois => Connection.GetData(
                            string.Format(Data.UrlBlacklistAdd,
                                whois.User.Hostname, user.Nick), "get",
                            jObject =>
                            {
                                Utils.SendChannel("Blacklist successfully added.");
                                Program.PopulateBlacklist();
                            }, Utils.HandleException));
                    }
                    else
                    {
                        Utils.SendChannel(Data.MessageRestricted);
                    }
                    break;

                case "reload":
                    if (Utils.IsOp(user.Nick))
                    {
                        Utils.SendNotice("Reloading blacklist.", user.Nick);
                        Program.PopulateBlacklist();
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
