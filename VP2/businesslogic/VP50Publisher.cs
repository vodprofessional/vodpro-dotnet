using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;

namespace VP2.businesslogic
{
    public class VP50Publisher : ApplicationBase
    {
        public VP50Publisher()
        {
            Document.AfterPublish += new Document.PublishEventHandler(Document_AfterPublish);
        }

        void Document_AfterPublish(Document sender, umbraco.cms.businesslogic.PublishEventArgs e)
        {
            if (sender.ContentType.Alias.Equals("DataFolder"))
            {
                if(sender.Text.StartsWith("VP50"))
                {
                    Publish(sender);
                }
            }
        }

        void Publish(Document d)
        {
            string content50VPUrl = umbraco.library.NiceUrl(d.Id);
            content50VPUrl = String.Format(@"/media/VP50/{0}.js", content50VPUrl.Replace("/", ""));


            List<VP50Json> vp50list = new List<VP50Json>();

            foreach (Document p in d.Children)
            {
                StringBuilder s = new StringBuilder();
                s.Append("{");
                s.Append(@"""rank"":");
                s.Append(@"""" + p.getProperty("rank").Value.ToString() + @""",");

                s.Append(@"""previous-rank"":");
                s.Append(@"""" + p.getProperty("lastYearsRank").Value.ToString() + @""",");

                s.Append(@"""name"":");
                s.Append(@"""" + p.getProperty("name").Value.ToString().Replace("\"", @"\""") + @""",");

                s.Append(@"""job"":");
                s.Append(@"""" + p.getProperty("jobTitle").Value.ToString().Replace("\"", @"\""") + @""",");

                s.Append(@"""brief"":");
                s.Append(@"""" + p.getProperty("briefDescription").Value.ToString().Replace("\"", @"\""") + @""",");

                s.Append(@"""long"":");
                s.Append(@"""" + p.getProperty("longDescription").Value.ToString().Replace("\"", @"\""").Replace("'", "\'").Replace(Environment.NewLine, " ") + @""",");

                s.Append(@"""img"":");
                s.Append(@"""" + p.getProperty("portrait").Value.ToString() + @""",");

                s.Append(@"""thumb"":");
                s.Append(@"""" + p.getProperty("smallPortrait").Value.ToString() + @""",");

                s.Append(@"""relatedlinktext"":");
                s.Append(@"""" + p.getProperty("relatedLinkText").Value.ToString().Replace("\"", @"\""") + @""",");

                s.Append(@"""link"":");
                try
                {
                    string link = umbraco.library.NiceUrl(Int32.Parse(p.getProperty("relatedLink").Value.ToString()));
                    s.Append(@"""" + link + @""" ");
                }
                catch (Exception ex)
                {
                    s.Append(@"""""");
                }
                s.Append("}");

                vp50list.Add(new VP50Json(Int32.Parse(p.getProperty("rank").Value.ToString()), s.ToString()));
            }

            IEnumerable<string> query = from vp in vp50list orderby vp.Rank ascending select vp.JSON;

            string outstr = @"[" + String.Join(",", query.ToArray<string>()) + @"]";

            File.WriteAllText(HttpContext.Current.Server.MapPath(content50VPUrl), outstr);
        }

    }

    class VP50Json
    {
        public int Rank { get; set; }
        public string JSON { get; set; }
        public VP50Json(int rank, string str)
        {
            this.Rank = rank;
            this.JSON = str;
        }
    }
}