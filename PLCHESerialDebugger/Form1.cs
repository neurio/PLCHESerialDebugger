using System.ComponentModel;

namespace PLCHESerialDebugger
{
    public partial class PLCHESerialMonitorForm : Form
    {
        public ComboBox ComboBoxForCOMPorts { get; set; }

        public ListBox ListBoxSystemLog { get; set; }

        public ListBox ListBoxTelemetryMonitor { get; set; }

        public TextBox txtCmdString { get; set; }

        public TextBox txtCmdArgument { get; set; }

        public CheckBox chkTogglePLCGatewayType { get; set; }

        public CheckBox chkPollingEnabled { get; set; }

        public Button btnSendCmd { get; set; }

        public System.Windows.Forms.Timer UIUpdateTimer { get; set; } = new System.Windows.Forms.Timer();

        public PLCGatewayController PLCGatewayController { get; set; }

        public LogController LogController { get; set; }

        public PLCHESerialMonitorForm()
        {
            InitializeComponent(); // Ensures the designer-generated code runs
            ResizeFormWindow(0.8f, 0.8f); // if these values change, it impacts where all controls get placed; leave alone for now.
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
            LogController.TelemetryDataBindingLog.ListChanged += TelemetryDataBindingLog_ListChanged;
        }

        public void SerialDataBindingLog_ListChanged(object sender, ListChangedEventArgs e)
        {
            // run nuanced GUI update logic here
        }

        public void SystemBaseDataBindingLog_ListChanged(object sender, ListChangedEventArgs e)
        {
            // run naunced GUI update logic here
        }

        public void TelemetryDataBindingLog_ListChanged(object sender, ListChangedEventArgs e)
        {
            // run naunced GUI update logic here
        }

        public void AttachDataSources()
        {
            // Bind WinForm control to BindingList
            ListBoxSystemLog.DataSource = bindingSourceSystemBaseData;
            ListBoxTelemetryMonitor.DataSource = bindingSourceTelemetryData;

            // Relate BindingList to 'normal' datatypes
            bindingSourceTelemetryData.DataSource = LogController.TelemetryDataBindingLog;
            bindingSourceRXData.DataSource = LogController.SerialDataBindingLog;
            bindingSourceSystemBaseData.DataSource = LogController.SystemBaseDataBindingLog;
        }

