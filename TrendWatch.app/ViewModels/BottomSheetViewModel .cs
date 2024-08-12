

using CommunityToolkit.Mvvm.ComponentModel;
using TrendWatch.App.DataModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TrendWatch.App.ViewModels
{
    public class BottomSheetViewModel : ObservableObject, INotifyPropertyChanged
    {
        public ObservableCollection<Category> Categories { get; private set; } = new ObservableCollection<Category>();
        public ObservableCollection<Merchant> Merchants { get; private set; } = new ObservableCollection<Merchant>();
        public ObservableCollection<InvoiceMin> FilteredInvoices { get; private set; } = new ObservableCollection<InvoiceMin>();
        public ICommand OnCategoryTappedCommand { get; private set; }

        public ICommand LoadDataCommand { get; private set; }

        public ICommand ApplyFiltersCommand { get; private set; }

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        private decimal? _minAmount;
        public decimal? MinAmount
        {
            get => _minAmount;
            set => SetProperty(ref _minAmount, value);
        }

        private decimal? _maxAmount;
        public decimal? MaxAmount
        {
            get => _maxAmount;
            set => SetProperty(ref _maxAmount, value);
        }

        private Merchant _selectedMerchant;
        public Merchant SelectedMerchant
        {
            get => _selectedMerchant;
            set => SetProperty(ref _selectedMerchant, value);
        }

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set => _selectedCategory = value;
        }

        //public DateTime? StartDate { get; set; }
        //public DateTime? EndDate { get; set; }
        //public decimal? MinAmount { get; set; }
        //public decimal? MaxAmount { get; set; }
        //public Merchant SelectedMerchant { get; set; }
        //public Category SelectedCategory { get; set; }

        public BottomSheetViewModel()
        {
            OnCategoryTappedCommand = new Command<Category>(OnCategoryTapped);

            LoadCategoryData();
           LoadMerchantData();

            //LoadDataCommand = new Command(async () => await InitializeAsync());
            //ApplyFiltersCommand = new Command(async () => await ApplyFilters());
        }
        private void OnCategoryTapped(Category item)
        {
            foreach (var i in Categories)
            {
                i.IsSelected = false;
            }
            item.IsSelected = true;
            SelectedCategory = item;
            OnPropertyChanged(nameof(Categories));

        }
        private async Task InitializeAsync()
        {
            //await LoadCategoryData();
            await LoadMerchantData();
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        //public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}


        public async void LoadCategoryData()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        string endpoint = "Category";
                        string url = $"{App.ApiBaseUrl}{endpoint}";

                        Console.WriteLine($"Sending request to: {url}");

                        var response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();

                        var resultContent = await response.Content.ReadAsStringAsync();
                        var resultViewModel = JsonConvert.DeserializeObject<ResultViewModel<List<Category>>>(resultContent);

                        Categories.Clear();
                        foreach (var category in resultViewModel.Data)
                        {
                            Categories.Add(category);
                        }

                        OnPropertyChanged(nameof(Categories));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error has occurred: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("No Internet Connection");
                // Handle no internet scenario, perhaps by notifying the user via the UI.
            }
        }



        public async Task<bool> LoadMerchantData()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string endpoint = "Merchant/GetMerchantForUser";
                    string url = $"{App.ApiBaseUrl}{endpoint}";

                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Add("accept", "text/plain");
                    request.Headers.Add("Authorization", "Bearer " + App.AccessToken);

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var resultViewModel = JsonConvert.DeserializeObject<ResultViewModel<List<Merchant>>>(content);
                        if (resultViewModel.Success)
                        {
                            Merchants.Clear();
                            Merchants.Add(new Merchant { Name = "All Stores" });
                            foreach (var merchant in resultViewModel.Data)
                            {
                                Merchants.Add(merchant);
                            }

                            if (Merchants.Any())
                            {
                                SelectedMerchant = Merchants.FirstOrDefault();
                                OnPropertyChanged(nameof(SelectedMerchant));
                            }

                            OnPropertyChanged(nameof(Merchants));
                        }
                    }
                }
                return true;

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error has occurred: {ex.Message}");
                return false;

            }
        }


        public async Task<List<Grouped_list>> ApplyFilters()
        {
            Guid merchantId = Guid.TryParse(SelectedMerchant?.Id, out Guid parsedId) ? parsedId : Guid.Empty;
            Guid categoryId = Guid.TryParse(SelectedCategory?.Id, out Guid parsedId1) ? parsedId1 : Guid.Empty;

            try
            {
                using (var client = new HttpClient())
                {
                    string endpoint = $"Customer/invoices?pageIndex=1&pageSize=10&invoiceDateFrom={StartDate}&invoiceDateTo={EndDate}&amountFrom={MinAmount}&amountTo={MaxAmount}&merchantId={merchantId}&categoryId={categoryId}";
                    string url = $"{App.ApiBaseUrl}{endpoint}";

                    var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Headers.Add("accept", "text/plain");
                    request.Headers.Add("Authorization", "Bearer " + App.AccessToken);

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseResult = JsonConvert.DeserializeObject<InvoiceSearchResponse>(content);
                        if (responseResult.success)
                        {
                            FilteredInvoices.Clear();
                            List<Grouped_list> groupedInvoices = new List<Grouped_list>();
                            // Group invoices by day (assuming there is a Date property in InvoiceMin)
                            var groupedResults = responseResult.data.GroupBy(i => i.InvoiceDate.Date)
                                                    .Select(g => new Grouped_list(g.Key, g.ToList())).ToList();
                            foreach (var group in groupedResults)
                            {
                                groupedInvoices.Add(group);
                                foreach (var invoice in group)
                                    FilteredInvoices.Add(invoice);
                            }
                            OnPropertyChanged(nameof(FilteredInvoices));
                            return groupedInvoices;
                        }
                    }
                    return new List<Grouped_list>(); // Return an empty list if the request fails
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error has occurred: {ex.Message}");
                return new List<Grouped_list>(); // Return an empty list on exception
            }
        }


        //public async Task<List<InvoiceMin>> ApplyFilters()
        //{
        //    Guid merchantId = Guid.TryParse(SelectedMerchant?.Id, out Guid parsedId) ? parsedId : Guid.Empty;
        //    Guid categoryId = Guid.TryParse(SelectedCategory?.Id, out Guid parsedId1) ? parsedId1 : Guid.Empty;

        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            string endpoint = $"Customer/invoices?pageIndex=1&pageSize=10&invoiceDateFrom={StartDate}&invoiceDateTo={EndDate}&amountFrom={MinAmount}&amountTo={MaxAmount}&merchantId={merchantId}&categoryId={categoryId}";
        //            string url = $"{App.ApiBaseUrl}{endpoint}";

        //            var request = new HttpRequestMessage(HttpMethod.Post, url);
        //            request.Headers.Add("accept", "text/plain");
        //            request.Headers.Add("Authorization", "Bearer " + App.AccessToken);

        //            var response = await client.SendAsync(request);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                var content = await response.Content.ReadAsStringAsync();
        //                var responseResult = JsonConvert.DeserializeObject<InvoiceSearchResponse>(content);
        //                if (responseResult.success)
        //                {
        //                    FilteredInvoices.Clear();
        //                    foreach (var invoice in responseResult.data)
        //                    {
        //                        FilteredInvoices.Add(invoice);
        //                    }
        //                    OnPropertyChanged(nameof(FilteredInvoices));
        //                    return responseResult.data;
        //                }
        //            }
        //            return new List<InvoiceMin>(); // Return an empty list if the request fails
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error has occurred: {ex.Message}");
        //        return new List<InvoiceMin>(); // Return an empty list on exception
        //    }
        //}

    }
}
