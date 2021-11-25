
namespace NativeDefinerGUI
{
    partial class FunctionControl
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
            this.btnCreateFunction = new System.Windows.Forms.Button();
            this.listViewFunctions = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // btnCreateFunction
            // 
            this.btnCreateFunction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateFunction.BackColor = System.Drawing.Color.Teal;
            this.btnCreateFunction.FlatAppearance.BorderSize = 0;
            this.btnCreateFunction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateFunction.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.btnCreateFunction.ForeColor = System.Drawing.Color.White;
            this.btnCreateFunction.Location = new System.Drawing.Point(336, 442);
            this.btnCreateFunction.Name = "btnCreateFunction";
            this.btnCreateFunction.Size = new System.Drawing.Size(137, 23);
            this.btnCreateFunction.TabIndex = 15;
            this.btnCreateFunction.Text = "Create Function";
            this.btnCreateFunction.UseVisualStyleBackColor = false;
            this.btnCreateFunction.Click += new System.EventHandler(this.btnCreateEvent_Click);
            // 
            // listViewFunctions
            // 
            this.listViewFunctions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewFunctions.BackColor = System.Drawing.Color.Gray;
            this.listViewFunctions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listViewFunctions.HideSelection = false;
            this.listViewFunctions.Location = new System.Drawing.Point(3, 3);
            this.listViewFunctions.Name = "listViewFunctions";
            this.listViewFunctions.Size = new System.Drawing.Size(470, 433);
            this.listViewFunctions.TabIndex = 18;
            this.listViewFunctions.UseCompatibleStateImageBehavior = false;
            this.listViewFunctions.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Functions";
            this.columnHeader2.Width = 300;
            // 
            // FunctionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.listViewFunctions);
            this.Controls.Add(this.btnCreateFunction);
            this.Name = "FunctionControl";
            this.Size = new System.Drawing.Size(476, 468);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCreateFunction;
        private System.Windows.Forms.ListView listViewFunctions;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}
