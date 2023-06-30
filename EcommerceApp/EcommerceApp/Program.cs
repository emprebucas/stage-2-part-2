using Autofac;
using Autofac.Extensions.DependencyInjection;
using EcommerceApp.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
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
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // configures the Serilog logger to write logs to the console and a log file
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            // builds and runs the web application
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// The `CreateHostBuilder` method sets up the application's configuration and web host.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseServiceProviderFactory(new AutofacServiceProviderFactory()) // use Autofac as the service provider
                // configures the Autofac container builder
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    // registers AutofacModule which is responsible for registering dependencies and configuring the container
                    builder.RegisterModule(new AutofacModule());
                })
                // configures Serilog as the logging provider
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration) // reads the Serilog configuration from the hosting context's configuration
                    .WriteTo.Console() // configures Serilog to write log events to the console output
                    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) // configures Serilog to write log events to a file located at `"logs/log.txt"`, a new log file is created each day
                    .Enrich.FromLogContext()) // enriches log events with contextual information from the log context (includes information such as the timestamp, log level, etc.)
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

                        // sets up API versioning
                        services.AddApiVersioning(options =>
                        {
                            options.ReportApiVersions = true;
                            options.DefaultApiVersion = new ApiVersion(1, 0);
                            options.AssumeDefaultVersionWhenUnspecified = true;
                        });

                        // adds API version exploration support
                        services.AddVersionedApiExplorer(options =>
                        {
                            options.GroupNameFormat = "'v'VVV";
                            options.SubstituteApiVersionInUrl = true;
                        });

                        // adds FluentValidation for automatic validation
                        services.AddFluentValidationAutoValidation();

                        // adds FluentValidation rules to Swagger so that validation rules are included in the generated documentation
                        services.AddFluentValidationRulesToSwagger();

                        // adds Swagger for generating API documentation
                        services.AddSwaggerGen(c =>
                        {
                            // gets the available API version descriptions
                            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                            // iterates over the available API version descriptions
                            foreach (var description in provider.ApiVersionDescriptions)
                            {
                                // defines a Swagger document for each API version
                                c.SwaggerDoc(description.GroupName, new OpenApiInfo { Title = "Ecommerce API", Version = description.ApiVersion.ToString() });
                            }

                            // registers the `FluentValidationOperationFilter` class as an operation filter for Swagger which integrates FluentValidation with Swagger and ensures that validation rules are included in the generated documentation for all API operations
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