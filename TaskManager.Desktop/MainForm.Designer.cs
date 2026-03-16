namespace TaskManager.Desktop
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblWelcome = new Label();
            dgvTasks = new DataGridView();
            btnRefresh = new Button();
            btnComplete = new Button();
            lblStatus = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvTasks).BeginInit();
            SuspendLayout();
            // 
            // lblWelcome
            // 
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblWelcome.Location = new Point(20, 20);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(101, 28);
            lblWelcome.TabIndex = 0;
            lblWelcome.Text = "Welcome";
            // 
            // dgvTasks
            // 
            dgvTasks.AllowUserToAddRows = false;
            dgvTasks.AllowUserToDeleteRows = false;
            dgvTasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTasks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTasks.Location = new Point(20, 65);
            dgvTasks.MultiSelect = false;
            dgvTasks.Name = "dgvTasks";
            dgvTasks.ReadOnly = true;
            dgvTasks.RowHeadersWidth = 51;
            dgvTasks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTasks.Size = new Size(740, 300);
            dgvTasks.TabIndex = 1;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(20, 385);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(120, 35);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnComplete
            // 
            btnComplete.Location = new Point(155, 385);
            btnComplete.Name = "btnComplete";
            btnComplete.Size = new Size(160, 35);
            btnComplete.TabIndex = 3;
            btnComplete.Text = "Complete Selected";
            btnComplete.UseVisualStyleBackColor = true;
            btnComplete.Click += btnComplete_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(20, 435);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 20);
            lblStatus.TabIndex = 4;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 471);
            Controls.Add(lblStatus);
            Controls.Add(btnComplete);
            Controls.Add(btnRefresh);
            Controls.Add(dgvTasks);
            Controls.Add(lblWelcome);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Task Manager";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvTasks).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblWelcome;
        private DataGridView dgvTasks;
        private Button btnRefresh;
        private Button btnComplete;
        private Label lblStatus;
    }
}