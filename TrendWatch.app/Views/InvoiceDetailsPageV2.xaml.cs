using TrendWatch.App.ViewModels;
using Newtonsoft.Json;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;
using QuestPDF.Fluent;
using SkiaSharp;
using System.IO;
using System.Text;
using Color = Microsoft.Maui.Graphics.Color;

namespace TrendWatch.App.Views
{
    public partial class InvoiceDetailsPageV2 : ContentPage
    {
        Guid Id { get; set; }

        public InvoiceDetailsPageV2(Guid invoiceId, bool FromScan)
        {
            try
            {
                InitializeComponent();

                BindingContext = new InvoiceDetailsViewModel(invoiceId, FromScan);
                Id = invoiceId;
            }
            catch (Exception ex)
            {

            }
        }
        


        

        private async void btnBack_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void StarClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            if (!int.TryParse(button.CommandParameter?.ToString(), out int rating))
                return;

            if (button.Parent is StackLayout stackLayout)
            {
                Color unselectedColor = Color.FromHex("#d6cbb5");
                Color selectedColor = Color.FromHex("#ffc700");

                foreach (var sibling in stackLayout.Children.OfType<Button>())
                    sibling.TextColor = unselectedColor;

                foreach (var sibling in stackLayout.Children.OfType<Button>())
                {
                    sibling.TextColor = selectedColor;
                    if (sibling == button) break;
                }
            }

            await Rate(rating);
        }

        private async Task Rate(int rate)
        {
            using (var client = new HttpClient())
            {
                string endpoint = $"Invoice/{Id}/Rate?Rate={rate}";
                string url = $"{App.ApiBaseUrl}{endpoint}";
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.AccessToken);

                var content = new StringContent("", Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseResult = JsonConvert.DeserializeObject<ResultViewModel<bool>>(responseContent);

                    if (responseResult != null && responseResult.Success)
                    {
                        await DisplayAlert("Alert", responseResult.Message, "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Failed to rate.", "OK");
                }
            }
        }

        private void StarPressed(object sender, EventArgs e)
        {
            var button = sender as Button;
            button.TextColor = Color.FromHex("#deb217");
        }

        private void StarReleased(object sender, EventArgs e)
        {
            var button = sender as Button;
            button.TextColor = button.TextColor.Equals(Color.FromHex("#ffc700")) ? Color.FromHex("#ffc700") : Color.FromHex("#d6cbb5");
        }


