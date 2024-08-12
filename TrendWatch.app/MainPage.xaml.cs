namespace TrendWatch.App;

using CommunityToolkit.Maui.Views;
using TrendWatch.App.Controls;


//using SkiaSharp;
using TrendWatch.App.DataModels;
using TrendWatch.App.ViewModels;
//using Microcharts;
//using Microcharts.Maui;
using TrendWatch.App.Views;
using Microcharts;
using Newtonsoft.Json;
using SkiaSharp;

public partial class MainPage : ContentPage
{
    QrScan invoiceScanner = new QrScan();
    private MainViewModel viewModel;
   

    // CameraView cameraView;
    public MainPage()
    {
        InitializeComponent();
        //if(!isAuthenticated().Result)
        //{ 
        //    Shell.Current.GoToAsync("start");
        //}
        //  lblWelcome.Text = "Welcome " + App.CustomerName;



        // ChartEntry[] entries = new[]
        //{
        //  new ChartEntry(1000)
        //  {
        //      Label = "1st week",
        //     ValueLabel = "112",
        //      Color = SKColor.Parse("#E9F7ED")
        //  },
        //  new ChartEntry(2000)
        //  {
        //      Label = "2nd week",
        //     ValueLabel = "648",
        //      Color = SKColor.Parse("#B84C65")
        //  }
        //  ,
        //  new ChartEntry(3000)
        //  {
        //      Label = "3rd week",
        //      ValueLabel = "648",
        //      Color = SKColor.Parse("#3E7E9B")
        //  }
        //   ,
        //  new ChartEntry(3500)
        //  {
        //      Label = "4th week",
        //      ValueLabel = "648",
        //      Color = SKColor.Parse("#D1A74B"),

        //  },

        //  new ChartEntry(4000)
        //  {
        //      Label = "5th week",
        //     ValueLabel = "116",
        //      Color = SKColor.Parse("#5F438E")
        //  },
        //  new ChartEntry(5000)
        //  {
        //      Label = "6 week",
        //     ValueLabel = "690",
        //      Color = SKColor.Parse("#7CB275")
        //  }
        //  ,
        //  new ChartEntry(6000)
        //  {
        //      Label = "7 week",
        //      ValueLabel = "550",
        //      Color = SKColor.Parse("#E04F33")
        //  }
        //   ,
        //  new ChartEntry(7000)
        //  {
        //      Label = "8 week",
        //      ValueLabel = "800",
        //      Color = SKColor.Parse("#E9F7ED"),

        //  }
        //  };

        // var lineChart = new LineChart() { Entries = entries };

        // Assign the entries to the chart

        //chartMain.Chart = new BarChart()
        //{
        //    Entries = entries,
        //    IsAnimated = true,
        //    LabelTextSize = 30,
        //    LabelOrientation = Orientation.Horizontal,
        //    Typeface = SKTypeface.FromFamilyName("InterRegular"),
        //    CornerRadius = 30,
        //    //AnimationDuration = TimeSpan.FromSeconds(2),
        //    ShowYAxisLines = true,
        //    ShowYAxisText = true,
        //    SerieLabelTextSize = 32,
        //    ValueLabelTextSize = 32,
        //    Margin = 60,
        //    //MinValue= 0,
        //    //MaxValue = 5000


        // };

        // LoadChart();
        MessagingCenter.Subscribe<MainViewModel>(this, "NoInternet", async (sender) =>
        {
            var popup = new NotificationPopup("No internet connection!", "Please check your network connection", "disconnect.svg", Color.FromRgb(242, 63, 47),true,"Dismiss","TryAgainMessageCenter");// to the internet.\n Please check your connection and try again.");
            await this.ShowPopupAsync(popup);
        });
        MessagingCenter.Subscribe<MainViewModel>(this, "InvoiceLoadingError", async (sender) =>
        {
            var popup = new NotificationPopup("Loading Error!", "An error has been occurred while loading your data", "error.svg", Color.FromRgb(242, 63, 47), false, "Close", "");// to the internet.\n Please check your connection and try again.");
            await this.ShowPopupAsync(popup);
        });

        MessagingCenter.Subscribe<NotificationPopup>(this, "Home_TryAgain",  (sender) =>
        {
            viewModel = new MainViewModel();
            BindingContext = viewModel;
        });
        MessagingCenter.Subscribe<AddManualExpense>(this, "ExpenseAdded", (sender) =>
        {
            viewModel = new MainViewModel();
            BindingContext = viewModel;
        });
        viewModel = new MainViewModel();
        BindingContext = viewModel;
        //cameraView = invoiceScanner.Scanner;
        //cameraView.CamerasLoaded += cameraView_CamerasLoaded;
        //cameraView.BarcodeDetected+= cameraView_BarcodeDetected;
        //TapGestureRecognizer scanTGR = new TapGestureRecognizer();
        //scanTGR.Tapped += async delegate
        //{
        //    invoiceScanner.Scanner.IsDetecting = true;

        //    await Navigation.PushAsync(invoiceScanner, false);

        //};
        //  invoiceScanner.Scanner.BarcodesDetected += Scanner_BarcodesDetected;
        // btnScan.GestureRecognizers.Add(scanTGR);
    }
    async Task<bool> isAuthenticated()
    {
        //  return false;
        //await Task.Delay(2000);
        var hasAuth = await SecureStorage.GetAsync("hasAuth");

        return !(hasAuth == null);
    }
    //private void Scanner_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    //{
    //    DisplayAlert("qr", e.Results.ToString(), "");
    //}
    //private void cameraView_CamerasLoaded(object sender, EventArgs e)
    //{
    //    if (cameraView.Cameras.Count > 0)
    //    {
    //        cameraView.Camera = cameraView.Cameras.First();
    //        MainThread.BeginInvokeOnMainThread(async () =>
    //        {
    //            await cameraView.StopCameraAsync();
    //            await cameraView.StartCameraAsync();
    //        });
    //    }
    //}
    //private void cameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
    //{
    //    string Result = "";
    //    MainThread.BeginInvokeOnMainThread(() =>
    //    {
    //        Result = $"{args.Result[0].BarcodeFormat}: {args.Result[0].Text}";

    //         Navigation.PopAsync();

    //    });
    //}

    private async void btnRadio_Clicked(object sender, EventArgs e)
    {
        App.PlayRadio = true;
        ((AppShell)App.Current.MainPage).SwitchRadio();
        return;
        
    }
   











    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }

  

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushModalAsync(new SearchPage(), false);

    }

    private async void settings_Tapped(object sender, TappedEventArgs e)
    {
        SecureStorage.RemoveAll();
        await Shell.Current.GoToAsync("start");
        //await Application.Current.MainPage.Navigation.PushModalAsync(new SettingPage(), false);

    }

   
}

