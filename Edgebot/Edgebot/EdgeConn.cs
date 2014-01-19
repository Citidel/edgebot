using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Edgebot
{
    internal class EdgeConn
    {
        public static void GetData(string url, string method, Action<JObject> taskSuccess,
            Action<AggregateException> taskError)
        {
            Task.Factory.StartNew(() => GetResponse(url, method))
                .ContinueWith(t =>
                {
                    if (t.Result == null)
                    {
                        EdgeUtils.Log("EdgeConn: Result is null");
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
                        EdgeUtils.Log("EdgeConn: Result is null");
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
                EdgeUtils.Log(ex.StackTrace);
            }

            return jsonResult;
        }

        private static JObject GetHttpResponse(string url, string method)
        {
            EdgeUtils.Log("EdgeConn: Getting response from {0}", url);
            var webRequest = (HttpWebRequest) WebRequest.Create(url);
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
                            EdgeUtils.Log("EdgeConn: Unable to parse stream response: {0}", reader.ReadToEnd());
                            return null;
                        }
                    }
                }
                else
                {
                    if (webResponse != null)
                        EdgeUtils.Log("EdgeConn: Error fetching data. Server returned status code : {0}",
                            webResponse.StatusCode);
                }

                return jsonResult;
            }
        }

        private static MojangStatus GetMojangStatus()
        {
            var status = new MojangStatus();
            try
            {
                EdgeUtils.Log("EdgeConn: Getting response from {0}", EdgeData.UrlMojangStatus);
                var webRequest = (HttpWebRequest) WebRequest.Create(EdgeData.UrlMojangStatus);
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
                                JObject jsonResult = JObject.Parse(jsonString);
                                status.Account = jsonResult["account.mojang.com"].Value<string>() == "green";
                                status.Authentication = jsonResult["auth.mojang.com"].Value<string>() == "green";
                                status.Login = jsonResult["login.minecraft.net"].Value<string>() == "green";
                                status.Session = jsonResult["session.minecraft.net"].Value<string>() == "green";
                                status.Website = jsonResult["minecraft.net"].Value<string>() == "green";
                            }
                            catch (Exception)
                            {
                                EdgeUtils.Log("EdgeConn: Unable to parse stream response: {0}", reader.ReadToEnd());
                                return null;
                            }
                        }
                    }
                    else
                    {
                        if (webResponse != null)
                            EdgeUtils.Log("EdgeConn: Error fetching data. Server returned status code : {0}",
                                webResponse.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                EdgeUtils.Log(ex.StackTrace);
            }

            return status;
        }
    }
}