using Camera.MAUI;
using CommunityToolkit.Maui;
using FFImageLoading.Maui;
//using FFImageLoading.Maui;
//using FFImageLoading.Svg.Maui;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using The49.Maui.BottomSheet;
using ZXing.Net.Maui.Controls;


namespace TrendWatch.App;

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
                #region Font Inter
                fonts.AddFont("Inter-Bold.ttf", "InterBold");
                fonts.AddFont("Inter-Black.ttf", "InterBlack");
                fonts.AddFont("Inter-ExtraBold.ttf", "InterExtraBold");
                fonts.AddFont("Inter-ExtraLight.ttf", "InterExtraLight");
                fonts.AddFont("Inter-Light.ttf", "InterLight");
                fonts.AddFont("Inter-Medium.ttf", "InterMedium");
                fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                fonts.AddFont("Inter-Thin.ttf", "InterThin");
                #endregion
                #region DIN Font
                fonts.AddFont("DIN_LIGHT.otf", "DinLight");
                fonts.AddFont("DIN_MEDIUM.otf", "DinMedium");
                fonts.AddFont("DIN_ULTRALIGHT.otf", "DinUltraLight");
                fonts.AddFont("din-next-arabic-bold3.ttf", "DinArabicBold");
                #endregion


            }).UseMauiCommunityToolkit()
            .UseMicrocharts()
            .UseBarcodeReader().UseMauiCameraView()
            .UseSkiaSharp()
            .UseBottomSheet()
            .UseFFImageLoading()

        .UseMauiApp<App>().UseBarcodeReader();

        //.UseBottomSheet()

        ;

#if DEBUG
        builder.Logging.AddDebug();
#endif
        Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("MyCustomization", (handler, view) =>
        {


#if ANDROID
            handler.PlatformView.BackgroundTintList =
Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif


        });
        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("MyCustomization", (handler, view) =>
        {


#if ANDROID
            handler.PlatformView.BackgroundTintList =
Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif


        });
        return builder.Build();
    }
}
