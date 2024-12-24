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

        public PLCHESerialMonitorForm()
        {
            InitializeComponent(); // Ensures the designer-generated code runs
            ResizeFormTo80Percent(0.8f);
            CreateDynamicControls(); // Add our dynamic controls
            PLCGatewayController = new PLCGatewayController();
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
            txtCmdString = new TextBox
            {
                Location = new Point(10, 280),
                Size = new Size(350, 25)
            };
            Controls.Add(txtCmdString);

            // === Argument TextBox ===
            txtCmdArgument = new TextBox
            {
                Location = new Point(370, 280),
                Size = new Size(350, 25)
            };
            Controls.Add(txtCmdArgument);

            // === Send Button ===
            var btnSendCmd = new Button
            {
                Text = "Send",
                Location = new Point(730, 280),
                Size = new Size(50, 25)
            };
            btnSendCmd.Click += (sender, args) =>
            MessageBox.Show($"Sending Command: {txtCmdString.Text}, Arg: {txtCmdArgument.Text}");
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

            chkEnableSerial = new CheckBox
            {
                Location = new Point(10, 350), // Adjust position as needed
                Size = new Size(200, 30),     // Adjust size as needed
                Text = "Enable Option",       // Display text
                Checked = false               // Default state
            };

            chkEnableSerial.CheckedChanged += ChkEnableSerial_CheckedChanged;

            Controls.Add(chkEnableSerial);

            // UI Update Timer
            UIUpdateTimer.Interval = 100; // Set interval to 100ms
            UIUpdateTimer.Tick += UIUpdateTimer_Tick; // Add event handler for the Tick event
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