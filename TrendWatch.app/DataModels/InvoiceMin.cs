


using System.ComponentModel.DataAnnotations;

namespace TrendWatch.App.DataModels
{
    public class InvoiceMin
    {
        public Guid ID { get; set; }
        public DateTime InvoiceDate { get; set; }

        public string transactionReference { get; set; }

        public decimal total { get; set; }

        public int NoOfItems { get; set; }
        public string Store { get; set; }
        public string MerchantLogo { get; set; }





    }
}
