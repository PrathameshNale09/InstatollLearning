using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using InstaTollLearningApp.Data;


namespace InstaTollLearningApp
{
    public partial class LoginForms : Form
    {
        public LoginForms()
        {
            InitializeComponent();
            string testPassword = "Demo@123";

            string testHash =
                InstaTollLearningApp.Security.PasswordHasher
                    .HashPassword(testPassword);

            bool correctPassword =
                InstaTollLearningApp.Security.PasswordHasher
                    .VerifyPassword(testPassword, testHash);

            bool wrongPassword =
                InstaTollLearningApp.Security.PasswordHasher
                    .VerifyPassword("WrongPassword", testHash);

            //MessageBox.Show(
            //    "Correct password: " + correctPassword +
            //    "\nWrong password: " + wrongPassword);

            try
            {
                InstaTollLearningApp.Data.DatabaseInitializer.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Database setup failed.\n" + ex.Message,
                    "Startup Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void LoginForms_Load(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter username and password.");
                return;
            }
            try
            {
                var authService =
                    new InstaTollLearningApp.Services
                        .AuthenticationService();

                if (authService.Authenticate(username, password))
                {
                    MessageBox.Show("Login successful!");

                    int userId = authService.GetUserId(username);

                    RecordsForm recordsForm = new RecordsForm(userId);
                    recordsForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show(
                        "Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                // Temporary diagnostic for this learning exercise.
                // We'll replace this with NLog shortly.
                System.Diagnostics.Debug.WriteLine(ex);

                MessageBox.Show(
                    "Unable to complete login. Please try again.");
            }

        }


        private void TestDatabaseConnection()
        {
            try
            {
                using (SqlConnection connection =
                    DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    MessageBox.Show(
                        "Database connection successful!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Database connection failed.\n" + ex.Message);
            }
        }

    }
}
