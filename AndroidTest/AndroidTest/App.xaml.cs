using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AndroidTest
{
    public partial class App : Application
    {
        public static Action MusicPausedCallback = null;
        public static Action ResumeMusicCallback = null;

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
            MobileGlobals.XamlApp = this;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
           
        }

        protected override void OnResume()
        {
            // Handle when your app resumes

            if (null != ResumeMusicCallback)
            {
                ResumeMusicCallback();
            }
        }
    }
}
