namespace TrendWatch.App.Views;

using TrendWatch.App.ViewModels;
using Microcharts;
using Newtonsoft.Json;
using SkiaSharp;

public partial class ExpensesPage : ContentPage
{

    private InsightsViewModel viewModel;

    public ExpensesPage()
    {
        InitializeComponent();
       
      
        viewModel = new InsightsViewModel();
        BindingContext = viewModel;

        MessagingCenter.Subscribe<InsightsViewModel>(this, "MonthChanged", (sender) =>
        {
            var selectedMonth = viewModel.SelectedMonth;
            collectionViewMonth.ScrollTo(selectedMonth, position: ScrollToPosition.MakeVisible, animate: true);
            var selectedDate = new DateTime(DateTime.Now.Year, selectedMonth.MonthValue, 1);

            LoadChartAsync(selectedDate);
        });
        MessagingCenter.Subscribe<InsightsViewModel>(this, "DefaultMonthChanged", (sender) =>
        {
            var selectedMonth = viewModel.SelectedMonth;
            collectionViewMonth.ScrollTo(selectedMonth, position: ScrollToPosition.MakeVisible, animate: true);

        });
        viewModel.Initialize();
        //collectionViewMonth.ScrollTo(8, position: ScrollToPosition.MakeVisible, animate: true);

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadChartAsync(DateTime.Now);
        await viewModel.LoadCustomerExpensesPerCategory();
        await viewModel.LoadCustomerExpensesPerMerchant();

        // GetCustomerMerchant();
    }
    private async Task LoadChartAsync(DateTime chartDate )
    {
        DateTime today = DateTime.Now;
        var graphPoints = await GenerateChartPointsAsync(chartDate);
        decimal totalSpendings = 0;
        foreach (var point in graphPoints)
        {
            totalSpendings = totalSpendings + point.Value;
        }
        lblSepndingValue.Text = totalSpendings.ToString() + " EGP";
        var entries = ConvertToChartEntries(graphPoints);

        if (entries.Any())
        {
            chartMain.Chart = new LineChart()
            {
                Entries = entries,
                IsAnimated = true,
                LabelTextSize = 26,
                LabelOrientation = Orientation.Horizontal,
                Typeface = SKTypeface.FromFamilyName("InterRegular"),
                //CornerRadius = 30,
                ShowYAxisLines = true,
                ShowYAxisText = true,
                SerieLabelTextSize = 20,
                ValueLabelTextSize = 40,
                Margin = 30,
                LineSize = 8

            };

            //  chartMain.HeightRequest = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density * 0.3;  // 30% من ارتفاع الشاشة
            // chartMain.WidthRequest = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density * 0.8;  // 80% من عرض الشاشة


        }
        else
        {
            Console.WriteLine("No data to display.");
        }
    }
    private async Task<List<GraphPoint>> GenerateChartPointsAsync(DateTime searchDate)
    {
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

    protected override bool OnBackButtonPressed()
    {
        ((AppShell)App.Current.MainPage).SwitchToHome();
        return true;
    }



    

    private async Task GetCustomerMerchant()
    {
        try
        {
            using (var client = new HttpClient())
            {
                string endpoint = "Customer/GetCustomerMerchant"; // Adjust the endpoint according to your controller setup
                string url = $"{App.ApiBaseUrl}{endpoint}";

                // Include your authorization token if needed
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.AccessToken);

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var responseResult = JsonConvert.DeserializeObject<ResultViewModel<List<ExpensesPerMerchant>>>(content);

                    if (responseResult != null && responseResult.Success)
                    {
                        // Handle your response data here
                        // For example, you can update your UI with the received data
                        // responseResult.Data contains the list of ExpensesPerCategory objects
                    }
                }
                else
                {
                    // Handle unsuccessful response
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Handle unauthorized access
                    }
                    else
                    {
                        // Handle other error scenarios
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching chart data: {ex.Message}");
        }
    }

   
    
}





