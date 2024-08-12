using TrendWatch.App.Views;

namespace TrendWatch.App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute("loading", typeof(LoadingPage));
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("home", typeof(HomePage));
        Routing.RegisterRoute("profile", typeof(ProfilePage));
        Routing.RegisterRoute("start", typeof(StartPage));
        Routing.RegisterRoute("signup", typeof(SignUpPage));
        Routing.RegisterRoute("AddPhoneNumber", typeof(AddPhoneNumber));
        Routing.RegisterRoute("OTPpage", typeof(OTPpage));
        Routing.RegisterRoute("settings", typeof(SettingsPage));
        Routing.RegisterRoute("main", typeof(MainPage));

        Routing.RegisterRoute("notifcation", typeof(NotifcationPage));
        Routing.RegisterRoute("ForgetPassword", typeof(ForgetPassword));

    }
    
    public void SwitchtoTab(int tabIndex)
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            switch (tabIndex)
            {
                case 0: shelltabbar.CurrentItem = pageHome; break;
                //case 1: shelltabbar.CurrentItem = shelltab_1; break;
                //case 2: shelltabbar.CurrentItem = shelltab_2; break;
                //case 3: shelltabbar.CurrentItem = shelltab_3; break;
            };
        });
    }
    public void SwitchToHome()
    {
        Device.BeginInvokeOnMainThread(() =>
        {
         shelltabbar.CurrentItem = pageHome; 
                
        });
    }
    public void SwitchRadio()
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            shelltabbar.CurrentItem = pageRadio;

        });
    }
    public void SwitchToInsights()
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            shelltabbar.CurrentItem = pageInsights;

        });
    }
    //public void SwitchToHistory()
    //{
    //    Device.BeginInvokeOnMainThread(() =>
    //    {
    //        shelltabbar.CurrentItem = pageHistory;

    //    });
   // }
}
