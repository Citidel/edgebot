using System;
using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [Command("dice")]
    public class JokeDice : CommandHandler
    {
        public JokeDice()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            int i;
            // check if the params number 4, that the number/sides are integers, and that number and sides are both greater than 0
            if (paramList.Count() == 3 && Int32.TryParse(paramList[1], out i) && Int32.TryParse(paramList[2], out i) && (Int32.Parse(paramList[1]) > 0) && (Int32.Parse(paramList[2]) > 0) && (Int32.Parse(paramList[1]) <= 4) && (Int32.Parse(paramList[2]) <= 100))
            {
                var dice = Int32.Parse(paramList[1]);
                var sides = Int32.Parse(paramList[2]);
                var random = new Random();

                var diceList = new List<int>();
                for (var j = 0; j < dice; j++)
                {
                    diceList.Add(random.Next(1, sides));
                }

                var outputString = String.Format("Rolling a {0} sided die, {1} time{2}: {3}", sides, dice, (dice > 1) ? "s" : "", diceList.Aggregate("", (current, roll) => current + roll + " ").Trim());
                Utils.SendChannel(outputString);
            }
            else
            {
                Utils.SendChannel("Usage: !dice <number 1-4 > <sides 1 - 100>");
            }
        }
    }
}
