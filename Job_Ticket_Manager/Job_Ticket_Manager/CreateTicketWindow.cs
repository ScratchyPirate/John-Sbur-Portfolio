/// <summary>
///  CreateTicketWindow.cs
///  Job Ticket Manager Project
///  Creator: John D. Sbur
/// </summary>
namespace Job_Ticket_Manager
{
    using JobTicketEngine;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    public partial class CreateTicketWindow : Form
    {
        // Storage variables for information gathered in the window.
        private List<Label> inputLabels;
        private List<ProjectTextbox> inputTextBoxes;
        private List<ComboBox> inputCheckBoxes;

        // Customer Associated Variables
        private string? customerFirstName;
        private string? customerLastName;

        // Bool associated with the completion of the form. Set to false initially and only true once the form is complete.
        private bool complete;

        // Storage for the TicketTemplate used to create the form's elements.
        private TicketTemplate? creationTemplate;
        private JobTicket? creationTicket;

        // Variables used in window creation
        private int currentX;
        private int currentY;
        private int maxWidthInColumn;
        private int currentTextboxID;

        /// <summary>
        ///  Constructor for create ticket window. Creates an empty window based on a template.
        /// </summary>
        public CreateTicketWindow(TicketTemplate creationBase)
        {
            // Set creationticket to null since we aren't using it in this case.
            this.creationTicket = null;

            // Sets up create ticket window to be centered on screen and have components initialized according to its designer.
            this.InitializeComponent();
            this.CenterToScreen();

            // Keeps track of where the objects are being placed or about to be placed. Starts at the separator label's x, y coordinate.
            this.currentX = this.label4.Location.X + 10;
            this.currentY = this.label4.Location.Y + this.label4.Height;
            this.maxWidthInColumn = 0;

            // Add intial separator
            this.AddSeparator(
                new Size(8, this.panel1.Height - (this.label4.Location.Y) - this.label4.Height - this.label5.Height),
                new Point(0, this.label4.Location.Y + this.label4.Height)
                );

            // Initialize storage lists on startup
            this.inputLabels = new List<Label>();
            this.inputTextBoxes = new List<ProjectTextbox>();
            this.inputCheckBoxes = new List<ComboBox>();
            this.inputLabels.Clear();
            this.inputTextBoxes.Clear();
            this.inputCheckBoxes.Clear();

            // Keeps track of textbox ID
            currentTextboxID = 0;

            // Load information objects based on the creationBase's contents
            // Textboxes + Checkboxes
            // Textboxes and Checkboxes
            for (int i = 0, j = 0; i < creationBase.TemplateTextboxes.Count || j < creationBase.TemplateCheckboxes.Count;)
            {
                // Compare the priority of the objects on i and j. Put the one with the lowest priority on first and increment the 
                //  counter associated with its list. 
                //  i => Textbox List
                //  j => Checkbox List
                // If one has reached the end of its list, put the other one's next content up and continue
                if (i == creationBase.TemplateTextboxes.Count && j == creationBase.TemplateCheckboxes.Count)
                {
                    break;
                }

                // If there are no more textboxes to look at, add the next checkbox
                else if (i == creationBase.TemplateTextboxes.Count)
                {
                    this.AddCheckbox(creationBase.TemplateCheckboxes[j], true);

                    j++;
                }

                // If there are no more checkboxes to look at, add the next textbox
                else if (j == creationBase.TemplateCheckboxes.Count)
                {
                    this.AddTextbox(creationBase.TemplateTextboxes[i], true);

                    i++;
                }

                // If there are entries in both lists, they should be organized, so compare their front member's priorities and add the one with the lowest priority.
                else
                {
                    // Compare priorities
                    if (creationBase.TemplateTextboxes[i].Priority < creationBase.TemplateCheckboxes[j].Priority)
                    {
                        this.AddTextbox(creationBase.TemplateTextboxes[i], true);

                        i++;
                    }
                    else
                    {
                        this.AddCheckbox(creationBase.TemplateCheckboxes[j], true);

                        j++;
                    }
                }

            }

            // Initialize other locals.
            this.complete = false;
            this.creationTemplate = creationBase;

            // Subscribe the first and last name textboxes to the ontextchanged event
            this.textBox1.MaxLength = InformationObjectConstants.CustomerNameMaxCharacters;
            this.textBox2.MaxLength = InformationObjectConstants.CustomerNameMaxCharacters;
        }

        /// <summary>
        ///  Constructor for create ticket window. Creates a window with entries filled based on the job ticket entered.
        /// </summary>
        /// <param name="creationBase"></param>
        public CreateTicketWindow(JobTicket creationBase)
        {
            // Set creation template to null since we aren't using it in this case
            this.creationTemplate = null;

            // Sets up create ticket window to be centered on screen and have components initialized according to its designer.
            this.InitializeComponent();
            this.CenterToScreen();

            // Keeps track of where the objects are being placed or about to be placed. Starts at the separator label's x, y coordinate.
            this.currentX = this.label4.Location.X + 10;
            this.currentY = this.label4.Location.Y + this.label4.Height;
            this.maxWidthInColumn = 0;

            // Add intial separator
            this.AddSeparator(
                new Size(8, this.panel1.Height - (this.label4.Location.Y) - this.label4.Height - this.label5.Height),
                new Point(0, this.label4.Location.Y + this.label4.Height)
                );

            // Set the title to "Modify ticket" instead of "Create Ticket"
            this.Text = "Modify Ticket";

            // Initialize textbox1 and textbox2 as containing the customer name
            this.textBox1.Text = creationBase.CustomerFirstName;
            this.textBox2.Text = creationBase.CustomerLastName;

            // Set the customer name and last name textboxes as being read only so the names can't be changed.s
            this.textBox1.ReadOnly = true;
            this.textBox2.ReadOnly = true;

            // Initialize storage lists on startup
            this.inputLabels = new List<Label>();
            this.inputTextBoxes = new List<ProjectTextbox>();
            this.inputCheckBoxes = new List<ComboBox>();
            this.inputLabels.Clear();
            this.inputTextBoxes.Clear();
            this.inputCheckBoxes.Clear();

            // Keeps track of textbox ID
            currentTextboxID = 0;

            // Load information objects based on the creationBase's contents
            // Textboxes + Checkboxes
            // Textboxes and Checkboxes
            for (int i = 0, j = 0; i < creationBase.JobTextboxes.Count || j < creationBase.JobCheckboxes.Count;)
            {
                // Compare the priority of the objects on i and j. Put the one with the lowest priority on first and increment the 
                //  counter associated with its list. 
                //  i => Textbox List
                //  j => Checkbox List
                // If one has reached the end of its list, put the other one's next content up and continue
                if (i == creationBase.JobTextboxes.Count && j == creationBase.JobCheckboxes.Count)
                {
                    break;
                }

                // If there are no more textboxes to look at, add the next checkbox
                else if (i == creationBase.JobTextboxes.Count)
                {
                    this.AddCheckbox(creationBase.JobCheckboxes[j]);

                    j++;
                }

                // If there are no more checkboxes to look at, add the next textbox
                else if (j == creationBase.JobCheckboxes.Count)
                {
                    this.AddTextbox(creationBase.JobTextboxes[i]);

                    i++;
                }

                // If there are entries in both lists, they should be organized, so compare their front member's priorities and add the one with the lowest priority.
                else
                {
                    // Compare priorities
                    if (creationBase.JobTextboxes[i].Priority < creationBase.JobCheckboxes[j].Priority)
                    {
                        this.AddTextbox(creationBase.JobTextboxes[i]);

                        i++;
                    }
                    else
                    {
                        this.AddCheckbox(creationBase.JobCheckboxes[j]);

                        j++;
                    }
                }

            }

            // Initialize other locals.
            this.complete = false;
            this.creationTicket = creationBase;
        }

        /// <summary>
        ///  Initiates a new label based on inputs. Returns it once created.
        /// </summary>
        /// <returns> Newly created label </returns>
        private Label CreateLabel(string labelText, bool highlight = false)
        {
            // Label
            Label newLabel = new Label();
            newLabel.AutoSize = true;
            newLabel.Text = labelText;
            // Add red background to label if the object is required to be filled out.
            if (highlight == true)
            {
                newLabel.BackColor = Color.Yellow;
            }

            return newLabel;
        }

        /// <summary>
        ///  Adds a separator based on inputted position and size to the create window tab.
        /// </summary>
        private void AddSeparator(Size separatorSize, Point separatorLocation)
        {
            Label separator = new Label();
            separator.Size = separatorSize;
            separator.Location = separatorLocation;
            separator.BackColor = Color.Black;
            this.panel1.Controls.Add(separator);
        }

        /// <summary>
        ///  Given an inputted textbox, this function adds it to the window.
        ///   If the object is new, entries are made blank.
        /// </summary>
        /// <param name="newTextbox"></param>
        private void AddTextbox(InformationTextbox newTextbox, bool newObject = false)
        {
            int requiredHeight = 0;
            int requiredWidth = 0;

            // Initialize objects for textbox.
            // Label
            Label textboxLabel = this.CreateLabel(newTextbox.Name, newTextbox.Required);

            // Textbox
            ProjectTextbox textboxTextBox = new ProjectTextbox();
            textboxTextBox.Multiline = true;
            textboxTextBox.Size = new Size(new Point((int)newTextbox.Width, (int)newTextbox.Height));
            textboxTextBox.ProjectFont = PaintingConstants.Arial((int)newTextbox.FontSize);
            textboxTextBox.ID = currentTextboxID;
            textboxTextBox.Name = newTextbox.Name;
            this.currentTextboxID++;
            // Set textbox to match jobtickets's textbox or leave blank if it's from a template. Remove any new line characters found 
            if (newObject == false)
            {
                textboxTextBox.Text = newTextbox.Text.Trim('\n');
                textboxTextBox.TextStorage = textboxTextBox.Text;
            }
            //
            textboxTextBox.TextChanged += this.OnProjectTextboxTextChange;

            // Calculate space needed as a rectangle (max width between the label and textbox * (height of label + height of textbox))
            requiredWidth = (textboxLabel.Size.Width > textboxTextBox.Size.Width) ? textboxLabel.Size.Width : textboxTextBox.Size.Width;
            requiredHeight = textboxLabel.Size.Height + textboxTextBox.Size.Height;

            // If the space required exceeds the lower bound of the panel, move over by the maxWidthColumn amount and place it there instead
            if (requiredHeight + this.currentY > this.panel1.Size.Height)
            {
                this.currentX += this.maxWidthInColumn + 10;

                // Add a divider at the max width to separate the columns
                this.AddSeparator(
                    new Size(6, this.panel1.Height - (this.label4.Location.Y) - this.label4.Height - this.label5.Height),
                    new Point(currentX - 8, this.label4.Location.Y + this.label4.Height)
                    );

                // Increase the label4 length.
                this.label4.Size = new Size(this.label4.Size.Width + this.maxWidthInColumn, this.label4.Size.Height);

                // Update label 5 (bottom label) to be equal to label 4 to bound the window.
                this.label5.Size = this.label4.Size;

                this.currentY = this.label4.Location.Y + this.label4.Height;
                this.maxWidthInColumn = 0;
            }

            // Calculate maxWidthColumn
            if (textboxLabel.Size.Width > maxWidthInColumn)
            {
                maxWidthInColumn = textboxLabel.Size.Width;
            }
            if (textboxTextBox.Size.Width > maxWidthInColumn)
            {
                maxWidthInColumn = textboxTextBox.Size.Width;
            }

            // Place the label and textbox on the window.
            textboxLabel.Location = new Point(currentX, currentY);
            this.panel1.Controls.Add(textboxLabel);
            currentY += textboxLabel.Size.Height;
            textboxTextBox.Location = new Point(currentX, currentY);
            this.panel1.Controls.Add(textboxTextBox);

            // Add labels and textboxes to storage lists for later use
            this.inputLabels.Add(textboxLabel);
            this.inputTextBoxes.Add(textboxTextBox);
            currentY += textboxTextBox.Size.Height + 10;
        }

        /// <summary>
        ///  Given an inputted checkbox, this function adds it to the window.
        /// </summary>
        /// <param name="newCheckbox"></param>
        /// <param name="newObject">
        ///  If newObject = true, then the combo box created that's associated with the checkbox will be set
        ///  to index 0 instead of based on its status
        /// </param>
        private void AddCheckbox(InformationCheckbox newCheckbox, bool newObject = false)
        {
            int requiredWidth = 0;
            int requiredHeight = 0;

            // Initialize objects for checkbox.
            // Label
            Label checkboxLabel = this.CreateLabel(newCheckbox.Name, newCheckbox.Required);

            // Checkbox's combobox
            ComboBox checkboxComboBox = new ComboBox();
            checkboxComboBox.Name = newCheckbox.Name;
            checkboxComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            checkboxComboBox.Items.Add(CreationWindowConstants.ComboBoxDefault);
            checkboxComboBox.Items.Add(CreationWindowConstants.ComboBoxTrue);
            checkboxComboBox.Items.Add(CreationWindowConstants.ComboBoxFalse);
            // Set combobox status to match job ticket's checkbox status or 0 if this is a new ticket.
            if (newObject == true)
            {
                checkboxComboBox.SelectedIndex = 0;
            }
            else if (newCheckbox.Status == false)
            {
                checkboxComboBox.SelectedIndex = 2;
            }
            else
            {
                checkboxComboBox.SelectedIndex = 1;
            }
            //

            // Calculate space needed as a rectangle (max width between the label and textbox * (height of label + height of textbox))
            requiredWidth = (checkboxLabel.Size.Width > checkboxComboBox.Size.Width) ? checkboxLabel.Size.Width : checkboxComboBox.Size.Width;
            requiredHeight = checkboxLabel.Size.Height + checkboxComboBox.Size.Height;

            // If the space required exceeds the lower bound of the panel, move over by the maxWidthColumn amount and place it there instead
            if (requiredHeight + this.currentY > this.panel1.Size.Height)
            {
                this.currentX += maxWidthInColumn + 10;

                // Add a divider at the max width to separate the columns
                this.AddSeparator(
                    new Size(6, this.panel1.Height - (this.label4.Location.Y) - this.label4.Height - this.label5.Height),
                    new Point(currentX - 8, this.label4.Location.Y + this.label4.Height)
                    );

                // Increase the label4 length.
                this.label4.Size = new Size(this.label4.Size.Width + maxWidthInColumn, this.label4.Size.Height);

                // Update label 5 (bottom label) to be equal to label 4 to bound the window.
                this.label5.Size = this.label4.Size;

                currentY = this.label4.Location.Y + this.label4.Height;
                maxWidthInColumn = 0;
            }

            // Calculate maxWidthColumn
            if (checkboxLabel.Size.Width > maxWidthInColumn)
            {
                maxWidthInColumn = checkboxLabel.Size.Width;
            }
            if (checkboxComboBox.Size.Width > maxWidthInColumn)
            {
                maxWidthInColumn = checkboxComboBox.Size.Width;
            }

            // Place the label and textbox on the window.
            checkboxLabel.Location = new Point(currentX, currentY);
            this.panel1.Controls.Add(checkboxLabel);
            currentY += checkboxLabel.Size.Height;
            checkboxComboBox.Location = new Point(currentX, currentY);
            this.panel1.Controls.Add(checkboxComboBox);

            // Add labels and textboxes to storage lists for later use
            this.inputLabels.Add(checkboxLabel);
            this.inputCheckBoxes.Add(checkboxComboBox);
            currentY += checkboxComboBox.Size.Height + 10;
        }

        /// <summary>
        ///  Getter for complete variable in this form.
        /// </summary>
        public bool Complete
        {
            get { return this.complete; }
        }

        /// <summary>
        ///  Returns the entered first name of the customer
        /// </summary>
        public string? CustomerFirstName
        {
            get { return this.customerFirstName; }
        }

        /// <summary>
        ///  Returns the entered last name of the customer
        /// </summary>
        public string? CustomerLastName
        {
            get { return this.customerLastName; }
        }

        /// <summary>
        ///  Gets the input labels from the window.
        /// </summary>
        public List<Label> InputLabels
        {
            get { return this.inputLabels; }
        }

        /// <summary>
        ///  Gets or Sets the input textboxes from the window.
        /// </summary>
        public List<ProjectTextbox> InputTextboxes
        {
            get { return this.inputTextBoxes; }
            set { this.inputTextBoxes = value; }
        }

        /// <summary>
        ///  Gets or Sets the input checkboxes from the window.
        /// </summary>
        public List<ComboBox> InputCheckboxes
        {
            get { return this.inputCheckBoxes; }
            set { this.inputCheckBoxes = value; }
        }

        /// <summary>
        ///  This checks if all required checkboxes and textboxes have been interacted with. If they have,
        ///     then we exit out. Otherwise, we send a message that not all required boxes have been filled out.
        /// </summary>
        private void Submit()
        {
            // Perform actions depending on which is active, either a job ticket or a template.
            // Template
            if (this.creationTemplate != null)
            {
                // Check customer textboxes.
                if (this.textBox1.Text == null ||
                    this.textBox1.Text == string.Empty ||
                    this.textBox2.Text == null ||
                    this.textBox2.Text == string.Empty)
                {
                    MessageBox.Show(ErrorMessages.CreateTicketMissingRequiredFieldsMessage, ErrorMessages.CreateTicketErrorTitle, MessageBoxButtons.OK);
                    return;
                }

                // Check textboxes
                for (int i = 0; i < this.inputTextBoxes.Count && i < this.creationTemplate.TemplateTextboxes.Count; i++)
                {
                    if (this.creationTemplate.TemplateTextboxes[i].Required == true &&
                        (this.inputTextBoxes[i].Text == string.Empty ||
                        this.inputTextBoxes[i].Text == null))
                    {
                        MessageBox.Show(ErrorMessages.CreateTicketMissingRequiredFieldsMessage, ErrorMessages.CreateTicketErrorTitle, MessageBoxButtons.OK);
                        return;
                    }
                }

                // Check checkboxes
                for (int i = 0; i < this.inputCheckBoxes.Count && i < this.creationTemplate.TemplateCheckboxes.Count; i++)
                {
                    if (this.creationTemplate.TemplateCheckboxes[i].Required == true &&
                        (this.inputCheckBoxes[i].SelectedIndex == 0))
                    {
                        MessageBox.Show(ErrorMessages.CreateTicketMissingRequiredFieldsMessage, ErrorMessages.CreateTicketErrorTitle, MessageBoxButtons.OK);
                        return;
                    }
                }
            }
            else
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                // Check customer textboxes.
                if (this.textBox1.Text == null ||
                    this.textBox1.Text == string.Empty ||
                    this.textBox2.Text == null ||
                    this.textBox2.Text == string.Empty)
                {
                    MessageBox.Show(ErrorMessages.ModifyTicketMissingRequiredFieldsMessage, ErrorMessages.ModifyTicketErrorTitle, MessageBoxButtons.OK);
                    return;
                }

                // Check textboxes
                for (int i = 0; i < this.inputTextBoxes.Count && i < this.creationTicket.JobTextboxes.Count; i++)
                {
                    if (this.creationTicket.JobTextboxes[i].Required == true &&
                        (this.inputTextBoxes[i].Text == string.Empty ||
                        this.inputTextBoxes[i].Text == null))
                    {
                        MessageBox.Show(ErrorMessages.ModifyTicketMissingRequiredFieldsMessage, ErrorMessages.ModifyTicketErrorTitle, MessageBoxButtons.OK);
                        return;
                    }
                }

                // Check checkboxes
                for (int i = 0; i < this.inputCheckBoxes.Count && i < this.creationTicket.JobCheckboxes.Count; i++)
                {
                    if (this.creationTicket.JobCheckboxes[i].Required == true &&
                        (this.inputCheckBoxes[i].SelectedIndex == 0))
                    {
                        MessageBox.Show(ErrorMessages.ModifyTicketMissingRequiredFieldsMessage, ErrorMessages.ModifyTicketErrorTitle, MessageBoxButtons.OK);
                        return;
                    }
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }


            // Set textbox text to contain newline markers where they should be based on the NewLinesAt list
            for (int i = 0; i < this.inputTextBoxes.Count; i++)
            {
                // Unsubscribe from the OnProjectTextboxTextChange event for each textbox
                this.inputTextBoxes[i].TextChanged -= this.OnProjectTextboxTextChange;

                // Add new lines into the textbox at submission time.
                using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
                {
                    SizeF stringSize = g.MeasureString(this.inputTextBoxes[i].Text, this.inputTextBoxes[i].Font);

                    if ((stringSize.Width) >= this.inputTextBoxes[i].Width)
                    {
                        // Calculate where the new line characters need to go.
                        string substring = string.Empty;
                        for (int j = 0; j < this.inputTextBoxes[i].Text.Length; j++)
                        {
                            // Add the next character in the text to the substring.
                            substring = substring.Insert(substring.Length, this.inputTextBoxes[i].Text[j].ToString());

                            // Measure the substring, if its length is greater than the width of the textbox, make a note of it
                            // in new line at and then reset the substring
                            if ((substring.Length >= (this.inputTextBoxes[i].Width / (g.MeasureString("X", this.Font).Width)) - 1))
                            {
                                this.inputTextBoxes[i].NewLineAt.Add(j);
                                substring = string.Empty;
                            }
                        }
                    }
                }

                // Insert new lines at the requested indexes
                for (int j = 0; j < this.inputTextBoxes[i].NewLineAt.Count; j++)
                {
                    this.inputTextBoxes[i].Text = this.inputTextBoxes[i].Text.Insert(this.inputTextBoxes[i].NewLineAt[j], "\n");
                }

            }

            // Set first and last name
            this.customerFirstName = this.textBox1.Text;
            this.customerLastName = this.textBox2.Text;

            // If we are at this point, set complete to true and close the window.
            this.complete = true;
            this.Close();
        }

        /// <summary>
        ///  Triggers the submit function when the submit button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Submit();
        }

