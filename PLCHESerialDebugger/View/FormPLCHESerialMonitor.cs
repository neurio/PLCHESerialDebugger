using System.ComponentModel;
using System.IO.Ports;
using PLCHESerialDebugger.Utilities;

namespace PLCHESerialDebugger
{
    public partial class PLCHESerialMonitorForm : Form
    {
        public ComboBox ComboBoxForCOMPorts { get; set; }

        public ComboBox ComboBoxForBaudRates { get; set; }

        public ListBox ListBoxSystemLog { get; set; }

        public ListBox ListBoxTelemetryMonitor { get; set; }

        public TextBox txtCmdString { get; set; }

        public TextBox txtCmdArgument { get; set; }

        public CheckBox chkTogglePLCGatewayType { get; set; }

        public CheckBox chkPollingEnabled { get; set; }

        public Button btnSendCmd { get; set; }

        public Button btnScanSerialPorts { get; set; }

        public Button btnOpenPLCGatewayConfigurationForm { get; set; }

        public Button btnClearSystemLog { get; set; }

        public System.Windows.Forms.Timer UIUpdateTimer { get; set; } = new System.Windows.Forms.Timer();

        public PLCGatewayController PLCGatewayController { get; set; }

        public LogController LogController { get; set; }

        public List<string> SupportedBaudRates { get; set; } = new List<string>() { "115200", "57600", "38400", "19200", "9600" };

        public PLCHESerialMonitorForm()
        {
            InitializeComponent(); // Ensures the designer-generated code runs
            FormUtilities.ResizeFormWindow(0.8f, 0.8f, this); // if these values change, it impacts where all controls get placed; leave alone for now.
            CreateDynamicControls(); // Add our dynamic controls
            LogController = new LogController();
            PLCGatewayController = new PLCGatewayController(LogController);
            AttachDataSources();
            AttachCustomEventHandlers();
            LockSerialControls();
        }

        public void AttachCustomEventHandlers()
        {
            LogController.SerialDataBindingLog.ListChanged += SerialDataBindingLog_ListChanged;
            LogController.SystemBaseDataBindingLog.ListChanged += SystemBaseDataBindingLog_ListChanged;

            PLCGatewayController.COMPortsBindingList.ListChanged += COMPortsBindingList_ListChanged;
            PLCGatewayController.TelemetryDataBindingLog.ListChanged += TelemetryDataBindingLog_ListChanged;
        }

        public void SerialDataBindingLog_ListChanged(object sender, ListChangedEventArgs e)
        {

            // run nuanced GUI update logic here
        }

        public void SystemBaseDataBindingLog_ListChanged(object sender, ListChangedEventArgs e)
        {
            // ListBoxSystemLog.TopIndex = (ListBoxSystemLog.Items.Count - 1);
            ListBoxSystemLog.SelectedIndex = (ListBoxSystemLog.Items.Count - 1);
            // run naunced GUI update logic here
        }

        public void TelemetryDataBindingLog_ListChanged(object sender, ListChangedEventArgs e)
        {
            // run naunced GUI update logic here
        }

        public void COMPortsBindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            // run naunced GUI update logic here
        }

        public void AttachDataSources()
        {
            // Bind WinForm control to BindingList
            ListBoxSystemLog.DataSource = bindingSourceSystemBaseData;
            ListBoxTelemetryMonitor.DataSource = bindingSourceTelemetryData;
            ComboBoxForCOMPorts.DataSource = bindingSourceCOMPorts;

            // Relate BindingList to 'normal' datatypes
            bindingSourceRXData.DataSource = LogController.SerialDataBindingLog;
            bindingSourceSystemBaseData.DataSource = LogController.SystemBaseDataBindingLog;
            bindingSourceCOMPorts.DataSource = PLCGatewayController.COMPortsBindingList;
            bindingSourceTelemetryData.DataSource = PLCGatewayController.TelemetryDataBindingLog;
        }

