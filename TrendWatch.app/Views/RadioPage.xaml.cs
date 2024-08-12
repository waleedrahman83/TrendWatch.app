namespace TrendWatch.App.Views;

using TrendWatch.App.ViewModels;
using Microcharts;
using Newtonsoft.Json;
using SkiaSharp;

public partial class RadioPage : ContentPage
{

    private RadioViewModel viewModel;
    CancellationTokenSource cts;

    public RadioPage()
    {
        InitializeComponent();
        cts = new CancellationTokenSource();
        MessagingCenter.Subscribe<RadioViewModel>(this, "RadioDataHasLoaded", async (sender) =>
        {
            if (App.PlayRadio)
            {
                App.PlayRadio = false;
                Play();
                //if (isPlaying)
                //{
                //    isPlaying = false;
                //    CancelSpeech();
                //    return;
                //}
            } 
        });

        viewModel = new RadioViewModel();


    }
   private async void Play()
    {
        try
        {
            viewModel.IsRadioPlaying = true;


            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var enLanguage = locales.Where(l => l.Language.ToLower() == "en").ToList();


            var commentorVoiceOption = new SpeechOptions
            {
                Volume = 1.0f,
                Pitch = 1.0f,
                Locale = enLanguage[2]
            };
            var readerVoiceOption = new SpeechOptions
            {
                Volume = 1.0f,
                Pitch = 1.0f,
                Locale = enLanguage[0]
            };
            foreach (var article in viewModel.FeaturedNew)
            {
                viewModel.UpdatePlaying(article);
                article.IsPlaying = true;
                if (viewModel.IsRadioPlaying)
                    await TextToSpeech.Default.SpeakAsync("News Title ", commentorVoiceOption, cancelToken: cts.Token);
                if (viewModel.IsRadioPlaying)
                    await TextToSpeech.Default.SpeakAsync(article.Title, readerVoiceOption, cancelToken: cts.Token);
                if (viewModel.IsRadioPlaying)
                    await TextToSpeech.Default.SpeakAsync("Summary ", commentorVoiceOption, cancelToken: cts.Token);
                if (viewModel.IsRadioPlaying)
                    await TextToSpeech.Default.SpeakAsync(article.Summary, readerVoiceOption, cancelToken: cts.Token);
                article.IsPlaying = false;

            }
        }
        catch
        {

        }

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
          BindingContext = viewModel;
       
    }
    public void CancelSpeech()
    {
        if (cts?.IsCancellationRequested ?? true)
            return;

        cts.Cancel();
    }


    protected override bool OnBackButtonPressed()
    {
        ((AppShell)App.Current.MainPage).SwitchToHome();
        return true;
    }

    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();

    }

    private void Stop_Tapped(object sender, TappedEventArgs e)
    {
        viewModel.IsRadioPlaying = false;
        CancelSpeech();
    }
    private void Play_Tapped(object sender, TappedEventArgs e)
    {
        Play();
    }
    }





