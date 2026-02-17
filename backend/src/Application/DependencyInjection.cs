using System.Globalization;

using Application.Abstractions.Behaviors;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddApplication()
        {
            var applicationAssembly = typeof(DependencyInjection).Assembly;
            services.AddMediatR(x => x.RegisterServicesFromAssembly(applicationAssembly));
            services.AddValidatorsFromAssembly(applicationAssembly);
            // Set FluentValidation to use English error messages
            ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
            // Add Pipeline Behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }
    }
}