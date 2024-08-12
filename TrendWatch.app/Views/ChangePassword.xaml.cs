//using Android.Media.TV;
using TrendWatch.App.ViewModels;
using Newtonsoft.Json;
using System.Text;

namespace TrendWatch.App.Views;

public partial class ChangePassword : ContentPage
{
    public ChangePassword()
    {
        InitializeComponent();
    }




    private async void LoginButton_Clicked(object sender, EventArgs e)
    {

        string newPassword = entryPassword.Text;
        string CurrentPassword = entryUsername.Text;
        string confirmPassword = entryConfirmPassword.Text;

        if (newPassword == confirmPassword)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string ipAddress = "10.0.2.2";
                    string port = ":5136";
                    string endpoint = "Authentication/changePassword";
                    string url = $"{App.ApiBaseUrl}{endpoint}";

                    var changePasswordRequest = new ChangePasswordRequest
                    {
                        CurrentPassword = CurrentPassword,
                        NewPassword = newPassword
                    };

                    string requestData = JsonConvert.SerializeObject(changePasswordRequest);

                    var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Headers.Add("accept", "application/json"); 
                    request.Headers.Add("Authorization", "Bearer " + App.AccessToken);
                    request.Content = new StringContent(requestData, Encoding.UTF8, "application/json");

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseResult = JsonConvert.DeserializeObject<ResultViewModel<bool>>(content);

                        if (responseResult.Success )
                        {
                            DisplayAlert("Done", "Password Changed", "OK");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching chart data: {ex.Message}");
            }

        }
        else
        {
            DisplayAlert("Error", "Passwords do not match!", "OK");
        }


    }



    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();

    }
    protected override bool OnBackButtonPressed()
    {
        Navigation.PopModalAsync();
        return true;
    }
}