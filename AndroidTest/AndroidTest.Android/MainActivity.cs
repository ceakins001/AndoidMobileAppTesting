using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

using Android.Content;
using Android.Widget;
using Android.Speech;

using Xamarin.Forms;
using AndroidTest.Interfaces;

using AndroidTest.Droid;
using System.Collections.Generic;
using System.Linq;
using Android.Provider;
using AndroidTest.Model;
using AndroidTest.Model.SortAndCompare;
using System.IO;
using Android.Media;
using System.Text;
using static Android.OS.PowerManager;
using Android.Support.V4.Content;
using Android;
using Android.Support.V4.App;
using System.Threading;
using Android.Media.Session;
using Android.Util;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using System.Threading.Tasks;

namespace AndroidTest.Droid
{
    
    [Activity(Label = "Android Test", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, SupportsPictureInPicture = true)]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new[] { "android.intent.category.LAUNCHER" })]
    [IntentFilter(new[] { Intent.ActionSend, Intent.ActionSendto }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataSchemes = new[] { "sms", "smsto", "mms", "mmsto" })]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private volatile bool PermissionsGranted = false;

        private WakeLock wakeLock;
        private bool isRecording = false;
        private readonly int VOICE = 10;
        //DeviceBroadcastReceiver receiver;
        AudioManager audioMan = null;

        private SmsListener smsListener = new SmsListener();
        private MmsListener mmsListener = new MmsListener();

        private bool justInstalled = false;
        internal static MainActivity Instance { get; private set; }

        // Field, property, and method for Picture Picker
        public static readonly int PickImageId = 1000;

        public TaskCompletionSource<PickedImage> PickImageTaskCompletionSource { set; get; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            ///////////////////////////// DEFAULT APP/////////////////////////////////////////////////////////////////
            // Make sure is default messaging app
            string myPackageName = this.ApplicationContext.PackageName;
            if (!Telephony.Sms.GetDefaultSmsPackage(this).Equals(myPackageName))
            {
                Intent intent = new Intent(Telephony.Sms.Intents.ActionChangeDefault);
                intent.PutExtra(Telephony.Sms.Intents.ExtraPackageName, myPackageName);
                StartActivity(intent);
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////

            PermissionsGranted = CheckAppPermissions();

            /////////////////////////////////////////////////////////////////////////////////////////////////
            // All items requiring permissions and the actual app load should happen in FinishLoadApp() method
            if (PermissionsGranted)
            {
                FinishLoadApp();

                
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////

            // Do not add more code below here

        }

      
        private void FinishLoadApp()
        {
            
            try
            {

                MobileGlobals.ApplicationDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                AndroidGlobals.MainActivity = this;

                MobileGlobals.CacheDirectory = AndroidGlobals.MainActivity.CacheDir.AbsolutePath;
                MobileGlobals.RootDir = Android.OS.Environment.RootDirectory.Path;
                MobileGlobals.SharedStorageDir = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

                MobileGlobals.ExternalStorageDirRoot = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

            
                MobileGlobals.ExternalStorageDirRemovableMusic = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).AbsolutePath;
                MobileGlobals.ExternalStorageDirForAppUninstalled = AndroidGlobals.MainActivity.GetExternalFilesDir(null).AbsolutePath;
                MobileGlobals.AppDataPermanentStorageDir = MobileGlobals.ExternalStorageDirRoot + "/com.AndroidTest.Data";

                if (!Directory.Exists(MobileGlobals.AppDataPermanentStorageDir))
                {
                    try
                    {
                        Directory.CreateDirectory(MobileGlobals.AppDataPermanentStorageDir);
                    }
                    catch (Exception ex)
                    {
                        MobileGlobals.AddNewException(ex);
                        MobileGlobals.WriteExceptionLog();
                    }
                }

            }
            catch (Exception ex)
            {
                MobileGlobals.AddNewException(ex);
                MobileGlobals.WriteExceptionLog();
            }

            // Get a Wakelock
            try
            {
                PowerManager powerManager = (PowerManager)this.GetSystemService(Context.PowerService);
                wakeLock = powerManager.NewWakeLock(WakeLockFlags.Full, "My Lock");
            }
            catch (Exception ex)
            {

                MobileGlobals.AddNewException(ex);
                ex = new Exception("Error Getting PowerManager Wake Lock.");
                MobileGlobals.AddNewException(ex);
                MobileGlobals.WriteExceptionLog();
            }

            try
            {
                LoadApplication(new App());
            }
            catch (Exception ex)
            {

                MobileGlobals.AddNewException(ex);
                ex = new Exception("Error on LoadApplication() method.");
                MobileGlobals.AddNewException(ex);
                MobileGlobals.WriteExceptionLog();
            }
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            int permCount = 0;

            if (grantResults.Length > 0)
            {
                bool allGranted = true;
                foreach (Android.Content.PM.Permission perm in grantResults)
                {
                    if (perm != Permission.Granted)
                    {
                        allGranted = false;
                        break;
                    }

                    permCount++;
                }

                if (allGranted)
                {
                    FinishLoadApp();
                }
                else
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Permissions Required");
                    alert.SetMessage("[Some Message]");
                    alert.SetPositiveButton("OK", HandleNoPermissionAppCloseDialog);
                    alert.Show();

                }
            }


            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void HandleNoPermissionAppCloseDialog(object sender, DialogClickEventArgs e)
        {
            this.FinishAffinity();
            return;
        }

        private bool CheckAppPermissions()
        {
            bool allPermsGranted = true;

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessNetworkState) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Internet) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadCalendar) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteCalendar) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadContacts) != (int)Permission.Granted)
            {
                allPermsGranted = false;
                MobileGlobals.JustInstalled = true;
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteContacts) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadSms) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.SendSms) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReceiveSms) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }


            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteSms) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReceiveMms) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }

            //if (ContextCompat.CheckSelfPermission(this, Manifest.Permission_group.Sms) != (int)Permission.Granted)
            //{
            //    allPermsGranted = false;
            //}

            //if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteApnSettings) != (int)Permission.Granted)
            //{
            //    allPermsGranted = false;
            //}

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReceiveWapPush) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }

            // TODO NOTE - not sure if i need this one.. test
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.UseCredentials) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }

            //if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.BindVoiceInteraction) != (int)Permission.Granted)
            //{
            //    allPermsGranted = false;
            //}

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadPhoneNumbers) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WakeLock) != (int)Permission.Granted)
            {
                allPermsGranted = false;
            }

            //if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.SendRespondViaMessage) != (int)Permission.Granted)
            //{
            //    allPermsGranted = false;
            //}

            //if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.SetActivityWatcher) != (int)Permission.Granted)
            //{
            //    allPermsGranted = false;
            //}


            if (!allPermsGranted)
            {
                string[] permissions = new string[] {
                    Manifest.Permission.AccessNetworkState,
                    Manifest.Permission.Internet,
                    Manifest.Permission.ReadExternalStorage,
                    Manifest.Permission.ReadCalendar,
                    Manifest.Permission.WriteCalendar,
                    Manifest.Permission.ReadContacts,
                    Manifest.Permission.WriteContacts,
                    Manifest.Permission.WriteExternalStorage,
                    Manifest.Permission.WakeLock,
                    Manifest.Permission.ReceiveSms,
                    Manifest.Permission.ReadSms,
                    Manifest.Permission.WriteSms,
                    Manifest.Permission.SendSms,
                    Manifest.Permission.ReceiveMms,
                    Manifest.Permission.ReceiveWapPush
                    //Manifest.Permission.SendRespondViaMessage
                    //Manifest.Permission_group.Sms,
                    //Manifest.Permission.ReadPhoneNumbers,
                    //Manifest.Permission.SetActivityWatcher,
                    //Manifest.Permission.BindVoiceInteraction
                };

                ActivityCompat.RequestPermissions(this, permissions, 1);

                
            }

            return allPermsGranted;
        }

        protected override void OnResume()
        {
            base.OnResume();

            try
            {
                wakeLock.Acquire();
            }
            catch { }

            if (null != MobileGlobals.NotifyPauseResume)
            {
                MobileGlobals.NotifyPauseResume();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            try
            {
                wakeLock.Release();
            }
            catch { }

            if (null != MobileGlobals.NotifyPauseResume)
            {
                MobileGlobals.NotifyPauseResume();
            }

        }

        public override void OnPictureInPictureModeChanged(bool isInPictureInPictureMode, Configuration newConfig)
        {
            base.OnPictureInPictureModeChanged(isInPictureInPictureMode, newConfig);

            if (isInPictureInPictureMode)
            {
                // Hide the full-screen UI (controls, etc.) while in picture-in-picture mode.
                SetContentView(Resource.Layout.PIPLayout);
            }
            else
            {
                // Restore the full-screen UI.
            }
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();

            try
            {
                wakeLock.Release();
            }
            catch { }

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {

            if (requestCode == PickImageId)
            {
                if ((resultCode == Result.Ok) && (intent != null))
                {
                    Android.Net.Uri uri = intent.Data;
                    System.IO.Stream stream = ContentResolver.OpenInputStream(uri);

                    PickedImage pi = new PickedImage();
                    pi.DecodedPath = uri.Path;
                    pi.EncodedPath = uri.EncodedPath;
                    pi.DataStream = stream;
                    pi.FilePath = GetRealPathFromMediaStoreImageURI(uri);
                    
                    // Set the Stream as the completion of the Task
                    PickImageTaskCompletionSource.SetResult(pi);
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }
            }

            base.OnActivityResult(requestCode, resultCode, intent);
        }

        private string GetRealPathFromMediaStoreImageURI(Android.Net.Uri contentUri)
        {
            Context ctx = AndroidGlobals.MainActivity;

            try
            {
                if (string.IsNullOrEmpty(contentUri.LastPathSegment))
                {
                    return null;
                }
                
                string[] idParts = contentUri.LastPathSegment.Split(new char[] { ':' });

                if (idParts.Length < 2 )
                {
                    return null;
                }

                string id = idParts[1];

                Android.Net.Uri uri = MediaStore.Images.Media.ExternalContentUri;

                string selection = "_id=?";
                string[] selArgs = new string[] { id };

                var mediaStoreImagesMediaData = "_data";
                string[] projection = { mediaStoreImagesMediaData };
                
                Android.Database.ICursor cursor = this.ApplicationContext.ContentResolver.Query(uri, projection, selection, selArgs, null);
                
                if (null != cursor)
                {
                    cursor.MoveToFirst();

                    return cursor.GetString(0);
                }
            }
            catch(Exception ex)
            {
                MobileGlobals.AddNewException(ex);
                MobileGlobals.WriteExceptionLog();
            }

            return null;
        }

    }
}