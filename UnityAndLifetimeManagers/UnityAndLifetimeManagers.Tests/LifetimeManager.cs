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

        /// <summary>
        /// This test will yield the same results as above. The MainService class will be instantiated for each resolve, 
        /// all dependencies in its constructor will be resolved by the container, and 
        /// IT WILL NOT BE DISPOSED of by the container.
        /// </summary>
        [TestMethod]
        public void TransientLifetimeManager()
        {
            MainService.ResetCounters();

            using (var container = new UnityContainer())
            {
                var manager = new TransientLifetimeManager();
                container.RegisterType<IMainService, MainService>(manager);

                var serviceA = container.Resolve<IMainService>();
                var serviceB = container.Resolve<IMainService>();

                Assert.AreNotSame(serviceA, serviceB);
                Assert.AreEqual(2, MainService.ConstructorCounter);
            }

            Assert.AreEqual(0, MainService.DisposeCounter);
        }

        /// <summary>
        /// Rather than registering a type to type conversion, you can just register a particular instance of an object 
        /// with the container. This effectively makes that container treat that object as a singleton, and 
        /// by default IT WILL BE DISPOSED of with the container.
        /// </summary>
        [TestMethod]
        public void RegisterInstance()
        {
            MainService.ResetCounters();

            using (var container = new UnityContainer())
            {
                var mainService = new MainService();
                container.RegisterInstance<IMainService>(mainService);

                var serviceA = container.Resolve<IMainService>();
                var serviceB = container.Resolve<IMainService>();

                Assert.AreSame(serviceA, serviceB);
                Assert.AreEqual(1, MainService.ConstructorCounter);
            }

            Assert.AreEqual(1, MainService.DisposeCounter);
        }

        /// <summary>
        /// This is the most straight forward way to make a single class that is BOTH INSTANTIATED AND DISPOSED by the container.
        /// </summary>
        [TestMethod]
        public void ContainerControlledLifetimeManager()
        {
            MainService.ResetCounters();

            using (var container = new UnityContainer())
            {
                var manager = new ContainerControlledLifetimeManager();
                container.RegisterType<IMainService, MainService>(manager);

                var serviceA = container.Resolve<IMainService>();
                var serviceB = container.Resolve<IMainService>();

                Assert.AreSame(serviceA, serviceB);
                Assert.AreEqual(1, MainService.ConstructorCounter);
            }

            Assert.AreEqual(1, MainService.DisposeCounter);
        }

        /// <summary>
        /// The Externally Controlled LifetimeManager tells the container to let an external source manage the object, but 
        /// it will instantiate the object for the first resolve if necessary. 
        /// This effectively means that you are creating a SINGLETON that the container WILL NOT DISPOSE.
        /// </summary>
        [TestMethod]
        public void ExternallyControlledLifetimeManager()
        {
            MainService.ResetCounters();

            using (var container = new UnityContainer())
            {
                var manager = new ExternallyControlledLifetimeManager();
                container.RegisterType<IMainService, MainService>(manager);

                var serviceA = container.Resolve<IMainService>();
                var serviceB = container.Resolve<IMainService>();

                Assert.AreSame(serviceA, serviceB);
                Assert.AreEqual(1, MainService.ConstructorCounter);
            }

            Assert.AreEqual(0, MainService.DisposeCounter);
        }
    }
}