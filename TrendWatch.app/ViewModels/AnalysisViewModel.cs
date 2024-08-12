

using CommunityToolkit.Mvvm.ComponentModel;
using TrendWatch.App.DataModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TrendWatch.App.Views;
using TrendWatch.App.Helpers;
using Microcharts;

namespace TrendWatch.App.ViewModels
{
    public class AnalysisViewModel : ObservableObject, INotifyPropertyChanged
    {
        private int _currentPage = 1;

        public ICommand ArticleClickCommand { get; }

        public FormattedString FormatedArticleAnalysis
        {
            get { return formattedString; }
            set { SetProperty(ref formattedString, value); }
        }


        bool _isLoading = true;
        bool _isLoadingAnalysis = true;
        bool _isChartLoading = true;
        bool _isKeywordsLoading = true;
        bool _isTopicsLoading = true;
        TrendData _trendData;
        public TrendData CurrentTrendData
        {
            get { return _trendData; }
            set { _trendData = value; }
        }
        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; }
        }
        public bool IsLoadingAnalysis
        {
            get { return _isLoadingAnalysis; }
            set { _isLoadingAnalysis = value; }
        }
        public bool IsChartLoading
        {
            get { return _isChartLoading; }
            set { _isChartLoading = value; }
        }
        public bool IsKeywordsLoading
        {
            get { return _isKeywordsLoading; }
            set { _isKeywordsLoading = value; }
        }
        public bool IsTopicsLoading
        {
            get { return _isTopicsLoading; }
            set { _isTopicsLoading = value; }
        }
        string analysisText = "";
        string chartData = "";
        List<string> _keyWords ;
        public List<string> KeyWords
        {
            get { return _keyWords; }
            set { _keyWords = value; }
        }
        List<string> _topics;
        public List<string> Topics
        {
            get { return _topics; }
            set { _topics = value; }
        }
        public ObservableCollection<Article> FeaturedNew { get; set; } = new ObservableCollection<Article>();
      

        public AnalysisViewModel()
        {
            ArticleClickCommand = new Command<Article>(OnArticleClick);
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    LoadData();
                }
                catch (Exception ex)
                {
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
       
            private async void LoadData()
        {

            FeaturedNew.Clear();
           // List<Article> featuredArticles = await LoadFeaturedArticles();
            _isLoadingAnalysis = true;
            RaisePropertyChanged("IsLoadingAnalysis");
            _trendData = await LoadTrendData();
            this.analysisText = _trendData.AnalysisText;
            // this.analysisText = await LoadTrendAnalysis();
            RaisePropertyChanged("CurrentTrendData");

            FormatArticleAnalys();
            RaisePropertyChanged("FormatedArticleAnalysis");
            _isLoadingAnalysis = false;
            RaisePropertyChanged("IsLoadingAnalysis");
            chartData = _trendData.Senti_Response;
            LoadChart();

            _isKeywordsLoading = true;
            RaisePropertyChanged("IsKeywordsLoading");
            string keyWordsStr = _trendData.TrendKeywords; //await ApiService.Generate("list trending keywords in the news: just word list: comma delimited");
            _keyWords = keyWordsStr.Split(",").ToList();


            string topicsStr = _trendData.TrendTopics;//await ApiService.Generate("list trending topics (topics with short description not kteywords) in the news: just topic list: comma delimited");
            int count = 1;
            _topics = new List<string>();
            foreach (var topic in topicsStr.Split(",").ToList()) {
                _topics.Add(count + ". " + topic.Trim());
                count++;
                if (count >= 6)
                    break;
            }
            RaisePropertyChanged("KeyWords");
            RaisePropertyChanged("Topics");

            _isKeywordsLoading = false;
            RaisePropertyChanged("IsKeywordsLoading");
           

        }
        public List<GraphPoint> chartPoints;
        private async void LoadChart()
        {

            _isChartLoading = true;
            RaisePropertyChanged("IsChartLoading");

            //string chartStr = await ApiService.Generate("Provide sentiment analysis of trending news in percentage for positive, negative and neutral .start response with in this format: positive:30,negative:40,nutral:50");
            var sentiLines = chartData.Split(",");
            chartPoints = new List<GraphPoint>();
            GraphPoint p;
            foreach (var line in sentiLines)
            {
                var senitValues = line.Split(":");
                p = new GraphPoint();
                if (senitValues[0].ToLower() == "positive")
                {
                    p.Title = "Positive";
                    p.Value = decimal.Parse(senitValues[1].Replace("%",""));
                    p.Color = "#48bf91";
                    chartPoints.Add(p);
                }
                else if (senitValues[0].ToLower() == "negative")
                {
                    p.Title = "Negative";
                    p.Value = decimal.Parse(senitValues[1].Replace("%", ""));
                    p.Color = "#ef1c26";
                    chartPoints.Add(p);
                }
                else
                {
                    p.Title = "Neutral";
                    p.Value = decimal.Parse(senitValues[1]);
                    p.Color = "#ccd7e9";
                    chartPoints.Add(p);
                }
            }
            MessagingCenter.Send(this, "ChartDataLoaded");
            
            _isChartLoading = false;
            RaisePropertyChanged("IsChartLoading");
        }
        FormattedString formattedString = new FormattedString();
        private void FormatArticleAnalys()
        {
            // analysisText = analysisText.Replace("{\"text\":\"", "").Replace("\\\"}\"", "");
            List<string> lines = analysisText.Split(new string[] { "\\n" }, StringSplitOptions.None).ToList();
            formattedString = new FormattedString();
            var span = new Span();

            foreach (var line in lines)
            {

                if (line.StartsWith("**"))
                {
                    span = new Span
                    {

                        Text = line.Replace("**", "") + Environment.NewLine,
                        FontSize = 13,
                        TextColor = Colors.Black,
                        FontAttributes = FontAttributes.Bold,
                    };
                    formattedString.Spans.Add(span);
                }

                else if (line.StartsWith("* **"))
                {
                    List<string> lines2 = line.Split(new string[] { " **" }, StringSplitOptions.None).ToList();

                    span = new Span
                    {

                        Text = lines2[0].Replace("*", ""),
                        FontSize = 12,
                        TextColor = Colors.Black,
                        FontAttributes = FontAttributes.Bold,
                    };
                    formattedString.Spans.Add(span);
                    span = new Span
                    {

                        Text = lines2[1].Replace("*", ""),
                        FontSize = 12,
                        TextColor = Colors.Black,
                    };
                    formattedString.Spans.Add(span);
                }
                else
                {
                    span = new Span
                    {

                        Text = line + Environment.NewLine,
                        FontSize = 12,
                        TextColor = Colors.Black
                    };
                    formattedString.Spans.Add(span);
                }


            }
        }
        async Task<string> LoadTrendAnalysis()
        {
            try
            {

                using (var client = new HttpClient())
                {

                    //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.AccessToken);



                    var request = new HttpRequestMessage(HttpMethod.Get, App.ApiBaseUrl + "Article/TrendAnalysis");
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
        async Task<TrendData> LoadTrendData()
        {
            try
            {

                using (var client = new HttpClient())
                {

                    //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.AccessToken);



                    var request = new HttpRequestMessage(HttpMethod.Get, App.ApiBaseUrl + "Article/GetTrendData");
                    request.Headers.Add("accept", "text/plain");
                    request.Headers.Add("Authorization", "Bearer " + App.AccessToken);
                    var response = await client.SendAsync(request);
                    TrendData result = new TrendData();
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseResult = JsonConvert.DeserializeObject<ApiResponse<TrendData>>(content);
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


