using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using System.Configuration;

namespace VP2.businesslogic
{
    public class Recaptcha2
    {
        private static string secret = ConfigurationManager.AppSettings["RecaptchaSiteSecret"];

        public static bool Validate()
        {
            string Response = HttpContext.Current.Request["g-recaptcha-response"];//Getting Response String Append to Post Method
            bool Valid = false;
            //Request to Google Server
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(" https://www.google.com/recaptcha/api/siteverify?secret=" + secret + "&response=" + Response);
            try
            {
                //Google recaptcha Response
                using (WebResponse wResponse = req.GetResponse())
                {

                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        RecaptchaResponse resp = JsonConvert.DeserializeObject<RecaptchaResponse>(jsonResponse);

                        Valid = Convert.ToBoolean(resp.success);
                    }
                }

                return Valid;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
    }

    public class RecaptchaResponse
    {
        public bool success;
        public string[] errorcodes;
    }
}