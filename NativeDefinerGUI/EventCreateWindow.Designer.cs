
namespace NativeDefinerGUI
{
    partial class EventCreateWindow
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
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxIdentifier = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCreateEvent = new System.Windows.Forms.Button();
            this.listViewTypes = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewParameters = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAddParam = new System.Windows.Forms.Button();
            this.richTextDescription = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRemoveParam = new System.Windows.Forms.Button();
            this.richTextEventText = new System.Windows.Forms.RichTextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 16);
            this.label2.TabIndex = 20;
            this.label2.Text = "Identifier:";
            // 
            // textBoxIdentifier
            // 
            this.textBoxIdentifier.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.textBoxIdentifier.ForeColor = System.Drawing.Color.White;
            this.textBoxIdentifier.Location = new System.Drawing.Point(15, 28);
            this.textBoxIdentifier.Name = "textBoxIdentifier";
            this.textBoxIdentifier.Size = new System.Drawing.Size(217, 20);
            this.textBoxIdentifier.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(23, 362);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 16);
            this.label1.TabIndex = 19;
            this.label1.Text = "Name:";
            // 
            // btnCreateEvent
            // 
            this.btnCreateEvent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateEvent.BackColor = System.Drawing.Color.Teal;
            this.btnCreateEvent.FlatAppearance.BorderSize = 0;
            this.btnCreateEvent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateEvent.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.btnCreateEvent.ForeColor = System.Drawing.Color.White;
            this.btnCreateEvent.Location = new System.Drawing.Point(852, 472);
            this.btnCreateEvent.Name = "btnCreateEvent";
            this.btnCreateEvent.Size = new System.Drawing.Size(117, 23);
            this.btnCreateEvent.TabIndex = 18;
            this.btnCreateEvent.Text = "Create Event";
            this.btnCreateEvent.UseVisualStyleBackColor = false;
            this.btnCreateEvent.Click += new System.EventHandler(this.btnCreateEvent_Click);
            // 
            // listViewTypes
            // 
            this.listViewTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewTypes.BackColor = System.Drawing.Color.Gray;
            this.listViewTypes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewTypes.HideSelection = false;
            this.listViewTypes.Location = new System.Drawing.Point(560, 54);
            this.listViewTypes.Name = "listViewTypes";
            this.listViewTypes.Size = new System.Drawing.Size(409, 294);
            this.listViewTypes.TabIndex = 21;
            this.listViewTypes.UseCompatibleStateImageBehavior = false;
            this.listViewTypes.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Type";
            this.columnHeader1.Width = 300;
            // 
            // listViewParameters
            // 
            this.listViewParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listViewParameters.BackColor = System.Drawing.Color.Gray;
            this.listViewParameters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listViewParameters.HideSelection = false;
            this.listViewParameters.Location = new System.Drawing.Point(15, 54);
            this.listViewParameters.Name = "listViewParameters";
            this.listViewParameters.Size = new System.Drawing.Size(400, 294);
            this.listViewParameters.TabIndex = 22;
            this.listViewParameters.UseCompatibleStateImageBehavior = false;
            this.listViewParameters.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Parameter";
            this.columnHeader2.Width = 300;
            // 
            // btnAddParam
            // 
            this.btnAddParam.BackColor = System.Drawing.Color.Teal;
            this.btnAddParam.FlatAppearance.BorderSize = 0;
            this.btnAddParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddParam.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.btnAddParam.ForeColor = System.Drawing.Color.White;
            this.btnAddParam.Location = new System.Drawing.Point(421, 112);
            this.btnAddParam.Name = "btnAddParam";
            this.btnAddParam.Size = new System.Drawing.Size(133, 23);
            this.btnAddParam.TabIndex = 23;
            this.btnAddParam.Text = "<- Add Param";
            this.btnAddParam.UseVisualStyleBackColor = false;
            this.btnAddParam.Click += new System.EventHandler(this.btnAddParam_Click);
            // 
            // richTextDescription
            // 
            this.richTextDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.richTextDescription.ForeColor = System.Drawing.Color.White;
            this.richTextDescription.Location = new System.Drawing.Point(655, 381);
            this.richTextDescription.Name = "richTextDescription";
            this.richTextDescription.Size = new System.Drawing.Size(314, 84);
            this.richTextDescription.TabIndex = 24;
            this.richTextDescription.Text = "";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(661, 362);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 16);
            this.label3.TabIndex = 25;
            this.label3.Text = "Description:";
            // 
            // btnRemoveParam
            // 
            this.btnRemoveParam.BackColor = System.Drawing.Color.Teal;
            this.btnRemoveParam.FlatAppearance.BorderSize = 0;
            this.btnRemoveParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveParam.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.btnRemoveParam.ForeColor = System.Drawing.Color.White;
            this.btnRemoveParam.Location = new System.Drawing.Point(421, 171);
            this.btnRemoveParam.Name = "btnRemoveParam";
            this.btnRemoveParam.Size = new System.Drawing.Size(133, 23);
            this.btnRemoveParam.TabIndex = 26;
            this.btnRemoveParam.Text = "Remove Param ->";
            this.btnRemoveParam.UseVisualStyleBackColor = false;
            this.btnRemoveParam.Click += new System.EventHandler(this.btnRemoveParam_Click);
            // 
            // richTextEventText
            // 
            this.richTextEventText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextEventText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.richTextEventText.ForeColor = System.Drawing.Color.White;
            this.richTextEventText.Location = new System.Drawing.Point(264, 381);
            this.richTextEventText.Name = "richTextEventText";
            this.richTextEventText.Size = new System.Drawing.Size(385, 84);
            this.richTextEventText.TabIndex = 27;
            this.richTextEventText.Text = "";
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.textBoxName.ForeColor = System.Drawing.Color.White;
            this.textBoxName.Location = new System.Drawing.Point(15, 381);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(243, 20);
            this.textBoxName.TabIndex = 29;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(270, 362);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 16);
            this.label4.TabIndex = 30;
            this.label4.Text = "Event Text:";
            // 
            // EventCreateWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(981, 507);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.richTextEventText);
            this.Controls.Add(this.btnRemoveParam);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.richTextDescription);
            this.Controls.Add(this.btnAddParam);
            this.Controls.Add(this.listViewParameters);
            this.Controls.Add(this.listViewTypes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxIdentifier);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCreateEvent);
            this.Name = "EventCreateWindow";
            this.Text = "EventCreateWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxIdentifier;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCreateEvent;
        private System.Windows.Forms.ListView listViewTypes;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ListView listViewParameters;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnAddParam;
        private System.Windows.Forms.RichTextBox richTextDescription;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRemoveParam;
        private System.Windows.Forms.RichTextBox richTextEventText;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label4;
    }
}