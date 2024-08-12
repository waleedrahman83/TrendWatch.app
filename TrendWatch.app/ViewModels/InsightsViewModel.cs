
using CommunityToolkit.Mvvm.ComponentModel;
using TrendWatch.App.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TrendWatch.App.ViewModels
{
    internal class InsightsViewModel : ObservableObject, INotifyPropertyChanged
    {
        public ObservableCollection<ExpensesPerCategory> CustomerExpensesPerCategory { get; set; } = new ObservableCollection<ExpensesPerCategory>();
        public ObservableCollection<ExpensesPerMerchant> CustomerExpensesPerMerchant { get; set; } = new ObservableCollection<ExpensesPerMerchant>();
        public ObservableCollection<Month> Months { get; set; } = new ObservableCollection<Month>();
        public ICommand OnMonthTappedCommand { get; private set; }
        public ICommand OnCategoryMerchantToggled { get; private set; }
        

        private Month _selectedMonth;

        public Month SelectedMonth
        {
            get => _selectedMonth;
            set => _selectedMonth = value;
        }
        private bool _isCategorySelected;

        public bool IsCategorySelected
        {
            get => _isCategorySelected;
            set => _isCategorySelected = value;
        }
        public async Task LoadCustomerExpensesPerCategory()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string endpoint = "Customer/GetCustomerCat"; // Adjust the endpoint according to your controller setup
                    string url = $"{App.ApiBaseUrl}{endpoint}";

                    // Include your authorization token if needed
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.AccessToken);

                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseResult = JsonConvert.DeserializeObject<ResultViewModel<List<ExpensesPerCategory>>>(content);

                        if (responseResult != null && responseResult.Success)
                        {
                            CustomerExpensesPerCategory.Clear();

                            foreach (ExpensesPerCategory inv in responseResult.Data)
                            {
                                CustomerExpensesPerCategory.Add(inv);
                            }
                            // Handle your response data here
                            // For example, you can update your UI with the received data
                            // responseResult.Data contains the list of ExpensesPerCategory objects
                            OnPropertyChanged(nameof(CustomerExpensesPerCategory)); // Notify UI to update

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
        public async Task LoadCustomerExpensesPerMerchant()
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
                            CustomerExpensesPerMerchant.Clear();

                            foreach (ExpensesPerMerchant inv in responseResult.Data)
                            {
                                CustomerExpensesPerMerchant.Add(inv);
                            }
                            // Handle your response data here
                            // For example, you can update your UI with the received data
                            // responseResult.Data contains the list of ExpensesPerCategory objects
                            OnPropertyChanged(nameof(CustomerExpensesPerMerchant)); // Notify UI to update

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
        public InsightsViewModel()
        {
            IsCategorySelected = true;
            OnMonthTappedCommand = new Command<Month>(OnMonthTapped);
            OnCategoryMerchantToggled = new Command<string>(OnCategoryMerchantTapped);
            


        }
        public async void Initialize()
        {
            var months = Enumerable.Range(1, 12)
    .Select(month => new Month
    {
        MonthName = new DateTime(DateTime.Now.Year, month, 1).ToString("MMM"),
        MonthValue = month,
        IsSelected = DateTime.Now.Month == month ? true : false
    })
    .ToList();
            foreach (var month in months)
            {
                Months.Add(month);
                if (month.IsSelected)
                    SelectedMonth = month;
            }
            await Task.Delay(2000);
            MessagingCenter.Send(this, "DefaultMonthChanged");
        }
        private void OnMonthTapped(Month item)
        {
            List< Month > months = new List<Month>();   
            foreach (var i in Months)
            {
                if (i != item)
                    i.IsSelected = false;
                else
                    i.IsSelected = true;
                months.Add(i);
            }
           // item.IsSelected = true;
            SelectedMonth = item;
           
            var tempMonths = Months;
            Months.Clear();
            Months = new ObservableCollection<Month> ( months );
            //foreach (var m in tempMonths)
            //{
            //    Months.Add(m);
            //}
            OnPropertyChanged(nameof(Months));

            MessagingCenter.Send(this, "MonthChanged");
        }
        private void OnCategoryMerchantTapped(string item)
        {
            IsCategorySelected = (item == "Category");
            OnPropertyChanged(nameof(IsCategorySelected));

        }
    }
}
