using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatSharp;
using ChatSharp.Events;


namespace TestChatSharp
{

	class Program
	{
		public class OTEmsg {
			public static string welcomemsg = "User Joined Channel" ;
		}
		static void Main(string[] args)
		{
			var client = new IrcClient("irc.esper.net", new IrcUser("EdgeSharp", "EdgeSharp"));
			client.NetworkError += (s, e) => Console.WriteLine("Error: " + e.SocketError);
			client.RawMessageRecieved += (s, e) => Console.WriteLine("<< {0}", e.Message);
			client.RawMessageSent += (s, e) => Console.WriteLine(">> {0}", e.Message);
			client.ConnectionComplete += (s, e) => client.JoinChannel("#otegamers");
			client.UserMessageRecieved += (s, e) =>
			{
				if (e.PrivateMessage.Message.StartsWith(".join "))
					client.Channels.Join(e.PrivateMessage.Message.Substring(6));
				else if (e.PrivateMessage.Message.StartsWith(".list "))
				{
					var channel = client.Channels[e.PrivateMessage.Message.Substring(6)];
					var list = channel.Users.Select(u => u.Nick).Aggregate((a, b) => a + "," + b);
					client.SendMessage(list, e.PrivateMessage.User.Nick);
				}
				else if (e.PrivateMessage.Message.StartsWith(".whois "))
					client.WhoIs(e.PrivateMessage.Message.Substring(7), null);
				else if (e.PrivateMessage.Message.StartsWith(".raw "))
					client.SendRawMessage(e.PrivateMessage.Message.Substring(5));
				else if (e.PrivateMessage.Message.StartsWith(".mode "))
				{
					var parts = e.PrivateMessage.Message.Split(' ');
					client.ChangeMode(parts[1], parts[2]);
				}
			};
			client.ChannelMessageRecieved += (s, e) =>
			{
				Console.WriteLine("<{0}> {1}", e.PrivateMessage.User.Nick, e.PrivateMessage.Message);
			};
			client.ChannelMessageRecieved += (s, e) =>
			{
				Console.WriteLine("<{0}> {1}",e.IrcMessage.RawMessage , e.PrivateMessage.Message);
				if(e.IrcMessage.RawMessage.Contains(" JOIN :"))
				{
					Console.WriteLine("Hi <{0}>", e.PrivateMessage.Message);
				}
				if(e.IrcMessage.RawMessage.Contains(" QUIT :"))
				{
					Console.WriteLine("Bye <{0}>", e.PrivateMessage.Message);
				}
				};
			client.ConnectAsync();
			while (true) ;
		}
	}
}
