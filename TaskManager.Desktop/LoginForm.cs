
using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using TaskManager.Desktop.Services;

namespace TaskManager.Desktop
{
    public partial class LoginForm : Form
    {



        private readonly ApiClient _apiClient;
        private readonly AuthService _authService;


        public LoginForm()
        {
            InitializeComponent();

            _apiClient = new ApiClient();
            _authService = new AuthService(_apiClient);

            lblError.Text = "";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblError.Text = "Please enter username and password.";
                return;
            }

            try
            {
                var result = await _authService.LoginAsync(username, password);

                if (result == null || string.IsNullOrWhiteSpace(result.Token))
                {
                    lblError.Text = "Login failed.";
                    return;
                }

                UserSession.Token = result.Token;
                UserSession.Username = result.Username;
                UserSession.Role = result.Role;

                _apiClient.SetToken(result.Token);

                
                 var mainForm = new MainForm(_apiClient);
                 mainForm.Show();
                 this.Hide();
            }
            catch (Exception)
            {
                lblError.Text = "Login failed. Please check your credentials.";
            }
        }
    }
}
