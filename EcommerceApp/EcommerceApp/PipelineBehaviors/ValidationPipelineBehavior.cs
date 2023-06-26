using FluentValidation;
using MediatR;

namespace EcommerceApp.PipelineBehaviors
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        //where TRequest : class, ICommand<TResponse>
    {
        private readonly IValidator<TRequest> _validator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validator"></param>
        public ValidationPipelineBehavior(IValidator<TRequest> validator)
        {
            _validator = validator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.ToDictionary(error => error.PropertyName, error => error.ErrorMessage);
                throw new ValidationException("One or more validation errors occurred.", (IEnumerable<FluentValidation.Results.ValidationFailure>)errors);
            }

            return await next();
        }

    }

}
