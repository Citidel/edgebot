using System.Collections.Generic;
using System.Linq;
using ChatSharp;
using EdgeBot.Classes.Common;
using EdgeBot.Classes.Core;
using Newtonsoft.Json.Linq;

namespace EdgeBot.Classes.Commands
{
    [CommandAttribute("minecheck", "Displays Minecraft-related status information. Powered by xPaw.")]
    public class ServerStatus : CommandHandler
    {
        public ServerStatus()
        {
        }

        public override void HandleCommand(IList<string> paramList, IrcUser user, bool isIngameCommand)
        {
            Connection.GetData("http://xpaw.ru/mcstatus/status.json", "get", jObject =>
            {
                var arrayStatus = jObject["report"].Cast<JProperty>().ToDictionary(item => Utils.UcFirst(item.Name), item => (string) item.Value.SelectToken("status") == "up" ? Utils.FormatStatus("U", true) : Utils.FormatStatus("D", false));
                var message = arrayStatus.Aggregate("", (current, item) => current + item.Key + "[" + item.Value + "] ");
                Utils.SendChannel(message.Substring(0, message.Length - 1));
            }, Utils.HandleException);
        }
    }
}