using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AndroidTest.Model;

namespace AndroidTest.Interfaces
{
    public interface IMessagingProvider
    {

        void GetInitialMessages();

        void GetSmsMmsMessagesForThread(MessageThread threadToGetMessagesFor, int limitCount);

        void GetSmsMmsMessagesLimitedSet(int? msgThreadLimit, DateTime? olderThanDate);

        void GetAllMessages();

        void GetUnreadMessages();

        byte[] GetMMSPduData(string phone, string imgFilePath, string txtMessage);

        bool SendMMSPduData(byte[] pduData);

        bool SendMessage(long threadId, string message, string phoneAddr);

        List<Model.SmsMessage> GetLastMmsMessage();

        string GetMmsImageData(string partId, string imgName);
    }
}
