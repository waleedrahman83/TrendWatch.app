

using CommunityToolkit.Mvvm.ComponentModel;
using TrendWatch.App.DataModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TrendWatch.App.Views;

namespace TrendWatch.App.ViewModels
{
    public class RadioViewModel : ObservableObject, INotifyPropertyChanged
    {
        private int _currentPage = 1;
        private string _nowPlayingTitle = "";
        private bool _isRadioPlaying = false;
        private string _nowPlayingSummary = "";

        public ICommand ArticleClickCommand { get; }

        public int currentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; }
        }
       
        public bool IsRadioPlaying
        {
            get { return _isRadioPlaying; }
            set { _isRadioPlaying = value; }
        }
        public string NowPlayingTitle
        {
            get { return _nowPlayingTitle; }
            set { _nowPlayingTitle = value; }
        }
        public string NowPlayingSummary
        {
            get { return _nowPlayingSummary; }
            set { _nowPlayingSummary = value; }
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

        public RadioViewModel()
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
        //    FeaturedNew = new ObservableCollection<Article>
        //{
        //    new Article
        //    {
        //        ID = Guid.NewGuid(),
        //        Title = "Breaking News: AI Revolution",
        //        Body = "Artificial Intelligence is taking over the world...",
        //        Summary = "An overview of the impact of AI on various industries.",
        //        Author = "Jane Doe",
        //        NewsSourceID = Guid.NewGuid(),
        //        ArticleDate = DateTime.Now,
        //        ImageURL = "https://cdn.cnn.com/cnnnext/dam/assets/221104122013-rsv-adults-wellness-stock-t1-main.jpg",
        //        ImageDescription = "A robot shaking hands with a human.",
        //        EstimatedReadingSeconds = 300,
        //        ArticleURL = "https://example.com/article1",
        //        IsFake = false
        //    },
        //    new Article
        //    {
        //        ID = Guid.NewGuid(),
        //        Title = "Health Benefits of Morning Walks",
        //        Body = "Morning walks are beneficial for your health...",
        //        Summary = "Exploring the various health benefits of morning walks.",
        //        Author = "John Smith",
        //        NewsSourceID = Guid.NewGuid(),
        //        ArticleDate = DateTime.Now,
        //        ImageURL = "https://cdn.cnn.com/cnnnext/dam/assets/221114145132-covid-booster-children-file-story-body.jpg",
        //        ImageDescription = "A person walking in a park in the morning.",
        //        EstimatedReadingSeconds = 180,
        //        ArticleURL = "https://example.com/article2",
        //        IsFake = false
        //    },
        //    new Article
        //    {
        //        ID = Guid.NewGuid(),
        //        Title = "The Future of Electric Cars",
        //        Body = "Electric cars are becoming more popular...",
        //        Summary = "A look at the advancements in electric car technology.",
        //        Author = "Alice Johnson",
        //        NewsSourceID = Guid.NewGuid(),
        //        ArticleDate = DateTime.Now,
        //        ImageURL = "https://cdn.cnn.com/cnnnext/dam/assets/140318154551-alzheimers-0317-story-body.jpg",
        //        ImageDescription = "An electric car charging at a station.",
        //        EstimatedReadingSeconds = 240,
        //        ArticleURL = "https://example.com/article3",
        //        IsFake = false
        //    }
        //};
        }
            private async void LoadAllNews()
        {

            FeaturedNew.Clear();
            List<Article> featuredArticles = await LoadFeaturedArticles();
                if (featuredArticles != null)
                {
                    foreach (Article article in featuredArticles)
                    {
                        FeaturedNew.Add(article);
                    }
                //_nowPlayingTitle = FeaturedNew[0].Title;
                //_nowPlayingSummary = FeaturedNew[0].Summary;
                //FeaturedNew[0].IsPlaying = true;

                
              

        MessagingCenter.Send(this, "RadioDataHasLoaded");
               

                _isLoading = false;
                    RaisePropertyChanged("IsLoading");
                    _hasInvoices = Invoices.Count > 0;
                }
                else
                {
                    _isLoading = false;
                    RaisePropertyChanged("IsLoading");
                   
                }
            
        }
        public void UpdatePlaying(Article article)
        {
            _nowPlayingTitle = article.Title;
            _nowPlayingSummary = article.Summary; 
            RaisePropertyChanged("FeaturedNew");
            RaisePropertyChanged("IsRadioPlaying");

            RaisePropertyChanged("NowPlayingTitle");
            RaisePropertyChanged("NnowPlayingSummary");
        }

        async Task<List<Article>> LoadFeaturedArticles()
        {
            using (var client = new HttpClient())
            {
                string endpoint = $"Article/GetFeaturedArticles";
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


