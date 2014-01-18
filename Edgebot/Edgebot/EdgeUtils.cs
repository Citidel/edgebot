using System;
using System.Linq;
using ChatSharp;

namespace Edgebot
{
    public class EdgeUtils
    {
        public static void SendNotice(IrcClient client, string message, params string[] destinations)
        {
            const string illegalCharacters = "\r\n\0";
            if (!destinations.Any()) throw new InvalidOperationException("Message must have at least one target.");
            if (illegalCharacters.Any(message.Contains)) throw new ArgumentException("Illegal characters are present in message.", "message");
            var to = string.Join(",", destinations);
            client.SendRawMessage("NOTICE {0} :{1}", to, message);
        }

        public static void SendChannel(IrcClient client, string message)
        {
            const string illegalCharacters = "\r\n\0";
            if (illegalCharacters.Any(message.Contains)) throw new ArgumentException("Illegal characters are present in message.", "message");
            client.SendRawMessage("PRIVMSG {0} :{1}", EdgeData.Channel, message);
        }

        public static bool IsDev(string nickname)
        {
            return EdgeData.Developers.Any(str => str.Equals(nickname));
        }

        public static void HandleException(Exception exception)
        {
            if (exception != null)
            {
                Log(exception.StackTrace);
            }
        }

        public static void Log(string message, params object[] args)
        {
            if (!Program.Debug) return;
            Console.WriteLine(message, args);
            System.Diagnostics.Debug.Write(String.Format(message, args));
        }

        public static void Log(object message)
        {
            if (!Program.Debug) return;
            Console.WriteLine(message);
            System.Diagnostics.Debug.Write(message);
        }
    }
}
