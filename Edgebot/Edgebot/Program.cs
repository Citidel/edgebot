using System.Linq;
using ChatSharp;
using System;

namespace Edgebot
{

    class Program
    {
        private const bool Debug = true;

        static void Main()
        {
            var client = new IrcClient("irc.esper.net:5555", new IrcUser("EdgeTest", "EdgeTest"));
            client.ConnectionComplete += (s, e) => client.JoinChannel("#otegamers");

            client.NetworkError += (s, e) => Console.WriteLine("Error: " + e.SocketError);

            client.RawMessageRecieved += (s, e) => Console.WriteLine("RAWRCV {0}", e.Message);
            client.RawMessageSent += (s, e) => Console.WriteLine("RAWSNT {0}", e.Message);

            client.ChannelMessageRecieved += (s, e) => Console.WriteLine("<{0}> {1}", e.PrivateMessage.User.Nick, e.PrivateMessage.Message);
            client.UserJoinedChannel += (sender, args) =>
            {
                if (Debug && EdgeData.Developers.Any(str => str.Equals(args.User.Nick)))
                {
                    EdgeUtils.SendNotice(client, String.Format(EdgeData.JoinMessage, args.User.Nick), args.User.Nick);
                }
            };

            client.ConnectAsync();
            while (true)
            {
            }
        }
    }
}
