using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UnityAndLifetimeManagers.App_Start;
using UnityAndLifetimeManagers.Service;

namespace UnityAndLifetimeManagers.Controllers
{
    public class HomeController : Controller
    {
        [Dependency]
        public MainService MainService { get; set; }

        public HomeController()
        {
            MainService mainService = null;
            
            if (MainService == null)
            {
                //mainService = ServiceLocator.Current.GetInstance<MainService>();
                var container = UnityConfig.GetConfiguredContainer();
                mainService = container.Resolve<MainService>();
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}