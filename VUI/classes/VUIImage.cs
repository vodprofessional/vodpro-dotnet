using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using umbraco.MacroEngines;
using umbraco.NodeFactory;
using System.Web.Security;
using System.Web.SessionState;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.datatype;
using System.Web;
using System.Xml;

namespace VUI.classes
{

    public class VUIImage
    {
        string VUI_mediafolder = String.Empty;
        public bool IsFavourite { get; set; }

        public DynamicNode Node { get; set; }

        public VUIRating personalRating { get; set; }
        public VUIRating teamRating { get; set; }
        public VUIRating overallRating { get; set; }
        

        public string PageType { get { return Node.GetProperty("pageType").Value.ToString(); } }
        public string ImageURL_th { get { return VUI_mediafolder + @"th/" + Node.GetProperty("thFile").Value.Replace("&", "%26"); } }
        public string ImageURL_md { get { return VUI_mediafolder + @"md/" + Node.GetProperty("thFile").Value.Replace("&", "%26"); } }
        public string ImageURL_lg { get { return VUI_mediafolder + @"lg/" + Node.GetProperty("lgFile").Value.Replace("&", "%26"); } }
        public string ImageURL_full { get { return VUI_mediafolder + @"full/" + Node.GetProperty("imageFile").Value.Replace("&", "%26"); } }

        public string VuiDate { get { return Node.GetProperty("vuidate").Value; } }




        public VUIImage(DynamicNode n)
        {
            Node = n;
            VUI_mediafolder = ConfigurationManager.AppSettings["VUI_mediafolder"].ToString().Replace("~", "");

            personalRating = new VUIRating(VUIRating.MODE_PERSONAL);
            teamRating = new VUIRating(VUIRating.MODE_TEAM);
            overallRating = new VUIRating(VUIRating.MODE_OVERALL);
            IsFavourite = false;

            SetIsFavourite();
        }

