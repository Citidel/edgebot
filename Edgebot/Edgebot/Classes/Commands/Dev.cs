using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("dev", "")]
    public class Dev : CommandHandler
    {
        public Dev()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (!Utils.IsDev(user.Nick)) return;
            //var temp = new int[100];
            //for (var i = 0; i < 100; i++)
            //{
            //    temp[i] = GenerateRandom(0, 5);
            //}

            //var occur = new Dictionary<int, int>();
            //foreach (var item in temp)
            //{
            //    if (occur.ContainsKey(item))
            //    {
            //        occur[item] ++;
            //    }
            //    else
            //    {
            //        occur.Add(item, 1);
            //    }
            //}

            //foreach (var item in occur)
            //{
            //    Console.WriteLine(item.Key + ": " + item.Value);
            //}
        }
    }
}
