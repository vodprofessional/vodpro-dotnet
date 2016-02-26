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
                    outstr += String.Format(s, new object[] {mid, loginname, enddate, companyname });

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
}