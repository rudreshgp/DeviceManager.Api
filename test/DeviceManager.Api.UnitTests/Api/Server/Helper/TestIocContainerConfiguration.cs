using DeviceManager.Api.Data.Management;
using DeviceManager.Api.Data.Management.Dapper;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DeviceManager.Api.UnitTests.Api.Server.Helper
{
    public static class TestIocContainerConfiguration
    {
        public static void ConfigureMockMethods(IServiceCollection services)
        {
            ReplaceUnitOfWorkInstancesWithMoqInstances(services);
        }

        private static void ReplaceUnitOfWorkInstancesWithMoqInstances(IServiceCollection services)
        {
            SwapTransient<IUnitOfWork>(services, provider =>
            GetServiceResolver<IUnitOfWork, UnitOfWork, IContextFactory>(
                services,
                uof => uof.Commit())
                );
            SwapTransient<IDapperUnitOfWork>(services, provider =>
            GetServiceResolver<IDapperUnitOfWork, DapperUnitOfWork, IConnectionFactory>(
                services,
                uof => uof.Commit())
                );
        }

        /// <summary>
        /// Replaces registered dependency with mocked dependency
        /// </summary>
        /// <typeparam name="TService">Interface type</typeparam>
        /// <param name="services">IOC registrations</param>
        /// <param name="implementationFactory">Dependency resolver</param>
        private static void SwapTransient<TService>(IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
        {
            if (services.Any(x => x.ServiceType == typeof(TService) && x.Lifetime == ServiceLifetime.Transient))
            {
                var serviceDescriptors = services.Where(x => x.ServiceType == typeof(TService) && x.Lifetime == ServiceLifetime.Transient).ToList();
                foreach (var serviceDescriptor in serviceDescriptors)
                {
                    services.Remove(serviceDescriptor);
                }
            }
            services.AddTransient(typeof(TService), (sp) => implementationFactory(sp));
        }

        private static TService GetServiceResolver<TService, TImplementation, TParam>(
            IServiceCollection services,
            params Expression<Action<TService>>[] mockMethods
            )
            where TService : class
            where TImplementation : class, TService
        {
            var serviceProvider = services.BuildServiceProvider();
            var paramInstance = serviceProvider.GetService<TParam>();

            var mockInstance = new Mock<TImplementation>(paramInstance) { CallBase = true }
                                   .As<TService>();
            //mockInstance.Setup(uow => uow.Commit());
            foreach (var mockMethod in mockMethods)
            {
                mockInstance.Setup(mockMethod);
            }

            return mockInstance.Object;

        }
    }
}
