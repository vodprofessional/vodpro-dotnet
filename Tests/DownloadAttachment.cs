using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace Tests
{
    public class DownloadAttachment
    {

        public static string GetAttachment()
        {
            string s = String.Empty;
            using (WebClient wc = new WebClient())
            {
                string ps = "reviewType=0&pageNum=0&id=com.netflix.mediaclient&reviewSortOrder=0&xhr=1&hl=en";
                wc.Headers.Add("Content-Type:application/x-www-form-urlencoded;charset=utf-8");
            
                s = wc.UploadString("https://play.google.com/store/getreviews", ps);    
            }
            s = s.Replace(@"\u003c", "<").Replace(@"\u003d", "=").Replace(@"\u003e", ">").Replace(@"\""",@"""").Replace(@")]}'","");

            Regex re = new Regex(@"(\[\[""grar"",\d*,"")");
            s = re.Replace(s, "");

            re = new Regex(@"("",\d*\])");
            s = re.Replace(s, "");

            re = new Regex(@"\]");
            s = re.Replace(s, "");

            s = "<div>" + Environment.NewLine + s + Environment.NewLine + "</div>";

            // System.IO.File.WriteAllText(@"D:\tmp\VODPRO\grev.txt", s);




            return s;
        }
    }
}
