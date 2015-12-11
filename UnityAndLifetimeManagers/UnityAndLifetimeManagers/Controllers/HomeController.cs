using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UnityAndLifetimeManagers.App_Start;
using UnityAndLifetimeManagers.DAL;
using UnityAndLifetimeManagers.Service;

namespace UnityAndLifetimeManagers.Controllers
{
    public class HomeController : Controller
    {
        /*
        // the old way...
        // need to pass all the services / repositories into the constructor
        // it is not really easy when you want to mock the main object

        private readonly IMainService _mainServer;

        public HomeController(IMainService mainService)
        {
            if (mainService == null)
                throw new ArgumentNullException("MainService is null !");

            _mainServer = mainService;
        }
        */


        // the right way ;)
        [Dependency]        
        public MainService MainService { get; set; }

        // the best way is just below !.. interface is not used here because of static properties
        //[Dependency]
        //public IMainService MainService { get; set; }


        // Empty constructor
        public HomeController()
        {
            // nothing here...
        }

        
        public ActionResult Index()
        {
            ViewBag.CtorCounter = MainService.ConstructorCounter;
            ViewBag.UniqueId = MainService.UniqueId;

            return View();
        }


        public ActionResult About()
        {
            // you can also resolve the repository by this way
            // but it is easier to use dependencies for unit tests with mock
            var container = UnityConfig.GetConfiguredContainer();
            var repository = container.Resolve<IRepository>();
            
            ViewBag.HelloWorld = repository.SayHelloWorld();

            return View();
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}