        private void ComposeHeader(QuestPDF.Infrastructure.IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeColumn().Stack(stack =>
                {
                    stack.Item().Text("Invoice Details").FontSize(20).Bold();
                    stack.Item().Text("Wellness starts here").FontSize(16);
                });
            });
        }

        private void ComposeMainContent(QuestPDF.Infrastructure.IContainer container)
        {
            container.Stack(stack =>
            {
                stack.Item().Text("Rate Your Visit").FontSize(16).SemiBold();
                stack.Item().Text("Well Pharmacy").FontSize(16).SemiBold();
                stack.Item().Text("Invoice Date: " + DateTime.Now.ToString("dd/MM/yyyy")).FontSize(12);
                stack.Item().Text("Customer: John Doe").FontSize(12);
            });
        }

        private void ComposeFooterContent(QuestPDF.Infrastructure.IContainer container)
        {
            container.Stack(stack =>
            {
                stack.Item().Text("**Thank You**").FontSize(16).Bold();
                stack.Item().Text("Let's plant the Earth again").FontSize(13);
                stack.Item().Text("Social Media: Facebook, Twitter, Instagram").FontSize(12);
            });
        }

        private async void btnExport_Clicked(object sender, EventArgs e)
        {
            //    var document = Document.Create(container =>
            //    {
            //        container.Page(page =>
            //        {
            //            page.Size(PageSizes.A4);
            //            page.Margin(2, Unit.Centimetre);
            //            page.Header().Element(ComposeHeader);
            //            page.Content().Column(column =>
            //            {
            //                column.Item().Element(ComposeMainContent);
            //                column.Item().Element(ComposeFooterContent);
            //            });
            //            page.Footer().AlignCenter().Text(x => x.CurrentPageNumber());
            //        });
            //    });

            //    string fileName = Path.Combine(FileSystem.AppDataDirectory, "InvoiceDetails.pdf");

            //    using (var stream = File.Create(fileName))
            //    {
            //        document.GeneratePdf(stream);
            //    }

            //    await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(fileName) });

            // CaptureAndSavePdf2

            await CreatePdfFromLayout();

        }


        public async Task CreatePdfFromLayout()
        {
            GlobalFontSettings.FontResolver = new CustomFontResolver();
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Use default fonts without specifying a font resolver
            gfx.DrawString("Invoice Details", new XFont("OpenSans", 20, XFontStyle.Bold), XBrushes.Black, new XRect(0, 20, page.Width, page.Height), XStringFormats.TopCenter);
            gfx.DrawString("Wellness starts here", new XFont("OpenSans", 16, XFontStyle.Bold), XBrushes.Black, new XRect(0, 50, page.Width, page.Height), XStringFormats.TopCenter);

            // Continue drawing other elements...

            string fileName = Path.Combine(FileSystem.AppDataDirectory, "invoice.pdf");
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                document.Save(stream);
            }

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(fileName)
            });
        }




        public async Task CaptureAndSavePdf2()
        {
            var screenshotResult = await Screenshot.CaptureAsync();
            var stream = await screenshotResult.OpenReadAsync();

            using (var skStream = new SKManagedStream(stream))
            using (var skBitmap = SKBitmap.Decode(skStream))
            {
                int cropHeight = 300; //the height i cropped
                SKBitmap croppedBitmap = new SKBitmap(skBitmap.Width, skBitmap.Height - cropHeight);
                SKRect destRect = new SKRect(0, 0, croppedBitmap.Width, croppedBitmap.Height);
                SKRect sourceRect = new SKRect(0, cropHeight, skBitmap.Width, skBitmap.Height);

                using (var canvas = new SKCanvas(croppedBitmap))
                {
                    canvas.DrawBitmap(skBitmap, sourceRect, destRect);
                }

                using (PdfDocument document = new PdfDocument())
                {
                    PdfPage page = document.AddPage();
                    page.Width = croppedBitmap.Width;
                    page.Height = croppedBitmap.Height;

                    using (XGraphics xGraphics = XGraphics.FromPdfPage(page))
                    using (var ms = new MemoryStream())
                    {
                        croppedBitmap.Encode(ms, SKEncodedImageFormat.Png, 100);
                        ms.Seek(0, SeekOrigin.Begin);
                        var xImage = XImage.FromStream(() => new MemoryStream(ms.ToArray()));

                        xGraphics.DrawImage(xImage, 0, 0, page.Width, page.Height);
                    }

                    string fileName = Path.Combine(FileSystem.AppDataDirectory, "CroppedInvoice.pdf");
                    using (FileStream pdfStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    {
                        document.Save(pdfStream);
                    }

                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(fileName)
                    });
                }
            }
        }
        public async Task CaptureAndSavePdf()
        {
            var screenshotResult = await Screenshot.CaptureAsync();
            var stream = await screenshotResult.OpenReadAsync();

            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XImage image = XImage.FromStream(() => stream);
            gfx.DrawImage(image, 0, 0, page.Width, page.Height);

            string fileName = Path.Combine(FileSystem.AppDataDirectory, "screenshot.pdf");
            using (FileStream pdfStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                document.Save(pdfStream);
            }

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(fileName)
            });
        }


        //private async void btnExport_Clicked(object sender, EventArgs e)
        //{
        //    var document = Document.Create(container =>
        //    {
        //        container.Page(page =>
        //        {
        //            page.Size(PageSizes.A4);
        //            page.Margin(2, Unit.Centimetre);
        //            page.Header().Text("Invoice Details").FontSize(20).Bold();
        //            page.Content().Column(column =>
        //            {
        //                column.Item().Text("This is a sample PDF page generated from InvoiceDetailsPageV2 using QuestPDF.");
        //                column.Item().PaddingVertical(20);
        //                column.Item().Text($"Date: {DateTime.Now.ToShortDateString()}").FontSize(12);
        //            });
        //            page.Footer().AlignCenter().Text(x => x.CurrentPageNumber());
        //        });
        //    });

        //    string fileName = Path.Combine(FileSystem.AppDataDirectory, "InvoiceDetails.pdf");

        //    using (var stream = File.Create(fileName))
        //    {
        //        document.GeneratePdf(stream);
        //    }

        //    await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(fileName) });
        //}
    }
}


