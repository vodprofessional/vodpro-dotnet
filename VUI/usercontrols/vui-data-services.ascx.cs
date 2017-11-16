using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.MacroEngines;

namespace VUI.usercontrols
{
    public partial class vui_data_services : System.Web.UI.UserControl, umbraco.editorControls.userControlGrapper.IUsercontrolDataEditor
    {

        public string umbracoValue;

        public object value
        {
            get
            {
                return umbracoValue;
            }

            set
            {
                umbracoValue = value.ToString();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {

            }
            LoadValues();
        }

        private void LoadValues()
        {
            DynamicNode node = new DynamicNode(Int32.Parse(ConfigurationManager.AppSettings["VUI2_ServiceMastersRoot"].ToString()));
            List<DynamicNode> nodes = node.Descendants().Items.ToList().OrderBy(s => s.GetProperty("serviceName")).ToList();

            foreach (DynamicNode n in nodes)
            {
                rptServices.Items.Add(new ListItem(n.GetProperty("serviceName").ToString(), n.Id.ToString()));
            }
        }
    }
}