using Newtonsoft.Json;
using TrendWatch.App.DataModels;
using TrendWatch.App.ViewModels;
using System.Text;
using TrendWatch.App.Controls;
using CommunityToolkit.Maui.Views;
using TrendWatch.App.Helpers;

namespace TrendWatch.App.Views;

public partial class SearchPage : ContentPage
{
    List<string> _topics;
    public List<string> Topics
    {
        get { return _topics; }
        set { _topics = value; }
    }
    public SearchPage()
	{
		InitializeComponent();
        Topics = new List<string>();
        //string topics =  ApiService.Generate("list trending keywords in the news: just word list: comma delimited").Result;
        //foreach (var topic in topics.Split(",").ToList())
        //{
        //    _topics.Add("- " + topic.Trim());

        //}
        Topics.Add("Russia-Ukraine war");
        Topics.Add("Climate change");
        Topics.Add("Inflation");
        Topics.Add("Cryptocurrency");
        Topics.Add("Artificial intelligence");
        Topics.Add("Mental health");
        Topics.Add("Social justice");
        Topics.Add("Racism");

        BindingContext = this;
    }

   


   

    private void btnBack_Clicked(object sender, TappedEventArgs e)
    {

    }
}