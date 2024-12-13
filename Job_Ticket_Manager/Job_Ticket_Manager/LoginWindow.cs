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
    public partial class LoginWindow : Form
    {
        /// <summary>
        ///  Variables used for keeping track of entered values from the user as well as what buttons are pressed if necessary
        /// </summary>
        private string enteredUsername;
        private string enteredPassword;

        private bool cancelButtonPressed;
        private bool submitButtonPressed;
        private bool newUserButtonPressed;
        private bool success;

        /// <summary>
        ///  Default constructor for login window.
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();

            this.enteredUsername = string.Empty;
            this.enteredPassword = string.Empty;
            this.cancelButtonPressed = false;
            this.submitButtonPressed = false;
            this.newUserButtonPressed = false;
            this.success = false;
        }

        /// Setters and Getters for window objects
        public string DescriptionTextbox
        {
            set { this.textBox1.Text = value; }
        }
        public string UsernameLabel
        {
            get { return this.label1.Text; }
            set { this.label1.Text = value; }
        }
        public string UsernameTextbox
        {
            get { return this.textBox2.Text; }
        }
        public string PasswordLabel
        {
            get { return this.label2.Text; }
            set { this.label2.Text = value; }
        }
        public string PasswordTextbox
        {
            get { return this.textBox3.Text; }
        }


        /// Setters and getters for user input and user's action based variables
        public bool CancelButtonPressed
        {
            get { return this.cancelButtonPressed; }
        }
        public bool SubmitButtonPressed
        {
            get { return this.submitButtonPressed; }
        }
        public bool NewUserButtonPressed
        {
            get { return this.newUserButtonPressed;}
        }
        public bool Success
        {
            get { return this.success; }
            set { this.success = value; }
        }

        
        /// Getters for data variables entered
        public string EnteredUsername
        {
            get { return this.enteredUsername; }
        }
        public string EnteredPassword
        {
            get { return this.enteredPassword; }
        }

        /// <summary>
        ///  On submit button being pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.UsernameTextbox == string.Empty)
            {
                MessageBox.Show(ErrorMessages.UsernameMissing, ErrorMessages.LoginErrorTitle, MessageBoxButtons.OK);
                return;
            }
            if (this.PasswordTextbox == string.Empty)
            {
                MessageBox.Show(ErrorMessages.PasswordMissing, ErrorMessages.LoginErrorTitle, MessageBoxButtons.OK);
                return;
            }

            // Assign
            this.enteredUsername = this.UsernameTextbox;
            this.enteredPassword = this.PasswordTextbox;

            // Assign to bools to show what happened
            this.cancelButtonPressed = false;
            this.newUserButtonPressed = false;
            this.submitButtonPressed = true;
            this.success = true;

            // Close
            this.Close();
        }

        /// <summary>
        ///  On cancel button being pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            // Assign to bools to show what happened
            this.cancelButtonPressed = true;
            this.newUserButtonPressed = false;
            this.submitButtonPressed = false;
            this.success = false;

            // Close
            this.Close();
        }

        /// <summary>
        ///  On new user button being pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            // Assign to bools to show what happened
            this.cancelButtonPressed = false;
            this.newUserButtonPressed = true;
            this.submitButtonPressed = false;
            this.success = false;

            // Close
            this.Close();
        }
    }
}
