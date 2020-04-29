using System;
using System.Collections.Generic;
using System.Text;
using AndroidTest.Interfaces;
using AndroidTest.Model.SortAndCompare;

namespace AndroidTest.Model
{
    public class MessageThread: IListViewResultItem
    {
        private List<SmsMessage> messages = null;

        public long ThreadId { get; set; }

        /// <summary>
        /// Key is Mobile Tel Num and Value is name of contact
        /// </summary>
        public List<KeyValuePair<string, PhoneContact>> Authors { get; set; }

        public string DisplayValue1 {
            get
            {
                StringBuilder sb = new StringBuilder(100);

                bool isFirst = true;
                foreach (KeyValuePair<string, PhoneContact> kvp in Authors)
                {
                    if (!isFirst)
                    {
                        sb.Append(", ");
                    }

                    if (null == kvp.Value)
                    {
                        sb.Append(kvp.Key);
                    }
                    else
                    {
                        if (Authors.Count == 1)
                        {// fullname
                            sb.Append(kvp.Value.FullName);
                        }
                        else
                        {// first name and last init
                            
                            if (string.IsNullOrEmpty(kvp.Value.FirstName))
                            {
                                sb.Append(kvp.Value.LastName);
                            }
                            else
                            {
                                sb.Append(kvp.Value.FirstName);

                                if (!string.IsNullOrEmpty(kvp.Value.LastName))
                                {
                                    sb.Append(kvp.Value.LastName.Substring(0, 1));
                                }
                            }
                        }
                        
                    }

                    isFirst = false;
                }
                
                return sb.ToString();
            }
        }

        public string DisplayValue2
        {
            get
            {
                if (null == Authors || Authors.Count == 0)
                {
                    return string.Empty;
                }
                else if (Authors.Count == 1 && null != Authors[0].Value && !string.IsNullOrEmpty(Authors[0].Value.PhoneNumber1))
                {
                    return Authors[0].Value.PhoneNumber1;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public int ListPosition { get; set; }

        public float SearchRsltSimilarity { get; set; }

        public List<SmsMessage> Messages
        {
            get
            {
                return messages;
            }
        }

        public DateTime LastSentReceivedDate { get; set; }
        public bool HasMms { get; set; }
        public bool AllDataLoaded { get; set; }

        public long LastReceivedSmsMsgId { get; set; }
        public long LastSentSmsMsgId { get; set; }
        public long LastMmsMsgId { get; set; }




        #region Methods

        public void AddMessage(Model.SmsMessage msg)
        {
            if (null == messages)
            {
                messages = new List<SmsMessage>(10);
            }

            messages.Add(msg);

            if (LastSentReceivedDate == DateTime.MinValue)
            {
                LastSentReceivedDate = msg.DateReceived;// we only use date received because date sent does not get set for some reason in the db
            }
            else
            {
                int cmp = msg.DateReceived.CompareTo(LastSentReceivedDate);

                if (cmp > 0)
                {
                    LastSentReceivedDate = msg.DateReceived;// we only use date received because date sent does not get set for some reason in the db
                }
            }

            if (!msg.IsOutboxSentItem && !msg.IsMMS && (LastReceivedSmsMsgId == 0 || msg.Id < LastReceivedSmsMsgId))
            {
                LastReceivedSmsMsgId = msg.Id;
            }
            else if (msg.IsOutboxSentItem && !msg.IsMMS && (LastSentSmsMsgId == 0 || msg.Id < LastSentSmsMsgId))
            {
                LastSentSmsMsgId = msg.Id;
            }
            else if (msg.IsMMS && (LastMmsMsgId == 0 || msg.Id < LastMmsMsgId))
            {
                LastMmsMsgId = msg.Id;
            }

        }

        public void AddMessages(List<Model.SmsMessage> msgLst)
        {
            if (null != msgLst)
            {
                if (msgLst.Count > 0)
                {
                    msgLst.Sort(new SmsMessagesSortByDateTimeAsc());
                    messages = msgLst;
                    LastSentReceivedDate = msgLst[msgLst.Count - 1].DateReceived;
                }
            }
        }

        #endregion

    }
}
