namespace TrendWatch.App.Views;

using TrendWatch.App.ViewModels;
using Microcharts;
using Newtonsoft.Json;
using SkiaSharp;

public partial class ForMePage : ContentPage
{

    private ForMeViewModel viewModel;


    public ForMePage()
    {
        InitializeComponent();

        viewModel = new ForMeViewModel();
          BindingContext = viewModel;

       
       
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
    }
   
   
    protected override bool OnBackButtonPressed()
    {
        ((AppShell)App.Current.MainPage).SwitchToHome();
        return true;
    }



    

  
}





