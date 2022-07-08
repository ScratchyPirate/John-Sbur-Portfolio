/// <summary>
///  TicketTemplate.cs
///  Job Ticket Manager Project
///  Creator: John D. Sbur
/// </summary>
namespace JobTicketEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    public class TicketTemplate
    {
        /// <summary>
        ///  Storage unit for all objects in a template.
        /// </summary>
        private List<InformationTextbox> templateTextboxes;
        private List<InformationCheckbox> templateCheckboxes;
        private List<InformationTextbox> staticObjects;

        /// <summary>
        ///  Name of template.
        /// </summary>
        private string templateName;

        /// <summary>
        ///  Path of the document the template is based on.
        /// </summary>
        private string documentPath;

        /// <summary>
        ///  Event handlers responsible for responding to when a property of an information object is changed.
        /// </summary>
        public event PropertyChangedEventHandler TextboxPropertyChanged = delegate { };
        public event PropertyChangedEventHandler CheckboxPropertyChanged = delegate { };
        public event PropertyChangedEventHandler StaticObjectPropertyChanged = delegate { };
        public event PropertyChangedEventHandler TemplatePropertyChanged = delegate { };

        /// <summary>
        ///  Constructor for the ticket template. Must have a document to base itself off of.
        /// </summary>
        /// <param name="newDocumentPath"></param>
        public TicketTemplate(string newDocumentPath)
        {
            // Initialize template lists as empty
            this.templateTextboxes = new List<InformationTextbox>();
            this.templateTextboxes.Clear();
            this.templateCheckboxes = new List<InformationCheckbox>();
            this.templateCheckboxes.Clear();
            this.staticObjects = new List<InformationTextbox>();
            this.staticObjects.Clear();


            // Initialize template name and document path.
            this.documentPath = newDocumentPath;
            this.templateName = string.Empty;
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
            for (int i = 0; i < this.TemplateTextboxes.Count; i++)
            {
                if (this.TemplateTextboxes[i].Name == targetTextbox)
                {
                    targetIndex = i;
                }
            }
            return targetIndex;
        }
        public int SearchCheckbox(string targetCheckbox)
        {
            int targetIndex = -1;
            for (int i = 0; i < this.TemplateCheckboxes.Count; i++)
            {
                if (this.TemplateCheckboxes[i].Name == targetCheckbox)
                {
                    targetIndex = i;
                }
            }
            return targetIndex;
        }
        public int SearchStaticObject(string targetObject)
        {
            int targetIndex = -1;
            for (int i = 0; i < this.staticObjects.Count; i++)
            {
                if (this.staticObjects[i].Name == targetObject)
                {
                    targetIndex = i;
                }
            }
            return targetIndex;
        }

        /// <summary>
        ///  Soring methods pertaining to each information object list. Sorts from low to high based on object priority.
        /// </summary>
        public void SortTextboxes()
        {
            // Quick Sort Implementation
            this.SortTextboxesHelper(0, this.TemplateTextboxes.Count - 1);
            //this.TemplatePropertyChanged.Invoke(this, new PropertyChangedEventArgs("textboxes sorted"));
        }
        private void SortTextboxesHelper(int low, int high)
        {
            if (low < high)
            {
                // Get the index where the two pieces of the array split based on the array index
                int splitIndex = this.SortTextboxesPartitonMaker(low, high);

                // Sort the lower half of the split.
                this.SortTextboxesHelper(low, splitIndex - 1);

                // Sort the upper half of the split.
                this.SortTextboxesHelper(splitIndex + 1, high);
            }
        }
        private int SortTextboxesPartitonMaker(int low, int high)
        {
            // Rightmost value = pivot value.
            int pivot = this.templateTextboxes[high].Priority;
            int smallestElement = (low - 1);
            InformationTextbox temporaryTextbox;

            // Loop through all members from low to high - 1 (to exclude the pivot)
            for (int j = low; j < high; j++)
            {
                // If we find a number that is less than or equal to the pivot, increment the number of small elements and then swap the current position
                //  and the smallest element index.
                if (this.templateTextboxes[j].SafePriority < pivot)
                {
                    smallestElement++;
                    temporaryTextbox = this.templateTextboxes[j];
                    this.templateTextboxes[j] = this.templateTextboxes[smallestElement];
                    this.templateTextboxes[smallestElement] = temporaryTextbox;
                }
            }

            // Swap the pivot and the smallest index.
            temporaryTextbox = this.templateTextboxes[high];
            this.templateTextboxes[high] = this.templateTextboxes[smallestElement + 1];
            this.templateTextboxes[smallestElement + 1] = temporaryTextbox;

            // Return the middle index when done.
            return smallestElement + 1;
        }
        public void SortCheckboxes()
        {
            // Quicksort Implementation
            this.SortCheckboxesHelper(0, this.TemplateCheckboxes.Count - 1);
            //this.TemplatePropertyChanged.Invoke(this, new PropertyChangedEventArgs("checkboxes sorted"));
        }
        private void SortCheckboxesHelper(int low, int high)
        {
            if (low < high)
            {
                // Get the index where the two pieces of the array split based on the array index
                int splitIndex = this.SortCheckboxesPartitionMaker(low, high);

                // Sort the lower half of the split.
                this.SortCheckboxesHelper(low, splitIndex - 1);

                // Sort the upper half of the split.
                this.SortCheckboxesHelper(splitIndex + 1, high);
            }
        }
        private int SortCheckboxesPartitionMaker(int low, int high)
        {
            // Rightmost value = pivot value.
            int pivot = this.templateCheckboxes[high].Priority;
            int smallestElement = (low - 1);
            InformationCheckbox temporaryCheckbox;

            // Loop through all members from low to high - 1 (to exclude the pivot)
            for (int j = low; j < high; j++)
            {
                // If we find a number that is less than or equal to the pivot, increment the number of small elements and then swap the current position
                //  and the smallest element index.
                if (this.templateCheckboxes[j].SafePriority < pivot)
                {
                    smallestElement++;
                    temporaryCheckbox = this.templateCheckboxes[j];
                    this.templateCheckboxes[j] = this.templateCheckboxes[smallestElement];
                    this.templateCheckboxes[smallestElement] = temporaryCheckbox;
                }
            }

            // Swap the pivot and the smallest index.
            temporaryCheckbox = this.templateCheckboxes[high];
            this.templateCheckboxes[high] = this.templateCheckboxes[smallestElement + 1];
            this.templateCheckboxes[smallestElement + 1] = temporaryCheckbox;

            // Return the middle index when done.
            return smallestElement + 1;
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
            for (int i = 0; i < this.TemplateTextboxes.Count; i++)
            {
                if (filteredName == this.TemplateTextboxes[i].Name)
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
            for (int i = 0; i < this.TemplateCheckboxes.Count; i++)
            {
                if (filteredName == this.TemplateCheckboxes[i].Name)
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
        private string FilterStaticObjectName(string unfilteredName)
        {
            // Filter the name so it matches the naming conventions "name (x)"
            int numberOfCopies = 0;
            string unfilteredNameCopy = unfilteredName;
            string filteredName = unfilteredName;
            for (int i = 0; i < this.staticObjects.Count; i++)
            {
                if (filteredName == this.staticObjects[i].Name)
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
        ///  Gets or Sets the document path for the ticket template.
        /// </summary>
        public string DocumentPath
        {
            get { return this.documentPath; }
            set
            {
                this.NotifyTemplatePropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
                this.documentPath = value;
            }
        }

        /// <summary>
        ///  Gets or Sets the document path for the ticket template.
        /// </summary>
        public string Name
        {
            get { return this.templateName; }
            set {
                if (this.templateName != value)
                {
                    this.templateName = value;
                    this.NotifyTemplatePropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
                }
            }
        }

        /// <summary>
        ///  Getters for the information objects in the template.
        /// </summary>
        public List<InformationTextbox> TemplateTextboxes
        {
            get { return templateTextboxes; }
        }
        public List<InformationCheckbox> TemplateCheckboxes
        {
            get { return templateCheckboxes; }
        }
        public List<InformationTextbox> StaticObjects
        {
            get { return this.staticObjects; }
        }

        /// <summary>
        ///  Instead of setters, these functions are designed to add, modify, and remove elements from each object list.
        ///  Add Condition: If the textbox is to be added and the name exists, the textbox is added with the postfix (x) where x is the number of other textboxes with the same name that
        ///   already exist.
        ///  Modify Condition: Object is modified only if the name exists in its corresponding list and the new value is different than the old value.
        ///  Remove Condition: Object is removed only if the name exists in its corresponding list.
        /// </summary>
        public void AddTextbox(string newTextboxName)
        {
            // Initialize new textbox
            InformationTextbox newTextbox = new InformationTextbox();

            // Verify the name doesn't exist already in the list. If it does, modify the name of the entered textbox and reset the loop.
            // This ensures that it's eventually added.
            newTextbox.Name = this.FilterTextboxName(newTextboxName);

            // Subscribe to the new checkbox's propertychanged event handler
            newTextbox.PropertyChanged += this.NotifyTextboxChanged;

            // Set the priority to be the lowest of all current information objects in the template
            if (this.templateTextboxes.Count == 0 && this.templateCheckboxes.Count == 0)
            {
                newTextbox.SafePriority = 0;
            }
            else if (this.templateTextboxes.Count == 0)
            {
                newTextbox.SafePriority = this.templateCheckboxes[this.templateCheckboxes.Count - 1].Priority + 1;
            }
            else if (this.templateCheckboxes.Count == 0)
            {
                newTextbox.SafePriority = this.templateTextboxes[this.templateTextboxes.Count - 1].Priority + 1;
            }
            else
            {
                if (this.templateTextboxes[this.templateTextboxes.Count - 1].Priority > this.templateCheckboxes[this.templateCheckboxes.Count - 1].Priority)
                {
                    newTextbox.SafePriority = this.templateTextboxes[this.templateTextboxes.Count - 1].Priority + 1;
                }
                else
                {
                    newTextbox.SafePriority = this.templateCheckboxes[this.templateCheckboxes.Count - 1].Priority + 1;
                }

            }

            // Now when we exit the loop, the textbox we have should be unique and we can add it to the list.
            this.templateTextboxes.Add(newTextbox);

            // Notify that a template property was changed
            this.NotifyTemplatePropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }
        public void AddCheckBox(string newCheckboxName)
        {
            // Initialize new checkbox
            InformationCheckbox newCheckbox = new InformationCheckbox();

            // Verify the name doesn't exist already in the list. If it does, modify the name of the entered textbox and reset the loop.
            // This ensures that it's eventually added.
            newCheckbox.Name = this.FilterCheckboxName(newCheckboxName);

            // Subscribe to the new checkbox's propertychanged event handler
            newCheckbox.PropertyChanged += this.NotifyCheckboxChanged;

            // Set the priority to be the lowest of all current information objects in the template
            // Set the priority to be the lowest of all current information objects in the template
            if (this.templateTextboxes.Count == 0 && this.templateCheckboxes.Count == 0)
            {
                newCheckbox.SafePriority = 0;
            }
            else if (this.templateTextboxes.Count == 0)
            {
                newCheckbox.SafePriority = this.templateCheckboxes[this.templateCheckboxes.Count - 1].Priority + 1;
            }
            else if (this.templateCheckboxes.Count == 0)
            {
                newCheckbox.SafePriority = this.templateTextboxes[this.templateTextboxes.Count - 1].Priority + 1;
            }
            else
            {
                if (this.templateTextboxes[this.templateTextboxes.Count - 1].Priority > this.templateCheckboxes[this.templateCheckboxes.Count - 1].Priority)
                {
                    newCheckbox.SafePriority = this.templateTextboxes[this.templateTextboxes.Count - 1].Priority + 1;
                }
                else
                {
                    newCheckbox.SafePriority = this.templateCheckboxes[this.templateCheckboxes.Count - 1].Priority + 1;
                }

            }

            // Now when we exit the loop, the textbox we have should be unique and we can add it to the list.
            this.templateCheckboxes.Add(newCheckbox);

            // Notify that a template property was changed
            this.NotifyTemplatePropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }

        /// <summary>
        ///  Specifically adds a type of static object to the staticObject list. Type indicates the type of object to be added
        ///      <para>type = 0 (default) -> customer first name</para>
        ///      <para>type = 1 -> customer last name</para>
        ///      <para>type = 2 -> counter</para>
        ///      <para>type = 3 -> day</para>
        ///      <para>type = 4 -> month</para>
        ///      <para>type = 5 -> year</para>
        ///      <para>type = 6 -> time stamp</para>
        ///      <para>type = 7 -> template ID</para>
        /// Returns the name of the recently created object.
        /// </summary>
        /// <param name="type"></param>
        public string AddStaticObject(int type)
        {
            InformationTextbox newObject;
            newObject = new InformationTextbox();

            // Customer first name case
            switch (type)
            {
                // Customer first name case
                case 0:
                    // Add a blank textbox with customer first name and then notify that the template has changed.
                    newObject.Name = this.FilterStaticObjectName(InformationObjectConstants.CustomerFirstName);
                    newObject.Width = newObject.FontSize * InformationObjectConstants.CustomerNameMaxCharacters + 1;
                    newObject.Height = newObject.FontSize * 1.66;
                    newObject.PropertyChanged += this.NotifyStaticObjectChanged;
                    this.staticObjects.Add(newObject);
                    this.NotifyTemplatePropertyChanged(this, new PropertyChangedEventArgs(string.Empty));

                    return newObject.Name;

                // Customer last name case
                case 1:
                    // Add a blank textbox with customer first name and then notify that the template has changed.
                    newObject.Name = this.FilterStaticObjectName(InformationObjectConstants.CustomerLastName);
                    newObject.Width = newObject.FontSize * InformationObjectConstants.CustomerNameMaxCharacters + 1;
                    newObject.Height = newObject.FontSize * 1.66;
                    newObject.PropertyChanged += this.NotifyStaticObjectChanged;
                    this.staticObjects.Add(newObject);
                    this.NotifyTemplatePropertyChanged(this, new PropertyChangedEventArgs(string.Empty));

                    return newObject.Name;

                // Counter case
                case 2:
                    // Add a blank textbox with a counter number starting at 0. Then notify that the template has changed
                    newObject.Name = this.FilterStaticObjectName(InformationObjectConstants.Counter);
                    newObject.Width = newObject.FontSize * InformationObjectConstants.CounterMaxCharacters + 1;
                    newObject.Height = newObject.FontSize * 1.66;
                    newObject.Text = "0";
                    newObject.PropertyChanged += this.NotifyStaticObjectChanged;
                    this.staticObjects.Add(newObject);
                    this.NotifyTemplatePropertyChanged(this, new PropertyChangedEventArgs(string.Empty));

                    return newObject.Name;

                // Day case
                case 3:
                    // Add a blank textbox with "Day" in it's name. Then notify that the template has changed
                    newObject.Name = this.FilterStaticObjectName(InformationObjectConstants.Day);
                    newObject.Width = newObject.FontSize * InformationObjectConstants.DayMaxCharacters + 1;
                    newObject.Height = newObject.FontSize * 1.66;
                    newObject.PropertyChanged += this.NotifyStaticObjectChanged;
                    this.staticObjects.Add(newObject);
                    this.NotifyTemplatePropertyChanged(this, new PropertyChangedEventArgs(string.Empty));

                    return newObject.Name;

                // Month case
                case 4:
                    // Add a blank textbox with "Month" in its name. Then notify that the template has changed
                    newObject.Name = this.FilterStaticObjectName(InformationObjectConstants.Month);
                    newObject.Width = newObject.FontSize * InformationObjectConstants.MonthMaxCharacters + 1;
                    newObject.Height = newObject.FontSize * 1.66;
                    newObject.PropertyChanged += this.NotifyStaticObjectChanged;
                    this.staticObjects.Add(newObject);
                    this.NotifyTemplatePropertyChanged(this, new PropertyChangedEventArgs(string.Empty));

                    return newObject.Name;

                // Year case
                case 5:
                    // Add a blank textbox with "Year" in its name. Then notify that the template has changed
                    newObject.Name = this.FilterStaticObjectName(InformationObjectConstants.Year);
                    newObject.Width = newObject.FontSize * InformationObjectConstants.YearMaxCharacters + 1;
                    newObject.Height = newObject.FontSize * 1.66;
                    newObject.PropertyChanged += this.NotifyStaticObjectChanged;
                    this.staticObjects.Add(newObject);
                    this.NotifyTemplatePropertyChanged(this, new PropertyChangedEventArgs(string.Empty));

                    return newObject.Name;

                // Timestamp Case
                case 6:
                    // Add a blank textbox with "Time Stamp" in its name. Then notify that the template has changed.
                    newObject.Name = this.FilterStaticObjectName(InformationObjectConstants.TimeStamp);
                    newObject.Width = newObject.FontSize * InformationObjectConstants.TimeStampMaxCharacters + 1;
                    newObject.Height = newObject.FontSize * 1.66;
                    newObject.PropertyChanged += this.NotifyStaticObjectChanged;
                    this.staticObjects.Add(newObject);
                    this.NotifyTemplatePropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
                    return newObject.Name;

                // Template ID Case
                case 7:
                    // Add a blank textbox with "Template ID" in its name. Then notify that the template has changed.
                    newObject.Name = this.FilterStaticObjectName(InformationObjectConstants.TemplateID);
                    newObject.Width = newObject.FontSize * InformationObjectConstants.TemplateIDMaxCharacters + 1;
                    newObject.Height = newObject.FontSize * 1.66;
                    newObject.PropertyChanged += this.NotifyStaticObjectChanged;
                    this.staticObjects.Add(newObject);
                    this.NotifyTemplatePropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
                    return newObject.Name;

                // Default case (Add nothing)
                default:
                    return string.Empty;
            }
        }

        // This function is an exception to the normal format as it also returns the filtered name after completion.
        public string SetTextboxName(string targetTextbox, string newName)
        {
            // Verify the textbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchTextbox(targetTextbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Textbox Not Found in List Exception");
            }

            // If the name is changing, we make sure the new name isn't repeating any existing names. If it is, add it with the same naming conventions.
            if (newName != this.templateTextboxes[targetIndex].Name)
            {
                // Change the name of the target to match the filtered one.
                this.templateTextboxes[targetIndex].Name = this.FilterTextboxName(newName);
            }

            return this.templateTextboxes[targetIndex].Name;
        }
        public void SetTextboxX(string targetTextbox, double newX)
        {
            // Verify the textbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchTextbox(targetTextbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Textbox Not Found in List Exception");
            }

            // If x is changing...
            if (newX != this.templateTextboxes[targetIndex].X)
            {
                // Change the x of the target to match the new one.
                this.templateTextboxes[targetIndex].X = newX;
            }
        }
        public void SetTextboxY(string targetTextbox, double newY)
        {
            // Verify the textbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchTextbox(targetTextbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Textbox Not Found in List Exception");
            }

            // If y is changing...
            if (newY != this.templateTextboxes[targetIndex].Y)
            {
                // Change the y of the target to match the new one.
                this.templateTextboxes[targetIndex].Y = newY;
            }
        }
        public void SetTextboxRequirement(string targetTextbox, bool newRequirement)
        {
            // Verify the textbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchTextbox(targetTextbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Textbox Not Found in List Exception");
            }

            // If "Required" is changing...
            if (newRequirement != this.templateTextboxes[targetIndex].Required)
            {
                // Change the requirement of the target to match the new one.
                this.templateTextboxes[targetIndex].Required = newRequirement;
            }
        }
        public void SetTextboxPriority(string targetTextbox, int newPriority, bool sort = true)
        {
            // Verify the textbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchTextbox(targetTextbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Textbox Not Found in List Exception");
            }

            // If "Priority" is changing...
            if (newPriority != this.templateTextboxes[targetIndex].SafePriority)
            {
                // Save the old priority to a holding variable
                int oldPriority = this.templateTextboxes[targetIndex].SafePriority;

                // Change the priority of the target to match the new one.
                this.templateTextboxes[targetIndex].Priority = newPriority;

                // See if there are any repeats in priority between the information object lists. If this is the case, then set the other object's
                //  priority to be the old priority of the recently edited object.
                bool found = false;
                bool sorting = true;
                bool priorityAdjusted = false;
                string changingObjectLocation = "Textboxes";
                while (sorting)
                {
                    sorting = false;
                    found = false;
                    for (int i = 0; i < this.templateTextboxes.Count && !found; i++)
                    {
                        if (this.templateTextboxes[i].SafePriority == newPriority)
                        {
                            if (changingObjectLocation == "Textboxes")
                            {
                                if (i != targetIndex)
                                {
                                    found = true;
                                    sorting = true;
                                    priorityAdjusted = true;
                                    targetIndex = i;
                                    this.templateTextboxes[i].SafePriority = ++newPriority;
                                }
                            }
                            else
                            {
                                found = true;
                                sorting = true;
                                priorityAdjusted = true;
                                targetIndex = i;
                                changingObjectLocation = "Textboxes";
                                this.templateTextboxes[i].SafePriority = ++newPriority;
                            }
                        }
                    }
                    if (!found)
                    {
                        for (int i = 0; i < this.templateCheckboxes.Count && !found; i++)
                        {
                            if (this.templateCheckboxes[i].SafePriority == newPriority)
                            {
                                if (changingObjectLocation == "Checkboxes")
                                {
                                    if (i != targetIndex)
                                    {
                                        found = true;
                                        sorting = true;
                                        priorityAdjusted = true;
                                        targetIndex = i;
                                        this.templateCheckboxes[i].SafePriority = ++newPriority;
                                    }
                                }
                                else
                                {
                                    found = true;
                                    sorting = true;
                                    priorityAdjusted = true;
                                    targetIndex = i;
                                    changingObjectLocation = "Checkboxes";
                                    this.templateCheckboxes[i].SafePriority = ++newPriority;
                                }
                            }
                        }
                    }
                }

                if (priorityAdjusted)
                {
                    this.NotifyStaticObjectChanged(this, new PropertyChangedEventArgs(""));
                }

                // If sort = true, sort the textboxes by priority
                if (sort == true)
                {
                    this.SortTextboxes();
                }
            }
        }
        public void SetTextboxFontSize(string targetTextbox, double newFontSize)
        {
            // Verify the textbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchTextbox(targetTextbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Textbox Not Found in List Exception");
            }

            // If size is changing...
            if (newFontSize != this.templateTextboxes[targetIndex].FontSize)
            {
                // Change the size of the target to match the new one.
                this.templateTextboxes[targetIndex].FontSize = newFontSize;
            }
        }
        public void SetTextboxDimensions(string targetTextbox, double newWidth, double newHeight)
        {
            // Verify the textbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchTextbox(targetTextbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Textbox Not Found in List Exception");
            }

            // If either the height or width are changing...
            if (newWidth != this.templateTextboxes[targetIndex].Width || newHeight != this.templateTextboxes[targetIndex].Height)
            {
                // Change
                this.templateTextboxes[targetIndex].Height = newHeight;
                this.templateTextboxes[targetIndex].Width = newWidth;
            }
        }
        public void SetTexboxText(string targetTextbox, string newText)
        {
            // Verify the textbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchTextbox(targetTextbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Textbox Not Found in List Exception");
            }

            // If either the height or width are changing...
            if (newText != this.templateTextboxes[targetIndex].Text)
            {
                // Change
                this.templateTextboxes[targetIndex].Text = newText;
            }
        }
        // This function is an exception to the normal format as it also returns the filtered name after completion.
        public string SetCheckboxName(string targetCheckbox, string newName)
        {
            // Verify the checkbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchCheckbox(targetCheckbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Checkbox Not Found in List Exception");
            }

            // If the name is changing, we make sure the new name isn't repeating any existing names. If it is, add it with the same naming conventions.
            if (newName != this.templateCheckboxes[targetIndex].Name)
            {
                // Change the name of the target to match the filtered one.
                this.templateCheckboxes[targetIndex].Name = this.FilterCheckboxName(newName);
            }

            return this.templateCheckboxes[targetIndex].Name;
        }
        public void SetCheckboxX(string targetCheckbox, double newX)
        {
            // Verify the checkbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchCheckbox(targetCheckbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Checkbox Not Found in List Exception");
            }

            // If x is changing...
            if (newX != this.templateCheckboxes[targetIndex].X)
            {
                // Change the x of the target to match the new one.
                this.templateCheckboxes[targetIndex].X = newX;
            }
        }
        public void SetCheckboxY(string targetCheckbox, double newY)
        {
            // Verify the checkbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchCheckbox(targetCheckbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Checkbox Not Found in List Exception");
            }

            // If y is changing...
            if (newY != this.templateCheckboxes[targetIndex].Y)
            {
                // Change the y of the target to match the new one.
                this.templateCheckboxes[targetIndex].Y = newY;
            }
        }
        public void SetCheckboxRequirement(string targetCheckbox, bool newRequirement)
        {
            // Verify the checkbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchCheckbox(targetCheckbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Checkbox Not Found in List Exception");
            }

            // If "Required" is changing...
            if (newRequirement != this.templateCheckboxes[targetIndex].Required)
            {
                // Change the "Required" field of the target to match the new one.
                this.templateCheckboxes[targetIndex].Required = newRequirement;
            }
        }
        public void SetCheckboxPriority(string targetCheckbox, int newPriority, bool sort = true)
        {
            // Verify the checkbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchCheckbox(targetCheckbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Textbox Not Found in List Exception");
            }

            // If "Priority" is changing...
            if (newPriority != this.templateCheckboxes[targetIndex].Priority)
            {
                // Change the priority of the target to match the new one.
                this.templateCheckboxes[targetIndex].Priority = newPriority;

                // See if there are any repeats in priority between the information object lists. If this is the case, then set the other object's
                //  priority to be the old priority of the recently edited object.
                bool found = false;
                bool sorting = true;
                bool priorityAdjusted = false;
                string changingObjectLocation = "Checkboxes";
                while (sorting)
                {
                    sorting = false;
                    found = false;
                    for (int i = 0; i < this.templateCheckboxes.Count && !found; i++)
                    {
                        if (this.templateCheckboxes[i].SafePriority == newPriority)
                        {
                            if (changingObjectLocation == "Checkboxes")
                            {
                                if (i != targetIndex)
                                {
                                    found = true;
                                    sorting = true;
                                    priorityAdjusted = true;
                                    targetIndex = i;
                                    this.templateCheckboxes[i].SafePriority = ++newPriority;
                                }                         
                            }
                            else
                            {
                                found = true;
                                sorting = true;
                                priorityAdjusted = true;
                                targetIndex = i;
                                changingObjectLocation = "Checkboxes";
                                this.templateCheckboxes[i].SafePriority = ++newPriority;
                            }
                        }
                    }
                    if (!found)
                    {
                        for (int i = 0; i < this.templateTextboxes.Count && !found; i++)
                        {
                            if (this.templateTextboxes[i].SafePriority == newPriority)
                            {
                                if (changingObjectLocation == "Textboxes")
                                {
                                    if (i != targetIndex)
                                    {
                                        found = true;
                                        sorting = true;
                                        priorityAdjusted = true;
                                        targetIndex = i;
                                        this.templateTextboxes[i].SafePriority = ++newPriority;
                                    }
                                }
                                else
                                {
                                    found = true;
                                    sorting = true;
                                    priorityAdjusted = true;
                                    targetIndex = i;
                                    changingObjectLocation = "Textboxes";
                                    this.templateTextboxes[i].SafePriority = ++newPriority;
                                }
                            }
                        }
                    }
                }      
                
                if (priorityAdjusted)
                {
                    this.NotifyStaticObjectChanged(this, new PropertyChangedEventArgs("")); 
                }

                // If sort = true, sort the checkboxes based on priority
                if (sort == true)
                {
                    this.SortCheckboxes();
                }
            }
        }
        public void SetCheckboxScale(string targetCheckbox, double newScale)
        {
            // Verify the checkbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchCheckbox(targetCheckbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Checkbox Not Found in List Exception");
            }

            // If scale is changing...
            if (newScale != this.templateCheckboxes[targetIndex].Scale)
            {
                // Change the y of the target to match the new one.
                this.templateCheckboxes[targetIndex].Scale = newScale;
            }
        }
        public void SetCheckboxStatus(string targetCheckbox, bool newStatus)
        {
            // Verify the checkbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchCheckbox(targetCheckbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Checkbox Not Found in List Exception");
            }

            // If status is changing...
            if (newStatus != this.templateCheckboxes[targetIndex].Status)
            {
                this.templateCheckboxes[targetIndex].Status = newStatus;
            }
        }
        public void SetStaticObjectX(string targetObject, double newX)
        {
            // Verify the static object exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchStaticObject(targetObject);
            if (targetIndex == -1)
            {
                throw new Exception("Target Static Object Not Found in List Exception");
            }

            // If x is changing...
            if (newX != this.staticObjects[targetIndex].X)
            {
                // Change the x of the target to match the new one.
                this.staticObjects[targetIndex].X = newX;
            }
        }
        public void SetStaticObjectY(string targetObject, double newY)
        {
            // Verify the static object exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchStaticObject(targetObject);
            if (targetIndex == -1)
            {
                throw new Exception("Target Static Object Not Found in List Exception");
            }

            // If y is changing...
            if (newY != this.staticObjects[targetIndex].Y)
            {
                // Change the y of the target to match the new one.
                this.staticObjects[targetIndex].Y = newY;
            }
        }
        public void SetStaticObjectRequirement(string targetObject, bool newRequirement)
        {
            // Verify the textbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchStaticObject(targetObject);
            if (targetIndex == -1)
            {
                throw new Exception("Target Textbox Not Found in List Exception");
            }

            // If "Required" is changing...
            if (newRequirement != this.staticObjects[targetIndex].Required)
            {
                // Change the requirement of the target to match the new one.
                this.staticObjects[targetIndex].Required = newRequirement;
            }
        }
        public void SetStaticObjectFontsize(string targetObject, double newFontSize)
        {
            // Verify the static object exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchStaticObject(targetObject);
            if (targetIndex == -1)
            {
                throw new Exception("Target Static Object Not Found in List Exception");
            }

            // If fontsize is changing...
            if (newFontSize != this.staticObjects[targetIndex].FontSize)
            {
                // Change the fontsize of the target to match the new one.
                this.staticObjects[targetIndex].FontSize = newFontSize;

                // Adjust the dimensions of the object to match the max size of the object
                // Customer first name and last name case
                if (this.staticObjects[targetIndex].Name.Contains(InformationObjectConstants.CustomerFirstName) || this.staticObjects[targetIndex].Name.Contains(InformationObjectConstants.CustomerLastName))
                {
                    this.staticObjects[targetIndex].Width = newFontSize * InformationObjectConstants.CustomerNameMaxCharacters + 1;
                    this.staticObjects[targetIndex].Height = newFontSize * 1.66;
                }
                // Counter case
                else if (this.staticObjects[targetIndex].Name.Contains(InformationObjectConstants.Counter))
                {
                    this.staticObjects[targetIndex].Width = newFontSize * InformationObjectConstants.CounterMaxCharacters + 1;
                    this.staticObjects[targetIndex].Height = newFontSize * 1.66;
                }
                // Day case
                else if (this.staticObjects[targetIndex].Name.Contains(InformationObjectConstants.Day))
                {
                    this.staticObjects[targetIndex].Width = newFontSize * InformationObjectConstants.DayMaxCharacters + 1;
                    this.staticObjects[targetIndex].Height = newFontSize * 1.66;
                }
                // Month case
                else if (this.staticObjects[targetIndex].Name.Contains(InformationObjectConstants.Month))
                {
                    this.staticObjects[targetIndex].Width = newFontSize * InformationObjectConstants.MonthMaxCharacters + 1;
                    this.staticObjects[targetIndex].Height = newFontSize * 1.66;
                }
                // Year case
                else if (this.staticObjects[targetIndex].Name.Contains(InformationObjectConstants.Year))
                {
                    this.staticObjects[targetIndex].Width = newFontSize * InformationObjectConstants.YearMaxCharacters + 1;
                    this.staticObjects[targetIndex].Height = newFontSize * 1.66;
                }
                // Timestamp case
                else if (this.staticObjects[targetIndex].Name.Contains(InformationObjectConstants.TimeStamp))
                {
                    this.staticObjects[targetIndex].Width = newFontSize * InformationObjectConstants.TimeStampMaxCharacters + 1;
                    this.staticObjects[targetIndex].Height = newFontSize * 1.66;
                }
                // Template ID case
                else if (this.staticObjects[targetIndex].Name.Contains(InformationObjectConstants.TemplateID))
                {
                    this.staticObjects[targetIndex].Width = newFontSize * InformationObjectConstants.TemplateIDMaxCharacters + 1;
                    this.staticObjects[targetIndex].Height = newFontSize * 1.66;
                }
            }
        }
        public void SetStaticObjectText(string targetObject, string newText)
        {
            // Verify the static object exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchStaticObject(targetObject);
            if (targetIndex == -1)
            {
                throw new Exception("Target Static Object Not Found in List Exception");
            }

            // If text is changing...
            if (newText != this.staticObjects[targetIndex].Text)
            {
                // Change the fontsize of the target to match the new one.
                this.staticObjects[targetIndex].Text = newText;
            }
        }
        public void RemoveTextbox(string targetTextbox)
        {
            // Verify the checkbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchTextbox(targetTextbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Textbox Not Found in List Exception");
            }

            // Unsubscribe from textbox changes
            this.templateTextboxes[targetIndex].PropertyChanged -= this.TextboxPropertyChanged;

            // Remove textbox
            this.templateTextboxes.RemoveAt(targetIndex);

            // Notify that a template property was changed
            this.NotifyTemplatePropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }
        public void RemoveCheckbox(string targetCheckbox)
        {
            // Verify the checkbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchCheckbox(targetCheckbox);
            if (targetIndex == -1)
            {
                throw new Exception("Target Checkbox Not Found in List Exception");
            }

            // Unsubscribe from textbox changes
            this.templateCheckboxes[targetIndex].PropertyChanged -= this.CheckboxPropertyChanged;

            // Remove checkbox
            this.templateCheckboxes.RemoveAt(targetIndex);

            // Notify that a template property was changed
            this.NotifyTemplatePropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }
        public void RemoveStaticObject(string targetObject)
        {
            // Verify the checkbox exists in the list. If it doesn't, throw an exception.
            int targetIndex = this.SearchStaticObject(targetObject);
            if (targetIndex == -1)
            {
                throw new Exception("Target Static Object Not Found in List Exception");
            }

            // Unsubscribe from textbox changes
            this.staticObjects[targetIndex].PropertyChanged -= this.StaticObjectPropertyChanged;

            // Remove checkbox
            this.staticObjects.RemoveAt(targetIndex);

            // Notify that a template property was changed
            this.NotifyTemplatePropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }
        /// <summary>
        ///  This function resets every counter in the ticket template if they exist back down to 0 and they have required set to true.
        /// </summary>
        public void ResetCounters()
        {
            for (int i = 0; i < this.staticObjects.Count; i++)
            {
                if (this.staticObjects[i].Name.Contains(InformationObjectConstants.Counter) && this.staticObjects[i].Required == true)
                {
                    this.staticObjects[i].Text = "0";
                }
            }
        }
        
        /// <summary>
        ///  Based on a file path, this functions saves the contents of the template to that file if able.
        /// </summary>
        /// <param name="directoryPath">
        ///     Path of folder to save to.
        /// </param>
        /// <returns>
        ///     True if the file was successfully saved
        ///     False otherwise.
        /// </returns>
        public bool SaveTemplate(Stream fileStream)
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
            //  Template Name
            //  Template Document Path
            //  Template Information Objects:
            //      -Textboxes
            //      -Checkboxes

            // Begin XML Document writing
            targetSaveFile.WriteStartDocument();

            // Template
            targetSaveFile.WriteStartElement(JobTicketXMLNames.Template);

            // Template Name
            targetSaveFile.WriteStartElement(JobTicketXMLNames.TemplateName);
            targetSaveFile.WriteString(this.Name);
            targetSaveFile.WriteEndElement();

            // Template Document 
            targetSaveFile.WriteStartElement(JobTicketXMLNames.TemplateDocumentPath);
            targetSaveFile.WriteString(this.DocumentPath);
            targetSaveFile.WriteEndElement();

            // Template Information Objects:
            // -Static Objects
            for (int i = 0; i < this.StaticObjects.Count; i++)
            {
                // Information Object title
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObject);

                // Information Object type
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectType);
                targetSaveFile.WriteString(JobTicketXMLNames.InformationStaticObject);
                targetSaveFile.WriteEndElement();

                // Textbox name
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectName);
                targetSaveFile.WriteString(this.StaticObjects[i].Name);
                targetSaveFile.WriteEndElement();

                // Customer Firstname/Lastname/Day/Month/Year/TimeStamp/TemplateID case
                if (this.StaticObjects[i].Name.Contains(InformationObjectConstants.CustomerFirstName) ||
                    this.StaticObjects[i].Name.Contains(InformationObjectConstants.CustomerLastName) ||
                    this.StaticObjects[i].Name.Contains(InformationObjectConstants.Day) ||
                     this.StaticObjects[i].Name.Contains(InformationObjectConstants.Month) ||
                     this.StaticObjects[i].Name.Contains(InformationObjectConstants.Year) ||
                     this.StaticObjects[i].Name.Contains(InformationObjectConstants.TimeStamp) ||
                     this.StaticObjects[i].Name.Contains(InformationObjectConstants.TemplateID))
                {
                    // Textbox x
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectX);
                    targetSaveFile.WriteString(this.StaticObjects[i].X.ToString());
                    targetSaveFile.WriteEndElement();

                    // Textbox y
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectY);
                    targetSaveFile.WriteString(this.StaticObjects[i].Y.ToString());
                    targetSaveFile.WriteEndElement();

                    // Textbox fontsize
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectFontSize);
                    targetSaveFile.WriteString(this.StaticObjects[i].FontSize.ToString());
                    targetSaveFile.WriteEndElement();
                }
                // Counter case
                else if (this.StaticObjects[i].Name.Contains(InformationObjectConstants.Counter))
                {
                    // Textbox x
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectX);
                    targetSaveFile.WriteString(this.StaticObjects[i].X.ToString());
                    targetSaveFile.WriteEndElement();

                    // Textbox y
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectY);
                    targetSaveFile.WriteString(this.StaticObjects[i].Y.ToString());
                    targetSaveFile.WriteEndElement();

                    // Textbox required (representing whether to reset the counter to 0 at the beginning of a new year)
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectRequired);
                    targetSaveFile.WriteString(this.StaticObjects[i].Required.ToString());
                    targetSaveFile.WriteEndElement();

                    // Textbox fontsize
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectFontSize);
                    targetSaveFile.WriteString(this.StaticObjects[i].FontSize.ToString());
                    targetSaveFile.WriteEndElement();

                    // Textbox text
                    targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationTextboxText);
                    targetSaveFile.WriteString(this.StaticObjects[i].Text.ToString());
                    targetSaveFile.WriteEndElement();
                }
                
                // End static object
                targetSaveFile.WriteEndElement();
            }

            // -Template Textboxes
            for(int i = 0; i < this.TemplateTextboxes.Count; i++)
            {
                // Information Object title
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObject);

                // Information Object type
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectType);
                targetSaveFile.WriteString(JobTicketXMLNames.InformationTextbox);
                targetSaveFile.WriteEndElement();

                // Textbox name
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectName);
                targetSaveFile.WriteString(this.TemplateTextboxes[i].Name);
                targetSaveFile.WriteEndElement();

                // Textbox x
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectX);
                targetSaveFile.WriteString(this.TemplateTextboxes[i].X.ToString());
                targetSaveFile.WriteEndElement();

                // Textbox y
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectY);
                targetSaveFile.WriteString(this.TemplateTextboxes[i].Y.ToString());
                targetSaveFile.WriteEndElement();

                // Textbox required
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectRequired);
                targetSaveFile.WriteString(this.TemplateTextboxes[i].Required.ToString());
                targetSaveFile.WriteEndElement();

                // Textbox priority
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectPriority);
                targetSaveFile.WriteString(this.TemplateTextboxes[i].Priority.ToString());
                targetSaveFile.WriteEndElement();

                // Textbox fontsize
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectFontSize);
                targetSaveFile.WriteString(this.TemplateTextboxes[i].FontSize.ToString());
                targetSaveFile.WriteEndElement();

                // Textbox height
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectHeight);
                targetSaveFile.WriteString(this.TemplateTextboxes[i].Height.ToString());
                targetSaveFile.WriteEndElement();

                // Textbox width
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectWidth);
                targetSaveFile.WriteString(this.TemplateTextboxes[i].Width.ToString());
                targetSaveFile.WriteEndElement();

                // End textbox
                targetSaveFile.WriteEndElement();
            }

            // -Template Checkboxes
            for (int i = 0; i < this.TemplateCheckboxes.Count; i++)
            {
                // Information Object title
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObject);

                // Information Object type
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectType);
                targetSaveFile.WriteString(JobTicketXMLNames.InformationCheckbox);
                targetSaveFile.WriteEndElement();

                // Checkbox name
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectName);
                targetSaveFile.WriteString(this.TemplateCheckboxes[i].Name);
                targetSaveFile.WriteEndElement();

                // Checkbox x
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectX);
                targetSaveFile.WriteString(this.TemplateCheckboxes[i].X.ToString());
                targetSaveFile.WriteEndElement();

                // Checkbox y
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectY);
                targetSaveFile.WriteString(this.TemplateCheckboxes[i].Y.ToString());
                targetSaveFile.WriteEndElement();

                // Checkbox required
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectRequired);
                targetSaveFile.WriteString(this.TemplateCheckboxes[i].Required.ToString());
                targetSaveFile.WriteEndElement();

                // Checkbox priority
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectPriority);
                targetSaveFile.WriteString(this.TemplateCheckboxes[i].Priority.ToString());
                targetSaveFile.WriteEndElement();

                // Checkbox scale
                targetSaveFile.WriteStartElement(JobTicketXMLNames.InformationObjectScale);
                targetSaveFile.WriteString(this.TemplateCheckboxes[i].Scale.ToString());
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
        ///  Based on a file path, this function attempts to load the data of a ticket from the file path onto this object.
        ///  WARNING: This function is largely prone to throwing exceptions due to the use of Convert.ToDouble() and Convert.ToBoolean().
        ///     Exceptions can also be thrown from elements not being read correctly.
        ///     SUGGESTION: It is advised to use try{}catch{} statements surrounding this function.
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns>
        ///     True if successfully loaded.
        ///     False otherwise.
        /// </returns>
        public bool LoadTemplate(Stream fileStream)
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
            // If the format isn't matching the format of template (Template isn't found), then we return false.
            if (targetReadFile.ReadToFollowing(JobTicketXMLNames.Template) == false)
            {
                return false;
            }
            targetReadFile.ReadToFollowing(JobTicketXMLNames.TemplateName);
            this.Name = targetReadFile.ReadElementContentAsString(); 

            // Get next element. Set the attribute to this object's document path
            //targetReadFile.ReadToFollowing(JobTicketXMLNames.TemplateDocumentPath);
            this.DocumentPath = targetReadFile.ReadElementContentAsString();

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
                        // Store the name of the current textbox.
                        string currentTextbox = targetReadFile.ReadElementContentAsString();

                        // Add the static object depending on the name of the object
                        // Customer first name/last name case
                        if (currentTextbox.Contains(InformationObjectConstants.CustomerFirstName))
                        {
                            currentTextbox = this.AddStaticObject(0);

                            // X
                            this.SetStaticObjectX(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Y
                            this.SetStaticObjectY(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Font Size
                            this.SetStaticObjectFontsize(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));
                        }
                        else if (currentTextbox.Contains(InformationObjectConstants.CustomerLastName))
                        {
                            currentTextbox = this.AddStaticObject(1);

                            // X
                            this.SetStaticObjectX(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Y
                            this.SetStaticObjectY(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Font Size
                            this.SetStaticObjectFontsize(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));
                        }
                        // Counter case
                        else if (currentTextbox.Contains(InformationObjectConstants.Counter))
                        {
                            currentTextbox = this.AddStaticObject(2);

                            // X
                            this.SetStaticObjectX(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Y
                            this.SetStaticObjectY(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Required
                            this.SetStaticObjectRequirement(currentTextbox, Convert.ToBoolean(targetReadFile.ReadElementContentAsString()));

                            // Font Size
                            this.SetStaticObjectFontsize(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Text
                            this.SetStaticObjectText(currentTextbox, targetReadFile.ReadElementContentAsString());
                        }
                        // Day case
                        else if (currentTextbox.Contains(InformationObjectConstants.Day))
                        {
                            currentTextbox = this.AddStaticObject(3);

                            // X
                            this.SetStaticObjectX(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Y
                            this.SetStaticObjectY(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Font Size
                            this.SetStaticObjectFontsize(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));
                        }
                        // Month case
                        else if (currentTextbox.Contains(InformationObjectConstants.Month))
                        {
                            currentTextbox = this.AddStaticObject(4);

                            // X
                            this.SetStaticObjectX(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Y
                            this.SetStaticObjectY(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Font Size
                            this.SetStaticObjectFontsize(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));
                        }
                        // Year case
                        else if (currentTextbox.Contains(InformationObjectConstants.Year))
                        {
                            currentTextbox = this.AddStaticObject(5);

                            // X
                            this.SetStaticObjectX(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Y
                            this.SetStaticObjectY(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Font Size
                            this.SetStaticObjectFontsize(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));
                        }
                        // Timestamp case
                        else if (currentTextbox.Contains(InformationObjectConstants.TimeStamp))
                        {
                            currentTextbox = this.AddStaticObject(6);

                            // X
                            this.SetStaticObjectX(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Y
                            this.SetStaticObjectY(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Font Size
                            this.SetStaticObjectFontsize(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));
                        }
                        // Template ID case
                        else if (currentTextbox.Contains(InformationObjectConstants.TemplateID))
                        {
                            currentTextbox = this.AddStaticObject(7);

                            // X
                            this.SetStaticObjectX(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Y
                            this.SetStaticObjectY(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                            // Font Size
                            this.SetStaticObjectFontsize(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));
                        }
                    }

                    // If it's a textbox, load the textbox
                    if (objectType == JobTicketXMLNames.InformationTextbox)
                    {
                        // Store the name of the current textbox.
                        string currentTextbox = targetReadFile.ReadElementContentAsString();
                        // Store the height of the current textbox.
                        double currentTextboxHeight = 0;

                        // Add textbox with textbox name
                        this.AddTextbox(currentTextbox);

                        // X
                        this.SetTextboxX(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                        // Y
                        this.SetTextboxY(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                        // Required
                        this.SetTextboxRequirement(currentTextbox, Convert.ToBoolean(targetReadFile.ReadElementContentAsString()));

                        // Priority
                        this.SetTextboxPriority(currentTextbox, Convert.ToInt32(targetReadFile.ReadElementContentAsString()), false);

                        // Font Size
                        this.SetTextboxFontSize(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                        // Height
                        currentTextboxHeight = Convert.ToDouble(targetReadFile.ReadElementContentAsString());
                        this.SetTextboxDimensions(currentTextbox, 0, currentTextboxHeight);

                        // Width
                        this.SetTextboxDimensions(currentTextbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()), currentTextboxHeight);
                    }

                    // If it's a checkbox, load the checkbox
                    else if (objectType == JobTicketXMLNames.InformationCheckbox)
                    {
                        // Store the name of the current checkbox.
                        string currentCheckbox = targetReadFile.ReadElementContentAsString();

                        // Add checkbox with textbox name
                        this.AddCheckBox(currentCheckbox);

                        // X
                        this.SetCheckboxX(currentCheckbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                        // Y
                        this.SetCheckboxY(currentCheckbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));

                        // Required
                        this.SetCheckboxRequirement(currentCheckbox, Convert.ToBoolean(targetReadFile.ReadElementContentAsString()));

                        // Priority
                        this.SetCheckboxPriority(currentCheckbox, Convert.ToInt32(targetReadFile.ReadElementContentAsString()), false);

                        // Scale
                        this.SetCheckboxScale(currentCheckbox, Convert.ToDouble(targetReadFile.ReadElementContentAsString()));
                    }

                } while (targetReadFile.ReadToFollowing(JobTicketXMLNames.InformationObject) == true);

                // Sort all textboxes and checkboxes
                this.SortTextboxes();
                this.SortCheckboxes();
            }

            // Close and return once completed.
            targetReadFile.Close();
            return true;
        }

        /// <summary>
        ///  Clears all data from this object.
        /// </summary>
        /// </summary>
        public void Clear()
        {
            this.Name = string.Empty;
            this.DocumentPath = string.Empty;
            this.TemplateCheckboxes.Clear();
            this.TemplateTextboxes.Clear();
        }

        /// <summary>
        ///  Event handler function for when a subscription textbox's property changes.
        /// </summary>
        private void NotifyTextboxChanged(object? sender, PropertyChangedEventArgs e)
        {
            this.TextboxPropertyChanged.Invoke(this, new PropertyChangedEventArgs("textbox property changed"));
        }

        /// <summary>
        ///  Event handler function for when a subscription checkbox's property changes.
        /// </summary>
        private void NotifyCheckboxChanged(object? sender, PropertyChangedEventArgs e)
        {
            this.CheckboxPropertyChanged.Invoke(this, new PropertyChangedEventArgs("checkbox property changed"));
        }

        /// <summary>
        ///  Event handler function for when a subscription static object's property changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyStaticObjectChanged(object? sender, PropertyChangedEventArgs e)
        {
            this.StaticObjectPropertyChanged.Invoke(this, new PropertyChangedEventArgs("static object property changed"));
        }

        /// <summary>
        ///  Event handler function for when a property specific to the template changes. This includes:
        ///     -Name changing
        ///     -Document path changing
        ///     -Items being added or removed from template object lists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyTemplatePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            this.TemplatePropertyChanged.Invoke(this, new PropertyChangedEventArgs("template property changed"));
        }
    }
}