        public void ResizeFormWindow(float xScalingRatio, float yScalingRatio)
        {
            var screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            var screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            int formWidth = (int)(screenWidth * xScalingRatio);
            int formHeight = (int)(screenHeight * yScalingRatio);

            this.Size = new Size(formWidth, formHeight);

            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void CreateDynamicControls()
        {
            // === System Transaction Window ===
            var lblSystemLog = CreateLabel("System Log", 0.01f, 0.015f);
            lblSystemLog.Font = new Font("Calibri Light", 16, FontStyle.Bold);
            lblSystemLog.BackColor = Color.Transparent;
            lblSystemLog.BorderStyle = BorderStyle.Fixed3D;
            lblSystemLog.Padding = new Padding(2);
            Controls.Add(lblSystemLog);

            ListBoxSystemLog = CreateListBox(0.01f, 0.05f, 0.98f, 0.35f); // Occupy full width at the top
            ListBoxSystemLog.Font = new Font("Calibri Light", 10);
            Controls.Add(ListBoxSystemLog);

            // === Telemetry Data Window ===
            var lblTelemetry = CreateLabel("Telemetry Data", 0.01f, 0.40f);
            lblTelemetry.Font = new Font("Calibri Light", 16, FontStyle.Bold);
            lblTelemetry.BackColor = Color.Transparent;
            lblTelemetry.BorderStyle = BorderStyle.Fixed3D;
            lblTelemetry.Padding = new Padding(2);
            Controls.Add(lblTelemetry);

            ListBoxTelemetryMonitor = CreateListBox(0.01f, 0.44f, 0.98f, 0.1f); // Full width
            Controls.Add(ListBoxTelemetryMonitor);

            // === Serial Port Configurations ===
            var lblSerialPortConfiguration = CreateLabel("Serial Port Config.", 0.01f, 0.64f);
            lblSerialPortConfiguration.Font = new Font("Calibri Light", 16, FontStyle.Bold);
            lblSerialPortConfiguration.BackColor = Color.Transparent;
            lblSerialPortConfiguration.BorderStyle = BorderStyle.Fixed3D;
            lblSerialPortConfiguration.Padding = new Padding(2);
            Controls.Add(lblSerialPortConfiguration);

            // === Command TextBox ===
            var lblCmdString = CreateLabel("Command String", 0.01f, 0.69f);
            lblCmdString.Font = new Font("Calibri Light", 14, FontStyle.Bold);
            lblCmdString.BackColor = Color.Transparent;
            lblCmdString.BorderStyle = BorderStyle.Fixed3D;
            lblCmdString.Padding = new Padding(2);
            Controls.Add(lblCmdString);

            txtCmdString = CreateTextBox(0.01f, 0.72f, 0.15f, 0.03f); // Reduced width
            txtCmdString.Font = new Font("Calibri Light", 12);
            Controls.Add(txtCmdString);

            // === Argument TextBox ===
            var lblCmdArgument = CreateLabel("Argument String", 0.16f, 0.69f);
            lblCmdArgument.Font = new Font("Calibri Light", 14, FontStyle.Bold);
            lblCmdArgument.BackColor = Color.Transparent;
            lblCmdArgument.BorderStyle = BorderStyle.Fixed3D;
            lblCmdArgument.Padding = new Padding(2);
            Controls.Add(lblCmdArgument);

            txtCmdArgument = CreateTextBox(0.16f, 0.72f, 0.15f, 0.03f); // Reduced width
            txtCmdArgument.Font = new Font("Calibri Light", 12);
            Controls.Add(txtCmdArgument);

            // === Send Button ===
            btnSendCmd = CreateButton("Send", 0.31f, 0.718f, 0.04f, 0.03f);
            btnSendCmd.Font = new Font("Calibri Light", 12);
            btnSendCmd.Click += BtnSendCmd_Click;
            Controls.Add(btnSendCmd);

            // === Status Panels ===
            var lblStatusWord1 = CreateLabel("Status Word 1", 0.01f, 0.56f);
            lblStatusWord1.Font = new Font("Calibri Light", 16, FontStyle.Bold);
            lblStatusWord1.BackColor = Color.Transparent;
            lblStatusWord1.BorderStyle = BorderStyle.Fixed3D;
            lblStatusWord1.Padding = new Padding(2);
            Controls.Add(lblStatusWord1);
            Controls.Add(CreateStatusPanel("Status Word 1", 0.01f, 0.60f, 0.48f, 0.035f)); // Reduced height for compactness

            var lblStatusWord2 = CreateLabel("Status Word 2", 0.51f, 0.56f);
            lblStatusWord2.Font = new Font("Calibri Light", 16, FontStyle.Bold);
            lblStatusWord2.BackColor = Color.Transparent;
            lblStatusWord2.BorderStyle = BorderStyle.Fixed3D;
            lblStatusWord2.Padding = new Padding(2);
            Controls.Add(lblStatusWord2);
            Controls.Add(CreateStatusPanel("Status Word 2", 0.51f, 0.60f, 0.48f, 0.035f)); // Reduced height for compactness

            // === COM Port Selections ===
            var lblCOMPorts = CreateLabel("COM Port #", 0.25f, 0.64f);
            lblCOMPorts.Font = new Font("Calibri Light", 14, FontStyle.Bold);
            lblCOMPorts.BackColor = Color.Transparent;
            lblCOMPorts.BorderStyle = BorderStyle.Fixed3D;
            lblCOMPorts.Padding = new Padding(2);
            Controls.Add(lblCOMPorts);

            ComboBoxForCOMPorts = CreateComboBox(0.25f, 0.67f, 0.05f, 0.1f);
            Controls.Add(ComboBoxForCOMPorts);

            // === Polling Checkbox ===
            chkPollingEnabled = CreateCheckBox("Enable Polling", 0.11f, 0.63f, 0.1f, 0.04f); // Adjusted position
            chkPollingEnabled.Font = new Font("Calibri Light", 12, FontStyle.Bold);
            chkPollingEnabled.CheckedChanged += ChkPollingEnabled_CheckedChanged;
            Controls.Add(chkPollingEnabled);

            // === Init Serial Port Button ===
            var btnInitSerial = CreateButton("Initialize Serial", 0.01f, 0.75f, 0.1f, 0.04f);
            btnInitSerial.Click += BtnInitSerial_Click;
            btnInitSerial.Font = new Font("Calibri Light", 14);
            Controls.Add(btnInitSerial);

            // === Dispose Serial Port Button ===
            var btnDisposeSerial = CreateButton("Dispose Serial", 0.11f, 0.75f, 0.1f, 0.04f);
            btnDisposeSerial.Click += BtnDisposeSerial_Click;
            btnDisposeSerial.Font = new Font("Calibri Light", 14);
            Controls.Add(btnDisposeSerial);

            // === Enable Serial Checkbox ===
            chkTogglePLCGatewayType = CreateCheckBox("Serial Comm.", 0.11f, 0.66f, 0.2f, 0.04f); // Placed closer to other controls
            chkTogglePLCGatewayType.Font = new Font("Calibri Light", 12, FontStyle.Bold);
            chkTogglePLCGatewayType.CheckedChanged += ChkTogglePLCGatewayType_CheckedChanged;
            Controls.Add(chkTogglePLCGatewayType);

            // UI Update Timer
            UIUpdateTimer.Interval = 100; // Set interval to 100ms
            UIUpdateTimer.Tick += UIUpdateTimer_Tick; // Add event handler for the Tick event
        }

        private ComboBox CreateComboBox(float xRatio, float yRatio, float widthRatio, float heightRatio)
        {
            var comboBox = new ComboBox
            {
                Location = new Point((int)(this.ClientSize.Width * xRatio), (int)(this.ClientSize.Height * yRatio)),
                Size = new Size((int)(this.ClientSize.Width * widthRatio), (int)(this.ClientSize.Height * heightRatio)),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.WhiteSmoke
            };

            return comboBox;
        }

        private void PopulateComboBox(ComboBox comboBox, IEnumerable<string> items)
        {
            comboBox.Items.Clear(); // Clear any existing items
            foreach (var item in items)
            {
                comboBox.Items.Add(item);
            }

            // Set default selected index if items exist
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }


        private ListBox CreateListBox(float xRatio, float yRatio, float widthRatio, float heightRatio)
        {
            var listBox = new ListBox
            {
                Location = new Point((int)(this.ClientSize.Width * xRatio), (int)(this.ClientSize.Height * yRatio)),
                Size = new Size((int)(this.ClientSize.Width * widthRatio), (int)(this.ClientSize.Height * heightRatio)),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };

            // Enable scrolling if items exceed visible area
            listBox.HorizontalScrollbar = true;

            return listBox;
        }


        private TextBox CreateTextBox(float xRatio, float yRatio, float widthRatio, float heightRatio)
        {
            var textBox = new TextBox
            {
                Location = new Point((int)(this.ClientSize.Width * xRatio), (int)(this.ClientSize.Height * yRatio)),
                Size = new Size((int)(this.ClientSize.Width * widthRatio), (int)(this.ClientSize.Height * heightRatio)),
                Multiline = false,  // Ensure it's single-line and prevent scrollbars
                ScrollBars = ScrollBars.None,  // Disable scrollbars
            };

            return textBox;
        }


        private Button CreateButton(string text, float xRatio, float yRatio, float widthRatio, float heightRatio)
        {
            var button = new Button
            {
                Text = text,
                Location = new Point((int)(this.ClientSize.Width * xRatio), (int)(this.ClientSize.Height * yRatio)),
                Size = new Size((int)(this.ClientSize.Width * widthRatio), (int)(this.ClientSize.Height * heightRatio))
            };
            return button;
        }

        private CheckBox CreateCheckBox(string text, float xRatio, float yRatio, float widthRatio, float heightRatio)
        {
            var chkBox = new CheckBox
            {
                Text = text,
                Location = new Point((int)(this.ClientSize.Width * xRatio), (int)(this.ClientSize.Height * yRatio)),
                Size = new Size((int)(this.ClientSize.Width * widthRatio), (int)(this.ClientSize.Height * heightRatio))
            };
            return chkBox;
        }

        private Panel CreateStatusPanel(string label, float xRatio, float yRatio, float widthRatio, float heightRatio)
        {
            var panel = new Panel
            {
                Location = new Point((int)(this.ClientSize.Width * xRatio), (int)(this.ClientSize.Height * yRatio)),
                Size = new Size((int)(this.ClientSize.Width * widthRatio), (int)(this.ClientSize.Height * heightRatio)),
                BorderStyle = BorderStyle.FixedSingle,
            };

            var lblStatus = new Label
            {
                Text = label,
                Location = new Point(10, 10),
                Size = new Size(panel.Width - 20, 20)
            };
            panel.Controls.Add(lblStatus);

            // Add more controls or placeholders inside this panel
            // Adjust sizes to match the content

            return panel;
        }




        private Label CreateLabel(string text, float xRatio, float yRatio)
        {
            var lbl = new Label
            {
                Text = text,
                Location = new Point((int)(this.ClientSize.Width * xRatio), (int)(this.ClientSize.Height * yRatio)),
                AutoSize = true
            };
            return lbl;
        }

        // Timer Tick event handler
        private void UIUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (PLCGatewayController.PersistentPollingEnabled == true)
            {
                string newRXData = PLCGatewayController.ReadAndUpdateInputBuffer(); // should only write new entries

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
            PLCGatewayController.InitializePLCGateway();
            UnlockSerialControls();

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

            try
            {
                LogController.AddLogMessage(new LogMessage(text: $"Writing {textData}", messageType: LogMessage.messageType.Base, timeStamp: DateTime.UtcNow, useTimeStamp: true));
                PLCGatewayController.SendPLCGatewayPacket(textData);
                Thread.Sleep(100);

                string rxData = await PLCGatewayController.RetrievePLCGatewayPacket();

                //PLCGatewayController.RetrievePLCGatewayPacket();
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
    }
}