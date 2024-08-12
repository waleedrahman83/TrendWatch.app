
using TrendWatch.App.DataModels;
using TrendWatch.App.ViewModels;


namespace TrendWatch.App.Views;

public partial class InvoiceDetailsPage : ContentPage
{
	public InvoiceDetailsPage(Guid invoiceId,bool FromScan)
	{
		InitializeComponent();
        BindingContext = new InvoiceDetailsViewModel(invoiceId, FromScan);

    }
    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();

    }
}