
namespace NativeDefinerGUI
{
    partial class ConstantControl
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
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxIdentifier = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listViewConstants = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCreateConstant = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.listViewTypes = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAddReturnType = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lblReturnType = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(3, 249);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 16);
            this.label2.TabIndex = 15;
            this.label2.Text = "Identifier:";
            // 
            // textBoxIdentifier
            // 
            this.textBoxIdentifier.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxIdentifier.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.textBoxIdentifier.ForeColor = System.Drawing.Color.White;
            this.textBoxIdentifier.Location = new System.Drawing.Point(6, 268);
            this.textBoxIdentifier.Name = "textBoxIdentifier";
            this.textBoxIdentifier.Size = new System.Drawing.Size(217, 20);
            this.textBoxIdentifier.TabIndex = 8;
            this.textBoxIdentifier.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxIdentifier_KeyDown);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 300);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 16);
            this.label1.TabIndex = 13;
            this.label1.Text = "Name:";
            // 
            // listViewConstants
            // 
            this.listViewConstants.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewConstants.BackColor = System.Drawing.Color.Gray;
            this.listViewConstants.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewConstants.HideSelection = false;
            this.listViewConstants.Location = new System.Drawing.Point(0, 0);
            this.listViewConstants.Name = "listViewConstants";
            this.listViewConstants.Size = new System.Drawing.Size(305, 237);
            this.listViewConstants.TabIndex = 14;
            this.listViewConstants.UseCompatibleStateImageBehavior = false;
            this.listViewConstants.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Constants";
            this.columnHeader1.Width = 300;
            // 
            // btnCreateConstant
            // 
            this.btnCreateConstant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateConstant.BackColor = System.Drawing.Color.Teal;
            this.btnCreateConstant.Enabled = false;
            this.btnCreateConstant.FlatAppearance.BorderSize = 0;
            this.btnCreateConstant.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateConstant.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.btnCreateConstant.ForeColor = System.Drawing.Color.White;
            this.btnCreateConstant.Location = new System.Drawing.Point(492, 370);
            this.btnCreateConstant.Name = "btnCreateConstant";
            this.btnCreateConstant.Size = new System.Drawing.Size(117, 23);
            this.btnCreateConstant.TabIndex = 10;
            this.btnCreateConstant.Text = "Create Constant";
            this.btnCreateConstant.UseVisualStyleBackColor = false;
            this.btnCreateConstant.Click += new System.EventHandler(this.btnCreateConstant_Click);
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.textBoxName.ForeColor = System.Drawing.Color.White;
            this.textBoxName.Location = new System.Drawing.Point(6, 319);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(216, 20);
            this.textBoxName.TabIndex = 9;
            this.textBoxName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxName_KeyDown);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.BackColor = System.Drawing.Color.Teal;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(255, 420);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(117, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "Create Constant";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(33, 422);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(216, 20);
            this.textBox1.TabIndex = 11;
            // 
            // listViewTypes
            // 
            this.listViewTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewTypes.BackColor = System.Drawing.Color.Gray;
            this.listViewTypes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listViewTypes.HideSelection = false;
            this.listViewTypes.Location = new System.Drawing.Point(307, 0);
            this.listViewTypes.Name = "listViewTypes";
            this.listViewTypes.Size = new System.Drawing.Size(305, 237);
            this.listViewTypes.TabIndex = 16;
            this.listViewTypes.UseCompatibleStateImageBehavior = false;
            this.listViewTypes.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Types";
            this.columnHeader2.Width = 300;
            // 
            // btnAddReturnType
            // 
            this.btnAddReturnType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddReturnType.BackColor = System.Drawing.Color.Teal;
            this.btnAddReturnType.FlatAppearance.BorderSize = 0;
            this.btnAddReturnType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddReturnType.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.btnAddReturnType.ForeColor = System.Drawing.Color.White;
            this.btnAddReturnType.Location = new System.Drawing.Point(492, 265);
            this.btnAddReturnType.Name = "btnAddReturnType";
            this.btnAddReturnType.Size = new System.Drawing.Size(117, 23);
            this.btnAddReturnType.TabIndex = 17;
            this.btnAddReturnType.Text = "Add Return Type";
            this.btnAddReturnType.UseVisualStyleBackColor = false;
            this.btnAddReturnType.Click += new System.EventHandler(this.btnAddReturnType_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(489, 300);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 16);
            this.label3.TabIndex = 18;
            this.label3.Text = "Returns:";
            // 
            // lblReturnType
            // 
            this.lblReturnType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblReturnType.AutoSize = true;
            this.lblReturnType.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.lblReturnType.ForeColor = System.Drawing.Color.White;
            this.lblReturnType.Location = new System.Drawing.Point(304, 323);
            this.lblReturnType.Name = "lblReturnType";
            this.lblReturnType.Size = new System.Drawing.Size(0, 16);
            this.lblReturnType.TabIndex = 19;
            // 
            // ConstantControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.lblReturnType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnAddReturnType);
            this.Controls.Add(this.listViewTypes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxIdentifier);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewConstants);
            this.Controls.Add(this.btnCreateConstant);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Name = "ConstantControl";
            this.Size = new System.Drawing.Size(612, 396);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxIdentifier;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listViewConstants;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button btnCreateConstant;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ListView listViewTypes;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnAddReturnType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblReturnType;
    }
}
