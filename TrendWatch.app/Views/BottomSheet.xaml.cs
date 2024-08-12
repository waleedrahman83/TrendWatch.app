//using TrendWatch.App.DataModels;
//using TrendWatch.App.ViewModels;
//using Newtonsoft.Json;


//namespace TrendWatch.App.Views
//{
//    public partial class BottomSheet
//    {
//        private List<Category> categoryData;
//        private List<TrendWatch.App.ViewModels.Merchant> MerchantData;

//        public BottomSheet()
//        {
//            InitializeComponent();
//            InitializeAsync();
//            LoadMerchantData1();
//        }

//        private async void InitializeAsync()
//        {
//            categoryData = await LoadCategoryData1();
//            // LoadMerchantData1();
//        }

//        async Task<List<Category>> LoadCategoryData1()
//        {
//            var current = Connectivity.NetworkAccess;

//            if (current == NetworkAccess.Internet)
//            {
//                try
//                {
//                    var client = new HttpClient();

//                    string ipAddress = "10.0.2.2";
//                    string port = ":5136";
//                    string endpoint = "Category";

//                    string url = $"http://{App.ApiBaseUrl}{endpoint}";

//                    Console.WriteLine($"Sending request to: {url}");

//                    var response = await client.GetAsync(url);
//                    response.EnsureSuccessStatusCode();

//                    var resultContent = await response.Content.ReadAsStringAsync();
//                    var token = JsonConvert.DeserializeObject<ResultViewModel<List<Category>>>(resultContent);

//                    categoryListView.ItemsSource = token.Data.ToList();


//                    return token.Data.ToList();
//                    //ListView.ItemsSource = categoryData;
//                }
//                catch (Exception ex)
//                {
//                    string message = "Error has occurred: " + ex.Message;
//                    return new List<Category>();
//                }
//            }
//            else
//            {
//                string message = "No Internet Connection";
//                return new List<Category>();
//            }
//        }

//        async void LoadMerchantData1()
//        {
//            try
//            {

//                using (var client = new HttpClient())
//                {

//                    string ipAddress = "10.0.2.2";
//                    string port = ":5136";
//                    string endpoint = "Merchant/GetMerchantForUser";
//                    string url = $"http://{App.ApiBaseUrl}{endpoint}";

//                    var request = new HttpRequestMessage(HttpMethod.Get, url);
//                    request.Headers.Add("accept", "text/plain");
//                    request.Headers.Add("Authorization", "Bearer " + App.AccessToken);

//                    var response = await client.SendAsync(request);




//                    if (response.IsSuccessStatusCode)
//                    {
//                        var content = await response.Content.ReadAsStringAsync();
//                        var responseResult = JsonConvert.DeserializeObject<ResultViewModel<List<TrendWatch.App.ViewModels.Merchant>>>(content);
//                        if (responseResult.Success)
//                        {
//                            MerchantData = responseResult.Data.ToList();
//                            merchantPicker.ItemsSource = MerchantData;

//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }

//        }


//        private async Task<List<InvoiceMin>> ApplyFilters()
//        {
//            DateTime startDate = startDatePicker.Date;
//            DateTime endDate = endDatePicker.Date;
//            decimal minAmount = Convert.ToDecimal(minAmountEntry.Text);
//            decimal maxAmount = Convert.ToDecimal(maxAmountEntry.Text);

//            var selectedItem = merchantPicker.SelectedItem as TrendWatch.App.ViewModels.Merchant;

//            string storeName = selectedItem?.Name;
//            Guid merchantId = Guid.TryParse(selectedItem?.Id, out Guid MerchantId) ? MerchantId : Guid.Empty;

//            var selectedCategory = categoryListView.SelectedItem as Category;
//            Guid CategoryId = Guid.TryParse(selectedItem?.Id, out Guid CatId) ? CatId : Guid.Empty;


//            try
//            {

//                using (var client = new HttpClient())
//                {

//                    string ipAddress = "10.0.2.2";
//                    string port = ":5136";
//                    string endpoint = $"Customer/invoices?pageIndex=1&pageSize=10&invoiceDateFrom={startDate}&invoiceDateTo={endDate}&amountFrom={minAmount}&amountTo={maxAmount}&merchantId={MerchantId}&categoryId={CatId}";
//                    string url = $"http://{App.ApiBaseUrl}{endpoint}";

//                    var request = new HttpRequestMessage(HttpMethod.Post, url);

//                    request.Headers.Add("accept", "text/plain");
//                    request.Headers.Add("Authorization", "Bearer " + App.AccessToken);
//                    var response = await client.SendAsync(request);

//                    List<InvoiceMin> result = new List<InvoiceMin>();
//                    if (response.IsSuccessStatusCode)
//                    {
//                        var content = await response.Content.ReadAsStringAsync();
//                        var responseResult = JsonConvert.DeserializeObject<InvoiceSearchResponse>(content);
//                        if (responseResult.success)
//                        {
//                            result = responseResult.data;
//                        }
//                    }
//                    return result;
//                }
//            }
//            catch (Exception ex)
//            {

//                return null;
//            }


//        }

//        private async void ApplyFilters_Clicked(object sender, EventArgs e)
//        {
//            //applyFiltersButton.IsEnabled = false;

//            try
//            {
//                List<InvoiceMin> filteredInvoices = await ApplyFilters();

//                //applyFiltersButton.IsEnabled = true;
//            }
//            catch (Exception ex)
//            {
//                //applyFiltersButton.IsEnabled = true;

//            }
//        }

//    }
//}

using TrendWatch.App.ViewModels;
using System.ComponentModel;

namespace TrendWatch.App.Views
{
    public partial class BottomSheet : INotifyPropertyChanged
    {
        public BottomSheetViewModel ViewModel { get; private set; }

        public BottomSheet()
        {
            InitializeComponent();
            ViewModel = new BottomSheetViewModel();
            this.BindingContext = ViewModel;
            // ViewModel.LoadDataCommand.Execute(null);


           // this.Loaded += OnPageLoaded;
        }

        private async void OnPageLoaded(object sender, EventArgs e)
        {
            ViewModel.LoadDataCommand.Execute(null);
        }

        private async void ApplyFilters_Clicked(object sender, EventArgs e)
        {
            //applyFiltersButton.IsEnabled = false;
            
            try
            {
               // List<Grouped_list> InvoicesGrouped = await ViewModel.ApplyFilters();
                this.DismissAsync();
                MessagingCenter.Send(this, "ApplyFilters_Clicked");
                //applyFiltersButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                //applyFiltersButton.IsEnabled = true;

            }
        }
        private async void category_Tapped(object sender, TappedEventArgs e)
        {
            // await Shell.Current.GoToAsync("settings");

        }
        private void ClearAll_Clicked(object sender, EventArgs e)
        {
            ViewModel.SelectedMerchant = null; 
            ViewModel.StartDate = DateTime.Now;
            ViewModel.EndDate = DateTime.Now;
            ViewModel.MinAmount = null;
            ViewModel.MaxAmount = null;
            ViewModel.SelectedCategory = null;
            //ViewModel.LoadMerchantData();

            //ViewModel.OnPropertyChanged(nameof(ViewModel.SelectedMerchant));
            //ViewModel.OnPropertyChanged(nameof(ViewModel.StartDate));
            //ViewModel.OnPropertyChanged(nameof(ViewModel.EndDate));
            //ViewModel.OnPropertyChanged(nameof(ViewModel.MinAmount));
            //ViewModel.OnPropertyChanged(nameof(ViewModel.MaxAmount));
        }

    }
}
