using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.MacroEngines;

namespace VUI.VUI2.classes
{
    public class Device
    {
        
        private int _nodeId;
        private DynamicNode _node;

        public Device(int nodeId)
        {
            _nodeId = nodeId;
            _node = new DynamicNode(_nodeId);
        }
        public Device(DynamicNode node)
        {
            _node = node;
            _nodeId = _node.Id;
        }

        public List<Service> Services()
        {
            List<Service> services = new List<Service>();
            List<DynamicNode> nodes = _node.Descendants(Utility.GetConst("VUI2_servicetype")).Items.OrderBy(n => n.Name).ToList();
            foreach (DynamicNode n in nodes)
            {
                services.Add(new Service(n));
            }
            return services;
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

        public static Device GetDevice(int nodeId)
        {
            return new Device(nodeId);
        }

        public static Device FindDevice(string name, string platformname)
        {
            DynamicNode vui2root = new DynamicNode(Utility.GetConst("VUI2_rootnodeid"));
            List<DynamicNode> nodes = vui2root.Descendants(Utility.GetConst("VUI2_devicetype")).Items.Where(n => n.Name.Equals(name)).ToList();
            if (nodes.Count > 0)
            {
                return new Device(nodes.First());
            }
            else
            {
                return null;
            }
        }

        public string DeviceName { get { return Utility.NiceUrlName(_node.Id); } }
        public int NodeId { get { return _nodeId; } }
        public DynamicNode Node { get { return _node; } }
        public string Name { get { return _node.Name; } }
        public bool IsComingSoon { get { return _node.GetProperty("isComingSoon").Value.Equals("1"); } }
        public string ImageURL { get { return _node.GetProperty("folderImage").Value; } }
    }
}