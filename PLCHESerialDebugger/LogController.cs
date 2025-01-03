using Microsoft.VisualBasic.Logging;
using System.ComponentModel;
using System.Text.RegularExpressions;

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

        public BindingList<string> SerialDataBindingLog { get; set; } = new BindingList<string>();

        public BindingList<string> SystemBaseDataBindingLog { get; set; } = new BindingList<string>();

        public BindingList<string> TelemetryDataBindingLog { get; set;} = new BindingList<string>();

        public static int LastSyncedSerialDataIndex { get; set; } = 0;

        public static int LastSyncedSystemBaseDataIndex { get; set; } = 0;

        private static readonly Regex logPattern = new Regex(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}:\d{3}): (.+)$");

        public void AddLogMessage(LogMessage message)
        {

            Console.WriteLine($"{message.Text}");

            switch (message.MessageType)
            {
                case LogMessage.messageType.Base:
                    {
                        BaseLog.Add(message);
                        SyncSystemBaseDataBindingLog(); // For GUI
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
                        Match match = logPattern.Match(message.Text);
                        if(match.Success)
                        {
                            SerialLog.Add(message);
                            SyncSerialDataBindingLog(); // For GUI
                            //string timestamp = match.Groups[1].Value;
                            //string messageText = match.Groups[2].Value;
                        }


                        break;
                }
            }
        }

        public void SyncSerialDataBindingLog()
        {
            for (int x = LastSyncedSerialDataIndex; x < SerialLog.Count; x++)
            {
                var logMessage = SerialLog[x];
                string formattedLog = $"{logMessage.Text}";
                SerialDataBindingLog.Add(formattedLog);
            }

            LastSyncedSerialDataIndex = SerialLog.Count; // Update the synced index
        }

        public void SyncSystemBaseDataBindingLog()
        {
            for (int x = LastSyncedSystemBaseDataIndex; x < BaseLog.Count; x++)
            {
                var logMessage = BaseLog[x];
                var lines = logMessage.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                foreach (var line in lines)
                {
                    SystemBaseDataBindingLog.Add(line);
                }

                // string formattedLog = $"{logMessage.Text}";
                // SystemBaseDataBindingLog.Add(formattedLog);
            }

            LastSyncedSystemBaseDataIndex = BaseLog.Count; // Update the synced index
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

        public LogMessage(string text, messageType messageType, DateTime timeStamp, bool? useTimeStamp = false)
        {
            TimeStamp = timeStamp.ToString("yyyy-MM-dd HH:mm:ss:FFF");
            if (useTimeStamp == true)
            {
                Text = $"{TimeStamp}: {text}";
            }
            else
            {
                Text = text;
            }
            MessageType = messageType;
        }
    }
}