        /// <summary>
        ///  If the cancel button is hit, exit out of the window without saving anything. Complete is still marked as false.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            // If we hit cancel, simply exit out without saving anything.
            this.Close();
        }

        /// <summary>
        ///  This event triggers whenever a textbox is changed. Limits the number of text that can be entered into a textbox listening to this event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// Prevents the event for going on forever.
        private void OnProjectTextboxTextChange(object? sender, EventArgs e)
        {
            // If sender is null, stop
            if (sender == null)
            {
                return;
            }

            // If the sender was a project textbox, go to project textbox case.
            if (sender.GetType() == typeof(ProjectTextbox))
            {
                // Create a temporary textbox from the sender
                ProjectTextbox textBox = (ProjectTextbox)sender;

                // If the number of characters in the textbox is greater than the maximum amount of characters in the textbox, set the
                // Text to the storage text within the project textbox
                if (textBox.Text.Length > textBox.MaxCharacters)
                {
                    // Update the textbox's textstorage based on its ID number
                    for (int i = 0; i < this.InputTextboxes.Count; i++)
                    {
                        if (textBox.ID == this.InputTextboxes[i].ID)
                        {
                            this.inputTextBoxes[i].Text = textBox.TextStorage;

                            // Show user that maximum number of characters have been entered. Message shown depending on what is being done, either modification or creation
                            if (this.creationTemplate != null)
                            {
                                MessageBox.Show(ErrorMessages.CreateTicketMaximumCharactersReachedMessage, ErrorMessages.CreateTicketErrorTitle, MessageBoxButtons.OK);
                            }
                            else
                            {
                                MessageBox.Show(ErrorMessages.ModifyTicketMaximumCharactersReachedMessage, ErrorMessages.ModifyTicketErrorTitle, MessageBoxButtons.OK);
                            }
                            return;
                        }
                    }
                }
                else
                {
                    // Update the textbox's textstorage based on its ID number
                    for (int i = 0; i < this.InputTextboxes.Count; i++)
                    {
                        if (textBox.ID == this.InputTextboxes[i].ID)
                        {
                            this.inputTextBoxes[i].TextStorage = textBox.Text;
                        }
                    }
                }
            }
        }
    }
}
