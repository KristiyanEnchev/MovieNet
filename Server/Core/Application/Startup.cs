namespace Application
{
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using FluentValidation;

    public static class Startup
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddValidators();
        }

        private static void AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
