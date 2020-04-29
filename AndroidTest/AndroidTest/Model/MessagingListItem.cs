using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidTest.Model
{
    public class MessagingListItem 
    {
        public string DisplayValue1 { get; set; }
        public string DisplayValue2 { get; set; }

        public bool IsSender { get; set; }

        public string SenderName {get; set;}

        public string SenderPhone { get; set; }

        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }

        public bool ResponseSent { get; set; }

        public string Message { get; set; }

    }
}
