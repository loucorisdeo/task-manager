using TaskManager.Desktop.Services;
using TaskManager.Domain.Models;

namespace TaskManager.Desktop
{
    public partial class MainForm : Form
    {
        private readonly ApiClient _apiClient;
        private readonly TaskService _taskService;
        private List<TaskItem> _tasks = new();

        public MainForm(ApiClient apiClient)
        {
            InitializeComponent();

            _apiClient = apiClient;
            _taskService = new TaskService(_apiClient);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = $"Welcome, {UserSession.Username}";
            await LoadTasksAsync();
        }

       private async Task LoadTasksAsync()
        {
            try
            {
                btnRefresh.Enabled = false;
                btnComplete.Enabled = false;
                lblStatus.Text = "Loading tasks...";

               _tasks = await _taskService.GetTasksAsync();

                dgvTasks.DataSource = null;
                dgvTasks.DataSource = _tasks;

                lblStatus.Text = $"{_tasks.Count} task(s) loaded.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Failed to load tasks.";
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRefresh.Enabled = true;
                btnComplete.Enabled = true;
            }
        }
     
        private async void  btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadTasksAsync();
        }

        private async void btnComplete_Click(object sender, EventArgs e)
        {
            if (dgvTasks.CurrentRow == null)
            {
                MessageBox.Show("Please select a task first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedTask = dgvTasks.CurrentRow.DataBoundItem as TaskItem;
            if (selectedTask == null)
            {
                MessageBox.Show("Could not determine the selected task.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (selectedTask.IsCompleted)
            {
                MessageBox.Show("That task is already completed.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                btnComplete.Enabled = false;
                lblStatus.Text = "Marking task complete...";

                await _taskService.CompleteTaskAsync(selectedTask.Id);

                lblStatus.Text = "Task updated.";
                await LoadTasksAsync();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Failed to complete task.";
                MessageBox.Show($"Error completing task: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnComplete.Enabled = true;
            }
        }
    }
}