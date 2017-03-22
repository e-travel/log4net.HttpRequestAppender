using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestWebSite
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            log4net.LogManager.GetLogger(typeof(SiteMaster)).Error("We're on the SiteMaster Page_Load");

            var task = Task.Run(() => { log4net.LogManager.GetLogger(typeof(SiteMaster)).Error("We are in a task"); });
            Task.WaitAll(new[] { task }, 2000);

            var thread = new Thread(() => { log4net.LogManager.GetLogger(typeof(SiteMaster)).Error("We are in a thread"); });
            thread.Start();
            thread.Join();
        }
    }
}
