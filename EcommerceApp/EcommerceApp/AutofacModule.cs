using Autofac;
using AutoMapper;
using EcommerceApp.Data;
using EcommerceApp.Interfaces;
using EcommerceApp.PipelineBehaviors;
using EcommerceApp.Repositories;
using EcommerceApp.Validators;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.DependencyInjection
{
    /// <summary>
    /// The `AutofacModule` class serves as a module that can be loaded into the Autofac container builder to configure the application's dependency injection container. 
    /// The registrations ensure that the required dependencies are resolved correctly and available for use throughout the application.
    /// </summary>
    public class AutofacModule : Module
    {
        /// <summary>
        /// In the`Load` method, it registers the types from the application assembly as closed types of `IRequestHandler` interface. 
        /// This registration allows MediatR to resolve and execute the request handlers.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>))
                .InstancePerLifetimeScope();

            // registers validators as their implemented interfaces
            // which allows the validators to be resolved and used by the MediatR pipeline
            builder.RegisterAssemblyTypes(typeof(OrderValidator).Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(CartItemValidator).Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(UserValidator).Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // registers ValidationPipelineBehavior which ensures that the validation pipeline behavior is executed as part of the MediatR pipeline
            builder.RegisterGeneric(typeof(ValidationPipelineBehavior<,>))
                .As(typeof(IPipelineBehavior<,>))
                .InstancePerLifetimeScope();


            builder.Register(ctx =>
            {
                var configuration = ctx.Resolve<IConfiguration>();
                var connectionString = configuration.GetConnectionString("ECommerceDb");

                var optionsBuilder = new DbContextOptionsBuilder<ECommerceDbContext>();
                optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 33)), mysqlOptions =>
                            {
                                // enables automatic retry on database failures.
                                // if a database operation fails, Entity Framework Core will automatically retry the operation a certain number of times before throwing an exception
                                mysqlOptions.EnableRetryOnFailure();
                            }).EnableServiceProviderCaching(false); // disables the caching of the service provider for the database context

                return optionsBuilder.Options;
            }).SingleInstance();

            // registers the database context
            builder.RegisterType<ECommerceDbContext>()
                .WithParameter((pi, ctx) => pi.ParameterType == typeof(DbContextOptions<ECommerceDbContext>),
                               (pi, ctx) => ctx.Resolve<DbContextOptions<ECommerceDbContext>>())
                .InstancePerLifetimeScope();

            // registers repositories
            builder.RegisterType<OrderRepository>().As<IOrderRepository>().InstancePerLifetimeScope();
            builder.RegisterType<CartItemRepository>().As<ICartItemRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();

            // registers MediatR
            builder.RegisterAssemblyTypes(typeof(IMediator).Assembly)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // registers AutoMapper
            builder.RegisterAssemblyTypes(typeof(Mapping).Assembly).As<Profile>();

            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(Program).Assembly);
            })).AsSelf().SingleInstance();

            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper(ctx.Resolve)).As<IMapper>().InstancePerLifetimeScope();


            // registers HttpContextAccessor and HttpContextHelper
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<HttpContextHelper>().SingleInstance();

            base.Load(builder);
        }
    }
}