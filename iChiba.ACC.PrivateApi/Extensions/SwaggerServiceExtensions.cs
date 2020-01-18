using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Collections.Generic;

namespace iChiba.ACC.PrivateApi.Extensions
{
  public static class SwaggerServiceExtensions
  {
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new Info
        {
          Version = "v1",
          Title = "MicroWF API Documentation",
          Description = "MicroWF API Documentation"
        });

        c.AddSecurityDefinition("Bearer", new ApiKeyScheme
        {
          In = "header",
          Description = "Example: \"Bearer {token}\"",
          Name = "Authorization",
          Type = "apiKey"
        });

        c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
        {
          { "Bearer", new string[] { } }
        });
      });

      return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroWF API V1");
        c.DocExpansion(DocExpansion.None);
      });

      return app;
    }
  }
}
