

using CommunityToolkit.Mvvm.ComponentModel;
using TrendWatch.App.DataModels;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Input;
using TrendWatch.App.Helpers;

namespace TrendWatch.App.ViewModels
{
    public class ArticleDetailsViewModel : ObservableObject, INotifyPropertyChanged
    {
        Article article;
        bool isFake = false;
        public ICommand OpenWebCommand { get; }
        List<string> _relatedNews;
        bool _isRelatedLoading = true;
        public bool IsRelatedLoading
        {
            get { return _isRelatedLoading; }
            set { _isRelatedLoading = value; }
        }
        public List<string> RelatedNews
        {
            get { return _relatedNews; }
            set { _relatedNews = value; }
        }
        public ArticleDetailsViewModel(Guid articleId)
        {
            try
            {
               
                LoadArticle(articleId);
            }
            catch
            {

            }
        }
        public async void Translate(string lang)
        {
            var translatedArticle = await TranslateArticle(article.ID, lang);
            this.article = translatedArticle;
            RaisePropertyChanged("CurrentArticle");

        }
        List<string> _tags;
        public List<string> Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }
        private async void LoadArticle(Guid articleId)
        {
            article = await GetArticleDetails(articleId);
            RaisePropertyChanged("CurrentArticle");
            string tags = article.Tags;//await ApiService.Generate("list trending topics (topics with short description not kteywords) in the news: just topic list: comma delimited");
            int count = 1;
            _tags = new List<string>();
            foreach (var topic in tags.Split(",").ToList())
            {
                _tags.Add( topic.Trim());
                count++;
                if (count >= 3)
                    break;
            }
            RaisePropertyChanged("Tags");
            //string isFakeText = //await CheckIsFake(articleId);
            //isFakeText = isFakeText.Replace("{\"text\":\"", "");
            //if(isFakeText.StartsWith("Fake News") || isFakeText.StartsWith("(Fake News)"))
            //{
            //    isFake = true;
            //}
            //else isFake = false;
            //RaisePropertyChanged("IsFake");

            string topicsStr = await ApiService.Generate("get related news of this (just list of titles, # separated):"+CurrentArticle.Title);
            _relatedNews = new List<string>();
            foreach (var topic in topicsStr.Split("#").ToList())
            {
                _relatedNews.Add("- " + topic.Trim());
               
            }
            RaisePropertyChanged("RelatedNews");

            _isRelatedLoading = false;
            RaisePropertyChanged("IsRelatedLoading");

        }
        public Article CurrentArticle
        {
            get { return article; }
            set { SetProperty(ref article, value); }
        }
        public bool IsFake
        {
            get { return isFake; }
            set { SetProperty(ref isFake, value); }
        }
        async Task<Article> GetArticleDetails(Guid articleId)
        {
            try
            {

                using (var client = new HttpClient())
                {

                    //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.AccessToken);



                    var request = new HttpRequestMessage(HttpMethod.Get, App.ApiBaseUrl + "Article/" + articleId);
                    request.Headers.Add("accept", "text/plain");
                    request.Headers.Add("Authorization", "Bearer " + App.AccessToken);
                    var response = await client.SendAsync(request);

                    Article result = new Article();
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseResult = JsonConvert.DeserializeObject<ArticleDetailsReponse>(content);
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
        async Task<string> CheckIsFake(Guid articleId)
        {
            try
            {

                using (var client = new HttpClient())
                {

                    //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.AccessToken);



                    var request = new HttpRequestMessage(HttpMethod.Get, App.ApiBaseUrl + "Article/" + articleId + "/IsFake");
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
        async Task<Article> TranslateArticle(Guid articleId,string lang)
        {
            try
            {

                using (var client = new HttpClient())
                {

                    //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.AccessToken);



                    var request = new HttpRequestMessage(HttpMethod.Get, App.ApiBaseUrl + "Article/" + articleId + "/TranslateArticle?lang="+lang);
                    request.Headers.Add("accept", "text/plain");
                    request.Headers.Add("Authorization", "Bearer " + App.AccessToken);
                    var response = await client.SendAsync(request);
                    Article result = new Article();

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseResult = JsonConvert.DeserializeObject<ArticleDetailsReponse>(content);

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
        //async Task<Invoice> GetInvoiceDetails(Guid invoiceId)
        //{
        //    try
        //    {

        //        using (var client = new HttpClient())
        //        {

        //          //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.AccessToken);



        //            var requestURI = App.ApiBaseUrl + "job/GetInvoiceDetails?invoiceId=" + invoiceId;
        //            var request = new HttpRequestMessage(HttpMethod.Get, requestURI);
        //            var response = await client.SendAsync(request);



        //            Invoice result = new Invoice();
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var content = await response.Content.ReadAsStringAsync();
        //                result = JsonConvert.DeserializeObject<Invoice>(content);
        //            }
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return null;
        //    }

        //}
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


