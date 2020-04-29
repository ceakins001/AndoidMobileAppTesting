using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidTest.Model.SortAndCompare
{
    public class MessageThreadsSortByReceivedTimeDesc : IComparer<MessageThread>
    {
        public int Compare(MessageThread x, MessageThread y)
        {
            // we sort by the last message received 
                int cmp = x.LastSentReceivedDate.CompareTo(y.LastSentReceivedDate);

                if (cmp > 0)
                {
                    return -1;
                }

                if (cmp < 0)
                {
                    return 1;
                }

                

            return 0;
        }
    }
}
