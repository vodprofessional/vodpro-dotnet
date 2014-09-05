using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using umbraco.NodeFactory;
using umbraco.cms.businesslogic.web;

namespace VODPro.usercontrols
{
    public partial class VPSPIList : System.Web.UI.UserControl
    {
        int SPIParent ;
        int SPIDocType;

        protected void Page_Load(object sender, EventArgs e)
        {
            SPIParent = Int32.Parse(ConfigurationManager.AppSettings["VPSPIParent"].ToString());
            SPIDocType= Int32.Parse(ConfigurationManager.AppSettings["VPSPIDocType"].ToString());
            

            if (!IsPostBack)
            {
                GetSPIDocs();
                spiGreyedout.Style.Add("display", "none");
            }
        }

        protected void btnSaveSPI_Click(object sender, System.EventArgs e)
        {

            string secret = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 8);
            string companyName = txtNewSPI.Text;
            // 1. Create new Document

            // The documenttype that should be used, replace 10 with the id of your documenttype
            umbraco.cms.businesslogic.web.DocumentType dt = new umbraco.cms.businesslogic.web.DocumentType(SPIDocType);

            // The umbraco user that should create the document,
            // 0 is the umbraco system user, and always exists

            umbraco.BusinessLogic.User u = new umbraco.BusinessLogic.User("websitecontentuser");

            // Create the document
            umbraco.cms.businesslogic.web.Document d = umbraco.cms.businesslogic.web.Document.MakeNew("SPI-" + companyName.Replace(' ', '-'), dt, u, SPIParent);

            d.getProperty("companyName").Value = companyName;
            d.getProperty("secretString").Value = secret;
            d.Save();

            txtSPIURL.Text = "https://" + HttpContext.Current.Request.Url.Host + umbraco.library.NiceUrl(SPIParent) + d.Id.ToString() + "/" + secret;
            spiGreyedout.Style.Add("display", "inline");
            GetSPIDocs();
        }

        protected void GetSPIDocs()
        {
            Document d = new Document(SPIParent);

            SPIList.DataSource = d.Children;
            SPIList.DataBind();
        }

        protected void SPIListItemBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Document n = (Document)e.Item.DataItem;

                Literal company = e.Item.FindControl("lt_Company") as Literal;
                Literal saved = e.Item.FindControl("lt_Saved") as Literal;
                Literal completed = e.Item.FindControl("lt_Completed") as Literal;
                HyperLink url = e.Item.FindControl("lt_URL") as HyperLink;

                company.Text = n.Text; // n.getProperty("companyName").Value.ToString();
                saved.Text = n.getProperty("userUpdated").Value.ToString().Equals("1") ? "Yes" : "";
                completed.Text = n.getProperty("isCompleted").Value.ToString().Equals("1") ? "Yes" : "";
                url.Text = "https://" + HttpContext.Current.Request.Url.Host + umbraco.library.NiceUrl(SPIParent) + n.Id.ToString() + "/" + n.getProperty("secretString").Value.ToString();
                url.NavigateUrl = url.Text;
                url.Target = "vpspi";
            }
        }

    }
}