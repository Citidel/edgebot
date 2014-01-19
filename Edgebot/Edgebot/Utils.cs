using System;
using System.Linq;
using ChatSharp;

namespace Edgebot
{
    /// <summary>
    /// Utility methods for EdgeBot
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Sends a notice to a set of destinations
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        /// <param name="destinations"></param>
        public static void SendNotice(IrcClient client, string message, params string[] destinations)
        {
            const string illegalCharacters = "\r\n\0";
            if (!destinations.Any()) throw new InvalidOperationException("Message must have at least one target.");
            if (illegalCharacters.Any(message.Contains)) throw new ArgumentException("Illegal characters are present in message.", "message");
            var to = string.Join(",", destinations);
            client.SendRawMessage("NOTICE {0} :{1}", to, message);
        }

        /// <summary>
        /// Sends a message to the channel
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void SendChannel(IrcClient client, string message)
        {
            const string illegalCharacters = "\r\n\0";
            if (illegalCharacters.Any(message.Contains)) throw new ArgumentException("Illegal characters are present in message.", "message");
            client.SendRawMessage("PRIVMSG {0} :{1}", Data.Channel, message);
        }

        /// <summary>
        /// Returns true if the nickname is a developer
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public static bool IsDev(string nickname)
        {
            return Data.Developers.Any(str => str.Equals(nickname));
        }

        /// <summary>
        /// Returns true if the nickname is an operator
        /// </summary>
        /// <param name="client"></param>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public static bool IsOp(IrcClient client, string nickname)
        {
            return client.Channels.Select(channel => channel.UsersByMode['o']).Any(users => users.Contains(nickname));
        }

        /// <summary>
        /// Returns a string formatted with the given style code
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="colorCode"></param>
        /// <returns></returns>
        public static string FormatText(object inputString, char colorCode)
        {
            return String.Format("{0}" + inputString + "{1}", colorCode, EdgeColors.Normal);
        }

        /// <summary>
        /// Returns a string formatted with the given color code
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="formatCode"></param>
        /// <returns></returns>
        public static string FormatColor(object inputString, string formatCode)
        {
            return String.Format("{0}" + inputString + "{1}", formatCode, EdgeColors.Normal);
        }
        /// <summary>
        /// Returns the string formatted with green if true and red if false
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static string FormatStatus(object inputString, bool check)
        {
            return String.Format("{0}" + FormatText(inputString, EdgeColors.Bold) + "{1}", check ? EdgeColors.DarkGreen : EdgeColors.Red, EdgeColors.Normal);
        }

        /// <summary>
        /// Exception handler for the connection class
        /// </summary>
        /// <param name="exception"></param>
        public static void HandleException(Exception exception)
        {
            if (exception != null)
            {
                Log(exception.StackTrace);
            }
        }

        /// <summary>
        /// Logs a message in the console window and to the debug window
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Log(string message, params object[] args)
        {
            if (!Program.Debug) return;
            if (message == null) return;
            Console.WriteLine(message, args);
            System.Diagnostics.Debug.Write(String.Format(message, args) + "\n");
        }

        /// <summary>
        /// Logs a message in the console window and to the debug window
        /// </summary>
        /// <param name="message"></param>
        public static void Log(object message)
        {
            if (!Program.Debug) return;
            if (message == null) return;
            Console.WriteLine(message);
            System.Diagnostics.Debug.Write(message + "\n");
        }
    }
}
