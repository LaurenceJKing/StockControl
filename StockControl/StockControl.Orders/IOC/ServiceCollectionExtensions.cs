using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControl.Orders.IOC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOrderServices(
            this IServiceCollection services)
        {
            return services
                .AddTransient<IOrderRepository, SqlOrderRepository>();
        }
    }
}
