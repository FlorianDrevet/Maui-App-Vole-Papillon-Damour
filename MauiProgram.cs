using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Refit;
using ShopAppVpd.Apis;
using ShopAppVpd.Databases;
using ShopAppVpd.Interfaces;
using ShopAppVpd.Services;
using ShopAppVpd.Settings;

namespace ShopAppVpd;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("CaveatBrush-Regular.ttf", "CaveatBrushRegular");
            });
        
        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("ShopAppVpd.appsettings.json");
        
        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        builder.Configuration.AddConfiguration(config);
        
        builder.Services.AddSingleton<ProductDatabase>();
        builder.Services.AddTransient<IProductService, ProductService>();
        
        var vpdSettings = config.GetSection("VpdSettings");
        builder.Services
            .AddRefitClient<IVpdApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(vpdSettings.GetValue<string>(nameof(VpdSettings.BaseUrl)) ?? throw new InvalidOperationException()));

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}