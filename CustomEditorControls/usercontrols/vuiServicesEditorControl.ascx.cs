using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.MacroEngines;

namespace CustomEditorControls.usercontrols
{
    public partial class vuiServicesEditorControl : System.Web.UI.UserControl, umbraco.editorControls.userControlGrapper.IUsercontrolDataEditor
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
                List<string> services = new List<string>();

                foreach (ListItem l in rptServices.Items)
                {
                    if (l.Selected)
                    {
                        services.Add(l.Value);
                    }
                }
                umbracoValue = string.Join(",", services);
            }
            LoadValues();
        }

        private void LoadValues()
        {
            List<string> services = new List<string>();

            if (!string.IsNullOrEmpty(umbracoValue))
            {
                services = umbracoValue.Split(',').ToList();
            }
            DynamicNode node = new DynamicNode(Int32.Parse(ConfigurationManager.AppSettings["VUI2_ServiceMastersRoot"].ToString()));
            List<DynamicNode> nodes = node.Descendants().Items.ToList().OrderBy(s => s.GetProperty("serviceName").ToString()).ToList();

            foreach (DynamicNode n in nodes)
            {
                ListItem l = new ListItem(n.GetProperty("serviceName").ToString(), n.Id.ToString());
                if (services.Contains(n.Id.ToString()))
                {
                    l.Selected = true;
                }
                rptServices.Items.Add(l);
            }
        }
    }
}