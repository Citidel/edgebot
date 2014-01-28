using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EdgeBot.Classes.Instances;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdgeBot.Classes.Common
{
    internal static class Connection
    {
        public static void GetData(string url, string method, Action<JObject> taskSuccess,
            Action<AggregateException> taskError)
        {
            Task.Factory.StartNew(() => GetResponse(url, method))
                .ContinueWith(t =>
                {
                    if (t.Result == null)
                    {
                        Utils.Log("Connection: Result is null");
                    }
                    else
                    {
                        taskSuccess(t.Result);
                    }
                })
                .ContinueWith(t => taskError(t.Exception));
        }

        public static void GetPlayerLookup(string playerName, Action<McBans> taskSuccess,
            Action<AggregateException> taskError)
        {
            Task.Factory.StartNew(() => GetMcBansLookup(playerName))
                .ContinueWith(t =>
                {
                    if (t.Result == null)
                    {
                        Utils.Log("Connection: Result is null");
                    }
                    else
                    {
                        taskSuccess(t.Result);
                    }
                })
                .ContinueWith(t => taskError(t.Exception));
        }

        public static void GetServerStatus(Action<MojangStatus> taskSuccess, Action<AggregateException> taskError)
        {
            Task.Factory.StartNew(() => GetMojangStatus())
                .ContinueWith(t =>
                {
                    if (t.Result == null)
                    {
                        Utils.Log("Connection: Result is null");
                    }
                    else
                    {
                        taskSuccess(t.Result);
                    }
                })
                .ContinueWith(t => taskError(t.Exception));
        }

        public static void GetLinkTitle(string url, Action<string> taskSuccess, Action<AggregateException> taskError)
        {
            Task.Factory.StartNew(() => GetWebTitle(url))
                .ContinueWith(t =>
                {
                    if (t.Result == null)
                    {
                        Utils.Log("Connection: Result is null");
                    }
                    else
                    {
                        taskSuccess(t.Result);
                    }
                })
                .ContinueWith(t => taskError(t.Exception));
        }

        public static void GetTs3(string url, Action<string> taskSuccess, Action<AggregateException> taskError)
        {
            Task.Factory.StartNew(() => GetTs3Response(url)).ContinueWith(t =>
            {
                if (t.Result == null)
                {
                    Utils.Log("Connection: Result is null");
                }
                else
                {
                    taskSuccess(t.Result);
                }
            })
                .ContinueWith(t => taskError(t.Exception));
        }

        private static JObject GetResponse(string url, string method)
        {
            var jsonResult = new JObject();
            try
            {
                jsonResult = GetHttpResponse(url, method);
            }
            catch (Exception ex)
            {
                Utils.Log(ex.StackTrace);
            }

            return jsonResult;
        }

        private static JObject GetHttpResponse(string url, string method)
        {
            Utils.Log("Connection: Getting response from {0}", url);
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = method.ToUpper();
            webRequest.ContentType = "application/json";
            webRequest.KeepAlive = true;

            using (var webResponse = webRequest.GetResponse() as HttpWebResponse)
            {
                JObject jsonResult = null;
                if (webResponse != null && webResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        try
                        {
                            jsonResult = JObject.Parse(reader.ReadToEnd());
                        }
                        catch (Exception)
                        {
                            Utils.Log("Connection: Unable to parse stream response: {0}", reader.ReadToEnd());
                            return null;
                        }
                    }
                }
                else
                {
                    if (webResponse != null)
                        Utils.Log("Connection: Error fetching data. Server returned status code : {0}",
                            webResponse.StatusCode);
                }

                return jsonResult;
            }
        }

        private static McBans GetMcBansLookup(string playerName)
        {
            McBans mcBans = null;
            if (Program.McBansApiUrl != null)
            {
                var url = string.Format("http://{0}/v2/{1}", Program.McBansApiUrl, Data.McBansApiKey);
                var postData = "exec=playerLookup&player=" + playerName;
                var byteArray = Encoding.UTF8.GetBytes(postData);

                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.Accept = "*/*";
                webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = byteArray.Length;
                webRequest.UserAgent = "runscope/0.1";

                var dataStream = webRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                using (var webResponse = webRequest.GetResponse() as HttpWebResponse)
                {
                    if (webResponse != null && webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var reader = new StreamReader(webResponse.GetResponseStream()))
                        {
                            try
                            {
                                var data = reader.ReadToEnd();
                                mcBans = JsonConvert.DeserializeObject<McBans>(data);
                            }
                            catch (Exception)
                            {
                                Utils.Log("Connection: Unable to parse stream response: {0}", reader.ReadToEnd());
                                return null;
                            }
                        }
                    }
                    else
                    {
                        if (webResponse != null)
                            Utils.Log("Connection: Error fetching data. Server returned status code : {0}",
                                webResponse.StatusCode);
                    }

                    return mcBans;
                }
            }

            Utils.Log("Failed to lookup player, MC Bans API Url is null.");
            return null;
        }

        private static MojangStatus GetMojangStatus()
        {
            var status = new MojangStatus();
            try
            {
                Utils.Log("Connection: Getting response from {0}", Data.UrlMojangStatus);
                var webRequest = (HttpWebRequest)WebRequest.Create(Data.UrlMojangStatus);
                webRequest.Method = "GET";
                webRequest.ContentType = "application/json";
                webRequest.KeepAlive = true;

                using (var webResponse = webRequest.GetResponse() as HttpWebResponse)
                {
                    if (webResponse != null && webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var reader = new StreamReader(webResponse.GetResponseStream()))
                        {
                            try
                            {
                                var jsonString =
                                    string.Concat("{", reader.ReadToEnd().Replace("{", "").Replace("}", ""), "}")
                                        .Replace("[", "")
                                        .Replace("]", "");
                                var jsonResult = JObject.Parse(jsonString);
                                status.Account = jsonResult["account.mojang.com"].Value<string>() == "green";
                                status.Authentication = jsonResult["auth.mojang.com"].Value<string>() == "green";
                                status.Login = jsonResult["login.minecraft.net"].Value<string>() == "green";
                                status.Session = jsonResult["session.minecraft.net"].Value<string>() == "green";
                                status.Website = jsonResult["minecraft.net"].Value<string>() == "green";
                            }
                            catch (Exception)
                            {
                                Utils.Log("Connection: Unable to parse stream response: {0}", reader.ReadToEnd());
                                return null;
                            }
                        }
                    }
                    else
                    {
                        if (webResponse != null)
                            Utils.Log("Connection: Error fetching data. Server returned status code : {0}",
                                webResponse.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log(ex.StackTrace);
            }

            return status;
        }

        private static string GetWebTitle(string url)
        {
            var returnString = "";
            try
            {
                Utils.Log("Connection: Getting response from {0}", url);
                var webRequest = (HttpWebRequest) WebRequest.Create(url);
                webRequest.Method = "GET";
                webRequest.ContentType = "application/json";
                webRequest.KeepAlive = true;

                using (var webResponse = webRequest.GetResponse() as HttpWebResponse)
                {
                    if (webResponse != null && webResponse.StatusCode == HttpStatusCode.OK &&
                        webResponse.Headers["Content-Type"].StartsWith("text/html"))
                    {
                        using (var reader = new StreamReader(webResponse.GetResponseStream()))
                        {
                            try
                            {
                                var page = reader.ReadToEnd();
                                var regex = new Regex(@"(?<=<title.*>)([\s\S]*)(?=</title>)", RegexOptions.IgnoreCase);
                                returnString = regex.Match(page).Value.Trim();
                            }
                            catch (Exception)
                            {
                                Utils.Log("Connection: Unable to parse stream response: {0}", reader.ReadToEnd());
                                return null;
                            }
                        }
                    }
                    else
                    {
                        if (webResponse != null)
                            Utils.Log("Connection: Error fetching data. Server returned status code : {0}",
                                webResponse.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log(ex.StackTrace);
            }

            return returnString;
        }

        private static string GetTs3Response(string url)
        {
            var returnString = "";
            var cookies = new CookieContainer();
            cookies.Add(new Cookie("authorization", "AzX3QBAL&level=USER", "/", "otegamers.com"));

            try
            {
                Utils.Log("Connection: Getting response from {0}", url);
                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "GET";
                webRequest.ContentType = "text/plain";
                webRequest.KeepAlive = true;
                webRequest.CookieContainer = cookies;
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1700.76 Safari/537.36";

                using (var webResponse = webRequest.GetResponse() as HttpWebResponse)
                {
                    if (webResponse != null && webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var reader = new StreamReader(webResponse.GetResponseStream()))
                        {
                            try
                            {
                                returnString = reader.ReadToEnd();
                            }
                            catch (Exception)
                            {
                                Utils.Log("Connection: Unable to parse stream response: {0}", reader.ReadToEnd());
                                return null;
                            }
                        }
                    }
                    else
                    {
                        if (webResponse != null)
                            Utils.Log("Connection: Error fetching data. Server returned status code : {0}",
                                webResponse.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log(ex.StackTrace);
            }

            return returnString;
        }
    }
}