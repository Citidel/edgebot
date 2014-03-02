using System;
using System.Collections.Generic;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("coords", "")]
    public class Coordinates : CommandHandler
    {
        public Coordinates()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            if (Utils.IsOp(user.Nick))
            {
                if (paramList.Count == 3)
                {
                    int num;
                    if (int.TryParse(paramList[1], out num) && int.TryParse(paramList[2], out num))
                    {
                        var chunkX = Math.Round(Math.Floor(Convert.ToDouble(paramList[1])/16), 0);
                        var chunkZ = Math.Round(Math.Floor(Convert.ToDouble(paramList[2])/16), 0);
                        var regionX = Math.Round(Math.Floor(chunkX/32), 0);
                        var regionZ = Math.Round(Math.Floor(chunkZ/32), 0);

                        Utils.SendChannel(String.Format("Chunk Coords: {0}, {1} Region Coords: {2}, {3}", chunkX, chunkZ, regionX, regionZ));
                    }
                    else
                    {
                        Utils.SendChannel("Invalid coordinates were provided.");
                    }
                }
                else
                {
                    Utils.SendChannel("Usage: !coords <xPos> <zPos>");
                }
            }
            else
            {
                Utils.SendChannel(Data.MessageRestricted);
            }
        }
    }
}
