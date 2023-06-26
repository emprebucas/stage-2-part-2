using Autofac;
using AutoMapper;
using EcommerceApp.Data;
using EcommerceApp.DTOs;
using EcommerceApp.Interfaces;
using EcommerceApp.PipelineBehaviors;
using EcommerceApp.Repositories;
using EcommerceApp.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;

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
            // Register your repositories
            builder.RegisterType<OrderRepository>().As<IOrderRepository>().InstancePerLifetimeScope();
            builder.RegisterType<CartItemRepository>().As<ICartItemRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();

            // Register your DbContext
            builder.RegisterType<ECommerceDbContext>().InstancePerLifetimeScope();

            // Add other dependencies you may have

            //builder.RegisterType<OrderDtoValidator>().As<IValidator<OrderDto>>().InstancePerLifetimeScope();

            // Register MediatR
            builder.RegisterAssemblyTypes(typeof(Program).Assembly).AsImplementedInterfaces();

            // Register AutoMapper
            builder.RegisterAssemblyTypes(typeof(Mapping).Assembly).As<Profile>();

            // Register authentication scheme
            builder.RegisterType<BasicAuthenticationHandler>().As<IAuthenticationHandler>()
                .InstancePerLifetimeScope();

            // Register authentication options
            builder.Register(ctx =>
                new AuthenticationOptions
                {
                    DefaultAuthenticateScheme = "BasicAuthenticationHandler",
                    DefaultChallengeScheme = "BasicAuthenticationHandler"
                    // Add any other options you need
                })
                .AsSelf()
                .SingleInstance();

            // Register HttpContextAccessor and HttpContextHelper
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<HttpContextHelper>().SingleInstance();

            // Register validators
            builder.RegisterAssemblyTypes(typeof(OrderValidator).Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // Register ValidationPipelineBehavior
            builder.RegisterGeneric(typeof(ValidationPipelineBehavior<,>))
                .As(typeof(IPipelineBehavior<,>))
                .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