        private void CreateDynamicControls()
        {
            // === System Transaction Window ===
            var lblSystemLog = FormUtilities.CreateLabel("System Log", 0.01f, 0.015f, this);
            lblSystemLog.Font = new Font("Calibri Light", 16, FontStyle.Bold);
            lblSystemLog.BackColor = Color.Transparent;
            lblSystemLog.BorderStyle = BorderStyle.Fixed3D;
            lblSystemLog.Padding = new Padding(2);
            Controls.Add(lblSystemLog);

            ListBoxSystemLog = FormUtilities.CreateListBox(0.01f, 0.05f, 0.98f, 0.35f, this); // Occupy full width at the top
            ListBoxSystemLog.Font = new Font("Calibri Light", 10);
            ListBoxSystemLog.HorizontalScrollbar = true;
            ListBoxSystemLog.ScrollAlwaysVisible = true;
            Controls.Add(ListBoxSystemLog);

            // === Telemetry Data Window ===
            var lblTelemetry = FormUtilities.CreateLabel("Telemetry Data", 0.01f, 0.40f, this);
            lblTelemetry.Font = new Font("Calibri Light", 16, FontStyle.Bold);
            lblTelemetry.BackColor = Color.Transparent;
            lblTelemetry.BorderStyle = BorderStyle.Fixed3D;
            lblTelemetry.Padding = new Padding(2);
            Controls.Add(lblTelemetry);

            ListBoxTelemetryMonitor = FormUtilities.CreateListBox(0.01f, 0.44f, 0.98f, 0.1f, this); // Full width
            Controls.Add(ListBoxTelemetryMonitor);

            // === Serial Port Configurations ===
            var lblSerialPortConfiguration = FormUtilities.CreateLabel("Serial Port Config.", 0.01f, 0.64f, this);
            lblSerialPortConfiguration.Font = new Font("Calibri Light", 16, FontStyle.Bold);
            lblSerialPortConfiguration.BackColor = Color.Transparent;
            lblSerialPortConfiguration.BorderStyle = BorderStyle.Fixed3D;
            lblSerialPortConfiguration.Padding = new Padding(2);
            Controls.Add(lblSerialPortConfiguration);

            // === Command TextBox ===
            var lblCmdString = FormUtilities.CreateLabel("Command String", 0.01f, 0.69f, this);
            lblCmdString.Font = new Font("Calibri Light", 14, FontStyle.Bold);
            lblCmdString.BackColor = Color.Transparent;
            lblCmdString.BorderStyle = BorderStyle.Fixed3D;
            lblCmdString.Padding = new Padding(2);
            Controls.Add(lblCmdString);

            txtCmdString = FormUtilities.CreateTextBox(0.01f, 0.72f, 0.15f, 0.03f, this); // Reduced width
            txtCmdString.Font = new Font("Calibri Light", 12);
            Controls.Add(txtCmdString);

            // === Argument TextBox ===
            var lblCmdArgument = FormUtilities.CreateLabel("Argument String", 0.16f, 0.69f, this);
            lblCmdArgument.Font = new Font("Calibri Light", 14, FontStyle.Bold);
            lblCmdArgument.BackColor = Color.Transparent;
            lblCmdArgument.BorderStyle = BorderStyle.Fixed3D;
            lblCmdArgument.Padding = new Padding(2);
            Controls.Add(lblCmdArgument);

            txtCmdArgument = FormUtilities.CreateTextBox(0.16f, 0.72f, 0.15f, 0.03f, this); // Reduced width
            txtCmdArgument.Font = new Font("Calibri Light", 12);
            Controls.Add(txtCmdArgument);

            // === Send Button ===
            btnSendCmd = FormUtilities.CreateButton("Send", 0.31f, 0.718f, 0.04f, 0.03f, this);
            btnSendCmd.Font = new Font("Calibri Light", 12);
            btnSendCmd.Click += BtnSendCmd_Click;
            Controls.Add(btnSendCmd);

            // === Scan Serial Button ===

            btnScanSerialPorts = FormUtilities.CreateButton("Scan Serial Ports", 0.25f, 0.695f, 0.07f, 0.025f, this);
            btnScanSerialPorts.Font = new Font("Calibri Light", 12);
            btnScanSerialPorts.Click += BtnScanSerialPorts_Click;
            Controls.Add(btnScanSerialPorts);

            // === Status Panels ===
            var lblStatusWord1 = FormUtilities.CreateLabel("Status Word 1", 0.01f, 0.56f, this);
            lblStatusWord1.Font = new Font("Calibri Light", 16, FontStyle.Bold);
            lblStatusWord1.BackColor = Color.Transparent;
            lblStatusWord1.BorderStyle = BorderStyle.Fixed3D;
            lblStatusWord1.Padding = new Padding(2);
            Controls.Add(lblStatusWord1);
            Controls.Add(FormUtilities.CreateStatusPanel("Status Word 1", 0.01f, 0.60f, 0.48f, 0.035f, this)); // Reduced height for compactness

            var lblStatusWord2 = FormUtilities.CreateLabel("Status Word 2", 0.51f, 0.56f, this);
            lblStatusWord2.Font = new Font("Calibri Light", 16, FontStyle.Bold);
            lblStatusWord2.BackColor = Color.Transparent;
            lblStatusWord2.BorderStyle = BorderStyle.Fixed3D;
            lblStatusWord2.Padding = new Padding(2);
            Controls.Add(lblStatusWord2);
            Controls.Add(FormUtilities.CreateStatusPanel("Status Word 2", 0.51f, 0.60f, 0.48f, 0.035f, this)); // Reduced height for compactness

            // === COM Port Selections ===
            var lblCOMPorts = FormUtilities.CreateLabel("COM Port #", 0.25f, 0.64f, this);
            lblCOMPorts.Font = new Font("Calibri Light", 14, FontStyle.Bold);
            lblCOMPorts.BackColor = Color.Transparent;
            lblCOMPorts.BorderStyle = BorderStyle.Fixed3D;
            lblCOMPorts.Padding = new Padding(2);
            Controls.Add(lblCOMPorts);

            ComboBoxForCOMPorts = FormUtilities.CreateComboBox(0.25f, 0.67f, 0.05f, 0.1f, this);
            Controls.Add(ComboBoxForCOMPorts);

            // === Baud Rate ComboBox ===
            var lblBaudRates = FormUtilities.CreateLabel("Baud Rate", 0.31f, 0.64f, this);
            lblBaudRates.Font = new Font("Calibri Light", 14, FontStyle.Bold);
            lblBaudRates.BackColor = Color.Transparent;
            lblBaudRates.BorderStyle = BorderStyle.Fixed3D;
            lblBaudRates.Padding = new Padding(2);
            Controls.Add(lblBaudRates);

            ComboBoxForBaudRates = FormUtilities.CreateComboBox(0.31f, 0.67f, 0.05f, 0.1f, this);
            Controls.Add(ComboBoxForBaudRates);

            FormUtilities.PopulateComboBox(ComboBoxForBaudRates, SupportedBaudRates);

            // === Polling Checkbox ===
            chkPollingEnabled = FormUtilities.CreateCheckBox("Enable Polling", 0.11f, 0.63f, 0.1f, 0.04f, this); // Adjusted position
            chkPollingEnabled.Font = new Font("Calibri Light", 12, FontStyle.Bold);
            chkPollingEnabled.CheckedChanged += ChkPollingEnabled_CheckedChanged;
            Controls.Add(chkPollingEnabled);

            // === Init Serial Port Button ===
            var btnInitSerial = FormUtilities.CreateButton("Initialize Serial", 0.01f, 0.75f, 0.1f, 0.04f, this);
            btnInitSerial.Click += BtnInitSerial_Click;
            btnInitSerial.Font = new Font("Calibri Light", 14);
            Controls.Add(btnInitSerial);

            // === Dispose Serial Port Button ===
            var btnDisposeSerial = FormUtilities.CreateButton("Dispose Serial", 0.11f, 0.75f, 0.1f, 0.04f, this);
            btnDisposeSerial.Click += BtnDisposeSerial_Click;
            btnDisposeSerial.Font = new Font("Calibri Light", 14);
            Controls.Add(btnDisposeSerial);

            // === Open PLC Gateway Form Button ===
            btnOpenPLCGatewayConfigurationForm = FormUtilities.CreateButton("PLC Configuration", 0.21f, 0.75f, 0.1f, 0.04f, this);
            btnOpenPLCGatewayConfigurationForm.Click += BtnPLCConfiguration_Click;
            btnOpenPLCGatewayConfigurationForm.Font = new Font("Calibri Light", 14);
            Controls.Add(btnOpenPLCGatewayConfigurationForm);

            // === Clear System Log Button ===
            btnClearSystemLog = FormUtilities.CreateButton("Clear Log", 0.31f, 0.75f, 0.1f, 0.04f, this);
            btnClearSystemLog.Click += btnClearSystemLog_Click;
            btnClearSystemLog.Font = new Font("Calibri Light", 14);
            Controls.Add(btnClearSystemLog);

            // === Enable Serial Checkbox ===
            chkTogglePLCGatewayType = FormUtilities.CreateCheckBox("Serial PLCHE", 0.11f, 0.66f, 0.2f, 0.04f, this); // Placed closer to other controls
            chkTogglePLCGatewayType.Font = new Font("Calibri Light", 12, FontStyle.Bold);
            chkTogglePLCGatewayType.CheckState = CheckState.Checked;
            chkTogglePLCGatewayType.CheckedChanged += ChkTogglePLCGatewayType_CheckedChanged;
            Controls.Add(chkTogglePLCGatewayType);

            // UI Update Timer
            UIUpdateTimer.Interval = 3000; // Set interval to 100ms
            UIUpdateTimer.Tick += UIUpdateTimer_Tick; // Add event handler for the Tick event
        }

