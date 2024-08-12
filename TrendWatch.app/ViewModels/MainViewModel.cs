

using CommunityToolkit.Mvvm.ComponentModel;
using TrendWatch.App.DataModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TrendWatch.App.Views;
using TrendWatch.App.Helpers;

namespace TrendWatch.App.ViewModels
{
    public class MainViewModel : ObservableObject, INotifyPropertyChanged
    {
        private int _currentPage = 1;
        private string _theRunDown = "";

        public ICommand ArticleClickCommand { get; }
        public ICommand CategoryClickCommand { get; }


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
        bool _isKeywordsLoading = true;
        public bool IsKeywordsLoading
        {
            get { return _isKeywordsLoading; }
            set { _isKeywordsLoading = value; }
        }
        List<string> _keyWords = new List<string>();
        public List<string> KeyWords
        {
            get { return _keyWords; }
            set { _keyWords = value; }
        }
        public ObservableCollection<InvoiceMin> Invoices { get; set; } = new ObservableCollection<InvoiceMin>();
        public ObservableCollection<Article> FeaturedNew { get; set; } = new ObservableCollection<Article>();
        public ObservableCollection<Article> CategorizedNews { get; set; } = new ObservableCollection<Article>();
        public ObservableCollection<Article> BreakingNews { get; set; } = new ObservableCollection<Article>();


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

        private ObservableCollection<Grouped_news> articleGrouped = new ObservableCollection<Grouped_news>();
        public ObservableCollection<Grouped_news> ArticleGrouped
        {
            get { return articleGrouped; }
            set
            {
                articleGrouped = value;
                OnPropertyChanged(nameof(ArticleGrouped));
            }

        }

        public MainViewModel()
        {
            ArticleClickCommand = new Command<Article>(OnArticleClick);
            CategoryClickCommand = new Command<string>(OnCategoryClicked);

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
        private async void OnCategoryClicked(string category)
        {
            CategorizedNews.Clear();
            foreach (Article article in catgorizedArticle.Where(cat => cat.CategoryName == category))
            {
                CategorizedNews.Add(article);
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
        List<Article> catgorizedArticle;
            private async void LoadAllNews()
        {

            FeaturedNew.Clear();
            BreakingNews.Clear();

            CategorizedNews.Clear();

            List<Article> featuredArticles = await LoadFeaturedArticles();
            List<Article> breakingArticles = await LoadBreakingNews();
             catgorizedArticle = await LoadCatgoriezedNews();


            _isKeywordsLoading = true;
            RaisePropertyChanged("IsKeywordsLoading");
           // string keyWordsStr = await ApiService.Generate("list trending keywords in the news: just word list: comma delimited");
            //_keyWords = keyWordsStr.Split(",").ToList();

            
            if (featuredArticles != null)
                {
                foreach (Article article in featuredArticles)
                {
                    FeaturedNew.Add(article);
                }
                foreach (Article article in breakingArticles)
                {
                    BreakingNews.Add(article);
                }
                var dict = (catgorizedArticle.OrderBy(x => x.CategoryName)).GroupBy(o => o.CategoryName).ToDictionary(g => g.Key, g => g.ToList());
                List<string> allCategories = new List<string>();
                foreach (KeyValuePair<string, List<Article>> item in dict)
                {
                   _keyWords.Add(item.Key);
                    ArticleGrouped.Add(new Grouped_news(item.Key, new List<Article>(item.Value)));
                }
                RaisePropertyChanged("KeyWords");

                _isKeywordsLoading = false;
                RaisePropertyChanged("IsKeywordsLoading");

                foreach (Article article in catgorizedArticle.Where(cat => cat.CategoryName == _keyWords[0])) 
                {
                    CategorizedNews.Add(article);
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
        async Task<List<Article>> LoadBreakingNews()
        {
            using (var client = new HttpClient())
            {
                string endpoint = $"Article/GetBreakingNews";
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
        async Task<List<Article>> LoadCatgoriezedNews()
        {
            using (var client = new HttpClient())
            {
                string endpoint = $"Article/GetCatgoriezedNews";
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

        //public async Task LoadMoreInvoices()
        //{
        //    if (!IsLoading || !HasMoreInvoices) return;

        //    IsLoading = true;

        //    int nextPage = currentPage + 1;
        //    var moreInvoices = await LoadCustomerInvoices(App.CustomerID, nextPage);

        //    if (moreInvoices.Any())
        //    {
        //        var groupedInvoices = moreInvoices.GroupBy(inv => inv.InvoiceDate.Date)
        //                                         .ToDictionary(g => g.Key, g => g.ToList());
        //        foreach (var group in groupedInvoices)
        //        {
        //            var existingGroup = InvoicesGrouped.FirstOrDefault(g => g.Day == group.Key);
        //            if (existingGroup != null)
        //            {
        //                foreach (var invoice in group.Value)
        //                {
        //                    existingGroup.Add(invoice);
        //                }
        //            }
        //            else
        //            {
        //                InvoicesGrouped.Add(new Grouped_list(group.Key, group.Value));
        //            }
        //        }
        //        currentPage = nextPage;
        //    }
        //    else
        //    {
        //        HasMoreInvoices = false;
        //    }

        //    IsLoading = false;
        //}




    }

    public class Grouped_list : List<InvoiceMin>
    {

        public DateTime Day { get; set; }

        public Grouped_list(DateTime day, List<InvoiceMin> invoice) : base(invoice)
        {
            Day = day;

        }
    }
    public class Grouped_news : List<Article>
    {

        public string category { get; set; }

        public Grouped_news(string _day, List<Article> articles) : base(articles)
        {
            category = _day;

        }
    }



}