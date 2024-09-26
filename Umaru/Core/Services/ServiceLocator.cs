using Microsoft.Extensions.DependencyInjection;
using System;

namespace Umaru.Core.Services
{
    public class ServiceLocator
    {
        private static IServiceProvider? _serviceProvider = null;

        public static void Registry(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T? Get<T>() where T : class
        {
            return _serviceProvider?.GetService<T>();
        }
    }
}
