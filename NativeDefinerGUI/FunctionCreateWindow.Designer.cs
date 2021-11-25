
namespace NativeDefinerGUI
{
    partial class FunctionCreateWindow
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
            this.btnCreateFunction = new System.Windows.Forms.Button();
            this.listViewTypes = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewParameters = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAddParam = new System.Windows.Forms.Button();
            this.richTextDescription = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRemoveParam = new System.Windows.Forms.Button();
            this.richTextParamText = new System.Windows.Forms.RichTextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblReturnType = new System.Windows.Forms.Label();
            this.btnAddReturnType = new System.Windows.Forms.Button();
            this.listViewCategory = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.label1.Location = new System.Drawing.Point(28, 311);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 16);
            this.label1.TabIndex = 19;
            this.label1.Text = "Name:";
            // 
            // btnCreateFunction
            // 
            this.btnCreateFunction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateFunction.BackColor = System.Drawing.Color.Teal;
            this.btnCreateFunction.Enabled = false;
            this.btnCreateFunction.FlatAppearance.BorderSize = 0;
            this.btnCreateFunction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateFunction.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.btnCreateFunction.ForeColor = System.Drawing.Color.White;
            this.btnCreateFunction.Location = new System.Drawing.Point(826, 472);
            this.btnCreateFunction.Name = "btnCreateFunction";
            this.btnCreateFunction.Size = new System.Drawing.Size(143, 23);
            this.btnCreateFunction.TabIndex = 18;
            this.btnCreateFunction.Text = "Create Function";
            this.btnCreateFunction.UseVisualStyleBackColor = false;
            this.btnCreateFunction.Click += new System.EventHandler(this.btnCreateParam_Click);
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
            this.listViewTypes.Size = new System.Drawing.Size(409, 251);
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
            this.listViewParameters.Size = new System.Drawing.Size(400, 251);
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
            this.richTextDescription.Location = new System.Drawing.Point(556, 330);
            this.richTextDescription.Name = "richTextDescription";
            this.richTextDescription.Size = new System.Drawing.Size(224, 165);
            this.richTextDescription.TabIndex = 24;
            this.richTextDescription.Text = "";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(562, 311);
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
            // richTextParamText
            // 
            this.richTextParamText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextParamText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.richTextParamText.ForeColor = System.Drawing.Color.White;
            this.richTextParamText.Location = new System.Drawing.Point(269, 330);
            this.richTextParamText.Name = "richTextParamText";
            this.richTextParamText.Size = new System.Drawing.Size(268, 165);
            this.richTextParamText.TabIndex = 27;
            this.richTextParamText.Text = "";
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.textBoxName.ForeColor = System.Drawing.Color.White;
            this.textBoxName.Location = new System.Drawing.Point(20, 330);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(228, 20);
            this.textBoxName.TabIndex = 29;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(266, 311);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 16);
            this.label4.TabIndex = 30;
            this.label4.Text = "Parameter Text:";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(828, 345);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 16);
            this.label5.TabIndex = 31;
            this.label5.Text = "Returns:";
            // 
            // lblReturnType
            // 
            this.lblReturnType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblReturnType.AutoSize = true;
            this.lblReturnType.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.lblReturnType.ForeColor = System.Drawing.Color.White;
            this.lblReturnType.Location = new System.Drawing.Point(828, 373);
            this.lblReturnType.Name = "lblReturnType";
            this.lblReturnType.Size = new System.Drawing.Size(0, 16);
            this.lblReturnType.TabIndex = 32;
            // 
            // btnAddReturnType
            // 
            this.btnAddReturnType.BackColor = System.Drawing.Color.Teal;
            this.btnAddReturnType.FlatAppearance.BorderSize = 0;
            this.btnAddReturnType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddReturnType.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.btnAddReturnType.ForeColor = System.Drawing.Color.White;
            this.btnAddReturnType.Location = new System.Drawing.Point(831, 319);
            this.btnAddReturnType.Name = "btnAddReturnType";
            this.btnAddReturnType.Size = new System.Drawing.Size(133, 23);
            this.btnAddReturnType.TabIndex = 33;
            this.btnAddReturnType.Text = "Add Return Type";
            this.btnAddReturnType.UseVisualStyleBackColor = false;
            this.btnAddReturnType.Click += new System.EventHandler(this.btnAddReturnType_Click);
            // 
            // listViewCategory
            // 
            this.listViewCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listViewCategory.BackColor = System.Drawing.Color.Gray;
            this.listViewCategory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.listViewCategory.HideSelection = false;
            this.listViewCategory.Location = new System.Drawing.Point(20, 356);
            this.listViewCategory.Name = "listViewCategory";
            this.listViewCategory.Size = new System.Drawing.Size(228, 139);
            this.listViewCategory.TabIndex = 34;
            this.listViewCategory.UseCompatibleStateImageBehavior = false;
            this.listViewCategory.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Category";
            this.columnHeader3.Width = 200;
            // 
            // FunctionCreateWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(981, 507);
            this.Controls.Add(this.listViewCategory);
            this.Controls.Add(this.btnAddReturnType);
            this.Controls.Add(this.lblReturnType);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.richTextParamText);
            this.Controls.Add(this.btnRemoveParam);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.richTextDescription);
            this.Controls.Add(this.btnAddParam);
            this.Controls.Add(this.listViewParameters);
            this.Controls.Add(this.listViewTypes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxIdentifier);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCreateFunction);
            this.Name = "FunctionCreateWindow";
            this.Text = "ParameterCreateWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxIdentifier;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCreateFunction;
        private System.Windows.Forms.ListView listViewTypes;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ListView listViewParameters;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnAddParam;
        private System.Windows.Forms.RichTextBox richTextDescription;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRemoveParam;
        private System.Windows.Forms.RichTextBox richTextParamText;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblReturnType;
        private System.Windows.Forms.Button btnAddReturnType;
        private System.Windows.Forms.ListView listViewCategory;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}