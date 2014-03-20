using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace VP2.businesslogic
{
    [Serializable]
    public class SimpleArticle
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Headline { get; set; }
        public string Teaser { get; set; }
        public string Image { get; set; }

        public SimpleArticle() { ; }

        public string AsJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}