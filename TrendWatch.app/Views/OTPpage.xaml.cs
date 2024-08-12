using CommunityToolkit.Maui.Views;
using TrendWatch.App.Controls;
using TrendWatch.App.DataModels;
using TrendWatch.App.ViewModels;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using System.Text;
using static System.Net.WebRequestMethods;

namespace TrendWatch.App.Views;

public partial class OTPpage : ContentPage
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
    public OTPpage()
	{
		InitializeComponent();
        lblTitle.Text = "A code was sent to "+App.PhoneNumber;
        BindingContext = this;
        MessagingCenter.Subscribe<object, string>(this, "OTPReceived", (sender, otpMessage) =>
        {
            try
            {
                otpEntry1.Text = otpMessage.Substring(0, 1);
                otpEntry2.Text = otpMessage.Substring(1, 1);
                otpEntry3.Text = otpMessage.Substring(2, 1);
                otpEntry4.Text = otpMessage.Substring(3, 1);
                otp = otpMessage.Substring(0, 4);
                SubmitOTP();
            }
            catch(Exception ex)
            {

            }
        });

    }


    string otp = "";

    private void OTPEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        var text = entry.Text;

        int currentEntryNumber = int.Parse(entry.StyleId.Replace("otpEntry", ""));

        if (text.Length == 1 && (e.OldTextValue == null || e.OldTextValue.Length < 1))
        {
            if (otp.Length >= currentEntryNumber)
            {
                otp = otp.Remove(currentEntryNumber - 1, 1).Insert(currentEntryNumber - 1, text);
            }
            else
            {
                otp += text;
            }

            var nextEntry = this.FindByName<Entry>("otpEntry" + (currentEntryNumber + 1));
            nextEntry?.Focus();
        }
        else if (text.Length == 0 && (e.OldTextValue?.Length ?? 0) > 0)
        {
            if (otp.Length >= currentEntryNumber)
            {
                otp = otp.Remove(currentEntryNumber - 1, 1);
            }

            var previousEntry = this.FindByName<Entry>("otpEntry" + (currentEntryNumber - 1));
            previousEntry?.Focus();
        }
    }



    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("AddPhoneNumber");

    }

    private async void submitOTPbtn_Clicked(object sender, EventArgs e)
    {
        SubmitOTP();
    }
    private async void SubmitOTP()
    {
        IsLoading = true;
        var current = Connectivity.NetworkAccess;

        if (current == NetworkAccess.Internet)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, App.ApiBaseUrl + "Authentication/CheckOTP");

                request.Headers.Add("accept", "*/*");

                // Constructing JSON body
                var jsonContent = $"{{\r\n  \"mobileNumber\": \"{App.PhoneNumber}\",\r\n  \"otp\": \"{otp}\"\r\n}}";
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");



                var response = await client.SendAsync(request);
                var resultContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResultViewModel<bool>>(resultContent);

                if (response.IsSuccessStatusCode && result.Success)
                {
                    if (!App.ForgetPasswordOrNot)
                        await Shell.Current.GoToAsync("/signup");
                    else
                        await Shell.Current.GoToAsync("/ForgetPassword");
                    //await DisplayAlert("Success", "OTP verification successful.", "OK");
                    // Proceed to next part of your application flow
                }
                else
                {
                    IsLoading = false;
                    var popup = new NotificationPopup("Invalid OTP!", result.Message, "error.svg", Color.FromRgb(242, 63, 47), false, "Close", "");// to the internet.\n Please check your connection and try again.");
                    await this.ShowPopupAsync(popup);
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                await DisplayAlert("Error", "Error occurred: " + ex.Message, "OK");
                var popup = new NotificationPopup("Error!", "An error has been occurred", "error.svg", Color.FromRgb(242, 63, 47), false, "Close", "");// to the internet.\n Please check your connection and try again.");
                await this.ShowPopupAsync(popup);
            }
        }
        else
        {
            IsLoading = false;
            var popup = new NotificationPopup("No internet connection!", "Please check your network connection", "disconnect.svg", Color.FromRgb(242, 63, 47), true, "Dismiss", "TryAgainMessageCenter");// to the internet.\n Please check your connection and try again.");
            await this.ShowPopupAsync(popup);
        }
       


    }
}