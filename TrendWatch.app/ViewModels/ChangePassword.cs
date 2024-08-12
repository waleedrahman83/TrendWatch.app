namespace TrendWatch.App.ViewModels
{
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string MobileNumber { get; set; }
        public string NewPassword { get; set; }
    }
}
