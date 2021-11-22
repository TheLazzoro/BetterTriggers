
namespace NativeDefinerGUI
{
    partial class EventControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCreateEvent = new System.Windows.Forms.Button();
            this.listViewEvents = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // btnCreateEvent
            // 
            this.btnCreateEvent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateEvent.BackColor = System.Drawing.Color.Teal;
            this.btnCreateEvent.FlatAppearance.BorderSize = 0;
            this.btnCreateEvent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateEvent.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.btnCreateEvent.ForeColor = System.Drawing.Color.White;
            this.btnCreateEvent.Location = new System.Drawing.Point(356, 442);
            this.btnCreateEvent.Name = "btnCreateEvent";
            this.btnCreateEvent.Size = new System.Drawing.Size(117, 23);
            this.btnCreateEvent.TabIndex = 15;
            this.btnCreateEvent.Text = "Create Event";
            this.btnCreateEvent.UseVisualStyleBackColor = false;
            this.btnCreateEvent.Click += new System.EventHandler(this.btnCreateEvent_Click);
            // 
            // listViewEvents
            // 
            this.listViewEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewEvents.BackColor = System.Drawing.Color.Gray;
            this.listViewEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listViewEvents.HideSelection = false;
            this.listViewEvents.Location = new System.Drawing.Point(3, 3);
            this.listViewEvents.Name = "listViewEvents";
            this.listViewEvents.Size = new System.Drawing.Size(470, 433);
            this.listViewEvents.TabIndex = 18;
            this.listViewEvents.UseCompatibleStateImageBehavior = false;
            this.listViewEvents.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Event";
            this.columnHeader2.Width = 300;
            // 
            // EventControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.listViewEvents);
            this.Controls.Add(this.btnCreateEvent);
            this.Name = "EventControl";
            this.Size = new System.Drawing.Size(476, 468);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCreateEvent;
        private System.Windows.Forms.ListView listViewEvents;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}
