

using CommunityToolkit.Mvvm.ComponentModel;
using TrendWatch.App.DataModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace TrendWatch.App.ViewModels
{
    public class InvoiceHistoryViewModel : ObservableObject, INotifyPropertyChanged
    {

        private int _currentPage = 1;

        private bool _isLoading = true;
        public Guid filter_merchantId = Guid.Empty;
        public Guid filter_categoryId = Guid.Empty;
        public DateTime? filter_StartDate;
        public DateTime? filter_EndDate;
        public decimal? filter_MinAmount;
        public decimal? filter_MaxAmount;
        public string searchQuery;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; }
        }
        bool _hasInvoices = false;
        public bool HasInvoices
        {
            get { return _hasInvoices; }
            set { SetProperty(ref _hasInvoices, value); }
        }
        int CurrentPageIndex = 1;
        int CurrentPagesize = 10;

        bool _hasMoreInvoices = true;
        public bool HasMoreInvoices
        {
            get { return _hasMoreInvoices; }
            set { SetProperty(ref _hasMoreInvoices, value); }
        }
        public ObservableCollection<InvoiceMin> Invoices { get; set; } = new ObservableCollection<InvoiceMin>();

        public ObservableCollection<Grouped_list> InvoicesGrouped { get; set; } = new ObservableCollection<Grouped_list>();
        //public ObservableCollection<List<Grouped_list> InvoiceList { get; set; } = new List<Grouped_list>();

        public InvoiceHistoryViewModel()
        {
          //  LoadInvoices();
        }
        private async void LoadInvoices()
        {
            Invoices.Clear();
            var invoices = await LoadCustomerInvoices(App.CustomerID);
            foreach (InvoiceMin inv in invoices)
            {
                Invoices.Add(inv);
            }
            // RaisePropertyChanged("Invoices");
            var dict = (Invoices.OrderByDescending(x => x.InvoiceDate)).GroupBy(o => o.InvoiceDate.Date).ToDictionary(g => g.Key, g => g.ToList());
            foreach (KeyValuePair<DateTime, List<InvoiceMin>> item in dict)
            {

                InvoicesGrouped.Add(new Grouped_list(item.Key, new List<InvoiceMin>(item.Value)));
            }
            RaisePropertyChanged("InvoicesGrouped");
            _isLoading = false;
            RaisePropertyChanged("IsLoading");
            _hasInvoices = Invoices.Count > 0;
            
            RaisePropertyChanged("HasInvoices");
        }
        async Task<List<InvoiceMin>> LoadCustomerInvoices(Guid customerId, int PageIndex = 1)
        {
            try
            {

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, App.ApiBaseUrl + "Customer/invoices?pageIndex=" + PageIndex + "&pageSize=10");
                    request.Headers.Add("accept", "text/plain");
                    request.Headers.Add("Authorization", "Bearer " + App.AccessToken);
                    var response = await client.SendAsync(request);




                    List<InvoiceMin> result = new List<InvoiceMin>();
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseResult = JsonConvert.DeserializeObject<InvoiceSearchResponse>(content);
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

        public async Task LoadCustomerInvoice(string query)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, App.ApiBaseUrl + "Customer/invoices?query=" + query);
                    request.Headers.Add("accept", "text/plain");
                    request.Headers.Add("Authorization", "Bearer " + App.AccessToken);
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseResult = JsonConvert.DeserializeObject<InvoiceSearchResponse>(content);
                        if (responseResult.success)
                        {
                            var result = responseResult.data ?? new List<InvoiceMin>(); // Ensure the data is not null

                            // Clear existing data
                            Invoices.Clear();
                            InvoicesGrouped.Clear();

                            foreach (InvoiceMin inv in result)
                            {
                                Invoices.Add(inv);
                            }

                            // Group by date and create grouped list
                            var dict = Invoices
                                .OrderByDescending(x => x.InvoiceDate)
                                .GroupBy(o => o.InvoiceDate.Date)
                                .ToDictionary(g => g.Key, g => g.ToList());

                            foreach (var item in dict)
                            {
                                InvoicesGrouped.Add(new Grouped_list(item.Key, new List<InvoiceMin>(item.Value)));
                            }

                            OnPropertyChanged(nameof(InvoicesGrouped)); // Notify UI to update
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception properly
                Debug.WriteLine("Error loading invoices: " + ex.Message);
            }
        }

        public async Task SearchCustomerInvoices()
        {
            

            try
            {
                using (var client = new HttpClient())
                {
                    string endpoint = $"Customer/invoices?pageIndex={CurrentPageIndex}&pageSize={CurrentPagesize}&invoiceDateFrom={filter_StartDate}&invoiceDateTo={filter_EndDate}&amountFrom={filter_MinAmount}&amountTo={filter_MaxAmount}&merchantId={filter_merchantId}&categoryId={filter_categoryId}&query={searchQuery}";
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
                            var result = responseResult.data ?? new List<InvoiceMin>(); // Ensure the data is not null

                            // Clear existing data
                            Invoices.Clear();
                            InvoicesGrouped.Clear();

                            foreach (InvoiceMin inv in result)
                            {
                                Invoices.Add(inv);
                            }

                            // Group by date and create grouped list
                            var dict = Invoices
                                .OrderByDescending(x => x.InvoiceDate)
                                .GroupBy(o => o.InvoiceDate.Date)
                                .ToDictionary(g => g.Key, g => g.ToList());

                            foreach (var item in dict)
                            {
                                InvoicesGrouped.Add(new Grouped_list(item.Key, new List<InvoiceMin>(item.Value)));
                            }

                            OnPropertyChanged(nameof(InvoicesGrouped)); // Notify UI to update
                          
                            RaisePropertyChanged("InvoicesGrouped");
                            _isLoading = false;
                            RaisePropertyChanged("IsLoading");
                            _hasInvoices = Invoices.Count > 0;

                            RaisePropertyChanged("HasInvoices");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error has occurred: {ex.Message}");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async Task LoadMoreInvoices()
        {
            if (!IsLoading || !HasMoreInvoices) 
                return;

            IsLoading = true;

            // Increment the page index for next request

            CurrentPageIndex = _currentPage + 1;
            Console.WriteLine($"LoadMoreInvoices: {CurrentPageIndex}");

            await SearchCustomerInvoices();

            if (Invoices.Any())
            {
                _currentPage = CurrentPageIndex;
            }
            else
            {
                HasMoreInvoices = false; // No more data to load
            }

            IsLoading = false;
        }



    }


}


