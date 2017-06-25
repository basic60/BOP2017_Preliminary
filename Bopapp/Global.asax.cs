using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Bopapp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            System.Configuration.ConfigurationSettings.AppSettings["JiebaConfigFileDir"] =HttpContext.Current.Server.MapPath("/Resources/");
        }
    }
}
