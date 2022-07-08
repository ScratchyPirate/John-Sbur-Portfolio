/// <summary>
///  JobTicket.cs
///  Job Ticket Manager Project
///  Creator: John D. Sbur
/// </summary>
namespace JobTicketEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    public class JobTicket
    {
        /// <summary>
        ///  Storage unit for all objects in a template.
        /// </summary>
        private List<InformationTextbox> jobTextboxes;
        private List<InformationCheckbox> jobCheckboxes;
        private List<InformationTextbox> jobStaticObjects;

        /// <summary>
        ///  Path of the document the template is based on.
        /// </summary>
        private string documentPath;

        /// <summary>
        ///  Keeps track of the name of the customer associated with this object.
        /// </summary>
        private string customerFirstName;
        private string customerLastName;

        /// <summary>
        ///  Constructor for the ticket template. Must have a document to base itself off of.
        /// </summary>
        public JobTicket()
        {
            this.jobTextboxes = new List<InformationTextbox>();
            this.jobTextboxes.Clear();
            this.jobCheckboxes = new List<InformationCheckbox>();
            this.jobCheckboxes.Clear();
            this.jobStaticObjects = new List<InformationTextbox>();
            this.jobStaticObjects.Clear();

            this.documentPath = string.Empty;
            this.customerFirstName = string.Empty;
            this.customerLastName = string.Empty;
        }

        /// <summary>
        ///  These two functions search for if the name inputted exists in 
        /// </summary>
        /// <param name="targetTextbox"></param>
        /// <returns>
        ///  1) A number greater than 0 if found
        ///  2) -1 if not found.
        /// </returns>
        public int SearchTextbox(string targetTextbox)
        {
            int targetIndex = -1;
            for (int i = 0; i < this.jobTextboxes.Count; i++)
            {
                if (this.jobTextboxes[i].Name == targetTextbox)
                {
                    targetIndex = i;
                }
            }
            return targetIndex;
        }
        public int SearchCheckbox(string targetCheckbox)
        {
            int targetIndex = -1;
            for (int i = 0; i < this.jobCheckboxes.Count; i++)
            {
                if (this.jobCheckboxes[i].Name == targetCheckbox)
                {
                    targetIndex = i;
                }
            }
            return targetIndex;
        }

        /// <summary>
        ///  Searches for static objects within the job ticket's static object list.
        /// </summary>
        /// <param name="targetObject"></param>
        /// <returns></returns>
        public int SearchStaticObject(string targetObject)
        {
            int targetIndex = -1;
            for (int i = 0; i < this.jobStaticObjects.Count; i++)
            {
                if (this.jobStaticObjects[i].Name == targetObject)
                {
                    targetIndex = i;
                }
            }
            return targetIndex;
        }

        /// <summary>
        ///  These two functions filter the name inputted based on their respective list and returns the filtered name. The format is "name (x)" where x is the number 
        ///   of copies of the name in the list.
        /// </summary>
        private string FilterTextboxName(string unfilteredName)
        {
            // Filter the name so it matches the naming conventions "name (x)"
            int numberOfCopies = 0;
            string unfilteredNameCopy = unfilteredName;
            string filteredName = unfilteredName;
            for (int i = 0; i < this.jobTextboxes.Count; i++)
            {
                if (filteredName == this.jobTextboxes[i].Name)
                {
                    // Increase the number of copies detected.
                    numberOfCopies++;

                    // Set the name to the format "name (x)"
                    filteredName = unfilteredNameCopy + " (" + numberOfCopies.ToString() + ")";

                    // Reset the loop
                    i = 0;
                }
            }

            return filteredName;
        }
        private string FilterCheckboxName(string unfilteredName)
        {
            // Filter the name so it matches the naming conventions "name (x)"
            int numberOfCopies = 0;
            string unfilteredNameCopy = unfilteredName;
            string filteredName = unfilteredName;
            for (int i = 0; i < this.jobCheckboxes.Count; i++)
            {
                if (filteredName == this.jobCheckboxes[i].Name)
                {
                    // Increase the number of copies detected.
                    numberOfCopies++;

                    // Set the name to the format "name (x)"
                    filteredName = unfilteredNameCopy + " (" + numberOfCopies.ToString() + ")";

                    // Reset the loop
                    i = 0;
                }
            }

            return filteredName;
        }

        /// <summary>
        ///  Gets or sets customer first name
        /// </summary>
        public string CustomerFirstName
        {
            get { return this.customerFirstName; }
            set { this.customerFirstName = value; }
        }

        /// <summary>
        ///  Gets or sets customer last name
        /// </summary>
        public string CustomerLastName
        {
            get { return this.customerLastName; }
            set { this.customerLastName = value; }
        }

        /// <summary>
        ///  Gets or Sets the document path for the ticket template.
        /// </summary>
        public string DocumentPath
        {
            get { return this.documentPath; }
            set { this.documentPath = value; }
        }

        /// <summary>
        ///  Getters for the information objects in the template.
        /// </summary>
        public List<InformationTextbox> JobTextboxes
        {
            get { return jobTextboxes; }
        }
        public List<InformationCheckbox> JobCheckboxes
        {
            get { return jobCheckboxes; }
        }
        public List<InformationTextbox> JobStaticObjects
        {
            get { return this.jobStaticObjects; }
        }

        /// <summary>
        ///  Allows setting of existing textbox text.
        /// </summary>
        /// <param name="targetTextbox"></param>
        /// <param name="newText"></param>
        /// <exception cref="Exception"></exception>
        public void SetTexboxText(string targetTextbox, string newText)
        {
            // Verify the textbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchTextbox(targetTextbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Textbox Not Found in List Exception");
            }

            // If either the height or width are changing...
            if (newText != this.jobTextboxes[targetIndex].Text)
            {
                // Change
                this.jobTextboxes[targetIndex].Text = newText;
            }
        }   
        
        /// <summary>
        ///  Allows settings of whether an existing checkbox is true or false.
        /// </summary>
        /// <param name="targetCheckbox"></param>
        /// <param name="newStatus"></param>
        /// <exception cref="Exception"></exception>
        public void SetCheckboxStatus(string targetCheckbox, bool newStatus)
        {
            // Verify the textbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchCheckbox(targetCheckbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Checkbox Not Found in List Exception");
            }

            // If status is changing...
            if (newStatus != this.jobCheckboxes[targetIndex].Status)
            {
                this.jobCheckboxes[targetIndex].Status = newStatus;
            }
        }

        /// <summary>
        ///  Delete all current contents of this object
        /// </summary>
        public void Clear()
        {
            this.jobTextboxes.Clear();
            this.jobCheckboxes.Clear();
            this.documentPath = string.Empty;
        }

        /// <summary>
        ///  The base from which this job ticket is created from. Loads all information objects from the ticketBase onto this object
        /// </summary>
        /// <param name="ticketBase"></param>
        /// <returns>
        ///  False if unsuccessfully loaded.
        ///  True if successfully loaded.
        /// </returns>
        public bool LoadFromTemplate(TicketTemplate ticketBase)
        {
            // If the template's base document isn't initialized, we should not continue and return false.
            if (ticketBase.DocumentPath == null || ticketBase.DocumentPath == string.Empty)
            {
                return false;
            }
            else
            {
                this.documentPath = ticketBase.DocumentPath;
            }

            // Set this object's information object lists.
            if (ticketBase.StaticObjects.Count > 0)
            {
                this.jobStaticObjects = ticketBase.StaticObjects;
            }
            if (ticketBase.TemplateTextboxes.Count > 0)
            {
                this.jobTextboxes = ticketBase.TemplateTextboxes;
            }
            if (ticketBase.TemplateCheckboxes.Count > 0)
            {
                this.jobCheckboxes = ticketBase.TemplateCheckboxes;
            }

            // Return when done.
            return true;
        }

        /// <summary>
        ///  Saves this ticket and its contents to the fileStream in XML format.
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public bool SaveTicket(Stream fileStream)
        {
            // Settings for the XmlWriter once created
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            settings.NewLineOnAttributes = true;
            settings.Encoding = Encoding.UTF8;

            // XmlWriter used for saving the data.
            XmlWriter targetSaveFile;

            // Attempt to open the file specified by "fileStream"
            try
            {
                targetSaveFile = XmlWriter.Create(fileStream, settings);
            }
            catch
            {
                return false;
            }

            // Clear previous contents of file out
            fileStream.SetLength(0);

            // Now that the file has been opened successfully, we need to save the elements of the tickettemplate.
            // We need to save:
            //  JobTicket Name
            //  JobTicket Document Path
            //  JobTicket Information Objects:
            //      -Textboxes
            //      -Checkboxes
            // Begin XML Document writing
            targetSaveFile.WriteStartDocument();

            // JobTicket
            targetSaveFile.WriteStartElement(JobTicketXMLNames.JobTicket);

            // JobTicket Document 
            targetSaveFile.WriteStartElement(JobTicketXMLNames.JobTicketDocumentPath);
            targetSaveFile.WriteString(this.DocumentPath);
            targetSaveFile.WriteEndElement();

            // JobTicket Customer First and Last name
            targetSaveFile.WriteStartElement(JobTicketXMLNames.JobTicketFirstName);
            targetSaveFile.WriteString(this.CustomerFirstName);
            targetSaveFile.WriteEndElement();
            targetSaveFile.WriteStartElement(JobTicketXMLNames.JobTicketLastName);
            targetSaveFile.WriteString(this.CustomerLastName);
            targetSaveFile.WriteEndElement();

            // JobTicket Information Objects:

            // -Static Objects
            for (int i = 0; i < this.JobStaticObjects.Count; i++)
            {
                // Information Object title
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObject);

                // Information Object type
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectType);
                targetSaveFile.WriteString(JobTicketXMLNames.InformationStaticObject);
                targetSaveFile.WriteEndElement();

                // Textbox name
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectName);
                targetSaveFile.WriteString(this.JobStaticObjects[i].Name);
                targetSaveFile.WriteEndElement();

                // Customer Firstname/Lastname case
                if (this.JobStaticObjects[i].Name.Contains(InformationObjectConstants.CustomerFirstName) ||
                    this.JobStaticObjects[i].Name.Contains(InformationObjectConstants.CustomerLastName))
                {
                    // Textbox x
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectX);
                    targetSaveFile.WriteString(this.JobStaticObjects[i].X.ToString());
                    targetSaveFile.WriteEndElement();

                    // Textbox y
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectY);
                    targetSaveFile.WriteString(this.JobStaticObjects[i].Y.ToString());
                    targetSaveFile.WriteEndElement();

                    // Textbox fontsize
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectFontSize);
                    targetSaveFile.WriteString(this.JobStaticObjects[i].FontSize.ToString());
                    targetSaveFile.WriteEndElement();
                }
                // Counter case
                else if (this.JobStaticObjects[i].Name.Contains(InformationObjectConstants.Counter))
                {
                    // Textbox x
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectX);
                    targetSaveFile.WriteString(this.JobStaticObjects[i].X.ToString());
                    targetSaveFile.WriteEndElement();

                    // Textbox y
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectY);
                    targetSaveFile.WriteString(this.JobStaticObjects[i].Y.ToString());
                    targetSaveFile.WriteEndElement();

                    // Textbox required (representing whether to reset the counter to 0 at the beginning of a new year)
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectRequired);
                    targetSaveFile.WriteString(this.JobStaticObjects[i].Required.ToString());
                    targetSaveFile.WriteEndElement();

                    // Textbox fontsize
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectFontSize);
                    targetSaveFile.WriteString(this.JobStaticObjects[i].FontSize.ToString());
                    targetSaveFile.WriteEndElement();

                    // Textbox text
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationTextboxText);
                    targetSaveFile.WriteString(this.JobStaticObjects[i].Text.ToString());
                    targetSaveFile.WriteEndElement();
                }
                // Day/Month/Year/TimeStamp/TemplateID case
                else if (this.JobStaticObjects[i].Name.Contains(InformationObjectConstants.Day) ||
                    this.JobStaticObjects[i].Name.Contains(InformationObjectConstants.Month) ||
                    this.JobStaticObjects[i].Name.Contains(InformationObjectConstants.Year) ||
                    this.JobStaticObjects[i].Name.Contains(InformationObjectConstants.TimeStamp) ||
                    this.JobStaticObjects[i].Name.Contains(InformationObjectConstants.TemplateID))
                {
                    // Textbox x
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectX);
                    targetSaveFile.WriteString(this.JobStaticObjects[i].X.ToString());
                    targetSaveFile.WriteEndElement();

                    // Textbox y
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectY);
                    targetSaveFile.WriteString(this.JobStaticObjects[i].Y.ToString());
                    targetSaveFile.WriteEndElement();

                    // Textbox fontsize
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectFontSize);
                    targetSaveFile.WriteString(this.JobStaticObjects[i].FontSize.ToString());
                    targetSaveFile.WriteEndElement();

                    // Textbox text
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationTextboxText);
                    targetSaveFile.WriteString(this.JobStaticObjects[i].Text.ToString());
                    targetSaveFile.WriteEndElement();
                }           

                // End static object
                targetSaveFile.WriteEndElement();
            }

            // -JobTicket Textboxes
            for (int i = 0; i < this.JobTextboxes.Count; i++)
            {
                // Information Object title
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObject);

                // Information Object type
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectType);
                targetSaveFile.WriteString(JobTicketXMLNames.InformationTextbox);
                targetSaveFile.WriteEndElement();

                // Textbox Text
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationTextboxText);
                targetSaveFile.WriteString(this.JobTextboxes[i].Text);
                targetSaveFile.WriteEndElement();

                // Textbox name
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectName);
                targetSaveFile.WriteString(this.JobTextboxes[i].Name);
                targetSaveFile.WriteEndElement();

                // Textbox x
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectX);
                targetSaveFile.WriteString(this.JobTextboxes[i].X.ToString());
                targetSaveFile.WriteEndElement();

                // Textbox y
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectY);
                targetSaveFile.WriteString(this.JobTextboxes[i].Y.ToString());
                targetSaveFile.WriteEndElement();

                // Textbox required
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectRequired);
                targetSaveFile.WriteString(this.JobTextboxes[i].Required.ToString());
                targetSaveFile.WriteEndElement();

                // Textbox priority
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectPriority);
                targetSaveFile.WriteString(this.JobTextboxes[i].Priority.ToString());
                targetSaveFile.WriteEndElement();

                // Textbox fontsize
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectFontSize);
                targetSaveFile.WriteString(this.JobTextboxes[i].FontSize.ToString());
                targetSaveFile.WriteEndElement();

                // Textbox height
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectHeight);
                targetSaveFile.WriteString(this.JobTextboxes[i].Height.ToString());
                targetSaveFile.WriteEndElement();

                // Textbox width
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectWidth);
                targetSaveFile.WriteString(this.JobTextboxes[i].Width.ToString());
                targetSaveFile.WriteEndElement();

                // End textbox
                targetSaveFile.WriteEndElement();
            }

            // -JobTicket Checkboxes
            for (int i = 0; i < this.JobCheckboxes.Count; i++)
            {
                // Information Object title
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObject);

                // Information Object type
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectType);
                targetSaveFile.WriteString(JobTicketXMLNames.InformationCheckbox);
                targetSaveFile.WriteEndElement();

                // Checkbox status
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationCheckboxStatus);
                targetSaveFile.WriteString(this.JobCheckboxes[i].Status.ToString());
                targetSaveFile.WriteEndElement();

                // Checkbox name
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectName);
                targetSaveFile.WriteString(this.JobCheckboxes[i].Name);
                targetSaveFile.WriteEndElement();

                // Checkbox x
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectX);
                targetSaveFile.WriteString(this.JobCheckboxes[i].X.ToString());
                targetSaveFile.WriteEndElement();

                // Checkbox y
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectY);
                targetSaveFile.WriteString(this.JobCheckboxes[i].Y.ToString());
                targetSaveFile.WriteEndElement();

                // Checkbox required
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectRequired);
                targetSaveFile.WriteString(this.JobCheckboxes[i].Required.ToString());
                targetSaveFile.WriteEndElement();

                // Checkbox priority
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectPriority);
                targetSaveFile.WriteString(this.JobCheckboxes[i].Priority.ToString());
                targetSaveFile.WriteEndElement();

                // Checkbox scale
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectScale);
                targetSaveFile.WriteString(this.JobCheckboxes[i].Scale.ToString());
                targetSaveFile.WriteEndElement();

                // End checkbox
                targetSaveFile.WriteEndElement();
            }

            // End off template title
            targetSaveFile.WriteEndElement();

            // Flush to file
            targetSaveFile.Flush();

            // Close when done
            targetSaveFile.Close();
            return true;
        }

        /// <summary>
        ///  Loads a job ticket from the file stream into this object. Assumes same XML format as SaveTicket saved as.
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public bool LoadTicket(Stream fileStream)
        {
            // Settings for Xml Reader
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;

            // From the fileStream, attempt to open it as an XMLReader. If it can't, return false.
            XmlReader targetReadFile;
            try
            {
                targetReadFile = XmlReader.Create(fileStream, settings);
            }
            catch
            {
                return false;
            }

            // Clear the current contents of the template
            this.Clear();

            // Get first element. Set the attribute to this object's name
            // If the format isn't matching the format of jobTicket (Ticket isn't found), then we return false.
            if (targetReadFile.ReadToFollowing(JobTicketXMLNames.JobTicket) == false)
            {
                return false;
            }

            // Get next element. Set the attribute to this object's document path
            targetReadFile.ReadToFollowing(JobTicketXMLNames.JobTicketDocumentPath);
            this.DocumentPath = targetReadFile.ReadElementContentAsString();

            // Get next two elements. Set the first and last name of the ticket to being the elements read in
            this.CustomerFirstName = targetReadFile.ReadElementContentAsString();
            this.CustomerLastName = targetReadFile.ReadElementContentAsString();

            // Helper variables for storing contents into information object lists.
            InformationTextbox tempTextbox;
            InformationCheckbox tempCheckbox;

            // For each textbox and checkbox in the document, load it into this template's lists
            if (targetReadFile.LocalName == JobTicketXMLNames.InformationObject)
            {
                do
                {
                    // Get the type of the object
                    targetReadFile.ReadToFollowing(JobTicketXMLNames.InformationObjectType);
                    string objectType = targetReadFile.ReadElementContentAsString();

                    // If it's a static object, load the textbox
                    if (objectType == JobTicketXMLNames.InformationStaticObject)
                    {
                        tempTextbox = new InformationTextbox();

                        // Store the name of the current textbox.
                        tempTextbox.Name = targetReadFile.ReadElementContentAsString();

                        // Add the static object depending on the name of the object
                        // Customer first name/last name case
                        if (tempTextbox.Name.Contains(InformationObjectConstants.CustomerFirstName))
                        {
                            // X
                            tempTextbox.X = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Y
                            tempTextbox.Y = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Font Size + Dimension
                            // Change the fontsize of the target to match the new one.
                            tempTextbox.FontSize = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Adjust the dimensions of the object to match the max size of the object
                            // Customer first name and last name case
                            if (tempTextbox.Name.Contains(InformationObjectConstants.CustomerFirstName) || tempTextbox.Name.Contains(InformationObjectConstants.CustomerLastName))
                            {
                                tempTextbox.Width = tempTextbox.FontSize * InformationObjectConstants.CustomerNameMaxCharacters + 1;
                                tempTextbox.Height = tempTextbox.FontSize * 1.66;
                            }
                        }
                        else if (tempTextbox.Name.Contains(InformationObjectConstants.CustomerLastName))
                        {
                            // X
                            tempTextbox.X = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Y
                            tempTextbox.Y = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Font Size + Dimension
                            // Change the fontsize of the target to match the new one.
                            tempTextbox.FontSize = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Adjust the dimensions of the object to match the max size of the object
                            tempTextbox.Width = tempTextbox.FontSize * InformationObjectConstants.CustomerNameMaxCharacters + 1;
                            tempTextbox.Height = tempTextbox.FontSize * 1.66;
                        }
                        // Counter case
                        else if (tempTextbox.Name.Contains(InformationObjectConstants.Counter))
                        {
                            // X
                            tempTextbox.X = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Y
                            tempTextbox.Y = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Required
                            tempTextbox.Required = Convert.ToBoolean(targetReadFile.ReadElementContentAsString());

                            // Font Size + Dimension
                            // Change the fontsize of the target to match the new one.
                            tempTextbox.FontSize = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Adjust the dimensions of the object to match the max size of the object                
                            tempTextbox.Width = tempTextbox.FontSize * InformationObjectConstants.CounterMaxCharacters + 1;
                            tempTextbox.Height = tempTextbox.FontSize * 1.66;   
                            
                            // Text
                            tempTextbox.Text = targetReadFile.ReadElementContentAsString();
                        }
                        // Day case
                        else if (tempTextbox.Name.Contains(InformationObjectConstants.Day))
                        {
                            // X
                            tempTextbox.X = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Y
                            tempTextbox.Y = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Font Size + Dimension
                            // Change the fontsize of the target to match the new one.
                            tempTextbox.FontSize = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Adjust the dimensions of the object to match the max size of the object                
                            tempTextbox.Width = tempTextbox.FontSize * InformationObjectConstants.DayMaxCharacters + 1;
                            tempTextbox.Height = tempTextbox.FontSize * 1.66;

                            // Text
                            tempTextbox.Text = targetReadFile.ReadElementContentAsString();
                        }
                        // Month case
                        else if (tempTextbox.Name.Contains(InformationObjectConstants.Month))
                        {
                            // X
                            tempTextbox.X = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Y
                            tempTextbox.Y = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Font Size + Dimension
                            // Change the fontsize of the target to match the new one.
                            tempTextbox.FontSize = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Adjust the dimensions of the object to match the max size of the object                
                            tempTextbox.Width = tempTextbox.FontSize * InformationObjectConstants.MonthMaxCharacters + 1;
                            tempTextbox.Height = tempTextbox.FontSize * 1.66;

                            // Text
                            tempTextbox.Text = targetReadFile.ReadElementContentAsString();
                        }
                        // Year case
                        else if (tempTextbox.Name.Contains(InformationObjectConstants.Year))
                        {
                            // X
                            tempTextbox.X = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Y
                            tempTextbox.Y = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Font Size + Dimension
                            // Change the fontsize of the target to match the new one.
                            tempTextbox.FontSize = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Adjust the dimensions of the object to match the max size of the object                
                            tempTextbox.Width = tempTextbox.FontSize * InformationObjectConstants.YearMaxCharacters + 1;
                            tempTextbox.Height = tempTextbox.FontSize * 1.66;

                            // Text
                            tempTextbox.Text = targetReadFile.ReadElementContentAsString();
                        }
                        // Timestamp case
                        else if (tempTextbox.Name.Contains(InformationObjectConstants.TimeStamp))
                        {
                            // X
                            tempTextbox.X = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Y
                            tempTextbox.Y = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Font Size + Dimension
                            // Change the fontsize of the target to match the new one.
                            tempTextbox.FontSize = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Adjust the dimensions of the object to match the max size of the object                
                            tempTextbox.Width = tempTextbox.FontSize * InformationObjectConstants.TimeStampMaxCharacters + 1;
                            tempTextbox.Height = tempTextbox.FontSize * 1.66;

                            // Text
                            tempTextbox.Text = targetReadFile.ReadElementContentAsString();
                        }
                        // Template ID case
                        else if (tempTextbox.Name.Contains(InformationObjectConstants.TemplateID))
                        {
                            // X
                            tempTextbox.X = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Y
                            tempTextbox.Y = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Font Size + Dimension
                            // Change the fontsize of the target to match the new one.
                            tempTextbox.FontSize = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                            // Adjust the dimensions of the object to match the max size of the object                
                            tempTextbox.Width = tempTextbox.FontSize * InformationObjectConstants.TemplateIDMaxCharacters + 1;
                            tempTextbox.Height = tempTextbox.FontSize * 1.66;

                            // Text
                            tempTextbox.Text = targetReadFile.ReadElementContentAsString();
                        }


                        // Add created static object into the list
                        this.jobStaticObjects.Add(tempTextbox);
                    }

                    // If it's a textbox, load the textbox
                    if (objectType == JobTicketXMLNames.InformationTextbox)
                    {
                        tempTextbox = new InformationTextbox();

                        // Store the text of the current textbox.
                        tempTextbox.Text = targetReadFile.ReadElementContentAsString();

                        // Store the name of the current textbox.
                        tempTextbox.Name = targetReadFile.ReadElementContentAsString();

                        // X
                        tempTextbox.X = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                        // Y
                        tempTextbox.Y = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                        // Required
                        tempTextbox.Required = Convert.ToBoolean(targetReadFile.ReadElementContentAsString());

                        // Priority
                        tempTextbox.Priority = Convert.ToInt32(targetReadFile.ReadElementContentAsString());

                        // Font Size
                        tempTextbox.FontSize = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                        // Height
                        tempTextbox.Height = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                        // Width
                        tempTextbox.Width = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                        // Add created textbox into the list
                        this.jobTextboxes.Add(tempTextbox);
                    }

                    // If it's a checkbox, load the checkbox
                    else if (objectType == JobTicketXMLNames.InformationCheckbox)
                    {
                        tempCheckbox = new InformationCheckbox();

                        // Store the status of the current checkbox
                        tempCheckbox.Status = Convert.ToBoolean(targetReadFile.ReadElementContentAsString());

                        // Store the name of the current checkbox.
                        tempCheckbox.Name = targetReadFile.ReadElementContentAsString();

                        // X
                        tempCheckbox.X = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                        // Y
                        tempCheckbox.Y = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                        // Required
                        tempCheckbox.Required = Convert.ToBoolean(targetReadFile.ReadElementContentAsString());

                        // Priority
                        tempCheckbox.Priority = Convert.ToInt32(targetReadFile.ReadElementContentAsString());

                        // Scale
                        tempCheckbox.Scale = Convert.ToDouble(targetReadFile.ReadElementContentAsString());

                        // Add created checkbox into the list
                        this.jobCheckboxes.Add(tempCheckbox);
                    }

                } while (targetReadFile.ReadToFollowing(JobTicketXMLNames.InformationObject) == true);
            }

            // Close and return once completed.
            targetReadFile.Close();
            return true;
        }
        
    }
}