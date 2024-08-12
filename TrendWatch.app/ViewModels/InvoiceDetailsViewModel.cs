

using CommunityToolkit.Mvvm.ComponentModel;
using TrendWatch.App.DataModels;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Input;

namespace TrendWatch.App.ViewModels
{
    public class InvoiceDetailsViewModel : ObservableObject, INotifyPropertyChanged
    {
        private bool _fromScan;
        Invoice invoice;
        public ICommand OpenWebCommand { get; }

        public InvoiceDetailsViewModel(Guid invoiceId, bool fromScan)
        {
            try
            {
                _fromScan = fromScan;
                OpenWebCommand = new Command<string>(async (url) =>  await Launcher.OpenAsync(url));

                RaisePropertyChanged("FromScan");
                LoadInvoice(invoiceId);
            }
            catch
            {

            }
        }
        private async void LoadInvoice(Guid invoiceId)
        {
            invoice = await GetInvoiceDetails(invoiceId);
            //  NotifyPropertyChanged("CurrentHomeStats");
            RaisePropertyChanged("CurrentInvoice");


        }
        public Invoice CurrentInvoice
        {
            get { return invoice; }
            set { SetProperty(ref invoice, value); }
        }
        public bool FromScan
        {
            get { return _fromScan; }
            set { SetProperty(ref _fromScan, value); }
        }
        async Task<Invoice> GetInvoiceDetails(Guid invoiceId)
        {
            try
            {

                using (var client = new HttpClient())
                {

                    //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.AccessToken);



                    var request = new HttpRequestMessage(HttpMethod.Get, App.ApiBaseUrl + "Invioce/" + invoiceId);
                    request.Headers.Add("accept", "text/plain");
                    request.Headers.Add("Authorization", "Bearer " + App.AccessToken);
                    var response = await client.SendAsync(request);

                    Invoice result = new Invoice();
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseResult = JsonConvert.DeserializeObject<InvoiceDetailsReponse>(content);
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
        //async Task<Invoice> GetInvoiceDetails(Guid invoiceId)
        //{
        //    try
        //    {

        //        using (var client = new HttpClient())
        //        {

        //          //  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.AccessToken);



        //            var requestURI = App.ApiBaseUrl + "job/GetInvoiceDetails?invoiceId=" + invoiceId;
        //            var request = new HttpRequestMessage(HttpMethod.Get, requestURI);
        //            var response = await client.SendAsync(request);



        //            Invoice result = new Invoice();
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var content = await response.Content.ReadAsStringAsync();
        //                result = JsonConvert.DeserializeObject<Invoice>(content);
        //            }
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return null;
        //    }

        //}
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


}


