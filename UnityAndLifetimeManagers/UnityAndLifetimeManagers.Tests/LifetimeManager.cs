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
        /// The Externally Controlled LifetimeManager tells the container to let an external source manage 
        /// the object, but it will instantiate the object for the first resolve if necessary. 
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

        /// <summary>
        /// You may register the same interface multiple times with the container so long as you provide a key 
        /// to discriminate between them. When you do this each registration has its own unique lifetime manager.
        /// </summary>
        [TestMethod]
        public void ContainerControlledLifetimeManagerWithKey()
        {
            MainService.ResetCounters();

            using (var container = new UnityContainer())
            {
                var manager1 = new ContainerControlledLifetimeManager();
                container.RegisterType<IMainService, MainService>("1", manager1);

                var manager2 = new ContainerControlledLifetimeManager();
                container.RegisterType<IMainService, MainService>("2", manager2);

                try
                {
                    container.Resolve<IMainService>();
                    Assert.Fail("An exception should have been thrown");
                }
                catch (ResolutionFailedException ex)
                {
                    Assert.IsNotNull(ex);
                }
                catch (Exception ex)
                {
                    Assert.Fail(string.Format("Unexpected exception of type {0} caught: {1}", ex.GetType(), ex.Message));
                }
                
                var service1A = container.Resolve<IMainService>("1");
                var service1B = container.Resolve<IMainService>("1");
                var service2A = container.Resolve<IMainService>("2");
                var service2B = container.Resolve<IMainService>("2");

                Assert.AreSame(service1A, service1B);
                Assert.AreSame(service2A, service2B);
                Assert.AreNotSame(service1A, service2A);
                Assert.AreEqual(2, MainService.ConstructorCounter);
            }

            Assert.AreEqual(2, MainService.DisposeCounter);
        }

        /// <summary>
        /// This just serves as a reminder that container registration only respects the mapping you specify. 
        /// Here we have registered MainService as IMainService, and thus we can not resolve IAnotherMainService even tough 
        /// the MainService class implements that interface too.
        /// </summary>
        [TestMethod]
        public void RegisterTypeForMultipleInterfaces()
        {
            MainService.ResetCounters();

            using (var container = new UnityContainer())
            {
                var manager = new ContainerControlledLifetimeManager();
                container.RegisterType<IMainService, MainService>(manager);

                var service = container.Resolve<IMainService>();

                try
                {
                    container.Resolve<IAnotherMainService>();
                    Assert.Fail("An exception should have been thrown");
                }
                catch (ResolutionFailedException ex)
                {
                    Assert.IsNotNull(ex);
                }
                catch (Exception ex)
                {
                    Assert.Fail(string.Format("Unexpected exception of type {0} caught: {1}", ex.GetType(), ex.Message));
                }
                
                Assert.IsNotNull(service);
                Assert.AreEqual(1, MainService.ConstructorCounter);
            }

            Assert.AreEqual(1, MainService.DisposeCounter);
        }
        
        /// <summary>
        /// This is a very odd behavior, and personally I consider it to be a bug! 
        /// When you map the same class to multiple interfaces, unity only respects the last LifetimeManager registered for that class.
        /// </summary>
        [TestMethod]
        public void ContainerControlledLifetimeManagerWithMultipleInterfaces()
        {
            MainService.ResetCounters();

            using (var container = new UnityContainer())
            {
                var manager = new TransientLifetimeManager();
                container.RegisterType<IMainService, MainService>(manager);

                var anotherManager = new ContainerControlledLifetimeManager();
                container.RegisterType<IAnotherMainService, MainService>(anotherManager);

                var serviceA = container.Resolve<IMainService>();
                var serviceB = container.Resolve<IMainService>();
                var anotherServiceA = container.Resolve<IAnotherMainService>();
                var anotherServiceB = container.Resolve<IAnotherMainService>();

                Assert.AreSame(serviceA, serviceB);
                Assert.AreSame(anotherServiceA, anotherServiceB);
                Assert.AreSame(serviceA, anotherServiceA);
                Assert.AreEqual(1, MainService.ConstructorCounter);
            }

            Assert.AreEqual(1, MainService.DisposeCounter);
        }

        /// <summary>
        /// This is the same bug demonstrated in the previous test, only the container registration is reversed. 
        /// Don't worry, further down I demonstrate better ways to register the same object with multiple interfaces.
        /// </summary>
        [TestMethod]
        public void TransientLifetimeManagerWithMultipleInterfaces()
        {
            MainService.ResetCounters();

            using (var container = new UnityContainer())
            {
                var manager = new ContainerControlledLifetimeManager();
                container.RegisterType<IMainService, MainService>(manager);

                var anotherManager = new TransientLifetimeManager();
                container.RegisterType<IAnotherMainService, MainService>(anotherManager);

                var demoA = container.Resolve<IMainService>();
                var demoB = container.Resolve<IMainService>();
                var anotherDemoA = container.Resolve<IAnotherMainService>();
                var anotherDemoB = container.Resolve<IAnotherMainService>();

                Assert.AreNotSame(demoA, demoB);
                Assert.AreNotSame(anotherDemoA, anotherDemoB);
                Assert.AreNotSame(demoA, anotherDemoA);
                Assert.AreEqual(4, MainService.ConstructorCounter);
            }

            Assert.AreEqual(0, MainService.DisposeCounter);
        }

        /// <summary>
        /// You can use register instance multiple times to resolve the same instance of an object for different interfaces. 
        /// However, this will cause your object to be disposed of multiple times.
        /// </summary>
        [TestMethod]
        public void RegisterInstanceMultipleTimes()
        {
            MainService.ResetCounters();

            using (var container = new UnityContainer())
            {
                var service = new MainService();

                container.RegisterInstance<IMainService>(service);
                container.RegisterInstance<IAnotherMainService>(service);

                var serviceA = container.Resolve<IMainService>();
                var anotherServiceA = container.Resolve<IAnotherMainService>();

                Assert.AreSame(serviceA, anotherServiceA);
                Assert.AreEqual(1, MainService.ConstructorCounter);
            }

            Assert.AreEqual(2, MainService.DisposeCounter);
        }
        
        /// <summary>
        /// For our final example we register the instance multiple times, but the first time we use a ContainerControlledLifetimeManager 
        /// and after that we use ExternallyControlledLifetimeManager for the other registrations. 
        /// This means that the dispose will only be called once against our object.
        /// </summary>
        [TestMethod]
        public void RegisterInstanceMultipleTimesWithExternal()
        {
            MainService.ResetCounters();

            using (var container = new UnityContainer())
            {
                var service = new MainService();

                var manager = new ContainerControlledLifetimeManager();
                container.RegisterInstance<IMainService>(service, manager);

                var anotherManager = new ExternallyControlledLifetimeManager();
                container.RegisterInstance<IAnotherMainService>(service, anotherManager);

                var serviceA = container.Resolve<IMainService>();
                var anotherServiceA = container.Resolve<IAnotherMainService>();

                Assert.AreSame(serviceA, anotherServiceA);
                Assert.AreEqual(1, MainService.ConstructorCounter);
            }

            Assert.AreEqual(1, MainService.DisposeCounter);
        }
    }
}