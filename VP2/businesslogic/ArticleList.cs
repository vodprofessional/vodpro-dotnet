using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.MacroEngines;
using Newtonsoft.Json;
using System.Configuration;
using umbraco.cms.businesslogic.web;
using umbraco.BusinessLogic;
using Examine;
using UmbracoExamine;

namespace VP2.businesslogic
{
    [Serializable]
    public class ArticleList
    {
        private int _rootNodeId = -1;
        private int _maxArticles = 200;


        public List<SimpleArticle> Articles { get; set; }
        public bool EndReached { get; set; }
        public int TotalResults { get; set; }

        public ArticleList(int rootnodeid)
        {
            _rootNodeId = rootnodeid;
            Articles = new List<SimpleArticle>();
            EndReached = false;
        }

        public ArticleList(string type)
        {
            if(TypeRoots.Keys.Contains(type))
            {
                if(!Int32.TryParse(ConfigurationManager.AppSettings[TypeRoots[type]], out _rootNodeId))
                {
                    if (!Int32.TryParse(ConfigurationManager.AppSettings[TypeRoots[TYPE_ANY]], out _rootNodeId))
                    {
                        throw new Exception("Unable to set root node for articles");
                    }
                }
            }
            Int32.TryParse(ConfigurationManager.AppSettings["VP2014_MAX_ARTICLES_TO_LOAD"], out _maxArticles);

            Articles = new List<SimpleArticle>();
            EndReached = false;
        }

        /// <summary>
        /// Get Some Articles
        /// </summary>
        /// <param name="num"></param>
        /// <param name="startnum"></param>
        public void LoadNext(int count, int startnum)
        {
            // Bomb out if there is no root node set
            if (_rootNodeId == -1)
            {
                throw new Exception("Unable to set root node for articles");
            }
            else
            {
                DynamicNode rootNode = new DynamicNode(_rootNodeId);
                List<DynamicNode> stories = rootNode.DescendantsOrSelf("vodProStory").Items
                                                    .Where(n => !String.IsNullOrEmpty(n.GetProperty("storydate").Value))
                                                    .OrderByDescending(n => DateTime.Parse(n.GetProperty("storydate").Value)).ToList();

                if (startnum + count > _maxArticles)
                {
                    count = _maxArticles - startnum;
                    if (count < 0) count = 0;
                }

                if (startnum < stories.Count)
                {
                    if (startnum + count > stories.Count)
                    {
                        count = stories.Count - startnum;
                        EndReached = true;
                    }

                    foreach (DynamicNode story in stories.GetRange(startnum, count))
                    {
                        SimpleArticle sa = new SimpleArticle();
                        sa.Id = story.Id;
                        sa.Headline = story.GetProperty("title").Value;
                        sa.Teaser = story.GetProperty("miniTeaser").Value;
                        sa.Url = story.Url;
                        if (story.GetProperty("storyimage").Value.Length > 0)
                        {
                            try
                            {
                                int mid;
                                if (Int32.TryParse(story.GetProperty("storyimage").Value, out mid))
                                {
                                    dynamic media = rootNode.MediaById(mid);
                                    sa.Image = media.umbracoFile;
                                    //sa.Image = story.GetProperty("storyimage").Value;
                                }
                            }
                            catch (Exception ex) { ; }
                        }
                        Articles.Add(sa);
                    }
                }
                else
                {
                    EndReached = true;
                }
            }
        }


        public static ArticleList ListFromSearch(string searchTerm, int numItems, int startPos)
        {
            ArticleList a = new ArticleList(TYPE_ANY);
            DynamicNode rootNode = new DynamicNode(a._rootNodeId);

            var criteria = ExamineManager.Instance
                   .SearchProviderCollection["VPSiteSearchSearcher"]
                   .CreateSearchCriteria();

            var filter = criteria
                .GroupedOr(new string[] { "title", "story", "storyteaser", "storytags" }, searchTerm)
                .Compile();

            List<SearchResult> sresults = ExamineManager.Instance
                            .SearchProviderCollection["VPSiteSearchSearcher"]
                            .Search(filter).ToList();

            a.TotalResults = sresults.Count();

            if (sresults.Count() > startPos + numItems)
            {
                sresults = sresults.GetRange(startPos, numItems);
            }
            else if (sresults.Count() > startPos)
            {
                sresults = sresults.Skip(startPos).ToList();
            }
            else
            {
                sresults = null;
                a.EndReached = true;
                return a;
            }

            foreach (SearchResult sr in sresults)
            {
                SimpleArticle sa = new SimpleArticle();
                sa.Id = sr.Id;
                sa.Headline = sr.Fields["title"];
                sa.Teaser = sr.Fields.Keys.Contains("miniTeaser") ? sr.Fields["miniTeaser"] : "";
                sa.Url = @umbraco.library.NiceUrl(sr.Id);
                if (sr.Fields["storyimage"].Length > 0)
                {
                    try
                    {
                        int mid;
                        if (Int32.TryParse(sr.Fields["storyimage"], out mid))
                        {
                            dynamic media = rootNode.MediaById(mid);
                            sa.Image = media.umbracoFile;
                        }
                    }
                    catch (Exception ex) { ; }
                }
                a.Articles.Add(sa);
            }
            return a;
        }



        public string AsJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        private Dictionary<string, string> TypeRoots = new Dictionary<string,string>() { 
            { TYPE_FEATURE, "VP2014_FEATURE_ROOT" } , 
            { TYPE_NEWS, "VP2014_NEWS_ROOT" } , 
            { TYPE_ANY, "VP2014_SITE_ROOT" } 
        };

        [JsonIgnore]
        public const string TYPE_FEATURE = "FEATURE";
        [JsonIgnore]
        public const string TYPE_NEWS = "NEWS";
        [JsonIgnore]
        public const string TYPE_ANY = "ANY";

    }




}