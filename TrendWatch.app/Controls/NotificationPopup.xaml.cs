using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Views;

namespace TrendWatch.App.Controls;

public partial class NotificationPopup : Popup
{
    string tryAgainMessageCenter = "";
    public string DialogTitle { get; set; }
    public string DialogMessage { get; set; }
    public string Icon { get; set; }

    public Color IconColor { get; set; }

    public bool CanTry { get; set; }
    public string CloseButtonText { get; set; }




    public NotificationPopup(string title,string message,string icon,Color iconColor,bool canTry,string closeButtonText,string tryAgainMessage)
	{
        DialogTitle = title;
        DialogMessage = message;
        Icon = icon;
        CanTry = canTry;
        //IconColor = iconColor;
        IconColor = Colors.Red;
        CloseButtonText = closeButtonText;
        tryAgainMessageCenter = tryAgainMessage;
        BindingContext = this;
        
        InitializeComponent();
       
            this.imgIcon.Behaviors.Add(new IconTintColorBehavior()
            {
                TintColor = iconColor
            });
        

    }

    void OnCloseButtonClicked(object sender, EventArgs e)
    {
        Close();
    }
    void OnTryAgainClicked(object sender, EventArgs e)
    {
        MessagingCenter.Send(this, "Home_TryAgain");
        Close();
    }
}