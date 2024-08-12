using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using IContainer = QuestPDF.Infrastructure.IContainer;

public class PdfGenerator
{

    public void GeneratePDFWithContent()
    {
        try
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.Header().Text("Header: My Application Screenshot as PDF");
                    page.Content().Column(column =>
                    {
                        column.Item().Text("Here's some selectable text that was on the screen.");
                        column.Item().PaddingVertical(20); // Add some vertical space
                                                           //column.Item().Element(ComposeGraphics);
                    });
                    page.Footer().AlignCenter().Text("Footer: Page 1");
                });
            });

            string filePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "exported.pdf");
            document.GeneratePdf(filePath);
        }
        catch(Exception ex)
        {

        }
    }

    //private IContainer ComposeGraphics(IContainer container)
    //{
    //    container.Canvas(canvas =>
    //    {
    //        var size = new Size(100, 100); // Size of the rectangle
    //        canvas.FillColor("#FF5733").Rectangle(0, 0, size.Width, size.Height);
    //    });
    //}
}