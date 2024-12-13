/// <summary>
///  MainWindow.cs
///  Job Ticket Manager Project
///  Creator: John D. Sbur
/// </summary>
namespace Job_Ticket_Manager
{
    using JobTicketEngine;
    using Microsoft.Office.Interop.Word;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Printing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Color = Color;
    using Font = Font;
    using Image = Image;
    using Point = Point;
    using Rectangle = Rectangle;

    public partial class MainWindow : Form
    {
        /// <summary>
        ///  Locals used during main window usage.
        /// </summary>
        // ******************
        // Job Ticket Tab
        // ******************
        // Properties of the job ticket outside of the object. Keeps track of the file path to the original ticket as well as the image associated with the job ticket.
        private Dictionary<int, JobTicket> activeDatabaseJobTickets;
        private Dictionary<string, int> jobTicketFilePaths;

        // Keeps track of which ticket is selected to display between all the tickets loaded in.
        private int? selectedTicketID;

        // Marks who is logged in. Defaults to guest privileges and default username
        private string userLoggedIn;
        private string userPrivilege;
        private event PropertyChangedEventHandler UserPropertyChanged = delegate { };

        // ******************
        // Template Tab
        // ******************
        // Template currently being created. Set to null on startup.
        private TicketTemplate? activeTemplate;
        // Image of the template currently being created.
        private Image? activeTemplateImage;
        // Template saved file path
        private string? activeTemplateSavedFilePath;

        // Corresponds to editing template tab's datagridview2. 
        // Set during cellbeginedit and cellendedit in datagridview2.
        // beginedit -> set to true.
        // endedit -> set to false.
        private bool editingTemplateTabCell;
        // Corresponds to the cell end edit event and redrawing data. If the priority of an object was edited, then
        // the datagridview2 needs to be redrawn. On edit, set to true. After being redrawn, set to false.
        private bool priorityChanged;

        // Used for cell begin and cell end edit events.
        private string? selectedCellProperty;
        private string? selectedCellOldValue;
        private int? selectedCellX;
        private int? selectedCellY;



        // ******************
        // Create Ticket Tab
        // ******************
        // Template currently being used in the create ticket tab. Set to null on startup.
        private TicketTemplate? creationTemplateBase;
        // Image of template currently being used in the create ticket tab.
        private Image? creationTemplateBaseImage;
        // Saves the path of the creation tab template
        private string? creationTemplateSavedFilePath;



        // ******************
        // General
        // ******************
        // Database where templates and tickets are stored.
        private ActiveDatabase activeDatabase;
        // Database where system information is stored
        private Database systemDatabase;

        // Helps manage keyboard events. When a key is pressed, this is set to true and when a key is released, this is set to false.
        private bool keyPressed;

        

        /// <summary>
        ///  Initialize the main window. Set up different elements as needed.
        /// </summary>
        public MainWindow()
        {
            // Sets up main window to be centered on screen and have components initialized according to its designer.
            this.InitializeComponent();
            this.CenterToScreen();

            // Subscribe the Update User Session function to changes made to user variables
            this.UserPropertyChanged += this.UpdateUserSession;

            // Set user constants to defaults
            this.userLoggedIn = "";
            this.userPrivilege = "";
            this.UserLoggedIn = SecurityConstants.DefaultUserName;
            this.UserPrivilege = SecurityConstants.GuestUser;

            // Initialize activeJobTicket to be null as well as the image list to be empty
            this.activeDatabaseJobTickets = new Dictionary<int, JobTicket>();
            this.activeDatabaseJobTickets.Clear();
            this.jobTicketFilePaths = new Dictionary<string, int>();
            this.jobTicketFilePaths.Clear();

            // Initialize activeTemplate to be null and its image to be null
            this.activeTemplate = null;
            this.activeTemplateImage = null;

            // Initialize creationTemplate to be null and its image to be null
            this.creationTemplateBase = null;
            this.creationTemplateBaseImage = null;

            // Set active and system databases.
            this.activeDatabase = new ActiveDatabase();
            this.systemDatabase = new Database();
            // Retrieve system database information. If we can't, then initialize the system database
            // Check at the same time so see if the default document image exists and if it doesn't, initialize the system database.
            if (!Directory.Exists(ProjectDataBaseConstants.AppDataDirectory +
                ProjectDataBaseConstants.SystemDatabaseName) 
                ||
                !File.Exists(ProjectDataBaseConstants.AppDataDirectory +
                ProjectDataBaseConstants.SystemDatabaseName +
                ProjectDataBaseConstants.DefaultDocumentName))
            {
                this.InitializeSystemDatabase();
            }
            else
            {
                // Attempt to load the system database. If it fails, initialize it from scratch
                try
                {
                    this.SystemDatabaseLoad();

                    // If the active database loaded was the default database, proceed without loggin in
                    if (this.activeDatabase.DatabaseName == ProjectDataBaseConstants.AppDataDirectory +
                        ProjectDataBaseConstants.SystemDatabaseName +
                        ProjectDataBaseConstants.DefaultActiveDatabaseName)
                    {
                        this.UserLoggedIn = SecurityConstants.DefaultUserName;
                        this.UserPrivilege = SecurityConstants.AdminUser;
                    }


                    // Try to log in or have the user create a profile. If that fails, set the active database
                    //  as the default database
                    // If the database is not the default database, look for a user file. If the user file doesn't exist, initialize it and request
                    //  admin name and login password.
                    else
                    {
                        // If the user database doesn't exist, create a new admin and proceed afterwards
                        if (this.activeDatabase.UserDatabaseInitialized == false)
                        {
                            // Attempt to initialize the new userdatabase. If unsuccessful, return to initial database.
                            if (this.AddNewUserToActiveDatabase(true) == false)
                            {
                                this.activeDatabase.DatabaseName = ProjectDataBaseConstants.AppDataDirectory +
                                    ProjectDataBaseConstants.SystemDatabaseName +
                                    ProjectDataBaseConstants.DefaultActiveDatabaseName;
                            }
                        }
                        // Try to log in. Log in only if user database file is available
                        else
                        {
                            // Try to log in first, but have the option to change to creating a new user as an option.
                            bool loggingIn = true;
                            bool creatingNewUser = false;

                            // Login screen
                            LoginWindow loginWindow = new LoginWindow();
                            loginWindow.DescriptionTextbox = SecurityMessages.LoginMessage(this.activeDatabase.DatabaseName);

                            // Create screen variables
                            do
                            {
                                // Log in case
                                if (loggingIn)
                                {
                                    // Attempt to log in. Proceed based on output
                                    loginWindow.ShowDialog();

                                    // If cancelled, set all to false and return. Set old database as active database
                                    if (loginWindow.CancelButtonPressed)
                                    {
                                        this.activeDatabase.DatabaseName = ProjectDataBaseConstants.AppDataDirectory +
                                            ProjectDataBaseConstants.SystemDatabaseName +
                                            ProjectDataBaseConstants.DefaultActiveDatabaseName;
                                        loggingIn = false;
                                        creatingNewUser = false;
                                    }
                                    // If new user, switch to creating a new user
                                    else if (loginWindow.NewUserButtonPressed)
                                    {
                                        creatingNewUser = true;
                                        loggingIn = false;
                                    }
                                    // If username and password given, try to log in. Proceed based on result
                                    else if (loginWindow.SubmitButtonPressed && loginWindow.Success)
                                    {
                                        switch (this.activeDatabase.Login(loginWindow.EnteredUsername, loginWindow.EnteredPassword))
                                        {
                                            // Logged in successfully as admin
                                            case (0):
                                                // Initialize database settings
                                                this.activeDatabase.InitializeSettings();

                                                loggingIn = false;
                                                creatingNewUser = false;
                                                this.UserLoggedIn = loginWindow.EnteredUsername;
                                                this.UserPrivilege = SecurityConstants.AdminUser;

                                                break;
                                            // Logged in successfully as guest
                                            case (1):
                                                // Initialize database settings
                                                this.activeDatabase.InitializeSettings();

                                                loggingIn = false;
                                                creatingNewUser = false;
                                                this.UserLoggedIn = loginWindow.EnteredUsername;
                                                this.UserPrivilege = SecurityConstants.GuestUser;

                                                break;
                                            case (2):
                                                DisplayMessage(ErrorMessages.LoginErrorTitle, ErrorMessages.LoginFailedUserDatabaseMissing, MessageBoxButtons.OK);
                                                break;
                                            case (3):
                                                DisplayMessage(ErrorMessages.LoginErrorTitle, ErrorMessages.LoginFailedUsernameNotFound, MessageBoxButtons.OK);
                                                break;
                                            case (4):
                                                DisplayMessage(ErrorMessages.LoginErrorTitle, ErrorMessages.LoginFailedIncorrectPassword, MessageBoxButtons.OK);
                                                break;
                                            case (5):
                                                DisplayMessage(ErrorMessages.LoginErrorTitle, ErrorMessages.LoginFailedFileError, MessageBoxButtons.OK);
                                                break;
                                        }
                                    }
                                }
                                // Create new user case
                                else
                                {
                                    // Add new user. If unsuccessful, return to logging in.
                                    if (!this.AddNewUserToActiveDatabase(false))
                                    {
                                        loggingIn = true;
                                        creatingNewUser = false;
                                    }
                                    else
                                    {
                                        this.DisplayMessage(string.Empty, SecurityMessages.UserCreatedSuccessfully, MessageBoxButtons.OK);
                                        loggingIn = true;
                                        creatingNewUser = false;
                                    }
                                }
                            } while (loggingIn || creatingNewUser);
                        }

                    }

                    // Reinitialize
                    this.InitializeJobTicketTab(true);
                }
                catch
                {
                    this.InitializeSystemDatabase();
                }
            }

            // Set picturebox pictures to system database default document picture.
            this.pictureBox1.Image = Image.FromFile(ProjectDataBaseConstants.AppDataDirectory +
                ProjectDataBaseConstants.SystemDatabaseName +
                ProjectDataBaseConstants.DefaultDocumentName);
            this.pictureBox2.Image = Image.FromFile(ProjectDataBaseConstants.AppDataDirectory +
                ProjectDataBaseConstants.SystemDatabaseName +
                ProjectDataBaseConstants.DefaultDocumentName);
            this.pictureBox3.Image = Image.FromFile(ProjectDataBaseConstants.AppDataDirectory +
                ProjectDataBaseConstants.SystemDatabaseName +
                ProjectDataBaseConstants.DefaultDocumentName);

            // Initialize the active database
            this.InitializeActiveDataBase();

            // Security: Warn user that there is no log in for the default active database and recommend creating a new one.
            //  Any person has read write permissions to the default database.
            if (this.activeDatabase.DatabaseName == ProjectDataBaseConstants.AppDataDirectory +
                ProjectDataBaseConstants.SystemDatabaseName +
                ProjectDataBaseConstants.DefaultActiveDatabaseName)
            {
                this.DisplayMessage(
                           SecurityMessages.WarningMessageTitle,
                           SecurityMessages.DefaultActiveDatabaseWarning,
                           MessageBoxButtons.OK);
            }

            // Allows for smoother painting of images in different tabs.
            this.DoubleBuffered = true;

            // Set global bools to false
            this.editingTemplateTabCell = false;
            this.priorityChanged = false;
            this.keyPressed = false;

            // Initialize the job ticket tab
            this.InitializeJobTicketTab();
        }

        /// Setter and Getter for user and user privilege
        private string UserPrivilege
        {
            get
            {
                return this.userPrivilege;
            }
            set
            {
                if (this.userPrivilege != value)
                {
                    this.userPrivilege = value;
                    this.NotifyUserPropertyChanged();
                }
            }
        }
        /// Setter and Getter for username
        private string UserLoggedIn
        {
            get
            {
                return this.userLoggedIn;
            }
            set
            {
                if (this.userLoggedIn != value)
                {
                    this.userLoggedIn = value;
                    this.NotifyUserPropertyChanged();
                }
            }
        }

        // ************************* General Functions *************************
        /// <summary>
        ///  Initializes the active database with basic folders to accomodate template and ticket creation.
        /// </summary>
        private void InitializeActiveDataBase()
        {
            // Create directories for templates and job tickets. Do this only if they don't exist.
            if (Directory.Exists(this.activeDatabase.DatabaseName + "\\Job Tickets") == false)
            {
                Directory.CreateDirectory(this.activeDatabase.DatabaseName + "\\Job Tickets");
            }
            if (Directory.Exists(this.activeDatabase.DatabaseName + "\\Templates") == false)
            {
                Directory.CreateDirectory(this.activeDatabase.DatabaseName + "\\Templates");
            }

            // Change the textbox holding the filepath of the database to match the current active database
            // -Adjust size to contain new text
            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                SizeF textSize = g.MeasureString(this.activeDatabase.DatabaseName, this.toolStripTextBox2.Font);
                this.toolStripTextBox2.Size = new Size(new Point((int)textSize.Width, (int)this.toolStripTextBox2.Size.Height));
            }
            // -Set the textbox text
            this.toolStripTextBox2.Text = this.activeDatabase.DatabaseName;
        }

