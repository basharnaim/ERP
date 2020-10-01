using AutoMapper;
using ERP.WebUI.AutoMapper;
using System.Linq;
using System.Web.Mvc;
using Unity.AspNet.Mvc;
[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(ERP.WebUI.UnityMvcActivator), nameof(ERP.WebUI.UnityMvcActivator.Start))]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(ERP.WebUI.UnityMvcActivator), nameof(ERP.WebUI.UnityMvcActivator.Shutdown))]

namespace ERP.WebUI
{
    /// <summary>
    /// Provides the bootstrapping for integrating Unity with ASP.NET MVC.
    /// </summary>
    public static class UnityMvcActivator
    {
        /// <summary>
        /// Integrates Unity when the application starts.
        /// </summary>
        public static void Start() 
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DomainToViewModelMappingProfile>();
                cfg.AddProfile<ViewModelToDomainMappingProfile>();
            });
            AutoMapperConfiguration.mapper = config.CreateMapper();
            var container = UnityConfig.Container;
            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        /// <summary>
        /// Disposes the Unity container when the application is shut down.
        /// </summary>
        public static void Shutdown()
        {
            UnityConfig.Container.Dispose();
        }
    }
}