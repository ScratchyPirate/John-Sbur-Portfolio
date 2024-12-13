/// <summary>
///     File: Universals.java
///         Description: Represents the class of constants used throughout the project under
///         different contexts. Meant as a method to provide consistency.
///         In an instance where a programmer uses a certain number or
///         certain string to represent data and is expected to remember
///         that certain string or number's meaning, there is room for
///         error. The class of universals seeks to eliminate ths error
///         case.
/// </summary>
package com.example.alcoholconsumptiontracker.system;


public class Universals {

    ///
    /// Contains general constants used in the app
    ///
    public static class General{
        public static String EmptyString(){
            return "";
        }
    }

    ///
    ///  The class of static error messages for each class
    ///
    public static class TestMessages{

        /// Generic tag associated with test messages for logging functions.
        public static String TestMessageTag = "<Test Message>";
        /// Generic message title that appears in front of all error messages.
        public static String FailureMessageTitle = "Failure - ";
        /// Generic message title that appears in front of all test passing messages.
        public static String PassMessageTitle = "Pass - ";

        ///
        ///  The class of test messages for drink template
        ///
        public static class DrinkTemplateMessages{

            ///
            public static String DrinkTemplateFailureMessageTitle = FailureMessageTitle + "DrinkTemplate: ";
            public static String DrinkTemplatePassMessageTitle = PassMessageTitle + "DrinkTemplate: ";

            public static String ProduceDrinkMessage(boolean pass, int testCase){
                if (pass){
                    return DrinkTemplatePassMessageTitle + "Produce Drink Method Success. Test Case <" + testCase + ">";
                }
                else
                    return DrinkTemplateFailureMessageTitle + "Produce Drink Method Failure. Test Case <" + testCase + ">";
            }


        }

        ///
        ///  The class of test messages for drink
        ///
        public static class DrinkMessages{
            ///
            public static String DrinkErrorMessageTitle = FailureMessageTitle + "Drink: ";
            public static String DrinkPassMessageTitle = PassMessageTitle + "Drink: ";

            public static String GetterSetterMessage(boolean pass, int testCase){
                if (pass){
                    return PassMessageTitle +  "Drink Getter Setter Methods Success. Test Case <" + testCase + ">";
                }
                else
                    return FailureMessageTitle + "Drink Getter Setter Methods Failure. Test Case <" + testCase + ">";
            }

        }

        ///
        ///  The class of test messages for drink template manager
        ///
        public static class DrinkTemplateManagerMessages{
            public static String DrinkTemplateManagerFailureMessageTitle = FailureMessageTitle + "DrinkTemplateManager: ";
            public static String DrinkTemplateManagerPassMessageTitle = PassMessageTitle + "DrinkTemplateManager: ";

            public static String TemplatePutMessage(boolean pass, int testCase){
                if (pass){
                    return DrinkTemplateManagerPassMessageTitle + "Template Put Pass. Test Case <" + testCase + ">";
                }
                else
                    return DrinkTemplateManagerFailureMessageTitle + "Template Put Failure. Test Case <" + testCase + ">";
            }
            public static String TemplateModifyMessage(boolean pass, int testCase){
                if (pass){
                    return DrinkTemplateManagerPassMessageTitle + "Template Modify Pass. Test Case <" + testCase + ">";
                }
                else
                    return DrinkTemplateManagerFailureMessageTitle + "Template Modify Failure. Test Case <" + testCase + ">";
            }
            public static String TemplateRemoveMessage(boolean pass, int testCase){
                if (pass){
                    return DrinkTemplateManagerPassMessageTitle + "Template Remove Pass. Test Case <" + testCase + ">";
                }
                else
                    return DrinkTemplateManagerFailureMessageTitle + "Template Remove Failure. Test Case <" + testCase + ">";
            }
            public static String TemplateReadWriteTemplateListMessage(boolean pass, int testCase){
                if (pass){
                    return DrinkTemplateManagerPassMessageTitle + "Write Template List Pass. Test Case <" + testCase + ">";
                }
                else
                    return DrinkTemplateManagerFailureMessageTitle + "Write Template List Failure. Test Case <" + testCase + ">";
            }
        }

        ///
        /// The class of test messages for drink template manager
        ///
        public static class DatabaseManagerMessages{
            public static String DatabaseManagerFailureMessageTitle = FailureMessageTitle + "DatabaseManager: ";
            public static String DatabaseManagerPassMessageTitle = PassMessageTitle + "DatabaseManager: ";

