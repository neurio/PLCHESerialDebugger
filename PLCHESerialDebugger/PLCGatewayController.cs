using CP110Library;
using CP110Library.New;
using System.Net;

namespace PLCHESerialDebugger
{
    public class PLCGatewayController
    {
        public enum PLCGatewayType
        {
            CP110,
            SerialPLCHE,
        }

        public bool GatewayInit { get; set; } = false;

        public IPLCGateway PLCGateway { get; set; }

        public static string DefaultCP110IpAddress = "10.0.0.117";

        public IPAddress DefaultCP110IPAddress { get; set; } = IPAddress.Parse(DefaultCP110IpAddress);

        public int DefaultUDPTransmitPortNumber { get; set; } = 4562;


        public PLCGatewayController()
        {

        }

        public void InitializePLCGateway(PLCGatewayType PLCGatewayType)
        {
            switch (PLCGatewayType)
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
        }

        public void SendPLCGatewayPacket()
        {
            // should perform *ALL* IPLCGateway operations from within PLCGatewayController- this keeps the code very transferrable between projects
            // functions called via UI action of course
        }

        public void ReceivePLCGatewayPacket()
        {

        }

        public void DeinitializePLCGateway()
        {
                
        }

    }
}
