namespace PLCHESerialDebugger
{
    partial class PLCHESerialMonitorForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            bindingSourceRXData = new BindingSource(components);
            bindingSourceSystemBaseData = new BindingSource(components);
            bindingSourceTelemetryData = new BindingSource(components);
            bindingSourceCOMPorts = new BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)bindingSourceRXData).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceSystemBaseData).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceTelemetryData).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceCOMPorts).BeginInit();
            SuspendLayout();
            // 
            // bindingSourceRXData
            // 
            bindingSourceRXData.CurrentChanged += bindingSourceRXData_CurrentChanged;
            // 
            // bindingSourceSystemBaseData
            // 
            bindingSourceSystemBaseData.CurrentChanged += bindingSourceSystemBaseData_CurrentChanged;
            // 
            // bindingSourceTelemetryData
            // 
            bindingSourceTelemetryData.CurrentChanged += bindingSourceTelemetryData_CurrentChanged;
            // 
            // bindingSourceCOMPorts
            // 
            bindingSourceCOMPorts.CurrentChanged += bindingSourceCOMPorts_CurrentChanged;
            // 
            // PLCHESerialMonitorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Name = "PLCHESerialMonitorForm";
            Text = "Form1";
            Load += PLCHESerialMonitorForm_Load;
            ((System.ComponentModel.ISupportInitialize)bindingSourceRXData).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceSystemBaseData).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceTelemetryData).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSourceCOMPorts).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private BindingSource bindingSourceRXData;
        private BindingSource bindingSourceSystemBaseData;
        private BindingSource bindingSourceTelemetryData;
        private BindingSource bindingSourceCOMPorts;
    }
}