        /// <summary>
        ///  Prints the currently selected job ticket in the job ticket tab.
        /// </summary>
        private void PrintSelectedJobTicket()
        {
            // If there is a cell selected and it's in column 3 in datagridview1, edit the selected
            // job ticket
            if (this.dataGridView1.SelectedCells.Count == 1)
            {
                if (this.dataGridView1.SelectedCells[0].ColumnIndex == 3)
                {
                    try
                    {
#pragma warning disable CS8604 // Possible null reference argument.
                        this.PrintJobTicket(
                            this.activeDatabaseJobTickets[this.jobTicketFilePaths[this.dataGridView1.SelectedCells[0].Value.ToString()]],
                            this.pictureBox3.Image);
#pragma warning restore CS8604 // Possible null reference argument.
                    }
                    catch
                    {
                        MessageBox.Show(ErrorMessages.PrintTicketFailedToPrintMessage, ErrorMessages.PrintTicketErrorTitle, MessageBoxButtons.OK);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(ErrorMessages.PrintTicketNoJobTicketSelectedMessage, ErrorMessages.PrintTicketErrorTitle, MessageBoxButtons.OK);
                    return;
                }
            }
            else
            {
                MessageBox.Show(ErrorMessages.PrintTicketNoJobTicketSelectedMessage,ErrorMessages.PrintTicketErrorTitle, MessageBoxButtons.OK);
                return;
            }
        }

        /// <summary>
        ///  Opens a print dialog box and tries to print the inputted job ticket.
        /// </summary>
        private void PrintJobTicket(JobTicket source, Image sourceImage, double imageCompression = 0.5, double xCompression = 2.06, double yCompression = 1.88)
        {
            // Turn the image into a bitmap followed by drawable graphics.
            Bitmap bs = new Bitmap(sourceImage, new Size((int)(sourceImage.Width * imageCompression), (int)(sourceImage.Height * imageCompression)));
            Graphics g = Graphics.FromImage(bs);

            // Draw contents of job ticket onto graphics
            // Static Objects
            for (int i = 0; i < source.JobStaticObjects.Count; i++)
            {
                // Customer first name case
                if (source.JobStaticObjects[i].Name.Contains(InformationObjectConstants.CustomerFirstName))
                {
                    g.DrawString(
                    source.CustomerFirstName,
                    PaintingConstants.Arial((int)(source.JobStaticObjects[i].FontSize * yCompression)),
                    Brushes.Black,
                    new Point(
                        (int)((source.JobStaticObjects[i].X * xCompression)),
                        (int)(source.JobStaticObjects[i].Y * yCompression))
                    );
                }
                // Customer last name case
                else if (source.JobStaticObjects[i].Name.Contains(InformationObjectConstants.CustomerLastName))
                {
                    g.DrawString(
                    source.CustomerLastName,
                    PaintingConstants.Arial((int)(source.JobStaticObjects[i].FontSize * yCompression)),
                    Brushes.Black,
                    new Point(
                        (int)((source.JobStaticObjects[i].X * xCompression)),
                        ((int)(source.JobStaticObjects[i].Y * yCompression)))
                    );
                }
                // All other cases
                else
                {
                    g.DrawString(
                    source.JobStaticObjects[i].Text,
                    PaintingConstants.Arial((int)(source.JobStaticObjects[i].FontSize * yCompression)),
                    Brushes.Black,
                    new Point(
                        (int)((source.JobStaticObjects[i].X * xCompression)),
                        ((int)(source.JobStaticObjects[i].Y * yCompression)))
                    );
                }

            }
            // Texboxes
            for (int i = 0; i < source.JobTextboxes.Count; i++)
            {
                string[] drawStrings = source.JobTextboxes[i].Text.Split("\n");
                for (int j = 0; j < drawStrings.Length; j++)
                {
                    g.DrawString(
                    drawStrings[j],
                    PaintingConstants.Arial((int)(source.JobTextboxes[i].FontSize * yCompression)),
                    Brushes.Black,
                    new Point(
                        (int)((source.JobTextboxes[i].X) * xCompression),
                        (int)((source.JobTextboxes[i].Y) * yCompression) + (j * (PaintingConstants.Arial((int)(source.JobTextboxes[i].FontSize * yCompression)).Height)))
                    );
                }

            }
            // Checkboxes
            for (int i = 0; i < source.JobCheckboxes.Count; i++)
            {
                // Create the rectangle based on the checkbox
                Rectangle checkboxRectangle = new Rectangle(
                    (int)((source.JobCheckboxes[i].X) * xCompression),
                    (int)(source.JobCheckboxes[i].Y * yCompression),
                    (int)(source.JobCheckboxes[i].Scale * InformationCheckbox.CheckboxDefaultEdgeLength * xCompression),
                    (int)(source.JobCheckboxes[i].Scale * InformationCheckbox.CheckboxDefaultEdgeLength * yCompression));

                // Draw a checkbox outline.
                g.DrawRectangle(PaintingConstants.BlackPen(), checkboxRectangle);

                // If true, draw an x within the rectangle.
                if (source.JobCheckboxes[i].Status == true)
                {
                    // Old code, fills in the rectangle instead of making an x.
                    //g.FillRectangle(Brushes.Black, checkboxRectangle);

                    // Create an x since the checkbox is filled in.
                    g.DrawLine(
                        PaintingConstants.BlackPen(),
                        new Point(
                            (int)(source.JobCheckboxes[i].X * xCompression),
                            (int)(source.JobCheckboxes[i].Y * yCompression)),
                        new Point(
                            (int)(source.JobCheckboxes[i].X * xCompression) + (int)(source.JobCheckboxes[i].Scale * InformationCheckbox.CheckboxDefaultEdgeLength * xCompression),
                            (int)(source.JobCheckboxes[i].Y * yCompression) + (int)(source.JobCheckboxes[i].Scale * InformationCheckbox.CheckboxDefaultEdgeLength * yCompression))
                        );
                    g.DrawLine(
                        PaintingConstants.BlackPen(),
                        new Point(
                            (int)(source.JobCheckboxes[i].X * xCompression) + (int)(source.JobCheckboxes[i].Scale * InformationCheckbox.CheckboxDefaultEdgeLength * xCompression),
                            (int)(source.JobCheckboxes[i].Y * yCompression)),
                        new Point(
                            (int)(source.JobCheckboxes[i].X * xCompression),
                            (int)(source.JobCheckboxes[i].Y * yCompression) + (int)(source.JobCheckboxes[i].Scale * InformationCheckbox.CheckboxDefaultEdgeLength * yCompression))
                        );

                }
            }

            // Save temporary file for use in document.
            bs.Save(Path.GetFullPath(this.activeDatabase.DatabaseName) + "\\" + source.CustomerFirstName + "tempTicketImage" + ".png", ImageFormat.Png);

            // Set up application instance that will save our ticket.
            Microsoft.Office.Interop.Word.Application saveDocumentApp = new Microsoft.Office.Interop.Word.Application();
            saveDocumentApp.Visible = true;
            saveDocumentApp.DisplayAlerts = WdAlertLevel.wdAlertsNone;

            // Create a document to save the image onto. Adjust small margins to house new image.
            Microsoft.Office.Interop.Word.Document jobTicketDocument = saveDocumentApp.Documents.Add();
            // 36 points = 0.5 inches as 72 points = 1 inch
            jobTicketDocument.PageSetup.TopMargin = 36;
            jobTicketDocument.PageSetup.BottomMargin = 36;
            jobTicketDocument.PageSetup.LeftMargin = 36;
            jobTicketDocument.PageSetup.RightMargin = 36;

            // Add picture to document.
            jobTicketDocument.Shapes.AddPicture(this.activeDatabase.DatabaseName + "\\" + source.CustomerFirstName + "tempTicketImage" + ".png");

            // Save document to active database
            jobTicketDocument.SaveAs((Path.GetFullPath(this.activeDatabase.DatabaseName) + "\\" + "temporaryJobTicket.docx"));

            // Print job Ticket if the user selected ok.
            //saveDocumentApp.Activate();
            Dialog printDialog = saveDocumentApp.Dialogs[WdWordDialog.wdDialogFilePrint];
            if (1 == printDialog.Show())
            {
                jobTicketDocument.PrintOut(Background: false, Pages: 1);
            }

            // Close the document and quit out of word when done
            jobTicketDocument.Close(SaveChanges: true);
            saveDocumentApp.Quit(SaveChanges: true);

            // Delete temporary image file as it's no longer required for use.
            if (File.Exists((this.activeDatabase.DatabaseName) + "\\" + source.CustomerFirstName + "tempTicketImage" + ".png"))
            {
                File.Delete((this.activeDatabase.DatabaseName) + "\\" + source.CustomerFirstName + "tempTicketImage" + ".png");
            }

            // Delete document when done
            if (File.Exists(Path.GetFullPath(this.activeDatabase.DatabaseName) + "\\" + "temporaryJobTicket.docx") == true)
            {
                File.Delete(Path.GetFullPath(this.activeDatabase.DatabaseName) + "\\" + "temporaryJobTicket.docx");
            }
        }

        /// <summary>
        ///  This function handles changing and reinitializing a database based on user input.
        /// </summary>
        private void ChangeActiveDatabase()
        {
            // Show a dialog for choosing a folder. Make it start on the current active database.
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = this.activeDatabase.DatabaseName;
            // If the user selects a path, change the active database to that path. Then reinitialize the active database under that
            //  selected path.
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                // Store active database in case entering the database fails
                string oldActiveDatabaseStorage = this.activeDatabase.DatabaseName;

                // Set database path
                this.activeDatabase.DatabaseName = folderBrowserDialog.SelectedPath;

                // Security: Warn user that there is no log in for the default active database and recommend creating a new one.
                //  Any person has read write permissions to the default database.
                if (this.activeDatabase.DatabaseName == ProjectDataBaseConstants.AppDataDirectory +
                    ProjectDataBaseConstants.SystemDatabaseName +
                    ProjectDataBaseConstants.DefaultActiveDatabaseName)
                {
                    this.DisplayMessage(
                        SecurityMessages.WarningMessageTitle,
                        SecurityMessages.DefaultActiveDatabaseWarning,
                        MessageBoxButtons.OK);
                }

                // If the database is not the default database, look for a user file. If the user file doesn't exist, initialize it and request
                //  admin name and login password.
                else
                {
                    // If the user database doesn't exist, create a new admin and proceed afterwards
                    if (this.activeDatabase.UserDatabaseInitialized == false)
                    {
                        // Attempt to initialize the new userdatabase. If unsuccessful, return to initial database.
                        if (this.AddNewUserToActiveDatabase(true) == false)
                        {
                            this.activeDatabase = new ActiveDatabase(oldActiveDatabaseStorage);
                        }
                    }
                    // Try to log in. Log in only if user database file is available
                    else
                    {
                        // Try to log in first, but have the option to change to creating a new user as an option.
                        bool loggingIn = true;
                        bool creatingNewUser = false;

                        // Login screen
                        LoginWindow loginWindow = new LoginWindow();
                        loginWindow.DescriptionTextbox = SecurityMessages.LoginMessage(this.activeDatabase.DatabaseName);

                        // Create screen variables
                        do
                        {
                            // Log in case
                            if (loggingIn)
                            {
                                // Attempt to log in. Proceed based on output
                                loginWindow.ShowDialog();

                                // If cancelled, set all to false and return. Set old database as active database
                                if (loginWindow.CancelButtonPressed)
                                {
                                    this.activeDatabase.DatabaseName = oldActiveDatabaseStorage;
                                    loggingIn = false;
                                    creatingNewUser = false;
                                }
                                // If new user, switch to creating a new user
                                else if (loginWindow.NewUserButtonPressed)
                                {
                                    creatingNewUser = true;
                                    loggingIn = false;
                                }
                                // If username and password given, try to log in. Proceed based on result
                                else if (loginWindow.SubmitButtonPressed && loginWindow.Success)
                                {
                                    switch(this.activeDatabase.Login(loginWindow.EnteredUsername, loginWindow.EnteredPassword))
                                    {
                                        // Logged in successfully as admin
                                        case (0):
                                            // Initialize database settings
                                            this.activeDatabase.InitializeSettings();

                                            loggingIn = false;
                                            creatingNewUser = false;
                                            this.UserLoggedIn = loginWindow.EnteredUsername;
                                            this.UserPrivilege = SecurityConstants.AdminUser;
                                            break;
                                        // Logged in successfully as guest
                                        case (1):

                                            // Initialize database settings
                                            this.activeDatabase.InitializeSettings();

                                            loggingIn = false;
                                            creatingNewUser = false;
                                            this.UserLoggedIn = loginWindow.EnteredUsername;
                                            this.UserPrivilege = SecurityConstants.GuestUser;
                                            break;
                                        case (2):
                                            DisplayMessage(ErrorMessages.LoginErrorTitle, ErrorMessages.LoginFailedUserDatabaseMissing, MessageBoxButtons.OK);
                                            break;
                                        case (3):
                                            DisplayMessage(ErrorMessages.LoginErrorTitle, ErrorMessages.LoginFailedUsernameNotFound, MessageBoxButtons.OK);
                                            break;
                                        case (4):
                                            DisplayMessage(ErrorMessages.LoginErrorTitle, ErrorMessages.LoginFailedIncorrectPassword, MessageBoxButtons.OK);
                                            break;
                                        case (5):
                                            DisplayMessage(ErrorMessages.LoginErrorTitle, ErrorMessages.LoginFailedFileError, MessageBoxButtons.OK);
                                            break;
                                    }
                                }
                            }
                            // Create new user case
                            else
                            {
                                // Add new user. Return to logging in afterwards.
                                if (!this.AddNewUserToActiveDatabase(false))
                                {
                                    loggingIn = true;
                                    creatingNewUser = false;
                                }
                                else
                                {
                                    loggingIn = true;
                                    creatingNewUser = false;
                                }
                            }
                        } while (loggingIn || creatingNewUser);
                    }
                   
                }

                // Reinitialize
                this.InitializeJobTicketTab(true);        
            }
        }

        /// <summary>
        ///  Initializes the system database in this client's %appdata% folder.
        /// </summary>
        private void InitializeSystemDatabase()
        {
            // *****Create the directory for the system database*****
            try
            {
                if (!Directory.Exists(ProjectDataBaseConstants.AppDataDirectory + ProjectDataBaseConstants.SystemDatabaseName))
                {
                    Directory.CreateDirectory(ProjectDataBaseConstants.AppDataDirectory + ProjectDataBaseConstants.SystemDatabaseName);
                }
            }
            catch
            {
                MessageBox.Show(ErrorMessages.ApplicationErrorSystemDatabaseNotCreated, ErrorMessages.ApplicationErrorTitle, MessageBoxButtons.OK);
                this.Close();
            }



            // *****Draw and create the default document missing picture. Save afterwards.*****
            // -Initialize Graphics
            Bitmap graphicsBitmap = new Bitmap(1240, 1754);
            Graphics defaultMissingDocumentGraphics = Graphics.FromImage(graphicsBitmap);

            // -Draw red lines across document image
            for (int i = 0; i - 1240 < 1754; i += 50)
            {
                defaultMissingDocumentGraphics.DrawLine(PaintingConstants.RedPen(5), new Point(0, i), new Point(1240, i - 1240));
            }

            // -Draw black outline
            defaultMissingDocumentGraphics.DrawRectangle(PaintingConstants.BlackPen(10), new Rectangle(0, 0, 1240, 1754));

            // -Save the graphics as a file to the system database.
            graphicsBitmap.Save(
                ProjectDataBaseConstants.AppDataDirectory + 
                ProjectDataBaseConstants.SystemDatabaseName + 
                ProjectDataBaseConstants.DefaultDocumentName);



            // *****Initialize a default active database to be within the system database*****
            this.activeDatabase.DatabaseName = ProjectDataBaseConstants.AppDataDirectory +
                ProjectDataBaseConstants.SystemDatabaseName +
                ProjectDataBaseConstants.DefaultActiveDatabaseName;
            this.InitializeActiveDataBase();



            // *****Initialize the system database object*****
            this.systemDatabase.DatabaseName = ProjectDataBaseConstants.AppDataDirectory + ProjectDataBaseConstants.SystemDatabaseName;



            // *****Save initialized system database*****
            this.SystemDatabaseSave();


            // *****Return when done*****
            return;
        }

        /// <summary>
        ///  This function saves continous data between different instances of this application to the system database.
        /// </summary>
        private void SystemDatabaseSave()
        {
            // If the systemdata file doesn't exist, create an XML file for saving data to in the systemdatabase.
            // -Open and create the file
            Stream saveFileStream;
            try
            {
                saveFileStream = File.OpenWrite(ProjectDataBaseConstants.AppDataDirectory + ProjectDataBaseConstants.SystemDatabaseName + ProjectDataBaseConstants.ApplicationDataFileName);

                // -Make an XML Writer for that file.
                // --Settings for the XmlWriter once created
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "   ";
                settings.NewLineOnAttributes = true;
                settings.Encoding = Encoding.UTF8;

                // --XmlWriter used for saving the data.
                XmlWriter targetSaveFile;

                // --Attempt to open the file specified by "saveFileStream"
                targetSaveFile = XmlWriter.Create(saveFileStream, settings);

                // --Set file to empty.
                saveFileStream.SetLength(0);

                // --Start writing
                targetSaveFile.WriteStartDocument();

                // ---System database xml doc starting element
                targetSaveFile.WriteStartElement(ProjectDataBaseConstants.SystemXMLSystemDatabase);

                // ---active database name
                targetSaveFile.WriteStartElement(ProjectDataBaseConstants.SystemXMLActiveDatabase);
                targetSaveFile.WriteString(this.activeDatabase.DatabaseName);
                targetSaveFile.WriteEndElement();

                // ---End system database xml doc
                targetSaveFile.WriteEndElement();

                // --Stop writing
                targetSaveFile.WriteEndDocument();

                // --Flush and save the document
                targetSaveFile.Flush();
                targetSaveFile.Close();
                saveFileStream.Close();
            }
            catch
            {
                MessageBox.Show(ErrorMessages.ApplicationErrorSystemDataNotCreated, ErrorMessages.ApplicationErrorTitle, MessageBoxButtons.OK);
                this.Close();
            }         
        }

        /// <summary>
        ///  This function loads continous data between different instances of this application from the system database.
        /// </summary>
        private void SystemDatabaseLoad()
        {
            // Attempt to load the system database 
            try
            {
                // If the directory doesn't exist, create and load a new system database.
                if (!Directory.Exists(ProjectDataBaseConstants.AppDataDirectory + ProjectDataBaseConstants.SystemDatabaseName))
                {
                    this.InitializeSystemDatabase();
                }

                // Otherwise, check to see if the system data xml document exists.
                else
                {
                    // If it does, retrieve the stored data
                    if (!File.Exists(ProjectDataBaseConstants.AppDataDirectory + ProjectDataBaseConstants.SystemDatabaseName + ProjectDataBaseConstants.ApplicationDataFileName))
                    {
                        this.InitializeSystemDatabase();
                    }

                    // Otherwise, initialize the systemdatabase.
                    else
                    {
                        // Open the file where system data is stored for reading

                        // -Open Normal File Stream
                        Stream inputFileStream;
                        inputFileStream = File.OpenRead(ProjectDataBaseConstants.AppDataDirectory + ProjectDataBaseConstants.SystemDatabaseName + ProjectDataBaseConstants.ApplicationDataFileName);

                        // -Open XML File Stream from this
                        // Settings for Xml Reader
                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.IgnoreComments = true;
                        settings.IgnoreWhitespace = true;
                        XmlReader targetReadFile;
                        targetReadFile = XmlReader.Create(inputFileStream, settings);

                        // Read elements from datafile into the application
                        // -First element (Should be ProjectDataBaseConstants.SystemXMLSystemDatabase)
                        targetReadFile.ReadToFollowing(ProjectDataBaseConstants.SystemXMLSystemDatabase);

                        // -Active database
                        targetReadFile.ReadToFollowing(ProjectDataBaseConstants.SystemXMLActiveDatabase);
                        this.activeDatabase.DatabaseName = targetReadFile.ReadElementContentAsString();

                        // -Close when done
                        targetReadFile.Close();
                        inputFileStream.Close();
                    }   
                }
            }
            catch
            {
                MessageBox.Show(ErrorMessages.ApplicationErrorSystemDataNotRead, ErrorMessages.ApplicationErrorTitle, MessageBoxButtons.OK);
                this.Close();
            }
        }
        
        /// <summary>
        ///  When called, this function updates the main window to accomodate the new user's details
        /// </summary>
        private void UpdateUserSession(object? s, PropertyChangedEventArgs e)
        {
            // Update session header
            this.Text = MainWindowConstants.MainWindowTitle + " [Logged in as: {Username: " + this.userLoggedIn + "} {Privilege: " + this.userPrivilege + "}]"; 
            
            // If the user logged in is a guest, make the template and create tab invisible
            if (this.userPrivilege == SecurityConstants.GuestUser)
            {
                if (this.tabControl1.TabPages.Count == 3)
                {
                    this.tabControl1.TabPages.Remove(tabPage2);
                    this.tabControl1.TabPages.Remove(tabPage3);
                }

                // After logging in, since the user that logged in isn't an admin, set the Admin menu on the Job Tickets tab to unviewable
                this.menuStrip3.Items[3].Visible = false;

                // Update admin tab settings to match active database settings.
                if (this.activeDatabase != null)
                {
                    this.onToolStripMenuItem.Text = this.activeDatabase.GuestCanView.ToString();
                }

                // Set open button to be seen for an admin
                this.openToolStripMenuItem.Visible = true;
            }
            // If the user logged in is an admin, readd template and create tabs if they have been made invisible
            else if (this.userPrivilege == SecurityConstants.AdminUser)
            {
                if (this.tabControl1.TabPages.Count == 1)
                {
                    this.tabControl1.TabPages.Add(tabPage2);
                    this.tabControl1.TabPages.Add(tabPage3);
                }

                // After logging in, since the user that logged in isn't an admin, set the Admin menu on the Job Tickets tab to unviewable
                this.menuStrip3.Items[3].Visible = true;

                // Set open button to be unseen from a normal user
                this.openToolStripMenuItem.Visible = false;
            }
        }

        // ************************* Security Functions *************************

        /// <summary>
        ///  Checks to see if a user database exists in the current active database. If it doesn't and the active database isn't the default database, 
        ///     initialize the user database and prompt the user for an admin username and admin password.
        ///  If the database does exist, try to add a guest user to the database.
        /// </summary>
        /// <returns>
        ///     True if successfully created and/or added
        ///     False if not created and/or added
        /// </returns>
        private bool AddNewUserToActiveDatabase(bool admin = false)
        {
            if (this.activeDatabase != null)
            {
                // If the user database already exists, don't initialize with a new user
                try
                {
                    // Determines result of adding user.
                    int result; 

                    // Get first user as admin
                    UserCreateScreen newUserScreen = new UserCreateScreen();
                    newUserScreen.Label2 = "Password (Minimum Length = " + SecurityConstants.PasswordMinimumLength.ToString() + ")";
                    if (admin == true)
                    {
                        newUserScreen.UserPrivilege = SecurityConstants.AdminUser;
                    }
                    else
                    {
                        newUserScreen.UserPrivilege = SecurityConstants.GuestUser;
                    }
                    newUserScreen.Description = SecurityMessages.NewActiveDatabaseMessage;

                    // While the user isn't successfully created, continuously ask the 
                    while(newUserScreen.Success == false && newUserScreen.Cancel == false)
                    {
                        newUserScreen.ShowDialog();

                        // If cancelled, throw so that the database resets.
                        if (newUserScreen.Cancel == true)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            // Initialize new user database with the new admin.
                            // If unsuccessful, repeat.
                            result = this.activeDatabase.AddNewUser(
                                newUserScreen.Username,
                                newUserScreen.Password,
                                newUserScreen.UserPrivilege,
                                SecurityConstants.PasswordAndSaltLength
                                );   
                            
                            if (result > 1)
                            {
                                switch (result)
                                {
                                    case 2:
                                        MessageBox.Show(ErrorMessages.UsernameAlreadyExists, ErrorMessages.CreateUserErrorTitle, MessageBoxButtons.OK);
                                        break;
                                    case 3:
                                        MessageBox.Show(ErrorMessages.UserDatabaseFileError, ErrorMessages.CreateUserErrorTitle, MessageBoxButtons.OK);
                                        break;
                                }
                                newUserScreen.Success = false;
                            }
                        }
                    }

                    return true;
                    
                }
                catch
                {
                    // On fail, mark as unsuccessful.
                    return false;

                }
            }

