
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using TatBlog.WebApp.Mapster;

public static class MapsterDependencyinjection {
    public static WebApplicationBuilder ConfigureMapster(
        this WebApplicationBuilder builder) {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(MapsterConfiguration).Assembly);
        builder.Services.AddSingleton(config);
        builder.Services.AddScoped<IMapper, ServiceMapper>();
        return builder;
    }
        
 }
