using System;
using System.Web;
using System.Net;
using System.IO;

namespace FeedProxy
{
    public class Proxy : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string url = context.Request.Params["u"];
            //string url = @"http://www.jobserve.com/MySearch/42FBC8D309E7AB31.rss";

//string url = @"http://www.labs.jobserve.com/services/v0.3.7/Core.svc/SiteGBR?source=39FB8404A11155&pagesize=100&sort=EXPLORER_DATE_DESC&skills=vod%20or%20%22video%20on%20demand%22%20or%20video%20or%20iptv%20or%20%22connected%20tv%22%20or%20television%20or%20transcode%20or%20ingest";
			

            if(!string.IsNullOrEmpty(url))
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.KeepAlive = true;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string responseBody = string.Empty;
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseBody = reader.ReadToEnd();
                }

                context.Response.ContentType = response.ContentType;
                context.Response.Write(responseBody);
            }
            else
            {
                context.Response.ContentType = "text/html";
                string message = string.Format("You must supply a 'url' query parameter.");
                context.Response.Write(message);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}