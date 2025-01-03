using PLCHESerialDebugger.Utilities;

namespace PLCHESerialDebugger
{
    public partial class FormPLCGatewayConfiguration : Form
    {
        public LogController LogController { get; set; }

        public PLCGatewayController PLCGatewayController { get; set; }

        public FormPLCGatewayConfiguration(PLCGatewayController plcGatewayController, LogController logController)
        {
            LogController = logController;
            PLCGatewayController = plcGatewayController;
            InitializeComponent();
            FormUtilities.ResizeFormWindow(0.4f, 0.4f, this); // if these values change, it impacts where all controls get placed; leave alone for now.
            CreateDynamicControls(); // Add our dynamic controls
        }

        private void CreateDynamicControls()
        {
            // === Dispose Serial Port Button ===
            var btnDumpIt900 = FormUtilities.CreateButton("Fetch it900 Data", 0.01f, 0.01f, 0.2f, 0.08f, this);
            btnDumpIt900.Click += btnDumpIt900_Click;
            btnDumpIt900.Font = new Font("Calibri Light", 14);
            Controls.Add(btnDumpIt900);
        }

        private async void btnDumpIt900_Click(object sender, EventArgs e)
        {
            PLCGatewayController.GetIT900Data();
            Thread.Sleep(100);
            string rxData = await PLCGatewayController.RetrievePLCGatewayPacket();
        }
    }
}