

using CommunityToolkit.Mvvm.ComponentModel;
using TrendWatch.App.DataModels;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Input;

namespace TrendWatch.App.ViewModels
{
    public class ArticleAnalysisViewModel : ObservableObject, INotifyPropertyChanged
    {
        string analysisText = "";
        private bool _isLoading = true;
        private bool _isSeniment = true;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; }
        }
        public ICommand OpenWebCommand { get; }

        public ArticleAnalysisViewModel(Guid articleId,bool isSentiment)
        {
            try
            {
                IsLoading = true;
                RaisePropertyChanged("IsLoading");
                _isSeniment = isSentiment;
                LoadArticle(articleId);
            }
            catch
            {

            }
        }
        private async void LoadArticle(Guid articleId)
        {
            if(!_isSeniment)
            this.analysisText = await LoadArticleAnalysis(articleId);
            else
                this.analysisText = await LoadArticleAnalysis_sentiment(articleId);

            FormatArticleAnalys();
            RaisePropertyChanged("FormatedArticleAnalysis");

            IsLoading = false;
            RaisePropertyChanged("IsLoading");

            //  NotifyPropertyChanged("CurrentHomeStats");
            RaisePropertyChanged("ArticleAnalysis");


        }
        FormattedString formattedString = new FormattedString();
        private void FormatArticleAnalys()
        {
           // analysisText = analysisText.Replace("{\"text\":\"", "").Replace("\\\"}\"", "");
            List<string> lines =  analysisText.Split(new string[] { "\\n" }, StringSplitOptions.None).ToList();
            formattedString = new FormattedString();
            var span = new Span();
            
            foreach (var line in lines)
            {

                if (line.StartsWith("**"))
                {
                    span = new Span
                    {

                        Text = line.Replace("**","") + Environment.NewLine,
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

                        Text = lines2[0].Replace("*", "") ,
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
        async Task<string> LoadArticleAnalysis(Guid articleId)
        {
            try
            {

                using (var client = new HttpClient())
                {

                    //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.AccessToken);



                    var request = new HttpRequestMessage(HttpMethod.Get, App.ApiBaseUrl + "Article/" + articleId + "/Analyze");
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
        async Task<string> LoadArticleAnalysis_sentiment(Guid articleId)
        {
            try
            {

                using (var client = new HttpClient())
                {

                    //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.AccessToken);



                    var request = new HttpRequestMessage(HttpMethod.Get, App.ApiBaseUrl + "Article/" + articleId + "/SentimentAnalyze");
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

        public String ArticleAnalysis
        {
            get { return analysisText; }
            set { SetProperty(ref analysisText, value); }
        }
        public FormattedString FormatedArticleAnalysis
        {
            get { return formattedString; }
            set { SetProperty(ref formattedString, value); }
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


