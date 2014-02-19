using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.MacroEngines;

namespace VUI.VUI2.classes
{
    public class Platform
    {
        private int _nodeId;
        private DynamicNode _node;

        public Platform(int nodeId)
        {
            _nodeId = nodeId;
            _node = new DynamicNode(_nodeId);
        }
        public Platform(DynamicNode node)
        {
            _node = node;
            _nodeId = _node.Id;
        }

        public Device FindDevice(string name)
        {
            List<DynamicNode> nodes = _node.Descendants(Utility.GetConst("VUI2_devicetype")).Items.Where(n => n.Name.Equals(name)).ToList();
            if (nodes.Count > 0)
            {
                return new Device(nodes.First());
            }
            else
            {
                return null;
            }
        }



        public List<Service> FindServices(string name)
        {
            // Get all from vui_Service where ServiceName = name

            return VUIDataFunctions.FindServiceOnPlatform(NodeId, name);
            /*
            List<DynamicNode> nodes = new List<DynamicNode>();
            if (name.ToLower().Equals("all"))
            {
                nodes = _node.Descendants(Utility.GetConst("VUI2_servicetype")).Items.OrderBy(n => n.Name).ToList();
            }
            else
            {
                nodes = _node.Descendants(Utility.GetConst("VUI2_servicetype")).Items.Where(n => n.Name.Equals(name)).ToList();
            }
            List<Service> services = new List<Service>();
            foreach (DynamicNode node in nodes)
            {
                Service s = new Service(node);
                services.Add(s);
            }
            return services;
             */
        }

        public static Platform GetPlatform(int nodeId)
        {
            return new Platform(nodeId);
        }

        public static Platform FindPlatform(string name)
        {
            DynamicNode vui2root = new DynamicNode(Utility.GetConst("VUI2_rootnodeid"));
            List<DynamicNode> nodes = vui2root.Descendants(Utility.GetConst("VUI2_platformtype")).Items.Where(n => n.Name.Equals(name)).ToList();
            if (nodes.Count > 0)
            {
                return new Platform(nodes.First());
            }
            else
            {
                return null;
            }
        }

        public static List<Platform> AllPlatforms()
        {
            DynamicNode vui2root = new DynamicNode(Utility.GetConst("VUI2_rootnodeid"));
            List<Platform> platforms = new List<Platform>();

            List<DynamicNode> nodes = vui2root.Descendants(Utility.GetConst("VUI2_platformtype")).Items.ToList();
            foreach (DynamicNode node in nodes )
            {
                platforms.Add(new Platform(node));
            }
            return platforms;
        }

        public bool HasDevices
        {
            get
            {
                return Node.Descendants(Utility.GetConst("VUI2_devicetype")).Items.Count > 0;
            }
        }

        public List<Device> Devices
        {
            get
            {
                List<Device> devices = new List<Device>();
                List<DynamicNode> nodes = Node.Descendants(Utility.GetConst("VUI2_devicetype")).Items.ToList();
                foreach (DynamicNode node in nodes )
                {
                    devices.Add(new Device(node));
                }
                return devices;
            }
        }
        public string PlatformName { get { return Utility.NiceUrlName(_node.Id); } }
        public int NodeId { get { return _nodeId; } }
        public DynamicNode Node { get { return _node; } }
        public string Name { get { return _node.Name; } }
        public bool IsComingSoon { get { return _node.GetProperty("isComingSoon").Value.Equals("1"); } }
        public string ImageURL { get { return _node.GetProperty("folderImage").Value; } }
    }
}