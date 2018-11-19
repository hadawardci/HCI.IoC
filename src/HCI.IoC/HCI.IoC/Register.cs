using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HCI.IoC
{
    /// <summary>
    /// Register services on container
    /// </summary>
    public static class Register
    {

        #region RegisterNamespace

        /// <summary>
        /// Register services for an namespace
        /// </summary>
        /// <typeparam name="T">project implementation</typeparam>
        /// <param name="lifestyle">Lifestyle</param>
        /// <param name="patternSuffix">pattern suffix</param>
        public static Container RegisterNamespace<T>(Lifestyle lifestyle, string patternSuffix)
            where T : class
        {
            var container = Activator.CreateInstance<Container>();
            RegisterNamespace<T>(container, lifestyle, patternSuffix, "I");
            return container;
        }

        /// <summary>
        /// Register services for an namespace
        /// </summary>
        /// <typeparam name="T">project implementation</typeparam>
        /// <param name="container">container of dependencies</param>
        /// <param name="lifestyle">Lifestyle</param>
        /// <param name="patternSuffix">pattern suffix</param>
        public static void RegisterNamespace<T>(Container container, Lifestyle lifestyle, string patternSuffix)
            where T : class
            => RegisterNamespace<T>(container, lifestyle, patternSuffix, "I");

        /// <summary>
        /// Register services for an namespace
        /// </summary>
        /// <typeparam name="T">project implementation</typeparam>
        /// <param name="container">container of dependencies</param>
        /// <param name="lifestyle">Lifestyle</param>
        /// <param name="patternSuffix">pattern suffix</param>
        /// <param name="servicePrefix">prefix of service contracts</param>
        public static void RegisterNamespace<T>(Container container, Lifestyle lifestyle, string patternSuffix, string servicePrefix)
            where T : class
        {
            var typeofProject = typeof(T);
            var assembly = typeofProject.Assembly;
            var @namespace = typeofProject.Namespace;

            var registrations =
              from type in assembly.GetExportedTypes()
              where type.Name.EndsWith(patternSuffix)
              where type.Namespace.Equals(@namespace)
              where !type.IsInterface
              where type.GetInterfaces().Any(x => x.Name.Equals(servicePrefix + type.Name))
              //where !type.GetInterfaces().Contains(typeof(ITransfer))
              select new { Service = type.GetInterfaces().FirstOrDefault(i => i.Name.Equals(servicePrefix + type.Name)), Implementation = type };

            foreach (var reg in registrations)
                container.Register(reg.Service, reg.Implementation, lifestyle);
        }

        #endregion

        #region RegisterAssembly

        /// <summary>
        /// Register services for an assembly
        /// </summary>
        /// <typeparam name="T">project implementation</typeparam>
        /// <param name="lifestyle">Lifestyle</param>
        /// <param name="patternSuffix">pattern suffix</param>
        public static Container RegisterAssembly<T>(Lifestyle lifestyle, string patternSuffix)
        where T : class
        {
            var container = Activator.CreateInstance<Container>();
            RegisterAssembly<T>(container, lifestyle, patternSuffix, "I");
            return container;
        }

        /// <summary>
        /// Register services for an assembly
        /// </summary>
        /// <typeparam name="T">project implementation</typeparam>
        /// <param name="container">container of dependencies</param>
        /// <param name="lifestyle">Lifestyle</param>
        /// <param name="patternSuffix">pattern suffix</param>
        public static void RegisterAssembly<T>(Container container, Lifestyle lifestyle, string patternSuffix)
        where T : class
            => RegisterAssembly<T>(container, lifestyle, patternSuffix, "I");

        /// <summary>
        /// Register services for an assembly
        /// </summary>
        /// <typeparam name="T">project implementation</typeparam>
        /// <param name="container">container of dependencies</param>
        /// <param name="lifestyle">Lifestyle</param>
        /// <param name="patternSuffix">pattern suffix</param>
        /// <param name="servicePrefix">prefix of service contracts</param>
        public static void RegisterAssembly<T>(Container container, Lifestyle lifestyle, string patternSuffix, string servicePrefix)
        where T : class
            => RegisterAssembly<T>(container, lifestyle, patternSuffix, servicePrefix, null);

        /// <summary>
        /// Register services for an assembly
        /// </summary>
        /// <typeparam name="T">project implementation</typeparam>
        /// <param name="container">container of dependencies</param>
        /// <param name="lifestyle">Lifestyle</param>
        /// <param name="patternSuffix">pattern suffix</param>
        /// <param name="servicePrefix">prefix of service contracts</param>
        /// <param name="notWorking"></param>
        public static void RegisterAssembly<T>(Container container, Lifestyle lifestyle, string patternSuffix, string servicePrefix, params Type[] notWorking)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(servicePrefix)) servicePrefix = string.Empty;
            var assembly = typeof(T).Assembly;
            var registrations =
              from type in assembly.GetExportedTypes()
              where type.Name.EndsWith(patternSuffix)
              where !type.IsInterface
              where type.GetInterfaces().Any(x => x.Name.Equals(servicePrefix + type.Name))
              select new { Service = type.GetInterfaces().FirstOrDefault(i => i.Name.Equals(servicePrefix + type.Name)), Implementation = type };

            foreach (var reg in registrations)
                if (notWorking == null || !notWorking.Contains(reg.Service))
                    container.Register(reg.Service, reg.Implementation, lifestyle);
        }

        #endregion

        #region RegisterFactory
        
        /// <summary>
        /// Register service factory
        /// </summary>
        /// <typeparam name="TFactory">factory of services</typeparam>
        /// <typeparam name="TService">type of services</typeparam>
        /// <param name="container">container of dependencies</param>
        /// <param name="lifestyle">Lifestyle</param>
        /// <param name="patternSuffix">pattern suffix</param>
        /// <returns></returns>
        public static TFactory RegisterFactory<TFactory, TService>(Container container, Lifestyle lifestyle, string patternSuffix)
            where TService : class
            where TFactory : Dictionary<string, Func<TService>>
            => RegisterFactory<TFactory, TService>(container, lifestyle, patternSuffix, null);

        /// <summary>
        /// Register service factory
        /// </summary>
        /// <typeparam name="TFactory">factory of services</typeparam>
        /// <typeparam name="TService">type of services</typeparam>
        /// <param name="container">container of dependencies</param>
        /// <param name="lifestyle">Lifestyle</param>
        /// <param name="patternSuffix">pattern suffix</param>
        /// <param name="callback">callback for product key</param>
        /// <returns></returns>
        public static TFactory RegisterFactory<TFactory, TService>(Container container, Lifestyle lifestyle, string patternSuffix, ProductName callback)
            where TService : class
            where TFactory : Dictionary<string, Func<TService>>
        {
            var factory = Activator.CreateInstance<TFactory>();
            var registrationsServiceFacade =
              from type in typeof(TFactory).Assembly.GetExportedTypes()
              where type.Name.EndsWith(patternSuffix)
              where !type.IsInterface
              where type.GetInterfaces().Any()
              where type.GetInterfaces().Contains(typeof(TService))
              select new
              {
                  Implementation = type,
                  ProductName = callback != null ? callback.Invoke(type) : type.Name
              };

            foreach (var reg in registrationsServiceFacade)
                factory.Add(reg.ProductName, () => (TService)container.GetInstance(reg.Implementation));
            return factory;
        }

        /// <summary>
        /// Key for register factory
        /// </summary>
        /// <param name="type">type of service</param>
        /// <returns></returns>
        public delegate string ProductName(Type type);

        #endregion

    }
}
