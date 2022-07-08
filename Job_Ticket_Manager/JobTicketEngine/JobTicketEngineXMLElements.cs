/// <summary>
///  JobTicketEngineXMLElements.cs
///  Job Ticket Manager Project
///  Creator: John D. Sbur
/// </summary>
namespace JobTicketEngine
{
    /// <summary>
    ///  Keeps track of project XML elements for each type of object.
    /// </summary>
    public static class JobTicketXMLNames
    {
        // **********************************
        // Pertaining to Template Ticket
        // **********************************

        // Pertaining to Template.
        public static string Template
        {
            get { return "template"; }
        }
        public static string TemplateName
        {
            get { return "templateName"; }
        }
        // Pertaining to Template document path.
        public static string TemplateDocumentPath
        {
            get { return "templateDocumentPath"; }
        }



        // **********************************
        // Pertaining to Job Ticket
        // **********************************
        public static string JobTicket
        {
            get { return "jobTicket"; }
        }
        public static string JobTicketFirstName
        {
            get { return "jobTicketFirstName"; }
        }
        public static string JobTicketLastName
        {
            get { return "jobTicketLastName"; }
        }
        public static string JobTicketDocumentPath
        {
            get { return "jobTicketDocumentPath"; }
        }



        // **********************************
        // Pertaining to Information Objects
        // **********************************

        // Pertaining to information objects.
        public static string InformationObject
        {
            get { return "informationObject"; }
        }
        // Pertaining to information object type.
        public static string InformationObjectType
        {
            get { return "informationObjectType"; }
        }
        // Pertaining to information object name.
        public static string InformationObjectName
        {
            get { return "objectName"; }
        }
        // Pertaining to information object x.
        public static string InformationObjectX
        {
            get { return "objectX"; }
        }
        // Pertaining to information object y.
        public static string InformationObjectY
        {
            get { return "objectY"; }
        }
        // Pertaining to information object required.
        public static string InformationObjectRequired
        {
            get { return "objectRequired"; }
        }
        // Pertaining to information object priority
        public static string InformationObjectPriority
        {
            get { return "objectPriority"; }
        }


        // Pertaining to information objects with font size.
        public static string InformationObjectFontSize
        {
            get { return "objectFontSize"; }
        }
        // Pertaining to information objects with height.
        public static string InformationObjectHeight
        {
            get { return "objectHeight"; }
        }
        // Pertaining to Template information objects with width.
        public static string InformationObjectWidth
        {
            get { return "objectWidth"; }
        }
        // Pertaining to information objects with scale.
        public static string InformationObjectScale
        {
            get { return "objectScale"; }
        }


        // Pertaining to information object type = InformationTextbox.
        public static string InformationTextbox
        {
            get { return "informationTextbox"; }
        }
        // Pertaining to information object type = InformationTextbox's containing text.
        public static string InformationTextboxText
        {
            get { return "informationTextboxText"; }
        }

        // Pertaining to information object type = InformationCheckbox.
        public static string InformationCheckbox
        {
            get { return "informationCheckbox"; }
        }
        // Pertaining to information object type = InformationCheckbox's status
        public static string InformationCheckboxStatus
        {
            get { return "informationCheckboxStatus"; }
        }

        // Pertaining to information objects that are static objects.
        public static string InformationStaticObject
        {
            get { return "informationStaticObject"; }
        }
    }
}