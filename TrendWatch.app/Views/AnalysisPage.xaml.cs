namespace TrendWatch.App.Views;

using TrendWatch.App.ViewModels;
using Microcharts;
using Newtonsoft.Json;
using SkiaSharp;
using System.Collections.Generic;

public partial class AnalysisPage : ContentPage
{

    private AnalysisViewModel viewModel;


    public AnalysisPage()
    {
        InitializeComponent();
        MessagingCenter.Subscribe<AnalysisViewModel>(this, "ChartDataLoaded", async (sender) =>
        {
            LoadChartAsync(viewModel.chartPoints);
        });
        viewModel = new AnalysisViewModel();
          BindingContext = viewModel;

       
       
    }
    private async Task LoadChartAsync(List<GraphPoint> graphPoints)
    {
      //  var graphPoints = await GenerateChartPointsAsync(chartDate);
        decimal totalSpendings = 0;
        foreach (var point in graphPoints)
        {
            totalSpendings = totalSpendings + point.Value;
        }
        var entries = ConvertToChartEntries(graphPoints);

        if (entries.Any())
        {
            chartMain.Chart = new PieChart()
            {
                Entries = entries,
                IsAnimated = true,
                LabelTextSize = 26,
                Typeface = SKTypeface.FromFamilyName("InterRegular"),
                //CornerRadius = 30,

            };

            //  chartMain.HeightRequest = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density * 0.3;  // 30% من ارتفاع الشاشة
            // chartMain.WidthRequest = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density * 0.8;  // 80% من عرض الشاشة


        }
        else
        {
            Console.WriteLine("No data to display.");
        }
    }
    private List<ChartEntry> ConvertToChartEntries(List<GraphPoint> graphPoints)
    {
        if (graphPoints == null)
        {
            return new List<ChartEntry>();
        }

        return graphPoints.Select(gp => new ChartEntry((float)gp.Value)
        {
            Label = gp.Title,
            ValueLabel = gp.Value.ToString(),
            Color = SKColor.Parse(gp.Color)
        }).ToList();
    }
    private async Task<List<GraphPoint>> GenerateChartPointsAsync(DateTime searchDate)
    {
        List < GraphPoint > points = new List<GraphPoint >();
        GraphPoint p1 = new GraphPoint();
        p1.Title = "Positive";
        p1.Value = 20;
        p1.Color = "#21c2b3";
        points.Add(p1);

        p1 = new GraphPoint();
        p1.Title = "Negative";
        p1.Value = 10;
        p1.Color = "#ffba6d";
        points.Add(p1);

        p1 = new GraphPoint();
        p1.Title = "Neutral";
        p1.Value = 30;
        p1.Color = "#d4d7df";
        points.Add(p1);
        return points;
        try
        {
            using (var client = new HttpClient())
            {
                string endpoint = "Customer/GenerateGraphPointsForSpecificMonth";
                string url = $"{App.ApiBaseUrl}{endpoint}";


                url += $"?date={searchDate.ToString("yyyy-MM-dd")}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("accept", "text/plain");
                request.Headers.Add("Authorization", "Bearer " + App.AccessToken);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var responseResult = JsonConvert.DeserializeObject<ResultViewModel<List<GraphPoint>>>(content);

                    if (responseResult.Success && responseResult.Data != null)
                    {
                        return responseResult.Data;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching chart data: {ex.Message}");
        }

        return null;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
      //  await LoadChartAsync(DateTime.Now);
      //  viewModel.LoadChart();
    }


    protected override bool OnBackButtonPressed()
    {
        ((AppShell)App.Current.MainPage).SwitchToHome();
        return true;
    }



    

  
}





