using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendWatch.App.DataModels
{
   
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Customer
    {
        public string name { get; set; }
        public string mobile { get; set; }
        public string id { get; set; }
    }

    public class Invoice
    {
        public string customerID { get; set; }
        public string marchantID { get; set; }
        public Guid? categoryID { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime invoiceDate { get; set; }
        public decimal total { get; set; }
        public string description { get; set; }
        public int noOfItems { get; set; }
        public Guid? createdBy { get; set; }
        public string customerName { get; set; }
        public string customerMobile { get; set; }
        public string store { get; set; }
        public string merchantLogo { get; set; }
        public string transactionReference { get; set; }
        public decimal? subtotal { get; set; }
        public decimal? discount { get; set; }
        public decimal? vat { get; set; }
        public decimal? grandTotal { get; set; }
        public string servedBy { get; set; }
        public List<InvoiceItem> invoiceItems { get; set; }
        public Customer customer { get; set; }
        public Merchant merchant { get; set; }
        public object category { get; set; }
        public object cashier { get; set; }
        public Branch branch { get; set; }
        public string id { get; set; }
        public int Rating { get; set; } 
        public DateTime? RatingDate { get; set; }
        public string PaymentMethod { get; set; }
    }
public class Branch 
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string Lng { get; set; }
    public string Lat { get; set; }
    
    public string DeliveryNumber { get; set; }


}
public class InvoiceItem
    {
        public string invoiceID { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public decimal? quantity { get; set; }
        public decimal? price { get; set; }
        public decimal? totalItemPrice { get; set; }
        public object createdOn { get; set; }
        public object createdBy { get; set; }
        public string id { get; set; }
    }

    public class Merchant
    {
        public string categoryID { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
        public string url { get; set; }
        public string desclimar { get; set; }
        public object category { get; set; }
        public int currentInvoicePurchased { get; set; }
        public int currentInvoiceConsumed { get; set; }
        public int currentInvoiceAvailable { get; set; }
        public object history { get; set; }
        public object branches { get; set; }
        public string id { get; set; }
        public string Solgan { get; set; }
        public string FacebookLink { get; set; }
        public string InstagramLink { get; set; }

        public string TwitterLink { get; set; }
        public string WhatsappLink { get; set; }
    }

    public class InvoiceDetailsReponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Invoice data { get; set; }
    }
    public class ArticleDetailsReponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Article data { get; set; }
    }

}
