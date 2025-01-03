namespace PLCHESerialDebugger.Utilities
{
    public static class FormUtilities
    {
        public static ComboBox CreateComboBox(float xRatio, float yRatio, float widthRatio, float heightRatio, Form form)
        {
            var comboBox = new ComboBox
            {
                Location = new Point((int)(form.ClientSize.Width * xRatio), (int)(form.ClientSize.Height * yRatio)),
                Size = new Size((int)(form.ClientSize.Width * widthRatio), (int)(form.ClientSize.Height * heightRatio)),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.WhiteSmoke
            };

            return comboBox;
        }

        public static void PopulateComboBox(ComboBox comboBox, IEnumerable<string> items)
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

        public static ListBox CreateListBox(float xRatio, float yRatio, float widthRatio, float heightRatio, Form form)
        {
            var listBox = new ListBox
            {
                Location = new Point((int)(form.ClientSize.Width * xRatio), (int)(form.ClientSize.Height * yRatio)),
                Size = new Size((int)(form.ClientSize.Width * widthRatio), (int)(form.ClientSize.Height * heightRatio)),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };

            // Enable scrolling if items exceed visible area
            listBox.HorizontalScrollbar = true;

            return listBox;
        }

        public static TextBox CreateTextBox(float xRatio, float yRatio, float widthRatio, float heightRatio, Form form)
        {
            var textBox = new TextBox
            {
                Location = new Point((int)(form.ClientSize.Width * xRatio), (int)(form.ClientSize.Height * yRatio)),
                Size = new Size((int)(form.ClientSize.Width * widthRatio), (int)(form.ClientSize.Height * heightRatio)),
                Multiline = false,  // Ensure it's single-line and prevent scrollbars
                ScrollBars = ScrollBars.None,  // Disable scrollbars
            };

            return textBox;
        }
        
        public static Button CreateButton(string text, float xRatio, float yRatio, float widthRatio, float heightRatio, Form form)
        {
            var button = new Button
            {
                Text = text,
                Location = new Point((int)(form.ClientSize.Width * xRatio), (int)(form.ClientSize.Height * yRatio)),
                Size = new Size((int)(form.ClientSize.Width * widthRatio), (int)(form.ClientSize.Height * heightRatio))
            };
            return button;
        }
        
        public static CheckBox CreateCheckBox(string text, float xRatio, float yRatio, float widthRatio, float heightRatio, Form form)
        {
            var chkBox = new CheckBox
            {
                Text = text,
                Location = new Point((int)(form.ClientSize.Width * xRatio), (int)(form.ClientSize.Height * yRatio)),
                Size = new Size((int)(form.ClientSize.Width * widthRatio), (int)(form.ClientSize.Height * heightRatio))
            };
            return chkBox;
        }
        
        public static Panel CreateStatusPanel(string label, float xRatio, float yRatio, float widthRatio, float heightRatio, Form form)
        {
            var panel = new Panel
            {
                Location = new Point((int)(form.ClientSize.Width * xRatio), (int)(form.ClientSize.Height * yRatio)),
                Size = new Size((int)(form.ClientSize.Width * widthRatio), (int)(form.ClientSize.Height * heightRatio)),
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
        
        public static Label CreateLabel(string text, float xRatio, float yRatio, Form form)
        {
            var lbl = new Label
            {
                Text = text,
                Location = new Point((int)(form.ClientSize.Width * xRatio), (int)(form.ClientSize.Height * yRatio)),
                AutoSize = true
            };
            return lbl;
        }

        public static void ResizeFormWindow(float xScalingRatio, float yScalingRatio, Form form)
        {
            var screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            var screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            int formWidth = (int)(screenWidth * xScalingRatio);
            int formHeight = (int)(screenHeight * yScalingRatio);

            form.Size = new Size(formWidth, formHeight);
            form.StartPosition = FormStartPosition.CenterScreen;
        }
    }
}