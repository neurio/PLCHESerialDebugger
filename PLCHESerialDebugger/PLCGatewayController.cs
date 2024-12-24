using CP110Library;
using CP110Library.New;
using System.Net;

namespace PLCHESerialDebugger
{
    public class PLCGatewayController
    {
        // should perform *ALL* IPLCGateway mutator operations from within PLCGatewayController --- this keeps the code very transferrable between projects
        // functions called via GUI action of course

        public PLCGatewayController()
        {
            // not much to do in constructor..
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

        public Dictionary<string, KeyValuePair<int, byte[]>> UDPInputBufferCached { get; set; } = new Dictionary<string, KeyValuePair<int, byte[]>>();

        public PLCGatewayType ActivePLCGatewayType { get; set; } = PLCGatewayType.SerialPLCHE;
        
        public bool PersistentPollingEnabled { get; set; } = false;

        public void EnablePersistentPolling()
        {
            try
            {
                PLCGateway.BeginPersistentPolling();
                PersistentPollingEnabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public void DisablePersistentPolling()
        {
            try
            {
                PLCGateway.AbortPersistentPolling();
                PersistentPollingEnabled = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
                Console.WriteLine($"Closing {PLCGateway.GetType()}");
                PLCGateway.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString());
            }
        }

        public void InitializePLCGateway()
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
                            PLCGateway = new SerialPLCHE();
                            break;
                        }
                }

                Console.WriteLine($"{PLCGateway.GetType()} init. and ready to use.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("PLCGateway not initialized; some issue occurred.");
            }
        }

        // not much sense in generalizing this cp110/serialplche single packet transmission, as required commands will be called via a GUI control
        public void SendPLCGatewayPacket(string textData) // for Serial PLCHE
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Console.WriteLine($"{timestamp}: Writing {textData}");
            PLCGateway.WriteRawString(textData);
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
                            UDPInputBufferCached = new Dictionary<string, KeyValuePair<int, byte[]>>(UDPPLCGateway.InputBuffer);
                        }

                        break;
                    }

                case PLCGatewayType.SerialPLCHE:
                    {
                        var SerialPLCHEGateway = PLCGateway as SerialPLCHE;

                        if (SerialPLCHEGateway != null)
                        {
                            newEntries = GetNewListEntries(SerialInputBufferCached, SerialPLCHEGateway.InputBuffer);
                            SerialInputBufferCached = new List<string>(SerialPLCHEGateway.InputBuffer);
                        }

                        break;
                    }
            }

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