using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using AndroidTest.Interfaces;
using AndroidTest.Model;
using AndroidTest.Model.SortAndCompare;


namespace AndroidTest
{
  
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private bool populatingControls = false;
        private bool busy;
        private PhoneContact selectedContact = null;

        public bool Busy
        {
            get { return busy; }
            set
            {
                if (busy == value)
                    return;

                busy = value;
            }
        }

        public bool IsRefreshing { get; set; } = false;

        public IList<MessageThread> MessageThreadsFound { get; private set; }
     
        public IList<Model.SmsMessage> ConversationMessages { get; private set; }

        private double currWidth = 0;
        private double currHeight = 0;

        bool firstSizeAllocated = false;

        public MainPage()
        {
            InitializeComponent();

            MobileGlobals.NotifyPauseResume = HandleAppPauseResume;
            MobileGlobals.NotifyNewSmsMessageArrivedCallback = HandleNewSmsMessageArrived;

            PopulateControls();
            SubscribeEvents();

            MobileGlobals.AppDataDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            if (MobileGlobals.JustInstalled)
            {// If this was just installed and there is a non-perm settings file still that didn't get removed with an uninstall, delete the settings file
                string appSetPath = Path.Combine(MobileGlobals.AppDataDir, MobileGlobals.AppSettingsFileName);
                if (File.Exists(appSetPath))
                {
                    try
                    {
                        File.Delete(appSetPath);
                    }
                    catch { }

                }

                MobileGlobals.JustInstalled = false;
            }

            try
            {

                MobileGlobals.SmsProviderInst = DependencyService.Get<IMessagingProvider>();
                MobileGlobals.FileSystemProviderInst = DependencyService.Get<IFileSystem>();
                MobileGlobals.PhotoPickerProviderInst = DependencyService.Get<IPhotoPickerService>();

            }
            catch (Exception ex)
            {
                MobileGlobals.AddNewException(ex);
                MobileGlobals.WriteExceptionLog();
            
            }

            currHeight = this.Height;
            currWidth = this.Width;


        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (!firstSizeAllocated)
            {
               // do anything we need to do the first time the display size is set

            }
            else
            {
                Device.BeginInvokeOnMainThread(() => { mainLayout.IsEnabled = true; });
            }
        }

        bool appPaused = true;
        private void HandleAppPauseResume()
        {
            // we want to avoid embarrassing messages playing out loud in the background
            appPaused = !appPaused;

        }

        private void MainLayout_Tapped(object sender, EventArgs e)
        {
           
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            if (firstSizeAllocated)
            {
               // DO something
            }
            else
            {
                // DO something
            }

            currWidth = width;
            currHeight = height;

            firstSizeAllocated = true;
            base.OnSizeAllocated(width, height); //must be called

        }


        private void PopulateControls()
        {
            populatingControls = true;

            // Populate any controls

            populatingControls = false;
        }

        private void SubscribeEvents()
        {
            // subscribe to any events


        }

        private void HandleNewSmsMessageArrived(Model.SmsMessage msg)
        {
            // handle the new message

        }

        private void btnSendMsg_Clicked(object sender, EventArgs e)
        {
         
            MessageThread conversationThreadFound = null;

            // look for a message thread having this person as the sole author
            if (null != MobileGlobals.SmsMessageThreadsDict && MobileGlobals.SmsMessageThreadsDict.Count > 0)
            {
                foreach (MessageThread t in MobileGlobals.SmsMessageThreadsDict.Values)
                {
                    if (null != t.Authors && t.Authors.Count == 1)
                    {
                        foreach (KeyValuePair<string, PhoneContact> auth in t.Authors)
                        {
                            if (null != auth.Value && auth.Value == selectedContact)
                            {
                                conversationThreadFound = t;
                                break;
                            }
                        }
                    }

                    if (null != conversationThreadFound)
                    {
                        break;
                    }
                }

                // if we found one, open that conversation
                if (null != conversationThreadFound)
                {
                    //ShowConversationThread(conversationThreadFound);
                    //textOpenedFromContacts = true;
                }
            }

            // if we didn't find one create a new message thread without a thread Id, 
            // once the first message is sent with that new thread without a thread id , we will get back the new threadid and then add that thread to the dictionary
            if (null == conversationThreadFound)
            {
                //ShowConversationThread(null, selectedContact);
                //textOpenedFromContacts = true;
            }
        }

        private void btnSendTextMessage_Clicked(object sender, EventArgs e)
        {
            SendTextMessage();
        }

        private MessageThread openConversationThread = null;
        private void SendTextMessage()
        {
            
        }

        private byte[] pduData = null;
        private async void btnPickImg_Clicked(object sender, EventArgs e)
        {
            

            if (null != MobileGlobals.PhotoPickerProviderInst)
            {
                PickedImage pi = await MobileGlobals.PhotoPickerProviderInst.GetImageStreamAsync();
                if (pi != null)
                {
                    if (null != MobileGlobals.SmsProviderInst)
                    {
                        pduData = MobileGlobals.SmsProviderInst.GetMMSPduData(txtAddress.Text, pi.FilePath, "This is a test message! Respond to let me know if you got it and whether you see the image.");

                        if (null != pduData)
                        {
                            MobileGlobals.SmsProviderInst.SendMMSPduData(pduData);
                        }
                        else
                        {// PDU shouldn't be null
                            throw new Exception("PDU Data is NULL!");
                        }   
                    }
                }
            }

            
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////CUSTOM ALERT DIALOG CODE//////////////////////////////////////////////////////////////////////////////////
        private void ShowCustomAlert(string message, string button1Text = "OK", string button2Text = "", Action<bool> callBackYesNo = null, bool whiteBackground = true)
        {
            customAlertCallbackYesNo = callBackYesNo;
            lblCustomAlertMessage.Text = message;
            btnAlertButton1.Text = button1Text;
            btnAlertButton2.Text = button2Text;

            if (string.IsNullOrEmpty(button2Text))
            {
                btnAlertButton2.IsVisible = false;
            }
            else
            {
                btnAlertButton2.IsVisible = true;
            }

            if (whiteBackground)
            {
                customAlertLayout.BackgroundColor = Color.White;
                lblCustomAlertMessage.TextColor = Color.Black;
            }
            else
            {
                customAlertLayout.BackgroundColor = Color.Black;
                lblCustomAlertMessage.TextColor = Color.White;
            }

            customAlertLayout.IsVisible = true;
            mainLayout.IsEnabled = false;

        }

        private Action<bool> customAlertCallbackYesNo = null;

        private void btnAlert1_Clicked(object sender, EventArgs e)
        {
            customAlertLayout.IsVisible = false;

            if (null != customAlertCallbackYesNo)
            {
                customAlertCallbackYesNo(true);
            }

            mainLayout.IsEnabled = true;
        }

        private void btnAlert2_Clicked(object sender, EventArgs e)
        {
            customAlertLayout.IsVisible = false;

            customAlertCallbackYesNo(false);

            mainLayout.IsEnabled = true;
        }

        private void txtAddress_Focused(object sender, FocusEventArgs e)
        {
            txtAddress.Text = string.Empty;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    }
}
