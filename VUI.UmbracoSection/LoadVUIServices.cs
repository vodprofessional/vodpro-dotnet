using umbraco.cms.presentation.Trees;
using umbraco.MacroEngines;
using umbraco.cms.businesslogic.web;
using System.Collections.Generic;
using System.Configuration;
using System;
using System.Linq;

namespace VUI.UmbracoSection
{
    public class LoadVUIServices : BaseTree
    {

        /*
         * 
         * False	True	0	vui	vuiServices	VUI Services	legacy	legacy	vui	VUI.VUI2.classes.LoadVUI	NULL
         */

        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(LoadVUIServices));

        public LoadVUIServices(string application) : base(application) { }

        protected override void CreateRootNode(ref XmlTreeNode rootNode)
        {

            log.Debug("Rendering VUI Tree");

            rootNode.Icon = FolderIcon;
            rootNode.OpenIcon = FolderIconOpen;
            rootNode.NodeType = TreeAlias;
            rootNode.NodeID = "init";

        }

        public override void RenderJS(ref System.Text.StringBuilder Javascript)
        {
            Javascript.Append(
                @"
                    function openVUI(id) //node id
                    {
                        parent.right.document.location.href='plugins/vui/pages/VUIService.aspx?id='+id;
                    }
                ");
        }

        public override void Render(ref XmlTree tree)
        {
            // Loop around services

            log.Debug("Rendering VUI Tree");

            if (this.NodeKey == string.Empty)
            {
                int smRootId = Int32.Parse(ConfigurationManager.AppSettings["VUI2_ServiceMastersRoot"]);
                Document smRoot = new Document(smRootId);
                Document[] sms = smRoot.Children;
                List<Document> ServiceMasters = new List<Document>();
                ServiceMasters.AddRange(sms);

                foreach (Document sm in ServiceMasters.OrderBy(s => s.Text))
                {
                    log.Debug("Rendering SM Node " + sm.Id.ToString());

                    // foreach service
                    XmlTreeNode xNode = XmlTreeNode.Create(this);
                    xNode.NodeID = sm.Id.ToString();
                    xNode.Text = String.Format(@"{0} [{1}]", sm.Text, sm.Id.ToString());
                    xNode.Icon = "vui-tree.png";
                    xNode.Action = "javascript:openVUI(" + sm.Id + ")";
                    xNode.IconClass = sm.Published ? "" : "unpublished";
                    xNode.Source = this.GetTreeServiceUrl(sm.Id.ToString()); // =  TreeService.GetServiceUrl(-1,TreeAlias,true,false,TreeDialogModes.id,app,sm.Id.ToString(),"openVUI");
                    tree.Add(xNode);
                }
            }
            else
            {
                int rootid = Int32.Parse(ConfigurationManager.AppSettings["umb_vuiFolderRoot"]);
                Document vuiRoot = new Document(rootid);
                Document[] rootChildren = vuiRoot.Children;

                List<Document> platforms = new List<Document>();
                foreach (Document d in rootChildren)
                {
                    if (d.ContentType.Alias.Equals("VUI2Platform"))
                    {
                        platforms.Add(d);
                    }
                }

                //foreach (

                log.Debug(" -- [" + this.NodeKey + "] Children");
            }
        }
    }
}