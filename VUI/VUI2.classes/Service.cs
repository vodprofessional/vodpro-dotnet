using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.MacroEngines;
using umbraco.cms.businesslogic.member;
using VUI.classes;
using System.Xml;
using System.Xml.XPath;

namespace VUI.VUI2.classes
{
    public class Service
    {
        
        private int _nodeId;
        private DynamicNode _node;
        private Device _device = null;
        private Platform _platform = null;
        private Boolean _parentsSet = false;
        private Boolean _ratingsSet = false;
        private Boolean _analysesSet = false;
        private Boolean _imagesSet = false;
        
        public Service(int nodeId)
        {
            _nodeId = nodeId;
            _node = new DynamicNode(_nodeId);
            Init();
        }
        public Service(DynamicNode node)
        {
            _node = node;
            _nodeId = _node.Id;
            Init();
        }

        /// <summary>
        /// Set the list of Analyses before using
        /// </summary>
        public void SetAnalyses(bool setScreenshots)
        {
            if (!_analysesSet)
            {
                List<DynamicNode> analysisNodes = Node.Descendants(Utility.GetConst("VUI2_analysistype")).Items.OrderByDescending(n => n.GetProperty("analysisDate").Value).ToList();
                foreach (DynamicNode an in analysisNodes)
                {
                    Analysis a = new Analysis(an);
                    if (setScreenshots)
                    {
                        a.SetScreenshots();
                    }
                    Analyses.Add(a);
                }
                _analysesSet = true;
            }
        }

        private void Init()
        {
            Analyses = new List<Analysis>();
            PersonalRating = new Rating(Rating.MODE_PERSONAL);
            TeamRating = new Rating(Rating.MODE_TEAM);
            OverallRating = new Rating(Rating.MODE_OVERALL);
        }


