//using Android.Media.TV;
using TrendWatch.App.DataModels;
using Newtonsoft.Json;

namespace TrendWatch.App.Views;

public partial class SignUpPage : ContentPage
{
	public SignUpPage()
	{
		InitializeComponent();
	}
    //protected override bool OnBackButtonPressed()
    //{
    //    Application.Current.Quit();
    //    return true;
    //}



    private async void SignupButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var result = await SignUpUser(entryName.Text.Trim(),App.PhoneNumber,entryEmail.Text.Trim(), entryPassword.Text, entryPasswordConf.Text);
            if (result.isAuthenticated)
            {
                App.CustomerID = result.id;
                App.AccessToken = result.token;

                await SecureStorage.SetAsync("hasAuth", "true");
                await SecureStorage.SetAsync("token", result.token);
                await SecureStorage.SetAsync("customerId", result.id.ToString());
                await SecureStorage.SetAsync("CustomerName", result.name.ToString());

                await Shell.Current.GoToAsync("///home");

            }
            else
            {
                await DisplayAlert("Signup failed", result.message, "Try again");

            }


        }
        catch (Exception ex)
        {
            await DisplayAlert("Signup failed", "Error", "Try again");

        }


    }

    async Task<AuthToken> SignUpUser(string name,string mobileNumber,string email, string password,string confirmPassword)
    {
        AuthToken token = new AuthToken();
        var current = Connectivity.NetworkAccess;

        if (current == NetworkAccess.Internet)
        {

            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, App.ApiBaseUrl + "Authentication/registerCustomer");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent("{\r\n  \"name\": \""+name+"\",\r\n  \"mobileNumber\": \""+mobileNumber+"\",\r\n  \"email\": \""+email+"\",\r\n  \"password\": \""+password+ "\",\r\n  \"confirmPassword\": \"" + confirmPassword+"\"\r\n}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                var resultContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    token = JsonConvert.DeserializeObject<AuthToken>(resultContent);
                   
                }
                else
                {
                    token.isAuthenticated = false;
                    token.message = resultContent;
                }
                return token;
            }
            catch (Exception ex)
            {

                token.message = "Error has been occurred";
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
        return App.PhoneNumber == "user" && entryPassword.Text == "1234";
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
}