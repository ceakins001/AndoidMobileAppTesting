
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Util;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidTest.Interfaces;

using System;
using Xamarin.Forms;
using Android.Content.Res;
using AndroidTest.Droid;
using System.IO;
using AndroidTest.Model;
using Android.Support.V4.Content;

[assembly: Dependency(typeof(FileSystemProvider))]
namespace AndroidTest.Droid
{
    public class FileSystemProvider: IFileSystem
    {
        public void OpenDirectoryFolder(string dirPath)
        {
            Java.IO.File file = new Java.IO.File(dirPath);
            
            
            Intent intent = new Intent(Intent.ActionView);
            //Android.Net.Uri mydir = Android.Net.Uri.Parse("file://" + dirPath);
            Android.Net.Uri apkURI = FileProvider.GetUriForFile(
                             AndroidGlobals.MainActivity.ApplicationContext,
                             AndroidGlobals.MainActivity.ApplicationContext.PackageName + ".fileprovider", file);

            
            if (null != apkURI)
            {
                intent.SetDataAndType(apkURI, "*/*");    // or use */*
                intent.AddFlags(ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                intent.AddFlags(ActivityFlags.GrantWriteUriPermission);
                AndroidGlobals.MainActivity.StartActivity(intent);
            }
        }
    }

   

}