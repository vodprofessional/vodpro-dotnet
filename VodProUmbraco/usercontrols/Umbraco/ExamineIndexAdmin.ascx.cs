using System;
using System.Web.UI.WebControls;
using Examine;

namespace usercontrols.Umbraco
{
    public partial class ExamineIndexAdmin : System.Web.UI.UserControl
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                InitRepeater();
            }

        }

        private void InitRepeater()
        {
  /*          indexManager.DataSource = ExamineManager.Instance.IndexProviderCollection;
            indexManager.DataBind();
    */        
        }

        protected void RebuildClick(Object sender,EventArgs e)
        {
            /*
            Button clickedButton = (Button)sender;
            string indexToRebuild = clickedButton.CommandArgument;
            ExamineManager.Instance.IndexProviderCollection[indexToRebuild].RebuildIndex();
            result.Text = string.Format("Index {0} added to index queue", indexToRebuild);
            result.Visible = true;
             */ 
        }
    }
}