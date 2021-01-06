using iRLeagueRESTService.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;

namespace iRLeagueRESTService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        { 
            AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configuration.EnsureInitialized();

            // Start watchers for statistic calculation
            StatisticCalculationWatcher.RegisterWatcher("SkippyCup_leagueDb");

            //Configure logger
            SetupLog4Net("C:\\Logging\\config.xml");

            var logger = log4net.LogManager.GetLogger(typeof(WebApiApplication));
            logger.Info("Starting iRLeagueRESTService ...");

            using (var context = new iRLeagueUserDatabase.UsersDbContext())
            {
                var user = context.Users.First();
            }
            using (var context = new iRLeagueDatabase.LeagueDbContext())
            {
                var season = context.Seasons.FirstOrDefault();
            }

            logger.Info("Startup finished.");
        }

        private void SetupLog4Net(string file)
        {
            if (File.Exists(file))
            {
                // set common log path for application
                log4net.GlobalContext.Properties["LogPath"] = "C:\\Logging";
                log4net.Config.XmlConfigurator.Configure(new FileInfo(file));

            }
            else
                throw new FileNotFoundException(file);
        }
    }
}
