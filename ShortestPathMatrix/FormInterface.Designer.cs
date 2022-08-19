namespace ShortestPathMatrix
{
    partial class FormInterface
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
            this.btnInsertSampleGraph = new System.Windows.Forms.Button();
            this.textBoxOutputFolder = new System.Windows.Forms.TextBox();
            this.labelOutputFolder = new System.Windows.Forms.Label();
            this.btnOutputFolder = new System.Windows.Forms.Button();
            this.btnGenerateShortestPathMatrix = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // btnInsertSampleGraph
            // 
            this.btnInsertSampleGraph.Location = new System.Drawing.Point(12, 12);
            this.btnInsertSampleGraph.Name = "btnInsertSampleGraph";
            this.btnInsertSampleGraph.Size = new System.Drawing.Size(180, 38);
            this.btnInsertSampleGraph.TabIndex = 2;
            this.btnInsertSampleGraph.Text = "Insert Sample Graph";
            this.btnInsertSampleGraph.UseVisualStyleBackColor = true;
            this.btnInsertSampleGraph.Click += new System.EventHandler(this.btnInsertSampleGraph_Click);
            // 
            // textBoxOutputFolder
            // 
            this.textBoxOutputFolder.Location = new System.Drawing.Point(12, 112);
            this.textBoxOutputFolder.Name = "textBoxOutputFolder";
            this.textBoxOutputFolder.Size = new System.Drawing.Size(128, 20);
            this.textBoxOutputFolder.TabIndex = 3;
            // 
            // labelOutputFolder
            // 
            this.labelOutputFolder.AutoSize = true;
            this.labelOutputFolder.Location = new System.Drawing.Point(12, 95);
            this.labelOutputFolder.Name = "labelOutputFolder";
            this.labelOutputFolder.Size = new System.Drawing.Size(68, 13);
            this.labelOutputFolder.TabIndex = 4;
            this.labelOutputFolder.Text = "Output folder";
            // 
            // btnOutputFolder
            // 
            this.btnOutputFolder.Location = new System.Drawing.Point(146, 110);
            this.btnOutputFolder.Name = "btnOutputFolder";
            this.btnOutputFolder.Size = new System.Drawing.Size(46, 23);
            this.btnOutputFolder.TabIndex = 5;
            this.btnOutputFolder.Text = "...";
            this.btnOutputFolder.UseVisualStyleBackColor = true;
            this.btnOutputFolder.Click += new System.EventHandler(this.btnOutputFolder_Click);
            // 
            // btnGenerateShortestPathMatrix
            // 
            this.btnGenerateShortestPathMatrix.Location = new System.Drawing.Point(12, 138);
            this.btnGenerateShortestPathMatrix.Name = "btnGenerateShortestPathMatrix";
            this.btnGenerateShortestPathMatrix.Size = new System.Drawing.Size(180, 38);
            this.btnGenerateShortestPathMatrix.TabIndex = 6;
            this.btnGenerateShortestPathMatrix.Text = "GENERATE \r\nSHORTEST PATH MATRIX";
            this.btnGenerateShortestPathMatrix.UseVisualStyleBackColor = true;
            this.btnGenerateShortestPathMatrix.Click += new System.EventHandler(this.btnGenerateShortestPathMatrix_Click);
            // 
            // FormInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(204, 190);
            this.Controls.Add(this.btnGenerateShortestPathMatrix);
            this.Controls.Add(this.btnOutputFolder);
            this.Controls.Add(this.labelOutputFolder);
            this.Controls.Add(this.textBoxOutputFolder);
            this.Controls.Add(this.btnInsertSampleGraph);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormInterface";
            this.Text = "Shortest Path Matrix";
            this.Load += new System.EventHandler(this.FormInterface_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnInsertSampleGraph;
        private System.Windows.Forms.TextBox textBoxOutputFolder;
        private System.Windows.Forms.Label labelOutputFolder;
        private System.Windows.Forms.Button btnOutputFolder;
        private System.Windows.Forms.Button btnGenerateShortestPathMatrix;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}