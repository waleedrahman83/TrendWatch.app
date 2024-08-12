using Android.App;
using Android.Content;
using Android.OS;
using Android.Telephony;
using System;
using Xamarin.Essentials;



namespace TrendWatch.App
{

    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" })]
    public class SmsReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                if (intent.Action != "android.provider.Telephony.SMS_RECEIVED")
                    return;

                Bundle bundle = intent.Extras;
                if (bundle != null)
                {
                    var pdus = (Java.Lang.Object[])bundle.Get("pdus");
                    var msgs = new Android.Telephony.SmsMessage[pdus.Length];
                    for (int i = 0; i < msgs.Length; i++)
                    {
                        var bytes = (byte[])pdus[i];
                        msgs[i] = Android.Telephony.SmsMessage.CreateFromPdu(bytes);
                        string msgBody = msgs[i].MessageBody;
                        MessagingCenter.Send<object, string>(this, "OTPReceived", msgBody);

                        // Handle the OTP extraction here
                        // You can broadcast it to your MAUI app
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}


