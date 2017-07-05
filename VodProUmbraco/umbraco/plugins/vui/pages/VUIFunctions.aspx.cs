using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using VUI.VUI3.classes;
using umbraco.MacroEngines;
using umbraco.cms.businesslogic.web;
using umbraco.cms.businesslogic.member;

namespace VUI.pages
{
    public partial class VUIFunctions : umbraco.BasePages.UmbracoEnsuredPage
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(VUIFunctions));

        protected void Page_Load(object sender, EventArgs e)
        {
            VUIAdminsAndUsers();


            
            if (!Page.IsPostBack)
            {
                RefreshServiceMasterList();
            }
        }

        protected void RefreshServiceMasterList()
        {
            try
            {

                DynamicNode smRoot = new DynamicNode(ConfigurationManager.AppSettings["VUI2_ServiceMastersRoot"]);
                List<DynamicNode> serviceMasters = smRoot.Descendants("VUI2ServiceMaster").Items.OrderBy(n => n.Name).ToList();

                foreach (DynamicNode n in serviceMasters)
                {
                    lstServices.Items.Add(new ListItem(n.Name, n.Name));
                }
            }
            catch (Exception ex)
            {
                log.Error("Error making service list", ex);
            }
        }

        protected void DeleteChildren(object sender, EventArgs e)
        {
            int id = -1;

            if (Int32.TryParse(txtParentId.Text, out id))
            {
                try
                {
                    Document parent = new Document(id);
                    if (parent.Children.Count() > 0)
                    {
                        int counter = 0;
                        foreach (Document child in parent.Children)
                        {
                            child.delete();
                            counter++;
                        }
                        litDeleteChildren.Text = "Deleted " + counter + " child nodes";
                    }
                    else
                    {
                        litDeleteChildren.Text = "No child nodes";
                    }
                }
                catch (Exception ex)
                {
                    litDeleteChildren.Text = "Could not delete. Check ID";
                }
            }
            else
            {
                litDeleteChildren.Text = "Not an ID";
            }
        }

        protected void UpdateServiceMasterName(object sender, EventArgs e)
        {
            string existingName = lstServices.SelectedValue;
            string newName = txtNewServiceName.Text;

            // Update Services
            DynamicNode vui_root = new DynamicNode(ConfigurationManager.AppSettings["VUI2_rootnodeid"]);
            List<DynamicNode> services = vui_root.Descendants("VUI2Service").Items.Where(n => n.Name.Equals(existingName)).ToList();

            foreach (DynamicNode svc in services)
            {
                Document svcDoc = new Document(svc.Id);
                svcDoc.Text = newName;
                svcDoc.Save();
                if (svcDoc.Published)
                {
                    svcDoc.Publish(umbraco.BasePages.UmbracoEnsuredPage.CurrentUser);
                }
            }

            // Update ServiceMaster
            DynamicNode smRoot = new DynamicNode(ConfigurationManager.AppSettings["VUI2_ServiceMastersRoot"]);
            List<DynamicNode> serviceMasters = smRoot.Descendants("VUI2ServiceMaster").Items.Where(n => n.Name.Equals(existingName)).ToList();

            foreach (DynamicNode svc in serviceMasters)
            {
                Document svcDoc = new Document(svc.Id);
                svcDoc.Text = newName;
                svcDoc.getProperty("serviceName").Value = newName;
                svcDoc.Save();
                if (svcDoc.Published)
                {
                    svcDoc.Publish(umbraco.BasePages.UmbracoEnsuredPage.CurrentUser);
                    string s = umbraco.library.NiceUrl(svc.Id);
                    umbraco.library.UpdateDocumentCache(svc.Id);
                    s = umbraco.library.NiceUrl(svc.Id);
                }
            }

            umbraco.library.RefreshContent();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "vui_ChangeServiceMasterName";
                comm.Parameters.AddWithValue("@ServiceMasterName", existingName);
                comm.Parameters.AddWithValue("@ServiceMasterNameNew", newName);
                comm.ExecuteNonQuery();
                conn.Close();
            }

            txtNewServiceName.Text = String.Empty;
            RefreshServiceMasterList();

        }

        protected void VUIAdminsAndUsers()
        {
            if (false)
            {
                string outstr = String.Empty;

                string sql1 = @" SELECT    MG.Member, M.LoginName, PD1.dataDate as EndDate, PD2.dataNVarchar as CompanyName
                            FROM       cmsMember2MemberGroup MG 
                            inner join umbracoNode AD  on MG.MemberGroup = AD.iD
                            inner join cmsMember M on MG.Member = M.nodeId
                            inner join cmsPropertyData PD1 on M.nodeId = PD1.contentNodeId
                            inner join cmsPropertyType PT1 on PD1.propertytypeid = PT1.id
                            inner join cmsPropertyData PD2 on M.nodeId = PD2.contentNodeId
                            inner join cmsPropertyType PT2 on PD2.propertytypeid = PT2.id
                            where      AD.text='vui_administrator'
                            and        PT1.Alias = 'vuiEndDate'
                            and        PT2.Alias = 'companyName'
                            order by   CompanyName ";

                string sql2 = @" SELECT    MG.Member, M.LoginName, PT1.Alias, PD1.dataInt
                            FROM       cmsMember2MemberGroup MG 
                            inner join umbracoNode AD  on MG.MemberGroup = AD.iD
                            inner join cmsMember M on MG.Member = M.nodeId
                            inner join cmsPropertyData PD1 on M.nodeId = PD1.contentNodeId
                            inner join cmsPropertyType PT1 on PD1.propertytypeid = PT1.id
                            where      AD.text='vui_user'
                            and        PT1.Alias = 'vuiAdministrator'
                            and        PD1.dataInt = @adminid 
                            order by   M.LoginName ";

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand(sql1, conn);
                    SqlCommand comm2 = new SqlCommand(sql2, conn);


                    SqlDataReader sr = comm.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(sr);
                    sr.Close();

                    outstr += @"<table class=""vui-users"">";
                    outstr += @"<tr><th>Company</th><th>Subscription End Date</th><th>Users</th></tr>";

                    foreach (DataRow r in dt.Rows)
                    {

                        int mid = (Int32)r["Member"];
                        string loginname = (String)r["LoginName"];

                        log.Debug(loginname);

                        string companyname = (String)r["CompanyName"];
                        string enddate = String.Empty;
                        if (r["EndDate"].GetType() != typeof(DBNull))
                        {
                            enddate = ((DateTime)r["EndDate"]).ToString("dd MMM yyy");
                        }
                        else
                        {
                            enddate = @"NOT SET!";
                        }

                        string s = @"<tr class=""admin""><td>{3}</td><td>{2}</td><td><a href=""/umbraco/members/editMember.aspx?id={0}""><strong>{1}</strong></a></td></tr>";
                        outstr += String.Format(s, new object[] { mid, loginname, enddate, companyname });

                        comm2.Parameters.Clear();
                        comm2.Parameters.AddWithValue("@adminid", (Int32)r["Member"]);

                        SqlDataReader sr2 = comm2.ExecuteReader();

                        while (sr2.Read())
                        {
                            outstr += @"<tr class=""user""><td></td><td></td><td><a href=""/umbraco/members/editMember.aspx?id=" + (Int32)sr2["Member"] + @"#member"">
                                " + (String)sr2["LoginName"] + @" </a> </td></tr>";
                        }
                        sr2.Close();
                        outstr += @"</ul></li>";
                    }
                    outstr += "</table>";
                    conn.Close();
                    litVUIUsers.Text = outstr;
                }
            }
        }


        protected List<Document> AllAnalyses()
        {
            List<Document> analyses = new List<Document>();
            Document vui_root = new Document(Int32.Parse(VUI2.classes.Utility.GetConst("VUI2_rootnodeid")));
            

            // Find Platofmrs
            foreach (Document p in vui_root.Children)
            {
                if (p.ContentType.Alias.Equals("VUI2Platform"))
                {
                    foreach (Document d in p.Children)
                    {
                        if (d.ContentType.Alias.Equals("VUI2Service"))
                        {
                            foreach (Document a in d.Children)
                            {
                                if (a.ContentType.Alias.Equals("VUI2Analysis"))
                                {
                                    analyses.Add(a);
                                }
                            }
                        }
                        else if (d.ContentType.Alias.Equals("VUI2Device"))
                        {
                            foreach (Document s in d.Children)
                            {
                                if (s.ContentType.Alias.Equals("VUI2Service"))
                                {
                                    foreach (Document a in s.Children)
                                    {
                                        if (a.ContentType.Alias.Equals("VUI2Analysis"))
                                        {
                                            analyses.Add(a);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return analyses;
        }

        protected List<int> AllImageIDs()
        {
            List<int> images = new List<int>();
            Document vui_root = new Document(Int32.Parse(VUI2.classes.Utility.GetConst("VUI2_rootnodeid")));


            // Find Platofmrs
            foreach (Document p in vui_root.Children)
            {
                if (p.ContentType.Alias.Equals("VUI2Platform"))
                {
                    foreach (Document d in p.Children)
                    {
                        if (d.ContentType.Alias.Equals("VUI2Service"))
                        {
                            foreach (Document a in d.Children)
                            {
                                if (a.ContentType.Alias.Equals("VUI2Analysis"))
                                {
                                    foreach (Document i in a.Children)
                                    {
                                        if (i.ContentType.Alias.Equals("VUI_Image"))
                                        {
                                            images.Add(i.Id);
                                        }
                                    }
                                }
                            }
                        }
                        else if (d.ContentType.Alias.Equals("VUI2Device"))
                        {
                            foreach (Document s in d.Children)
                            {
                                if (s.ContentType.Alias.Equals("VUI2Service"))
                                {
                                    foreach (Document a in s.Children)
                                    {
                                        if (a.ContentType.Alias.Equals("VUI2Analysis"))
                                        {
                                            foreach (Document i in a.Children)
                                            {
                                                if (i.ContentType.Alias.Equals("VUI_Image"))
                                                {
                                                    images.Add(i.Id);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return images;
        }


        private void UnpublishWithChildren(Document parent, umbraco.BusinessLogic.User u, int level)
        {
            string indent = "";
            for (int i=0; i<level+1; i++)
            {
                indent += "-";
            }

            log.Debug(" Unpublishing " + indent + " " + parent.Path);
            parent.UnPublish();
            umbraco.library.UnPublishSingleNode(parent.Id);
            try {
                umbraco.library.UpdateDocumentCache(parent.Id);
            }
            catch (Exception ex)
            {
                log.Error(indent + "- Something went wrong");
            }

            if (parent.HasChildren)
            {
                foreach (Document d in parent.GetDescendants())
                {
                    UnpublishWithChildren(d, u, level+1);
                }
            }
        }

        protected void UnpublishContent(object sender, EventArgs e)
        {

            umbraco.BusinessLogic.User u = umbraco.BusinessLogic.User.GetAllByLoginName("websitecontentuser", false).First();
            umbraco.library.UpdateDocumentCache(1058);

            UnpublishWithChildren(new Document(1059), u, 1); //News
            UnpublishWithChildren(new Document(1077), u, 1); //Research
            UnpublishWithChildren(new Document(1078), u, 1); //Features
            UnpublishWithChildren(new Document(36404), u, 1); //Calendar
            UnpublishWithChildren(new Document(1079), u, 1); //Reviews
            UnpublishWithChildren(new Document(1913), u, 1); //Blog
        }



        protected void btnSetAnalyses2016_Click(object sender, EventArgs e)
        {

            List<Document> analyses = AllAnalyses();

            umbraco.BusinessLogic.User u = umbraco.BusinessLogic.User.GetAllByLoginName("websitecontentuser", false).First();
            int i = 0;
            foreach (Document analysis in analyses)
            {
                analysis.getProperty("pre2016Score").Value = true;
                analysis.getProperty("updateComplete2016").Value = false;
                analysis.Save();
            }            
        }

        protected void btnSetImagePageTypes2016_Click(object sender, EventArgs e)
        {
            List<int> imageids = AllImageIDs();

            umbraco.BusinessLogic.User u = umbraco.BusinessLogic.User.GetAllByLoginName("websitecontentuser", false).First();
            int i = 0;

            foreach(int id in imageids)
            {
                Document image = new Document(id);

                bool updated = false;
                object upd = image.getProperty("updateComplete2016").Value;
                if (upd != null)
                {
                    bool.TryParse(upd.ToString(), out updated);

                }
                if (!updated)
                {
                    log.Debug(" - Updating image " + image.Text + "{" + image.Id + "} at " + image.Path);

                    try
                    {
                        string pt = image.getProperty("pageType").Value as string;

                    }
                    catch(Exception ex)
                    { }
                }
            }

        }



        protected void btnSetAnalysesScores2016_Click(object sender, EventArgs e)
        {
            Init2016PageTypePreVals();

            List<Document> analyses = AllAnalyses();

            umbraco.BusinessLogic.User u = umbraco.BusinessLogic.User.GetAllByLoginName("websitecontentuser", false).First();
            int i = 0;
            foreach (Document analysis in analyses)
            {
                bool updated = false;
                object upd = analysis.getProperty("updateComplete2016").Value;
                if (upd != null)
                {
                    bool.TryParse(upd.ToString(), out updated);

                }

                if (!updated)
                {
                    bool shouldPublish = analysis.Published;

                    log.Debug(" Updating analysis " + analysis.Text + "{" + analysis.Id + "} at " + analysis.Path);

                    try
                    {
                        string sc = analysis.getProperty("serviceCapabilities").Value as string;
                        string hf = analysis.getProperty("hotFeatures").Value as string;

                        string[] scidarr = string.Concat(sc, ",", hf).Split(',');

                        log.Debug(" -- Found Old scores " + sc);

                        List<string> newids = new List<string>();

                        foreach (string s in scidarr)
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                string oldval = scoringFeatureVals[s];
                                if (oldval.Equals("Playlists"))
                                {
                                    oldval = "Favourites";
                                    log.Debug(" -- rationalising Playlists to Favourites ");
                                }
                                string newid = scoringFeature2016Vals[oldval];
                                log.Debug(" -- Old [" + oldval + "] goes to [" + newid + "]");
                                newids.Add(newid);
                            }
                        }
                        analysis.getProperty("features2016").Value = string.Join(",", newids.Distinct());
                        analysis.getProperty("updateComplete2016").Value = true;

                        i++;
                    }
                    catch (Exception ex)
                    {
                        log.Error(" -- !! Error doing analyisis ", ex);
                    }
                    analysis.Save();

                    if (shouldPublish)
                    {
                        try
                        {
                            analysis.Publish(u);
                            umbraco.library.UpdateDocumentCache(analysis.Id);
                        }
                        catch(Exception exp)
                        {
                            log.Error(" -- !! Error publishing ", exp);
                        }
                    }
                }
            }
        }


        protected List<Member> regMembers = new List<Member>();
        protected List<Member> vuiMembers = new List<Member>();

        protected void btnExportMembers_Click(object sender, EventArgs e)
        {
            List<Member> allmembers = Member.GetAllAsList().ToList();

            MemberGroup mg = MemberGroup.GetByName("registrant");
            regMembers.AddRange(mg.GetMembers("%").ToList<Member>());

            mg = MemberGroup.GetByName("vui_user");
            vuiMembers.AddRange(mg.GetMembers("%").ToList<Member>());
            mg = MemberGroup.GetByName("vui_administrator");
            vuiMembers.AddRange(mg.GetMembers("%").ToList<Member>());


            rptUsers.DataSource = allmembers;
            rptUsers.DataBind();
        }

        protected void User_DataBound(object sender, RepeaterItemEventArgs e)
        {
            if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Member m = (Member)e.Item.DataItem;

                Literal litID = e.Item.FindControl("litID") as Literal;
                Literal litEmail = e.Item.FindControl("litEmail") as Literal;
                Literal litTitle = e.Item.FindControl("litTitle") as Literal;
                Literal litFirstName = e.Item.FindControl("litFirstName") as Literal;
                Literal litSurname = e.Item.FindControl("litSurname") as Literal;
                Literal litFullname = e.Item.FindControl("litFullname") as Literal;
                Literal litJobTitle = e.Item.FindControl("litJobTitle") as Literal;
                Literal litOrganisation = e.Item.FindControl("litOrganisation") as Literal;
                Literal litOrganisationType = e.Item.FindControl("litOrganisationType") as Literal;
                Literal litEmplyees = e.Item.FindControl("litEmplyees") as Literal;
                Literal litCountry = e.Item.FindControl("litCountry") as Literal;
                Literal litAddress1 = e.Item.FindControl("litAddress1") as Literal;
                Literal litAddress2 = e.Item.FindControl("litAddress2") as Literal;
                Literal litCity = e.Item.FindControl("litCity") as Literal;
                Literal litState = e.Item.FindControl("litState") as Literal;
                Literal litPostcode = e.Item.FindControl("litPostcode") as Literal;
                Literal litPhone = e.Item.FindControl("litPhone") as Literal;
                Literal litMobile = e.Item.FindControl("litMobile") as Literal;
                Literal litRegistrant = e.Item.FindControl("litRegistrant") as Literal;
                Literal litNewsletter = e.Item.FindControl("litNewsletter") as Literal;
                Literal litVui = e.Item.FindControl("litVui") as Literal;
                Literal litDate = e.Item.FindControl("litDate") as Literal;

                litID.Text = m.Id.ToString();
                litEmail.Text = m.Email;
                litTitle.Text = m.getProperty("title").Value.ToString();
                litFirstName.Text = m.getProperty("firstName").Value.ToString();
                litSurname.Text = m.getProperty("lastName").Value.ToString();
                litFullname.Text = m.getProperty("fullName").Value.ToString();
                litJobTitle.Text = m.getProperty("jobTitle").Value.ToString();
                litOrganisationType.Text = m.getProperty("organisationType").Value.ToString() + m.getProperty("organisationTypeOther").Value.ToString();
                litOrganisation.Text = m.getProperty("companyName").Value.ToString();
                litEmplyees.Text = m.getProperty("numberOfEmployees").Value.ToString();
                litCountry.Text = m.getProperty("companyCountry").Value.ToString();
                litAddress1.Text = m.getProperty("companyAddress1").Value.ToString();
                litAddress2.Text = m.getProperty("companyAddress2").Value.ToString();
                litCity.Text = m.getProperty("companyTown").Value.ToString();
                litState.Text = m.getProperty("companyState").Value.ToString();
                litPostcode.Text = m.getProperty("companyPostcodeZip").Value.ToString();
                litPhone.Text = m.getProperty("businessPhone").Value.ToString();
                litMobile.Text = m.getProperty("mobile").Value.ToString();
                litRegistrant.Text = regMembers.Contains(m) ? "Y" : "";
                litNewsletter.Text = m.getProperty("receivePromoEmails").Value.ToString().Equals("1") ? "Y" : "";
                litVui.Text = vuiMembers.Contains(m) ? "Y" : "";
                litDate.Text = m.CreateDateTime.ToString("dd/MM/yyyy");

                
            }
        }


        private Dictionary<string, string> scoringFeature2016Vals = new Dictionary<string, string>();
        private Dictionary<string, string> scoringFeatureVals = new Dictionary<string, string>();

        private Dictionary<string, string> pageType2016Vals = new Dictionary<string, string>();
        private Dictionary<string, string> pageTypeVals = new Dictionary<string, string>();
        private Dictionary<string, string> pageTypeMapping = new Dictionary<string, string>();

        private void Init2016PageTypePreVals()
        {
            scoringFeature2016Vals.Clear();

            // Note this is in the form "Value","id"
            string sql = String.Format(@"select value, id from [dbo].[cmsDataTypePreValues] where datatypeNodeId = {0}", VUI2.classes.Utility.GetConst("VUI_Scoring2016Features"));
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    scoringFeature2016Vals.Add(sr[0].ToString(), sr[1].ToString());
                }
                conn.Close();
            }
       
            scoringFeatureVals.Clear();

            // ScoringPageTypes and HotFeatures
            sql = String.Format(@"select id, value from [dbo].[cmsDataTypePreValues] where datatypeNodeId IN ( {0}, {1} )", VUI2.classes.Utility.GetConst("VUI_ScoringPageTypes"), VUI2.classes.Utility.GetConst("VUI_hotfeatures_list"));
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    scoringFeatureVals.Add(sr[0].ToString(), sr[1].ToString());
                }
                conn.Close();
            }




            pageType2016Vals.Clear();

            // Note this is in the form "Value","id"
            sql = String.Format(@"select value, id from [dbo].[cmsDataTypePreValues] where datatypeNodeId = {0}", VUI2.classes.Utility.GetConst("VUI_2016PageTypes"));
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    pageType2016Vals.Add(sr[0].ToString(), sr[1].ToString());
                }
                conn.Close();
            }


            pageTypeVals.Clear();

            sql = String.Format(@"select id, value from [dbo].[cmsDataTypePreValues] where datatypeNodeId = {0}", VUI2.classes.Utility.GetConst("VUI_pagetypelist"));
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["umbracoDbDSN"].ToString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataReader sr = comm.ExecuteReader();
                while (sr.Read())
                {
                    pageTypeVals.Add(sr[0].ToString(), sr[1].ToString());
                }
                conn.Close();
            }

            // This maps the OLD Page Type to the new
            pageTypeMapping.Clear();
            pageTypeMapping.Add("Splash page", "");
            pageTypeMapping.Add("Homepage", "");
            pageTypeMapping.Add("Category homepage", "Category homepage");
            pageTypeMapping.Add("Categorisation", "Categorisation");
            pageTypeMapping.Add("Video description", "Video description");
            pageTypeMapping.Add("Branded video player", "");
            pageTypeMapping.Add("Native video player", "");
            pageTypeMapping.Add("YouTube video player", "");
            pageTypeMapping.Add("Content recommendation", "Content recommendation");
            pageTypeMapping.Add("Most popular", "Most popular");
            pageTypeMapping.Add("Recently added", "Recently added");
            pageTypeMapping.Add("Favourites", "Favourites");
            pageTypeMapping.Add("Viewing history", "");
            pageTypeMapping.Add("EPG", "EPG");
            pageTypeMapping.Add("Help", "Help");
            pageTypeMapping.Add("Watch on other platforms", "Watch on other platforms");
            pageTypeMapping.Add("Parental controls", "Parental controls");
            pageTypeMapping.Add("Contact", "Contact");
            pageTypeMapping.Add("About", "About");
            pageTypeMapping.Add("Search", "Search");
            pageTypeMapping.Add("Predictive search", "Predictive search");
            pageTypeMapping.Add("A-Z page", "A-Z page");
            pageTypeMapping.Add("Playlists", "Favourites");
            pageTypeMapping.Add("Sign in / Register", "Sign in / Register");
            pageTypeMapping.Add("Social sharing (out)", "Social sharing (out)");
            pageTypeMapping.Add("Social sharing (in-service)", "Social sharing (in-service)");
            pageTypeMapping.Add("Buy / Subscribe", "In-app purchasing");
            pageTypeMapping.Add("Special functionality", "");
            pageTypeMapping.Add("Accessibility", "Accessibility");
            pageTypeMapping.Add("Advertising functionality", "");
            pageTypeMapping.Add("Social sign-on", "Social sign-on");
            pageTypeMapping.Add("Customised video player", "Customised video player");
            pageTypeMapping.Add("Featured content", "Featured content");
            pageTypeMapping.Add("More episodes", "More episodes");
            pageTypeMapping.Add("Device synchronisation", "Device synchronisation");
            pageTypeMapping.Add("Adaptive bitrate streaming", "Adaptive bitrate streaming");
            pageTypeMapping.Add("Resume after stopping", "Resume after stopping");
            pageTypeMapping.Add("Download to device", "Download to device");
            pageTypeMapping.Add("Live Restart", "Live Restart");
            pageTypeMapping.Add("Last Viewed", "");
            pageTypeMapping.Add("Live Viewing", "Live Viewing");
            pageTypeMapping.Add("Extended Archive", "Extended Archive");
            pageTypeMapping.Add("Social recommendation", "Social content recommendation");
            pageTypeMapping.Add("Audio-described Shows", "Audio-described Shows");
        }
    }
}