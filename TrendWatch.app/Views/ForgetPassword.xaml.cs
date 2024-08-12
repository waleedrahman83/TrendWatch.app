using TrendWatch.App.ViewModels;
using Newtonsoft.Json;
using System.Text;

namespace TrendWatch.App.Views;

public partial class ForgetPassword : ContentPage
{
	public ForgetPassword()
	{
		InitializeComponent();
	}

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        string newPassword = entryPassword.Text;
        string confirmPassword = entryConfirmPassword.Text;

        if (newPassword != confirmPassword)
        {
            await DisplayAlert("Error", "Passwords do not match!", "OK");
            return;
        }

        try
        {
            using (var client = new HttpClient())
            {
                string endpoint = "Authentication/ForgetPassword";
                string url = $"{App.ApiBaseUrl}{endpoint}";

                var changePasswordRequest = new ChangePasswordRequest
                {
                    CurrentPassword = "", // Consider if you need to send the current password
                    NewPassword = newPassword,
                    MobileNumber=App.PhoneNumber,
                };

                string requestData = JsonConvert.SerializeObject(changePasswordRequest);
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(requestData, Encoding.UTF8, "application/json") // Ensure the request content is set
                };

                request.Headers.Add("Accept", "application/json"); 

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var responseResult = JsonConvert.DeserializeObject<ResultViewModel<bool>>(content);

                    if (responseResult.Success)
                    {
                        await DisplayAlert("Success", "Password Changed", "OK");
                        await Shell.Current.GoToAsync("/login"); 
                    }
                    else
                    {
                        await DisplayAlert("Error", responseResult.Message, "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Failed to change password", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }



    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("login");
    }
    protected override bool OnBackButtonPressed()
    {
        Navigation.PopModalAsync();
        return true;
    }

   
}