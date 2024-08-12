using TrendWatch.App.ViewModels;
using Newtonsoft.Json;

namespace TrendWatch.App.Views;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
        lblName.Text = App.CustomerName;

    }

    private async void LogOutButton_Clicked(object sender, EventArgs e)
    {
        if (await DisplayAlert("Are you sure?", "You will be logged out.", "Yes", "No"))
        {
            SecureStorage.RemoveAll();
        await Navigation.PushModalAsync(new StartPage(), false);

//            await Shell.Current.GoToAsync("start");
        }
    }
    protected override bool OnBackButtonPressed()
    {
        ((AppShell)App.Current.MainPage).SwitchToHome();
        return true;
    }

    private async void editButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new EditProfile(), false);

    }

    private async void changePasswordButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ChangePassword(), false);

    }

    private async Task LoadUserAsync()
    {
        try
        {
            using (var client = new HttpClient())
            {
                string ipAddress = "10.0.2.2";
                string port = ":5136";
                string endpoint = "/api/Authentication/GetUser";
                string url = $"http://{ipAddress}{port}{endpoint}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("accept", "text/plain");
                request.Headers.Add("Authorization", "Bearer " + App.AccessToken);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var responseResult = JsonConvert.DeserializeObject<ResultViewModel<User>>(content);

                    if (responseResult.Success && responseResult.Data != null)
                    {
                        lblName.Text = responseResult.Data.FullName;
                        lblEmail.Text = responseResult.Data.Email;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching chart data: {ex.Message}");
        }

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUserAsync();
    }
}