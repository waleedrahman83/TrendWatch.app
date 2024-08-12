namespace TrendWatch.App.Views;

public partial class StartPage : ContentPage
{
	public StartPage()
	{
		InitializeComponent();
	}

    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("/login");

    }

    private async void btnSignUp_Clicked(object sender, EventArgs e)
    {
        App.ForgetPasswordOrNot = false;
        await Shell.Current.GoToAsync("/AddPhoneNumber");

    }
    protected override bool OnBackButtonPressed()
    {
        Application.Current.Quit();
        return true;
    }
}