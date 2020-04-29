using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AndroidTest.Droid.CustomRendering;
using AndroidTest.Model;

[assembly: ExportRenderer(typeof(MessagingListView), typeof(MessagingListViewRenderer))]
namespace AndroidTest.Droid.CustomRendering
{

    public class MessagingListViewRenderer : ListViewRenderer
    {
        Context _context;

        public MessagingListViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // unsubscribe
                Control.ItemClick -= OnItemClick;
            }

            if (e.NewElement != null)
            {
                // subscribe
                // THIS NativeAndroidListViewAdapter needs to be created by me (need to rename as well) see https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/custom-renderer/listview
                //Control.Adapter = new NativeAndroidListViewAdapter(_context as Android.App.Activity, e.NewElement as MessagingListView);
                Control.ItemClick += OnItemClick;
            }
        }

        void OnItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            ((MessagingListView)Element).NotifyItemSelected(((MessagingListView)Element).Items.ToList()[e.Position - 1], e.Position);
        }
    }
    
}