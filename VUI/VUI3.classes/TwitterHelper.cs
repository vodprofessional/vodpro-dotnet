using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Configuration;
using System.Security.Cryptography;

namespace VUI.VUI3.classes
{
    public class TwitterHelper
    {


        //function to post message to twitter (parameter string message)
        public static void Tweet(string message)
        {
            //The facebook json url to update the status
            string facebookURL = "http://api.twitter.com/1.1/statuses/update.json";

            //set the access tokens (REQUIRED)
            /*string oauth_consumer_key = "Enter customer key here";
            string oauth_consumer_secret = "Enter customer secret here";
            string oauth_token = "Enter access token";
            string oauth_token_secret = "Enter access token secret";
            */


            string oauth_token = ConfigurationManager.AppSettings["TwitterAccessToken"].ToString();
            string oauth_token_secret = ConfigurationManager.AppSettings["TwitterAccessTokenSecret"].ToString();
            string oauth_consumer_key = ConfigurationManager.AppSettings["TwitterConsumerKey"].ToString();
            string oauth_consumer_secret = ConfigurationManager.AppSettings["TwitterConsumerSecret"].ToString();



            // set the oauth version and signature method
            string oauth_version = "1.0";
            string oauth_signature_method = "HMAC-SHA1";

            // create unique request details
            string oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            System.TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            string oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            // create oauth signature
            string baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" + "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&status={6}";

            string baseString = string.Format(
                baseFormat,
                oauth_consumer_key,
                oauth_nonce,
                oauth_signature_method,
                oauth_timestamp, oauth_token,
                oauth_version,
                Uri.EscapeDataString(message)
            );

            string oauth_signature = null;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(Uri.EscapeDataString(oauth_consumer_secret) + "&" + Uri.EscapeDataString(oauth_token_secret))))
            {
                oauth_signature = Convert.ToBase64String(hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes("POST&" + Uri.EscapeDataString(facebookURL) + "&" + Uri.EscapeDataString(baseString))));
            }

            // create the request header
            string authorizationFormat = "OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", " + "oauth_signature=\"{2}\", oauth_signature_method=\"{3}\", " + "oauth_timestamp=\"{4}\", oauth_token=\"{5}\", " + "oauth_version=\"{6}\"";

            string authorizationHeader = string.Format(
                authorizationFormat,
                Uri.EscapeDataString(oauth_consumer_key),
                Uri.EscapeDataString(oauth_nonce),
                Uri.EscapeDataString(oauth_signature),
                Uri.EscapeDataString(oauth_signature_method),
                Uri.EscapeDataString(oauth_timestamp),
                Uri.EscapeDataString(oauth_token),
                Uri.EscapeDataString(oauth_version)
            );

            ServicePointManager.Expect100Continue = false;

            HttpWebRequest objHttpWebRequest = (HttpWebRequest)WebRequest.Create(facebookURL);
            objHttpWebRequest.Headers.Add("Authorization", authorizationHeader);
            objHttpWebRequest.Method = "POST";
            objHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            using (Stream objStream = objHttpWebRequest.GetRequestStream())
            {
                byte[] content = ASCIIEncoding.ASCII.GetBytes("status=" + Uri.EscapeDataString(message));
                objStream.Write(content, 0, content.Length);
            }

            var responseResult = "";
            try
            {
                //success posting
                WebResponse objWebResponse = objHttpWebRequest.GetResponse();
                StreamReader objStreamReader = new StreamReader(objWebResponse.GetResponseStream());
                responseResult = objStreamReader.ReadToEnd().ToString();
            }
            catch (Exception ex)
            {
                //throw exception error
                responseResult = "Twitter Post Error: " + ex.Message.ToString() + ", authHeader: " + authorizationHeader;
            }
        }

        private static void Tweetold(string status)
        {

            string token = GetBearerToken();
            if (!token.Equals("ERROR"))
            {

                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(status);

                // Now get the Twitter Data itself
                string url = @"https://api.twitter.com/1.1/statuses/update.json";
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                req.Headers.Add("Authorization", token);
                req.ContentLength = byteArray.Length;
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";

                Stream dataStream = req.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse resp = req.GetResponse();
                var json = string.Empty;
                using (resp)
                {
                    using (var reader = new StreamReader(resp.GetResponseStream()))
                    {
                        json = reader.ReadToEnd();
                    }
                }
            }
        }

        private static string GetBearerToken()
        {
            try
            {
                // Generate an OAuth token
                var oAuthConsumerKey = ConfigurationManager.AppSettings["TwitterConsumerKey"].ToString(); // @"7IwP3uRyad4gRjgIs19BQ";
                var oAuthConsumerSecret = ConfigurationManager.AppSettings["TwitterConsumerSecret"].ToString();  //@"oHSBSduYJULtMyGTJpeCfVe4Y8CgpdtvhPDZaz8w";
                var oAuthUrl = ConfigurationManager.AppSettings["TwitterOAuthURL"].ToString();

                var authHeaderFormat = "Basic {0}";
                var authHeader = string.Format(authHeaderFormat,
                Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(oAuthConsumerKey) + ":" +
                        Uri.EscapeDataString((oAuthConsumerSecret)))
                        ));

                var postBody = "grant_type=client_credentials";

                HttpWebRequest authRequest = (HttpWebRequest)WebRequest.Create(oAuthUrl);
                authRequest.Headers.Add("Authorization", authHeader);
                authRequest.Method = "POST";
                authRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                authRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (Stream stream = authRequest.GetRequestStream())
                {
                    byte[] content = ASCIIEncoding.ASCII.GetBytes(postBody);
                    stream.Write(content, 0, content.Length);
                }
                authRequest.Headers.Add("Accept-Encoding", "gzip");

                WebResponse authResponse = authRequest.GetResponse();
                // deserialize into an object
                TwitAuthenticateResponse twitAuthResponse;
                using (authResponse)
                {
                    using (var reader = new StreamReader(authResponse.GetResponseStream()))
                    {
                        // JavaScriptSerializer js = new JavaScriptSerializer();
                        var objectText = reader.ReadToEnd();
                        twitAuthResponse = JsonConvert.DeserializeObject<TwitAuthenticateResponse>(objectText);
                    }
                }
                string timelineHeaderFormat = "{0} {1}";
                return string.Format(timelineHeaderFormat, twitAuthResponse.token_type, twitAuthResponse.access_token);
            }
            catch (Exception ex)
            {
                return "ERROR";
            }
        }

    }
    
    public class TwitAuthenticateResponse 
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
    }
}