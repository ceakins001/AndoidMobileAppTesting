using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media.Session;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AndroidTest.Droid
{
    public static class AndroidGlobals
    {
        
        public static Activity MainActivity = null;
        public static Keycode PrevAudioControllerCommand;

        public static MediaSession mediaSession = null;

        public static EditText EditorLastTouched = null;
        public static int CursorPosOfEditorLastTouched = -1;

        public static DateTime ConvertUnixTimeStampIntoDateTimeLocal(long timeStamp)
        {
            DateTime offset = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime localDateTime = offset.AddMilliseconds(timeStamp).ToLocalTime();

            return localDateTime;
        }

        public static DateTime ConvertUnixTimeStampIntoDateTime(long timeStamp)
        {
            DateTime offset = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime thisDt = offset.AddMilliseconds(timeStamp);

            return thisDt;
        }

        public static long GetUnixTimeStamp()
        {
            long unixTimestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

            return unixTimestamp;
        }

        public static long GetUnixTimeStampFromDateTime(DateTime dt)
        {
            long unixTimestamp = (long)(dt.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

            return unixTimestamp;
        }
    }
}