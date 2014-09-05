using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using umbraco.NodeFactory;
using umbraco.cms.businesslogic.web;

namespace VODPro.usercontrols
{
    public partial class VP50admin : System.Web.UI.UserControl
    {
        string jsonPath = String.Empty;
        int VP50Parent;

        protected void Page_Load(object sender, EventArgs e)
        {
            jsonPath = ConfigurationManager.AppSettings["VP50jsonpath"].ToString();
            VP50Parent = Int32.Parse(ConfigurationManager.AppSettings["VP50Parent"].ToString());
            
        }

        protected void btnGenerateVP50_Click(object sender, EventArgs e)
        {
            Document d = new Document(VP50Parent);



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
                s.Append(@"""" + p.getProperty("longDescription").Value.ToString().Replace("\"", @"\""").Replace("'", "\'").Replace(Environment.NewLine," ") + @""",");

                s.Append(@"""img"":");
                s.Append(@"""" + p.getProperty("portrait").Value.ToString() + @""",");

                s.Append(@"""thumb"":");
                s.Append(@"""" + p.getProperty("smallPortrait").Value.ToString() + @""",");
                
                s.Append(@"""relatedlinktext"":");
                s.Append(@"""" + p.getProperty("relatedLinkText").Value.ToString().Replace("\"", @"\""") + @""",");

                s.Append(@"""link"":");
                try {
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

            File.WriteAllText(MapPath(jsonPath), outstr);

            litConfirm.Text = "Generated JSON file at: " + jsonPath;
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