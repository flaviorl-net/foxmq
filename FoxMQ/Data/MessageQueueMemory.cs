using FoxMQ.Model;
using System.Collections.Generic;
using System;

namespace FoxMQ.Data
{
    public class MessageQueueMemory
    {
        public List<MessageQueue> MessageQueue { get; set; }

        public MessageQueueMemory()
        {
            MessageQueue = new List<MessageQueue>();
        }

        public void Add(string queueName, string body)
        {
            MessageQueue.Add(new MessageQueue() {
                Body = body,
                QueueName = queueName,
                Id = 0,
                InsertionDate = DateTime.Now,
                Lock = false
            });

        }
    }
}
