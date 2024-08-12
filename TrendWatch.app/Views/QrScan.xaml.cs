//using Android.App;
//using Android.Content.Res;
using Camera.MAUI;
//using Java.Util;
//using ZXing.Net.Maui;

namespace TrendWatch.App.Views;

public partial class QrScan : ContentPage
{
    // public CameraView Scanner;
    bool scanDone = false;
    public QrScan()
    {
        InitializeComponent();
        scanDone = false ;
        //<cv:CameraView x:Name="cameraView" WidthRequest="300" HeightRequest="200" CamerasLoaded="cameraView_CamerasLoaded" BarCodeDetectionEnabled="True"
        // BarcodeDetected = "cameraView_BarcodeDetected" />
        //Scanner = new CameraView
        //{
        //    //HorizontalOptions = LayoutOptions.FillAndExpand,
        //    //VerticalOptions = LayoutOptions.FillAndExpand,
        //    WidthRequest= 300,
        //    HeightRequest= 300,

        //};
       
        //Scanner.BarCodeDetectionEnabled = true;
        //gridScanner.Children.Add(Scanner);
    }
    private void cameraView_CamerasLoaded(object sender, EventArgs e)
    {
        if (cameraView.Cameras.Count > 0)
        {
            cameraView.Camera = cameraView.Cameras.First();
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();
            });
        }
    }

    private async void cameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if(scanDone)
            {
                return;
            }
            scanDone = true;
            cameraView.BarCodeDetectionEnabled = false;
            Guid currentInvoiceId = Guid.Empty;
           
            var r   =  args.Result[0].Text;
            if (r.Contains("="))
            {
                var res = r.Split('=');

                Guid.TryParse(res[1], out currentInvoiceId);
            }
            else if (r.Contains("PDF"))
            {
                var res = r.Split('/');

                Guid.TryParse(res[4], out currentInvoiceId);
            }
            else return;
             Navigation.PushAsync(new InvoiceDetailsPage(currentInvoiceId,true), false);


        });

    }
    protected  override bool OnBackButtonPressed()
    {
        cameraView.StopCameraAsync();

        if (IsModal(this))
        {
            Navigation.PopModalAsync();

        }
        else
        ((AppShell)App.Current.MainPage).SwitchToHome();
        return true;
    }
    public static bool IsModal( Page page)
    {
        if (page.Navigation.ModalStack.Count > 0)
        {
            foreach (var thisPage in page.Navigation.ModalStack)
            {
                if (thisPage.Equals(page))
                    return true;
            }
            return false;
        }
        else
            return false;
    }
    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        await cameraView.StopCameraAsync();

        // cameraView.StopCameraAsync();
        if (IsModal(this))
        {
            await Navigation.PopModalAsync();

        }
        else
            ((AppShell)App.Current.MainPage).SwitchToHome();

    }

    private async void btnAddManualExpense_Clicked(object sender, EventArgs e)
    {
        await cameraView.StopCameraAsync();
        await Navigation.PushModalAsync(new AddManualExpense(), false);

    }

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        scanDone = false;
        cameraView.BarCodeDetectionEnabled= true;
        cameraView.StartCameraAsync();
    }
}