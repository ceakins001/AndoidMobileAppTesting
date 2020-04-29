using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace AndroidTest.Model
{
    public class SmsMessage
    {
        public long Id { get; set; }
        public long ThreadId { get; set; }

        public long MessageGroupId { get; set; }
        public bool IsRead { get; set; }

        public bool IsMMS { get; set; }

        public bool IsOutboxSentItem { get; set; }
        public string ToFromPhoneAddress { get; set; }

        public string PhoneLookupKey { get; set; }

        public string FirstNameToFrom { get; set; }
        public string LastNameToFrom { get; set; }

        public string FullNameToFrom { 
            get
            {
                string name = string.Empty;

                if (!string.IsNullOrEmpty(FirstNameToFrom))
                {
                    name += FirstNameToFrom;

                    if (!string.IsNullOrEmpty(LastNameToFrom))
                    {
                        name += " " + LastNameToFrom;
                    }
                }
                else
                {
                    name = LastNameToFrom;
                }

                return name;

            }
        }

        public string Sender
        {
            get
            {
                if (IsOutboxSentItem)
                {

                    return "                                       🌟[YOU]🌟 ";
                }
                else
                {
                    string val = "♦️[";
                    if (string.IsNullOrEmpty(FullNameToFrom))
                    {
                        val += ToFromPhoneAddress;
                    }
                    else
                    {
                        val += FullNameToFrom;
                    }

                    val += "]♦️\r\n";

                    return val;

                }
            }
        }

        private string image = null;
        public string Image
        {
            get
            {
                if (IsMMS)
                {

                    if (string.IsNullOrEmpty(ImagePath))
                    {
                        if (string.IsNullOrEmpty(ImageName) || string.IsNullOrEmpty(ImagePartId))
                        {
                            return null;
                        }

                        if (null != MobileGlobals.SmsProviderInst)
                        {
                            ImagePath = MobileGlobals.SmsProviderInst.GetMmsImageData(ImagePartId, ImageName);

                        }
                    }

                    if (!string.IsNullOrEmpty(ImagePath))
                    {
                        return ImagePath;
                    }
                }
                
                return null;

            }

            set
            {
                image = value;
            }
        }

        //public ImageSource Image { get; set; }

        public string ImageName { get; set; }
        public string ImagePartId { get; set; }

        public string ImagePath { get; set; }

        public DateTime DateSent { get; set; }
        public DateTime DateReceived { get; set; }

        public string DateReceivedAsString
        {
            get
            {
                string shortDate = DateReceived.ToShortDateString();

                if (IsOutboxSentItem)
                {
                    if (shortDate == DateTime.Now.ToShortDateString())
                    {
                        return "                                       Today " + DateReceived.ToShortTimeString();
                    }
                    else if (shortDate == DateTime.Now.AddDays(-1).ToShortDateString())
                    {
                        return "                                       Yesterday " + DateReceived.ToShortTimeString();
                    }
                    else
                    {
                        return "                                       " + shortDate + " " + DateReceived.ToShortTimeString();
                    }
                }
                else
                {
                    if (shortDate == DateTime.Now.ToShortDateString())
                    {
                        return "Today " + DateReceived.ToShortTimeString();
                    }
                    else if (shortDate == DateTime.Now.AddDays(-1).ToShortDateString())
                    {
                        return "Yesterday " + DateReceived.ToShortTimeString();
                    }
                    else
                    {
                        return shortDate + " " + DateReceived.ToShortTimeString();
                    }
                }
            }
        }
        public string Message { get; set; }

        private Color textColor = Color.White;
        public Color TextColor
        {
            get
            {
                return TextColor;
            }

            set
            {
                textColor = value;
            }
        }
    }
}
