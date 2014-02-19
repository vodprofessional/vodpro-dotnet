using umbraco.cms.presentation.Trees;
using umbraco.MacroEngines;
using umbraco.cms.businesslogic.web;
using System.Collections.Generic;
using System.Configuration;
using System;
using System.Linq;

namespace VUI.UmbracoSection
{
    public class LoadVUIFunctions : BaseTree
    {

        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(LoadVUIServices));

        public LoadVUIFunctions(string application) : base(application) { }

        protected override void CreateRootNode(ref XmlTreeNode rootNode)
        {

            log.Debug("Rendering VUI Functions");

            rootNode.Icon = FolderIcon;
            rootNode.OpenIcon = FolderIconOpen;
            rootNode.NodeType = TreeAlias;
            rootNode.NodeID = "init";

        }

        public override void RenderJS(ref System.Text.StringBuilder Javascript)
        {
            Javascript.Append(
                @"
                    function openVUIFunctions()
                    {
                        parent.right.document.location.href='plugins/vui/pages/VUIFunctions.aspx';
                    }
                ");
        }

        public override void Render(ref XmlTree tree)
        {
            // Loop around services

            log.Debug("Rendering VUI Functions");

            XmlTreeNode xNode = XmlTreeNode.Create(this);
            xNode.NodeID = "vuifunctions";
            xNode.Text = "VUI Functions";
            xNode.Icon = "vui-tree.png";
            xNode.Action = "javascript:openVUIFunctions()";
            // xNode.Source = this.GetTreeServiceUrl("none"); // =  TreeService.GetServiceUrl(-1,TreeAlias,true,false,TreeDialogModes.id,app,sm.Id.ToString(),"openVUI");
            tree.Add(xNode);

        }
    }
}
