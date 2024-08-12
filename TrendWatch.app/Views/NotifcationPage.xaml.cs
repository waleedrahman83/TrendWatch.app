using TrendWatch.App.ViewModels;

namespace TrendWatch.App.Views;

public partial class NotifcationPage : ContentPage
{
    private NotificationViewModel viewModel;
    public NotifcationPage()
	{
		InitializeComponent();

        viewModel = new NotificationViewModel();
        BindingContext = viewModel;
    }

    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("main");
    }
}