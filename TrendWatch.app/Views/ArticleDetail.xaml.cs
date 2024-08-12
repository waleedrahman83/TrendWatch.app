//using Android.Media.TV;
using TrendWatch.App.ViewModels;
using Newtonsoft.Json;
using System.Text;
using TrendWatch.App.DataModels;
using System.Globalization;
using TrendWatch.App.Controls;
using CommunityToolkit.Maui.Views;

namespace TrendWatch.App.Views;

public partial class ArticleDetail : ContentPage
{
    Guid CurrentAricleId;
    ArticleDetailsViewModel vm;
    public ArticleDetail(Guid articleId)
    {
        CurrentAricleId = articleId;
        InitializeComponent();
        vm = new ArticleDetailsViewModel(articleId);
        BindingContext = vm;
        //var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
        //                            .OrderBy(c => c.DisplayName)
        //                            .Select(c => c.DisplayName)
        //                            .ToList();
        var commonLanguages = new List<string>
            {
                "Arabic",
                "Chinese",
                "Hindi",
                "Spanish",
                "French",
                "Bengali",
                "Russian",
                "Portuguese",
                "Indonesian",
                "Urdu",
                "German",
                "Japanese",
                "Turkish",
                "Vietnamese",
                "Korean",
                "Italian",
                "Thai",
                "Polish",
                "Ukrainian",
                "Malayalam"
                // Add more languages if needed
            };

        // Set the Picker's ItemsSource
        picker.ItemsSource = commonLanguages;
        picker.SelectedIndexChanged += Picker_SelectedIndexChanged;
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (picker.SelectedIndex != -1)
        {
            int selectedIndex = picker.SelectedIndex;
            string selectedItem = picker.Items[selectedIndex];
            picker.IsVisible = false;
            vm.Translate(selectedItem);
            // Display the selected index
        }
    }

    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
       
    }
    private async void fact_Clicked(object sender, EventArgs e)
    {
        
        var popup = new NotificationPopup("Fact Checking!", vm.CurrentArticle.IsFakeText, "check.svg", Color.FromRgb(242, 63, 47), false, "Dismiss", "TryAgainMessageCenter");// to the internet.\n Please check your connection and try again.");
        await this.ShowPopupAsync(popup);
    }




    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }

    private async void Analyez_Tapped(object sender, TappedEventArgs e)
    {
        string text = await LoadArticleAnalysis(CurrentAricleId);

        var popup = new Analysis_NotificationPopup("Article Analysis!", text, "analyze.svg", Color.FromRgb(242, 63, 47), false, "Dismiss", "TryAgainMessageCenter");// to the internet.\n Please check your connection and try again.");
        await this.ShowPopupAsync(popup);
        //await Navigation.PushModalAsync(new ArticleAnalysis(CurrentAricleId,false), false);

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
    private async void Sentiment_Tapped(object sender, TappedEventArgs e)
    {
        // await Navigation.PushModalAsync(new ArticleAnalysis(CurrentAricleId, true), false);
        string text = await LoadArticleAnalysis_sentiment(CurrentAricleId);

        var popup = new Analysis_NotificationPopup("Sentiment Analysis!", text, "analyze.svg", Color.FromRgb(242, 63, 47), false, "Dismiss", "TryAgainMessageCenter");// to the internet.\n Please check your connection and try again.");
        await this.ShowPopupAsync(popup);

    }
    private async void Translate_Tapped(object sender, TappedEventArgs e)
    {
        picker.IsVisible = true;
        picker.Focus();

    }
    private async void External_Tapped(object sender, TappedEventArgs e)
    {
        await Browser.OpenAsync(new Uri(vm.CurrentArticle.ArticleURL));


    }

    private async void Speek_Tapped(object sender, EventArgs e)
    {
        var locales = await TextToSpeech.Default.GetLocalesAsync();
        var enLanguage = locales.Where(l=>l.Language.ToLower()=="en").ToList();


        var speechOptions = new SpeechOptions
        {
            Volume = 1.0f,
            Pitch = 1.0f,
            Locale = enLanguage[0]
        };
        await TextToSpeech.Default.SpeakAsync("News Title ", speechOptions);
        speechOptions = new SpeechOptions
        {
            Volume = 1.0f,
            Pitch = 1.2f,
            Locale = enLanguage[1]
        };
        await TextToSpeech.Default.SpeakAsync(vm.CurrentArticle.Title, speechOptions);

        speechOptions = new SpeechOptions
        {
            Volume = 1.0f,
            Pitch = 1.0f,
            Locale = enLanguage[0]
        };
        await TextToSpeech.Default.SpeakAsync("Summary ", speechOptions);
        speechOptions = new SpeechOptions
        {
            Volume = 1.0f,
            Pitch = 1.2f,
            Locale = enLanguage[1]
        };
        await TextToSpeech.Default.SpeakAsync(vm.CurrentArticle.Summary, speechOptions);
        // var speechOptions2 = new SpeechOptions
        //{
        //    Volume = 1.0f,
        //    Pitch = 1.0f,
        //     Locale = enLanguage[2]
        // };
        //await TextToSpeech.Default.SpeakAsync(vm.CurrentArticle.Summary, speechOptions2);
    }
}