            return false;
        }

        // ************************* Job Ticket Tab Functions *************************
        /// <summary>
        ///  Adds a row set up for DataGridView 1 (Job Ticket Tab datagridview)
        /// </summary>
        private void AddGridView1Row(
            string entry1,
            string entry2,
            string entry3,
            string entry4,
            bool firstEntryEditable = true,
            bool secondEntryEditable = true,
            bool thirdEntryEditable = true,
            bool fourthEntryEditable = true,
            Color? firstEntryColor = null,
            Color? secondEntryColor = null,
            Color? thirdEntryColor = null,
            Color? fourthEntryColor = null
            )
        {
            // If there are no columns, initialize the two required columns
            if (this.dataGridView1.Columns.Count == 0)
            {
                this.dataGridView1.Columns.Add("Column1", "First Name");
                this.dataGridView1.Columns.Add("Column2", "Last Name");
                this.dataGridView1.Columns.Add("Column3", "Date");
                this.dataGridView1.Columns.Add("Column2", "File Path");
            }

            // Add new row. Initialize entries with inputted strings.
            this.dataGridView1.Rows.Add();

            // Set properties of row according to inputted parameters

            // Cell 0
            this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[0].Value = entry1;
            if (firstEntryEditable == false)
            {
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[0].ReadOnly = true;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[1].Style.BackColor = Color.LightGray;
            }
            if (firstEntryColor != null)
            {
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[0].Style.BackColor = (Color)firstEntryColor;
            }

            // Cell 1
            this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[1].Value = entry2;
            if (secondEntryEditable == false)
            {
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[1].ReadOnly = true;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[1].Style.BackColor = Color.LightGray;
            }
            // Set background to user specified color
            if (secondEntryColor != null)
            {
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[1].Style.BackColor = (Color)secondEntryColor;
            }

            // Cell 2
            this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[2].Value = entry3;
            if (thirdEntryEditable == false)
            {
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[2].ReadOnly = true;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[2].Style.BackColor = Color.LightGray;
            }
            // Set background to user specified color
            if (thirdEntryColor != null)
            {
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[2].Style.BackColor = (Color)thirdEntryColor;
            }

            // Cell 3
            this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[3].Value = entry4;
            if (fourthEntryEditable == false)
            {
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[3].ReadOnly = true;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[3].Style.BackColor = Color.LightGray;
            }
            // Set background to user specified color
            if (fourthEntryColor != null)
            {
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[3].Style.BackColor = (Color)fourthEntryColor;
            }
        }

        /// <summary>
        ///  Initializes all data to the jobticket tab from the active database.
        /// </summary>
        private void InitializeJobTicketTab(bool forceUpdate = false)
        {
            // Initialize database in case it isn't already
            this.InitializeActiveDataBase();

            // Destroy any image on picturebox 3
            this.pictureBox3.Image = null;

            // Set the selected id to null
            this.selectedTicketID = null;

            // Get the number of files inside of the activedatabase. If the number is different than the previously recorded amount or the number is null, update
            //  the job ticket tab. Otherwise, exit.
            // If forceUpdate is set to true, then the datagridview will be updated no matter what.
            if (forceUpdate == false)
            {
                try
                {
                    if (this.jobTicketFilePaths.Count == Directory.GetFiles(this.activeDatabase.DatabaseName + ProjectDataBaseConstants.JobTicketDirectory).Length)
                    {
                        return;
                    }
                }
                catch
                {
                    return;
                }
            }
 
            // Clear datagridview 1
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Rows.Clear();

            // Clear previous dictionaries
            this.activeDatabaseJobTickets.Clear();
            this.jobTicketFilePaths.Clear();

            // If the logged in user is a guest, don't display anything and return
            if (this.userPrivilege == SecurityConstants.GuestUser)
            {
                return;
            }

            // Look at every file inside of the JobTicket folder. For each one, try and open and load data about the job ticket to the datagridview1.
            // Load Customer Name (First, Last), File Path, and Date Last Edited
            string[] JobTicketFilePaths = Directory.GetFiles(this.activeDatabase.DatabaseName + ProjectDataBaseConstants.JobTicketDirectory);

            // Temporary Job Ticket Holder
            JobTicket temporaryJobTicket;
            foreach (string FilePath in JobTicketFilePaths)
            {
                // File used to read in data.
                Stream? inputFile = null;
                try
                {
                    // Open a file path to the job ticket.
                    inputFile = File.OpenRead(FilePath);

                    // Load job ticket into a temporary variable
                    temporaryJobTicket = new JobTicket();
                    temporaryJobTicket.LoadTicket(inputFile);

                    // Load job ticket data onto datagridview1
                    this.AddGridView1Row(
                        temporaryJobTicket.CustomerFirstName,
                        temporaryJobTicket.CustomerLastName,
                        File.GetLastWriteTime(FilePath).ToString(),
                        FilePath,
                        false,
                        false,
                        false,
                        false,
                        Color.LightGray,
                        Color.LightGray,
                        Color.LightGray,
                        Color.Lime);

                    // Close file when done
                    inputFile.Close();
                    inputFile = null;

                    // Add path to job ticket document
                    this.jobTicketFilePaths.Add(FilePath, this.jobTicketFilePaths.Count);

                    // Add the job ticket to the job ticket dictionary
                    this.activeDatabaseJobTickets.Add(this.activeDatabaseJobTickets.Count, temporaryJobTicket);
                }
                catch
                {
                    if (inputFile != null)
                    {
                        inputFile.Close();
                        inputFile = null;
                    }
                }
            }
        }

        /// <summary>
        ///  This function creates an image based on the inputted job ticket in the form it would be printed it and returns it.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private Image? CreateJobTicketImage(JobTicket source)
        {
            // Try and pull the image of the source through it's document path. If it can't be done, return a blank image.
            if (File.Exists(source.DocumentPath) == false)
            {
                return null;
            }

            // Since the file exists, proceed with creating an image.
            // Initialize a new image and new graphics
            Image newImage = new Bitmap(1, 1);
            MemoryStream targetDocumentMemoryStream;
            dynamic targetPageBits;
            Microsoft.Office.Interop.Word.Page targetPage;

            // -Load base image from document.
            // Open the document associated with the template. If we fail to do so, return false.
            Microsoft.Office.Interop.Word.Application backgroundWordApp = new Microsoft.Office.Interop.Word.Application();
            backgroundWordApp.Visible = false;
            Microsoft.Office.Interop.Word.Document targetDocument = backgroundWordApp.Documents.Open(source.DocumentPath);
            foreach (Microsoft.Office.Interop.Word.Window window in targetDocument.Windows)
            {
                foreach (Microsoft.Office.Interop.Word.Pane pane in window.Panes)
                {
                    targetPage = pane.Pages[1];
                    targetPageBits = targetPage.EnhMetaFileBits;
                      
                    try
                    {
                        targetDocumentMemoryStream = new MemoryStream((byte[])(targetPageBits));
                        newImage = Image.FromStream(targetDocumentMemoryStream);

                        // Resize image to be 1/4th the size it normally is.
                        //   In the future if there are memory issues with loading images, uncomment this. This
                        //   will lower the quality of the image printed, but will lead to less crashing.
                        //newImage = newImage.GetThumbnailImage(1240, 1754, null, IntPtr.Zero);
                    }
                    catch
                    {
                        targetDocument.Close(Type.Missing, Type.Missing, Type.Missing);
                        backgroundWordApp.Quit(Type.Missing, Type.Missing, Type.Missing);
                        return null;
                    }
                }
            }
           
            // Close and return when finished.
            targetDocument.Close(Type.Missing, Type.Missing, Type.Missing);
            backgroundWordApp.Quit(Type.Missing, Type.Missing, Type.Missing);

            // Return the new image
            return newImage;
        }

        /// <summary>
        ///  Helper function for the modify and save ticket function.
        /// </summary>
        private void ModifyAndSaveSelectedTicket()
        {
            // If there is a cell selected and it's in column 3 in datagridview1, edit the selected
            // job ticket
            if (this.dataGridView1.SelectedCells.Count == 1)
            {
                if (this.dataGridView1.SelectedCells[0].ColumnIndex == 3)
                {
                    try
                    {
#pragma warning disable CS8604 // Possible null reference argument.
                        this.ModifyAndSaveTicket(this.activeDatabaseJobTickets[this.jobTicketFilePaths[this.dataGridView1.SelectedCells[0].Value.ToString()]], this.dataGridView1.SelectedCells[0].Value.ToString());
#pragma warning restore CS8604 // Possible null reference argument.
                    }
                    catch
                    {
                        MessageBox.Show(ErrorMessages.ModifyTicketFailedToModifyTicketMessage, ErrorMessages.ModifyTicketErrorTitle, MessageBoxButtons.OK);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(ErrorMessages.ModifyTicketNoJobTicketSelectedMessage, ErrorMessages.ModifyTicketErrorTitle, MessageBoxButtons.OK);
                    return;
                }
            }
            else
            {
                MessageBox.Show(ErrorMessages.ModifyTicketNoJobTicketSelectedMessage, ErrorMessages.ModifyTicketErrorTitle, MessageBoxButtons.OK);
                return;
            }
        }

        /// <summary>
        ///  This function creates a window where the user can edit entries inside of the jobticket.
        /// </summary>
        /// <param name="source"></param>
        private void ModifyAndSaveTicket(JobTicket source, string sourceFilePath)
        {
            // Get inputs from the user corresponding to all template information objects. Inputs recieved from CreateTicketWindow class instance.
            CreateTicketWindow creationWindow = new CreateTicketWindow(source);
            creationWindow.ShowDialog();

            // If the creation window was not completed by the user, exit out. Otherwise, continue.
            if (creationWindow.Complete == true)
            {
                // Bring in customer name information from the creation window used.
                if (creationWindow.CustomerFirstName != null && creationWindow.CustomerLastName != null)
                {
                    source.CustomerFirstName = creationWindow.CustomerFirstName;
                    source.CustomerLastName = creationWindow.CustomerLastName;
                }

                // Set the job ticket's information object entries based on the window's entries
                int currentIndex = 0;
                // -Textboxes
                for (currentIndex = 0; currentIndex < creationWindow.InputTextboxes.Count; currentIndex++)
                {
                    source.SetTexboxText(creationWindow.InputTextboxes[currentIndex].Name, creationWindow.InputTextboxes[currentIndex].Text);
                }
                // -Checkboxes
                for (currentIndex = 0; currentIndex < creationWindow.InputCheckboxes.Count; currentIndex++)
                {
                    if (creationWindow.InputCheckboxes[currentIndex].Text == CreationWindowConstants.ComboBoxDefault ||
                        creationWindow.InputCheckboxes[currentIndex].Text == CreationWindowConstants.ComboBoxFalse)
                    {
                        source.SetCheckboxStatus(creationWindow.InputCheckboxes[currentIndex].Name, false);
                    }
                    else
                    {
                        source.SetCheckboxStatus(creationWindow.InputCheckboxes[currentIndex].Name, true);
                    }
                }

                // Save jobTicket as an xml file in the current active database's job ticket folder
                // Initialize the active database just in case the folders don't exist
                this.InitializeActiveDataBase();
                // Open a file to save to
                // If we fail to open it, show an error message and return
                Stream saveFileStream;
                try
                {
                    saveFileStream = File.OpenWrite(sourceFilePath);
                    if (source.SaveTicket(saveFileStream) == false)
                    {
                        MessageBox.Show(ErrorMessages.ModifyTicketFailedToSaveFileMessage, ErrorMessages.ModifyTicketErrorTitle, MessageBoxButtons.OK);
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show(ErrorMessages.ModifyTicketFailedToSaveFileMessage, ErrorMessages.ModifyTicketErrorTitle, MessageBoxButtons.OK);
                    return;
                }

                // Close file when done
                saveFileStream.Close();

                // Let the user know the file was saved successfully
                MessageBox.Show("Ticket successfully modified and saved.", "Modify Ticket", MessageBoxButtons.OK);

                // Force Reinitialize the job ticket datagridview
                this.InitializeJobTicketTab(true);

                // Return when done.
                return;
            }
            else
            {
                return;
            }
        }

        /// <summary>
        ///  Helper function for the delete ticket function.
        /// </summary>
        private void DeleteSelectedTicket()
        {
            // If there is a cell selected and it's in column 3 in datagridview1, edit the selected
            // job ticket
            if (this.dataGridView1.SelectedCells.Count == 1)
            {
                if (this.dataGridView1.SelectedCells[0].ColumnIndex == 3)
                {
                    try
                    {
                        // Prompt to make sure the user wants to delete the ticket. If they are sure, delete the selected ticket.
                        if (MessageBox.Show("Are you sure you want to delete the selected Job Ticket?", "Delete Ticket", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
#pragma warning disable CS8604 // Possible null reference argument.
                            this.DeleteTicket(this.dataGridView1.SelectedCells[0].Value.ToString());
#pragma warning restore CS8604 // Possible null reference argument.
                        }
                    }
                    catch
                    {
                        MessageBox.Show(ErrorMessages.DeleteTicketFileNotFoundMessage, ErrorMessages.DeleteTicketErrorTitle, MessageBoxButtons.OK);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(ErrorMessages.DeleteTicketNoJobTicketSelectedMessage, ErrorMessages.DeleteTicketErrorTitle, MessageBoxButtons.OK);
                    return;
                }
            }
            else
            {
                MessageBox.Show(ErrorMessages.DeleteTicketNoJobTicketSelectedMessage, ErrorMessages.DeleteTicketErrorTitle, MessageBoxButtons.OK);
                return;
            }
        }

        /// <summary>
        ///  Deletes a job ticket given by the specific path given it exists in the saved job ticket file paths.
        /// </summary>
        /// <param name="sourceFilePath"></param>
        private void DeleteTicket(string sourceFilePath)
        {
            // Check to see whether the source exists.
            try
            {
                // Try to see if the id exists and if it's a valid job ticket id.
                if (this.jobTicketFilePaths[sourceFilePath] < 0)
                {
                    // If not found, tell user that the file requested is not in the active database.
                    MessageBox.Show(ErrorMessages.DeleteTicketFileNotFoundMessage, ErrorMessages.DeleteTicketErrorTitle, MessageBoxButtons.OK);
                    return;
                }
            }
            catch
            {
                // If not found, tell user that the file requested is not in the active database.
                MessageBox.Show(ErrorMessages.DeleteTicketFileNotFoundMessage, ErrorMessages.DeleteTicketErrorTitle, MessageBoxButtons.OK);
                return;
            }

            // Since we know the file exists at this point, delete it.
            try
            {
                // Try and delete the file
                File.Delete(sourceFilePath);
            }
            catch
            {
                // If the deletion failed, let the user know
                MessageBox.Show(ErrorMessages.DeleteTicketFailedToDeleteMessage, ErrorMessages.DeleteTicketErrorTitle, MessageBoxButtons.OK);
                return;
            }

            // Reload the job ticket tab
            this.InitializeJobTicketTab(true);       
        }

        /// <summary>
        ///  Streamlined message display. 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        private void DisplayMessage(string title, string message, MessageBoxButtons buttons)
        {
            MessageBox.Show(message, title, buttons);
        }

        // ************************* Template Tab Functions *************************
        /// <summary>
        ///  Adds a row set up for DataGridView 2 (Template Tab datagridview)
        /// </summary>
        private void AddGridView2Row(string entry1, string entry2, bool secondEntryEditable = true, Color? firstEntryColor = null, Color? secondEntryColor = null)
        {
            // If there are no columns, initialize the two required columns
            if (this.dataGridView2.Columns.Count == 0)
            {
                this.dataGridView2.Columns.Add("Column1", "Property Name");
                this.dataGridView2.Columns.Add("Column2", "Value");

                // Make columns static (unsortable)
                this.dataGridView2.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dataGridView2.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // Add new row. Initialize entries with inputted strings.
            this.dataGridView2.Rows.Add();

            // Entry 1 is read only and represents the property name. Highlighted Yellow or user specified color
            this.dataGridView2.Rows[this.dataGridView2.Rows.Count - 1].Cells[0].Value = entry1;
            this.dataGridView2.Rows[this.dataGridView2.Rows.Count - 1].Cells[0].ReadOnly = true;
            if (firstEntryColor != null)
            {
                this.dataGridView2.Rows[this.dataGridView2.Rows.Count - 1].Cells[0].Style.BackColor = (Color)firstEntryColor;
            }
            else
            {
                this.dataGridView2.Rows[this.dataGridView2.Rows.Count - 1].Cells[0].Style.BackColor = Color.Yellow;
            }

            // Entry 2 is editable and represents the value associated with the property. Read only if "secondEntryEditable" = false
            this.dataGridView2.Rows[this.dataGridView2.Rows.Count - 1].Cells[1].Value = entry2;
            if (secondEntryEditable == false)
            {
                this.dataGridView2.Rows[this.dataGridView2.Rows.Count - 1].Cells[1].ReadOnly = true;

                // Set background to gray
                this.dataGridView2.Rows[this.dataGridView2.Rows.Count - 1].Cells[1].Style.BackColor = Color.LightGray;
            }
            // Set background to user specified color
            if (secondEntryColor != null)
            {
                this.dataGridView2.Rows[this.dataGridView2.Rows.Count - 1].Cells[1].Style.BackColor = (Color)secondEntryColor;
            }

        }

        /// <summary>
        ///  This function is responsible for creating a tickettemplate based on user input and returning said template.
        /// </summary>
        /// <returns></returns>
        private TicketTemplate? CreateTemplateTabTemplate()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Microsoft Word Document (*.docx)|*.docx";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the document file path.
                string targetDocumentPath = openFileDialog.FileName;

                // Copy the file into the active database.

                // a) Get file name
                string targetDocumentName = Path.GetFileName(openFileDialog.FileName);

                // b) Make a new file path based on the name and the active database
                string newDocumentPath = Path.Combine(this.activeDatabase.DatabaseName, targetDocumentName);
                newDocumentPath = Path.GetFullPath(newDocumentPath);

                // c) Copy the file over if it needs to be copied over.
                try
                {
                    if (openFileDialog.FileName != newDocumentPath)
                    {
                        File.Copy(openFileDialog.FileName, newDocumentPath, true);
                    }
                }
                catch
                {
                    MessageBox.Show(ErrorMessages.ImageLoadDocumentCurrentlyOpenedMessage, ErrorMessages.ImageLoadErrorTitle, MessageBoxButtons.OK);
                    return null;
                }

                // d) Create a ticket template. Set the name to be the file name of the original + " template";
                TicketTemplate newTemplate = new TicketTemplate(newDocumentPath);
                newTemplate.Name = targetDocumentName + " Template";

                // e) Have the LoadTemplate function subscribe to the template changed properties
                newTemplate.TemplatePropertyChanged += this.ActiveTemplateChangedEventHandler;
                newTemplate.TextboxPropertyChanged += this.RedrawTemplateData;
                newTemplate.CheckboxPropertyChanged += this.RedrawTemplateData;
                newTemplate.StaticObjectPropertyChanged += this.RedrawTemplateData;


                // f) Return the newly created template
                return newTemplate;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///  This function creates and loads assets of a blank template into view.
        /// </summary>
        private void CreateLoadTemplateTabTemplate()
        {
            /// 1) Close active template if open. Save template to a folder of the user's choice.
            if (this.activeTemplate != null)
            {
                // Save
                this.SaveTemplate();

                // Set values of template to empty.
                this.activeTemplate = null;
                this.activeTemplateSavedFilePath = null;
            }

            /// 2 + 3) Ask for a word document to base the template on. If a file is chosen, make a copy of it and move it to the active ticket database.
            TicketTemplate? holderTemplate = this.CreateTemplateTabTemplate();

            // If no document was chosen, return here. Otherwise continue.
            if (holderTemplate == null)
            {
                return;
            }

            // 4) Now that the template has been created, we need to load it into view.
            //  a) Unsubscribe old template from functions in code. Do this only if there is a template in place.
            //  b) Set the active template as the returned template from the CreateTemplateTabTemplate() function.
            //  c) Convert the first page of the document into an image, then load it into view.
            //  d) Load assets of the template onto the left side panel of the window.
            //  e) Load assets of the template onto the right picture box of the window.

            // 4.a
            if (this.activeTemplate != null)
            {
                this.activeTemplate.TemplatePropertyChanged -= this.ActiveTemplateChangedEventHandler;
                this.activeTemplate.TextboxPropertyChanged -= this.RedrawTemplateData;
                this.activeTemplate.CheckboxPropertyChanged -= this.RedrawTemplateData;
                this.activeTemplate.StaticObjectPropertyChanged -= this.RedrawTemplateData;
            }

            // 4.b
            this.activeTemplate = holderTemplate;

            // 4.c
            // If we fail to load, return here. Otherwise, continue.
            if (this.LoadTemplateTabImage(this.activeTemplate) != true)
            {
                MessageBox.Show(ErrorMessages.CreateTemplateImageMissingErrorMessage, ErrorMessages.CreateTemplateErrorTitle, MessageBoxButtons.OK);
                this.pictureBox1.Image = Image.FromFile(this.systemDatabase.DatabaseName + "fail_to_load_image_graphic");
                return;
            }
            // Load image into picturebox.
            this.pictureBox1.Image = this.activeTemplateImage;

            // 4.d + e
            // Load template properties into left panel as well as images onto right panel
            this.LoadTemplateData(this.activeTemplate);
        }

        /// <summary>
        ///  This function creates and loads assets of an existing template of the user's choice into view.
        /// </summary>
        private void OpenLoadTemplateTabTemplate(string? filePath = null, bool saveOldTemplate = true)
        {
            // Initialize the databases in case they haven't
            this.InitializeActiveDataBase();

            // If there's no inputted filePath, prompt the user for one.
            if (filePath == null)
            {
                // Prompt the user for a file to open from. If they select one, continue to try and load from it.
                OpenFileDialog openTemplateDialog = new OpenFileDialog();
                openTemplateDialog.Filter = "XML files(.xml)| *.xml";
                openTemplateDialog.InitialDirectory = this.activeDatabase.DatabaseName + ProjectDataBaseConstants.TemplateDirectory;
                if (openTemplateDialog.ShowDialog() == DialogResult.OK)
                {
                    TicketTemplate loadedTemplate = new TicketTemplate("");

                    Stream targetFile;
                    try
                    {
                        // Attempt to open the chosen file for read and load in the template from there. If it fails, tell the user and return.
                        targetFile = File.OpenRead(Path.GetFullPath(openTemplateDialog.FileName));
                        if (loadedTemplate.LoadTemplate(targetFile) == false)
                        {
                            MessageBox.Show(ErrorMessages.CreateTemplateFailedToLoadFileMessage, ErrorMessages.CreateTemplateErrorTitle, MessageBoxButtons.OK);
                            return;
                        }

                        // Have the active template subscribe to relevant event handlers.
                        loadedTemplate.TemplatePropertyChanged += this.ActiveTemplateChangedEventHandler;
                        loadedTemplate.TextboxPropertyChanged += this.RedrawTemplateData;
                        loadedTemplate.CheckboxPropertyChanged += this.RedrawTemplateData;
                        loadedTemplate.StaticObjectPropertyChanged += this.RedrawTemplateData;

                        // Close active template if open.Save template to a folder of the user's choice.
                        if (this.activeTemplate != null)
                        {
                            if (saveOldTemplate)
                            {
                                // Save
                                this.SaveTemplate();
                            }

                            // Unsubscribe from window event handlers
                            this.activeTemplate.TemplatePropertyChanged -= this.ActiveTemplateChangedEventHandler;
                            this.activeTemplate.TextboxPropertyChanged -= this.RedrawTemplateData;
                            this.activeTemplate.CheckboxPropertyChanged -= this.RedrawTemplateData;
                            this.activeTemplate.StaticObjectPropertyChanged -= this.RedrawTemplateData;

                            // Set values of template to empty.
                            this.activeTemplate = null;
                            this.activeTemplateSavedFilePath = null;
                        }

                        // Set the active template as the newly loaded one.
                        this.activeTemplate = loadedTemplate;
                        this.activeTemplateSavedFilePath = Path.GetFullPath(openTemplateDialog.FileName);

                        // If the last write time is different than the current year, reset each counter that might exist in the template.
                        string templateYear = File.GetLastWriteTime(openTemplateDialog.FileName).ToString().Split('/')[2];
                        templateYear = templateYear.Remove(4, templateYear.Length - 4);
                        string currentYear = DateTime.Now.ToString().Split('/')[2];
                        currentYear = currentYear.Remove(4, currentYear.Length - 4);
                        if (currentYear != templateYear)
                        {
                            this.activeTemplate.ResetCounters();
                        }

                        // Load template data and image
                        if (this.LoadTemplateTabImage(this.activeTemplate) != true)
                        {
                            MessageBox.Show(ErrorMessages.CreateTemplateImageMissingErrorMessage, ErrorMessages.CreateTemplateErrorTitle, MessageBoxButtons.OK);
                            this.pictureBox1.Image = Image.FromFile(this.systemDatabase.DatabaseName + "fail_to_load_image_graphic");
                            return;
                        }
                        // Load image into picturebox.
                        this.pictureBox1.Image = this.activeTemplateImage;

                        // Load template properties into left panel as well as images onto right panel
                        this.LoadTemplateData(this.activeTemplate);

                        // Close file after use.
                        targetFile.Close();
                    }
                    catch
                    {
                        MessageBox.Show(ErrorMessages.CreateTemplateFailedToLoadFileMessage, ErrorMessages.CreateTemplateErrorTitle, MessageBoxButtons.OK);
                        return;
                    }
                }     
            }
            else
            {
                TicketTemplate loadedTemplate = new TicketTemplate("");

                Stream targetFile;
                try
                {
                    // Attempt to open the chosen file for read and load in the template from there. If it fails, tell the user and return.
                    targetFile = File.OpenRead(filePath);
                    if (loadedTemplate.LoadTemplate(targetFile) == false)
                    {
                        MessageBox.Show(ErrorMessages.CreateTemplateFailedToLoadFileMessage, ErrorMessages.CreateTemplateErrorTitle, MessageBoxButtons.OK);
                        return;
                    }

                    // Have the active template subscribe to relevant event handlers.
                    loadedTemplate.TemplatePropertyChanged += this.ActiveTemplateChangedEventHandler;
                    loadedTemplate.TextboxPropertyChanged += this.RedrawTemplateData;
                    loadedTemplate.CheckboxPropertyChanged += this.RedrawTemplateData;
                    loadedTemplate.StaticObjectPropertyChanged += this.RedrawTemplateData;

                    // Close active template if open.Save template to a folder of the user's choice.
                    // Do not save
                    if (this.activeTemplate != null)
                    {
                        if (saveOldTemplate)
                        {
                            this.SaveTemplate();
                        }

                        // Unsubscribe from window event handlers
                        this.activeTemplate.TemplatePropertyChanged -= this.ActiveTemplateChangedEventHandler;
                        this.activeTemplate.TextboxPropertyChanged -= this.RedrawTemplateData;
                        this.activeTemplate.CheckboxPropertyChanged -= this.RedrawTemplateData;
                        this.activeTemplate.StaticObjectPropertyChanged -= this.RedrawTemplateData;

                        // Set values of template to empty.
                        this.activeTemplate = null;
                        this.activeTemplateSavedFilePath = null;
                    }

                    // Set the active template as the newly loaded one.
                    this.activeTemplate = loadedTemplate;
                    this.activeTemplateSavedFilePath = Path.GetFullPath(filePath);

                    // If the last write time is different than the current year, reset each counter that might exist in the template.
                    string templateYear = File.GetLastWriteTime(filePath).ToString().Split('/')[2];
                    templateYear = templateYear.Remove(4, templateYear.Length - 4);
                    string currentYear = DateTime.Now.ToString().Split('/')[2];
                    currentYear = currentYear.Remove(4, currentYear.Length - 4);
                    if (currentYear != templateYear)
                    {
                        this.activeTemplate.ResetCounters();
                    }

                    // Load template data and image
                    if (this.LoadTemplateTabImage(this.activeTemplate) != true)
                    {
                        MessageBox.Show(ErrorMessages.CreateTemplateImageMissingErrorMessage, ErrorMessages.CreateTemplateErrorTitle, MessageBoxButtons.OK);
                        this.pictureBox1.Image = Image.FromFile(this.systemDatabase.DatabaseName + "fail_to_load_image_graphic");
                        return;
                    }
                    // Load image into picturebox.
                    this.pictureBox1.Image = this.activeTemplateImage;

                    // Load template properties into left panel as well as images onto right panel
                    this.LoadTemplateData(this.activeTemplate);

                    // Close file after use.
                    targetFile.Close();
                }
                catch
                {
                    MessageBox.Show(ErrorMessages.CreateTemplateFailedToLoadFileMessage, ErrorMessages.CreateTemplateErrorTitle, MessageBoxButtons.OK);
                    return;
                }
            }
        }

        /// <summary>
        ///  If there is an opened template path, this function saves the current active template to that path.
        ///  Otherwise, this function calls SaveTemplateAs() to save the template to a new location.
        /// </summary>
        private void SaveTemplate(bool showErrorMessages = true)
        {
            // If there is no active template, return.
            if (this.activeTemplate == null)
            {
                if (showErrorMessages == true)
                {
                    MessageBox.Show(ErrorMessages.SaveTemplateMissingTemplateMessage, ErrorMessages.SaveTemplateErrorTitle, MessageBoxButtons.OK);
                }
                return;
            }

            // If there is no savedFilePath, call save as
            if (this.activeTemplateSavedFilePath == null)
            {
                this.SaveTemplateAs();
            }

            // If we are at this point, save the template to the active template's file path.
            // Save the file based on the activeTemplateSavedFilePath. If fail, say to user the file couldn't be opened or written to.
            try
            {
                // Open a filestream to the file being saved
#pragma warning disable CS8604 // Possible null reference argument.
                Stream targetFile = File.OpenWrite(this.activeTemplateSavedFilePath);
#pragma warning restore CS8604 // Possible null reference argument.

                // Save the template
                this.activeTemplate.SaveTemplate(targetFile);

                // Close file when done.
                targetFile.Close();
            }
            catch
            {
                MessageBox.Show(ErrorMessages.SaveTemplateOpenFileFailedMessage, ErrorMessages.SaveTemplateErrorTitle, MessageBoxButtons.OK);
                return;
            }
        }

        /// <summary>
        ///  Saves the current active template to a new file location if able.
        /// </summary>
        private void SaveTemplateAs()
        {
            // If there is no active template, don't allow the user to save
            if (this.activeTemplate == null)
            {
                MessageBox.Show(ErrorMessages.SaveTemplateMissingTemplateMessage, ErrorMessages.SaveTemplateErrorTitle, MessageBoxButtons.OK);
                return;
            }

            // Create a new savedFilePath for this system. Prompt the user for a new folder.
            // Allow user to navigate to a folder as the target to save it to.
            // Files saved in an xml file format.
            SaveFileDialog saveTemplateDialog = new SaveFileDialog();
            saveTemplateDialog.InitialDirectory = this.activeDatabase.DatabaseName + ProjectDataBaseConstants.TemplateDirectory;
            saveTemplateDialog.FileName = this.activeTemplate.Name;
            saveTemplateDialog.Filter = "XML files(.xml)| *.xml";

            // Based on the result, set the activeTemplateSavedFilePath to that.
            //  Do this only if the result same back as OK.
            //  Otherwise, show an error message box
            if (saveTemplateDialog.ShowDialog() == DialogResult.OK)
            {
                this.activeTemplateSavedFilePath = Path.GetFullPath(saveTemplateDialog.FileName);
                this.activeTemplate.Name = Path.GetFileName(saveTemplateDialog.FileName);
            }
            else
            {
                return;
            }

            // Save the file based on the activeTemplateSavedFilePath. If fail, say to user the file couldn't be opened or written to.
            try
            {
                // Open a filestream to the file being saved
                Stream targetFile = File.OpenWrite(this.activeTemplateSavedFilePath);

                // Save the template
                this.activeTemplate.SaveTemplate(targetFile);

                // Close file when done.
                targetFile.Close();
            }
            catch
            {
                MessageBox.Show(ErrorMessages.SaveTemplateOpenFileFailedMessage, ErrorMessages.SaveTemplateErrorTitle, MessageBoxButtons.OK);
                return;
            }
        }

        /// <summary>
        ///  Given a ticket template, this resets the template tab datagridview and then initializes it with all the information associated with the template.
        ///  It also draws the template's data onto the template image displayed.
        /// </summary>
        /// <param name="ticketTemplate">
        ///  Template to be loaded.
        /// </param>
        private void LoadTemplateData(TicketTemplate ticketTemplate)
        {
            // a) Reset datagridview2
            if (this.dataGridView2.Columns.Count > 0)
            {
                this.dataGridView2.Columns.Clear();
            }
            if (this.dataGridView2.Rows.Count > 0)
            {
                this.dataGridView2.Rows.Clear();
            }

            // b) Initialize the first rows and columns. Load the name of the template onto the first entry
            this.AddGridView2Row("Template Properties", "", false, Color.Gray, Color.Gray);
            this.AddGridView2Row(InformationObjectConstants.NameProperty, ticketTemplate.Name, false);

            // c) Initialize file path. Set editable to false for value
            this.AddGridView2Row(InformationObjectConstants.DocumentPathProperty, ticketTemplate.DocumentPath, false);

            // d) For each data object in the template, load it into the datagridview.
            using (Graphics g = this.pictureBox1.CreateGraphics())
            {
                // Static Objects
                for (int i = 0; i < ticketTemplate.StaticObjects.Count; i++)
                {
                    // Add static object to datagridview
                    this.LoadStaticObjectToTemplateTab(ticketTemplate.StaticObjects[i]);

                    // Add static object to image
                    this.LoadStaticObjectToTemplatePicture(ticketTemplate.StaticObjects[i], g);
                }

                // Textboxes and Checkboxes
                for (int i = 0, j = 0; i < ticketTemplate.TemplateTextboxes.Count || j < ticketTemplate.TemplateCheckboxes.Count;)
                {
                    // Compare the priority of the objects on i and j. Put the one with the lowest priority on first and increment the 
                    //  counter associated with its list. 
                    //  i => Textbox List
                    //  j => Checkbox List
                    // If one has reached the end of its list, put the other one's next content up and continue
                    if (i == ticketTemplate.TemplateTextboxes.Count && j == ticketTemplate.TemplateCheckboxes.Count)
                    {
                        break;
                    }
                    else if (i == ticketTemplate.TemplateTextboxes.Count)
                    {
                        // Add textbox properties to grid
                        this.LoadCheckboxToTemplateTab(ticketTemplate.TemplateCheckboxes[j]);

                        // Add textbox to image
                        this.LoadCheckboxToTemplatePicture(ticketTemplate.TemplateCheckboxes[j], g);

                        j++;
                    }
                    else if (j == ticketTemplate.TemplateCheckboxes.Count)
                    {
                        // Add textbox properties to grid
                        this.LoadTextboxToTemplateTab(ticketTemplate.TemplateTextboxes[i]);

                        // Add textbox to image
                        this.LoadTextboxToTemplatePicture(ticketTemplate.TemplateTextboxes[i], g);

                        i++;
                    }
                    else
                    {
                        // Compare priorities
                        if (ticketTemplate.TemplateTextboxes[i].Priority < ticketTemplate.TemplateCheckboxes[j].Priority)
                        {
                            // Add textbox properties to grid
                            this.LoadTextboxToTemplateTab(ticketTemplate.TemplateTextboxes[i]);

                            // Add textbox to image
                            this.LoadTextboxToTemplatePicture(ticketTemplate.TemplateTextboxes[i], g);

                            i++;
                        }
                        else
                        {
                            // Add textbox properties to grid
                            this.LoadCheckboxToTemplateTab(ticketTemplate.TemplateCheckboxes[j]);

                            // Add textbox to image
                            this.LoadCheckboxToTemplatePicture(ticketTemplate.TemplateCheckboxes[j], g);

                            j++;
                        }
                    }
                   
                }
            }
            
        }

        /// <summary>
        ///  Given a template, this function loads the template's document onto the Template Tab based on its word document associated with it.
        ///  If the document is loaded successfully, return true. Otherwise, return false.
        /// </summary>
        private bool LoadTemplateTabImage(TicketTemplate targetTemplate)
        {
            // If the document associated with the ticket template is empty, return. Otherwise, continue.
            if (File.Exists(targetTemplate.DocumentPath) == false)
            {
                return false;
            }


            // Open the document associated with the template. If we fail to do so, return false.
            Microsoft.Office.Interop.Word.Application backgroundWordApp = new Microsoft.Office.Interop.Word.Application();
            backgroundWordApp.Visible = false;
            Microsoft.Office.Interop.Word.Document targetDocument = backgroundWordApp.Documents.Open(targetTemplate.DocumentPath);
            foreach (Microsoft.Office.Interop.Word.Window window in targetDocument.Windows)
            {
                foreach (Microsoft.Office.Interop.Word.Pane pane in window.Panes)
                {
                    Microsoft.Office.Interop.Word.Page targetPage = pane.Pages[1];
                    var targetPageBits = targetPage.EnhMetaFileBits;

                    try
                    {
                        MemoryStream targetDocumentMemoryStream = new MemoryStream((byte[])(targetPageBits));
                        Image newImage = Image.FromStream(targetDocumentMemoryStream);
                        this.activeTemplateImage = newImage;
                    }
                    catch
                    {
                        targetDocument.Close(Type.Missing, Type.Missing, Type.Missing);
                        backgroundWordApp.Quit(Type.Missing, Type.Missing, Type.Missing);
                        return false;
                    }
                }
            }

            // Close and return when finished.
            targetDocument.Close(Type.Missing, Type.Missing, Type.Missing);
            backgroundWordApp.Quit(Type.Missing, Type.Missing, Type.Missing);
            return true;
        }

        /// <summary>
        ///  Given a textbox, this prints all entries besides text to the template tab datagridview.
        /// </summary>
        /// <param name="source">
        ///  Textbox to be printed.
        /// </param>
        private void LoadTextboxToTemplateTab(InformationTextbox source)
        {
            // Load all properties of source onto the template tab

            // Title
            this.AddGridView2Row(InformationObjectConstants.TextboxTitle, "X", true, Color.Gray, Color.Gray);

            // Name
            this.AddGridView2Row(InformationObjectConstants.NameProperty, source.Name);

            // X
            this.AddGridView2Row(InformationObjectConstants.XProperty, source.X.ToString());

            // Y
            this.AddGridView2Row(InformationObjectConstants.YProperty, source.Y.ToString());

            // Required
            if (source.Required == true)
            {
                this.AddGridView2Row(InformationObjectConstants.RequiredProperty, source.Required.ToString(), true, Color.Yellow, Color.Green);
            }
            else
            {
                source.Required = false;
                this.AddGridView2Row(InformationObjectConstants.RequiredProperty, source.Required.ToString(), true, Color.Yellow, Color.Red);
            }

            // Priority
            this.AddGridView2Row(InformationObjectConstants.PriorityProperty, source.Priority.ToString());

            // FontSize
            this.AddGridView2Row(InformationObjectConstants.FontSizeProperty, source.FontSize.ToString());

            // Height
            this.AddGridView2Row(InformationObjectConstants.HeightProperty, source.Height.ToString());

            // Width
            this.AddGridView2Row(InformationObjectConstants.WidthProperty, source.Width.ToString());
        }

        /// <summary>
        ///  Given a textbox, this displays the textbox location, dimension, and name to it's corresponding location on the template.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="g"></param>
        /// <param name="imageScale">
        ///  Scales the size at which the data is displayed at.
        /// </param>
        private void LoadTextboxToTemplatePicture(InformationTextbox source, Graphics g, double imageScale = 1)
        {
            // Draw a rectangle in the area of the textbox. Draw name of textbox within rectangle.
            Rectangle textboxRectangle = new Rectangle(
                (int)(source.X * imageScale),
                (int)(source.Y * imageScale),
                (int)(source.Width * imageScale),
                (int)(source.Height * imageScale));

            // Add highlight around area in red. 2 pixels wide
            g.DrawRectangle(
                PaintingConstants.RedPen(),
                textboxRectangle);

            // Add name of textbox to area in font size within highlighted area
            g.DrawString(
                source.Name,
                PaintingConstants.Arial((int)(source.FontSize * imageScale)),
                Brushes.DarkRed,
                new PointF(
                    (float)(source.X * imageScale) + 2,
                    (float)(source.Y * imageScale) + 2)
                );
        }

        /// <summary>
        ///  Given a checkbox, this prints all entries besides text to the template tab datagridview.
        /// </summary>
        /// <param name="source">
        ///  Checkbox to be printed.
        /// </param>
        private void LoadCheckboxToTemplateTab(InformationCheckbox source)
        {
            // Load all properties of source onto the template tab

            // Title
            this.AddGridView2Row(InformationObjectConstants.CheckboxTitle, "X", true, Color.Gray, Color.Gray);

            // Name
            this.AddGridView2Row(InformationObjectConstants.NameProperty, source.Name);

            // X
            this.AddGridView2Row(InformationObjectConstants.XProperty, source.X.ToString());

            // Y
            this.AddGridView2Row(InformationObjectConstants.YProperty, source.Y.ToString());

            // Required
            if (source.Required == true)
            {
                this.AddGridView2Row(InformationObjectConstants.RequiredProperty, source.Required.ToString(), true, Color.Yellow, Color.Green);
            }
            else
            {
                source.Required = false;
                this.AddGridView2Row(InformationObjectConstants.RequiredProperty, source.Required.ToString(), true, Color.Yellow, Color.Red);
            }

            // Priority
            this.AddGridView2Row(InformationObjectConstants.PriorityProperty, source.Priority.ToString());

            // Scale
            this.AddGridView2Row(InformationObjectConstants.ScaleProperty, source.Scale.ToString());
        }

        /// <summary>
        ///  Given a checkbox, this displays the checkbox location, dimension, and name to it's corresponding location on the template.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="imageScale">
        ///  Scales the size at which the data is displayed at.
        /// </param>
        private void LoadCheckboxToTemplatePicture(InformationCheckbox source, Graphics g, double imageScale = 1)
        {
            // Draw a rectangle in the area of the textbox. Draw name of textbox within rectangle.
            Rectangle textboxRectangle = new Rectangle(
                (int)(source.X * imageScale),
                (int)(source.Y * imageScale),
                (int)(source.Scale * InformationCheckbox.CheckboxDefaultEdgeLength * imageScale),
                (int)(source.Scale * InformationCheckbox.CheckboxDefaultEdgeLength * imageScale));

            // Add highlight around area in red. 2 pixels wide
            g.DrawRectangle(
                PaintingConstants.RedPen(),
                textboxRectangle);

            // Add name of textbox to area in font size within highlighted area
            g.DrawString(
                source.Name,
                PaintingConstants.Arial((int)(11 * imageScale)),
                Brushes.DarkRed,
                new PointF(
                    (float)(source.X * imageScale) + 2,
                    (float)(source.Y * imageScale) + 2)
                );
        }

        /// <summary>
        ///  Given a static object represented by a textbox, this prints relevant properties to the template tab datagridview.
        /// </summary>
        /// <param name="source"></param>
        private void LoadStaticObjectToTemplateTab(InformationTextbox source)
        {
            // Load all properties of source onto the template tab depending on what the source is
            // Title
            this.AddGridView2Row(InformationObjectConstants.StaticObjectTitle, "X", true, Color.Gray, Color.Gray);

            // Add different properties depending on which static object is being added.
            // Customer first name, last name, day, month, year, timestamp, and template id case
            if (source.Name.Contains(InformationObjectConstants.CustomerFirstName) ||
                source.Name.Contains(InformationObjectConstants.CustomerLastName) ||
                source.Name.Contains(InformationObjectConstants.Day) ||
                source.Name.Contains(InformationObjectConstants.Month) ||
                source.Name.Contains(InformationObjectConstants.Year) ||
                source.Name.Contains(InformationObjectConstants.TimeStamp) ||
                source.Name.Contains(InformationObjectConstants.TemplateID))
            {
                // Name
                this.AddGridView2Row(InformationObjectConstants.NameProperty, source.Name, false);

                // X
                this.AddGridView2Row(InformationObjectConstants.XProperty, source.X.ToString());

                // Y
                this.AddGridView2Row(InformationObjectConstants.YProperty, source.Y.ToString());

                // FontSize
                this.AddGridView2Row(InformationObjectConstants.FontSizeProperty, source.FontSize.ToString());
            }
            // Counter case
            else if (source.Name.Contains(InformationObjectConstants.Counter))
            {
                // Name
                this.AddGridView2Row(InformationObjectConstants.NameProperty, source.Name, false);

                // X
                this.AddGridView2Row(InformationObjectConstants.XProperty, source.X.ToString());

                // Y
                this.AddGridView2Row(InformationObjectConstants.YProperty, source.Y.ToString());

                // Reset Each Year (Corresponds to required property)
                if (source.Required == true)
                {
                    this.AddGridView2Row(InformationObjectConstants.ResetEachYearProperty, source.Required.ToString(), true, Color.Yellow, Color.Green);
                }
                else
                {
                    source.Required = false;
                    this.AddGridView2Row(InformationObjectConstants.ResetEachYearProperty, source.Required.ToString(), true, Color.Yellow, Color.Red);
                }

                // FontSize
                this.AddGridView2Row(InformationObjectConstants.FontSizeProperty, source.FontSize.ToString());

                // Text (expected to be a number)
                this.AddGridView2Row(InformationObjectConstants.CurrentCounterValueProperty, source.Text);
            }
        }

        /// <summary>
        ///  Given a static object represented by a textbox, this displays where the static object will be placed on the 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="g"></param>
        /// <param name="imageScale"></param>
        private void LoadStaticObjectToTemplatePicture(InformationTextbox source, Graphics g, double imageScale = 1)
        {
            // Draw a rectangle in the area of the textbox. Draw name of textbox within rectangle.
            Rectangle textboxRectangle = new Rectangle(
                (int)(source.X * imageScale),
                (int)(source.Y * imageScale),
                (int)(source.Width * imageScale),
                (int)(source.Height * imageScale));

            // Add highlight around area in red. 2 pixels wide
            g.DrawRectangle(
                PaintingConstants.RedPen(),
                textboxRectangle);

            // Add name of textbox to area in font size within highlighted area
            g.DrawString(
                source.Name,
                PaintingConstants.Arial((int)(source.FontSize * imageScale)),
                Brushes.DarkRed,
                new PointF(
                    (float)(source.X * imageScale) + 2,
                    (float)(source.Y * imageScale) + 2)
                );
        }

        /// <summary>
        ///  Displays the current mouse coordinates as the mouse moves relative to the mouse and the offset inputted.
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        private void DisplayMouseCoordinates(Graphics targetImage, int offsetX = 0, int offsetY = 0)
        {
            targetImage.DrawString(
                    "(" + (Cursor.Position.X - offsetX - 115).ToString() + "," + (Cursor.Position.Y - offsetY + this.panel1.VerticalScroll.Value) + ")",
                    PaintingConstants.Arial(),
                    Brushes.DarkRed,
                    new Point((Cursor.Position.X - offsetX - 115),
                    (Cursor.Position.Y - offsetY + 10 + this.panel1.VerticalScroll.Value)));
        }

        // ************************* Create Ticket Tab Functions *************************
        /// <summary>
        ///  Opens an already created template into view on the create ticket template tab based on user input.
        /// </summary>
        private void OpenLoadCreateTicketTabTemplate(string? filePath = null)
        {
            // Initialize databases in case they aren't already
            this.InitializeActiveDataBase();

            // Prompt the user for a file to open from. If they select one, continue to try and load from it.
            if (filePath == null)
            {
                OpenFileDialog openTemplateDialog = new OpenFileDialog();
                openTemplateDialog.Filter = "XML files(.xml)| *.xml";
                openTemplateDialog.InitialDirectory = this.activeDatabase.DatabaseName + ProjectDataBaseConstants.TemplateDirectory;
                if (openTemplateDialog.ShowDialog() == DialogResult.OK)
                {
                    TicketTemplate loadedTemplate = new TicketTemplate("");

                    Stream targetFile;
                    try
                    {
                        // Attempt to open the chosen file for read and load in the template from there. If it fails, tell the user and return.
                        targetFile = File.OpenRead(Path.GetFullPath(openTemplateDialog.FileName));
                        if (loadedTemplate.LoadTemplate(targetFile) == false)
                        {
                            MessageBox.Show(ErrorMessages.CreateTemplateFailedToLoadFileMessage, ErrorMessages.CreateTemplateErrorTitle, MessageBoxButtons.OK);
                            return;
                        }

                        // Set the active template as the newly loaded one.
                        this.creationTemplateBase = loadedTemplate;
                        // Set the saved creation template path
                        this.creationTemplateSavedFilePath = Path.GetFullPath(openTemplateDialog.FileName);

                        // If the last write time is different than the current year, reset each counter that might exist in the template.
                        string templateYear = File.GetLastWriteTime(openTemplateDialog.FileName).ToString().Split('/')[2];
                        templateYear = templateYear.Remove(4, templateYear.Length - 4);
                        string currentYear = DateTime.Now.ToString().Split('/')[2];
                        currentYear = currentYear.Remove(4, currentYear.Length - 4);
                        if (currentYear != templateYear)
                        {
                            this.creationTemplateBase.ResetCounters();
                        }

                        // Load template data and image
                        if (this.LoadCreateTicketTabImage(this.creationTemplateBase) != true)
                        {
                            MessageBox.Show(ErrorMessages.CreateTemplateImageMissingErrorMessage, ErrorMessages.CreateTemplateErrorTitle, MessageBoxButtons.OK);
                            this.pictureBox2.Image = Image.FromFile(this.systemDatabase.DatabaseName + "fail_to_load_image_graphic");
                            return;
                        }
                        // Load image into picturebox.
                        this.pictureBox2.Image = this.creationTemplateBaseImage;

                        // Close file after use.
                        targetFile.Close();

                        // Load template data into view
                        using (Graphics g = this.pictureBox2.CreateGraphics())
                        {
                            // Textboxes
                            for (int i = 0; i < this.creationTemplateBase.TemplateTextboxes.Count; i++)
                            {
                                // Add textbox to image
                                this.LoadTextboxToTemplatePicture(this.creationTemplateBase.TemplateTextboxes[i], g);
                            }
                            // Checkboxes
                            for (int i = 0; i < this.creationTemplateBase.TemplateCheckboxes.Count; i++)
                            {
                                // Add checkbox to image
                                this.LoadCheckboxToTemplatePicture(this.creationTemplateBase.TemplateCheckboxes[i], g);
                            }
                        }

                        // Set the textbox in the create template tab to the document path of the template
                        this.toolStripTextBox1.Text = Path.GetFullPath(openTemplateDialog.FileName);
                    }
                    catch
                    {
                        MessageBox.Show(ErrorMessages.CreateTemplateFailedToLoadFileMessage, ErrorMessages.CreateTemplateErrorTitle, MessageBoxButtons.OK);
                        return;
                    }
                }
            }
            else
            {
                TicketTemplate loadedTemplate = new TicketTemplate("");

                Stream targetFile;
                try
                {
                    // Attempt to open the chosen file for read and load in the template from there. If it fails, tell the user and return.
                    targetFile = File.OpenRead(Path.GetFullPath(filePath));
                    if (loadedTemplate.LoadTemplate(targetFile) == false)
                    {
                        MessageBox.Show(ErrorMessages.CreateTemplateFailedToLoadFileMessage, ErrorMessages.CreateTemplateErrorTitle, MessageBoxButtons.OK);
                        return;
                    }

                    // Set the active template as the newly loaded one.
                    this.creationTemplateBase = loadedTemplate;
                    // Set the saved creation template path
                    this.creationTemplateSavedFilePath = Path.GetFullPath(filePath);

                    // If the last write time is different than the current year, reset each counter that might exist in the template.
                    string templateYear = File.GetLastWriteTime(filePath).ToString().Split('/')[2];
                    templateYear = templateYear.Remove(4, templateYear.Length - 4);
                    string currentYear = DateTime.Now.ToString().Split('/')[2];
                    currentYear = currentYear.Remove(4, currentYear.Length - 4);
                    if (currentYear != templateYear)
                    {
                        this.creationTemplateBase.ResetCounters();
                    }

                    // Load template data and image
                    if (this.LoadCreateTicketTabImage(this.creationTemplateBase) != true)
                    {
                        MessageBox.Show(ErrorMessages.CreateTemplateImageMissingErrorMessage, ErrorMessages.CreateTemplateErrorTitle, MessageBoxButtons.OK);
                        this.pictureBox2.Image = Image.FromFile(this.systemDatabase.DatabaseName + "fail_to_load_image_graphic");
                        return;
                    }
                    // Load image into picturebox.
                    this.pictureBox2.Image = this.creationTemplateBaseImage;

                    // Close file after use.
                    targetFile.Close();

                    // Load template data into view
                    using (Graphics g = this.pictureBox2.CreateGraphics())
                    {
                        // Textboxes
                        for (int i = 0; i < this.creationTemplateBase.TemplateTextboxes.Count; i++)
                        {
                            // Add textbox to image
                            this.LoadTextboxToTemplatePicture(this.creationTemplateBase.TemplateTextboxes[i], g);
                        }
                        // Checkboxes
                        for (int i = 0; i < this.creationTemplateBase.TemplateCheckboxes.Count; i++)
                        {
                            // Add checkbox to image
                            this.LoadCheckboxToTemplatePicture(this.creationTemplateBase.TemplateCheckboxes[i], g);
                        }
                    }

                    // Set the textbox in the create template tab to the document path of the template
                    this.toolStripTextBox1.Text = Path.GetFullPath(filePath);
                }
                catch
                {
                    MessageBox.Show(ErrorMessages.CreateTemplateFailedToLoadFileMessage, ErrorMessages.CreateTemplateErrorTitle, MessageBoxButtons.OK);
                    return;
                }
            }
        }

        /// <summary>
        ///  Based on the inputted tickettemplate, load its image onto the picturebox in the create ticket tab.
        /// </summary>
        /// <param name="targetTemplate"></param>
        /// <returns></returns>
        private bool LoadCreateTicketTabImage(TicketTemplate targetTemplate)
        {
            // If the document associated with the ticket template is empty, return. Otherwise, continue.
            if (File.Exists(targetTemplate.DocumentPath) == false)
            {
                return false;
            }


            // Open the document associated with the template. If we fail to do so, return false.
            Microsoft.Office.Interop.Word.Application backgroundWordApp = new Microsoft.Office.Interop.Word.Application();
            backgroundWordApp.Visible = false;
            Microsoft.Office.Interop.Word.Document targetDocument = backgroundWordApp.Documents.Open(targetTemplate.DocumentPath);
            foreach (Microsoft.Office.Interop.Word.Window window in targetDocument.Windows)
            {
                foreach (Microsoft.Office.Interop.Word.Pane pane in window.Panes)
                {
                    Microsoft.Office.Interop.Word.Page targetPage = pane.Pages[1];
                    var targetPageBits = targetPage.EnhMetaFileBits;

                    try
                    {
                        MemoryStream targetDocumentMemoryStream = new MemoryStream((byte[])(targetPageBits));
                        Image newImage = Image.FromStream(targetDocumentMemoryStream);
                        this.creationTemplateBaseImage = newImage;
                    }
                    catch
                    {
                        targetDocument.Close(Type.Missing, Type.Missing, Type.Missing);
                        backgroundWordApp.Quit(Type.Missing, Type.Missing, Type.Missing);
                        return false;
                    }
                }
            }

            // Close and return when finished.
            targetDocument.Close(Type.Missing, Type.Missing, Type.Missing);
            backgroundWordApp.Quit(Type.Missing, Type.Missing, Type.Missing);
            return true;
        }

        /// <summary>
        ///  This function creates and saves a ticket based on the current creationTemplateBase
        /// </summary>
        private void CreateAndSaveTicket()
        {
            // If the creationTemplateBase is null, return here as we can't proceed otherwise.
            if (this.creationTemplateBase == null)
            {
                return;
            }

            // Get inputs from the user corresponding to all template information objects. Inputs recieved from CreateTicketWindow class instance.
            CreateTicketWindow creationWindow = new CreateTicketWindow(this.creationTemplateBase);
            creationWindow.ShowDialog();

            // If the creation window was not completed by the user, exit out. Otherwise, continue.
            if (creationWindow.Complete == true)
            {
                // Save jobTicket as an xml file in the current active database's job ticket folder
                // Initialize the active database just in case the folders don't exist
                this.InitializeActiveDataBase();
                // Open a file to save to
                // If we fail to open it, show an error message and return
                Stream saveFileStream;
                int copies = 1;
                try
                {
                    // Make a unique name for the job ticket being created based on the template name.
                    if (File.Exists(this.activeDatabase.DatabaseName + ProjectDataBaseConstants.JobTicketDirectory + "\\" + copies.ToString() + " " + this.creationTemplateBase.Name))
                    {
                        do
                        {
                            copies++;
                        } while (File.Exists(this.activeDatabase.DatabaseName + ProjectDataBaseConstants.JobTicketDirectory + "\\" + copies.ToString() + " " + this.creationTemplateBase.Name) == true);
                        File.Create(this.activeDatabase.DatabaseName + ProjectDataBaseConstants.JobTicketDirectory + "\\" + copies.ToString() + " " + this.creationTemplateBase.Name).Close();
                        saveFileStream = File.OpenWrite(this.activeDatabase.DatabaseName + ProjectDataBaseConstants.JobTicketDirectory + "\\" + copies.ToString() + " " + this.creationTemplateBase.Name);
                    }
                    else
                    {
                        File.Create(this.activeDatabase.DatabaseName + ProjectDataBaseConstants.JobTicketDirectory + "\\" + copies.ToString() + " " + this.creationTemplateBase.Name).Close();
                        saveFileStream = File.OpenWrite(this.activeDatabase.DatabaseName + ProjectDataBaseConstants.JobTicketDirectory + "\\" + copies.ToString() + " " + this.creationTemplateBase.Name);
                    }
                   
                }
                catch
                {
                    MessageBox.Show(ErrorMessages.CreateTicketFailedToSaveFileMessage, ErrorMessages.CreateTicketErrorTitle, MessageBoxButtons.OK);
                    return;
                }

                // Look for any counter static objects. If they exist, increment the number on the template it was loaded from and save it again.
                // Save the template afterwards if any counters were found.
                bool staticObjectsEdited = false;
                string temp;
                for (int i = 0; i < this.creationTemplateBase.StaticObjects.Count; i++)
                {
                    if (this.creationTemplateBase.StaticObjects[i].Name.Contains(InformationObjectConstants.Counter))
                    {
                        this.creationTemplateBase.StaticObjects[i].Text = (Convert.ToDouble(this.creationTemplateBase.StaticObjects[i].Text) + 1).ToString();
                        staticObjectsEdited = true;
                    }
                    else if (this.creationTemplateBase.StaticObjects[i].Name.Contains(InformationObjectConstants.Day))
                    {
                        this.creationTemplateBase.StaticObjects[i].Text = DateTime.Now.ToString().Split('/')[1];
                    }
                    else if (this.creationTemplateBase.StaticObjects[i].Name.Contains(InformationObjectConstants.Month))
                    {
                        this.creationTemplateBase.StaticObjects[i].Text = DateTime.Now.ToString().Split('/')[0];
                    }
                    else if (this.creationTemplateBase.StaticObjects[i].Name.Contains(InformationObjectConstants.Year))
                    {
                        temp = DateTime.Now.ToString().Split('/')[2];
                        temp = temp.Remove(4, temp.Length - 4);
                        this.creationTemplateBase.StaticObjects[i].Text = temp;
                    }
                    else if (this.creationTemplateBase.StaticObjects[i].Name.Contains(InformationObjectConstants.TimeStamp))
                    {
                        temp = DateTime.Now.ToString().Split(' ')[1] + DateTime.Now.ToString().Split(' ')[2];
                        this.creationTemplateBase.StaticObjects[i].Text = temp;
                    }
                    else if (this.creationTemplateBase.StaticObjects[i].Name.Contains(InformationObjectConstants.TemplateID))
                    {
                        this.creationTemplateBase.StaticObjects[i].Text = copies.ToString();
                    }
                }
                if (staticObjectsEdited)
                {
                    try
                    {
#pragma warning disable CS8604 // Possible null reference argument.
                        Stream saveTemplateStream = File.OpenWrite(this.creationTemplateSavedFilePath);
#pragma warning restore CS8604 // Possible null reference argument.
                        this.creationTemplateBase.SaveTemplate(saveTemplateStream);
                        saveTemplateStream.Close();
                    }
                    catch
                    {
                        MessageBox.Show(ErrorMessages.CreateTicketBaseTemplateFileMissingMessage, ErrorMessages.CreateTicketErrorTitle, MessageBoxButtons.OK);
                    }
                }

                // Create a JobTicket and save it to the active database based on the inputted values.
                JobTicket newTicket = new JobTicket();

                // Set the job ticket's information up based on the template used
                newTicket.LoadFromTemplate(this.creationTemplateBase);

                // Bring in customer name information from the creation window used.
                if (creationWindow.CustomerFirstName != null && creationWindow.CustomerLastName != null)
                {
                    newTicket.CustomerFirstName = creationWindow.CustomerFirstName;
                    newTicket.CustomerLastName = creationWindow.CustomerLastName;
                }

                // Set the job ticket's information object entries based on the window's entries
                int currentIndex;
                // -Textboxes
                for (currentIndex = 0; currentIndex < creationWindow.InputTextboxes.Count; currentIndex++)
                {
                    newTicket.SetTexboxText(creationWindow.InputTextboxes[currentIndex].Name, creationWindow.InputTextboxes[currentIndex].Text);
                }
                // -Checkboxes
                for (currentIndex = 0; currentIndex < creationWindow.InputCheckboxes.Count; currentIndex++)
                {
                    if (creationWindow.InputCheckboxes[currentIndex].Text == CreationWindowConstants.ComboBoxDefault ||
                        creationWindow.InputCheckboxes[currentIndex].Text == CreationWindowConstants.ComboBoxFalse)
                    {
                        newTicket.SetCheckboxStatus(creationWindow.InputCheckboxes[currentIndex].Name, false);
                    }
                    else
                    {
                        newTicket.SetCheckboxStatus(creationWindow.InputCheckboxes[currentIndex].Name, true);
                    }
                }

                // Save
                try
                {
                    if (newTicket.SaveTicket(saveFileStream) == false)
                    {
                        MessageBox.Show(ErrorMessages.CreateTicketFailedToSaveFileMessage, ErrorMessages.CreateTicketErrorTitle, MessageBoxButtons.OK);
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show(ErrorMessages.CreateTicketFailedToSaveFileMessage, ErrorMessages.CreateTicketErrorTitle, MessageBoxButtons.OK);
                    return;
                }
                
                // Close file when done
                saveFileStream.Close();

                // Prompt the user and see if they would want to print the created ticket. If they select yes, create the ticket and print it to the printer of their choice. Delete the ticket afterwards.
                if (MessageBox.Show("Ticket successfully create. Would you like to print it now?", "Create Ticket", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    this.PrintJobTicket(newTicket, this.pictureBox2.Image);
                }

                // Return when done.
                return;
            }
            else
            {
                return;
            }
        }

        /// <summary>
        ///  Filters the job ticket tab based on the filterType as well as the filterString.
        ///  -filterType defaults to null. If it's null, then, nothing is filtered.
        ///     If it's set to a number, then different aspects are filtered.
        ///     1) First Name
        ///     2) Last Name
        ///     3) Day last modified
        ///     4) Month last modified
        ///     5) Year last modified
        ///     6) Template
        ///     7) Template ID#
        ///  -filterString filters the tab to entries pertaining to the filterType that contain
        ///     the filterString as a substring in its text.
        /// </summary> 
        private void FilterJobTicketTab(string filterString, int? filterType = null)
        {
            // Initialize database in case it isn't already
            this.InitializeActiveDataBase();

            // Destroy any image on picturebox 3
            this.pictureBox3.Image = null;

            // Set the selected id to null
            this.selectedTicketID = null;

            // If filterType = null, initialize the job ticket tab as normal
            if (filterType == null)
            {
                this.InitializeJobTicketTab();
            }

            // Filter objects based on the filterString and filterType
            // Clear datagridview 1
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Rows.Clear();

            // Clear previous dictionaries
            this.activeDatabaseJobTickets.Clear();
            this.jobTicketFilePaths.Clear();

            // Look at every file inside of the JobTicket folder. For each one, try and open and load data about the job ticket to the datagridview1.
            // Load Customer Name (First, Last), File Path, and Date Last Edited
            string[] JobTicketFilePaths = Directory.GetFiles(this.activeDatabase.DatabaseName + ProjectDataBaseConstants.JobTicketDirectory);


            // Temporary variables for filtering and storing data.
            bool matchesFilter;
            string temporaryString1;
            string temporaryString2;
            JobTicket temporaryJobTicket;

            foreach (string FilePath in JobTicketFilePaths)
            {
                // File used to read in data.
                Stream? inputFile = null;
                try
                {
                    // Reset variables used in each loop iteration.
                    matchesFilter = false;
                    temporaryString1 = string.Empty;

                    // Open a file path to the job ticket.
                    inputFile = File.OpenRead(FilePath);

                    // Load job ticket into a temporary variable
                    temporaryJobTicket = new JobTicket();
                    temporaryJobTicket.LoadTicket(inputFile);

                    // Add the job ticket to the datagridview1 if it matches the filter.
                    switch (filterType)
                    {
                        // By first name
                        case 1:

                            // Get the firstname into the temporaryString and then trim it to match the size
                            // of the filterString
                            temporaryString1 = temporaryJobTicket.CustomerFirstName;

                            // If the length of the filter string is less than or equal to the firstname, then continue.
                            // Otherwise, the filter can't apply to it and does not match.
                            temporaryString1 = temporaryString1.ToLower();
                            temporaryString2 = filterString.ToLower();
                            if (temporaryString1.Length >= filterString.Length)
                            {
                                // Cut the name down to the length of the filterString, cutting off the end of the string. 
                                // Then, if the two strings match, then the filter applies to it.
                                temporaryString1 = temporaryString1.Remove(filterString.Length);
                                if (temporaryString1 == temporaryString2)
                                {
                                    matchesFilter = true;
                                }
                            }
                            break;

                        // By last name
                        case 2:

                            // Get the lastname into the temporaryString and then trim it to match the size
                            // of the filterString
                            temporaryString1 = temporaryJobTicket.CustomerLastName;

                            // If the length of the filter string is less than or equal to the lastname, then continue.
                            // Otherwise, the filter can't apply to it and does not match.
                            temporaryString1 = temporaryString1.ToLower();
                            temporaryString2 = filterString.ToLower();
                            if (temporaryString1.Length >= filterString.Length)
                            {
                                // Cut the name down to the length of the filterString, cutting off the end of the string. 
                                // Then, if the two strings match, then the filter applies to it.
                                temporaryString1 = temporaryString1.Remove(filterString.Length);
                                if (temporaryString1 == filterString)
                                {
                                    matchesFilter = true;
                                }
                            }

                            break;

                        // By Day last modified
                        case 3:

                            // Get the entire last time modified into temporaryString1
                            temporaryString1 = File.GetLastWriteTime(FilePath).ToString();

                            // Read the entry for day.
                            temporaryString1 = temporaryString1.Split('/')[1];

                            // Check if the text entered is equal to the day. If it is, then matchesFilter = true.
                            if (temporaryString1 == filterString)
                            {
                                matchesFilter = true;
                            }

                            break;

                        // By Month last modified
                        case 4:

                            // Get the entire last time modified into temporaryString1
                            temporaryString1 = File.GetLastWriteTime(FilePath).ToString();

                            // Read the entry for month.
                            temporaryString1 = temporaryString1.Split('/')[0];

                            // Check if the text entered is equal to the month. If it is, then matchesFilter = true.
                            if (temporaryString1 == filterString)
                            {
                                matchesFilter = true;
                            }

                            break;

                        // By Year last modified
                        case 5:

                            // Get the entire last time modified into temporaryString1
                            temporaryString1 = File.GetLastWriteTime(FilePath).ToString();

                            // Read the entry for year.
                            temporaryString1 = temporaryString1.Split('/')[2];
                            temporaryString1 = temporaryString1.Remove(4, temporaryString1.Length - 4);

                            // Check if the text entered is equal to the day. If it is, then matchesFilter = true.
                            if (temporaryString1 == filterString)
                            {
                                matchesFilter = true;
                            }

                            break;

                        // By template name
                        case 6:

                            // Get the template name
                            temporaryString1 = Path.GetFileName(FilePath).Split(' ')[1];

                            // If the length of the filter string is less than or equal to the lastname, then continue.
                            // Otherwise, the filter can't apply to it and does not match.
                            temporaryString1 = temporaryString1.ToLower();
                            temporaryString2 = filterString.ToLower();
                            if (temporaryString1.Length >= filterString.Length)
                            {
                                // Cut the name down to the length of the filterString, cutting off the end of the string. 
                                // Then, if the two strings match, then the filter applies to it.
                                temporaryString1 = temporaryString1.Remove(filterString.Length);
                                if (temporaryString1 == filterString)
                                {
                                    matchesFilter = true;
                                }
                            }
                            break;

                        // By template ID
                        case 7:

                            // Get the template ID number
                            temporaryString1 = Path.GetFileName(FilePath).Split(' ')[0];

                            // If the length of the filter string is less than or equal to the lastname, then continue.
                            // Otherwise, the filter can't apply to it and does not match.
                            temporaryString1 = temporaryString1.ToLower();
                            temporaryString2 = filterString.ToLower();
                            if (temporaryString1.Length >= filterString.Length)
                            {
                                // Cut the name down to the length of the filterString, cutting off the end of the string. 
                                // Then, if the two strings match, then the filter applies to it.
                                temporaryString1 = temporaryString1.Remove(filterString.Length);
                                if (temporaryString1 == filterString)
                                {
                                    matchesFilter = true;
                                }
                            }
                            break;
                    }
                       
                    // If the jobTicket applied to the filter, add it to the jobTicketTab.
                    if (matchesFilter == true)
                    {
                        this.AddGridView1Row(
                        temporaryJobTicket.CustomerFirstName,
                        temporaryJobTicket.CustomerLastName,
                        File.GetLastWriteTime(FilePath).ToString(),
                        FilePath,
                        false,
                        false,
                        false,
                        false,
                        Color.LightGray,
                        Color.LightGray,
                        Color.LightGray,
                        Color.Lime);
                    }

                    // Close file when done
                    inputFile.Close();
                    inputFile = null;

                    // Add path to job ticket document
                    this.jobTicketFilePaths.Add(FilePath, this.jobTicketFilePaths.Count);

                    // Add the job ticket to the job ticket dictionary
                    this.activeDatabaseJobTickets.Add(this.activeDatabaseJobTickets.Count, temporaryJobTicket);
                }
                catch
                {
                    if (inputFile != null)
                    {
                        inputFile.Close();
                        inputFile = null;
                    }
                }
            }


        }



        // *****************************
        //  Job Ticket Tab
        // *****************************
        // ************************* (Job Ticket Tab) File Menu Buttons *************************
        /// <summary>
        ///  Edit button pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ModifyAndSaveSelectedTicket();
        }

        /// <summary>
        ///  Delete button pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteCtrlRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DeleteSelectedTicket();
        }

        /// <summary>
        ///  Print button pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.PrintSelectedJobTicket();
        }

        // ************************* (Job Ticket Tab) Filter Menu Buttons/Textboxes *************************
        /// <summary>
        ///  On the "Reset Filter" button being pressed, this button reinitializes the job ticket tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set the filter textbox to be empty.
            this.toolStripTextBox8.Text = "Filter={}";

            this.InitializeJobTicketTab(true);
        }

        /// <summary>
        ///  On enter button being pressed in the firstname textbox, filter the job tickets based on the entry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBox3_KeyDown(object sender, KeyEventArgs e)
        {
            // If the enter key is pressed.
            if (e.KeyCode == Keys.Enter)
            {
                // Set the filter textbox to match the current filter.
                this.toolStripTextBox8.Text = "Filter={firstname:'" + this.toolStripTextBox3.Text + "'}";

                // Filter the job ticket tab
                this.FilterJobTicketTab(this.toolStripTextBox3.Text, 1);
                this.toolStripTextBox3.Text = string.Empty;              
            }
        }

        /// <summary>
        ///  On enter button being pressed in the lastname textbox, filter the job tickets based on the entry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private void toolStripTextBox4_KeyDown(object sender, KeyEventArgs e)
        {
            // If the enter key is pressed.
            if (e.KeyCode == Keys.Enter)
            {
                // Set the filter textbox to match the current filter.
                this.toolStripTextBox8.Text = "Filter={lastname:'" + this.toolStripTextBox4.Text + "'}";

                // Filter the job ticket tab
                this.FilterJobTicketTab(this.toolStripTextBox4.Text, 2);
            }
        }

        /// <summary>
        ///  On enter button being pressed in the day last modified textbox, filter the job ticket tab by the entered string
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBox5_KeyDown(object sender, KeyEventArgs e)
        {
            // If the enter key is pressed.
            if (e.KeyCode == Keys.Enter)
            {
                // Set the filter textbox to match the current filter.
                this.toolStripTextBox8.Text = "Filter={day:'" + this.toolStripTextBox5.Text + "'}";

                // Filter the job ticket tab
                this.FilterJobTicketTab(this.toolStripTextBox5.Text, 3);
            }
        }

        /// <summary>
        ///  On enter button being pressed in the month last modified textbox, filter the job ticket tab by the entered string
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBox6_KeyDown(object sender, KeyEventArgs e)
        {
            // If the enter key is pressed.
            if (e.KeyCode == Keys.Enter)
            {
                // Set the filter textbox to match the current filter.
                this.toolStripTextBox8.Text = "Filter={month:'" + this.toolStripTextBox6.Text + "'}";

                // Filter the job ticket tab
                this.FilterJobTicketTab(this.toolStripTextBox6.Text, 4);
            }
        }

        /// <summary>
        ///  On enter button being pressed in the year last modified textbox, filter the job ticket tab by the entered string
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBox7_KeyDown(object sender, KeyEventArgs e)
        {
            // If the enter key is pressed.
            if (e.KeyCode == Keys.Enter)
            {
                // Set the filter textbox to match the current filter.
                this.toolStripTextBox8.Text = "Filter={year:'" + this.toolStripTextBox7.Text + "'}";

                // Filter the job ticket tab
                this.FilterJobTicketTab(this.toolStripTextBox7.Text, 5);
            }
        }

        /// <summary>
        ///  On enter button being pressed in the template textbox, filter all job tickets by template.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBox9_KeyDown(object sender, KeyEventArgs e)
        {
            // If the enter key is pressed.
            if (e.KeyCode == Keys.Enter)
            {
                // Set the filter textbox to match the current filter.
                this.toolStripTextBox8.Text = "Filter={template:'" + this.toolStripTextBox9.Text + "'}";

                // Filter the job ticket tab
                this.FilterJobTicketTab(this.toolStripTextBox9.Text, 6);
            }
        }

        /// <summary>
        ///  On enter button being pressed in the template ID textbox, filter all job tickets by template.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBox10_KeyDown(object sender, KeyEventArgs e)
        {
            // If the enter key is pressed.
            if (e.KeyCode == Keys.Enter)
            {
                // Set the filter textbox to match the current filter.
                this.toolStripTextBox8.Text = "Filter={templateID:'" + this.toolStripTextBox10.Text + "'}";

                // Filter the job ticket tab
                this.FilterJobTicketTab(this.toolStripTextBox10.Text, 7);
            }
        }

        /// <summary>
        ///     On admin guest can view pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set database guest user can view to opposite of what it is currently. Save settings as well within function call.
            this.activeDatabase.UpdateGuestsCanView(!this.activeDatabase.GuestCanView);

            // Update button text
            this.onToolStripMenuItem.Text = this.activeDatabase.GuestCanView.ToString();
        }

        /// <summary>
        ///  On enter button being pressed in the promote user textbox, try to promote the user entered by the administrator to admin privilege.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBox11_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Try to promote the entered user. Return message if failed
                switch(this.activeDatabase.PromoteUser(this.toolStripTextBox11.Text, SecurityConstants.AdminUser))
                {
                    // Success case
                    case (1):
                        this.DisplayMessage(string.Empty, SecurityMessages.UserPromotedSuccessfully(this.toolStripTextBox11.Text), MessageBoxButtons.OK);
                        break;

                    // Failure case
                    case (2):
                        this.DisplayMessage(ErrorMessages.PromoteUserErrorTitle, ErrorMessages.UserDatabaseFileError, MessageBoxButtons.OK);
                        break;
                }
            }
        }

        // ************************* (Job Ticket Tab) Database Menu Buttons *************************
        /// <summary>
        ///  Change database button. Switches the database to a folder of the user's choice.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.ChangeActiveDatabase();
        }

        // ************************* (Job Ticket Tab) Event Handlers *************************
        /// <summary>
        ///  On cell click, check to see if it's a path to a job ticket. If it is, make it the active job ticket and load it in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // If the cell clicked on comes from a path, proceed. Otherwise, return.
            if (e.ColumnIndex == 3)
            {
                // Set the current id to the associated filepath's id in the loaded filepath dictionary. Then redraw the picturebox by calling the paint event.
                try
                {
                    if (this.dataGridView1.Rows[e.RowIndex].Cells[3].Value != null)
                    {
                        if (this.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString() != null)
                        {
#pragma warning disable CS8604 // Possible null reference argument.
                            if (this.jobTicketFilePaths.ContainsKey(this.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString()))
#pragma warning restore CS8604 // Possible null reference argument.
                            {
#pragma warning disable CS8604 // Possible null reference argument.
                                this.selectedTicketID = this.jobTicketFilePaths[this.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString()];
#pragma warning restore CS8604 // Possible null reference argument.

                                // Set image to cause a paint event.
                                this.pictureBox3.Image = null;
                            }
                        }
                    }
                }
                catch
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        ///  Paint event pertaining to the job ticket tab picture box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            // If user logged in is a guest and guestviewable is false, don't paint and return
            if (this.activeDatabase.GuestCanView == false && this.userPrivilege == SecurityConstants.GuestUser)
            {
                return;
            }

            // Paint based on the current selectedJobTicketID. If id == null, don't print
            if (this.selectedTicketID == null)
            {
                return;
            }
            else
            {
                // If the current image isn't equal to the selected one, set it to be.
                if (this.pictureBox3.Image == null)
                {
                    this.pictureBox3.Image = this.CreateJobTicketImage(this.activeDatabaseJobTickets[(int)this.selectedTicketID]);
                }

                //-Load information objects onto graphics.
                // --Static objects
                for (int i = 0; i < this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects.Count; i++)
                {
                    // Customer first name case
                    if (this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].Name.Contains(InformationObjectConstants.CustomerFirstName))
                    {
                        e.Graphics.DrawString(
                        this.activeDatabaseJobTickets[(int)this.selectedTicketID].CustomerFirstName,
                        PaintingConstants.Arial((int)this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].FontSize),
                        Brushes.Black,
                        new Point(
                            (int)this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].X,
                            ((int)this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].Y))
                        );
                    }
                    // Customer last name case
                    else if (this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].Name.Contains(InformationObjectConstants.CustomerLastName))
                    {
                        e.Graphics.DrawString(
                        this.activeDatabaseJobTickets[(int)this.selectedTicketID].CustomerLastName,
                        PaintingConstants.Arial((int)this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].FontSize),
                        Brushes.Black,
                        new Point(
                            (int)this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].X,
                            ((int)this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].Y))
                        );
                    }
                    // Counter, Day, Month, Year, TimeStamp, and Template ID case
                    else if (this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].Name.Contains(InformationObjectConstants.Counter) ||
                        this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].Name.Contains(InformationObjectConstants.Day) ||
                        this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].Name.Contains(InformationObjectConstants.Month) ||
                        this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].Name.Contains(InformationObjectConstants.Year) ||
                        this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].Name.Contains(InformationObjectConstants.TimeStamp) ||
                        this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].Name.Contains(InformationObjectConstants.TemplateID))
                    {
                        e.Graphics.DrawString(
                            this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].Text,
                                PaintingConstants.Arial((int)this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].FontSize),
                                Brushes.Black,
                                new Point(
                                    (int)this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].X,
                                    ((int)this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobStaticObjects[i].Y)
                                )
                            );
                    }
                }
                //--Texboxes
                for (int i = 0; i < this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobTextboxes.Count; i++)
                {
                    string[] drawStrings = this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobTextboxes[i].Text.Split("\n");
                    for (int j = 0; j < drawStrings.Length; j++)
                    {
                        e.Graphics.DrawString(
                        drawStrings[j],
                        PaintingConstants.Arial((int)this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobTextboxes[i].FontSize),
                        Brushes.Black,
                        new Point(
                            (int)this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobTextboxes[i].X,
                            ((int)this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobTextboxes[i].Y) + (j * (PaintingConstants.Arial((int)this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobTextboxes[i].FontSize).Height)))
                        );
                    }

                }
                //--Checkboxes
                for (int i = 0; i < this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes.Count; i++)
                {
                    // Create the rectangle based on the checkbox
                    Rectangle checkboxRectangle = new Rectangle(
                        (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].X),
                        (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].Y),
                        (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].Scale * InformationCheckbox.CheckboxDefaultEdgeLength),
                        (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].Scale * InformationCheckbox.CheckboxDefaultEdgeLength));

                    // Draw a checkbox outline.
                    e.Graphics.DrawRectangle(PaintingConstants.BlackPen(), checkboxRectangle);

                    // If true, draw an x within the rectangle.
                    if (this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].Status == true)
                    {
                        // Old code, fills in the rectangle instead of making an x.
                        //g.FillRectangle(Brushes.Black, checkboxRectangle);

                        // Create an x since the checkbox is filled in.
                        e.Graphics.DrawLine(
                            PaintingConstants.BlackPen(),
                            new Point(
                                (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].X),
                                (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].Y)),
                            new Point(
                                (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].X) + (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].Scale * InformationCheckbox.CheckboxDefaultEdgeLength),
                                (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].Y) + (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].Scale * InformationCheckbox.CheckboxDefaultEdgeLength))
                            );
                        e.Graphics.DrawLine(
                            PaintingConstants.BlackPen(),
                            new Point(
                                (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].X) + (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].Scale * InformationCheckbox.CheckboxDefaultEdgeLength),
                                (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].Y)),
                            new Point(
                                (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].X),
                                (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].Y) + (int)(this.activeDatabaseJobTickets[(int)this.selectedTicketID].JobCheckboxes[i].Scale * InformationCheckbox.CheckboxDefaultEdgeLength))
                            );

                    }
                }
            }
        }



        // *****************************
        //  Template Tab 
        // *****************************
        // ************************* (Template Tab) File Menu Buttons *************************
        /// <summary>
        ///  When a user clicks on the create button in the template window, we close any openned templates and create a new ticket.
        ///  Steps to complete
        ///  1) Close template if it's active
        ///  2) Ask the user to open a word document to base the template on
        ///  3) If we receive a document to base it on, then we go ahead and create the template
        ///  4) Load it into view. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CreateLoadTemplateTabTemplate();
        }

        /// <summary>
        ///  When a user clicks on the open button in the template window, we call the open and load function to try and load
        ///  an existing template into view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenLoadTemplateTabTemplate();
        }

        /// <summary>
        ///  When the user hits this button, this function will try and save contents of the current template to a location specified by the user on the computer.
        ///     This doesn't work if there isn't an active template initialized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveTemplate();
        }

        /// <summary>
        ///  Save functionality 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveTemplateAs();
        }

        // ************************* (Template Tab) Add Menu Buttons *************************
        /// <summary>
        ///  Called once the user clicks on the add textbox button. Adds a textbox to the active template or fails if there is no active template.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textboxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If there is an active template in place, add a blank textbox to the template. Otherwise, notify the user a template isn't in place.
            if (this.activeTemplate != null)
            {
                this.activeTemplate.AddTextbox(InformationObjectConstants.DefaultTextboxName);
            }
            else
            {
                MessageBox.Show(ErrorMessages.CreateObjectMissingTemplateMessage, ErrorMessages.CreateObjectErrorTitle, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        ///  Called once the user clicks on the add checkbox button. Adds a checkbox to the active template or fails if there is no active template.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkboxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If there is an active template in place, add a blank checkbox to the template. Otherwise, notify the user a template isn't in place.
            if (this.activeTemplate != null)
            {
                this.activeTemplate.AddCheckBox(InformationObjectConstants.DefaultCheckboxName);
            }
            else
            {
                MessageBox.Show(ErrorMessages.CreateObjectMissingTemplateMessage, ErrorMessages.CreateObjectErrorTitle, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        ///  On the customer first name being clicked, this adds a textbox which has some properties restricted. 
        ///  `This textbox will represent the customer's first name on the creation page and will be filled in
        ///   with what the customer enters as their first name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customerFirstNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If there is an active template in place, add a customer first name static object to the template. Otherwise, notify the user a template isn't in place.
            if (this.activeTemplate != null)
            {
                this.activeTemplate.AddStaticObject(0);
            }
            else
            {
                MessageBox.Show(ErrorMessages.CreateObjectMissingTemplateMessage, ErrorMessages.CreateObjectErrorTitle, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        ///  On the customer last name being clicked, this adds a textbox representing the static object of the customer last name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customerLastNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If there is an active template in place, add a customer last name static object to the template. Otherwise, notify the user a template isn't in place.
            if (this.activeTemplate != null)
            {
                this.activeTemplate.AddStaticObject(1);
            }
            else
            {
                MessageBox.Show(ErrorMessages.CreateObjectMissingTemplateMessage, ErrorMessages.CreateObjectErrorTitle, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        ///  On the counter being clicked, this adds a textbox representing a counter static object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void counterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If there is an active template in place, add a counter static object to the template. Otherwise, notify the user a template isn't in place.
            if (this.activeTemplate != null)
            {
                this.activeTemplate.AddStaticObject(2);
            }
            else
            {
                MessageBox.Show(ErrorMessages.CreateObjectMissingTemplateMessage, ErrorMessages.CreateObjectErrorTitle, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        ///  On the day tool being clicked, this adds a text representing the day static object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If there is an active template in place, add a day static object to the template. Otherwise, notify the user a template isn't in place.
            if (this.activeTemplate != null)
            {
                this.activeTemplate.AddStaticObject(3);
            }
            else
            {
                MessageBox.Show(ErrorMessages.CreateObjectMissingTemplateMessage, ErrorMessages.CreateObjectErrorTitle, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        ///  On the month tool being clicked, this adds a text representing the month static object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void monthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If there is an active template in place, add a month static object to the template. Otherwise, notify the user a template isn't in place.
            if (this.activeTemplate != null)
            {
                this.activeTemplate.AddStaticObject(4);
            }
            else
            {
                MessageBox.Show(ErrorMessages.CreateObjectMissingTemplateMessage, ErrorMessages.CreateObjectErrorTitle, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        ///  On the year tool being clicked, this adds a text representing the year static object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void yearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If there is an active template in place, add a year static object to the template. Otherwise, notify the user a template isn't in place.
            if (this.activeTemplate != null)
            {
                this.activeTemplate.AddStaticObject(5);
            }
            else
            {
                MessageBox.Show(ErrorMessages.CreateObjectMissingTemplateMessage, ErrorMessages.CreateObjectErrorTitle, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        ///  On the time stamp tool being clicked, this adds a text representing the timestamp to the active template.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timeStampToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If there is an active template in place, add a year static object to the template. Otherwise, notify the user a template isn't in place.
            if (this.activeTemplate != null)
            {
                this.activeTemplate.AddStaticObject(6);
            }
            else
            {
                MessageBox.Show(ErrorMessages.CreateObjectMissingTemplateMessage, ErrorMessages.CreateObjectErrorTitle, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        ///  On the template id tool being clicked, this adds a text representing the template ID to the template.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void templateIDToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // If there is an active template in place, add a year static object to the template. Otherwise, notify the user a template isn't in place.
            if (this.activeTemplate != null)
            {
                this.activeTemplate.AddStaticObject(7);
            }
            else
            {
                MessageBox.Show(ErrorMessages.CreateObjectMissingTemplateMessage, ErrorMessages.CreateObjectErrorTitle, MessageBoxButtons.OK);
            }
        }

        // ************************* (Template Tab) Event Handlers  *************************    
        /// <summary>
        ///  When procced, loads all assets associated with the template onto the template tab window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActiveTemplateChangedEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            // If the object that procced the event was a ticket, then we load the ticket into view.
            if (sender != null)
            {
                if (sender.GetType() == typeof(TicketTemplate) && this.activeTemplate != null)
                {
                    this.LoadTemplateTabImage(this.activeTemplate);
                    this.LoadTemplateData(this.activeTemplate);
                }
            }
        }

        /// <summary>
        ///  Sets the image of the template tab's picture box to being the activeTemplateImage,
        ///  causing a paint event which will redraw all assets relating to it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RedrawTemplateData(object? sender, PropertyChangedEventArgs e)
        {
            // If the object that procced the event was a ticket, then we load the ticket into view.
            if (this.activeTemplateImage != null)
            {
                this.pictureBox1.Image = this.activeTemplateImage;
            }
        }

        /// <summary>
        ///  Called once the user enters a cell. Stores data relating to the cell for later use in cellEndEdit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView2_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Set editing to true.
            this.editingTemplateTabCell = true;

            // Store values of selected cell for use in cellEndEdit
            this.selectedCellOldValue = this.dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            this.selectedCellProperty = this.dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString();
            this.selectedCellX = e.RowIndex;
            this.selectedCellY = e.ColumnIndex;

            // If the selected cell is the required field of an information object or if it's the "X"
            //  entry (for removing objects), end edit. Handle changes in cell end edit event.
            if (this.dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value.ToString() == InformationObjectConstants.RequiredProperty ||
                this.dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value.ToString() == InformationObjectConstants.ResetEachYearProperty ||
                this.dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "X")
            {
                SendKeys.Send("{UP}");
                SendKeys.Send("{DOWN}");
            }
        }

        /// <summary>
        ///  After a cell is done being editted, this function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Set editing to false
            this.editingTemplateTabCell = false;

            // We are only editing cells in column 1. If the cell isn't in column 1, we exit. Also exit if there isn't an active template instantiated. Otherwise, continue
            if ((this.selectedCellX == null || this.selectedCellY == null) || this.selectedCellOldValue == null)
            {
                return;
            }
            else if ((int)this.selectedCellY != 1 || this.activeTemplate == null)
            {
                return;
            }

            // Find out what is being edited.
            // From the editted cell, go up until we encounter a cell whose backcolor is gray, representing the beginning of an object. From there, check the cell to the left of
            // it and that determines what we are editting.
            string? objectType = string.Empty;
            int objectIndex = -1;
            for (int i = e.RowIndex; i >= 0; i--)
            {
                // If we find the beginning of an object, check to see what the object is.
                if (this.dataGridView2.Rows[i].Cells[0].Style.BackColor == Color.Gray)
                {
                    objectType = this.dataGridView2.Rows[i].Cells[0].Value.ToString();
                    objectIndex = i;
                    i = -1;
                }
            }

            // If we haven't found it after the loop, return.
            if (objectIndex == -1)
            {
                return;
            }

            // Check for if the value is empty, if it is, restore the old value, prompt and say the entry cannot be blank, then return.
            if (this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value == null
                || this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString() == string.Empty
                || this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value == null)
            {
                MessageBox.Show(
                    ErrorMessages.PropertyEditMissingEntryErrorMessage,
                    ErrorMessages.PropertyEditErrorTitle,
                    MessageBoxButtons.OK);
                this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.selectedCellOldValue;
                return;
            }
            if (this.activeTemplateImage == null)
            {
                return;
            }

            // If statements for each type of object it could be.
            // Static Object case
            if (objectType == InformationObjectConstants.StaticObjectTitle)
            {

                // Remove case
                // Redraw afterwards
                if (this.selectedCellProperty == InformationObjectConstants.StaticObjectTitle)
                {
                    this.activeTemplate.RemoveStaticObject((string)this.dataGridView2.Rows[(int)this.selectedCellX + 1].Cells[1].Value);
                    this.pictureBox1.Image = this.activeTemplateImage;
                }

                // X Case
                else if (this.selectedCellProperty == InformationObjectConstants.XProperty)
                {
                    // If the value is a number, edit the number if able. Otherwise, prompt that a number wasn't entered right and return.
                    if (ProjectMethods.IsDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString(), 0, this.pictureBox1.Width) == true)
                    {
                        this.activeTemplate.SetStaticObjectX(
                             (string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value,
                             Convert.ToDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString()));
                    }
                    else
                    {
                        MessageBox.Show(
                            ErrorMessages.PropertyEditInvalidNumberEntryErrorMessage(0, this.pictureBox1.Width),
                            ErrorMessages.PropertyEditErrorTitle,
                            MessageBoxButtons.OK);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.selectedCellOldValue;
                        return;
                    }
                }

                // Y Case
                else if (this.selectedCellProperty == InformationObjectConstants.YProperty)
                {
                    // If the value is a number, edit the number if able. Otherwise, prompt that a number wasn't entered right and return.
                    if (ProjectMethods.IsDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString(), 0, this.pictureBox1.Height) == true)
                    {
                        this.activeTemplate.SetStaticObjectY(
                             (string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value,
                             Convert.ToDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString()));
                    }
                    else
                    {
                        MessageBox.Show(
                            ErrorMessages.PropertyEditInvalidNumberEntryErrorMessage(0, this.pictureBox1.Height),
                            ErrorMessages.PropertyEditErrorTitle,
                            MessageBoxButtons.OK);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.selectedCellOldValue;
                        return;
                    }
                }

                // Fontsize case
                else if (this.selectedCellProperty == InformationObjectConstants.FontSizeProperty)
                {
                    // If the value is a number, edit the number if able. Otherwise, prompt that a number wasn't entered right and return.
                    if (ProjectMethods.IsDouble(
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString(), 8, 100) == true)
                    {
                        this.activeTemplate.SetStaticObjectFontsize(
                            (string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value,
                            Convert.ToDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString()));
                    }
                    else
                    {
                        MessageBox.Show(
                            ErrorMessages.PropertyEditInvalidNumberEntryErrorMessage(8, 100),
                            ErrorMessages.PropertyEditErrorTitle,
                            MessageBoxButtons.OK);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.selectedCellOldValue;
                        return;
                    }
                }

                // Reset Each Year (Applies only to certain static objects, but can be set for all)
                else if (this.selectedCellProperty == InformationObjectConstants.ResetEachYearProperty)
                {
                    if (this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Style.BackColor == Color.Green)
                    {
                        this.activeTemplate.SetStaticObjectRequirement((string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value, false);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Style.BackColor = Color.Red;
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = "False";
                    }
                    else if (this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Style.BackColor == Color.Red)
                    {
                        this.activeTemplate.SetStaticObjectRequirement((string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value, true);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Style.BackColor = Color.Green;
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = "True";
                    }
                }

                // Set Counter number (Applies to counter static object only)
                else if (this.selectedCellProperty == InformationObjectConstants.CurrentCounterValueProperty)
                {
                    // If the value is a number, edit the number if able. Otherwise, prompt that a number wasn't entered right and return.
                    if (ProjectMethods.IsDouble(
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString(), 0, 100000000) == true)
                    {
                        this.activeTemplate.SetStaticObjectText(
                            (string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value,
#pragma warning disable CS8604 // Possible null reference argument.
                            (this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString()));
#pragma warning restore CS8604 // Possible null reference argument.
                    }
                    else
                    {
                        MessageBox.Show(
                            ErrorMessages.PropertyEditInvalidNumberEntryErrorMessage(0, 100000000),
                            ErrorMessages.PropertyEditErrorTitle,
                            MessageBoxButtons.OK);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.selectedCellOldValue;
                        return;
                    }
                }
            }

            // Textbox case
            else if (objectType == InformationObjectConstants.TextboxTitle)
            {
                // Remove case (Remove textbox from template)
                // Redraw afterwards.
                if (this.selectedCellProperty == InformationObjectConstants.TextboxTitle)
                {
                    this.activeTemplate.RemoveTextbox((string)this.dataGridView2.Rows[(int)this.selectedCellX + 1].Cells[1].Value);
                    this.pictureBox1.Image = this.activeTemplateImage;
                }

                // Name case
                else if (this.selectedCellProperty == InformationObjectConstants.NameProperty)
                {
                    // Set name and update name on datagridview2 in case the name was filtered.
                    this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.activeTemplate.SetTextboxName(
                        this.selectedCellOldValue,
#pragma warning disable CS8604 // Possible null reference argument.
                            this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString());
#pragma warning restore CS8604 // Possible null reference argument.
                }

                // X case
                else if (this.selectedCellProperty == InformationObjectConstants.XProperty)
                {
                    // If the value is a number, edit the number if able. Otherwise, prompt that a number wasn't entered right and return.
                    if (ProjectMethods.IsDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString(), 0, this.pictureBox1.Width) == true)
                    {
                        this.activeTemplate.SetTextboxX(
                             (string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value,
                             Convert.ToDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString()));
                    }
                    else
                    {
                        MessageBox.Show(
                            ErrorMessages.PropertyEditInvalidNumberEntryErrorMessage(0, this.pictureBox1.Width),
                            ErrorMessages.PropertyEditErrorTitle,
                            MessageBoxButtons.OK);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.selectedCellOldValue;
                        return;
                    }
                }

                // Y case
                else if (this.selectedCellProperty == InformationObjectConstants.YProperty)
                {
                    // If the value is a number, edit the number if able. Otherwise, prompt that a number wasn't entered right and return.
                    if (ProjectMethods.IsDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString(), 0, this.pictureBox1.Height) == true)
                    {
                        this.activeTemplate.SetTextboxY(
                            (string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value,
                            Convert.ToDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString()));
                    }
                    else
                    {
                        MessageBox.Show(
                            ErrorMessages.PropertyEditInvalidNumberEntryErrorMessage(0, this.pictureBox1.Height),
                            ErrorMessages.PropertyEditErrorTitle,
                            MessageBoxButtons.OK);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.selectedCellOldValue;
                        return;
                    }
                }

                // Required 
                else if (this.selectedCellProperty == InformationObjectConstants.RequiredProperty)
                {
                    if (this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Style.BackColor == Color.Green)
                    {
                        this.activeTemplate.SetTextboxRequirement((string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value, false);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Style.BackColor = Color.Red;
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = "False";
                    }
                    else if (this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Style.BackColor == Color.Red)
                    {
                        this.activeTemplate.SetTextboxRequirement((string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value, true);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Style.BackColor = Color.Green;
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = "True";
                    }
                }

                // Priority
                else if (this.selectedCellProperty == InformationObjectConstants.PriorityProperty)
                {
                    if (ProjectMethods.IsDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString(), 0, 1000) == true)
                    {
                        this.activeTemplate.SetTextboxPriority(
                            (string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value,
                            Convert.ToInt32(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString()),
                            false);
                        this.priorityChanged = true;
                    }
                    else
                    {
                        MessageBox.Show(
                            ErrorMessages.PropertyEditInvalidNumberEntryErrorMessage(0, 1000),
                            ErrorMessages.PropertyEditErrorTitle,
                            MessageBoxButtons.OK);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.selectedCellOldValue;
                        return;
                    }
                }

                // Height
                else if (this.selectedCellProperty == InformationObjectConstants.HeightProperty)
                {
                    // If the value is a number, edit the number if able. Otherwise, prompt that a number wasn't entered right and return.
                    if (ProjectMethods.IsDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString(), 1, 500) == true)
                    {
                        this.activeTemplate.SetTextboxDimensions(
                            (string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value,
                            Convert.ToDouble(this.dataGridView2.Rows[(int)this.selectedCellX + 1].Cells[1].Value.ToString()),
                            Convert.ToDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString())
                        );
                    }
                    else
                    {
                        MessageBox.Show(
                            ErrorMessages.PropertyEditInvalidNumberEntryErrorMessage(1, 500),
                            ErrorMessages.PropertyEditErrorTitle,
                            MessageBoxButtons.OK);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.selectedCellOldValue;
                        return;
                    }
                }

                // Width
                else if (this.selectedCellProperty == InformationObjectConstants.WidthProperty)
                {
                    // If the value is a number, edit the number if able. Otherwise, prompt that a number wasn't entered right and return.
                    if (ProjectMethods.IsDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString(), 1, 1000) == true)
                    {
                        this.activeTemplate.SetTextboxDimensions(
                            (string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value,
                            Convert.ToDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString()),
                            Convert.ToDouble(this.dataGridView2.Rows[(int)this.selectedCellX - 1].Cells[1].Value.ToString())
                        );
                    }
                    else
                    {
                        MessageBox.Show(
                            ErrorMessages.PropertyEditInvalidNumberEntryErrorMessage(1, 1000),
                            ErrorMessages.PropertyEditErrorTitle,
                            MessageBoxButtons.OK);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.selectedCellOldValue;
                        return;
                    }
                }

                // Fontsize
                else if (this.selectedCellProperty == InformationObjectConstants.FontSizeProperty)
                {
                    // If the value is a number, edit the number if able. Otherwise, prompt that a number wasn't entered right and return.
                    if (ProjectMethods.IsDouble(
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString(), 8, 100) == true)
                    {
                        this.activeTemplate.SetTextboxFontSize(
                            (string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value,
                            Convert.ToDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString()));
                    }
                    else
                    {
                        MessageBox.Show(
                            ErrorMessages.PropertyEditInvalidNumberEntryErrorMessage(8, 100),
                            ErrorMessages.PropertyEditErrorTitle,
                            MessageBoxButtons.OK);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.selectedCellOldValue;
                        return;
                    }
                }
            }

            // Checkbox case
            else if (objectType == InformationObjectConstants.CheckboxTitle)
            {

                // Remove case (Remove checkbox from template)
                // Redraw afterwards.
                if (this.selectedCellProperty == InformationObjectConstants.CheckboxTitle)
                {
                    this.activeTemplate.RemoveCheckbox((string)this.dataGridView2.Rows[(int)this.selectedCellX + 1].Cells[1].Value);
                    this.pictureBox1.Image = this.activeTemplateImage;
                }

                // Name case
                else if (this.selectedCellProperty == InformationObjectConstants.NameProperty)
                {
                    // Set name and update name on datagridview2 in case the name was filtered.
                    this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.activeTemplate.SetCheckboxName(
                        this.selectedCellOldValue,
#pragma warning disable CS8604 // Possible null reference argument.
                            this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString());
#pragma warning restore CS8604 // Possible null reference argument.
                }

                // X case
                else if (this.selectedCellProperty == InformationObjectConstants.XProperty)
                {
                    // If the value is a number, edit the number if able. Otherwise, prompt that a number wasn't entered right and return.
                    if (ProjectMethods.IsDouble(
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString(), 1, this.pictureBox1.Width) == true)
                    {
                        this.activeTemplate.SetCheckboxX(
                        (string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value,
                        Convert.ToDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString())
                        );
                    }
                    else
                    {
                        MessageBox.Show(
                            ErrorMessages.PropertyEditInvalidNumberEntryErrorMessage(1, this.pictureBox1.Width),
                            ErrorMessages.PropertyEditErrorTitle,
                            MessageBoxButtons.OK);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.selectedCellOldValue;
                        return;
                    }
                }

                // Y case
                else if (this.selectedCellProperty == InformationObjectConstants.YProperty)
                {
                    // If the value is a number, edit the number if able. Otherwise, prompt that a number wasn't entered right and return.
                    if (ProjectMethods.IsDouble(
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString(), 1, this.pictureBox1.Height) == true)
                    {
                        this.activeTemplate.SetCheckboxY(
                            (string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value,
                            Convert.ToDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString()));
                    }
                    else
                    {
                        MessageBox.Show(
                            ErrorMessages.PropertyEditInvalidNumberEntryErrorMessage(1, this.pictureBox1.Height),
                            ErrorMessages.PropertyEditErrorTitle,
                            MessageBoxButtons.OK);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.selectedCellOldValue;
                        return;
                    }
                }

                // Required 
                else if (this.selectedCellProperty == InformationObjectConstants.RequiredProperty)
                {
                    if (this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Style.BackColor == Color.Green)
                    {
                        this.activeTemplate.SetCheckboxRequirement((string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value, false);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Style.BackColor = Color.Red;
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = "False";
                    }
                    else if (this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Style.BackColor == Color.Red)
                    {
                        this.activeTemplate.SetCheckboxRequirement((string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value, true);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Style.BackColor = Color.Green;
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = "True";
                    }
                }

                // Priority
                else if (this.selectedCellProperty == InformationObjectConstants.PriorityProperty)
                {
                    if (ProjectMethods.IsDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString(), 0, 1000) == true)
                    {
                        this.activeTemplate.SetCheckboxPriority(
                            (string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value,
                            Convert.ToInt32(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString()),
                            false);
                        this.priorityChanged = true;
                    }
                    else
                    {
                        MessageBox.Show(
                            ErrorMessages.PropertyEditInvalidNumberEntryErrorMessage(0, 1000),
                            ErrorMessages.PropertyEditErrorTitle,
                            MessageBoxButtons.OK);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.selectedCellOldValue;
                        return;
                    }
                }

                // Scale
                else if (this.selectedCellProperty == InformationObjectConstants.PriorityProperty)
                {
                    // If the value is a number, edit the number if able. Otherwise, prompt that a number wasn't entered right and return.
                    if (ProjectMethods.IsDouble(
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString(), 1, 100) == true)
                    {
                        this.activeTemplate.SetCheckboxScale(
                            (string)this.dataGridView2.Rows[objectIndex + 1].Cells[1].Value,
                            Convert.ToDouble(this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value.ToString()));
                    }
                    else
                    {
                        MessageBox.Show(
                            ErrorMessages.PropertyEditInvalidNumberEntryErrorMessage(1, 100),
                            ErrorMessages.PropertyEditErrorTitle,
                            MessageBoxButtons.OK);
                        this.dataGridView2.Rows[(int)this.selectedCellX].Cells[1].Value = this.selectedCellOldValue;
                        return;
                    }
                }
            }
        }

        /// <summary>
        ///  As the mouse moves across the picture box, this function draws the location next to the mouse so that the user can see the coordinates of the current point.
        ///  Only applies in template tab picture.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            this.RedrawTemplateData(sender, new PropertyChangedEventArgs(""));
        }

        /// <summary>
        ///  This event redraws the current template tab image with its information objects when different parts are loaded into view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // If the object that procced the event was a ticket, then we load the ticket into view.
            if (sender != null && this.activeTemplate != null)
            {
                // Display mouse
                this.DisplayMouseCoordinates(e.Graphics, 400, 100);

                // Static Objects
                for (int i = 0; i < this.activeTemplate.StaticObjects.Count; i++)
                {
                    // Add static object to image
                    this.LoadStaticObjectToTemplatePicture(this.activeTemplate.StaticObjects[i], e.Graphics);
                }
                // Textboxes
                for (int i = 0; i < this.activeTemplate.TemplateTextboxes.Count; i++)
                {
                    // Add textbox to image
                    this.LoadTextboxToTemplatePicture(this.activeTemplate.TemplateTextboxes[i], e.Graphics);
                }
                // Checkboxes
                for (int i = 0; i < this.activeTemplate.TemplateCheckboxes.Count; i++)
                {
                    // Add checkbox to image
                    this.LoadCheckboxToTemplatePicture(this.activeTemplate.TemplateCheckboxes[i], e.Graphics);
                }
            }
        }

        /// <summary>
        ///  When the tab page 2 is redrawn...
        ///     - Organize the template tab datagridview if the priority has changed recently.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabPage2_Paint(object sender, PaintEventArgs e)
        {
            // If priority changed was set to true, redraw the template data
            if (this.priorityChanged == true && this.activeTemplate != null)
            {
                this.activeTemplate.SortCheckboxes();
                this.activeTemplate.SortTextboxes();
                this.LoadTemplateData(this.activeTemplate);
                this.priorityChanged = false;
            }
        }



        // *****************************
        //  Create Ticket Tab
        // *****************************
        // ************************* (Create Ticket Tab) Button Handlers  *************************    
        /// <summary>
        ///  On the button being pressed, this button begins creating a job ticket based on the creationTemplateBase template.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // If the template has been instantiated, begin creation of a job ticket.
            if (this.creationTemplateBase != null && this.creationTemplateBaseImage != null)
            {
                this.CreateAndSaveTicket();
            }
            else
            {
                MessageBox.Show(ErrorMessages.CreateTicketMissingBaseTemplateMessage, ErrorMessages.CreateTicketErrorTitle, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        ///  On click, allow user to select a template to load into view. This template will be the base for any ticket created from it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void templateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenLoadCreateTicketTabTemplate();
        }

        // ************************* (Create Ticket Tab) Event Handlers  *************************    
        /// <summary>
        ///  Redraws the information objects onto the create ticket tab image whenever the paint event occurs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            // Load template data into view
            if (this.creationTemplateBaseImage != null && this.creationTemplateBase != null)
            {   // Static Objects
                for (int i = 0; i < this.creationTemplateBase.StaticObjects.Count; i++)
                {
                    // Add static object to image
                    this.LoadStaticObjectToTemplatePicture(this.creationTemplateBase.StaticObjects[i], e.Graphics);
                }
                // Textboxes
                for (int i = 0; i < this.creationTemplateBase.TemplateTextboxes.Count; i++)
                {
                    // Add textbox to image
                    this.LoadTextboxToTemplatePicture(this.creationTemplateBase.TemplateTextboxes[i], e.Graphics);
                }
                // Checkboxes
                for (int i = 0; i < this.creationTemplateBase.TemplateCheckboxes.Count; i++)
                {
                    // Add checkbox to image
                    this.LoadCheckboxToTemplatePicture(this.creationTemplateBase.TemplateCheckboxes[i], e.Graphics);
                }
            }
        }



        // *****************************
        //  General
        // *****************************
        // ************************* Generic Event Handlers *************************
        /// <summary>
        ///  Keyboard events corresponding to each different tab. When a key is pressed, this event happens.
        ///  Designed to handle hotkey setups like control+s to save as an example.
        ///     When a key is pressed, the variable "keyPressed" becomes true.
        ///     -While isPressed is true, no other keyboard events can occur.
        ///  Some occur only if an active Template is in place.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If control + another key is pressed while no other keys are pressed...
            if (!this.keyPressed && ModifierKeys == Keys.Control)
            {
                // Handle hotkeys based on the selected tab.
                // Job Ticket tab keydown events
                if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage1"])
                {
                    // Ctrl + D = Delete Functionality
                    if (e.KeyChar == (char)4)
                    {
                        this.DeleteSelectedTicket();
                    }
                    
                    // Ctrl + O = Open for Modification Functionality.
                    else if (e.KeyChar == (char)15)
                    {
                        this.ModifyAndSaveSelectedTicket();
                    }

                    // Ctrl + P = Print Functionality
                    else if (e.KeyChar == (char)16)
                    {
                        this.PrintSelectedJobTicket();
                    }
                }
                // Template tab keydown events
                else if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage2"])
                {
                    // Ctrl + S = Save Functionality. Occurs only if no cells are being edited and if there is an active template. char 19 = S key 
                    if (e.KeyChar == (char)19 && (!this.editingTemplateTabCell && this.activeTemplate != null))
                    {
                        this.SaveTemplate();
                    }

                    // Ctrl + O = Open Functionality. Occurs if no cells are being edited. char 15 = O key
                    else if (e.KeyChar == (char)15 && !this.editingTemplateTabCell)
                    {
                        this.OpenLoadTemplateTabTemplate();
                    }

                    // Ctrl + N = New Functionality. Occurs if no cells are being edited. char 14 = N key
                    else if (e.KeyChar == (char)14 && !this.editingTemplateTabCell)
                    {
                        this.CreateLoadTemplateTabTemplate();
                    }

                    // Ctrl + T = Add Textbox Functionality. Occurs if there is an active template. char 20 = T key
                    else if (e.KeyChar == (char)20 && this.activeTemplate != null)
                    {
                        this.activeTemplate.AddTextbox(InformationObjectConstants.DefaultTextboxName);
                    }

                    // Ctrl + K = Add Checkbox Functionality. Occurs if there is an active template. char 11 = K key
                    else if (e.KeyChar == (char)11 && this.activeTemplate != null)
                    {
                        this.activeTemplate.AddCheckBox(InformationObjectConstants.DefaultCheckboxName);
                    }
                }
                // Create Ticket tab keydown events
                else if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage3"])
                {
                    // Ctrl + O = Open Template
                    if (e.KeyChar == (char)15)
                    {
                        this.OpenLoadCreateTicketTabTemplate();
                    }

                    // Ctrl + N = New Ticket based on current create ticket tab template
                    else if (e.KeyChar == (char)14)
                    {
                        // If the template has been instantiated, begin creation of a job ticket.
                        if (this.creationTemplateBase != null && this.creationTemplateBaseImage != null)
                        {
                            this.CreateAndSaveTicket();
                        }
                        else
                        {
                            MessageBox.Show(ErrorMessages.CreateTicketMissingBaseTemplateMessage, ErrorMessages.CreateTicketErrorTitle, MessageBoxButtons.OK);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Handles when a key press is released.
        ///     Main functionality releases "keyPressed" into false allowing for other inputs to be accepted in the keydown event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_KeyUp(object sender, KeyEventArgs e)
        {
            // Release keyPressed to false when a key is lifted.
            this.keyPressed = false;
        }

        /// <summary>
        ///  This event fires whenever the tab changes in the main tab control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (this.activeTemplate != null && this.activeTemplateSavedFilePath != null)
            //{
            //    this.SaveTemplate(false);
            //}
            // When the user switches to the first tab control with the job tickets, reload all tickets in the active database.
            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage1"])
            {
                this.InitializeJobTicketTab();
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage2"])
            {
                //if (this.activeTemplate != null && this.activeTemplateSavedFilePath != null)
                //{
                //    this.OpenLoadTemplateTabTemplate(this.activeTemplateSavedFilePath, false);
                //}
            }
            // When the user switches to the third tab control with the creation window, reload the template in the tab if there was one previously loaded.
            else if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage3"])
            {
                if (this.creationTemplateBase != null && this.creationTemplateSavedFilePath != null)
                {
                    this.OpenLoadCreateTicketTabTemplate(this.creationTemplateSavedFilePath);
                }
            }

            // Reset tab specific variables
            this.editingTemplateTabCell = false;
        }

        /// <summary>
        ///  On the form closing, this event saves features to be 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If the user is the entity closing the application, save system data before exitting. Save any active template as well before exiting
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Save system database
                this.SystemDatabaseSave();
            }
        }

        /// <summary>
        ///  Notifies whenever a property of the current user is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyUserPropertyChanged()
        {
            this.UserPropertyChanged.Invoke(this, new PropertyChangedEventArgs("userPropertyChanged"));
        }
    }
}