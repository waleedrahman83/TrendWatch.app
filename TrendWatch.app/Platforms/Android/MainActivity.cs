using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace TrendWatch.App;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    SmsReceiver _smsReceiver;
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReceiveSms) != Permission.Granted ||
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadSms) != Permission.Granted)
        {
            ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReceiveSms, Manifest.Permission.ReadSms }, 1);
        }
       // Register the SMS receiver
        _smsReceiver = new SmsReceiver();
        RegisterReceiver(_smsReceiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnregisterReceiver(_smsReceiver);
    }
}
