using CommunityToolkit.Mvvm.ComponentModel;
using TrendWatch.App.DataModels;
using Newtonsoft.Json;
using System.ComponentModel;

namespace TrendWatch.App.ViewModels
{
    public class LoginViewModel: ObservableObject, INotifyPropertyChanged
    {
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        bool _isLoading = false;
        public bool IsLoading {
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
       internal async Task<AuthToken> UserLogin(string mobileNumber, string password)
        {
            IsLoading = true;
            AuthToken token = new AuthToken();
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {

                try
                {
                  
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, App.ApiBaseUrl + "Authentication/customer/token");
                    //var request = new HttpRequestMessage(HttpMethod.Post, "10.0.2.2:5136/api/Authentication/customer/token");

                    request.Headers.Add("accept", "*/*");
                    //var content = new StringContent("{\"password\": \""+ password + "\",\"mobileNumber\": \""+ mobileNumber + "\"}", null, "application/json");
                    var content = new StringContent("{\r\n  \"password\": \"" + password + "\",\r\n  \"mobileNumber\": \"" + mobileNumber + "\"\r\n}", null, "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var resultContent = await response.Content.ReadAsStringAsync();
                        token = JsonConvert.DeserializeObject<AuthToken>(resultContent);
                       // IsLoading = false;

                        return token;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        IsLoading = false;
                        token.message = "Invalid username or password";
                        return token;
                    }
                    else
                    {
                        response.EnsureSuccessStatusCode();
                        token.message = "Error has been occurred";
                        IsLoading = false;
                        return token;
                    }
                    

                }
                catch (Exception ex)
                {
                    token.message = "Error has been occurred";
                    IsLoading = false;
                    return token;
                }
            }
            else
            {
                IsLoading = false;
                MessagingCenter.Send(this, "NoInternet");

                token.message = "No Internet Connection";
                return token;


            }

        }

    }
}
