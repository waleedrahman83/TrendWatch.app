//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Text.Json.Serialization;
//using System.Threading.Tasks;

//namespace TrendWatch.App.DataModels
//{
//    public class InvoiceItem 
//    {
//        public Guid? InvoiceID { get; set; }

//        [StringLength(50)]
//        public string? ItemCode { get; set; }

//        [StringLength(200)]
//        public string? ItemName { get; set; }

//        public decimal? Quantity { get; set; }

//        public decimal? Price { get; set; }

//        public decimal? TotalItemPrice { get; set; }

//        public DateTime? CreatedOn { get; set; }
//        public string? CreatedBy { get; set; }

//        [JsonIgnore]
//        public Invoice? Invoice { get; set; }
//    }
//}
