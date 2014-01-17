using ChatSharp;
using System;

namespace Edgebot
{

    class Program
    {
        private const bool Debug = true;

        static void Main()
        {
            var client = new IrcClient(EdgeData.Host, new IrcUser(EdgeData.Nickname, EdgeData.Username));
            client.ConnectionComplete += (s, e) => client.JoinChannel(EdgeData.Channel);

            client.NetworkError += (s, e) => Console.WriteLine("Error: " + e.SocketError);

            client.RawMessageRecieved += (s, e) => Console.WriteLine("RAWRCV {0}", e.Message);
            client.RawMessageSent += (s, e) => Console.WriteLine("RAWSNT {0}", e.Message);

            client.ChannelMessageRecieved += (s, e) => Console.WriteLine("<{0}> {1}", e.PrivateMessage.User.Nick, e.PrivateMessage.Message);
            client.UserJoinedChannel += (sender, args) =>
            {
                if (Debug && EdgeUtils.IsDev(args.User.Nick))
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
