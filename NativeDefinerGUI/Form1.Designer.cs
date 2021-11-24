
namespace NativeDefinerGUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageType = new System.Windows.Forms.TabPage();
            this.tabEvents = new System.Windows.Forms.TabPage();
            this.tabParameters = new System.Windows.Forms.TabPage();
            this.tabPageConstants = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPageType);
            this.tabControl.Controls.Add(this.tabEvents);
            this.tabControl.Controls.Add(this.tabParameters);
            this.tabControl.Controls.Add(this.tabPageConstants);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(467, 426);
            this.tabControl.TabIndex = 8;
            // 
            // tabPageType
            // 
            this.tabPageType.Location = new System.Drawing.Point(4, 22);
            this.tabPageType.Name = "tabPageType";
            this.tabPageType.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageType.Size = new System.Drawing.Size(459, 400);
            this.tabPageType.TabIndex = 0;
            this.tabPageType.Text = "Types";
            this.tabPageType.UseVisualStyleBackColor = true;
            // 
            // tabEvents
            // 
            this.tabEvents.Location = new System.Drawing.Point(4, 22);
            this.tabEvents.Name = "tabEvents";
            this.tabEvents.Padding = new System.Windows.Forms.Padding(3);
            this.tabEvents.Size = new System.Drawing.Size(459, 400);
            this.tabEvents.TabIndex = 1;
            this.tabEvents.Text = "Events";
            this.tabEvents.UseVisualStyleBackColor = true;
            // 
            // tabParameters
            // 
            this.tabParameters.Location = new System.Drawing.Point(4, 22);
            this.tabParameters.Name = "tabParameters";
            this.tabParameters.Size = new System.Drawing.Size(459, 400);
            this.tabParameters.TabIndex = 2;
            this.tabParameters.Text = "Functions";
            this.tabParameters.UseVisualStyleBackColor = true;
            // 
            // tabPageConstants
            // 
            this.tabPageConstants.Location = new System.Drawing.Point(4, 22);
            this.tabPageConstants.Name = "tabPageConstants";
            this.tabPageConstants.Size = new System.Drawing.Size(459, 400);
            this.tabPageConstants.TabIndex = 3;
            this.tabPageConstants.Text = "Constants";
            this.tabPageConstants.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(491, 450);
            this.Controls.Add(this.tabControl);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageType;
        private System.Windows.Forms.TabPage tabEvents;
        private System.Windows.Forms.TabPage tabParameters;
        private System.Windows.Forms.TabPage tabPageConstants;
    }
}

