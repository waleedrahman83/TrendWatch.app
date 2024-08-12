

using CommunityToolkit.Mvvm.ComponentModel;
using TrendWatch.App.DataModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TrendWatch.App.Views;

namespace TrendWatch.App.ViewModels
{
    public class ForMeViewModel : ObservableObject, INotifyPropertyChanged
    {
        private int _currentPage = 1;
        private string _theRunDown = "";

        public ICommand ArticleClickCommand { get; }

        public int currentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; }
        }
        public string TheRunDown
        {
            get { return _theRunDown; }
            set { _theRunDown = value; }
        }
        bool _isLoading = true;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; }
        }
        bool _hasInvoices = false;
        public bool HasInvoices
        {
            get { return _hasInvoices; }
            set { _hasInvoices = value; }
        }

        bool _hasMoreInvoices = true;
        public bool HasMoreInvoices
        {
            get { return _hasMoreInvoices; }
            set { SetProperty(ref _hasMoreInvoices, value); }
        }

        public ObservableCollection<InvoiceMin> Invoices { get; set; } = new ObservableCollection<InvoiceMin>();
        public ObservableCollection<Article> FeaturedNew { get; set; } = new ObservableCollection<Article>();
        public ObservableCollection<Article> ForMe { get; set; } = new ObservableCollection<Article>();
        public ObservableCollection<Article> BookMarkedNews { get; set; } = new ObservableCollection<Article>();


        //public ObservableCollection<Grouped_list> InvoicesGrouped { get; set; } = new ObservableCollection<Grouped_list>();

        //public ObservableCollection<List<Grouped_list> InvoiceList { get; set; } = new List<Grouped_list>();



        private ObservableCollection<Grouped_list> invoicesGrouped = new ObservableCollection<Grouped_list>();
        public ObservableCollection<Grouped_list> InvoicesGrouped
        {
            get { return invoicesGrouped; }
            set
            {
                invoicesGrouped = value;
                OnPropertyChanged(nameof(InvoicesGrouped));
            }

        }

        public ForMeViewModel()
        {
            ArticleClickCommand = new Command<Article>(OnArticleClick);
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    LoadNews();
                }
                catch (Exception ex)
                {
                    MessagingCenter.Send(this, "InvoiceLoadingError");
                }
            }
            else
            {
                MessagingCenter.Send(this, "NoInternet");
            }
        }
        private async void OnArticleClick(Article article)
        {
            if (article != null)
            {

                await Application.Current.MainPage.Navigation.PushModalAsync(new ArticleDetail(article.ID), false);
                //Application.Current.MainPage.DisplayAlert("Item Clicked", $"You clicked: {article.Title}", "OK");
            }
        }
        private async void LoadNews()
        {
            LoadAllNews();
          
        }
        private async void LoadAllNews()
        {

            FeaturedNew.Clear();
            List<Article> featuredArticles = await LoadUserNews(App.CustomerID);
            if (featuredArticles != null)
            {
                foreach (Article article in featuredArticles)
                {
                    ForMe.Add(article);
                }
                List<Article> bookmarkedArticles = await LoadUserBookMarks(App.CustomerID);
                if (bookmarkedArticles != null)
                {
                    foreach (Article article in bookmarkedArticles)
                    {
                        BookMarkedNews.Add(article);
                    }
                }

                    _isLoading = false;
                    RaisePropertyChanged("IsLoading");
                    _hasInvoices = Invoices.Count > 0;
                }
                else
                {
                    _isLoading = false;
                    RaisePropertyChanged("IsLoading");

                }
                _theRunDown = await LoadRunDown();
                RaisePropertyChanged("TheRunDown");

                //Invoices = new ObservableCollection<Invoice>
                //    {
                //        new Invoice
                //        {
                //           InvoiceDate= DateTime.Now,
                //           ReferenceNumber = "1234",
                //           Amount= 300,
                //           NoOfItems = 2
                //        },
                //        new Invoice
                //        {
                //           InvoiceDate= DateTime.Now,
                //           ReferenceNumber = "456",
                //           Amount= 500,
                //           NoOfItems = 3
                //        }
                //        ,
                //        new Invoice
                //        {
                //           InvoiceDate= DateTime.Now.AddDays(-1),
                //           ReferenceNumber = "11456",
                //           Amount= 500,
                //           NoOfItems = 3
                //        }
                //         ,
                //        new Invoice
                //        {
                //           InvoiceDate= DateTime.Now.AddDays(-3),
                //           ReferenceNumber = "11456",
                //           Amount= 500,
                //           NoOfItems = 3
                //        }
                //};
                //var dict = (Invoices.OrderByDescending(x => x.InvoiceDate)).GroupBy(o => o.InvoiceDate.Date).ToDictionary(g => g.Key, g => g.ToList());
                //foreach (KeyValuePair<DateTime, List<Invoice>> item in dict)
                //{

                //    InvoicesGrouped.Add(new Grouped_list(item.Key, new List<Invoice>(item.Value)));
                //}
            }
            async Task<string> LoadRunDown()
            {
                try
                {

                    using (var client = new HttpClient())
                    {

                        //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.AccessToken);



                        var request = new HttpRequestMessage(HttpMethod.Get, App.ApiBaseUrl + "Article/TheRunDown");
                        request.Headers.Add("accept", "text/plain");
                        request.Headers.Add("Authorization", "Bearer " + App.AccessToken);
                        var response = await client.SendAsync(request);
                        string result = "";
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var responseResult = JsonConvert.DeserializeObject<ApiResponse<string>>(content);
                            if (responseResult.success)
                            {
                                result = responseResult.data;
                            }
                        }
                        return result;
                    }
                }
                catch (Exception ex)
                {

                    return null;
                }

            }

            async Task<List<Article>> LoadUserNews(Guid userID)
            {
                using (var client = new HttpClient())
                {
                    string endpoint = $"Article/GetNewsForUser/" + userID;
                    string url = $"{App.ApiBaseUrl}{endpoint}";

                    //var request = new HttpRequestMessage(HttpMethod.Post, App.ApiBaseUrl +"/Customer/"+customerId+"/invoices?pageIndex=1&pageSize=10");

                    var request = new HttpRequestMessage(HttpMethod.Get, url);

                    request.Headers.Add("accept", "text/plain");
                    request.Headers.Add("Authorization", "Bearer " + App.AccessToken);
                    var response = await client.SendAsync(request);




                    List<Article> result = new List<Article>();
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseResult = JsonConvert.DeserializeObject<ApiResponse<List<Article>>>(content);
                        if (responseResult.success)
                        {
                            result = responseResult.data;
                        }
                    }
                    return result;
                }


            }
            async Task<List<Article>> LoadUserBookMarks(Guid userID)
            {
                using (var client = new HttpClient())
                {
                    string endpoint = $"Article/GetBookMarksForUser/" + userID;
                    string url = $"{App.ApiBaseUrl}{endpoint}";

                    //var request = new HttpRequestMessage(HttpMethod.Post, App.ApiBaseUrl +"/Customer/"+customerId+"/invoices?pageIndex=1&pageSize=10");

                    var request = new HttpRequestMessage(HttpMethod.Get, url);

                    request.Headers.Add("accept", "text/plain");
                    request.Headers.Add("Authorization", "Bearer " + App.AccessToken);
                    var response = await client.SendAsync(request);




                    List<Article> result = new List<Article>();
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseResult = JsonConvert.DeserializeObject<ApiResponse<List<Article>>>(content);
                        if (responseResult.success)
                        {
                            result = responseResult.data;
                        }
                    }
                    return result;
                }
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

    

    }

    




}


