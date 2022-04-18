using System;

namespace FoxMQ.Model
{
    public class MessageQueue
    {
        public long Id { get; set; }

        public string QueueName { get; set; } = string.Empty;

        public DateTime InsertionDate { get; set; }

        public bool Lock { get; set; }

        public string Body { get; set; } = string.Empty;
    }
}
