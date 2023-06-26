using FluentValidation;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace EcommerceApp
{
    /// <summary>
    /// 
    /// </summary>
    public class FluentValidationOperationFilter : IOperationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var validators = context.MethodInfo.GetParameters()
                .SelectMany(p => p.ParameterType.GetGenericArguments())
                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IValidator<>))
                .ToList();

            foreach (var validatorType in validators)
            {
                var dtoType = validatorType.GetGenericArguments()[0];
                var dtoProperties = dtoType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                var requestPropertyName = context.SchemaGenerator.GenerateSchema(dtoType, context.SchemaRepository).Properties.Keys.First();

                var errorsSchema = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema { Type = "string" }
                };

                var errorsContent = new OpenApiMediaType
                {
                    Schema = errorsSchema
                };

                var response = new OpenApiResponse
                {
                    Description = "One or more validation errors occurred.",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = errorsContent
                    }
                };

                operation.Responses.Add("400", response);

                foreach (var property in dtoProperties)
                {
                    var propertyName = property.Name;

                    var propertySchema = new OpenApiSchema { Type = "string" };

                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = $"{requestPropertyName}.{propertyName}",
                        In = ParameterLocation.Path,
                        Description = $"The {propertyName} field is required.",
                        Required = true,
                        Schema = propertySchema
                    });

                }
            }
        }
    }
}
