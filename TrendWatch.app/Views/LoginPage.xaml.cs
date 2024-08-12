//using Android.Media.TV;
using CommunityToolkit.Maui.Views;
using TrendWatch.App.Controls;
using TrendWatch.App.DataModels;
using TrendWatch.App.ViewModels;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace TrendWatch.App.Views;

public partial class LoginPage : ContentPage
{
    private LoginViewModel viewModel;

    public LoginPage()
	{

        InitializeComponent();
        viewModel = new LoginViewModel();
        BindingContext = viewModel;
        //MessagingCenter.Subscribe<LoginViewModel>(this, "NoInternet", async (sender) =>
        //{
        //    var popup = new NotificationPopup("No internet connection!", "Please check your network connection", "disconnect.svg", Color.FromRgb(242, 63, 47), true, "Dismiss", "TryAgainMessageCenter");// to the internet.\n Please check your connection and try again.");
        //    await this.ShowPopupAsync(popup);
        //});
    }
    //protected override bool OnBackButtonPressed()
    //{
    //    Application.Current.Quit();
    //    return true;
    //}



    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var result = await viewModel.UserLogin(entryUsername.Text, entryPassword.Text);
            if (result.isAuthenticated)
            {
                App.CustomerID = result.id;
                App.AccessToken = result.token;
                App.CustomerName = result.name;

                await SecureStorage.SetAsync("hasAuth", "true");
                await SecureStorage.SetAsync("token", result.token);
                await SecureStorage.SetAsync("customerId", result.id.ToString());
                if (!string.IsNullOrEmpty(result.name))
                await SecureStorage.SetAsync("CustomerName", result.FullName.ToString());


                await Shell.Current.GoToAsync("///home");

            }
            else
            {
               viewModel.IsLoading = false;
                if (result.message == "No Internet Connection")
                {
                    var popup = new NotificationPopup("No internet connection!", "Please check your network connection", "disconnect.svg", Color.FromRgb(242, 63, 47), true, "Dismiss", "TryAgainMessageCenter");// to the internet.\n Please check your connection and try again.");
                    await this.ShowPopupAsync(popup);
                    
                }
                else
                {
                    var popup = new NotificationPopup("Login failed!", result.message, "error.svg", Color.FromRgb(242, 63, 47), false, "Close", "");// to the internet.\n Please check your connection and try again.");
                    await this.ShowPopupAsync(popup);
                }
            
               // await DisplayAlert("Login failed", result.message, "Try again");

            }


        }
        catch (Exception ex)
        {
            //await DisplayAlert("Login failed", "Username or password if invalid", "Try again");
            var popup = new NotificationPopup("Login failed!", "An error has been occurred", "error.svg", Color.FromRgb(242, 63, 47), false, "Close", "");// to the internet.\n Please check your connection and try again.");
            await this.ShowPopupAsync(popup);

        }


    }

    async Task<AuthToken> UserLogin1(string UserName, string password)
    {
        AuthToken token = new AuthToken();
        var current = Connectivity.NetworkAccess;

        if (current == NetworkAccess.Internet)
        {
            try
            {
                var client = new HttpClient();

                string ipAddress = "10.0.2.2";
                //string localhost = ":localhost";
                string port = ":5136";
                string endpoint = "/api/Authentication/customer/token";

                string url = $"http://{ipAddress}{port}{endpoint}";


                var requestContent = new StringContent(
                    JsonConvert.SerializeObject(new { password, mobileNumber = UserName }),
                    Encoding.UTF8,
                    "application/json"
                );

                Console.WriteLine($"Sending request to: {url}");

                var response = await client.PostAsync(url, requestContent);
                response.EnsureSuccessStatusCode();

                var resultContent = await response.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<AuthToken>(resultContent);

                return token;
            }
            catch (Exception ex)
            {
                token.message = "Error has occurred: " + ex.Message;
                return token;
            }
        }
        else
        {
            token.message = "No Internet Connection";
            return token;
        }
    }




    bool IsCredentialCorrect(string username, string password)
    {
        return entryUsername.Text == "user" && entryPassword.Text == "1234";
    }
    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("start");

    }
    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("start");
        return true;
    }

    private async void OnForgotPasswordTapped(object sender, TappedEventArgs e)
    {
        App.ForgetPasswordOrNot = true;
        await Shell.Current.GoToAsync("AddPhoneNumber");
    }
}