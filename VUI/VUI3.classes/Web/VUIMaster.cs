using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace VUI.VUI3.classes.Web
{
    public partial class VUIMaster : MasterPage, IMasterPage
    {

        public string BodyId
        {
            get;
            set;
        }

        public string BodyClass
        {
            get;
            set;
        }
    }

    public interface IMasterPage
    {
        String BodyId { get; set; }
        String BodyClass { get; set; }
    } 
}