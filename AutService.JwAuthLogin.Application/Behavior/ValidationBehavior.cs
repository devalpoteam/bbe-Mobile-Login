using AutService.JwAuthLogin.Domain.Exceptions;

namespace AutService.JwAuthLogin.Application.Behavior
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
        : IPipelineBehavior<TRequest, TResponse> where TRequest
        : class, IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var errors = _validators
                .Select(x => x.Validate(context))
                .SelectMany(x => x.Errors)
                .Where(x => x is not null)
                .GroupBy(x => x.PropertyName, x => x.ErrorMessage, (name, message) =>
                new
                {
                    Key = name,
                    Values = message.Distinct().ToArray()
                })
                .ToDictionary(x => x.Key, x => x.Values);

            if (errors.Count != 0)
            {
                throw new FluentValidationException(errors);
            }

            return await next();
        }
    }
}
