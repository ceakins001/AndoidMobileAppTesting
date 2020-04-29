using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidTest.Droid;
using AndroidTest.Interfaces;
using AndroidTest.Model;
using Xamarin.Forms;
using Android.Support.V4.Content;
using System.IO;
using System.Threading.Tasks;

[assembly: Dependency(typeof(PhotoPickerService))]
namespace AndroidTest.Droid
{
    
    public class PhotoPickerService : IPhotoPickerService
    {
        public Task<PickedImage> GetImageStreamAsync()
        {
            // Define the Intent for getting images
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);

            // Start the picture-picker activity (resumes in MainActivity.cs)
            MainActivity.Instance.StartActivityForResult(
                Intent.CreateChooser(intent, "Select Picture"),
                MainActivity.PickImageId);

            // Save the TaskCompletionSource object as a MainActivity property
            MainActivity.Instance.PickImageTaskCompletionSource = new TaskCompletionSource<PickedImage>();

            // Return Task object
            return MainActivity.Instance.PickImageTaskCompletionSource.Task;
        }

        
    }
    
}