using Newtonsoft.Json;
using TrendWatch.App.DataModels;
using TrendWatch.App.ViewModels;
using System.Text;
using TrendWatch.App.Controls;
using CommunityToolkit.Maui.Views;

namespace TrendWatch.App.Views;

public partial class SettingPage : ContentPage
{
  
    public SettingPage()
	{
		InitializeComponent();
        BindingContext = this;
       
	}

   


   

    private void btnBack_Clicked(object sender, TappedEventArgs e)
    {

    }
}