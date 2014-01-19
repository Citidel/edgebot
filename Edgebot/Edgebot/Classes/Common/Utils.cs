using System;
using System.Linq;
using ChatSharp;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace EdgeBot.Classes.Common
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
            client.SendRawMessage("PRIVMSG {0} :{1}", Config.Channel, message);
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

        /// <summary>
        /// Gets webpage Title
        /// </summary>
        /// <param name="message"></param>
        public static string GetWebPageTitle(string url)
        {
            // Create a request to the url
            HttpWebRequest webRequest = HttpWebRequest.Create(url) as HttpWebRequest;
            // If the request wasn't an HTTP request (like a file), ignore it
            if (webRequest == null) return null;
            // Use the user's credentials
            webRequest.UseDefaultCredentials = true;
            // Obtain a response from the server, if there was an error, return nothing
            HttpWebResponse response = null;
            try { response = webRequest.GetResponse() as HttpWebResponse; }
            catch (WebException) { return null; }
            // Regular expression for an HTML title
            string regex = @"(?<=<title.*>)([\s\S]*)(?=</title>)";

            // If the correct HTML header exists for HTML text, continue
            if (new List <string>(response.Headers.AllKeys).Contains("Content-Type"))
                if (response.Headers["Content-Type"].StartsWith("text/html"))
                {
                    // Download the page
                    WebClient web = new WebClient();
                    web.UseDefaultCredentials = true;
                    string page = web.DownloadString(url);
                    // Extract the title
                    Regex ex = new Regex(regex, RegexOptions.IgnoreCase);
                    return ex.Match(page).Value.Trim();
                }
            // Not a valid HTML page
            return null;
        }
    }
}
