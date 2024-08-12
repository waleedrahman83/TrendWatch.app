using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace TrendWatch.App.ViewModels
{
    public class AddExpenseViewModel : ObservableObject, INotifyPropertyChanged
    {
        public ObservableCollection<Category> Categories { get; private set; } = new ObservableCollection<Category>();
        public ObservableCollection<Merchant> AllMerchants { get; private set; } = new ObservableCollection<Merchant>();

        private Merchant _selectedMerchant;
        public Merchant SelectedMerchant
        {
            get => _selectedMerchant;
            set => SetProperty(ref _selectedMerchant, value);
          
        }
        bool _isLoading = false;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }
        bool _isValid = false;

        public bool IsValid
        {
            get
            {

                return _isValid;
            }
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }
        public ObservableCollection<Merchant> FilteredMerchants { get; set; } = new ObservableCollection<Merchant>();

        private string _filter;
        public string Filter
        {
            get => _filter;
            set
            {
                if (SetProperty(ref _filter, value))
                    FilterMerchants();
            }
        }
        private bool _displayMerchantList;
        public bool DisplayMerchantList
        {
            get => _displayMerchantList;
            set => SetProperty(ref _displayMerchantList, value);

        }
        public ICommand OnCategoryTappedCommand { get; private set; }

        public AddExpenseViewModel()
        {
            OnCategoryTappedCommand = new Command<Category>(OnCategoryTapped);

            LoadCategoryData();
            LoadMerchantData();


        }
        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
            OnPropertyChanged(nameof(Categories));

            }
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
                MessagingCenter.Send(this, "NoInternet");

                // Handle no internet scenario, perhaps by notifying the user via the UI.
            }
        }

        public async Task<bool> LoadMerchantData()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string endpoint = "Merchant";
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
                            AllMerchants.Clear();
                           // AllMerchants.Add(new Merchant { Name = "All Stores" });
                            foreach (var merchant in resultViewModel.Data)
                            {
                                AllMerchants.Add(merchant);
                            }

                            if (AllMerchants.Any())
                            {
                                SelectedMerchant = AllMerchants.FirstOrDefault();
                                OnPropertyChanged(nameof(SelectedMerchant));
                            }

                            OnPropertyChanged(nameof(AllMerchants));
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

        private void FilterMerchants()
        {
            if (string.IsNullOrEmpty(_filter))
            {
                FilteredMerchants.Clear();
                foreach (var merchant in AllMerchants)
                    FilteredMerchants.Add(merchant);
            }
            else
            {
                var filtered = AllMerchants.Where(m => m.Name.ToLower().Contains(_filter.ToLower())).ToList();
                FilteredMerchants.Clear();
                foreach (var merchant in filtered)
                    FilteredMerchants.Add(merchant);
            }
        }
    }
}
