using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace AndroidTest.Droid
{
    class LinkerPleaseInclude
    {
        static bool falseflag = false;
        static LinkerPleaseInclude()
        {
            if (falseflag)
            {
                var ignore = new FitWindowsFrameLayout(AndroidGlobals.MainActivity);
            }
        }
    }
}