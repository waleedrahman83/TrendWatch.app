using Newtonsoft.Json;
using TrendWatch.App.DataModels;
using TrendWatch.App.ViewModels;
using System.Text;
using TrendWatch.App.Controls;
using CommunityToolkit.Maui.Views;

namespace TrendWatch.App.Views;

public partial class AddPhoneNumber : ContentPage
{
    bool _isLoading = false;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
    }
    public AddPhoneNumber()
	{
		InitializeComponent();
        BindingContext = this;
        if (!string.IsNullOrEmpty(App.PhoneNumber))
        {
            phoneNumberEntry.Text = App.PhoneNumber;    
        }
	}

    private void OnPhoneNumberChanged(object sender, TextChangedEventArgs e)
    {
        // Check if the phone number is non-empty and less than 12 characters
        continueButton.IsEnabled = !string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length < 12;
    }

    //private async void Button_Clicked(object sender, EventArgs e)
    //{
    //    App.PhoneNumber = phoneNumberEntry.Text;  

    //    var current = Connectivity.NetworkAccess;

    //    if (current == NetworkAccess.Internet)
    //    {
    //        try
    //        {
    //            var client = new HttpClient();
    //            if(!App.ForgetPasswordOrNot)
    //            var request = new HttpRequestMessage(HttpMethod.Post, App.ApiBaseUrl + "Authentication/GetOTPForUser?MobileNumber="+App.PhoneNumber);
    //            else
    //            var request = new HttpRequestMessage(HttpMethod.Post, App.ApiBaseUrl + "Authentication/RestPin?phoneNum=" + App.PhoneNumber);

    //            //request.Headers.Add("Accept", "application/json"); 
    //            //request.Headers.Add("Content-Type", "application/json");  

    //            // var requestContent = new StringContent(
    //            //    JsonConvert.SerializeObject(new {  mobileNumber = App.PhoneNumber }),
    //            //    Encoding.UTF8,
    //            //    "application/json"
    //            //);

    //            var response = await client.SendAsync(request);
    //            var resultContent = await response.Content.ReadAsStringAsync();
    //            var result = JsonConvert.DeserializeObject<ResultViewModel<bool>>(resultContent);

    //            if (response.IsSuccessStatusCode && result.Success)
    //            {
    //                await Shell.Current.GoToAsync("/OTPpage");  
    //            }
    //            else
    //            {
    //                await DisplayAlert("OTP Send failed", result.Message, "Cancel");  
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            await DisplayAlert("OTP Send failed", "Error occurred: " + ex.Message, "Cancel");
    //        }
    //    }
    //    else
    //    {
    //        await DisplayAlert("OTP Send failed", "No Internet Connection", "Cancel");
    //    }
    //}


    private async void Button_Clicked(object sender, EventArgs e)
    {
        IsLoading = true;
        App.PhoneNumber = phoneNumberEntry.Text;
        var networkAccess = Connectivity.NetworkAccess;

        if (networkAccess != NetworkAccess.Internet)
        {
            var popup = new NotificationPopup("No internet connection!", "Please check your network connection", "disconnect.svg", Color.FromRgb(242, 63, 47), true, "Dismiss", "TryAgainMessageCenter");// to the internet.\n Please check your connection and try again.");
            await this.ShowPopupAsync(popup);
            IsLoading = false;
            return;
        }

        using (var client = new HttpClient())
        {
            try
            {
                string url = App.ForgetPasswordOrNot ?
                    $"{App.ApiBaseUrl}Authentication/RestPin?phoneNum={App.PhoneNumber}" :
                    $"{App.ApiBaseUrl}Authentication/GetOTPForUser?MobileNumber={App.PhoneNumber}";

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                // Uncomment and use these headers if the server expects them.
                //request.Headers.Add("Accept", "application/json");
                //request.Headers.Add("Content-Type", "application/json");

                var response = await client.SendAsync(request);
                var resultContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResultViewModel<bool>>(resultContent);

                if (response.IsSuccessStatusCode && result.Success)
                {
                    await Shell.Current.GoToAsync("/OTPpage");
                }
                else
                {
                    IsLoading = false;
                    var popup = new NotificationPopup("OTP Sending Failed!", result.Message, "error.svg", Color.FromRgb(242, 63, 47), false, "Close", "");// to the internet.\n Please check your connection and try again.");
                    await this.ShowPopupAsync(popup);
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                var popup = new NotificationPopup("OTP Sending Failed!", "Unable to send OTP. Please try again", "error.svg", Color.FromRgb(242, 63, 47), false, "Close", "");// to the internet.\n Please check your connection and try again.");
                await this.ShowPopupAsync(popup);
            }
        }
        IsLoading = false;
    }


    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        if(App.ForgetPasswordOrNot)
        await Shell.Current.GoToAsync("login");
        else
        await Shell.Current.GoToAsync("start");

        // Navigation.PopAsync();

    }

    private void btnBack_Clicked(object sender, TappedEventArgs e)
    {

    }
}