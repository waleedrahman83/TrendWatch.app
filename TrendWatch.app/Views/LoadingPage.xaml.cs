namespace TrendWatch.App.Views;


public partial class LoadingPage : ContentPage
{
	public LoadingPage()
	{

        
		InitializeComponent();
	}
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        if (await isAuthenticated())
        {
            App.AccessToken = await SecureStorage.GetAsync("token");
            var custId = await SecureStorage.GetAsync("customerId");
            App.CustomerName = await SecureStorage.GetAsync("CustomerName");

            App.CustomerID = Guid.Parse(custId);
            await Shell.Current.GoToAsync("///home");
            
        }
        else
        {
            await Shell.Current.GoToAsync("start");
        }
        base.OnNavigatedTo(args);
    }

    async Task<bool> isAuthenticated()
    {
      //  return false;
        //await Task.Delay(2000);
        var hasAuth = await SecureStorage.GetAsync("hasAuth");

        return !(hasAuth == null);
    }
    
}