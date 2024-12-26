namespace PLCHESerialDebugger
{
    public partial class PLCHESerialMonitorForm : Form
    {
        public TextBox txtMonitorWrite { get; set; }

        public TextBox txtMonitorRead { get; set; }

        public TextBox txtTelemetry { get; set; }

        public TextBox txtCmdString { get; set; }

        public TextBox txtCmdArgument { get; set; }

        public CheckBox chkEnableSerial { get; set; }

        public System.Windows.Forms.Timer UIUpdateTimer { get; set; } = new System.Windows.Forms.Timer();

        public PLCGatewayController PLCGatewayController { get; set; }

        public LogController LogController { get; set; }

        public PLCHESerialMonitorForm()
        {
            InitializeComponent(); // Ensures the designer-generated code runs
            ResizeFormTo80Percent(0.8f);
            CreateDynamicControls(); // Add our dynamic controls
            LogController = new LogController();
            PLCGatewayController = new PLCGatewayController(LogController);
        }

        public void ResizeFormTo80Percent(float scalingRatio)
        {
            var screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            var screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            int formWidth = (int)(screenWidth * scalingRatio);
            int formHeight = (int)(screenHeight * scalingRatio);

            this.Size = new Size(formWidth, formHeight);

            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void CreateDynamicControls()
        {
            // === Written Serial Data Window ===
            var lblMonitorWrite = CreateLabel("Written Data", 0.01f, 0.01f);
            Controls.Add(lblMonitorWrite);
            txtMonitorWrite = CreateTextBox(0.01f, 0.06f, 0.35f, 0.15f);
            Controls.Add(txtMonitorWrite);

            // === Read Serial Data Window ===
            var lblMonitorRead = CreateLabel("Read Data", 0.37f, 0.01f);
            Controls.Add(lblMonitorRead);
            txtMonitorRead = CreateTextBox(0.37f, 0.06f, 0.35f, 0.15f);
            Controls.Add(txtMonitorRead);

            // === Telemetry Data Window ===
            var lblTelemetry = CreateLabel("Telemetry Data", 0.01f, 0.17f);
            Controls.Add(lblTelemetry);
            txtTelemetry = CreateTextBox(0.01f, 0.22f, 0.7f, 0.1f);
            Controls.Add(txtTelemetry);

            // === Command TextBox ===
            var lblCmdString = CreateLabel("Command String", 0.01f, 0.33f);
            Controls.Add(lblCmdString);
            txtCmdString = CreateTextBox(0.01f, 0.37f, 0.35f, 0.03f);
            Controls.Add(txtCmdString);

            // === Argument TextBox ===
            var lblCmdArgument = CreateLabel("Argument String", 0.37f, 0.33f);
            Controls.Add(lblCmdArgument);
            txtCmdArgument = CreateTextBox(0.37f, 0.37f, 0.35f, 0.03f);
            Controls.Add(txtCmdArgument);

            // === Send Button ===
            var btnSendCmd = CreateButton("Send", 0.73f, 0.37f, 0.07f, 0.03f);
            btnSendCmd.Click += BtnSendCmd_Click;
            Controls.Add(btnSendCmd);

            // === Status Panels ===
            var lblStatusWord1 = CreateLabel("Status Word 1", 0.01f, 0.45f);
            Controls.Add(lblStatusWord1);
            Controls.Add(CreateStatusPanel("Status Word 1", 0.01f, 0.5f));

            var lblStatusWord2 = CreateLabel("Status Word 2", 0.37f, 0.45f);
            Controls.Add(lblStatusWord2);
            Controls.Add(CreateStatusPanel("Status Word 2", 0.37f, 0.5f));

            // === Polling Checkbox ===
            var lblPollingEnabled = CreateLabel("Polling Active", 0.01f, 0.65f);
            Controls.Add(lblPollingEnabled);
            var chkPollingEnabled = CreateCheckBox("Enable Polling", 0.01f, 0.69f, 0.15f, 0.03f);
            chkPollingEnabled.CheckedChanged += ChkPollingEnabled_CheckedChanged;
            Controls.Add(chkPollingEnabled);

            // === Init Serial Port Button ===
            var lblInitSerial = CreateLabel("Initialize Serial Port", 0.2f, 0.65f);
            Controls.Add(lblInitSerial);
            var btnInitSerial = CreateButton("Init Serial", 0.2f, 0.69f, 0.1f, 0.04f);
            btnInitSerial.Click += BtnInitSerial_Click;
            Controls.Add(btnInitSerial);

            // === Dispose Serial Port Button ===
            var lblDisposeSerial = CreateLabel("Dispose Serial Port", 0.32f, 0.65f);
            Controls.Add(lblDisposeSerial);
            var btnDisposeSerial = CreateButton("Dispose Serial", 0.32f, 0.69f, 0.1f, 0.04f);
            btnDisposeSerial.Click += BtnDisposeSerial_Click;
            Controls.Add(btnDisposeSerial);

            // === Enable Serial Checkbox ===
            chkEnableSerial = CreateCheckBox("Serial Comm.", 0.01f, 0.82f, 0.2f, 0.04f);
            chkEnableSerial.CheckedChanged += ChkEnableSerial_CheckedChanged;
            Controls.Add(chkEnableSerial);

            // UI Update Timer
            UIUpdateTimer.Interval = 100; // Set interval to 100ms
            UIUpdateTimer.Tick += UIUpdateTimer_Tick; // Add event handler for the Tick event
        }

        private TextBox CreateTextBox(float xRatio, float yRatio, float widthRatio, float heightRatio)
        {
            var txtBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point((int)(this.ClientSize.Width * xRatio), (int)(this.ClientSize.Height * yRatio)),
                Size = new Size((int)(this.ClientSize.Width * widthRatio), (int)(this.ClientSize.Height * heightRatio)),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };
            return txtBox;
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

        private Panel CreateStatusPanel(string label, float xRatio, float yRatio)
        {
            var panel = new Panel
            {
                Location = new Point((int)(this.ClientSize.Width * xRatio), (int)(this.ClientSize.Height * yRatio)),
                Size = new Size((int)(this.ClientSize.Width * 0.35f), (int)(this.ClientSize.Height * 0.15f)),
                BorderStyle = BorderStyle.FixedSingle,
            };

            var lblStatus = new Label
            {
                Text = label,
                Location = new Point(10, 10),
                Size = new Size(panel.Width - 20, 20)
            };
            panel.Controls.Add(lblStatus);

            // You can add more controls (like Bit Fields) here

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
                    AppendToTextbox(txtMonitorRead, newRXData);
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

        private void ChkEnableSerial_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnableSerial.Checked)
            {
                MessageBox.Show("Serial PLCHE Selected!", "CheckBox State");
                PLCGatewayController.UpdatePLCGatewayType(PLCGatewayController.PLCGatewayType.SerialPLCHE);
            }
            else
            {
                MessageBox.Show("CP110 Selected!", "CheckBox State");
                PLCGatewayController.UpdatePLCGatewayType(PLCGatewayController.PLCGatewayType.CP110);
            }
        }

        private void BtnInitSerial_Click(object sender, EventArgs e)
        {
            PLCGatewayController.InitializePLCGateway();
        }

        private void BtnDisposeSerial_Click(object sender, EventArgs e)
        {
            PLCGatewayController.DeinitializePLCGateway();
        }

        private void BtnSendCmd_Click(object sender, EventArgs e)
        {
            string cmdString = txtCmdArgument.Text;
            string cmdArgument = txtCmdArgument.Text;
            string textData = $"{cmdString} {cmdArgument}";
            PLCGatewayController.SendPLCGatewayPacket(textData);
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

        public void UpdateTxData(string data)
        {
            AppendToTextbox(txtMonitorWrite, data);
            // should utilize data binding to populate textbox
        }

        public void UpdateRxData(string data)
        {
            AppendToTextbox(txtMonitorRead, data);
            // should utilize data binding to populate textbox
        }

        public void UpdateTelemetryData(string parameter, string value)
        {
            AppendToTextbox(txtTelemetry, $"{parameter}: {value}");
            // should utilize data binding to populate textbox
        }
    }
}