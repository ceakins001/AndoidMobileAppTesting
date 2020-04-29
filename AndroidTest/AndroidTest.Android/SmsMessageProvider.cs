
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
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
using AndroidTest.Model.SortAndCompare;
using Android.Telephony;
using Android.Graphics;
using Java.Net;
using Com.Google.Mms.Pdu;
using System.Threading;

[assembly: Dependency(typeof(SmsMessageProvider))]
namespace AndroidTest.Droid
{
    public class SmsMessageProvider : IMessagingProvider
    {
        
        private Dictionary<long, List<Model.SmsMessage>> mmsPartsDict = new Dictionary<long, List<Model.SmsMessage>>(1000);

        public bool SendMMSPduData(byte[] pduData)
        {
            Context ctx = AndroidGlobals.MainActivity.ApplicationContext;
            SmsManager sm = SmsManager.Default;
            Random rnd = new Random();

            try
            {
                // write the pdu to a temp cache file
                string cacheFilePath = System.IO.Path.Combine(ctx.CacheDir.AbsolutePath, "send." + rnd.Next().ToString() + ".dat");
                File.WriteAllBytes(cacheFilePath, pduData);

                if (File.Exists(cacheFilePath))
                {

                    // create the contentUri
                    Android.Net.Uri contentUri = (new Android.Net.Uri.Builder())
                       .Authority(ctx.PackageName + ".fileprovider")
                       .Path(cacheFilePath)
                       .Scheme(ContentResolver.SchemeContent)
                       .Build();

                    PendingIntent pendingIntent = PendingIntent.GetBroadcast(ctx, 0, new Intent(ctx.PackageName + ".WAP_PUSH_DELIVER"), 0);

                    //sm.SendMultimediaMessage(ctx, contentUri, null, null, pendingIntent);
                    sm.SendMultimediaMessage(ctx, contentUri, null, null, null);

                }
            }
            catch (Exception ex)
            {
                MobileGlobals.AddNewException(ex);
                MobileGlobals.WriteExceptionLog();

                return false;
            }

            return true;
        }

        public byte[] GetMMSPduData(string phone, string imgFilePath, string txtMessage)
        {
            Context ctx = AndroidGlobals.MainActivity;

            byte[] pduData = null;

            try
            {
                if (!string.IsNullOrEmpty(phone) && (!string.IsNullOrEmpty(imgFilePath) || !string.IsNullOrEmpty(txtMessage)))
                {
                    
                    ///////////////////PDU Management Method///////////////////////////////////////////////////////

                    // First instantiate the PDU we will be turning into a byte array that
                    // will be ultimately sent to the MMS service provider.
                    // In this case, we are using a SendReq PDU type.
                    SendReq sendReqPdu = new SendReq();
                    // Set the recipient number of our MMS
                    sendReqPdu.AddTo(new EncodedStringValue(phone));
                    // Instantiate the body of our MMS
                    PduBody pduBody = new PduBody();

                    // Add any text message data
                    if (!string.IsNullOrEmpty(txtMessage))
                    {
                        PduPart txtPart = new PduPart();
                        txtPart.SetData(Encoding.ASCII.GetBytes(txtMessage));
                        txtPart.SetContentType(new EncodedStringValue("text/plain").GetTextString());
                        txtPart.SetName(new EncodedStringValue("Message").GetTextString());
                        pduBody.AddPart(txtPart);
                    }

                    // add any image data
                    if (!string.IsNullOrEmpty(imgFilePath))
                    {
                        PduPart imgPart = new PduPart();

                        byte[] sampleImageData = GetFileBytes(imgFilePath);

                        imgPart.SetData(sampleImageData);
                        imgPart.SetContentType(new EncodedStringValue("image/png").GetTextString());
                        imgPart.SetFilename(new EncodedStringValue(System.IO.Path.GetFileName(imgFilePath)).GetTextString());
                        pduBody.AddPart(imgPart);
                    }

                    // Set the body of our MMS
                    sendReqPdu.Body = pduBody;
                    // Finally, generate the byte array to send to the MMS provider
                    PduComposer composer = new PduComposer(sendReqPdu);
                    pduData = composer.Make();

                }
            }
            catch (Exception ex)
            {
                MobileGlobals.AddNewException(ex);
                MobileGlobals.WriteExceptionLog();

            }

            return pduData;
        }

        public byte[] GetFileBytes(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }

        public bool SendMessage(long threadId, string message, string phoneAddr)
        {
           
            try
            {
                if (!string.IsNullOrEmpty(phoneAddr))
                {
                    SmsManager sm = SmsManager.Default;
                    List<string> parts = new List<string>(4);

                    if (message.Length <= 160)
                    {
                        parts.Add(message);
                    }
                    else
                    {
                        parts = BreakMessageIntoParts(message, 140);
                    }

                    sm.SendMultipartTextMessage(phoneAddr, null, parts, null, null);

                }
            }
            catch(Exception ex)
            {
                MobileGlobals.AddNewException(ex);
                MobileGlobals.WriteExceptionLog();

                return false;

            }

            return true;
        }

