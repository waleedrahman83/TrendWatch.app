//using Android.Media.TV;
using TrendWatch.App.DataModels;
using TrendWatch.App.ViewModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;
using ZXing;
using TrendWatch.App.ViewModels;
using System.Security.Cryptography.X509Certificates;
using TrendWatch.App.Controls;
using CommunityToolkit.Maui.Views;

namespace TrendWatch.App.Views;

public partial class AddManualExpense : ContentPage
{
    private AddExpenseViewModel viewModel;
   
    public AddManualExpense()
	{
        MessagingCenter.Subscribe<AddExpenseViewModel>(this, "NoInternet", async (sender) =>
        {
            var popup = new NotificationPopup("No internet connection!", "Please check your network connection", "disconnect.svg", Color.FromRgb(242, 63, 47), true, "Dismiss", "TryAgainMessageCenter");// to the internet.\n Please check your connection and try again.");
            await this.ShowPopupAsync(popup);
        });
        InitializeComponent();
        viewModel = new AddExpenseViewModel();
        BindingContext = viewModel;
    }
   



    private async void btnAddExpense_Clicked(object sender, EventArgs e)
    {
        viewModel.IsLoading = true;
        var current = Connectivity.NetworkAccess;

        if (current == NetworkAccess.Internet)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, App.ApiBaseUrl + "Customer/AddInvoice");
                Guid categorId = Guid.TryParse(viewModel.SelectedCategory?.Id, out Guid parsedMCategoryId) ? parsedMCategoryId : Guid.Empty;
                var validationResult = ValidateInput();
                if(validationResult != "")
                {
                    var popup = new NotificationPopup("Invalid imput!", validationResult, "error.svg", Color.FromRgb(242, 63, 47), false, "Close", "");// to the internet.\n Please check your connection and try again.");
                    await this.ShowPopupAsync(popup);
                    viewModel.IsLoading = false;

                    return;
                }
                // Create an instance of your data class
                AddManualExpenseViewModel expanse = new AddManualExpenseViewModel
                {
                    Amount = decimal.Parse(entryAmount.Text),
                    //StoreName = entryPassword.Text,
                    StoreName = searchBox.Text,
                    TransactionReference = entryBillRef.Text,
                    ExpanseDate = endDatePicker.Date,
                    CatId = categorId
                    // CatId = Guid.NewGuid() 
                };

                

                // Serialize your data object to JSON
                string jsonContent = JsonConvert.SerializeObject(expanse);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                request.Headers.Add("accept", "text/plain");
                request.Headers.Add("Authorization", "Bearer " + App.AccessToken);


                // Send the request
                var response = await client.SendAsync(request);
                var resultContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Assume response is a JSON of your Response<bool> object
                    var result = JsonConvert.DeserializeObject<ResultViewModel<bool>>(resultContent);
                    if (result != null && result.Success)
                    {
                        ClearForm();
                        MessagingCenter.Send(this, "ExpenseAdded");

                        var popup = new NotificationPopup("Expense Added Successfully!", "Your expense has been added. Check the 'Insights' section to manage your records!", "success.svg", Color.FromRgb(9, 194, 29), false, "Close", "");// to the internet.\n Please check your connection and try again.");
                        await this.ShowPopupAsync(popup);
                      //  await DisplayAlert("Invoice added successfully","", "OK");
                    }
                    else
                    {
                        var popup = new NotificationPopup("Failed to add Expense!", "An error has been occurred while adding expense", "error.svg", Color.FromRgb(242, 63, 47), false, "Close", "");// to the internet.\n Please check your connection and try again.");
                        await this.ShowPopupAsync(popup);
                        
                    }
                }
                else
                {
                    var popup = new NotificationPopup("Failed to add Expense!", "An error has been occurred  while adding expense", "error.svg", Color.FromRgb(242, 63, 47), false, "Close", "");// to the internet.\n Please check your connection and try again.");
                    await this.ShowPopupAsync(popup);
                }
                viewModel.IsLoading = false;

            }
            catch (Exception ex)
            {

                var popup = new NotificationPopup("Failed to add Expense!", "An error has been occurred  while adding expense", "error.svg", Color.FromRgb(242, 63, 47), false, "Close", "");// to the internet.\n Please check your connection and try again.");
                await this.ShowPopupAsync(popup);
            }
        }
        else
        {
            var popup = new NotificationPopup("No internet connection!", "Please check your network connection", "disconnect.svg", Color.FromRgb(242, 63, 47), true, "Dismiss", "TryAgainMessageCenter");// to the internet.\n Please check your connection and try again.");
            await this.ShowPopupAsync(popup);
            viewModel.IsLoading = false;
        }

    }
    private string ValidateInput()
    {
        if (!decimal.TryParse(entryAmount.Text, out decimal amount))
            return "Please enter a valid amount";
       if(string.IsNullOrEmpty(searchBox.Text))
            return "Please enter or select a store";
        if (string.IsNullOrEmpty(entryBillRef.Text))
            return "Please enter bill reference number";
      if(viewModel.SelectedCategory == null)
            return "Please enter select Category";
        return "";
    }
    private void ClearForm()
    {
        entryAmount.Focus();
        entryAmount.Text = string.Empty;
        searchBox.Text = string.Empty;
        viewModel.SelectedCategory = null;
        viewModel.SelectedMerchant = null;
        entryBillRef.Text = string.Empty;
        entryToDate.Text = string.Empty;
        viewModel.DisplayMerchantList = false;
    }
    public ObservableCollection<Category> Categories { get; private set; } = new ObservableCollection<Category>();


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
        }
    }

    private void HandleSelectionChanged()
    {
        
    }

    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();

    }
    protected override bool OnBackButtonPressed()
    {
        Navigation.PopModalAsync();

        return true;
    }

    private void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedMerchant = (e.CurrentSelection[0] as TrendWatch.App.ViewModels.Merchant);
        viewModel.SelectedMerchant = selectedMerchant;

        viewModel.Filter = selectedMerchant.Name;
        // cvInvoices.SelectedItem = null;
        viewModel.DisplayMerchantList = false;
      


    }

    private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        viewModel.DisplayMerchantList = true;

    }
}