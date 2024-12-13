/// <summary>
///  File: DatabaseManager.java
///     Description: Contains the class and methods of setting up and providing access information
///     to the internal storage backend of this app.
/// </summary>
package com.example.alcoholconsumptiontracker.system;

import android.content.Context;
import android.content.ContextWrapper;
import android.os.Environment;
import android.util.Log;

import java.io.File;

public class DatabaseManager {

    ///
    /// Locals
    ///
    // Represents whether the database is initialized or not for backend use.
    private boolean initialized;
    // The internal storage root directory and name
    private File appRootDirectory;
    private static String appRootDirectoryName = "appDir";

    // The internal storage image directory and name
    private File imageDirectory;
    private static String imageDirectoryName = "imgDir";

    /// <summary>
    ///  Constructor. Initializes the database manager by
    ///     -Initializing the appRootDirectory and maintaining its folderPath
    ///     -Initializing other directories and maintaining their folderPaths (within internal storage)
    ///  If successfully initialized, initialized is set to true. Otherwise, it is set to false.
    ///     *database access isn't possible with this class if uninitialized.
    /// </summary>
    public DatabaseManager(Context context){
        this.initialized = this.InitializeDatabase(context);
    }

    ///
    /// Getters and Setters
    ///

    /// <summary>
    ///     Gets the app root directory.
    ///     If initialized, returns directory
    ///     Returns null otherwise.
    /// </summary>
    public File GetAppRootDirectory() {
        if (initialized)
            return appRootDirectory;
        else
            return null;
    }
    /// <summary>
    ///     Gets the app image directory.
    ///     If initialized, returns directory
    ///     Returns null otherwise.
    /// </summary>
    public File GetImageDirectory(){
        if (initialized)
            return imageDirectory;
        else
            return null;
    }
    public boolean Initialized(){
        return this.initialized;
    }

    ///
    /// Methods
    ///

    /// <summary>
    ///  Given an inputted context, this method initializes the database of the app
    ///     within the given context.
    ///     If initialized successfully, set initialized to true
    ///     Otherwise, set initialized to false.
    ///     Returns the initialized status of the database manager when complete.
    /// </summary>
    public boolean InitializeDatabase(Context context){

        try{
            File rootDir = context.getFilesDir();

            // Verify app directory's existence or create it.
            File appDir = new File(rootDir.getAbsolutePath() + "/" + appRootDirectoryName);
            if (appDir.exists()){
                if (appDir.isDirectory()){
                    this.appRootDirectory = appDir;
                }
                else{
                    if (appDir.mkdir()){
                        this.appRootDirectory = appDir;
                    }
                    else{
                        Log.d(Universals.ErrorMessages.ErrorMessageTag,Universals.ErrorMessages.DatabaseManagerErrorMessages.InitializeDatabaseSecurityError);
                        this.initialized = false;
                        return false;
                    }
                }
            }
            else{
                if (appDir.mkdir()){
                    this.appRootDirectory = appDir;
                }
                else{
                    Log.d(Universals.ErrorMessages.ErrorMessageTag,Universals.ErrorMessages.DatabaseManagerErrorMessages.InitializeDatabaseSecurityError);
                    this.initialized = false;
                    return false;
                }
            }

            // Verify image directory's existence or create it
            File imgDir = new File(appDir.getAbsolutePath() + "/" + imageDirectoryName);
            if (imgDir.exists()){
                if (imgDir.isDirectory()){
                    this.imageDirectory = imgDir;
                }
                else{
                    if (imgDir.mkdir()){
                        this.imageDirectory = imgDir;
                    }
                    else{
                        Log.d(Universals.ErrorMessages.ErrorMessageTag,Universals.ErrorMessages.DatabaseManagerErrorMessages.InitializeDatabaseSecurityError);
                        this.initialized = false;
                        return false;
                    }
                }
            }
            else{
                if (imgDir.mkdir()){
                    this.imageDirectory = imgDir;
                }
                else{
                    Log.d(Universals.ErrorMessages.ErrorMessageTag,Universals.ErrorMessages.DatabaseManagerErrorMessages.InitializeDatabaseSecurityError);
                    this.initialized = false;
                    return false;
                }
            }

            // Set initialized to true once complete
            this.initialized = true;

        } catch (SecurityException e) {
            // If an error was encountered, set initialized to false
            Log.d(Universals.ErrorMessages.ErrorMessageTag,Universals.ErrorMessages.DatabaseManagerErrorMessages.InitializeDatabaseSecurityError);
            this.initialized = false;
            return false;
        }

        return true;
    }


    ///
    /// Test Methods
    ///
    /// <summary>
    ///  Each test method is self contained and runs different scenarios based on the
    ///     method associated with that method. Results of tests are printed to LogCat using
    ///     Log.d method
    ///     Cases are split into two categories: Test non-exception and test exception
    ///         Test non-exception tests normal use of methods
    ///         Test exception test methods throwing exceptions when they should be
    ///     Additionally, tests can be set to only print failure messages or all messages.
    /// </summary>
    ///
    // Test Initialize Database Method
    public static void TestInitializeDatabase(boolean printAllMessages, Context testContext){

        // Locals

        // Non-exception case
        //  Case 1, Initialize and expect success. Check for initialized Variable
        DatabaseManager testManager = new DatabaseManager(testContext);
        if (!testManager.initialized){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DatabaseManagerMessages.DatabaseInitializeMessage(false, 1));
        }
        else if (printAllMessages){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DatabaseManagerMessages.DatabaseInitializeMessage(true, 1));
        }

        //  Case 2, Initialize and expect success. Check for initialized files.
        testManager = new DatabaseManager(testContext);
        if (!testManager.GetAppRootDirectory().exists() || !testManager.GetImageDirectory().exists()){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DatabaseManagerMessages.DatabaseInitializeMessage(false, 2));
        }
        else if (printAllMessages){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DatabaseManagerMessages.DatabaseInitializeMessage(true, 1));
        }

    }
}
