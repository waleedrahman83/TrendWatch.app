namespace TrendWatch.App;

using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
public partial class QRScanPage : ContentPage
{
    public CameraBarcodeReaderView Scanner;
    public QRScanPage()
    {
        InitializeComponent();
        //Scanner = new CameraBarcodeReaderView();
        Scanner = new CameraBarcodeReaderView
        {
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand

        };
        //Scanner.Options = new BarcodeReaderOptions
        //{
        //    Formats = BarcodeFormat.def
        //};
        Scanner.BarcodesDetected += Scanner_BarcodesDetected;
        
        gridScanner.Children.Add(Scanner);


    }

    private void Scanner_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        foreach (var barcode in e.Results)
            Console.WriteLine(barcode.Value);
    }

    private void btnToggleFlash_Clicked(object sender, EventArgs e)
    {
        Scanner.IsTorchOn = !Scanner.IsTorchOn;
    }
}

