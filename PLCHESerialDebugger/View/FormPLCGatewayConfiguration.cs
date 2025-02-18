using PLCHESerialDebugger.Utilities;

namespace PLCHESerialDebugger
{
    public partial class FormPLCGatewayConfiguration : Form
    {
        public LogController LogController { get; set; }

        public PLCGatewayController PLCGatewayController { get; set; }

        public Button btnClearTelemetryMonitor { get; set; }

        public Button btnDumpIt900 { get; set; }

        public Button btnAddNode { get; set; }

        public Button btnDeleteNode { get; set; }

        public Button btnListNodes { get; set; }

        public TextBox txtNodeID { get; set; }

        public ListBox ListBoxTelemetryMonitor { get; set; }

        public CheckedListBox CheckedListBoxCurrentPageSelection { get; set; }

        public Dictionary<int, ListBox> TelemetryListBoxes { get; set; } = new Dictionary<int, ListBox>(); // <NodeID, (associated) ListBox)

        public FormPLCGatewayConfiguration(PLCGatewayController plcGatewayController, LogController logController)
        {
            LogController = logController;
            PLCGatewayController = plcGatewayController;
            InitializeComponent();
            FormUtilities.ResizeFormWindow(0.4f, 0.4f, this); // if these values change, it impacts where all controls get placed; leave alone for now.
            CreateDynamicControls(); // Add our dynamic controls
            AttachDataSources();
            LockSerialControls();

            Resize += new EventHandler(FormPLCGatewayConfiguration_Resize);
        }

        private void FormPLCGatewayConfiguration_Resize(object sender, EventArgs e)
        {
            FormUtilities.AdjustControlsSizesAndPositions(this);
        }

        private void AttachDataSources()
        {
            ListBoxTelemetryMonitor.DataSource = bindingSourceTelemetryData;
            bindingSourceTelemetryData.DataSource = LogController.TelemetryDataBindingLog;
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

            // === Node ID TextBox ===
            var lblNodeID = FormUtilities.CreateLabel("Node ID:", 0.01f, 0.4f, this);
            lblNodeID.Font = new Font("Calibri Light", 14, FontStyle.Bold);
            lblNodeID.BackColor = Color.Transparent;
            lblNodeID.BorderStyle = BorderStyle.Fixed3D;
            lblNodeID.Padding = new Padding(2);
            Controls.Add(lblNodeID);

            txtNodeID = FormUtilities.CreateTextBox(0.11f, 0.4f, 0.15f, 0.03f, this);
            txtNodeID.Font = new Font("Calibri Light", 12);
            Controls.Add(txtNodeID);

            // === Telemetry Data Window ===
            var lblTelemetry = FormUtilities.CreateLabel("Telemetry Data", 0.30f, 0.01f, this);
            lblTelemetry.Font = new Font("Calibri Light", 16, FontStyle.Bold);
            lblTelemetry.BackColor = Color.Transparent;
            lblTelemetry.BorderStyle = BorderStyle.Fixed3D;
            lblTelemetry.Padding = new Padding(2);
            Controls.Add(lblTelemetry);

            ListBoxTelemetryMonitor = FormUtilities.CreateListBox(0.30f, 0.09f, 0.60f, 0.75f, this);
            Controls.Add(ListBoxTelemetryMonitor);

            // === Clear Telemetry Monitor Window Button ===
            btnClearTelemetryMonitor = FormUtilities.CreateButton("Clear Data", 0.54f, 0.82f, 0.1f, 0.06f, this);
            btnClearTelemetryMonitor.Click += btnClearTelemetryMonitor_Click;
            btnClearTelemetryMonitor.Font = new Font("Calibri Light", 14);
            Controls.Add(btnClearTelemetryMonitor);

            // === Page Selection CheckedListBox ===

            var lblPageSelection = FormUtilities.CreateLabel("Page #", 0.01f, 0.48f, this);
            lblPageSelection.Font = new Font("Calibri Light", 14, FontStyle.Bold);
            lblPageSelection.BackColor = Color.Transparent;
            lblPageSelection.BorderStyle = BorderStyle.Fixed3D;
            lblPageSelection.Padding = new Padding(2);
            Controls.Add(lblPageSelection);

            List<int> pageNumbers = new List<int>() { 0, 1, 2, 3, 4 };
            CheckedListBoxCurrentPageSelection = FormUtilities.CreateCheckedListBox(0.01f, 0.55f, 0.04f, 0.20f, this);
            CheckedListBoxCurrentPageSelection.Items.AddRange(pageNumbers.Cast<object>().ToArray());
            CheckedListBoxCurrentPageSelection.ItemCheck += CheckedListBoxCurrentPageSelection_ItemCheck;

            Controls.Add(CheckedListBoxCurrentPageSelection);
        }

        private async void btnClearTelemetryMonitor_Click(object sender, EventArgs e)
        {
            LogController.TelemetryDataBindingLog.Clear();
        }

        private void CheckedListBoxCurrentPageSelection_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var checkedListBox = sender as CheckedListBox;
            if (checkedListBox == null) return;

            // Rebuild the list from checked items
            LogController.ActivePageNumbers = checkedListBox.CheckedItems.Cast<int>().ToList();
        }

        private async void btnDumpIt900_Click(object sender, EventArgs e)
        {
            PLCGatewayController.GetIT900Data();
            Thread.Sleep(100);
            string rxData = await PLCGatewayController.RetrievePLCGatewayPacket(isTelemetryPacket: false);
        }

        private async void btnAddNode_Click(object sender, EventArgs e)
        {
            string NodeID = txtNodeID.Text;

            int nodeIDLength = 16;

            if (txtNodeID.Text.Length != nodeIDLength)
            {
                LogController.AddLogMessage(new LogMessage(text: $"Invalid Node ID: {txtNodeID.Text}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }

            bool addedNode = await PLCGatewayController.AddNode(NodeID);

            if (addedNode)
            {
                LogController.AddLogMessage(new LogMessage(text: $"Added Node: {NodeID}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }
            else
            {
                LogController.AddLogMessage(new LogMessage(text: $"Failed to Add Node ID: {NodeID}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }
        }

        private async void btnDeleteNode_Click(object sender, EventArgs e)
        {
            int NodeID = -1;

            if (txtNodeID.Text.Length > 0)
            {
                if (!int.TryParse(txtNodeID.Text, out NodeID))
                {
                    LogController.AddLogMessage(new LogMessage(text: $"Invalid Node ID: {txtNodeID.Text}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
                }
            }

            bool deletedNode = await PLCGatewayController.DeleteNode(NodeID);

            if (deletedNode)
            {
                LogController.AddLogMessage(new LogMessage(text: $"Deleted Node: {NodeID}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }
            else
            {
                LogController.AddLogMessage(new LogMessage(text: $"Failed to Delete Node ID: {NodeID}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }
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