using FluentValidation;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace EcommerceApp
{
    /// <summary>
    /// FluentValidationOperationFilter automatically adds documentation for validation errors and required parameters based on the FluentValidation validators.
    /// </summary>
    public class FluentValidationOperationFilter : IOperationFilter
    {
        /// <summary>
        /// The Apply method is called during the Swagger document generation process.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // the filter selects all validator types (`IValidator<>`) that are associated with the input parameters of the method
            var validators = context.MethodInfo.GetParameters()
                .SelectMany(p => p.ParameterType.GetGenericArguments())
                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IValidator<>))
                .ToList();

            // for each validator type, the filter extracts the corresponding DTO type (the generic argument of the validator)
            foreach (var validatorType in validators)
            {
                var dtoType = validatorType.GetGenericArguments()[0];
                var dtoProperties = dtoType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // generates the schema for the DTO
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

                // `OpenApiResponse` object with a description indicating that one or more validation errors occurred
                // the response uses the media type "application/json" and contains a schema representing an array of strings (validation error messages)
                var response = new OpenApiResponse
                {
                    Description = "One or more validation errors occurred.",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = errorsContent
                    }
                };

                // adds the generated response to the operation's responses collection with the key "400" (indicating a bad request)
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
