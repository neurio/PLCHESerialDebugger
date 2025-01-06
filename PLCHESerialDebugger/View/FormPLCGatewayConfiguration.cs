using PLCHESerialDebugger.Utilities;

namespace PLCHESerialDebugger
{
    public partial class FormPLCGatewayConfiguration : Form
    {
        public LogController LogController { get; set; }

        public PLCGatewayController PLCGatewayController { get; set; }

        public Button btnDumpIt900 { get; set; }

        public Button btnAddNode { get; set; }

        public Button btnDeleteNode { get; set; }

        public Button btnListNodes { get; set; }

        public FormPLCGatewayConfiguration(PLCGatewayController plcGatewayController, LogController logController)
        {
            LogController = logController;
            PLCGatewayController = plcGatewayController;
            InitializeComponent();
            FormUtilities.ResizeFormWindow(0.4f, 0.4f, this); // if these values change, it impacts where all controls get placed; leave alone for now.
            CreateDynamicControls(); // Add our dynamic controls
            LockSerialControls();
        }

        private void LockSerialControls()
        {
            if (!PLCGatewayController.GatewayInit)
            {
                btnDumpIt900.Enabled = false;
                btnAddNode.Enabled = false;
                btnDeleteNode.Enabled = false;
                btnListNodes.Enabled = false;
            }
            else
            {
                btnDumpIt900.Enabled = true;
                btnAddNode.Enabled = true;
                btnDeleteNode.Enabled = true;
                btnListNodes.Enabled = true;
            }
        }

        private void CreateDynamicControls()
        {
            // === Dump It900 Data Button ===
            btnDumpIt900 = FormUtilities.CreateButton("Fetch it900 Data", 0.01f, 0.01f, 0.2f, 0.08f, this);
            btnDumpIt900.Click += btnDumpIt900_Click;
            btnDumpIt900.Font = new Font("Calibri Light", 14);
            Controls.Add(btnDumpIt900);

            // === Add Node Button ===
            btnAddNode = FormUtilities.CreateButton("Add Node", 0.01f, 0.1f, 0.2f, 0.08f, this);
            btnAddNode.Click += btnAddNode_Click;
            btnAddNode.Font = new Font("Calibri Light", 14);
            Controls.Add(btnAddNode);

            // === Delete Node Button ===
            btnDeleteNode = FormUtilities.CreateButton("Delete Node", 0.01f, 0.2f, 0.2f, 0.08f, this);
            btnDeleteNode.Click += btnDeleteNode_Click;
            btnDeleteNode.Font = new Font("Calibri Light", 14);
            Controls.Add(btnDeleteNode);

            // === List Nodes Button ===
            btnListNodes = FormUtilities.CreateButton("List Nodes", 0.01f, 0.3f, 0.2f, 0.08f, this);
            btnListNodes.Click += btnListNodes_Click;
            btnListNodes.Font = new Font("Calibri Light", 14);
            Controls.Add(btnListNodes);
        }

        private async void btnDumpIt900_Click(object sender, EventArgs e)
        {
            PLCGatewayController.GetIT900Data();
            Thread.Sleep(100);
            string rxData = await PLCGatewayController.RetrievePLCGatewayPacket();
        }

        private async void btnAddNode_Click(object sender, EventArgs e)
        {
            int NodeID = -1;
            PLCGatewayController.AddNode(NodeID);
            LogController.AddLogMessage(new LogMessage(text: $"Adding Node: {NodeID}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
        }

        private async void btnDeleteNode_Click(object sender, EventArgs e)
        {
            int NodeID = -1;
            PLCGatewayController.DeleteNode(NodeID);
            LogController.AddLogMessage(new LogMessage(text: $"Deleting Node: {NodeID}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
        }

        private async void btnListNodes_Click(object sender, EventArgs e)
        {
            int NodeID = -1;
            int tempIndex = 1;
            string IdentifiedNodes = await PLCGatewayController.ListVisibleNodes();
            LogController.AddLogMessage(new LogMessage(text: $"Visible Nodes:", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));

        }
    }
}