﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VUI.Sanitiser;

namespace VUI.pages
{
    public partial class SanitiseDB : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Btn_Click(object sender, EventArgs e)
        {
            DBSanitiser.Sanitise();
        }
    }
}