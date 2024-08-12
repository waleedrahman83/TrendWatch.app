using TrendWatch.App.DataModels;
using TrendWatch.App.ViewModels;
using TrendWatch.App.Views;
using FFImageLoading.Maui;
//using FFImageLoading.Maui;
using System.ComponentModel;

namespace TrendWatch.App;
[XamlCompilation(XamlCompilationOptions.Skip)]
public partial class InvoiceHistoryPage : ContentPage
{
    // FilterPage sheet = new FilterPage();

    private ContentView _bottomSheet;
    private InvoiceHistoryViewModel viewModel;
    public InvoiceHistoryPage()
    {
        try
        {
            InitializeComponent();

            viewModel = new InvoiceHistoryViewModel();

            BindingContext = viewModel;

            ReloadCustomerInvoices();

            // collectionView.Scrolled += CollectionView_Scrolled;

            _bottomSheet = new ContentView
            {
                IsVisible = false,
                // BackgroundColor = Color.,
            };

        }
        catch (Exception ex) {

        }
    }

    private async void OnThresholdReached(object sender, EventArgs e)
    {
        return;
        if (!viewModel.IsLoading && viewModel.HasMoreInvoices)
        {
            viewModel.IsLoading = true;
            await viewModel.LoadMoreInvoices();
            viewModel.IsLoading = false;
        }
    }


    protected override bool OnBackButtonPressed()
    {
        ((AppShell)App.Current.MainPage).SwitchToHome();
        return true;
    }
    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentInvoiceId = (e.CurrentSelection[0] as InvoiceMin).ID;
        // cvInvoices.SelectedItem = null;

        await Navigation.PushModalAsync(new InvoiceDetailsPageV2(currentInvoiceId, false), false);
    }

    private void FilterButton_Tapped(object sender, TappedEventArgs e)
    {
    }

    bool isFilterVisible = false;




    void OnEntryTapped(object sender, EventArgs args)
    {
        isFilterVisible = !isFilterVisible;

    }



    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        //BindingContext = new InvoiceHistoryViewModel();

    }

    //private void OpenBottomSheet(object sender, FocusEventArgs e)
    //{
    //    double screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
    //    double desiredHeight = screenHeight * 0.75;

    //    // _bottomSheet.IsVisible = true;
    //    var Page = new BottomSheet();
    //    Page.HeightRequest = desiredHeight;
    //    //Page.Shown=true; 
    //    //Page.ismo=true;

    //    Page.ShowAsync(Window);
    //}
    BottomSheet filterBottomSheet = new BottomSheet();
    private bool bottomSheetOpened = false;

    private void OpenBottomSheet(object sender, EventArgs e)
    {
        double screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        double desiredHeight = screenHeight * 0.75;

        // _bottomSheet.IsVisible = true;

        filterBottomSheet.HeightRequest = desiredHeight;

        filterBottomSheet.ShowAsync(Window);
        
        bottomSheetOpened = true; // Set the flag to indicate that the bottom sheet has been opened
        MessagingCenter.Subscribe<BottomSheet>(this, "ApplyFilters_Clicked", (sender) =>
        {
            ReloadCustomerInvoices();
        });
    }


    private async void FilterSearch(object sender, EventArgs e)
    {

        var searchResult = await ReloadCustomerInvoices();
    }
    private async Task<bool> ReloadCustomerInvoices()
    {
        try
        {
            Guid merchantId = Guid.TryParse(filterBottomSheet.ViewModel.SelectedMerchant?.Id, out Guid parsedMerchantId) ? parsedMerchantId : Guid.Empty;
            Guid categoryId = Guid.TryParse(filterBottomSheet.ViewModel.SelectedCategory?.Id, out Guid parsedMCategoryId) ? parsedMCategoryId : Guid.Empty;
            viewModel.filter_merchantId = merchantId;
            viewModel.filter_categoryId = categoryId;
            viewModel.filter_StartDate = filterBottomSheet.ViewModel.StartDate;
            viewModel.filter_EndDate = filterBottomSheet.ViewModel.EndDate;
            viewModel.filter_MinAmount = filterBottomSheet.ViewModel.MinAmount;
            viewModel.filter_MaxAmount = filterBottomSheet.ViewModel.MaxAmount;
            viewModel.searchQuery = entrySearchQuery.Text;

            await viewModel.SearchCustomerInvoices();
            return true;
        }
        catch (Exception ex)
        {
            return false;

        }
        //OnPropertyChanged(nameof(viewModel.Invoices));
        //OnPropertyChanged(nameof(viewModel.InvoicesGrouped));
    }
    private void OnImageLoadingError(object sender, CachedImageEvents.ErrorEventArgs e)
    {
        Console.WriteLine($"Error loading image: {e.Exception}");
    }
   
}

