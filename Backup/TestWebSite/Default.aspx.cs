using System;
using System.Threading;
using log4net;

namespace TestWebSite
{
    public partial class _Default : System.Web.UI.Page
    {
        protected ILog Log = LogManager.GetLogger(typeof (_Default));
        protected void Page_Load(object sender, EventArgs e)
        {
            Log.Info("We're on the default Page page_load");
            Thread.Sleep(1000);
        }
    }
}
