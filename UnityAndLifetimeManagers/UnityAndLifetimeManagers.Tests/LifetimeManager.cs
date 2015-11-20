using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityAndLifetimeManagers;
using UnityAndLifetimeManagers.Controllers;
using UnityAndLifetimeManagers.Service;
using Microsoft.Practices.Unity;

namespace UnityAndLifetimeManagers.Tests
{
    [TestClass]
    public class LifetimeManager
    {
        /// <summary>
        /// This first demo shows what happens when you resolve a concrete class without first registering that class with the container. 
        /// It will be instantiated for each resolve (as though registered with a TransientLifetimeManager), 
        /// all dependencies in its constructor will be resolved by the container, and it will not be disposed.
        /// </summary>
        [TestMethod]
        public void ResolveType()
        {
            MainService.ResetCounters();

            using (var container = new UnityContainer())
            {
                var serviceA = container.Resolve<MainService>();
                var serviceB = container.Resolve<MainService>();

                Assert.AreNotSame(serviceA, serviceB);
                Assert.AreEqual(2, MainService.ConstructorCounter);
            }

            Assert.AreEqual(0, MainService.DisposeCounter);
        }
        
        /// <summary>
        /// When an interface is registered with a container it defaults to using a TransientLifetimeManager.
        /// </summary>
        [TestMethod]
        public void DefaultLifetimeManager()
        {
            MainService.ResetCounters();

            using (var container = new UnityContainer())
            {
                container.RegisterType<IMainService, MainService>();

                var serviceA = container.Resolve<IMainService>();
                var serviceB = container.Resolve<IMainService>();

                Assert.AreNotSame(serviceA, serviceB);
                Assert.AreEqual(2, MainService.ConstructorCounter);
            }

            Assert.AreEqual(0, MainService.DisposeCounter);
        }
    }
}