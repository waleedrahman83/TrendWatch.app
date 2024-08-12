using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendWatch.App.ViewModels
{
    public class NotificationViewModel : ObservableObject, INotifyPropertyChanged
    {
        private bool _isLoading = true;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; }
        }
        bool _hasNoti = false;
        public bool HasNoti
        {
            get { return _hasNoti; }
            set { SetProperty(ref _hasNoti, value); }
        }
        public ObservableCollection<Notification> Notification { get; set; } = new ObservableCollection<Notification>();

        public NotificationViewModel()
        {
            LoadNotification();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async void LoadNotification()
        {
            Notification.Clear();
            var Noti = await LoadNoti();
            foreach (var notification in Noti)
            {
                Notification.Add(notification);
            }
            RaisePropertyChanged("Notification");
            _isLoading = false;
            RaisePropertyChanged("IsLoading");
            _hasNoti = Notification.Count > 0;

            RaisePropertyChanged("HasNoti");
        }

        async Task<List<Notification>> LoadNoti()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, App.ApiBaseUrl + "Notification");
                    request.Headers.Add("accept", "text/plain");
                    request.Headers.Add("Authorization", "Bearer " + App.AccessToken);
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseResult = JsonConvert.DeserializeObject<ResultViewModel<List<Notification>>>(content); // تغيير النوع إلى List<Notification>
                        if (responseResult.Success )
                        {
                            //Notification.Clear(); 
                            //foreach (var notification in responseResult.Data)
                            //{
                            //    Notification.Add(notification); 
                            //}
                            return responseResult.Data;
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in LoadNoti: {ex.Message}");
                return null;
            }
        }

    }
}
