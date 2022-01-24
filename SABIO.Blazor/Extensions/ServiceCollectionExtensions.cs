namespace SABIO.Blazor.Extensions
{
    public static class ServiceCollectionExtensions
    {
       
        public static IServiceCollection AddDerivedFrom<TBase>(this IServiceCollection services, Func<IServiceProvider, Type, object> implementationFactory = null)
        {
            var @interface = typeof(TBase);

            foreach (var type in @interface.Assembly.GetTypes()
                .Where(type => @interface.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface && !type.IsGenericType))
            {
                if (implementationFactory != null)
                    services.AddSingleton(type, provider => implementationFactory(provider, type));
                else
                    services.AddSingleton(type);
            }

            return services;
        }
    }
}