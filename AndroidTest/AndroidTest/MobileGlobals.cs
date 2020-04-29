using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using AndroidTest.Interfaces;
using AndroidTest.Model;

namespace AndroidTest
{
    public delegate void NewMessageArrivedDelegate(Model.SmsMessage msg);
    public static class MobileGlobals
    {
        public const string ExceptionsLogName = "ExceptionsLog.txt";

        public static App XamlApp = null;

        public static bool JustInstalled = false;

        public static Action NotifyPauseResume = null;

        public static NewMessageArrivedDelegate NotifyNewSmsMessageArrivedCallback = null;

        public static string ApplicationDir = null;
        public static string AppDataDir = null;
        public static string RootDir = null;

        public static string SharedStorageDir = null;
        public static string CacheDirectory = null;
        public static string ExternalStorageDirForAppUninstalled = null;
        public static string AppDataPermanentStorageDir = null;
        public static string ExternalStorageDirRemovableMusic = null;
        public static string ExternalStorageDirRoot = null;

        public static string ImagesDirectory = null;

        public static bool DataPopulated = false;

        public static Dictionary<long, MessageThread> SmsMessageThreadsDict = null;
        public static IMessagingProvider SmsProviderInst = null;
        public static IFileSystem FileSystemProviderInst = null;
        public static IPhotoPickerService PhotoPickerProviderInst = null;

        public static List<string> ExceptionLog = new List<string>();


        /// <summary>
        /// Exceptions can be gathered without writing to the log file yet
        /// </summary>
        /// <param name="ex"></param>
        public static void AddNewException(Exception ex, string customMessage = "")
        {
            if (null != ex)
            {
                string entry = DateTime.Now.ToString() + "\t" + customMessage + "\t" + ex.Message + "\t" + ex.StackTrace;

                ExceptionLog.Add(entry);
            }
        }

        public static void WriteExceptionLog()
        {

            if (null != ExceptionLog && ExceptionLog.Count > 0)
            {
                if (!Directory.Exists(MobileGlobals.AppDataPermanentStorageDir))
                {
                    try
                    {
                        Directory.CreateDirectory(MobileGlobals.AppDataPermanentStorageDir);
                    }
                    catch { }

                    if (!Directory.Exists(MobileGlobals.AppDataPermanentStorageDir))
                    {
                        return;
                    }
                }

                string path = Path.Combine(MobileGlobals.AppDataPermanentStorageDir, ExceptionsLogName);
                StringBuilder sb = new StringBuilder(1000);

                if (null != MobileGlobals.ExceptionLog && MobileGlobals.ExceptionLog.Count > 0)
                {
                    foreach (string e in MobileGlobals.ExceptionLog)
                    {
                        sb.Append(e + "\r\n\r\n");
                    }
                }

                File.AppendAllText(path, sb.ToString());

                ExceptionLog.Clear();
            }
        }

        public static string GeneratePhoneLookupKey(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return string.Empty;
            }

            string phoneKey = NormalizePhone(phone);

            if (phoneKey.Length > 10)
            {
                phoneKey = phoneKey.Substring(1, phoneKey.Length - 1);
            }

            return phoneKey;
        }

        public static string NormalizePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return string.Empty;
            }

            return phone.Replace("+", "").Replace(" ", "").Replace(".", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace(";", "").Replace("#", "");
        }

    }
}
