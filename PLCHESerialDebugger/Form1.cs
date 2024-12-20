using System;
using System.Windows.Forms;

namespace PLCHESerialDebugger
{
    public partial class PLCHESerialMonitorForm : Form
    {
        public PLCHESerialMonitorForm()
        {
            InitializeComponent(); // Ensures the designer-generated code runs
            CreateDynamicControls(); // Add our dynamic controls
        }

        private void CreateDynamicControls()
        {
            // === Written Serial Data Window ===
            var txtMonitorWrite = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(350, 150),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.WhiteSmoke
            };
            Controls.Add(txtMonitorWrite);

            // === Read Serial Data Window ===
            var txtMonitorRead = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new System.Drawing.Point(370, 10),
                Size = new System.Drawing.Size(350, 150),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.WhiteSmoke
            };
            Controls.Add(txtMonitorRead);

            // === Telemetry Data Window ===
            var txtTelemetry = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new System.Drawing.Point(10, 170),
                Size = new System.Drawing.Size(710, 100),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.WhiteSmoke
            };
            Controls.Add(txtTelemetry);

            // === Command TextBox ===
            var txtCmdString = new TextBox
            {
                Location = new System.Drawing.Point(10, 280),
                Size = new System.Drawing.Size(350, 25)
            };
            Controls.Add(txtCmdString);

            // === Argument TextBox ===
            var txtCmdArg = new TextBox
            {
                Location = new System.Drawing.Point(370, 280),
                Size = new System.Drawing.Size(350, 25)
            };
            Controls.Add(txtCmdArg);

            // === Send Button ===
            var btnSendCmd = new Button
            {
                Text = "Send",
                Location = new System.Drawing.Point(730, 280),
                Size = new System.Drawing.Size(50, 25)
            };
            btnSendCmd.Click += (sender, args) =>
                MessageBox.Show($"Sending Command: {txtCmdString.Text}, Arg: {txtCmdArg.Text}");
            Controls.Add(btnSendCmd);

            // === Status Panels ===
            Controls.Add(CreateStatusPanel("Status Word 1", 10, 320));
            Controls.Add(CreateStatusPanel("Status Word 2", 370, 320));

            // === Check Polling Checkbox ===
            var chkPollingEnabled = new CheckBox
            {
                Text = "Enable Polling",
                Location = new System.Drawing.Point(10, 500),
                Size = new System.Drawing.Size(150, 25)
            };
            Controls.Add(chkPollingEnabled);

            // === Init Serial Port Button ===
            var btnInitSerial = new Button
            {
                Text = "Init Serial",
                Location = new System.Drawing.Point(200, 500),
                Size = new System.Drawing.Size(100, 30)
            };
            btnInitSerial.Click += (sender, args) =>
                MessageBox.Show("Serial port initialized.");
            Controls.Add(btnInitSerial);

            // === Dispose Serial Port Button ===
            var btnDisposeSerial = new Button
            {
                Text = "Dispose Serial",
                Location = new System.Drawing.Point(320, 500),
                Size = new System.Drawing.Size(100, 30)
            };
            btnDisposeSerial.Click += (sender, args) =>
                MessageBox.Show("Serial port disposed.");
            Controls.Add(btnDisposeSerial);
        }

        private Panel CreateStatusPanel(string labelText, int x, int y)
        {
            var panel = new Panel
            {
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(350, 150),
                BorderStyle = BorderStyle.FixedSingle
            };

            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };

            panel.Controls.Add(label);

            // Placeholder for status bits
            for (int i = 0; i < 32; i++)
            {
                var bitLabel = new Label
                {
                    Text = $"Bit {i}: --",
                    AutoSize = true,
                    Location = new System.Drawing.Point(10, 20 + (i * 20))
                };
                panel.Controls.Add(bitLabel);
            }

            return panel;
        }
    }
}
