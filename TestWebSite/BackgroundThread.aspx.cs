using System;
using System.Threading;
using log4net;

namespace TestWebSite
{
    public partial class BackgroundThread : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var log = LogManager.GetLogger(typeof(BackgroundThread));
            log.Info("BackgroundThread Page_Load");
            var thread = new Thread(MyThreadDoSomething);
            thread.Start();
        }

        private void MyThreadDoSomething(object data)
        {
            var log = LogManager.GetLogger(typeof (BackgroundThread));
            log.Info("Thread starting doing something");
            Thread.Sleep(1500);
            log.Info("Thread ended doing something");
        }
    }
}