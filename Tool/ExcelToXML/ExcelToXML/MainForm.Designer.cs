
namespace ExcelToXML
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label labelProjectPath;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            this.excelFileInput = new System.Windows.Forms.TextBox();
            this.btnSelectExcel = new System.Windows.Forms.Button();
            this.btnGenOneXML = new System.Windows.Forms.Button();
            this.btnGenAllXML = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.projectPathInput = new System.Windows.Forms.TextBox();
            this.btnBrowserProject = new System.Windows.Forms.Button();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.btnCommitXML = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            label1 = new System.Windows.Forms.Label();
            labelProjectPath = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(17, 122);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(119, 12);
            label1.TabIndex = 5;
            label1.Text = "选择单个excel文件：";
            // 
            // labelProjectPath
            // 
            labelProjectPath.AutoSize = true;
            labelProjectPath.Location = new System.Drawing.Point(17, 22);
            labelProjectPath.Name = "labelProjectPath";
            labelProjectPath.Size = new System.Drawing.Size(167, 12);
            labelProjectPath.TabIndex = 15;
            labelProjectPath.Text = "项目目录(指定到trunk目录)：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label2.ForeColor = System.Drawing.Color.Red;
            label2.Location = new System.Drawing.Point(142, 122);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(213, 12);
            label2.TabIndex = 22;
            label2.Text = "只有生成单个文件才用这个选项！！";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label3.ForeColor = System.Drawing.Color.Red;
            label3.Location = new System.Drawing.Point(17, 261);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(291, 12);
            label3.TabIndex = 23;
            label3.Text = "这里的一键生成是将所有的配置表都进行转化！！";
            // 
            // excelFileInput
            // 
            this.excelFileInput.Location = new System.Drawing.Point(19, 140);
            this.excelFileInput.Name = "excelFileInput";
            this.excelFileInput.Size = new System.Drawing.Size(280, 21);
            this.excelFileInput.TabIndex = 3;
            // 
            // btnSelectExcel
            // 
            this.btnSelectExcel.Location = new System.Drawing.Point(306, 138);
            this.btnSelectExcel.Name = "btnSelectExcel";
            this.btnSelectExcel.Size = new System.Drawing.Size(75, 23);
            this.btnSelectExcel.TabIndex = 4;
            this.btnSelectExcel.Text = "选择文件";
            this.btnSelectExcel.UseVisualStyleBackColor = true;
            this.btnSelectExcel.Click += new System.EventHandler(this.BtnSelectOneExcelFile);
            // 
            // btnGenOneXML
            // 
            this.btnGenOneXML.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnGenOneXML.Location = new System.Drawing.Point(144, 181);
            this.btnGenOneXML.Name = "btnGenOneXML";
            this.btnGenOneXML.Size = new System.Drawing.Size(136, 35);
            this.btnGenOneXML.TabIndex = 6;
            this.btnGenOneXML.Text = "生成单个XML配置";
            this.btnGenOneXML.UseVisualStyleBackColor = false;
            this.btnGenOneXML.Click += new System.EventHandler(this.GenerateOneXML);
            // 
            // btnGenAllXML
            // 
            this.btnGenAllXML.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnGenAllXML.Location = new System.Drawing.Point(19, 288);
            this.btnGenAllXML.Name = "btnGenAllXML";
            this.btnGenAllXML.Size = new System.Drawing.Size(136, 64);
            this.btnGenAllXML.TabIndex = 7;
            this.btnGenAllXML.Text = "一键生成XML配置";
            this.btnGenAllXML.UseVisualStyleBackColor = false;
            this.btnGenAllXML.Click += new System.EventHandler(this.GenerateAllXML);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Wheat;
            this.pictureBox2.Location = new System.Drawing.Point(12, 114);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(380, 117);
            this.pictureBox2.TabIndex = 9;
            this.pictureBox2.TabStop = false;
            // 
            // projectPathInput
            // 
            this.projectPathInput.Location = new System.Drawing.Point(19, 48);
            this.projectPathInput.Name = "projectPathInput";
            this.projectPathInput.Size = new System.Drawing.Size(280, 21);
            this.projectPathInput.TabIndex = 17;
            // 
            // btnBrowserProject
            // 
            this.btnBrowserProject.Location = new System.Drawing.Point(306, 48);
            this.btnBrowserProject.Name = "btnBrowserProject";
            this.btnBrowserProject.Size = new System.Drawing.Size(75, 23);
            this.btnBrowserProject.TabIndex = 16;
            this.btnBrowserProject.Text = "浏览";
            this.btnBrowserProject.UseVisualStyleBackColor = true;
            this.btnBrowserProject.Click += new System.EventHandler(this.SelectProjectPath);
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.PaleTurquoise;
            this.pictureBox4.Location = new System.Drawing.Point(12, 12);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(380, 82);
            this.pictureBox4.TabIndex = 14;
            this.pictureBox4.TabStop = false;
            // 
            // btnCommitXML
            // 
            this.btnCommitXML.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnCommitXML.Location = new System.Drawing.Point(245, 288);
            this.btnCommitXML.Name = "btnCommitXML";
            this.btnCommitXML.Size = new System.Drawing.Size(136, 67);
            this.btnCommitXML.TabIndex = 19;
            this.btnCommitXML.Text = "上传XML配置表";
            this.btnCommitXML.UseVisualStyleBackColor = false;
            this.btnCommitXML.Click += new System.EventHandler(this.CommitXML);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Lime;
            this.pictureBox1.Location = new System.Drawing.Point(12, 250);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(380, 117);
            this.pictureBox1.TabIndex = 24;
            this.pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 411);
            this.Controls.Add(label3);
            this.Controls.Add(label2);
            this.Controls.Add(this.btnCommitXML);
            this.Controls.Add(this.projectPathInput);
            this.Controls.Add(this.btnBrowserProject);
            this.Controls.Add(labelProjectPath);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.btnGenAllXML);
            this.Controls.Add(this.btnGenOneXML);
            this.Controls.Add(label1);
            this.Controls.Add(this.btnSelectExcel);
            this.Controls.Add(this.excelFileInput);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(420, 520);
            this.MinimumSize = new System.Drawing.Size(420, 450);
            this.Name = "MainForm";
            this.Text = "神奇的配置表工具";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox excelFileInput;
        private System.Windows.Forms.Button btnSelectExcel;
        private System.Windows.Forms.Button btnGenOneXML;
        private System.Windows.Forms.Button btnGenAllXML;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TextBox projectPathInput;
        private System.Windows.Forms.Button btnBrowserProject;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Button btnCommitXML;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
