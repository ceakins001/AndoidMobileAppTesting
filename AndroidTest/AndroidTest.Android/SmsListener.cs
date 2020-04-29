using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Telephony;
using Android.Views;
using Android.Widget;

namespace AndroidTest.Droid
{
    // , "android.provider.Telephony.MMS_RECEIVED"
    // "android.provider.Telephony.WAP_PUSH_RECEIVED"

    [BroadcastReceiver(Enabled = true, Permission = "android.permission.BROADCAST_SMS")]
    [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED"  }, Priority = (int)IntentFilterPriority.HighPriority)]
    [IntentFilter(new[] { "android.provider.Telephony.SMS_DELIVER" })]
    [IntentFilter(new[] { "android.provider.Telephony.SMS_DELIVER_ACTION" })]
    public class SmsListener: BroadcastReceiver
    {
        
        //public static readonly string IntentAction = "android.provider.Telephony.SMS_RECEIVED";

        //public static readonly string INTENT_ACTION = "android.provider.Telephony.SMS_RECEIVED";

        public string message = "";
        public string address = "";

        public override void OnReceive(Context context, Intent intent)
        {

            // SMS_RECEIVED
            if (Telephony.Sms.Intents.SmsReceivedAction.Equals(intent.Action))
            {

                Model.SmsMessage newMsg = null;
                string message = string.Empty;
                string phoneKey = null;

                SmsMessage[] messages = Telephony.Sms.Intents.GetMessagesFromIntent(intent);

                SmsMessage prevMsg = null;

                if (messages.Length > 0)
                {
                    for(int i = 0; i < messages.Length; i++)
                    {
                        SmsMessage msg = messages[i];


                        if (null != prevMsg && msg.OriginatingAddress != prevMsg.OriginatingAddress)
                        {// send the curren previous set
                            if (null != MobileGlobals.NotifyNewSmsMessageArrivedCallback)
                            {
                                phoneKey = MobileGlobals.GeneratePhoneLookupKey(prevMsg.OriginatingAddress);
                                DateTime now = DateTime.Now;
                                newMsg = new Model.SmsMessage() { DateReceived = now, DateSent = now, ToFromPhoneAddress = prevMsg.OriginatingAddress, PhoneLookupKey = phoneKey, Message = message, ThreadId = -1 };
                                MobileGlobals.NotifyNewSmsMessageArrivedCallback(newMsg);
                            }

                            // reset the message
                            message = string.Empty;
                        }

                        message += msg.MessageBody;

                        if (messages.Length > 1 && i < messages.Length -1)
                        {
                            message += " ";
                        }

                        prevMsg = msg;
                    }
                    
                }

                if (null != MobileGlobals.NotifyNewSmsMessageArrivedCallback && !string.IsNullOrEmpty(message))
                {
                    phoneKey = MobileGlobals.GeneratePhoneLookupKey(prevMsg.OriginatingAddress);
                    DateTime now = DateTime.Now;
                    newMsg = new Model.SmsMessage() { DateReceived = now, DateSent = now, ToFromPhoneAddress = prevMsg.OriginatingAddress, PhoneLookupKey = phoneKey, Message = message, ThreadId = -1 };
                    MobileGlobals.NotifyNewSmsMessageArrivedCallback(newMsg);
                }
            }

            // SMS_DELIVER_ACTION
            if (Telephony.Sms.Intents.SmsDeliverAction.Equals(intent.Action))
            {
                // TODO
            }

            
        }

    }

    [BroadcastReceiver(Enabled = true, Permission = "android.permission.BROADCAST_WAP_PUSH")]
    [IntentFilter(new[] { "android.provider.Telephony.WAP_PUSH_RECEIVED" }, DataMimeType = "application/vnd.wap.mms-message")]
    [IntentFilter(new[] { "android.provider.Telephony.WAP_PUSH_DELIVER" }, DataMimeType = "application/vnd.wap.mms-message")]
    public class MmsListener : BroadcastReceiver
    {

        //public static readonly string IntentAction = "android.provider.Telephony.SMS_RECEIVED";

        //public static readonly string INTENT_ACTION = "android.provider.Telephony.SMS_RECEIVED";

        public string message = "";
        public string address = "";

        public override void OnReceive(Context context, Intent intent)
        {

            //MMS WAP_PUSH_RECEIVED
            if (Telephony.Sms.Intents.WapPushReceivedAction.Equals(intent.Action))
            {
                List<Model.SmsMessage> msgLst = null;

                // since we cannot directly read the MMS message here, we have to query the db and see if we can get the message out of the db
                if (null != MobileGlobals.SmsProviderInst)
                {
                    msgLst = MobileGlobals.SmsProviderInst.GetLastMmsMessage();
                }

                if (null != MobileGlobals.NotifyNewSmsMessageArrivedCallback && null != msgLst)
                {
                    foreach (Model.SmsMessage msg in msgLst)
                    {
                        MobileGlobals.NotifyNewSmsMessageArrivedCallback(msg);
                    }
                }
            }

            //MMS WAP_PUSH_DELIVER
            if (Telephony.Sms.Intents.WapPushDeliverAction.Equals(intent.Action))
            {
                // TODO
            }


        }

    }

    #region Intent Service for ACTION_RESPONSE_VIA_MESSAGE

    [Service(Label = "HeadlessSmsSendService", Permission = "android.permission.SEND_RESPOND_VIA_MESSAGE", Exported = true)]
    [IntentFilter(new[] { TelephonyManager.ActionRespondViaMessage}, Categories = new[] { "android.intent.category.DEFAULT" }, DataSchemes = new[] { "sms", "smsto", "mms", "mmsto" }, Priority = (int)IntentFilterPriority.HighPriority)]
    public class SmsSendService : Service
    {
        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override IBinder OnBind(Intent intent)
        {
            // TODO

            bool teststop = true;
            if (teststop) { }

            return null;
        }

        public override bool OnUnbind(Intent intent)
        {
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public string GetTimestamp()
        {
            // TODO

            return null;
        }

    }
    #endregion

    
}