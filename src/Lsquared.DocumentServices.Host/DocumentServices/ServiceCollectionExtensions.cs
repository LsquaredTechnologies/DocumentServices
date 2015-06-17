using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;

namespace Lsquared.DocumentServices.Host
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureSharepoint(this IServiceCollection services, Action<SharepointSharingOptions> setupAction)
        {
            var options = new SharepointSharingOptions();
            setupAction(options);
            services.AddInstance<IDocumentSharingService>(new SharepointSharingService(options));
            return services;
        }
    }
}
