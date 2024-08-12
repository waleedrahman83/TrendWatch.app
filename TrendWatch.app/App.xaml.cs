using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using PdfSharpCore.Fonts;
using static System.Net.WebRequestMethods;

namespace TrendWatch.App;

public partial class App : Application
{
    //public static string ApiBaseUrl = "https://api.bill2go.net/api/";
    public static string ApiBaseUrl = "http://37.61.221.159:9900/api/";

    //public static string ApiBaseUrl = "http://10.0.2.2:5136/api/";
    public static bool PlayRadio = false;

    public static Guid CustomerID;
	public static string AccessToken = "";
    public static string CustomerName = "";

    public static string PhoneNumber = "";

    public static string Blance = "";

    public static bool ForgetPasswordOrNot = false;

    public App()
	{
		InitializeComponent();

       // GlobalFontSettings.FontResolver = new GlobalFontResolver();


        MainPage = new AppShell();
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderLine", (handler, view) =>
        {
#if __ANDROID__

          (handler.PlatformView as Android.Views.View).SetBackgroundColor(Android.Graphics.Color.Transparent);
#endif
        });
        //  (handler.PlatformView as Android.Views.View).SetBackgroundColor(Microsoft.Maui.Graphics.Colors.Transparent.ToAndroid());
    }
}
