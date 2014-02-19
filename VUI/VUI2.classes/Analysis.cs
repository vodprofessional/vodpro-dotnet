using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.MacroEngines;
using VUI.classes;
using System.Xml.XPath;

namespace VUI.VUI2.classes
{
    public class Analysis
    {
        private int _nodeId;
        private DynamicNode _node;
        private Boolean _imagesSet = false;
        private Boolean _scoreSet = false;
        private int _benchmarkScore = 0;


        public Analysis(int nodeId)
        {
            _nodeId = nodeId;
            _node = new DynamicNode(_nodeId);
            Init();
        }
        public Analysis(DynamicNode node)
        {
            _node = node;
            _nodeId = _node.Id;
            Init();
        }

        private void Init()
        {
            Date = DateTime.Parse(Node.GetProperty("analysisDate").Value);
            Screenshots = new List<VUIImage>();
        }

        public void SetScore()
        {
            if (!_scoreSet)
            {
                int score = 0;
                Int32.TryParse(Node.GetProperty("serviceScore").Value, out score);
                _benchmarkScore = score;
            }
        }

        public void SetBenchmark()
        {
            Capabilities = Node.GetProperty("serviceCapabilities").Value.Split(',');
            HotFeatures = Node.GetProperty("hotFeatures").Value.Split(',');
            if(!String.IsNullOrEmpty(Node.GetProperty("benchmarkDate").Value)) {
                BenchmarkDate = DateTime.Parse(Node.GetProperty("benchmarkDate").Value);
            }
            DeviceDescription = Node.GetProperty("benchmarkDevice").Value;
            if (Node.HasProperty("marketplaceRating"))
            {
                MarketplaceRating = Node.GetProperty("marketplaceRating").Value;
            }
            else
            {
                MarketplaceRating = String.Empty;
            }
        }

        public bool HideHotFeatures
        {
            get 
            {
                bool _ics = false;
                _ics = _node.GetProperty("hideHotFeatures").Value.Equals("1");
                return _ics;
            } 
        }

        public void SetScreenshots()
        {
            if (!_imagesSet)
            {
                XPathNodeIterator iterator = umbraco.library.GetPreValues(VUIfunctions.VUI_pagetypelist);
                iterator.MoveNext(); //move to first
                XPathNodeIterator preValues = iterator.Current.SelectChildren("preValue", "");

                List<VUI.classes.VUIfunctions.Preval> prevals = new List<VUI.classes.VUIfunctions.Preval>();
                int sort = 0;

                while (preValues.MoveNext())
                {
                    prevals.Add(new VUI.classes.VUIfunctions.Preval(sort++, preValues.Current.Value));
                }
                prevals.Add(new VUI.classes.VUIfunctions.Preval(sort++, ""));

                // This needs changing
                List<DynamicNode> imageNodes = Node.Descendants().Items.ToList();
                if (imageNodes.Count > 0)
                {
                    var orderedImages = from image in imageNodes
                                        join preval in prevals
                                            on image.GetProperty("pageType").Value equals preval.Val
                                        orderby preval.Order
                                        select image;
                    Screenshots = VUIfunctions.NodeListToVUIImages(orderedImages.ToList<DynamicNode>());
                }

                _imagesSet = true;
            }
        }


        public bool HasCapability(string capability)
        {
            if (Capabilities != null)
            {
                return Capabilities.Contains(capability);
            }
            return false;
        }
        public bool HasHotFeature(string capability)
        {
            if (HotFeatures != null)
            {
                return HotFeatures.Contains(capability);
            }
            return false;
        }

        public bool HasAnyCapabilityIn(string[] capabilityList)
        {
            if (Capabilities != null)
            {
                foreach (string c in capabilityList)
                {
                    if (Capabilities.Contains(c))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HasScreenshots
        {
            get
            {
                if (!_imagesSet)
                {
                    SetScreenshots();
                }
                return (Screenshots.Count > 0);
            }
        }
        public DateTime Date { get; set; }
        public DateTime BenchmarkDate { get; set; }
        public string DeviceDescription { get; set; }
        public string[] Capabilities { get; set; }
        public string[] HotFeatures { get; set; }
        public int BenchmarkScore { get { if(!_scoreSet) { SetScore(); } return _benchmarkScore; } }
        public List<VUIImage> Screenshots { get; private set; }
        public int NodeId { get { return _nodeId; } }
        public DynamicNode Node { get { return _node; } }
        public string Name { get { return _node.Name; } }
        public string MarketplaceRating { get; private set; }
    }
}