using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Job_Ticket_Manager
{
    public partial class UserCreateScreen : Form
    {
        /// <summary>
        ///  Variables that keep track of entered data from user.
        /// </summary>
        private string username;
        private string password;

        // Minimum length the password entered can be. Defaults to 0  
        private int minPasswordLength;

        // Maximum length the password entered can be. Defaults to 30
        private int maxPasswordLength;

        // Designate which privilege this is setting. Defaults to guest.
        private string userPrivilege;

        // Shows whether the creation process was successful. True if so False if not.
        private bool success;

        // Indicates whether the user wanted to cancel creating the new user.
        private bool cancel;

        /// <summary>
        ///  Default constructor for create user screen.
        /// </summary>
        public UserCreateScreen()
        {
            InitializeComponent();

            this.username = string.Empty;
            this.password = string.Empty;
            this.minPasswordLength = SecurityConstants.PasswordMinimumLength;
            this.maxPasswordLength = SecurityConstants.PasswordMaximumLength;
            this.userPrivilege = SecurityConstants.GuestUser;
            this.success = false;
            this.cancel = false;
        }

        /// <summary>
        ///  Setter and getter for top-most read only textbox. Used to store descriptions.
        /// </summary>
        public string Description
        {
            get { return this.textBox4.Text; }
            set { this.textBox4.Text = value; }
        }

        /// <summary>
        ///  Name for label1. Defaults to "Username"
        /// </summary>
        public string Label1
        {
            get { return this.label1.Text; }
            set { this.label1.Text = value; }
        }

        /// <summary>
        ///  Name for label2. Defaults to "Password"
        /// </summary>
        public string Label2
        {
            get { return this.label2.Text; }
            set { this.label2.Text = value; }
        }

        /// <summary>
        ///  Name for label3. Defaults to "Re-enter Password"
        /// </summary>
        public string Label3
        {
            get { return this.label3.Text; }
            set { this.label3.Text = value; }
        }

        /// <summary>
        ///  Getter for entered username
        /// </summary>
        public string Username
        {
            get { return this.username;}
        }
        
        /// <summary>
        ///  Getter for entered password.
        /// </summary>
        public string Password
        {
            get { return this.password; }
        }

        /// <summary>
        ///  Gets or Sets minimum password length.
        /// </summary>
        public int MinPasswordLength
        {
            get { return this.minPasswordLength; }
            set { this.minPasswordLength = value;}
        }

        /// <summary>
        ///  Gets or Sets user privilege for the session (just label, does not give admin or guest privileges)
        /// </summary>
        public string UserPrivilege
        {
            get { return this.userPrivilege;}
            set { this.userPrivilege = value;}
        }

        /// <summary>
        ///  Gets or Sets success of the user creation
        /// </summary>
        public bool Success
        {
            get { return this.success; }
            set { this.success = value;}
        }

        /// <summary>
        ///  Gets cancel (if the user opted out of creating a user)
        /// </summary>
        public bool Cancel
        {
            get { return this.cancel; }
        }

        /// <summary>
        ///  On submit button press. Verify that a username is entered and both password entries match. If all is true, exit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            // Verify username
            if (this.textBox1.Text == string.Empty)
            {
                // If no username is entered, return.
                MessageBox.Show(ErrorMessages.UsernameMissing, ErrorMessages.CreateUserErrorTitle, MessageBoxButtons.OK);
                return;
            }

            // Verify password
            // Check minimum length
            if (this.textBox2.Text.Length < this.minPasswordLength)
            {
                MessageBox.Show(ErrorMessages.PasswordMinimumLength, ErrorMessages.CreateUserErrorTitle, MessageBoxButtons.OK);
                return;

            }
            // Check maximum length
            if (this.textBox2.Text.Length > this.maxPasswordLength)
            {
                MessageBox.Show(ErrorMessages.PasswordMaximumLength, ErrorMessages.CreateUserErrorTitle, MessageBoxButtons.OK);
                return;
            }
            // If both textboxes match
            if (this.textBox2.Text != this.textBox3.Text)
            {
                // Return if they don't
                MessageBox.Show(ErrorMessages.PasswordsDoNotMatch, ErrorMessages.CreateUserErrorTitle, MessageBoxButtons.OK);
                return;
            }

            // Assign all values 
            this.username = this.textBox1.Text;
            this.password = this.textBox2.Text;

            // If all have passed at this point, mark as successful and return.
            this.success = true;
            this.Close();
        }

        /// <summary>
        ///  On cancel button press.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            // Mark as unsuccessful and close.
            this.success = false;
            this.cancel = true;
            this.Close();
        }
    }
}
