/*
 * Created by SharpDevelop.
 * User: Giovanni.Brogiolo
 * Date: 20/10/2018
 * Time: 5:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace WPA
{
    partial class FormOpenFile
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.openBtn = new System.Windows.Forms.Button();
            this.tBoxCsvFilePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_browse = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // openBtn
            // 
            this.openBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.openBtn.Location = new System.Drawing.Point(463, 53);
            this.openBtn.Name = "openBtn";
            this.openBtn.Size = new System.Drawing.Size(75, 23);
            this.openBtn.TabIndex = 1;
            this.openBtn.Text = "OK";
            this.openBtn.UseCompatibleTextRendering = true;
            this.openBtn.UseVisualStyleBackColor = true;
            this.openBtn.Click += new System.EventHandler(this.OpenBtnClick);
            // 
            // tBoxCsvFilePath
            // 
            this.tBoxCsvFilePath.Location = new System.Drawing.Point(12, 25);
            this.tBoxCsvFilePath.Name = "tBoxCsvFilePath";
            this.tBoxCsvFilePath.Size = new System.Drawing.Size(526, 20);
            this.tBoxCsvFilePath.TabIndex = 2;
            this.tBoxCsvFilePath.Text = "C:\\Users\\giovanni.brogiolo\\Desktop\\excelTable.csv";
            this.tBoxCsvFilePath.TextChanged += new System.EventHandler(this.TBoxRvtFilePathTextChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "CSV File path";
            this.label1.UseCompatibleTextRendering = true;
            // 
            // btn_browse
            // 
            this.btn_browse.Location = new System.Drawing.Point(354, 53);
            this.btn_browse.Name = "btn_browse";
            this.btn_browse.Size = new System.Drawing.Size(75, 23);
            this.btn_browse.TabIndex = 4;
            this.btn_browse.Text = "Browse...";
            this.btn_browse.UseCompatibleTextRendering = true;
            this.btn_browse.UseVisualStyleBackColor = true;
            this.btn_browse.Click += new System.EventHandler(this.Btn_browseClick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialog1FileOk);
            // 
            // FormOpenFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 89);
            this.Controls.Add(this.btn_browse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tBoxCsvFilePath);
            this.Controls.Add(this.openBtn);
            this.Name = "FormOpenFile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Set Parameters From Excel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btn_browse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tBoxCsvFilePath;
        private System.Windows.Forms.Button openBtn;
    }
}
