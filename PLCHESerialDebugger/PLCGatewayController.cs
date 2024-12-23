using CP110Library;
using CP110Library.New;
using System.Net;

namespace PLCHESerialDebugger
{
    public class PLCGatewayController
    {
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
            Console.WriteLine($"Closing {PLCGateway.GetType()}");
            PLCGateway.Close();
            // PLCGateway.Dispose(); // dispose should be called internally only
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
            PLCGateway.WriteRawString(textData);
            // should perform *ALL* IPLCGateway operations from within PLCGatewayController- this keeps the code very transferrable between projects
            // functions called via UI action of course
        }

        public void ReceivePLCGatewayPacket()
        {
            CacheInputBuffer();
            ParseInputBuffer();
        }

        public void ParseInputBuffer()
        {
            switch (ActivePLCGatewayType)
            {
                case PLCGatewayType.CP110:
                {

                    break;
                }
                case PLCGatewayType.SerialPLCHE:
                {

                    break;
                }
            }
        }

        public void CacheInputBuffer()
        {
            switch (ActivePLCGatewayType)
            {
                case PLCGatewayType.CP110:
                {
                    var UDPPLCGateway = PLCGateway as CP110Gateway;

                    if (UDPPLCGateway != null)
                    {
                        if (UDPInputBufferCached != UDPPLCGateway.InputBuffer)
                        {
                            UDPInputBufferCached = UDPPLCGateway.InputBuffer;
                        }
                    }

                    break;
                }

                case PLCGatewayType.SerialPLCHE:
                {
                    var SerialPLCHEGateway = PLCGateway as SerialPLCHE;

                    if (SerialPLCHEGateway != null)
                    {
                        if (SerialInputBufferCached != SerialPLCHEGateway.InputBuffer)
                        {
                            SerialInputBufferCached = SerialPLCHEGateway.InputBuffer;
                        }
                    }

                    break;
                }
            }
        }
    }
}
