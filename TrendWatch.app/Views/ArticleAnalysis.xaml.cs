//using Android.Media.TV;
using TrendWatch.App.ViewModels;
using Newtonsoft.Json;
using System.Text;
using TrendWatch.App.DataModels;
using Microsoft.Maui.Controls;

namespace TrendWatch.App.Views;

public partial class ArticleAnalysis : ContentPage
{
    public ArticleAnalysis(Guid articleId,bool sentiment)
    {

        InitializeComponent();
        BindingContext = new ArticleAnalysisViewModel(articleId, sentiment);
       
    }


    



    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PopModalAsync();

        //Navigation.PopModalAsync();

    }
    protected override bool OnBackButtonPressed()
    {
         Application.Current.MainPage.Navigation.PopModalAsync();

        //Navigation.PopModalAsync();
        return true;
    }
    async Task<string> LoadArticleAnalysis(Guid articleId)
    {
        try
        {

            using (var client = new HttpClient())
            {

                //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.AccessToken);



                var request = new HttpRequestMessage(HttpMethod.Get, App.ApiBaseUrl + "Article/" + articleId+ "/Analyze");
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


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
    }
}