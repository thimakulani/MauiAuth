using MauiAuth.Extensions;
using MauiAuth.Storage.Services;
using Microsoft.Extensions.Logging;

namespace MauiAuth.Sample;

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
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services
            .AddMauiAuth()
            .AddMauiAuthSecureStorage(_ => new DelegateSecureStorage(
                SecureStorage.Default.GetAsync,
                SecureStorage.Default.SetAsync,
                SecureStorage.Default.Remove));

        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}
