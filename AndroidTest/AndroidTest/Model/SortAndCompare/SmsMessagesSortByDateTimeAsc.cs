using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidTest.Model.SortAndCompare
{
    public class SmsMessagesSortByDateTimeAsc : IComparer<Model.SmsMessage>
    {
        public int Compare(Model.SmsMessage x, Model.SmsMessage y)
        {
            // we sort by the last message received which should be the first in the thread
            int cmp = 0;

            cmp = x.DateReceived.CompareTo(y.DateReceived);

            // Commented out because we can't rely on the date_sent android db value to get set. It only gets set to 0 now when sending sms even if we specify a date in the values we use in the insert for some reason.
            //if (x.IsOutboxSentItem && y.IsOutboxSentItem)
            //{
            //    cmp = x.DateSent.CompareTo(y.DateSent);
            //}
            //else if (x.IsOutboxSentItem && !y.IsOutboxSentItem)
            //{
            //    cmp = x.DateSent.CompareTo(y.DateReceived);
            //}
            //else if (!x.IsOutboxSentItem && y.IsOutboxSentItem)
            //{
            //    cmp = x.DateReceived.CompareTo(y.DateSent);
            //}
            //else
            //{
            //    cmp = x.DateReceived.CompareTo(y.DateReceived);
            //}

            if (cmp > 0)
            {
                return 1;
            }

            if (cmp < 0)
            {
                return -1;
            }

            // if sent/received at the same time, sent is first
            if (x.IsOutboxSentItem && !y.IsOutboxSentItem)
            {
                return -1;
            }

            if (!x.IsOutboxSentItem && y.IsOutboxSentItem)
            {
                return 1;
            }

            // text also comes before image if sent/received same time
            if (!string.IsNullOrEmpty(x.Message) && string.IsNullOrEmpty(y.Message))
            {
                return -1;
            }

            if (string.IsNullOrEmpty(x.Message) && !string.IsNullOrEmpty(y.Message))
            {
                return 1;
            }

            return 0;
        }
    }
}