        // Timer Tick event handler
        private async void UIUpdateTimer_Tick(object sender, EventArgs e)
        {
            /// need to add:
            /// 1. Node ID selection
            /// 2. Page Number Selection
            /// 3. Add some logic to iterate over list of known, active node IDs and populate to respective windows.
            /// TODO
            /// 1. Convert logmessagetype to typetelemetry
            /// 2. create new datagridview for telemetetrydata, store telemetry data into a datatable, each column representing a row from page0 and the value corresponding with each datatable entry.
            ///             THINK MORE ABOUT THE STRUCTURE FOR TELEMETRY DataTable/DataGridView implementation
            /// 

            if (PLCGatewayController.PersistentPollingEnabled == true)
            {
                PLCGatewayController.SendPLCGatewayPacket("plc-page-dump -i2");
              
                string newRXData = PLCGatewayController.ReadAndUpdateInputBuffer(); // Assuming telemetry data is posted cyclically, NOT requested.
                await PLCGatewayController.RetrievePLCGatewayPacket();

                if (newRXData != null || newRXData != string.Empty)
                {
                    // do UI update if needed 
                }
            }
        }

        private void ChkPollingEnabled_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                bool isChecked = checkBox.Checked;
                MessageBox.Show($"Polling is now {(isChecked ? "enabled" : "disabled")}");