        public void SetIsFavourite()
        {

            Member m = VUIfunctions.CurrentUser();
            if (m != null)
            {
                

                /*
                 * Node.GetProperty("ratings")	{
                 * <items>
                 *  <item id="1" sortOrder="0">
                 *      <vuiRatingsUser nodeName="User" nodeType="1036">2936</vuiRatingsUser>
                 *      <vuiUserRating nodeName="Rating" nodeType="-51">7</vuiUserRating>
                 *  </item>
                 *  <item id="2" sortOrder="1">
                 *      <vuiRatingsUser nodeName="User" nodeType="1036">2935</vuiRatingsUser>
                 *      <vuiUserRating nodeName="Rating" nodeType="-51">9</vuiUserRating>
                 *  </item>
                 *  </items>}
                 * 
                 * 
                 */

                if (Node.GetProperty("ratings") != null)
                {
                    string ratings = Node.GetProperty("ratings").Value.Replace("{", "").Replace("}", "");
                    XmlDocument xml = new XmlDocument();
                    try
                    {
                        xml.LoadXml(ratings);

                        foreach (XmlNode item in xml.SelectSingleNode("items").SelectNodes("item"))
                        {
                            int userid = Int32.Parse(item.SelectSingleNode("vuiRatingsUser").InnerText);
                            int rating = Int32.Parse(item.SelectSingleNode("vuiUserRating").InnerText);

                            // If this user's rating, update personal, team and overall
                            if (userid == m.Id)
                            {
                                IsFavourite = true;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Probably no root node. Lazy coding!
                    }
                }
            }
        }

        public void OldSetRatings()
        {
            Member m = VUIfunctions.CurrentUser();
            if (m != null)
            {
                string status = VUIfunctions.MemberVUIStatus(m);

                List<Member> teamMembers;

                if (status.Equals(VUIfunctions.VUI_USERTYPE_ADMIN))
                {
                    teamMembers = VUIfunctions.GetVUIUsersForAdmin(m);
                }
                else
                {
                    Member admin = VUIfunctions.GetVUIAdminForCurrentUser();
                    teamMembers = VUIfunctions.GetVUIUsersForAdmin(admin);
                    teamMembers.Add(admin);
                }

                /*
                 * Node.GetProperty("ratings")	{
                 * <items>
                 *  <item id="1" sortOrder="0">
                 *      <vuiRatingsUser nodeName="User" nodeType="1036">2936</vuiRatingsUser>
                 *      <vuiUserRating nodeName="Rating" nodeType="-51">7</vuiUserRating>
                 *  </item>
                 *  <item id="2" sortOrder="1">
                 *      <vuiRatingsUser nodeName="User" nodeType="1036">2935</vuiRatingsUser>
                 *      <vuiUserRating nodeName="Rating" nodeType="-51">9</vuiUserRating>
                 *  </item>
                 *  </items>}
                 * 
                 * 
                 */

                if (Node.GetProperty("ratings") != null)
                {
                    string ratings = Node.GetProperty("ratings").Value.Replace("{", "").Replace("}", "");
                    XmlDocument xml = new XmlDocument();
                    try
                    {
                        xml.LoadXml(ratings);
                    
                        foreach (XmlNode item in xml.SelectSingleNode("items").SelectNodes("item"))
                        {
                            int userid = Int32.Parse(item.SelectSingleNode("vuiRatingsUser").InnerText);
                            int rating = Int32.Parse(item.SelectSingleNode("vuiUserRating").InnerText);

                            // If this user's rating, update personal, team and overall
                            if (userid == m.Id)
                            {
                                personalRating.AddRating(rating);
                                teamRating.AddRating(rating);
                                overallRating.AddRating(rating);
                            }
                            else
                            {
                                // Cycle through the team members and try to match to the rating user
                                bool isTeamMember = false;
                                foreach (Member tm in teamMembers)
                                {
                                    if (userid == tm.Id)
                                    {
                                        isTeamMember = true;
                                        break;
                                    }
                                }


                                if (isTeamMember)
                                {
                                    teamRating.AddRating(rating);
                                    overallRating.AddRating(rating);
                                }

                                else
                                {
                                    overallRating.AddRating(rating);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Probably no root node. Lazy coding!
                    }
                }
            }
        }



        public string GetJson()
        {
            StringBuilder s = new StringBuilder();

            s.Append("{");

            s.Append(@"""id"":");
            s.Append(@"""" + Node.Id.ToString() + @""",");

            s.Append(@"""platform"":");
            s.Append(@"""" + Node.GetProperty("platform").Value + @""",");

            s.Append(@"""device"":");
            s.Append(@"""" + Node.GetProperty("device").Value + @""",");

            s.Append(@"""service"":");
            s.Append(@"""" + Node.GetProperty("service").Value + @""",");

            s.Append(@"""pagetype"":");
            s.Append(@"""" + Node.GetProperty("pageType").Value + @""",");

            s.Append(@"""vuidate"":");
            s.Append(@"""" + Node.GetProperty("vuidate").Value + @""",");


            s.Append(@"""isFavourite"":");
            s.Append(@"""" + IsFavourite.ToString() + @""",");
            s.Append(@"""ratersPersonal"":");
            s.Append(@"""" + personalRating.NumRaters + @""",");

            s.Append(@"""ratingTeam"":");
            s.Append(@"""" + String.Format("{0:0.0}", teamRating.Rating) + @""",");
            s.Append(@"""ratersTeam"":");
            s.Append(@"""" + teamRating.NumRaters + @""",");

            s.Append(@"""ratingOverall"":");
            s.Append(@"""" + String.Format("{0:0.0}", overallRating.Rating) + @""",");
            s.Append(@"""ratersOverall"":");
            s.Append(@"""" + overallRating.NumRaters + @""",");

            s.Append(@"""img_th"":");
            s.Append(@"""" + VUI_mediafolder + @"th/" + Node.GetProperty("thFile").Value.Replace("&","%26") + @""",");

            s.Append(@"""img_lg"":");
            s.Append(@"""" + VUI_mediafolder + @"lg/" + Node.GetProperty("lgFile").Value.Replace("&", "%26") + @""",");

            s.Append(@"""img_full"":");
            s.Append(@"""" + VUI_mediafolder + @"full/" + Node.GetProperty("imageFile").Value.Replace("&", "%26") + @"""");

            s.Append("}");

            return s.ToString();
        }
    }

}