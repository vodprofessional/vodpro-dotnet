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
using System.Xml.XPath;

namespace VUI.classes
{

    public class VUIService
    {

        string VUI_mediafolder = ConfigurationManager.AppSettings["VUI_mediafolder"].ToString().Replace("~", "");

        private bool IsFullMode { get; set; }
        public bool Disabled { get; set; }
        public string ServiceName { get; set; }
        public int ServiceId { get; set; }
        public int NumImages { get; set; }
        public string Device { get; set; }
        public string Platform { get; set; }
        public string thumbimage { get; set; }
        public string Description { get; set; }
        public string Region { get; set; }
        public string ServiceType { get; set; }
        public string ServiceCategory { get; set; }
        public string[] Capabilities { get; set; }
        public int ServiceCount { get; set; }
        public int Score { get; set; }
        public DynamicNode Node { get; set; }
        public VUIRating personalRating { get; set; }
        public VUIRating teamRating { get; set; }
        public VUIRating overallRating { get; set; }
        public List<VUIImage> Screenshots { get; private set; }
        public string ImageURL_th { get { return VUI_mediafolder + @"md/" + thumbimage.Replace("&", "%26"); } }
        public bool IsComingSoon { get; set; }

        public VUIService(int id)
        {
            Init();
            ServiceId = id;
            SetNode();
            SetIsDisabled();
        }

        public VUIService(string s, int id, int n)
        {
            Init();
            ServiceName = s;
            ServiceId = id;
            NumImages = n;

            SetNode();
            SetIsDisabled();
        }

        public VUIService(string s, int id, string p, string d)
        {
            Init();
            ServiceName = s;
            ServiceId = id;
            Platform = p;
            Device = d;

            SetNode();
            SetIsDisabled();
        }

        private void Init()
        {
            Disabled = false;
            ServiceName = String.Empty;
            ServiceId = -1;
            NumImages = 0;
            Device = String.Empty;
            Platform = String.Empty;
            thumbimage = String.Empty;
            Node = null;
            ServiceCount = 1;
            IsComingSoon = false;

            personalRating = new VUIRating(VUIRating.MODE_PERSONAL);
            teamRating = new VUIRating(VUIRating.MODE_TEAM);
            overallRating = new VUIRating(VUIRating.MODE_OVERALL);
        }

        private void SetIsDisabled()
        {
            IsFullMode = VUIfunctions.MemberVUIFullMode(VUIfunctions.MemberVUIStatus(VUIfunctions.CurrentUser()));
            Disabled = !(IsFullMode || ServiceName.Equals(VUIfunctions.VUIPreviewService));
        }

        private void SetNode()
        {
            if (Node == null)
            {
                try
                {
                    Node = new DynamicNode(ServiceId);
                    ServiceName = Node.Name;
                    ServiceId = Node.Id;
                    NumImages = Node.Descendants().Items.Count;

                    if (Node.Parent.GetProperty("folderLevel").Value.ToString().Equals(VUIfunctions.VUI_DEVICE))
                    {
                        Device = Node.Parent.Name;
                        Platform = Node.Parent.Parent.Name;
                    }
                    else if (Node.Parent.GetProperty("folderLevel").Value.ToString().Equals(VUIfunctions.VUI_PLATFORM))
                    {
                        Device = String.Empty;
                        Platform = Node.Parent.Name;
                    }

                    thumbimage = String.Empty;
                    try
                    {
                        thumbimage = Node.Descendants().Items.First().GetProperty("thFile").Value;
                    }
                    catch (Exception ex2) { ; }
                    Description = Node.GetProperty("description").Value;
                    Region = Node.GetProperty("regionAvailability").Value;
                    ServiceType = Node.GetProperty("subscriptionType").Value;
                    if (Node.GetProperty("subscriptionType") != null)
                    {
                        int score = 0;
                        Int32.TryParse(Node.GetProperty("vuiScore").Value, out score);
                        Score = score;
                    }
                    ServiceCategory = Node.GetProperty("serviceCategory").Value;

                    Capabilities = Node.GetProperty("serviceCapabilities").Value.Split(',');

                    // CHeck the Service IsComingSoon
                    if (Node.GetProperty("isComingSoon") != null && !String.IsNullOrEmpty(Node.GetProperty("isComingSoon").Value))
                    {
                        IsComingSoon = Node.GetProperty("isComingSoon").Value.Equals("1");
                    }
                    // Check the Parent Device (but might be platform)
                    if (!IsComingSoon)
                    {
                        if (Node.Parent.NodeTypeAlias.Equals(VUIfunctions.VUI_FOLDERTYPE))
                        {
                            if (Node.Parent.GetProperty("isComingSoon") != null && !String.IsNullOrEmpty(Node.Parent.GetProperty("isComingSoon").Value))
                            {
                                IsComingSoon = Node.Parent.GetProperty("isComingSoon").Value.Equals("1");
                            }
                        }
                    }
                    // Check the Parent Platfomr (but might be something else, hence the NodeTypeLaias check)
                    if (!IsComingSoon)
                    {
                        if (Node.Parent.Parent.NodeTypeAlias.Equals(VUIfunctions.VUI_FOLDERTYPE))
                        {
                            if (Node.Parent.Parent.GetProperty("isComingSoon") != null && !String.IsNullOrEmpty(Node.Parent.Parent.GetProperty("isComingSoon").Value))
                            {
                                IsComingSoon = Node.Parent.Parent.GetProperty("isComingSoon").Value.Equals("1");
                            }
                        }
                    }
                }
                catch (Exception ex) { ; }
            }
        }

