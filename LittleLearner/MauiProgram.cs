using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace LittleLearner
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<LittleLearner.CFG.CfgComparer>();
            builder.Services.AddSingleton<LittleLearner.LCS.Modals.CodeCreationConfiguratoin>();
            builder.Services.AddSingleton<LittleLearner.CFG.ViewModel.Dashboard>();
            builder.Services.AddSingleton<LittleLearner.LCS.ViewModel.DifficultySettingsViewModel>();
            builder.Services.AddSingleton<LittleLearner.LCS.ViewModel.TableViewModel>();
            builder.Services.AddSingleton<LittleLearner.LCS.ViewModel.TableCell>();
            builder.Services.AddSingleton<LittleLearner.LCS.LimitCSolver>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