        public void SetRatings(Member m)
        {
            if (!_ratingsSet)
            {
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
                                    PersonalRating.AddRating(rating);
                                    TeamRating.AddRating(rating);
                                    OverallRating.AddRating(rating);
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
                                        TeamRating.AddRating(rating);
                                        OverallRating.AddRating(rating);
                                    }

                                    else
                                    {
                                        OverallRating.AddRating(rating);
                                    }
                                }
                            }
                        }
                        catch (Exception ex) { ; }
                    }
                }
                _ratingsSet = true;
            }
        }


        public static List<Service> FindServices(string serviceName)
        {

            return VUIDataFunctions.FindServiceOnAllPlatforms(serviceName);
            /*
            DynamicNode rootNode = new DynamicNode(Utility.GetConst("VUI2_rootnodeid"));
            List<Service> services = new List<Service>();
            List<DynamicNode> nodes = rootNode.Descendants(Utility.GetConst("VUI2_servicetype")).Items.Where(n => n.Name.Equals(name)).ToList();
            if (nodes.Count > 0)
            {
                foreach (DynamicNode node in nodes)
                {
                    Service s = new Service(node);
                    services.Add(s);
                }
                return services;
            }
            else
            {
                return null;
            }
             * */
        }


        public Service FindService(string name)
        {
            List<DynamicNode> nodes = _node.Descendants(Utility.GetConst("VUI2_servicetype")).Items.Where(n => n.Name.Equals(name)).ToList();
            if (nodes.Count > 0)
            {
                return new Service(nodes.First());
            }
            else
            {
                return null;
            }
        }

        public static Service GetService(int nodeId)
        {
            return new Service(nodeId);
        }


        #region Public Properties


        public List<Analysis> Analyses { get; private set; }
        /// <summary>
        /// Return only the Analyses that have benchmarking scores
        /// </summary>
        public List<Analysis> Benchmarks
        {
            get
            {
                List<Analysis> _benchmarks = new List<Analysis>();
                if (!_analysesSet)
                {
                    SetAnalyses(true);
                }
                foreach (Analysis a in Analyses)
                {
                    if (a.BenchmarkScore > 0)
                    {
                        a.SetBenchmark();
                        _benchmarks.Add(a);
                    }
                }
                return _benchmarks;
            }
        }

        public bool HasBenchmark
        {
            get
            {
                if (!_analysesSet)
                {
                    SetAnalyses(true);
                }
                bool hasBenchmark = false;
                foreach (Analysis a in Analyses)
                {
                    if (a.BenchmarkScore > 0)
                    {
                        hasBenchmark = true;
                        break;
                    }
                }
                return hasBenchmark;
            }
        }

        public int BenchmarkScore
        {
            get
            {
                if (!_analysesSet)
                {
                    SetAnalyses(true);
                }
                int benchmarkScore = 0;
                foreach (Analysis a in Analyses)
                {
                    if (a.BenchmarkScore > 0)
                    {
                        benchmarkScore = a.BenchmarkScore;
                        break;
                    }
                }
                return benchmarkScore;
            }
        }

        public int ScreenshotCount
        {
            get
            {
                int i = 0;
                foreach (Analysis a in Analyses)
                {
                    i += a.Screenshots.Count;
                }
                return i;
            }
        }
        public Rating PersonalRating { get; set; }
        public Rating TeamRating { get; set; }
        public Rating OverallRating { get; set; }

        public Device Device
        {
            get
            {
                if (!_parentsSet) { InitParents(); }
                return _device;
            }
        }
        public Platform Platform
        {
            get
            {
                if (!_parentsSet) { InitParents(); }
                return _platform;
            }
        }
        public string Availability { get { return _node.GetProperty("availability").Value; } }
        public string SubscriptionType { get { return _node.GetProperty("subscriptionType").Value; } }
        public string ServiceCategory { get { return _node.GetProperty("serviceCategory").Value; } }
        public string Description { get { return _node.GetProperty("description").Value; } }
        public string MarketplaceRating
        {
            get
            {
                if (!_analysesSet)
                {
                    SetAnalyses(true);
                }
                string marketplaceRating = String.Empty;
                foreach (Analysis a in Analyses)
                {
                    if (!String.IsNullOrEmpty(a.MarketplaceRating))
                    {
                        marketplaceRating = a.MarketplaceRating;
                        break;
                    }
                }
                return marketplaceRating;
            }
        }

        public bool HasHotFeatures
        {
            get
            {
                if (!_analysesSet)
                {
                    SetAnalyses(true);
                }
                bool hhf = false;
                foreach (Analysis a in Analyses)
                {
                    if (!a.HideHotFeatures)
                    {
                        hhf = true;
                        break;
                    }
                }
                return hhf;
            }
        }

        public string ServiceName { get { return Utility.NiceUrlName(_node.Id); } }
        public int NodeId { get { return _nodeId; } }
        public DynamicNode Node { get { return _node; } }
        public string Name { get { return _node.Name; } }
        public bool IsComingSoon 
        { 
            get 
            {
                bool _ics = false;
                bool.TryParse(_node.GetProperty("isComingSoon").Value, out _ics);
                return _ics;
            } 
        }

        public string DefaultScreenshotMedium
        {
            get
            {
                string defaultScreenshot = String.Empty;
                string defaultScreenshotId = _node.GetProperty("defaultScreenshot").Value;
                if (!String.IsNullOrEmpty(defaultScreenshotId))
                {
                    DynamicNode img = new DynamicNode(defaultScreenshotId);
                    string thumbimage = img.GetProperty("thFile").Value;
                    defaultScreenshot = thumbimage.Replace("&", "%26");
                }
                else
                {
                    foreach (Analysis a in Analyses)
                    {
                        if (a.HasScreenshots)
                        {
                            defaultScreenshot = a.Screenshots.First().ImageURL_md;
                            return defaultScreenshot;
                        }
                    }
                }
                if (!String.IsNullOrEmpty(defaultScreenshot))
                {
                    return Utility.GetConst("VUI_mediafolder").Replace("~", "") + @"md/" + defaultScreenshot;
                }
                else
                {
                    return Utility.GetConst("VUI_mediafolder").Replace("~", "") + @"md/holding-screenshot.png";
                }
            }
        }

        public string DefaultScreenshot 
        { 
            get 
            {
                string defaultScreenshot = String.Empty;
                string defaultScreenshotId = _node.GetProperty("defaultScreenshot").Value;
                if (!String.IsNullOrEmpty(defaultScreenshotId))
                {
                    DynamicNode img = new DynamicNode(defaultScreenshotId);
                    string thumbimage = img.GetProperty("thFile").Value;
                    defaultScreenshot = thumbimage.Replace("&", "%26");
                }
                else
                {
                    foreach (Analysis a in Analyses)
                    {
                        if (a.HasScreenshots)
                        {
                            defaultScreenshot = a.Screenshots.First().ImageURL_th;
                            return defaultScreenshot;
                        }
                    }
                }
                if (!String.IsNullOrEmpty(defaultScreenshot))
                {
                    return Utility.GetConst("VUI_mediafolder").Replace("~", "") + @"md/" + defaultScreenshot;
                }
                else
                {
                    return Utility.GetConst("VUI_mediafolder").Replace("~", "") + @"md/holding-screenshot.png";
                }
            }
        }


        #endregion

        // The parents of a Service are important - a Platform and Device (optional)
        private void InitParents()
        {
            if (_node.Parent.NodeTypeAlias.Equals(Utility.GetConst("VUI2_devicetype")))
            {
                _device = new Device(_node.Parent);

                if (_node.Parent.Parent.NodeTypeAlias.Equals(Utility.GetConst("VUI2_platformtype")))
                {
                    _platform = new Platform(_node.Parent.Parent);
                }
            }
            else if (_node.Parent.NodeTypeAlias.Equals(Utility.GetConst("VUI2_platformtype")))
            {
                _platform = new Platform(_node.Parent);
            }
            _parentsSet = true;
        }

        public bool IsPreviewable
        {
            get
            {
                return Name.Equals(Utility.GetConst("VUI_previewservice"));
            }
        }

        public static bool ServiceIsPreviewable(string name)
        {
            return name.Equals(Utility.GetConst("VUI_previewservice"));
        }



    }
}