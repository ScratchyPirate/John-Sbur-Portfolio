﻿/// <summary>
///  Constants.cs
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
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Font = Font;
    using Point = Point;
    using Rectangle = Rectangle;

    /// <summary>
    ///  Constants pertaining to the creation window class.
    /// </summary>
    public static class CreationWindowConstants
    {
        // General contants
        public static string RequiredToEnter
        {
            get { return "[Required] "; }
        }
        
        // ComboBox constants
        public static string ComboBoxDefault
        {
            get { return "-"; }
        }
        public static string ComboBoxTrue
        {
            get { return "Yes"; }
        }
        public static string ComboBoxFalse
        {
            get { return "No"; }
        }

        // A constant scale that determines how big the creation tab image and data are displayed as compared to their original.
        public static double CreationTabDataScale
        {
            get { return 0.25; }
        }
    }

    /// <summary>
    ///  Class meant to contain constants available to all parts of the program with drawing tools
    ///  such as pens and fonts for easy access.
    /// </summary>
    public static class PaintingConstants
    {
        // Default drawing tools.

        // Pen
        public static Pen BlackPen(int penSize = 2)
        {
            return new Pen(Color.Black, penSize);
        }
        public static Pen RedPen(int penSize = 2)
        {
            return new Pen(Color.Red, penSize);
        }
        public static Pen BluePen(int penSize = 2)
        {
            return new Pen(Color.Blue, penSize);
        }
        public static Pen GreenPen(int penSize = 2)
        {
            return new Pen(Color.Green, penSize);
        }

        // Fonts
        public static Font Arial(int fontSize = 11)
        {
            return new Font("Arial", fontSize);
        }
        public static Font TimesNewRoman(int fontSize = 11)
        {
            return new Font("Times New Roman", fontSize);
        }
        public static Font ComicSans(int fontSize = 11)
        {
            return new Font("Comic Sans MS", fontSize);
        }
    }

    /// <summary>
    ///  Contains messages meant to be displayed when an error occurs. Keeps all strings in one place so that they don't need to be kept track of.
    /// </summary>
    public static class ErrorMessages
    {
        // ********************
        // Error Message Titles
        // ********************
        public static string PropertyEditErrorTitle
        {
            get { return "Edit Property Error"; }
        }
        public static string ImageLoadErrorTitle
        {
            get { return "Image Load Error"; }
        }
        public static string CreateTemplateErrorTitle
        {
            get { return "Create Template Error"; }
        }
        public static string CreateObjectErrorTitle
        {
            get { return "Create Object Error"; }
        }
        public static string SaveTemplateErrorTitle
        {
            get { return "Save Template Error"; }
        }
        public static string CreateTicketErrorTitle
        {
            get { return "Create Ticket Error"; }
        }
        public static string ModifyTicketErrorTitle
        {
            get { return "Modify Ticket Error"; }
        }
        public static string DeleteTicketErrorTitle
        {
            get { return "Delete Ticket Error"; }
        }
        public static string PrintTicketErrorTitle
        {
            get { return "Print Ticket Error"; }
        }
        public static string ApplicationErrorTitle
        {
            get { return "Application Error"; }
        }



        // ********************
        // Error Messages
        // ********************
        // Property Edit Error Messages
        public static string PropertyEditMissingEntryErrorMessage
        {
            get { return "Entry cannot be left empty."; }
        }
        public static string PropertyEditInvalidNumberEntryErrorMessage(int minimumNumber = 0, int maximumNumber = int.MaxValue)
        {
            return "Entry entered was not valid." + Environment.NewLine + "(Not a number, below minimum possible value '" + minimumNumber.ToString() + "', or above maximum possible value '" + maximumNumber.ToString() + "')";
        }

        // Image Load Error Messages
        public static string ImageLoadDocumentCurrentlyOpenedMessage
        {
            get { return "Target document is open in another process and cannot be opened."; }
        }

        // Create Template Error Messages
        public static string CreateTemplateImageMissingErrorMessage
        {
            get { return "Failed to find requested image."; }
        }
        public static string CreateTemplateFailedToLoadFileMessage
        {
            get { return "Failed to load chosen file."; }
        }

        // Create Object Error Messages
        public static string CreateObjectMissingTemplateMessage
        {
            get { return "No active template selected. Cannot create a template object."; }
        }

        // Save Template Error Messages
        public static string SaveTemplateMissingTemplateMessage
        {
            get { return "No active template to save. Save failed"; }
        }
        public static string SaveTemplateMissingTemplatePathMessage
        {
            get { return "No path entered to save template to."; }
        }
        public static string SaveTemplateOpenFileFailedMessage
        {
            get { return "Could not open file chosen for writing."; }
        }

        // Create Ticket Error Messages
        public static string CreateTicketMissingRequiredFieldsMessage
        {
            get { return "Not all required fields are filled out."; }
        }
        public static string CreateTicketFailedToSaveFileMessage
        {
            get { return "Failed to save Job Ticket to Active Database."; }
        }
        public static string CreateTicketMaximumCharactersReachedMessage
        {
            get { return "Maximum number of characters have been entered."; }
        }
        public static string CreateTicketMissingBaseTemplateMessage
        {
            get { return "No template chosen."; }
        }
        public static string CreateTicketBaseTemplateFileMissingMessage
        {
            get { return "Base template missing. Cannot update information pertaining to template after creating this job ticket."; }
        }

        // Modify Ticket Error Messages
        public static string ModifyTicketMissingRequiredFieldsMessage
        {
            get { return "Not all required fields are filled out."; }
        }
        public static string ModifyTicketFailedToSaveFileMessage
        {
            get { return "Failed to save Job Ticket to Active Database."; }
        }
        public static string ModifyTicketMaximumCharactersReachedMessage
        {
            get { return "Maximum number of characters have been entered."; }
        }
        public static string ModifyTicketFailedToOpenTicketMessage
        {
            get { return "Failed to open requested Job Ticket."; }
        }
        public static string ModifyTicketNoJobTicketSelectedMessage
        {
            get { return "No Job Ticket Selected." + Environment.NewLine + "(Select a single Green tab of an existing Job Ticket and then try again)"; }
        }
        public static string ModifyTicketFailedToModifyTicketMessage
        {
            get { return "Failed to modify requested Job Ticket."; }
        }

        // Delete Ticket Error Messages
        public static string DeleteTicketFileNotFoundMessage
        {
            get { return "Requested Job Ticket not found in Active Database."; }
        }
        public static string DeleteTicketNoJobTicketSelectedMessage
        {
            get { return "No Job Ticket Selected." + Environment.NewLine + "(Select a single Green tab of an existing Job Ticket and then try again)"; }
        }
        public static string DeleteTicketFailedToDeleteMessage
        {
            get { return "Failed to delete requested Job Ticket."; }
        }

        // Print Ticket Error Messages
        public static string PrintTicketFileNotFoundMessage
        {
            get { return "Requested Job Ticket not found in Active Database."; }
        }
        public static string PrintTicketNoJobTicketSelectedMessage
        {
            get { return "No Job Ticket Selected." + Environment.NewLine + "(Select a single Green tab of an existing Job Ticket and then try again)"; }
        }
        public static string PrintTicketFailedToPrintMessage
        {
            get { return "Failed to print requested Job Ticket."; }
        }

        // Application Error Messages
        public static string ApplicationErrorSystemDatabaseNotCreated
        {
            get { return "FATAL ERROR: System Database could not be created. Check to see if there is an %appdata% folder on your system"; }
        }
        public static string ApplicationErrorSystemDataNotCreated
        {
            get { return "FATAL ERROR: System Data File could not be created. Check to see if there is an %appdata% folder on your system"; }
        }
        public static string ApplicationErrorSystemDataNotRead
        {
            get { return "FATAL ERROR: System data read error."; }
        }
    }

    /// <summary>
    ///  Generic methods that can be applied across the project.
    /// </summary>
    public static class ProjectMethods
    {
        /// <summary>
        ///  Tests to see whether the testString is a double that is at least "minimum".
        /// </summary>
        /// <param name="testString">
        ///  String being tested to see whether it is a double or not.
        /// </param>
        /// <param name="atLeast">
        ///  Value that the testString needs to evaluate to or more.
        /// </param>
        /// <returns>
        ///  True if the testString is a double that is at least the minimum value.
        ///  False otherwise.
        /// </returns>
        public static bool IsDouble(string? testString, double minimum = 0, double max = double.MaxValue)
        {
            try
            {
                double evaluatedNumber = Convert.ToDouble(testString);
                
                if (evaluatedNumber >= minimum && evaluatedNumber <= max)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///  Tests to see whether the testString is a integer that is at least "minimum".
        /// </summary>
        /// <param name="testString">
        ///  String being tested to see whether it is a integer or not.
        /// </param>
        /// <param name="atLeast">
        ///  Value that the testString needs to evaluate to or more.
        /// </param>
        /// <returns>
        ///  True if the testString is a integer that is at least the minimum value.
        ///  False otherwise.
        /// </returns>
        public static bool IsInt(string? testString, int minimum = 0, int maximum = int.MaxValue)
        {
            try
            {
                int evaluatedNumber = Convert.ToInt32(testString);

                if (evaluatedNumber >= minimum && evaluatedNumber <= maximum)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    ///  Constants pertaining to databases
    /// </summary>
    public static class ProjectDataBaseConstants
    {
        // Active database folder paths
        public static string DefaultActiveDatabaseName
        {
            get { return "\\DefaultActiveDatabase"; }
        }
        public static string JobTicketDirectory
        {
            get { return "\\Job Tickets"; }
        }
        public static string TemplateDirectory
        {
            get { return "\\Templates"; }
        }

        // System database folder paths
        public static string SystemDatabaseName
        {
            get { return "\\JCSTicketManager"; }
        }

        // Client %appdata% folder
        public static string AppDataDirectory
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)); }
        }
        public static string DefaultDocumentName
        {
            get { return "\\fail_to_load_image_graphic"; }
        }
        public static string ApplicationDataFileName
        {
            get { return "\\SystemData"; }
        }

        // System XML Names
        public static string SystemXMLSystemDatabase
        {
            get { return "systemDatabase"; }
        }
        public static string SystemXMLActiveDatabase
        {
            get { return "activeDatabase"; }
        }
        
    }
}
