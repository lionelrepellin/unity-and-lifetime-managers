using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using UnityAndLifetimeManagers.Service;
using UnityAndLifetimeManagers.DAL;

namespace UnityAndLifetimeManagers.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            // container.RegisterType<IProductRepository, ProductRepository>();

            // The MainService class will be instantiated for each resolve
            // If no lifetime manager is defined, TransientLifetimeManager will be used by default
            //container.RegisterType<IMainService, MainService>();

            // Register a particular instance of an object with the container, it works like a Singleton
            // it is useful when storing configuration parameters for example
            //var mainService = new MainService();
            //container.RegisterInstance<MainService>(mainService);

            // See UnityMvcActivator.cs when using PerRequestLifetimeManager
            // objects will be disposed at the end of the http request
            container.RegisterType<IMainService, MainService>(new PerRequestLifetimeManager());
            container.RegisterType<IRepository, Repository>(new PerRequestLifetimeManager());
        }
    }
}
