namespace PLCHESerialDebugger
{
    public partial class PLCHESerialMonitorForm : Form
    {
        private TextBox txtMonitorWrite { get; set; }

        private TextBox txtMonitorRead { get; set; }

        private TextBox txtTelemetry { get; set; }

        public PLCGatewayController PLCGatewayController { get; set; }

        public PLCHESerialMonitorForm()
        {
            InitializeComponent(); // Ensures the designer-generated code runs
            CreateDynamicControls(); // Add our dynamic controls
        }

        private void CreateDynamicControls()
        {
            // === Written Serial Data Window ===
            txtMonitorWrite = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(10, 10),
                Size = new Size(350, 150),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };
            Controls.Add(txtMonitorWrite);

            // === Read Serial Data Window ===
            txtMonitorRead = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(370, 10),
                Size = new Size(350, 150),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };
            Controls.Add(txtMonitorRead);

            // === Telemetry Data Window ===
            txtTelemetry = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(10, 170),
                Size = new Size(710, 100),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };
            Controls.Add(txtTelemetry);

            // === Command TextBox ===
            var txtCmdString = new TextBox
            {
                Location = new Point(10, 280),
                Size = new Size(350, 25)
            };
            Controls.Add(txtCmdString);

            // === Argument TextBox ===
            var txtCmdArg = new TextBox
            {
                Location = new Point(370, 280),
                Size = new Size(350, 25)
            };
            Controls.Add(txtCmdArg);

            // === Send Button ===
            var btnSendCmd = new Button
            {
                Text = "Send",
                Location = new Point(730, 280),
                Size = new Size(50, 25)
            };
            btnSendCmd.Click += (sender, args) =>
            MessageBox.Show($"Sending Command: {txtCmdString.Text}, Arg: {txtCmdArg.Text}");
            btnSendCmd.Click += BtnSendCmd_Click;
            Controls.Add(btnSendCmd);

            // === Status Panels ===
            Controls.Add(CreateStatusPanel("Status Word 1", 10, 320));
            Controls.Add(CreateStatusPanel("Status Word 2", 370, 320));

            // === Check Polling Checkbox ===
            var chkPollingEnabled = new CheckBox
            {
                Text = "Enable Polling",
                Location = new Point(10, 500),
                Size = new Size(150, 25)
            };
            chkPollingEnabled.CheckedChanged += ChkPollingEnabled_CheckedChanged;
            Controls.Add(chkPollingEnabled);

            // === Init Serial Port Button ===
            var btnInitSerial = new Button
            {
                Text = "Init Serial",
                Location = new Point(200, 500),
                Size = new Size(100, 30)
            };
            btnInitSerial.Click += (sender, args) =>
            MessageBox.Show("Serial port initialized.");
            btnInitSerial.Click += BtnInitSerial_Click;
            Controls.Add(btnInitSerial);

            // === Dispose Serial Port Button ===
            var btnDisposeSerial = new Button
            {
                Text = "Dispose Serial",
                Location = new Point(320, 500),
                Size = new Size(100, 30)
            };
            btnDisposeSerial.Click += (sender, args) =>
            MessageBox.Show("Serial port disposed.");
            btnDisposeSerial.Click += BtnDisposeSerial_Click;

            Controls.Add(btnDisposeSerial);
        }

        private Panel CreateStatusPanel(string labelText, int x, int y)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(350, 150),
                BorderStyle = BorderStyle.FixedSingle
            };

            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            panel.Controls.Add(label);

            // Placeholder for status bits
            for (int i = 0; i < 32; i++)
            {
                var bitLabel = new Label
                {
                    Text = $"Bit {i}: --",
                    AutoSize = true,
                    Location = new Point(10, 20 + (i * 20))
                };
                panel.Controls.Add(bitLabel);
            }

            return panel;
        }

        // move this stuff into a different file since 
        private void ChkPollingEnabled_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                bool isChecked = checkBox.Checked;
                MessageBox.Show($"Polling is now {(isChecked ? "enabled" : "disabled")}");
                // Add logic to enable/disable the serial polling thread
            }
        }

        private void BtnInitSerial_Click(object sender, EventArgs e)
        {
            // Logic to initialize serial port
        }

        private void BtnDisposeSerial_Click(object sender, EventArgs e)
        {
            // Logic to dispose of serial port
        }

        private void BtnSendCmd_Click(object sender, EventArgs e)
        {
            // Logic to send command
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