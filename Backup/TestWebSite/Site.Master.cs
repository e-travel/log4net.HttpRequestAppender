using System;

namespace TestWebSite
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            log4net.LogManager.GetLogger(typeof(SiteMaster)).Error("We're on the SiteMaster Page_Load");
        }
    }
}
