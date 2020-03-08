using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Util;

namespace Shared
{
    public static class JaegerServiceCollectionExtensions
    {
        private static readonly Uri JaegerUri = new Uri("http://localhost:14268/api/traces");

        public static IServiceCollection AddJaeger(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            
            services.AddSingleton<ITracer>(serviceProvider =>
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                var config = Jaeger.Configuration.FromEnv(loggerFactory);

                var tracer = config.GetTracer();

                // Allows code that can't use DI to also access the tracer.
                GlobalTracer.Register(tracer);

                return tracer;
            });

            return services;
        }
    }
}
