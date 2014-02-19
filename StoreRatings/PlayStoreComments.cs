using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace StoreRatings
{
    public class PlayStoreComments
    {
        private static string _url = "https://play.google.com/store/getreviews";

        public static string GetComments(string id, int pageNum)
        {
            string s = String.Empty;
            using (WebClient wc = new WebClient())
            {
                string ps = String.Format(@"reviewType=0&pageNum={0}&id={1}&reviewSortOrder=0&xhr=1&hl=en", pageNum, id );
                wc.Headers.Add("Content-Type:application/x-www-form-urlencoded;charset=utf-8");

                s = wc.UploadString(_url, ps);
            }
            s = s.Replace(@"\u003c", "<").Replace(@"\u003d", "=").Replace(@"\u003e", ">").Replace(@"\""", @"""").Replace(@")]}'", "");

            Regex re = new Regex(@"(\[\[""grar"",\d*,"")");
            s = re.Replace(s, "");

            re = new Regex(@"("",\d*\])");
            s = re.Replace(s, "");

            re = new Regex(@"\]");
            s = re.Replace(s, "");

            s = "<div>" + Environment.NewLine + s + Environment.NewLine + "</div>";

            return s;
        }

    }
}
