using FluentValidation;
using MediatR;

namespace EcommerceApp.PipelineBehaviors
{
    /// <summary>
    /// ValidationPipelineBehavior is a pipeline behavior class for validation using FluentValidation in combination with MediatR.
    /// It implements the `IPipelineBehavior TRequest, TResponse` interface provided by MediatR. 
    /// This interface allows for the creation of custom pipeline behaviors that can be executed before and after the handling of requests.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IValidator<TRequest> _validator;

        /// <summary>
        /// The class takes an instance of IValidator TRequest as a constructor parameter. 
        /// This allows the behavior to validate the incoming request of type TRequest using the specified validator.
        /// </summary>
        /// <param name="validator"></param>
        public ValidationPipelineBehavior(IValidator<TRequest> validator)
        {
            _validator = validator;
        }

        /// <summary>
        /// Handle is the main method of the pipeline behavior, where the actual validation is performed.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // the `_validator` instance is used to validate the `request` asynchronously
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            // if the `validationResult` is not valid (i.e., it contains validation errors), 
            if (!validationResult.IsValid)
            {
                // a `ValidationException` is thrown. 
                var errors = validationResult.Errors.ToDictionary(error => error.PropertyName, error => error.ErrorMessage);
                // the validation errors are converted to a dictionary of property names and error messages,
                // and this dictionary is passed to the `ValidationException` constructor along with an error message
                throw new ValidationException("One or more validation errors occurred.", (IEnumerable<FluentValidation.Results.ValidationFailure>)errors);
            }

            return await next();
        }

    }

}
