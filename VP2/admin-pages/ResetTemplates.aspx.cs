using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using umbraco.MacroEngines;
using umbraco.cms.businesslogic.template;
using umbraco.cms.businesslogic.web;

namespace VP2.admin_pages
{
    public partial class ResetTemplates : umbraco.BasePages.UmbracoEnsuredPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnResetTemplates_Click(object sender, EventArgs e)
        {
            umbraco.BusinessLogic.User u = new umbraco.BusinessLogic.User("websitecontentuser");

            List<TemplateSwitch> switches = new List<TemplateSwitch>();

            switches.Add(new TemplateSwitch() { DocType = "vodProStory", FromTemplate = "Story", ToTemplate = "VP2014-story" });
            switches.Add(new TemplateSwitch() { DocType = "vodProSection", FromTemplate = "Login", ToTemplate = "VP2014-signin" });
            switches.Add(new TemplateSwitch() { DocType = "vodProSection", FromTemplate = "Section", ToTemplate = "VP2014-section" });
            switches.Add(new TemplateSwitch() { DocType = "vodProSection", FromTemplate = "Register", ToTemplate = "VP2014-registration" });
            switches.Add(new TemplateSwitch() { DocType = "vodProSection", FromTemplate = "Profile", ToTemplate = "VP2014-profile" });
            switches.Add(new TemplateSwitch() { DocType = "vodProSection", FromTemplate = "Section-Jobs", ToTemplate = "VP2014-jobs" });
            switches.Add(new TemplateSwitch() { DocType = "vodGenericPage", FromTemplate = "Story", ToTemplate = "VP2014-story" });
            switches.Add(new TemplateSwitch() { DocType = "vodGenericPage", FromTemplate = "Submit contact request", ToTemplate = "VP2014-contactrequest" });
            switches.Add(new TemplateSwitch() { DocType = "vodProSection", FromTemplate = "Section-Search", ToTemplate = "VP2014-search" });
            switches.Add(new TemplateSwitch() { DocType = "vodProSection", FromTemplate = "Section", ToTemplate = "VP2014-section" });

            // Find all the content of type vodProStory
            DynamicNode rootNode = new DynamicNode(-1).Descendants().Items.First(n => n.Name.Equals("Home"));

            foreach (TemplateSwitch ts in switches)
            {

                int migrationTemplateId = Template.GetByAlias(ts.ToTemplate).Id;


                List<DynamicNode> articles = rootNode.Descendants(ts.DocType).Items;
                List<int> updateIds = new List<int>();

                foreach (DynamicNode article in articles)
                {
                    if (new Template(article.template).Alias.Equals(ts.FromTemplate))
                    {
                        updateIds.Add(article.Id);
                    }
                }

                foreach (int id in updateIds)
                {
                    Document d = new Document(id);
                    d.Template = migrationTemplateId;
                    d.Save();
                    if (d.Published)
                    {
                        d.Publish(u);
                    }
                    umbraco.library.UpdateDocumentCache(id);
                }
            }




        }
    }

    class TemplateSwitch
    {
        public string DocType { get; set; }
        public string FromTemplate { get; set; }
        public string ToTemplate { get; set; }
        public int DocId { get; set; }
    }
}