using Autofac;
using Autofac.Extensions.DependencyInjection;
using EcommerceApp.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

namespace EcommerceApp
{
    /// <summary>
    /// The Program class serves as the entry point for the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The `Main` method is the starting point of the program. 
        /// It calls the `CreateHostBuilder` method to build and run the web application.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// The `CreateHostBuilder` method sets up the application's configuration and web host.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseServiceProviderFactory(new AutofacServiceProviderFactory()) // Use Autofac as the service provider
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    // Register your Autofac modules
                    builder.RegisterModule(new AutofacModule());
                })
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .Enrich.FromLogContext())
                // configures the application to use a JSON configuration file named "appsettings.json"
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                // configures the web host and sets up the application's services
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // configuration of services with the application's dependency injection container, allowing them to be used throughout the application
                    webBuilder.ConfigureServices((hostContext, services) =>
                    {
                        // adds controllers and endpoint routing
                        services.AddControllers();
                        services.AddEndpointsApiExplorer();

                        services.AddApiVersioning(options =>
                        {
                            options.ReportApiVersions = true;
                            options.DefaultApiVersion = new ApiVersion(1, 0);
                            options.AssumeDefaultVersionWhenUnspecified = true;
                        });

                        services.AddVersionedApiExplorer(options =>
                        {
                            options.GroupNameFormat = "'v'VVV";
                            options.SubstituteApiVersionInUrl = true;
                        });

                        // Add FV to Asp.net
                        services.AddFluentValidationAutoValidation();

                        services.AddFluentValidationRulesToSwagger();

                        // adds Swagger for generating API documentation
                        services.AddSwaggerGen(c =>
                        {
                            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                            foreach (var description in provider.ApiVersionDescriptions)
                            {
                                c.SwaggerDoc(description.GroupName, new OpenApiInfo { Title = "Ecommerce API", Version = description.ApiVersion.ToString() });
                            }

                            c.OperationFilter<FluentValidationOperationFilter>();

                            // registers the `AddCustomHeaderParameter` class as an operation filter for Swagger
                            // ensures that the custom header parameter is included in the generated Swagger documentation for all API operations
                            c.OperationFilter<AddCustomHeaderParameter>();

                            // set the XML comments file path for generating accurate Swagger documentation
                            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                            c.IncludeXmlComments(xmlPath);
                        });

                        // adds authentication and authorization
                        // uses a custom authentication scheme named "BasicAuthentication" with a corresponding handler (BasicAuthenticationHandler.cs)
                        services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
                        services.AddAuthorization();

                    })
                    // configures the application's request processing pipeline
                    .Configure((hostContext, app) =>
                    {
                        // if the application is running in the development environment, it enables the developer exception page
                        if (hostContext.HostingEnvironment.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }

                        // sets up Swagger and Swagger UI middleware
                        app.UseSwagger();
                        app.UseSwaggerUI(c =>
                        {
                            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

                            foreach (var description in provider.ApiVersionDescriptions.Reverse())
                            {
                                c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Ecommerce API {description.GroupName}");
                            }
                        });

                        // enables routing, authentication, and authorization
                        app.UseRouting();

                        app.UseAuthentication();
                        app.UseAuthorization();

                        // maps the API controllers as endpoints
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });

                    });
                });
    }
}