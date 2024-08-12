using System.ComponentModel;

namespace TrendWatch.App.ViewModels
{
   public class ResultViewModel<T>
    {

        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;

       // public Category[]? Data { get; set; }
        public T? Data { get; set; }
    }

public class Category : INotifyPropertyChanged
    {
        public string Name { get; set; }
       
        public string CatIcon { get; set; }
        public string Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public bool? IsDeleted { get; set; }
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class Merchant
    {
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Url { get; set; }
        public string Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public bool? IsDeleted { get; set; }
      

    }

    public class GraphPoint
    {
        public string Title { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Color { get; set; }

        public long? TotalSales { get; set; }
        public long? ReceiptCount { get; set; }

    }

    public class User
    {
        public string UserName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; }


    }

    public class CustomerRegister
    {
        public string MobileNumber { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;



    }


    public class AddManualExpenseViewModel
    {
        public decimal Amount { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string TransactionReference { get; set; } = string.Empty;

        public DateTime ExpanseDate { get; set; } = DateTime.Now;

        public Guid? CatId { get; set; }


    }


    public class Notification
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReadOn { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsRead { get; set; } = false;

        //public string CustomerId { get; set; }
       

    }


    public class ExpensesPerCategory
    {
        public Guid CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryIconURL { get; set; }
        public int TransactionCount { get; set; }
        public decimal TransactionAmount { get; set; }
        public double TransactionPercentage { get; set; }

    }

    public class ExpensesPerMerchant
    {
        public Guid MerchantID { get; set; }
        public string MerchantName { get; set; }
        public string MerchantLogoURL { get; set; }
        public int TransactionCount { get; set; }
        public decimal TransactionAmount { get; set; }
        public double TransactionPercentage { get; set; }
    }
}