            public static String DatabaseInitializeMessage(boolean pass, int testCase){
                if (pass){
                    return DatabaseManagerPassMessageTitle + "Database Manager Initialize Pass. Test Case <" + testCase + ">";
                }
                else
                    return DatabaseManagerFailureMessageTitle + "Database Manager Initialize Failure. Test Case <" + testCase + ">";
            }
        }
    }

    ///
    /// Class of messages associated with system classes that, when an error occurs
    ///     are used to be displayed in the app logcat
    ///
    public static class ErrorMessages{
        /// Generic tag associated with test messages for logging functions.
        public static String ErrorMessageTag = "<Non-Fatal Error>";

        ///
        /// Database manager messages
        ///
        public static class DatabaseManagerErrorMessages{

            public static String MessageTitle = "DatabaseManager: ";
            public static String InitializeDatabaseSecurityError = MessageTitle + "Security error incurred while accessing database. Check app security settings.";
        }

        public static class DrinkTemplateManagerErrorMessages{
            public static String MessageTitle = "DrinkTemplateManager: ";
            public static String WriteTemplatesErrorFileNotFound = MessageTitle + "Target file not found.";
            public static String WriteTemplatesErrorFailedToCreateFile = MessageTitle + "IO Error. Failed to create new XML file.";
            public static String WriteTemplatesErrorFailedToCreateDocument = MessageTitle + "XML DOM Error. Failed to create XML document object.";
            public static String WriteTemplatesErrorTransformerError = MessageTitle + "XML transformer error. Failed to convert DOM Document to XML file.";
            public static String ReadTemplatesErrorDirectoryNotFound = MessageTitle + "Failed to find inputted directory.";
            public static String ReadTemplatesErrorFileNotFound = MessageTitle + "Failed to find inputted file within inputted directory.";
            public static String ReadTemplatesErrorFailedToCreateDocument = MessageTitle + "XML DOM Error. Failed to create XML document object.";
            public static String ReadTemplatesErrorFileIOError = MessageTitle + "Failed to open target XML file for parsing.";
            public static String ReadTemplatesErrorInvalidXMLFile = MessageTitle + "Found file contained content not in an XML format and couldn't be parsed.";

            public static String ReadTemplatesErrorFileParseError = MessageTitle + "XML file found parsed incorrectly. Wasn't found to be a DrinkTemplateManager XML format file.";



        }

    }

    ///
    /// Class of file names for the system
    ///
    public static class FileNames{
        /// <summary>
        ///     Represents the file name for the file containing created templates
        ///         in the drink template manager.
        /// </summary>
        public static String TemplateListFile = "templates";
    }

    ///
    /// Class of XML tags used in app backend
    ///
    public static class XMLTags{
        ///
        ///  Tags for Drink
        ///
        public static class DrinkTags{
            public static String Header = "drink";
            public static String Name = "name";
            public static String Type = "type";
            public static String Servings = "servings";
            public static String APV = "apv";
            public static String Calories = "calories";
            public static String Price = "price";
            public static String ImageFilePath = "imgFilePath";
            public static String Occasion = "occasion";
            public static String HourOfConsumption = "hour";
            public static String MinuteOfConsumption = "minute";
        }
        ///
        ///  Tags for DrinkTemplate
        ///
        public static class DrinkTemplateTags{
            public static String Header =  "drinkTemplate";
            public static String Name = "name";
            public static String Type = "type";
            public static String Servings = "servings";
            public static String APV = "apv";
            public static String Calories = "calories";
            public static String Price = "price";
            public static String ImageFilePath = "imgFilePath";
        }
        ///
        ///  Tags for DrinkTemplateManager
        ///     -Manager stores templates as a list within its header
        ///
        public static class DrinkTemplateManagerTags{
            public static String Header = "drinkTemplateManager";
        }

    }

    ///
    ///  Contains tags used in DrinkLogging frontend objects.
    ///     Text the user sees on UI elements originates from here.
    ///
    public static class DrinkLoggingUI{

        ///
        /// Drink Tags
        ///
        public static class DrinkTemplateTags{
            public static String price = "Price ($): ";
            public static String servings = "Servings: ";
            public static String name = "Drink Name: ";
            public static String calories = "Calories (kcal): ";
            public static String type = "Drink Type: ";
        }

        ///
        /// Drink Logging UI
        ///
        public static class UITags{
            public static String OccasionDefaultText = "(Optional)";
        }
    }

}
