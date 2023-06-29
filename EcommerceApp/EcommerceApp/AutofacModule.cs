using Autofac;
using AutoMapper;
using EcommerceApp.Commands;
using EcommerceApp.Data;
using EcommerceApp.Interfaces;
using EcommerceApp.PipelineBehaviors;
using EcommerceApp.Queries;
using EcommerceApp.Repositories;
using EcommerceApp.Validators;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public class AutofacModule : Module
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>))
                .InstancePerLifetimeScope();

            // Register validators
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

            // Register ValidationPipelineBehavior
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

            builder.RegisterType<ECommerceDbContext>()
                .WithParameter((pi, ctx) => pi.ParameterType == typeof(DbContextOptions<ECommerceDbContext>),
                               (pi, ctx) => ctx.Resolve<DbContextOptions<ECommerceDbContext>>())
                .InstancePerLifetimeScope();

            // Register repositories
            builder.RegisterType<OrderRepository>().As<IOrderRepository>().InstancePerLifetimeScope();
            builder.RegisterType<CartItemRepository>().As<ICartItemRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();

            // Register MediatR
            builder.RegisterAssemblyTypes(typeof(IMediator).Assembly)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // Register AutoMapper
            builder.RegisterAssemblyTypes(typeof(Mapping).Assembly).As<Profile>();

            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(Program).Assembly);
            })).AsSelf().SingleInstance();

            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper(ctx.Resolve)).As<IMapper>().InstancePerLifetimeScope();


            // Register HttpContextAccessor and HttpContextHelper
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<HttpContextHelper>().SingleInstance();

            base.Load(builder);
        }
    }
}