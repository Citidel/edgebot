using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Edgebot
{
    class EdgeConn
    {
        public static void GetData(string url, string method, Action<JObject> taskSuccess, Action<AggregateException> taskError)
        {
            Task.Factory.StartNew(() => GetResponse(url, method))
                .ContinueWith(t =>
                {
                    if (t.Result == null)
                    {
                        Console.WriteLine("EdgeConn: Result is null");
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
            while (true)
            {
                var jsonResult = new JObject();
                try
                {
                    jsonResult = GetHttpResponse(url, method);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }

                return jsonResult;
            }
        }

        private static JObject GetHttpResponse(string url, string method)
        {
            Console.WriteLine("EdgeConn: Getting response from {0}", url);
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
                            Console.WriteLine("EdgeConn: Unable to parse stream response: {0}", reader.ReadToEnd());
                            return null;
                        }
                    }
                }
                else
                {
                    if (webResponse != null)
                        Console.WriteLine("EdgeConn: Error fetching data. Server returned status code : {0}", webResponse.StatusCode);
                }

                return jsonResult;
            }
        }
    }
}
