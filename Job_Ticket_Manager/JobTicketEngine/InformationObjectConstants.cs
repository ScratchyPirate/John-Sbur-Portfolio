/// <summary>
///  InformationObjectConstants.cs
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

    /// <summary>
    ///  Constants pertaining to information object constants
    /// </summary>
    public static class InformationObjectConstants
    {
        // Applies to all information objects
        public static string DefaultName
        {
            get { return "Information Object"; }
        }
        public static double DefaultX
        {
            get { return 0; }
        }
        public static double DefaultY
        {
            get { return 0; }
        }
        public static bool DefaultRequire
        {
            get { return false; }
        } 


        // Applies to objects with fontsize
        public static double DefaultFontSize
        {
            get { return 11; }
        }

        // Applies to objects with height
        public static double DefaultHeight
        {
            get { return 10; }
        }

        // Applies to objects with width
        public static double DefaultWidth
        {
            get { return 10; }
        }

        // Applies to objects with scale
        public static double DefaultScale
        {
            get { return 1; }
        }

        // Applies to specific information objects
        // Textbox
        public static string DefaultTextboxName
        {
            get { return "Textbox"; }
        }
        public static string DefaultTextboxText
        {
            get { return string.Empty; }
        }
        // Checkbox
        public static string DefaultCheckboxName
        {
            get { return "Checkbox"; }
        }
        public static bool DefaultCheckboxStatus
        {
            get { return false; }
        }

        // Static Objects
        public static string CustomerFirstName
        {
            get { return "Customer First Name"; }
        }
        public static string CustomerLastName
        {
            get { return "Customer Last Name"; }
        }
        public static string Counter
        {
            get { return "Counter"; }
        }
        public static string Day
        {
            get { return "Day"; }
        }
        public static string Month
        {
            get { return "Month"; }
        }
        public static string Year
        {
            get { return "Year"; }
        }
        public static string TimeStamp
        {
            get { return "Time Stamp"; }
        }
        public static string TemplateID
        {
            get { return "Template ID"; }
        }
        public static int CustomerNameMaxCharacters
        {
            get { return 10; }
        }
        public static int CounterMaxCharacters
        {
            get { return 12; }
        }
        public static int DayMaxCharacters
        {
            get { return 2; }
        }
        public static int MonthMaxCharacters
        {
            get { return 2; }
        }
        public static int YearMaxCharacters
        {
            get { return 4; }
        }
        public static int TimeStampMaxCharacters
        {
            get { return 11; }
        }
        public static int TemplateIDMaxCharacters
        {
            get { return 9; }
        }
        // Information Object Titles
        public static string StaticObjectTitle
        {
            get { return "Static Object"; }
        }
        public static string TextboxTitle
        {
            get { return "Textbox"; }
        }
        public static string CheckboxTitle
        {
            get { return "Checkbox"; }
        }

        // Property Names
        public static string DocumentPathProperty
        {
            get { return "File Path"; }
        }
        public static string NameProperty
        {
            get { return "Name"; }
        }
        public static string XProperty
        {
            get { return "X"; }
        }
        public static string YProperty
        {
            get { return "Y"; }
        }
        public static string RequiredProperty
        {
            get { return "Required to Enter"; }
        }
        public static string PriorityProperty
        {
            get { return "Priority"; }
        }
        public static string FontSizeProperty
        {
            get { return "Font Size"; }
        }
        public static string HeightProperty
        {
            get { return "Height"; }
        }
        public static string WidthProperty
        {
            get { return "Width"; }
        }
        public static string TextProperty
        {
            get { return "Text"; }
        }
        public static string ScaleProperty
        {
            get { return "Scale"; }
        }
        public static string ResetEachYearProperty
        {
            get { return "Reset Each Year"; }
        }
        public static string CurrentCounterValueProperty
        {
            get { return "Current Counter Value"; }
        }

    }
    
}