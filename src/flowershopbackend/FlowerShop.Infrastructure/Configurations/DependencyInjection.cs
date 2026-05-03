using FlowerShop.Application.Common;
using FlowerShop.Application.Dtos;
using FlowerShop.Application.Dtos.Auth;
using FlowerShop.Application.Features.Auth.Commands;
using FlowerShop.Application.Features.Flowers.Commands;
using FlowerShop.Application.Features.Flowers.Queries;
using FlowerShop.Application.Features.Orders.Commands;
using FlowerShop.Application.Features.Orders.Queries;
using FlowerShop.Application.Features.Users.Commands;
using FlowerShop.Application.Features.Users.Queries;
using FlowerShop.Application.Interfaces;
using FlowerShop.Domain.Entities;
using FlowerShop.Domain.Interfaces;
using FlowerShop.Infrastructure.Auth;
using FlowerShop.Infrastructure.Agent;
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

        /// <summary>
        /// Register Azure Blob Storage client via Aspire integration using the "blobs" connection name.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostApplicationBuilder AddAspireBlobStorage(this IHostApplicationBuilder builder)
        {
            builder.AddAzureBlobServiceClient("blobs");
            return builder;
        }
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, Action<SqlDbOptions> sqlOptions)
        {
            // Add DbContext
            services.Configure(sqlOptions);

            // Register repositories
            services.AddScoped<IFlowerResponsitory, FlowerRespository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddTransient<IFlowerService, FlowerService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IAiSearchService, AiSearchService>();
            services.AddTransient<IWriterAgentService, WriterAgentService>();
            services.AddTransient<ISalesAgentService, SalesAgentService>();
            services.AddTransient<IImageStorageService, BlobStorageService>();
            services.AddTransient<IFlowerImageService, FlowerImageService>();

            // Auth services
            services.AddTransient<IJwtService, JwtService>();
            services.AddTransient<IEmailService, SmtpEmailService>();

            return services;
        }

        /// <summary>
        /// Registers JWT and SMTP options from the application configuration.
        /// Call this in <c>Program.cs</c> after <see cref="AddInfrastructure"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="jwtOptions">Action to configure <see cref="JwtOptions"/>.</param>
        /// <param name="smtpOptions">Action to configure <see cref="SmtpOptions"/>.</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
        public static IServiceCollection AddAuthInfrastructure(
            this IServiceCollection services,
            Action<JwtOptions> jwtOptions,
            Action<SmtpOptions> smtpOptions)
        {
            services.Configure(jwtOptions);
            services.Configure(smtpOptions);
            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddTransient<IFlowerGetAllActiveHandler<IEnumerable<FlowerResponseItem>>, FlowerGetAllActive>();
            services.AddTransient<IFlowerGetAllActivePagedHandler<PagedResult<FlowerResponseItem>>, FlowerGetAllActivePaged>();
            services.AddTransient<IFlowerGetAllHandler<IEnumerable<FlowerAdminResponse>>, FlowerGetAll>();
            services.AddTransient<IFlowerGetByIds<IEnumerable<FlowerResponseItem>>, FlowerGetByIds>();
            services.AddTransient<IFlowerGetByIdHandler<FlowerDetailResponseItem?>, FlowerGetById>();
            services.AddTransient<IFlowerSearch<IEnumerable<FlowerResponseItem>>, FlowerSearch>();
            services.AddTransient<IFlowerDeleteCommand<long, bool>, FlowerDeleteCommand>();
            services.AddTransient<IFlowerUpdateCommand<UpdateFlowerDto, bool>, FlowerUpdateCommand>();
            services.AddTransient<IFlowerCreateCommand<CreateFlowerDto, FlowerResponseItem>, FlowerCreateCommand>();
            services.AddTransient<IFlowerUpdateStatusCommand<(long, bool), Flower?>, FlowerUpdateStatusCommand>();
            services.AddTransient<IFlowerValidateCartHandler<CartValidationResponseDto>, FlowerValidateCart>();

            // Auth command handlers
            services.AddTransient<IRegisterUserCommand<RegisterDto, AuthResponseDto>, RegisterUserCommand>();
            services.AddTransient<ILoginCommand<LoginDto, AuthResponseDto?>, LoginCommand>();
            services.AddTransient<IConfirmEmailCommand<ConfirmEmailDto, bool>, ConfirmEmailCommand>();

            // Order handlers
            services.AddTransient<IPlaceOrderCommand<CreateOrderDto, Result<OrderResponseDto>>, PlaceOrderCommand>();
            services.AddTransient<IGetOrderByIdQuery<Result<OrderResponseDto>>, GetOrderByIdQuery>();
            services.AddTransient<IGetAllOrdersQuery<IEnumerable<OrderResponseDto>>, GetAllOrdersQuery>();
            services.AddTransient<IGetOrdersPagedQuery<PagedResult<OrderResponseDto>>, GetOrdersPagedQuery>();
            services.AddTransient<IUpdateOrderStatusCommand<UpdateOrderStatusDto, Result<OrderResponseDto>>, UpdateOrderStatusCommand>();

            // Admin user management handlers
            services.AddTransient<IGetUsersPagedQuery<PagedResult<UserResponseDto>>, GetUsersPagedQuery>();
            services.AddTransient<IGetUserOrdersQuery<Result<IEnumerable<OrderResponseDto>>>, GetUserOrdersQuery>();
            services.AddTransient<IUpdateUserStatusCommand<UpdateUserStatusDto, Result<UserResponseDto>>, UpdateUserStatusCommand>();
            services.AddTransient<IUpdateUserCommand<UpdateUserDto, Result<UserResponseDto>>, UpdateUserCommand>();

            return services;
        }
    }
}
