using Ninject;
using Ninject.Modules;
using Ninject.Extensions.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Http.Filters;
using System.Security.Principal;

//  A small Library to configure Ninject (A Dependency Injection Library) with a WebAPI Application. 
//  To configure, take the following steps.
// 
//  1. Install Packages Ninject and Ninject.Web.Common 
//  2. Remove NinjectWebCommon.cs in your App_Start Directory
//  3. Add this file to your project  (preferrably in the App_Start Directory)  
//  4. Add Your Bindings to Concrete Types to Load method of the main Module. You can add as many additional modules as you want, simply add them to the Modules property of the NinjectModules class
//  5. Add the following Line to your Global.asax
//          NinjectHttpContainer.RegisterModules(NinjectHttpModules.Modules);  
//  You are done. 

namespace Hadco.Web
{
    /// <summary>
    /// Resolves Dependencies Using Ninject
    /// </summary>
    public class NinjectHttpResolver : IDependencyResolver, IDependencyScope
    {
        /// <summary>
        ///     Kernel is the core class for Ninject
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="modules"></param>
        public NinjectHttpResolver(params NinjectModule[] modules)
        {
            Kernel = new StandardKernel(modules);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
        public object GetService(Type serviceType)
        {
            return Kernel.TryGet(serviceType);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Kernel.GetAll(serviceType);
        }

		/// <summary>
		/// 
		/// </summary>
        public void Dispose()
        {
            //Do Nothing
        }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
        public IDependencyScope BeginScope()
        {
            return this;
        }
    }

	
	/// <summary>
	/// List and Describe Necessary HttpModules
	/// This class is optional if you already Have NinjectMvc
	/// </summary>
    public class NinjectHttpModules
    {
        /// <summary>
		/// Return Lists of Modules in the Application
        /// </summary>
        public static NinjectModule[] Modules
        {
            get
            {
                return new[] { new MainModule() };
            }
        }

		
		/// <summary>
		/// Main Module For Application
		/// </summary>
        public class MainModule : NinjectModule
        {
			/// <summary>
			/// 
			/// </summary>
            public override void Load()
            {
                // This binds by convention. To bing concrete types just add them after this.
                Kernel.Bind(x => x
                    .FromAssembliesMatching("Hadco*")
                    .SelectAllClasses()
                    .BindDefaultInterface()
                );

                //Bind to Concrete Types Here
                Kernel.Bind<IPrincipal>().ToMethod(ctx => HttpContext.Current.User);
                // e.g. Kernel.Bind<IAuthorizationContext>().To<CurrentUser>();
            }
        }
    }


    /// <summary>
    /// Its job is to Register Ninject Modules and Resolve Dependencies
    /// </summary>
    public class NinjectHttpContainer
    {
        private static NinjectHttpResolver _resolver;

		/// <summary>
		/// Register the Ninject Modules
		/// </summary>
		/// <param name="configuration">configuration to register the modules to.</param>
		/// <param name="modules">Array of ninject modules to register with the provided configuration.</param>
        public static void RegisterModules(HttpConfiguration configuration, NinjectModule[] modules)
        {
            _resolver = new NinjectHttpResolver(modules);
            configuration.DependencyResolver = _resolver;
        }

		/// <summary>
		/// Manually Resolve Dependencies
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
        public static T Resolve<T>()
        {
            return _resolver.Kernel.Get<T>();
        }
    }
}