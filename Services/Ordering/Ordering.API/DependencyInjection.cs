namespace Ordering.API;

public static class DependencyInjection
{
   public static IServiceCollection AddApiServices(this IServiceCollection services)
   {
       // REGISTER SERVICES HERE
       
       return services;
   }

   public static WebApplication UseApiServices(this WebApplication app)
   {
       // CONFIGURE MIDDLEWARE HERE

       return app;
   }
}