using CP110Library;
using CP110Library.New;
using System.ComponentModel;
using System.IO.Ports;
using System.Net;

namespace PLCHESerialDebugger
{
    public class PLCGatewayController
    {
        // should perform *ALL* IPLCGateway mutator operations from within PLCGatewayController --- this keeps the code very transferrable between projects
        // functions called via GUI action of course

        public PLCGatewayController(LogController logController)
        {
            LogController = logController;
        }

        public enum PLCGatewayType
        {
            CP110,
            SerialPLCHE,
            Invalid,
        }

        public bool GatewayInit { get; set; } = false;

        public IPLCGateway PLCGateway { get; set; }

        public static string DefaultCP110IpAddress = "10.0.0.117";

        public IPAddress DefaultCP110IPAddress { get; set; } = IPAddress.Parse(DefaultCP110IpAddress);

        public int DefaultUDPTransmitPortNumber { get; set; } = 4562;

        public List<String> SerialInputBufferCached { get; set; } = new List<string>();

        public Dictionary<string, KeyValuePair<int, byte[]>> UDPInputBufferCached { get; set; } = new Dictionary<string, KeyValuePair<int, byte[]>>(); // <timestamp, <page #, page data>>

        public PLCGatewayType ActivePLCGatewayType { get; set; } = PLCGatewayType.SerialPLCHE;
        
        public bool PersistentPollingEnabled { get; set; } = false;

        public LogController LogController { get; set; }

        public BindingList<string> COMPortsBindingList { get; set; } = new BindingList<string>();

