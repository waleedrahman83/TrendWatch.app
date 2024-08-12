//using Android.Media.TV;
using TrendWatch.App.ViewModels;
using Newtonsoft.Json;
using System.Text;

namespace TrendWatch.App.Views;

public partial class EditProfile : ContentPage
{
    public EditProfile()
    {
        InitializeComponent();
    }


    //private void EntryEmail_TextChanged(object sender, TextChangedEventArgs e)
    //{
    //    bool isValid = IsValidEmail(e.NewTextValue);
    //    if (!isValid)
    //    {
    //        errorMessageLabel.Text = "Please enter a valid email address";

    //    }
    //    else
    //    {

    //        // Clear the error message
    //        errorMessageLabel.Text = "";
    //    }
    //}

    //private bool IsValidEmail(string email)
    //{
    //    if (string.IsNullOrWhiteSpace(email))
    //        return false;

    //    try
    //    {
    //        // This regular expression pattern checks for a valid email format
    //        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    //        return Regex.IsMatch(email, pattern);
    //    }
    //    catch (RegexMatchTimeoutException)
    //    {
    //        return false;
    //    }
    //}


    private async void LoginButton_Clicked(object sender, EventArgs e)
    {

        try
        {
            using (var client = new HttpClient())
            {
                string ipAddress = "10.0.2.2";
                string port = ":5136";
                string endpoint = "Authentication/UpdateUser";
                string url = $"{App.ApiBaseUrl}{endpoint}";

                var UpdateUser = new User
                {
                    FullName = entryUsername.Text,
                    Email = entryPassword.Text,
                };

                string requestData = JsonConvert.SerializeObject(UpdateUser);

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", "Bearer " + App.AccessToken);
                request.Content = new StringContent(requestData, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var responseResult = JsonConvert.DeserializeObject<ResultViewModel<bool>>(content);

                    if (responseResult.Success)
                    {
                        DisplayAlert("Done", "information Changed", "OK");
                    }
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching chart data: {ex.Message}");
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

    private async Task LoadUserAsync()
    {
        try
        {
            using (var client = new HttpClient())
            {
                string ipAddress = "10.0.2.2";
                string port = ":5136";
                string endpoint = "Authentication/GetUser";
                string url = $"{App.ApiBaseUrl}{endpoint}";

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
                        entryUsername.Text = responseResult.Data.FullName;
                        entryPassword.Text = responseResult.Data.Email;
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