        private List<string> BreakMessageIntoParts(string message, int maxPartLen)
        {
            if (null == message)
            {
                return null;
            }

            if (message.Length <= maxPartLen)
            {
                return new List<string>() { message };
            }

            // go to the 160th character and see if it is blank space or the end of a word
            List<string> parts = new List<string>(3);

            bool noMoreParts = false;
            int blockStart = 0;

            bool isBlankOrEnd = false;
            string charBuff = null;
            int thisPos = 0;
            string thisPart = null;

            while (!noMoreParts)
            {
                thisPart = string.Empty;

                if (blockStart > message.Length)
                {
                    noMoreParts = true;
                    break;
                }

                thisPos = blockStart + maxPartLen -1;

                if (thisPos <= message.Length - 1)
                {
                    charBuff = message.Substring(thisPos, 1);

                    if (charBuff == " ")
                    {
                        isBlankOrEnd = true;
                    }
                    else
                    {
                        // check the next char to the right
                        if (thisPos + 1 < message.Length)
                        {
                            charBuff = message.Substring(thisPos + 1, 1);

                            if (charBuff == " ")
                            {
                                isBlankOrEnd = true;
                            }
                        }
                        else
                        {
                            isBlankOrEnd = true;
                        }
                    }

                    // if it is NOT the blank or the end, we need to back up until we get to the start of the word
                    if (!isBlankOrEnd)
                    {
                        for (int i = thisPos; i >= 0; i--)
                        {
                            charBuff = message.Substring(i, 1);

                            if (i == 0)
                            {// if they just try to send one big long single word message of 160 chars we will jsut return it
                                noMoreParts = true;
                                return new List<string>() { message };
                            }
                            else if (charBuff == " ")
                            {
                                thisPart = message.Substring(blockStart, i - blockStart);
                                blockStart = i + 1;
                                break;
                            }
                        }
                    }
                    else
                    {// we can just set the next block start to this position + 1
                        thisPart = message.Substring(blockStart, thisPos - blockStart);
                        blockStart = thisPos + 1;

                    }


                }
                else
                {
                    thisPart = message.Substring(blockStart, message.Length - blockStart);
                    noMoreParts = true;
                }

                if (!string.IsNullOrEmpty(thisPart))
                {
                    parts.Add(thisPart + " ");
                }

            }

            return parts;


        }

        //
      

        public void GetInitialMessages()
        {
        }

        public void GetAllMessages()
        {


        }

        public void SortAllMessageThreadMessages()
        {
            if (null != MobileGlobals.SmsMessageThreadsDict && MobileGlobals.SmsMessageThreadsDict.Count > 0)
            {
                SmsMessagesSortByDateTimeAsc sorter = new SmsMessagesSortByDateTimeAsc();

                foreach (MessageThread t in MobileGlobals.SmsMessageThreadsDict.Values)
                {
                    if (null != t.Messages && t.Messages.Count > 1)
                    {
                        t.Messages.Sort(sorter);
                    }
                }
            }
        }



        public void GetUnreadMessages()
        {
            
        }


        public void GetSmsMmsMessagesForThread(MessageThread threadToGetMessagesFor, int limitCount)
        {
            
       
        }

        public void GetSmsMmsMessagesLimitedSet(int? msgThreadLimit, DateTime? olderThanDate)
        {
           
        }

        public void GetAllMessagesInBox()
        {
          
        }

        public void GetAllMessagesSent()
        {
           

        }

        /// <summary>
        /// Since an MMS message can have multiple parts including multiple images... we will return them broken out as multiple messages in a list
        /// </summary>
        /// <returns></returns>
        public List<Model.SmsMessage> GetLastMmsMessage()
        {

            return null;
        }

       

        public string GetMmsImageData(string partId, string imgName)
        {
            string newPath = System.IO.Path.Combine(MobileGlobals.ImagesDirectory, imgName);

            // make sure the image doesn't already exist at the newPath
            if (File.Exists(newPath))
            {
                return newPath;
            }

            // otherwise get the data and copy it to the new path
            Context ctx = AndroidGlobals.MainActivity;
            Android.Net.Uri partURI = Android.Net.Uri.Parse("content://mms/part/" + partId);
            Stream inStr = null;
            Bitmap bitmap = null;
            try
            {
                inStr = ctx.ApplicationContext.ContentResolver.OpenInputStream(partURI);

                bitmap = BitmapFactory.DecodeStream(inStr);

                if (null != bitmap)
                {
                    try 
                    {
                        FileStream outStr = new FileStream(newPath, FileMode.Create);
                        bitmap.Compress(Bitmap.CompressFormat.Png, 100, outStr); // bmp is your Bitmap instance
                                                                                 // PNG is a lossless format, the compression factor (100) is ignored
                        outStr.Close();
                    } 
                    catch (Exception e)
                    {
                        bool teststop = true;
                        if (teststop) { }
                    }
                }
            }
            catch (Exception e) 
            {
                bool teststop = true;
                if (teststop) { }
            }
            finally
            {
                if (inStr != null)
                {
                    try
                    {
                        inStr.Close();
                    }
                    catch (Exception e) 
                    { }
                }
            }

            return newPath;
        }

  
        
    }
}