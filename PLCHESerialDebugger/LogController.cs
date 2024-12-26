namespace PLCHESerialDebugger
{
    public class LogController // better to remain non-static 
    {
        public LogController()
        {

        }

        public List<LogMessage> BaseLog = new List<LogMessage>();

        public List<LogMessage> VISALog = new List<LogMessage>();

        public List<LogMessage> UDPLog = new List<LogMessage>();

        public List<LogMessage> SerialLog = new List<LogMessage>();

        public void AddLogMessage(LogMessage message)
        {
            Console.WriteLine($"{message.TimeStamp}: {message.Text}");

            switch (message.MessageType)
            {
                case LogMessage.messageType.Base:
                    {
                        BaseLog.Add(message);
                        break;
                    }
                case LogMessage.messageType.VISA:
                    {
                        VISALog.Add(message);
                        break;
                    }
                case LogMessage.messageType.UDP:
                    {
                        UDPLog.Add(message);
                        break;
                    }
                case LogMessage.messageType.Serial:
                {
                    SerialLog.Add(message);
                    break;
                }
            }
        }
    }

    public class LogMessage
    {
        public string Text { get; set; }

        public messageType MessageType { get; set; }

        public string TimeStamp { get; set; }

        public enum messageType
        {
            Base,
            VISA,
            UDP,
            Serial,
        }

        public LogMessage(string text, messageType messageType, DateTime timeStamp)
        {
            TimeStamp = timeStamp.ToString("yyyy-MM-dd HH:mm:ss:FFF");
            Text = $"{TimeStamp}: {text}";
            MessageType = messageType;
        }
    }
}