                if (isChecked)
                {
                    PLCGatewayController.EnablePersistentPolling();
                    UIUpdateTimer.Start();
                }
                else
                {
                    PLCGatewayController.DisablePersistentPolling();
                    UIUpdateTimer.Stop();
                }
            }
        }

        private void ChkTogglePLCGatewayType_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTogglePLCGatewayType.Checked)
            {
                MessageBox.Show("Serial PLCHE Selected!", "CheckBox State");
                PLCGatewayController.UpdatePLCGatewayType(PLCGatewayController.PLCGatewayType.SerialPLCHE);
                chkTogglePLCGatewayType.Text = "Serial PLCHE";
            }
            else
            {
                MessageBox.Show("CP110 Selected!", "CheckBox State");
                PLCGatewayController.UpdatePLCGatewayType(PLCGatewayController.PLCGatewayType.CP110);
                chkTogglePLCGatewayType.Text = "CP110";
            }
        }

        private void BtnInitSerial_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedPortNumberIndex = ComboBoxForCOMPorts.SelectedIndex;
                string selectedPort = ComboBoxForCOMPorts.Items[selectedPortNumberIndex].ToString();
                string portNumber = selectedPort.Substring(3);
                int.TryParse(portNumber, out int selectedPortNumberInt);

                string selectedBaudRate = ComboBoxForBaudRates.Text;
                int.TryParse(selectedBaudRate, out int selectedBaudRateInt);
                PLCGatewayController.InitializePLCGateway(selectedPortNumberInt, selectedBaudRateInt);
            }
            catch (Exception ex)
            {
                LogController.AddLogMessage(new LogMessage(text: $"{ex.ToString()}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }

            if (PLCGatewayController.GatewayInit)
            {
                UnlockSerialControls();
            }
        }

        private void BtnPLCConfiguration_Click(object sender, EventArgs e)
        {
            // need to spawn a new form here
            // cannot block the current thread
            // should exist adjacent to current screen
            // both forms should be editable at once
            // the 2nd form will have to update values in PLCGatewayController so ideally this form will be passed this controller object too

            FormPLCGatewayConfiguration formPLCGatewayConfiguration = new FormPLCGatewayConfiguration(PLCGatewayController, LogController);
            formPLCGatewayConfiguration.Show(); // Open Form2 non-blocking
        }

        private void btnClearSystemLog_Click(object sender, EventArgs e)
        {
            LogController.SystemBaseDataBindingLog.Clear();
        }

        private void UnlockSerialControls()
        {
            if (PLCGatewayController.GatewayInit)
            {
                txtCmdString.Enabled = true;
                txtCmdArgument.Enabled = true;
                chkPollingEnabled.Enabled = true;
                btnSendCmd.Enabled = true;
                chkTogglePLCGatewayType.Enabled = false;
            }
        }

        private void LockSerialControls()
        {
            if (!PLCGatewayController.GatewayInit)
            {
                txtCmdString.Enabled = false;
                txtCmdArgument.Enabled = false;
                chkPollingEnabled.Enabled = false;
                btnSendCmd.Enabled = false;
                chkTogglePLCGatewayType.Enabled = true;
            }
        }

        private void BtnDisposeSerial_Click(object sender, EventArgs e)
        {
            PLCGatewayController.DeinitializePLCGateway();
            LockSerialControls();
        }

        private async void BtnSendCmd_Click(object sender, EventArgs e)
        {
            string cmdString = txtCmdString.Text;
            string cmdArgument = txtCmdArgument.Text;
            string textData = $"{cmdString} {cmdArgument}";
            string[] substringsToCheck = { "get-page", "help" };

            try
            {
                LogController.AddLogMessage(new LogMessage(text: $"Writing '{textData}'", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow, useTimeStamp: true));
                PLCGatewayController.SendPLCGatewayPacket(textData);

                if (substringsToCheck.Any(textData.Contains))
                {
                    Thread.Sleep(3000); // huge delay
                }
                else
                {
                    Thread.Sleep(100); // typical delay
                }

                string rxData = await PLCGatewayController.RetrievePLCGatewayPacket();
            }
            catch (Exception ex)
            {
                LogController.AddLogMessage(new LogMessage(text: $"{ex.ToString()}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }
        }

        private async void BtnScanSerialPorts_Click(object sender, EventArgs e)
        {
            string[] portNames = { };

            try
            {
                LogController.AddLogMessage(new LogMessage(text: $"Scanning Serial Ports", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow, useTimeStamp: true));
                PLCGatewayController.ScanSerialPorts();
            }
            catch (Exception ex)
            {
                LogController.AddLogMessage(new LogMessage(text: $"{ex.ToString()}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow));
            }
        }

        private void AppendToTextbox(TextBox textbox, string message)
        {
            if (textbox.InvokeRequired)
            {
                textbox.Invoke(new Action(() => AppendToTextbox(textbox, message)));
            }
            else
            {
                textbox.AppendText($"{DateTime.Now}: {message}{Environment.NewLine}");
            }
        }

        private void bindingSourceRXData_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void bindingSourceSystemBaseData_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void bindingSourceTelemetryData_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void bindingSourceCOMPorts_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void PLCHESerialMonitorForm_Load(object sender, EventArgs e)
        {

        }
    }
}