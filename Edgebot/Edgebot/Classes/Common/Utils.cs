using System;
using System.Linq;

namespace EdgeBot.Classes.Common
{
    /// <summary>
    /// Utility methods for EdgeBot
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Sends a notice to a set of destinations
        /// </summary>
        /// <param name="message"></param>
        /// <param name="destinations"></param>
        public static void SendNotice(string message, params string[] destinations)
        {
            const string illegalCharacters = "\r\n\0";
            if (!destinations.Any()) throw new InvalidOperationException("Message must have at least one target.");
            if (illegalCharacters.Any(message.Contains)) throw new ArgumentException("Illegal characters are present in message.", "message");
            var to = string.Join(",", destinations);
            Program.Client.SendRawMessage("NOTICE {0} :{1}", to, message);
        }

        /// <summary>
        /// Sends a message to the channel
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void SendChannel(string message)
        {
            const string illegalCharacters = "\r\n\0";
            if (illegalCharacters.Any(message.Contains)) throw new ArgumentException("Illegal characters are present in message.", "message");
            Program.Client.SendRawMessage("PRIVMSG {0} :{1}", Config.Channel, message);
        }

        /// <summary>
        /// Sends a private message to a set of destinations
        /// </summary>
        /// <param name="message"></param>
        /// <param name="destinations"></param>
        public static void SendPm(string message, params string[] destinations)
        {
            const string illegalCharacters = "\r\n\0";
            if (!destinations.Any()) throw new InvalidOperationException("Message must have at least one target.");
            if (illegalCharacters.Any(message.Contains)) throw new ArgumentException("Illegal characters are present in message.", "message");
            var to = string.Join(",", destinations);
            Program.Client.SendRawMessage("PRIVMSG {0} :{1}", to, message);
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
        /// Returns true if the nickname is a server admin
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public static bool IsAdmin(string nickname)
        {
            return Data.Admin.Any(str => str.Equals(nickname));
        }

        /// <summary>
        /// Returns true if the nickname is an operator
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public static bool IsOp(string nickname)
        {
            return Program.Client.Channels.Select(channel => channel.UsersByMode['o']).Any(users => users.Contains(nickname));
        }

        /// <summary>
        /// Returns a string formatted with the given style code
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="colorCode"></param>
        /// <returns></returns>
        public static string FormatText(object inputString, char colorCode)
        {
            return String.Format("{0}" + inputString + "{1}", colorCode, Colors.Normal);
        }

        /// <summary>
        /// Returns a string formatted with the given color code
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="formatCode"></param>
        /// <returns></returns>
        public static string FormatColor(object inputString, string formatCode)
        {
            return String.Format("{0}" + inputString + "{1}", formatCode, Colors.Normal);
        }
        /// <summary>
        /// Returns the string formatted with green if true and red if false
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static string FormatStatus(object inputString, bool check)
        {
            return String.Format("{0}" + FormatText(inputString, Colors.Bold) + "{1}", check ? Colors.DarkGreen : Colors.Red, Colors.Normal);
        }

        /// <summary>
        /// Returns the tps float formatted with the appropriate colors
        /// </summary>
        /// <param name="inputFloat"></param>
        /// <returns></returns>
        public static string FormatTps(float inputFloat)
        {
            if (inputFloat > 15)
            {
                return String.Format("{0}" + inputFloat + "{1}", Colors.DarkGreen, Colors.Normal);
            }
            if (inputFloat >= 10 && inputFloat < 15)
            {
                return String.Format("{0}" + inputFloat + "{1}", Colors.Yellow, Colors.Normal);
            }
            if (inputFloat >= 0 && inputFloat < 10)
            {
                return String.Format("{0}" + inputFloat + "{1}", Colors.Red, Colors.Normal);
            }
            return inputFloat + "";
        }

        /// <summary>
        /// Returns the server version for the given short code and id
        /// </summary>
        /// <param name="shortCode"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetVersion(string shortCode, string id)
        {
            foreach (var server in Program.ServerList.Where(server => server.ShortCode == shortCode && server.Id == id))
            {
                return server.Version;
            }

            return "Unknown";
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
            if (message == null) return;
            Console.WriteLine(message, args);
            System.Diagnostics.Debug.Write(String.Format(message, args) + "\n");
        }
    }
}
