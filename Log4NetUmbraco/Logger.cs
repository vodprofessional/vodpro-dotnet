using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Log4NetUmbraco
{
    public class Logger : System.Web.HttpApplication
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(Logger));

        public Logger()
		{
			InitializeComponent();
		}

        protected void Application_Start(Object sender, EventArgs e)
        {
            // Application start-up code goes here.
            log4net.Config.XmlConfigurator.Configure();

            log.Debug("Fired up logger in application_start");
        }

        protected void Application_End(Object sender, EventArgs e)
        {

            log.Debug("closed logger in application_end");
            log4net.LogManager.Shutdown();

        }

 	    protected void Application_Error(object sender, EventArgs e)
 	    {
 	        // Code that runs when an unhandled error occurs

 	        log.Error("Unhandled exception", Context.Error);

 	    }

        private void InitializeComponent()
        {

        }

    }
}

