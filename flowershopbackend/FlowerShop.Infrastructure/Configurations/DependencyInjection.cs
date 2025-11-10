using FlowerShop.Application.Dtos;
using FlowerShop.Application.Features.Flowers.Commands;
using FlowerShop.Application.Features.Flowers.Queries;
using FlowerShop.Application.Interfaces;
using FlowerShop.Domain.Entities;
using FlowerShop.Domain.Interfaces;
using FlowerShop.Infrastructure.KafkaServices;
using FlowerShop.Infrastructure.Persistence;
using FlowerShop.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FlowerShop.Infrastructure.Configurations
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Add Kafka Producer from package: Aspire.Confluent.Kafka - client package integration.
        /// Start with sample value: string
        /// You can config 'ProducerBuilder' to set 'Serializer'
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostApplicationBuilder AddKafkaServices(this IHostApplicationBuilder builder)
        {
            builder.AddKafkaProducer<string, string>("kafka");
            builder.Services.AddKeyedTransient<IKafakaProducerService<string, string>
                , KafkaProducerService<string, string>>("vectorproducer");

            return builder;
        }
        public static IHostApplicationBuilder AddAspireSqlServer(this IHostApplicationBuilder builder)
        {
            builder.AddSqlServerDbContext<FlowerShopDbContext>("sql");
            return builder;
        }
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, Action<SqlDbOptions> sqlOptions)
        {
            // Add DbContext
            services.Configure(sqlOptions);

            // Comment to verify the .NET Aspire
            //services.AddDbContext<FlowerShopDbContext>((services, options) =>
            //{
            //    var sqlServerOptions = services.GetRequiredService<IOptions<SqlDbOptions>>();
            //    options.UseSqlServer(sqlServerOptions.Value.ConnectionString
            //        //, optionBuilder => optionBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            //        );

            //    if (sqlServerOptions.Value.EnableDetailedErrors)
            //    {
            //        options.LogTo(Console.WriteLine, LogLevel.Information);
            //        options.EnableDetailedErrors();
            //    }
            //}, ServiceLifetime.Scoped, ServiceLifetime.Scoped);


            // Register repositories
            services.AddScoped<IFlowerResponsitory, FlowerRespository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IFlowerService, FlowerService>();
            services.AddTransient<IAiSearchService, AiSearchService>();

            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddTransient<IFlowerGetAllActiveHandler<IEnumerable<FlowerResponseItem>>, FlowerGetAllActive>();
            services.AddTransient<IFlowerGetAllHandler<IEnumerable<FlowerAdminResponse>>, FlowerGetAll>();
            services.AddTransient<IFlowerGetByIds<IEnumerable<FlowerResponseItem>>, FlowerGetByIds>();
            services.AddTransient<IFlowerSearch<IEnumerable<FlowerResponseItem>>, FlowerSearch>();
            services.AddTransient<IFlowerDeleteCommand<long, bool>, FlowerDeleteCommand>();
            services.AddTransient<IFlowerUpdateCommand<UpdateFlowerDto, bool>, FlowerUpdateCommand>();
            services.AddTransient<IFlowerCreateCommand<CreateFlowerDto, FlowerResponseItem>, FlowerCreateCommand>();
            services.AddTransient<IFlowerUpdateStatusCommand<(long, bool), Flower?>, FlowerUpdateStatusCommand>();
            return services;
        }
    }
}