        public void SendPLCGatewayPacket(string textData) // for Serial PLCHE
        {
            // not much sense in generalizing this cp110/serialplche single packet transmission, as required commands will be called via a GUI control

            //LogController.AddLogMessage(new LogMessage(text: $"Writing {textData}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));

            try
            {
                PLCGateway.WriteRawString(textData);
            }
            catch (Exception ex)
            {
                LogController.AddLogMessage(new LogMessage(text: $"{ex.ToString()}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }
        }

        public async Task<string> RetrievePLCGatewayPacket(bool isTelemetryPacket, int? msTimeout = 100)
        {
            string returnValue = "plc packet retrieval error";

            try
            {
                LogMessage.messageType messageType; // type TBD

                await Task.Delay((int)msTimeout);

                returnValue = await PLCGateway.ReadRawString();

                if (isTelemetryPacket)
                {
                    messageType = LogMessage.messageType.Telemetry;
                }
                else
                {
                    messageType = LogMessage.messageType.Base;
                }

                LogController.AddLogMessage(new LogMessage(text: $"{returnValue}", messageType: messageType, timeStamp: DateTime.UtcNow));
            }
            catch (Exception ex)
            {
                LogController.AddLogMessage(new LogMessage(text: $"{ex.ToString()}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }

            return returnValue;
        }

        public void SetParameter(int parameterNumber, int value, int? address = 1)
        {
            SendPLCGatewayPacket($"plc-set-param {parameterNumber} {value} {address}");
        }

        public void EnableUL1741TestMode()
        {
            SetParameter(4, 1);
        }

        public void EnableDCSupplyMode()
        {
            SetParameter(3, 1);
        }

        public async Task<bool> AddNode(int nodeID)
        {
            bool success;
            SendPLCGatewayPacket($"it900-node-add {nodeID}");
            string result = await RetrievePLCGatewayPacket(isTelemetryPacket: false, msTimeout: 20000);
            
            if (result.Contains(""))
            {
                success = true;
            }
            else
            {
                success = false;
            }

            return success;
        }

        public async Task<bool> DeleteNode(int nodeID)
        {
            bool success;
            SendPLCGatewayPacket($"it900-node-del {nodeID}");
            string result = await RetrievePLCGatewayPacket(isTelemetryPacket: false);
            
            if (result.Contains(""))
            {
                success = true;
            }
            else
            {
                success = false;
            }

            return success;
        }

        public async Task<string> ListVisibleNodes()
        {
            SendPLCGatewayPacket($"it900-nodes");
            Thread.Sleep(100);
            string NodeList = await RetrievePLCGatewayPacket(isTelemetryPacket: false);

            return NodeList;
        }

        public void GetIT900Data()
        {
            SendPLCGatewayPacket("it900");
        }

        public void ScanSerialPorts()
        {
            List<string> portNames = SerialPort.GetPortNames().ToList();

            foreach (string portName in portNames)
            {
                COMPortsBindingList.Add(portName);
                LogController.AddLogMessage(new LogMessage(text: $"{portName} detected!", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }
        }

        public void EnablePersistentPolling()
        {
            // need some identifier mechanism that allows me to know that this packet is page data
            // this polling task and serial writes can happen around each other.

            try
            {
                PLCGateway.BeginPersistentPolling();
                PersistentPollingEnabled = true;
                LogController.AddLogMessage(new LogMessage(text: $"Persistent Polling Enabled", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }
            catch (Exception ex)
            {
                LogController.AddLogMessage(new LogMessage(text: $"{ex.ToString()}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }
        }


        public void DisablePersistentPolling()
        {
            try
            {
                PLCGateway.AbortPersistentPolling();
                PersistentPollingEnabled = false;
                LogController.AddLogMessage(new LogMessage(text: $"Persistent Polling Disabled", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }
            catch (Exception ex)
            {
                LogController.AddLogMessage(new LogMessage(text: $"{ex.ToString()}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }
        }

        public void UpdatePLCGatewayType(PLCGatewayType gatewayType)
        {
            switch (gatewayType)
            {
                case PLCGatewayType.SerialPLCHE:
                    {
                        ActivePLCGatewayType = PLCGatewayType.SerialPLCHE;
                        break;
                    }
                case PLCGatewayType.CP110:
                    {
                        ActivePLCGatewayType = PLCGatewayType.CP110;
                        break;
                    }
            }
        }

        public void DeinitializePLCGateway()
        {
            try
            {
                LogController.AddLogMessage(new LogMessage(text: $"Closing {PLCGateway.GetType()}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
                PLCGateway.Close();
                GatewayInit = false;
            }
            catch (Exception ex)
            {
                LogController.AddLogMessage(new LogMessage(text: $"{ex.ToString()}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }
        }

        public bool InitializePLCGateway(int? SerialPortNumber, int? SerialBaudRate)
        {
            if (PLCGateway != null)
            {
                DeinitializePLCGateway();
            }

            try
            {
                switch (ActivePLCGatewayType)
                {
                    case PLCGatewayType.CP110:
                        {
                            PLCGateway = new CP110Gateway(iPAddress: DefaultCP110IPAddress, portNumber: DefaultUDPTransmitPortNumber);
                            break;
                        }
                    case PLCGatewayType.SerialPLCHE:
                        {
                            PLCGateway = new SerialPLCHE(ComPortNumber: SerialPortNumber, BaudRate: SerialBaudRate);
                            break;
                        }
                }

                GatewayInit = true;
                LogController.AddLogMessage(new LogMessage(text: $"{PLCGateway.GetType()} init. and ready to use.", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
                return true;
            }
            catch (Exception ex)
            {
                LogController.AddLogMessage(new LogMessage(text: $"{ex.ToString()}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
                LogController.AddLogMessage(new LogMessage(text: $"PLCGateway not initialized; some issue occurred.", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }

            return false;
        }

        public string ReadAndUpdateInputBuffer()
        {
            string newEntries = string.Empty;

            switch (ActivePLCGatewayType)
            {
                case PLCGatewayType.CP110:
                    {
                        var UDPPLCGateway = PLCGateway as CP110Gateway;

                        if (UDPPLCGateway != null)
                        {
                            newEntries = GetNewDictionaryEntries(UDPInputBufferCached, UDPPLCGateway.InputBuffer);
                            UDPInputBufferCached = UDPPLCGateway.InputBuffer;
                        }

                        break;
                    }

                case PLCGatewayType.SerialPLCHE:
                    {
                        var SerialPLCHEGateway = PLCGateway as SerialPLCHE;

                        if (SerialPLCHEGateway != null)
                        {
                            newEntries = GetNewListEntries(SerialInputBufferCached, SerialPLCHEGateway.InputBuffer);
                            SerialInputBufferCached = SerialPLCHEGateway.InputBuffer;
                        }

                        break;
                    }
            }

            LogController.AddLogMessage(new LogMessage(text: $"{newEntries}", messageType: LogMessage.messageType.Serial, timeStamp: DateTime.UtcNow));
            return newEntries;
        }

        private string GetNewDictionaryEntries(Dictionary<string, KeyValuePair<int, byte[]>> oldBuffer, Dictionary<string, KeyValuePair<int, byte[]>> newBuffer)
        {
            var changes = new List<string>();

            foreach (var key in newBuffer.Keys)
            {
                if (!oldBuffer.ContainsKey(key))
                {
                    var entry = newBuffer[key];
                    string pageDataHex = BitConverter.ToString(entry.Value).Replace("-", " ");
                    changes.Add($"New Entry: Timestamp = {key}, Page# = {entry.Key}, PageData = {pageDataHex}");
                }
            }

            return string.Join(Environment.NewLine, changes);
        }

        private string GetNewListEntries(List<string> oldBuffer, List<string> newBuffer)
        {
            var changes = new List<string>();

            foreach (var item in newBuffer)
            {
                if (!oldBuffer.Contains(item))
                {
                    changes.Add($"New Entry: {item}");
                }
            }

            return string.Join(Environment.NewLine, changes);
        }
    }
}