        public bool HasCapability(string capability)
        {
            if (Capabilities != null)
            {
                return Capabilities.Contains(capability);
            }
            return false;
        }

        public bool HasAnyCapabilityIn(string[] capabilityList)
        {
            if (Capabilities != null)
            {
                foreach (string c in capabilityList)
                {
                    if (Capabilities.Contains(c))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public void SetImages()
        {
            XPathNodeIterator iterator = umbraco.library.GetPreValues(VUIfunctions.VUI_pagetypelist);
            iterator.MoveNext(); //move to first
            XPathNodeIterator preValues = iterator.Current.SelectChildren("preValue", "");

            List<VUI.classes.VUIfunctions.Preval> prevals = new List<VUI.classes.VUIfunctions.Preval>();
            int sort = 0;

            while (preValues.MoveNext())
            {
                prevals.Add(new VUI.classes.VUIfunctions.Preval(sort++, preValues.Current.Value));
            }
            prevals.Add(new VUI.classes.VUIfunctions.Preval(sort++, ""));

            List<DynamicNode> imageNodes = Node.Descendants(VUIfunctions.VUI_IMAGETYPE).Items.ToList();
            var orderedImages = from image in imageNodes
                                join preval in prevals
                                    on image.GetProperty("pageType").Value equals preval.Val
                                orderby preval.Order
                                select image;


            Screenshots = VUIfunctions.NodeListToVUIImages(orderedImages.ToList<DynamicNode>());
        }
        /// <summary>
        /// Returns in the form /Platform[/Device]/Service
        /// </summary>
        public string WebTileUIURL
        {
            get
            {
                if (String.IsNullOrEmpty(Device))
                {
                    return @"/" + Platform.Replace(' ', '-') + @"/" + ServiceName.Replace(' ', '-');
                }
                else
                {
                    return @"/" + Platform.Replace(' ', '-') + @"/" + ServiceName.Replace(' ', '-') + @"/" + Device.Replace(' ', '-');
                }
            }

        }


        public void SetRatings()
        {
            SetNode();
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

                    try
                    {
                        string ratings = Node.GetProperty("ratings").Value.Replace("{", "").Replace("}", "");
                        XmlDocument xml = new XmlDocument();
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
                    catch (Exception ex) { ; }
                }
            }
        }

        public string GetJson()
        {
            StringBuilder s = new StringBuilder();

            s.Append("{");

            s.Append(@"""id"":");
            s.Append(@"""" + ServiceId + @""",");

            s.Append(@"""platform"":");
            s.Append(@"""" + VUIfunctions.EncodeJsString(Platform) + @""",");

            s.Append(@"""device"":");
            s.Append(@"""" + VUIfunctions.EncodeJsString(Device) + @""",");

            s.Append(@"""service"":");
            s.Append(@"""" + VUIfunctions.EncodeJsString(ServiceName) + @""",");


            s.Append(@"""disabled"":");
            if (Disabled)
            {
                s.Append(@"""yes"",");
            }
            else
            {

                s.Append(@"""no"",");
                s.Append(@"""full"":");
                if (IsFullMode)
                {
                    s.Append(@"""yes"",");
                }
                else
                {
                    s.Append(@"""no"",");
                }
            
                s.Append(@"""ratingPersonal"":");
                s.Append(@"""" + String.Format("{0:0.0}", personalRating.Rating) + @""",");
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

                s.Append(@"""description"":");
                s.Append(@"""" + VUIfunctions.EncodeJsString(Description) + @""",");

                s.Append(@"""region"":");
                s.Append(@"""" + VUIfunctions.EncodeJsString(Region) + @""",");

                s.Append(@"""types"":");
                s.Append(@"""" + VUIfunctions.EncodeJsString(ServiceType) + @""",");

                s.Append(@"""cats"":");
                s.Append(@"""" + VUIfunctions.EncodeJsString(ServiceType) + @""",");

                s.Append(@"""vuiscore"":");
                s.Append(@"""" + Score.ToString() + @""",");

                s.Append(@"""numimages"":");
                s.Append(@"""" + NumImages.ToString() + @""",");

            }

            s.Append(@"""img_th"":");
            s.Append(@"""" + VUI_mediafolder + @"md/" + thumbimage.Replace("&","%26") + @"""");

            s.Append("}");

            return s.ToString();
        }

        public string GetJsonInjectImages(string ImageJson)
        {
            string json = GetJson();

            json = json.Substring(0, json.Length - 1) + @", ""images"": " + ImageJson + " }";

            return json;
        }
    }

}