/// <summary>
///  ActiveDatabase.cs
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

    /// <summary>
    ///  A child of database that also includes functions and methods for supporting a user database.
    /// </summary>
    public class ActiveDatabase : Database
    {
        /// <summary>
        ///  Name of the user database file.
        /// </summary>
        protected const string userDatabaseName = "\\UDB";

        /// <summary>
        ///  Name of settings file
        /// </summary>
        protected const string adminSettingsFileName = "\\ASF";

        /// <summary>
        ///  Determines whether guest accounds can read data from the database.
        /// </summary>
        private bool guestCanView;

        /// <summary>
        ///  Default constructor for active database.
        /// </summary>
        public ActiveDatabase()
        {
            this.guestCanView = false;
        }

        /// <summary>
        ///  Initializes active database based on given path
        /// </summary>
        /// <param name="path"></param>
        public ActiveDatabase(string path)
        {
            this.guestCanView = false;

            this.InitializeSettings();
        }

        /// <summary>
        ///  Getter for guest can view.
        /// </summary>
        public bool GuestCanView
        {
            get { return this.guestCanView; }
        }

        /// <summary>
        ///  Overriden method of Database name that also initializes the settings once the database name is changed.
        /// </summary>
        public override string DatabaseName
        {
            get { return this.databaseName; }
            set { 
                this.databaseName = value;
                this.InitializeSettings();
            }
        }

        /// <summary>
        ///  Returns whether the user database exists or not.
        /// </summary>
        public bool UserDatabaseInitialized
        {
            get
            {
                return File.Exists(this.DatabaseName + userDatabaseName);
            }
        }

        /// <summary>
        ///     If the user database file doesn't exist, initialize it. Retrieves settings from file if it already exists
        /// </summary>
        /// <returns>
        ///     0 when a file error or reading error has occurred.
        ///     1 when read and updated successfully.
        /// </returns>
        public int InitializeSettings()
        {
            // If it doesn't exist, create it and open it.
            if (!File.Exists(this.DatabaseName + adminSettingsFileName))
            {
                Stream saveStream;
                XmlWriter targetSaveFile;

                // Settings for the XmlWriter once created
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "   ";
                settings.NewLineOnAttributes = true;
                settings.Encoding = Encoding.UTF8;

                // Open file for writing in XML
                saveStream = File.OpenWrite(this.DatabaseName + adminSettingsFileName);
                targetSaveFile = XmlWriter.Create(saveStream, settings);

                // Set filestream length 0 to clear file
                saveStream.SetLength(0);

                // Begin document
                targetSaveFile.WriteStartDocument();

                // Begin settings object
                targetSaveFile.WriteStartElement(SecurityXMLNames.SettingsObject);

                // Guests can view setting
                targetSaveFile.WriteStartElement(SecurityXMLNames.GuestsCanView);
                targetSaveFile.WriteString(Cipher.TranspositionEncrypt(false.ToString()));
                targetSaveFile.WriteEndElement();

                // End settings
                targetSaveFile.WriteEndElement();

                // Flush to file
                targetSaveFile.Flush();

                // Close when done
                targetSaveFile.Close();
                saveStream.Close();

                return 1;
            }
            else
            {
                // Try extracting settings from settings file. If it fails, throw and set settings to default
                try
                {
                    // Local
                    Stream readStream;
                    XmlReader parser;

                    // Settings for stream reader
                    XmlReaderSettings readerSettings = new XmlReaderSettings();
                    readerSettings.IgnoreComments = true;
                    readerSettings.IgnoreWhitespace = true;

                    // Open reader XML stream
                    readStream = File.OpenRead(this.DatabaseName + adminSettingsFileName);
                    parser = XmlReader.Create(readStream);

                    try
                    {
                        // Begin Parse to settings
                        parser.ReadToFollowing(SecurityXMLNames.SettingsObject);

                        // Guests can view settings
                        parser.ReadToFollowing(SecurityXMLNames.GuestsCanView);
                        this.guestCanView = Convert.ToBoolean(Cipher.TranspositionDecrypt(parser.ReadElementContentAsString()));

                        // Close once all settings have been read
                        parser.Close();
                        readStream.Close();

                        return 1;
                    }
                    catch
                    {
                        // Close files and return on error
                        parser.Close();
                        readStream.Close();
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
           
        }

        /// <summary>
        ///  Saves settings of active database to settings file.
        /// </summary>
        /// <returns></returns>
        public int SaveSettings()
        {
            // If it doesn't exist, create it and save it.
            Stream saveStream;
            XmlWriter targetSaveFile;

            // Settings for the XmlWriter once created
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            settings.NewLineOnAttributes = true;
            settings.Encoding = Encoding.UTF8;

            // Open file for writing in XML
            saveStream = File.OpenWrite(this.DatabaseName + adminSettingsFileName);
            targetSaveFile = XmlWriter.Create(saveStream, settings);

            // Set filestream length 0 to clear file
            saveStream.SetLength(0);

            // Begin document
            targetSaveFile.WriteStartDocument();

            // Begin settings object
            targetSaveFile.WriteStartElement(SecurityXMLNames.SettingsObject);

            // Guests can view setting
            targetSaveFile.WriteStartElement(SecurityXMLNames.GuestsCanView);
            targetSaveFile.WriteString(Cipher.TranspositionEncrypt(this.guestCanView.ToString()));
            targetSaveFile.WriteEndElement();

            // End settings
            targetSaveFile.WriteEndElement();

            // Flush to file
            targetSaveFile.Flush();

            // Close when done
            targetSaveFile.Close();
            saveStream.Close();

            return 1;
        }

        /// <summary>
        /// Updates guests can view setting to match new value. Saves result to settings file.
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns>
        ///     0 if unsuccessfully saved to settings file.
        ///     1 if successfully saved to settings file.
        /// </returns>
        public int UpdateGuestsCanView(bool newValue)
        {
            this.guestCanView = newValue;

            return this.SaveSettings();
        }

        /// <summary>
        ///  This function handles logging in based on a user input. 
        /// </summary>
        /// <param name="enteredUsername"></param>
        /// <param name="enteredPassword"></param>
        /// <returns>
        ///     0 if successfully logged in (admin)
        ///     1 if successfully logged in (guest)
        ///     2 if unsuccessfully logged in (user database missing)
        ///     3 if unsuccessfully logged in (username not found)
        ///     4 if unsuccessfully logged in (passwords didn't match)
        ///     5 if unsuccessfully logged in (file error)
        /// </returns>
        public int Login(string enteredUsername, string enteredPassword)
        {
            // Check to see if the userDatabase file exists. If it doesn't, return false.
            if (!File.Exists(this.DatabaseName + userDatabaseName))
            {
                return 2;
            }

            // Open the file and parse through for users. If found, extract salt and password
            // Local variables
            Stream readStream;

            // Settings for stream reader
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreComments = true;
            readerSettings.IgnoreWhitespace = true;

            // Local var. Temporary var
            string currentUsername = "";
            string extractedPassword = "";
            string extractedSalt = "";
            string extractedPrivilege = "";
            bool doneParsing = false;

            // Open reader XML stream
            readStream = File.OpenRead(this.DatabaseName + userDatabaseName);
            XmlReader parser = XmlReader.Create(readStream);

            try
            {
                // Read to first user.
                parser.ReadToFollowing(SecurityXMLNames.UserObject);
                do
                {
                    // Look at each username. If username matches the inputted user's name, return and show an error
                    // ***************************************************************************************************************************************************

                    // Read to the username element
                    parser.ReadToFollowing(SecurityXMLNames.UsernameObject);

                    // Extract username
                    currentUsername = Cipher.TranspositionDecrypt(parser.ReadElementContentAsString());

                    // Compare. If match, extract password, salt, and privilege
                    if (enteredUsername == currentUsername)
                    {
                        // Extract password, salt, and privilege
                        parser.ReadToFollowing(SecurityXMLNames.PasswordObject);
                        extractedPassword = Cipher.TranspositionDecrypt(parser.ReadElementContentAsString());
                        parser.ReadToFollowing(SecurityXMLNames.SaltObject);
                        extractedSalt = Cipher.TranspositionDecrypt(parser.ReadElementContentAsString());
                        parser.ReadToFollowing(SecurityXMLNames.PrivilegeObject);
                        extractedPrivilege = Cipher.TranspositionDecrypt(parser.ReadElementContentAsString());

                        // Mark as done parsing.
                        doneParsing = true;
                    }

                }
                while (parser.ReadToFollowing(SecurityXMLNames.UserObject) && !doneParsing);

                // Close
                parser.Close();
                readStream.Close();

                // If never found, return
                if (doneParsing == false)
                {
                    return 2;
                }
            }
            catch
            {
                return 5;
            }

            // Hash entered password with salt and check if they match. If they done, return fail
            enteredPassword = enteredPassword + extractedSalt;
            if (extractedPassword == (SecurityHash(enteredPassword))){
                if (extractedPrivilege == SecurityXMLNames.XMLAdminUser)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }

            }
            else
            {
                return 4;
            }       
        }

        /// <summary>
        ///  Given a filestream, this function adds a username with the given name and password to the filestream appended to the end.
        /// </summary>
        /// <param name="saveStream"></param>
        /// <param name="newUserName"></param>
        /// <param name="newUserPassword"></param>
        /// <returns>
        ///     1 if successfully added
        ///     2 if unsuccessfully added (user already exists)
        ///     3 if unsuccessfully added (file error)
        /// </returns>
        public int AddNewUser(string newUserName, string newUserPassword, string newUserPrivilege, int desiredSaltedLength)
        {
            // Filestream of user database
            Stream saveStream;
            Stream readStream;

            // XmlWriter used for saving the user.
            XmlWriter targetSaveFile;

            // Generate salt for user. Ensure salt is not null.
            string? salt = this.GenerateSalt(newUserPassword, desiredSaltedLength);
            if (salt == null)
            {
                salt = string.Empty;
            }

            // Attempt to open the file specified by "saveStream"
            try
            {
                // If it doesn't exist, create it and save the user
                if (!File.Exists(this.DatabaseName + userDatabaseName))
                {
                    // Settings for the XmlWriter once created
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.IndentChars = "   ";
                    settings.NewLineOnAttributes = true;
                    settings.Encoding = Encoding.UTF8;

                    // Open file for writing in XML
                    saveStream = File.OpenWrite(this.DatabaseName + userDatabaseName);
                    targetSaveFile = XmlWriter.Create(saveStream, settings);

                    // Set filestream length 0 to clear file
                    saveStream.SetLength(0);

                    // Begin document
                    targetSaveFile.WriteStartDocument();

                    // Begin Userdatabase
                    targetSaveFile.WriteStartElement(SecurityXMLNames.UserDatabaseObject);

                    // Begin user
                    targetSaveFile.WriteStartElement(SecurityXMLNames.UserObject);

                    // Username
                    targetSaveFile.WriteStartElement(SecurityXMLNames.UsernameObject);
                    targetSaveFile.WriteString(Cipher.TranspositionEncrypt(newUserName));
                    targetSaveFile.WriteEndElement();

                    // Password
                    targetSaveFile.WriteStartElement(SecurityXMLNames.PasswordObject);
                    // Store hashed password
                    targetSaveFile.WriteString(Cipher.TranspositionEncrypt(this.SecurityHash(newUserPassword + salt)));
                    targetSaveFile.WriteEndElement();

                    // Salt
                    targetSaveFile.WriteStartElement(SecurityXMLNames.SaltObject);
                    targetSaveFile.WriteString(Cipher.TranspositionEncrypt(salt));
                    targetSaveFile.WriteEndElement();

                    // Privilege
                    targetSaveFile.WriteStartElement(SecurityXMLNames.PrivilegeObject);
                    targetSaveFile.WriteString(Cipher.TranspositionEncrypt(newUserPrivilege));
                    targetSaveFile.WriteEndElement();

                    // End user
                    targetSaveFile.WriteEndElement();

                    // End off user database 
                    targetSaveFile.WriteEndElement();

                    // Flush to file
                    targetSaveFile.Flush();

                    // Close when done
                    targetSaveFile.Close();
                    saveStream.Close();

                    return 1;
                }
                // If it does exist, open it and add the new user to the end
                else
                {
                    // Settings for stream reader
                    XmlReaderSettings readerSettings = new XmlReaderSettings();
                    readerSettings.IgnoreComments = true;
                    readerSettings.IgnoreWhitespace = true;

                    // Local var. Temporary var
                    string currentUsername;

                    // Open reader XML stream
                    readStream = File.OpenRead(this.DatabaseName + userDatabaseName);
                    XmlReader parser = XmlReader.Create(readStream);

                    // Look at each username. If username matches the inputted user's name, return and show an error
                    // ***************************************************************************************************************************************************
                    parser.ReadToFollowing(SecurityXMLNames.UserObject);
                    // For each user in the user database. Check to see if the user's name matches. If it does, error
                    do
                    {
                        // Read to the username element
                        parser.ReadToFollowing(SecurityXMLNames.UsernameObject);

                        // Extract username
                        currentUsername = Cipher.TranspositionDecrypt(parser.ReadElementContentAsString());

                        // Compare. Possibly error
                        if (newUserName == currentUsername)
                        {
                            parser.Close();
                            readStream.Close();
                            return 2;
                        }

                    } while (parser.ReadToFollowing(SecurityXMLNames.UserObject));

                    // Close once done.
                    parser.Close();
                    readStream.Close();
                    // ***************************************************************************************************************************************************

                    // Load the exiting xml document
                    XmlDocument userDatabaseFile = new XmlDocument();
                    userDatabaseFile.Load(this.DatabaseName + userDatabaseName);

                    // Start from root of document
                    XmlNode root;
                    try
                    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                        root = userDatabaseFile.DocumentElement;

                        if (root == null)
                        {
                            return 3;
                        }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    }
                    catch
                    {
                        return 3;
                    }

                    // Create user node
                    XmlElement newUserNode = userDatabaseFile.CreateElement(SecurityXMLNames.UserObject);
                    XmlElement holder;

                    // Add user content
                    // Username
                    holder = userDatabaseFile.CreateElement(SecurityXMLNames.UsernameObject);
                    holder.InnerText = Cipher.TranspositionEncrypt(newUserName);
                    newUserNode.AppendChild(holder);

                    // Password
                    holder = userDatabaseFile.CreateElement(SecurityXMLNames.PasswordObject);
                    holder.InnerText = Cipher.TranspositionEncrypt(this.SecurityHash(newUserPassword + salt));
                    newUserNode.AppendChild(holder);

                    // Salt
                    holder = userDatabaseFile.CreateElement(SecurityXMLNames.SaltObject);
                    holder.InnerText = Cipher.TranspositionEncrypt(salt);
                    newUserNode.AppendChild(holder);

                    // Privilege
                    holder = userDatabaseFile.CreateElement(SecurityXMLNames.PrivilegeObject);
                    holder.InnerText = Cipher.TranspositionEncrypt(newUserPrivilege);
                    newUserNode.AppendChild(holder);

                    // Append new user
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    root.AppendChild(newUserNode);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                    // Save changes
                    userDatabaseFile.Save(this.DatabaseName + userDatabaseName);
                    return 1;
                }

            }
            catch
            {
                return 3;
            }

        }

        /// <summary>
        ///  Given a targetUserName, this function looks for that user in the active database and promotes them to admin if they exist.
        /// </summary>
        /// <param name="targetUser"></param>
        /// <returns>
        ///     1 if successfully promoted
        ///     2 if unsuccessfully promoted (file error)
        /// </returns>
        public int PromoteUser(string targetUser, string newPrivilege)
        {
            // If user database file doesn't exist, return 2 and stop
            if (!File.Exists(this.DatabaseName + userDatabaseName))
            {
                return 2;
            }

            // Load the exiting xml document
            XmlDocument userDatabaseFile = new XmlDocument();
            userDatabaseFile.Load(this.DatabaseName + userDatabaseName);

            // Start from root of document
            XmlNode root;
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                root = userDatabaseFile.DocumentElement;

                if (root == null)
                {
                    return 2;
                }

                // Look in each user in the root
                foreach (XmlNode user in root)
                {
                    // Look at the first element of user.
                    if (user[SecurityXMLNames.UsernameObject] != null)
                    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        if (user[SecurityXMLNames.UsernameObject].InnerText != null)
                        {
                            if (Cipher.TranspositionDecrypt(user[SecurityXMLNames.UsernameObject].InnerText) == targetUser)
                            {
                                // Update admin permission here
                                user[SecurityXMLNames.PrivilegeObject].InnerText = Cipher.TranspositionEncrypt(newPrivilege);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                                // Save changes and quit after
                                userDatabaseFile.Save(this.DatabaseName + userDatabaseName);
                                return 1;
                            }
                        }
                    }
                }

#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            }
            catch
            {
                return 2;
            }

            // If we reach this point, we didn't find the user and need to return 2.
            return 2;
        }

        /// <summary>
        ///  This function is designed to generate salt based on a given input. Start is the initial string and desiredLength is the length of the salt added to the start.
        ///     ex start="test" and desiredLength=8. Outputted salt would be of length 4 so that when start and the output are combined, the total length is 8.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="desiredLength"></param>
        /// <returns></returns>
        public string? GenerateSalt(string start, int desiredLength)
        {
            // If the desiredLength is greater than the start, then return the empty string.
            if (start.Length >= desiredLength)
            {
                return string.Empty;
            }
            // Otherwise, randomly generate a sequence of bytes and return them as a string.
            else
            {
                Random random = new Random();
                string? saltHolder;
                char[] buffer = new char[desiredLength - start.Length];

                // If any characters are 0, set to 1
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                }

                // Convert to string and return.
                saltHolder = string.Join("", buffer);

                return saltHolder;
            }
        }

        /// <summary>
        ///  Given a string, this function hashes it together to be half its original length.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        private string SecurityHash(string start)
        {
            // Convert input to a character array
            char[] rawStart = start.ToCharArray();
            int[] rawStartInteger = new int[rawStart.Length];
            char[] hashedStart = new char[(rawStart.Length) / 2];
            hashedStart[0] = '\0';

            // For each character in the character array, assign it to an integer
            // For each integer in the integer array, multiply by 5, then mod it by 255
            for (int i = 0; i < rawStart.Length; i++)
            {
                // Convert
                rawStartInteger[i] = Convert.ToInt32(rawStart[i]);

                // Hash
                rawStartInteger[i] = ((rawStartInteger[i] * 5) % 254) + 1;
            }

            // Add every 2 number together, then multiply by 26 and add 65. Then convert into a character.
            for (int i = 0; i + 1 < rawStart.Length && i / 2 < hashedStart.Length; i += 2)
            {
                hashedStart[i / 2] = Convert.ToChar(26 * (rawStartInteger[i] + rawStartInteger[i + 1]) + 65);
            }

            // Convert char array back to string and return hash
            return string.Join("", hashedStart);

        }
    }

    /// <summary>
    ///  Handles constants used for security that are XML based. (XML Writing, Reading, etc.)
    /// </summary>
    public static class SecurityXMLNames
    {
        public static string UserDatabaseObject
        {
            get { return "UserDatabase"; }
        }
        public static string UserObject
        {
            get { return "User"; }
        }
        public static string UsernameObject
        {
            get { return "Username"; }
        }
        public static string PasswordObject
        {
            get { return "Password"; }
        }
        public static string SaltObject
        {
            get { return "Salt"; }
        }
        public static string PrivilegeObject
        {
            get { return "Privilege"; }
        }
        public static string SettingsObject
        {
            get { return "Setting"; }
        }
        public static string GuestsCanView
        {
            get { return "GCV"; }
        }

        // Privilege names by default
        public static string XMLGuestUser
        {
            get { return "Guest"; }
        }
        public static string XMLAdminUser
        {
            get { return "Admin"; }
        }
    }
}
