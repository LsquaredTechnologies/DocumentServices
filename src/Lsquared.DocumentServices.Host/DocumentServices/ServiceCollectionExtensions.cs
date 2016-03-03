using System;
using Microsoft.Extensions.DependencyInjection;

namespace Lsquared.DocumentServices.Host
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureSharepoint(this IServiceCollection services, Action<SharepointSharingOptions> setupAction)
        {
            var options = new SharepointSharingOptions();
            setupAction?.Invoke(options);
            services.AddInstance<IDocumentSharingService>(new SharepointSharingService(options));
            return services;
        }
    }
}
