/// <summary>
///  File: Drink.java
///     Description: Meant to contain the object representing a collection of DrinkTemplates
///         Provides drink templates to the frontend for alcohol drink logging
///         Provides drink templates to the backend for storage
///         Provides drinks to the frontend produced by contained drink templates for reporting
/// </summary>
package com.example.alcoholconsumptiontracker.system;

import android.content.Context;
import android.util.Log;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Scanner;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerConfigurationException;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.TransformerFactoryConfigurationError;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

///
/// Drink Template Class
///     Stores drink templates for the app
///
public class DrinkTemplateManager {

    ///
    /// Local variables
    ///
    // Represents a dictionary of templates.
    // Key = template name (String), value = template (DrinkTemplate)
    private HashMap<String, DrinkTemplate> templateHashMap;

    ///
    /// Constructors
    ///
    // Default. Initializes with no templates
    public DrinkTemplateManager(){
        this.templateHashMap = new HashMap<>();
    }
    // Initialize with templates from files

    ///
    /// Setters and Getters
    ///
    public HashMap<String, DrinkTemplate> GetTemplateList(){
        return this.templateHashMap;
    }

    ///
    /// Methods
    ///
    ///
    /// - Frontend
    ///

    /// <summary>
    /// Given a string representing the drink template key, a string representing
    ///     a drink occasion, an hour and minute representing time of day being drunk,
    ///     this method searches this object's templateHashMap for that template,
    ///     makes a drink from that template, then returns that drink. If this
    ///     method fails to do so, it returns null.
    /// </summary>
    public Drink ProduceDrink(String key, String occasion, short hour, short minute){

        DrinkTemplate temp = this.GetTemplate(key);
        if (temp != null) return temp.ProduceDrink(occasion, hour, minute);
        else return null;
    }

    ///
    /// - System
    ///

    /// <summary>
    /// Searches for a template within the template hashmap by key (name) and returns
    /// it if it exists. returns null otherwise.
    /// </summary>
    public DrinkTemplate GetTemplate(String key){
        // If the key isn't contained in the map, return null
        if (!this.templateHashMap.containsKey(key))  return null;

        // If the key is contained in the map, return that template
        //  *Template can be null
        return this.templateHashMap.get(key);
    }
    /// <summary>
    /// Puts a new template into the dictionary using its name as its key.
    /// Given an inputted template, this method checks to see if a template with the input's
    ///     same name (key) exists in the dictionary.
    ///     If it doesn't, enter the inputted template into the dictionary with its name being its key, return true
    ///     If it does, return false
    /// </summary>
    public boolean PutTemplate(DrinkTemplate newTemplate){
        if (this.templateHashMap.containsKey(newTemplate.GetName())) return false;
        this.templateHashMap.put(newTemplate.GetName(), newTemplate);
        return true;
    }
    /// <summary>
    /// Modifies an existing template by changing one value with another.
    /// Given an inputted template, this method checks to see if a template with the input's
    ///     same name (key) exists in the dictionary.
    ///     If it does, enter the inputted template into the dictionary with its name being its key, return true
    ///     If it doesn't, return false
    /// </summary>
    public boolean ModifyTemplate(DrinkTemplate newTemplateVersion){
        if (this.templateHashMap.containsKey(newTemplateVersion.GetName())) {
            this.templateHashMap.put(newTemplateVersion.GetName(), newTemplateVersion);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Deletes an existing template within the dictionary with a matching key.
    /// Given an inputted template key, this method checks to see if a template with the input's
    ///     same key exists in the dictionary.
    ///     If it does, delete the template, return true
    ///     If it doesn't, return false
    /// </summary>
    public boolean RemoveTemplate(String templateKey){
        if (this.templateHashMap.containsKey(templateKey)) {
            this.templateHashMap.remove(templateKey);
            return true;
        }
        return false;
    }

    /// <summary>
    ///  Given a key representing a template name, this function
    ///     returns whether the template manager has a template
    ///     with that key or not.
    ///     Returns
    ///     True if it contains a template with that key
    ///     False otherwise
    /// </summary>
    public boolean ContainsTemplate(String templateKey){
        return this.templateHashMap.containsKey(templateKey);
    }
    ///
    /// - Backend
    ///

    /// <summary>
    ///     Writes the contents of the DrinkTemplateManager to a directory.
    ///     targetDirectory: directory to store xml file
    ///     fileName: name of file to be created or overwritten and stored to.
    ///         name of file expected to NOT contain ".xml"
    ///     Given an inputted directory file and file name,
    ///     this method:
    ///         - Creates a new XML file within the targetDirectory
    ///             with the name of fileName
    ///         - Stores the contents of its templateDictionary within fileName
    ///     Returns true if successful in both steps.
    ///     Returns false otherwise.
    /// </summary>
    public boolean WriteTemplateList(File targetDirectory, String fileName)  {

        // Locals
        StreamResult targetStream;
        Element root;
        Element tempElement;
        Element tempElement2;
        DrinkTemplate tempTemplate;
        Iterator<Map.Entry<String, DrinkTemplate>> hashmapIterator = this.templateHashMap.entrySet().iterator();
        DocumentBuilderFactory f;
        DocumentBuilder b;
        Document d;
        TransformerFactory tf;
        Transformer t;
        DOMSource DOMDocument;
        File outputFile;

        // From the inputted directory file and name, attempt to
        //  create a file within that directory.
        //      -If target directory doesn't exist, return false
        if (!targetDirectory.exists()) return false;
        //      -If file within directory exists, set the output stream to that file
        //          Otherwise, create that file
        //      Set the output stream of this method to the result of either case
        try {
            outputFile = new File(targetDirectory.getAbsolutePath() + "/" + fileName + ".xml");
            if (outputFile.exists()) {
                targetStream = new StreamResult(new PrintWriter(new FileOutputStream(outputFile, false)));
            } else {
                if (!outputFile.createNewFile()) return false;
                targetStream = new StreamResult(new PrintWriter(new FileOutputStream(outputFile)));
            }
        }
        catch (FileNotFoundException e){
            // If file isn't found, file was deleted immediately after creation. Return false.
            Log.d(Universals.ErrorMessages.ErrorMessageTag,Universals.ErrorMessages.DrinkTemplateManagerErrorMessages.WriteTemplatesErrorFailedToCreateFile);
            return false;
        }
        catch (IOException e){
            // If IO error encountered, return false.
            Log.d(Universals.ErrorMessages.ErrorMessageTag,Universals.ErrorMessages.DrinkTemplateManagerErrorMessages.WriteTemplatesErrorFileNotFound);
            return false;
        }

        // Create document to write XML elements to
        try{
            f = DocumentBuilderFactory.newInstance();
            b = f.newDocumentBuilder();
            d = b.newDocument();
        } catch (ParserConfigurationException e) {
            // If error encountered, close output stream an return false.
            Log.d(Universals.ErrorMessages.ErrorMessageTag,Universals.ErrorMessages.DrinkTemplateManagerErrorMessages.WriteTemplatesErrorFailedToCreateDocument);
            try{
                targetStream.getOutputStream().close();
            } catch (IOException ex) {
                return false;
            }
            return false;
        }

        // Write header as root
        root = d.createElement(Universals.XMLTags.DrinkTemplateManagerTags.Header);
        d.appendChild(root);

        // For each template member, write its contents
        while (hashmapIterator.hasNext()){

            // Get current element
            tempTemplate = hashmapIterator.next().getValue();

            // Create template with following format:
            //  -Name
            //  -Servings
            //  -Type
            //  -APV
            //  -Calories
            //  -Price
            //  -ImageFilePath
            //

            // Header
            tempElement = d.createElement(Universals.XMLTags.DrinkTemplateTags.Header);

            // Name
            tempElement2 = d.createElement(Universals.XMLTags.DrinkTemplateTags.Name);
            tempElement2.appendChild(d.createTextNode(tempTemplate.GetName()));
            tempElement.appendChild(tempElement2);

            // Servings
            tempElement2 = d.createElement(Universals.XMLTags.DrinkTemplateTags.Servings);
            tempElement2.appendChild(d.createTextNode(String.valueOf(tempTemplate.GetServings())));
            tempElement.appendChild(tempElement2);

            // Type
            tempElement2 = d.createElement(Universals.XMLTags.DrinkTemplateTags.Type);
            tempElement2.appendChild(d.createTextNode(String.valueOf(tempTemplate.GetType().GetValue())));
            tempElement.appendChild(tempElement2);

            // APV
            tempElement2 = d.createElement(Universals.XMLTags.DrinkTemplateTags.APV);
            tempElement2.appendChild(d.createTextNode(String.valueOf(tempTemplate.GetAPV())));
            tempElement.appendChild(tempElement2);

            // Calories
            tempElement2 = d.createElement(Universals.XMLTags.DrinkTemplateTags.Calories);
            tempElement2.appendChild(d.createTextNode(String.valueOf(tempTemplate.GetCalories())));
            tempElement.appendChild(tempElement2);

            // Price
            tempElement2 = d.createElement(Universals.XMLTags.DrinkTemplateTags.Price);
            tempElement2.appendChild(d.createTextNode(String.valueOf(tempTemplate.GetPrice())));
            tempElement.appendChild(tempElement2);

            // Image File Path
            tempElement2 = d.createElement(Universals.XMLTags.DrinkTemplateTags.ImageFilePath);
            tempElement2.appendChild(d.createTextNode(tempTemplate.GetImageFilePath()));
            tempElement.appendChild(tempElement2);

            // Append onto root
            root.appendChild(tempElement);
        }

        // Write to file via transformer
        try{
            tf = TransformerFactory.newInstance();
            t = tf.newTransformer();
            DOMDocument = new DOMSource(d);
            t.transform(DOMDocument, targetStream);

        } catch (TransformerFactoryConfigurationError | TransformerException e) {
            // If error encountered, close output stream an return false.
            Log.d(Universals.ErrorMessages.ErrorMessageTag,Universals.ErrorMessages.DrinkTemplateManagerErrorMessages.WriteTemplatesErrorTransformerError);
            try{
                targetStream.getOutputStream().close();
            } catch (IOException ex) {
                return false;
            }
            return false;
        }

        // Close stream when finished. Verify file exists and is unlocked before returning true
        if (outputFile.exists()) return true;
        return false;
    }

    /// <summary>
    ///     Reads the contents of the DrinkTemplateManager from a file within a directory.
    ///     targetDirectory: directory to read xml file from
    ///     targetFileName: name of file to be read from.
    ///         name of file expected to NOT contain ".xml"
    ///     append: Determines whether the contents found in the file will override the
    ///         current contents of the DrinkTemplateManager or will append onto existing
    ///         dictionary members. If a Template is found with a new name, it is added
    ///         to the dictionary. If a Template is found with an existing name, it doesn't
    ///         modify the existing template and doesn't add its contents.
    ///     Given an inputted directory file and file name,
    ///     this method:
    ///         - Verifies the file's existence within the inputted directory
    ///         - Attempts to read the contents of the XML file as if it were
    ///             formatted as a DrinkTemplateManager object.
    ///     Returns true if successful in both steps.
    ///     Returns false otherwise.
    ///
    ///     *Note if while parsing for templates an invalid template is found,
    ///     expected behavior is to not add that template to the manager. An invalid
    ///     template is a template with at least 1 field that could not be parsed.
    /// </summary>
    public boolean ReadTemplateList(File targetDirectory, String targetFileName, boolean append){

        // Locals
        DocumentBuilderFactory f;
        DocumentBuilder b;
        Document d;
        FileInputStream inputFileStream;
        File inputFile;
        Element root;
        NodeList templatesRaw;
        DrinkTemplate tempDrinkTemplate;
        Node tempDrinkTemplateRaw;
        NodeList tempDrinkTemplateRawFields;
        Node rawField;
        Element rawFieldAsElement;
        boolean invalidTemplate = false;
        ArrayList<DrinkTemplate> holderList = new ArrayList<DrinkTemplate>();

        // Verify file's existence
        //  -Verify directory's existence
        if (!targetDirectory.exists()){
            Log.d(
              Universals.ErrorMessages.ErrorMessageTag,
              Universals.ErrorMessages.DrinkTemplateManagerErrorMessages.ReadTemplatesErrorDirectoryNotFound
            );
            return false;
        }
        else if(!targetDirectory.isDirectory()){
            Log.d(
                    Universals.ErrorMessages.ErrorMessageTag,
                    Universals.ErrorMessages.DrinkTemplateManagerErrorMessages.ReadTemplatesErrorDirectoryNotFound
            );
            return false;
        }
        //  -Verify file's existence
        inputFile = new File(targetDirectory + "/" + targetFileName + ".xml");
        if (!inputFile.exists()){
            Log.d(
                    Universals.ErrorMessages.ErrorMessageTag,
                    Universals.ErrorMessages.DrinkTemplateManagerErrorMessages.ReadTemplatesErrorFileNotFound
            );
            return false;
        }
        else if (!inputFile.isFile()){
            Log.d(
                    Universals.ErrorMessages.ErrorMessageTag,
                    Universals.ErrorMessages.DrinkTemplateManagerErrorMessages.ReadTemplatesErrorFileNotFound
            );
            return false;
        }

        try{
            inputFileStream = new FileInputStream(inputFile.getAbsolutePath());
        } catch (FileNotFoundException e) {
            Log.d(
                    Universals.ErrorMessages.ErrorMessageTag,
                    Universals.ErrorMessages.DrinkTemplateManagerErrorMessages.ReadTemplatesErrorFileNotFound
            );
            return false;
        }

        // Set up DOM Document parser
        f = DocumentBuilderFactory.newInstance();
        try{
            b = f.newDocumentBuilder();
        }
        catch (ParserConfigurationException e){
            Log.d(
                    Universals.ErrorMessages.ErrorMessageTag,
                    Universals.ErrorMessages.DrinkTemplateManagerErrorMessages.ReadTemplatesErrorFailedToCreateDocument
            );
            return false;
        }
        // Read file's contents into the document
        try{
            d = b.parse(new InputSource(inputFileStream));
        }
        catch (IOException e){
            Log.d(
                    Universals.ErrorMessages.ErrorMessageTag,
                    Universals.ErrorMessages.DrinkTemplateManagerErrorMessages.ReadTemplatesErrorFileIOError
            );
            return false;
        }
        catch (SAXException e) {
            Log.d(
                    Universals.ErrorMessages.ErrorMessageTag,
                    Universals.ErrorMessages.DrinkTemplateManagerErrorMessages.ReadTemplatesErrorFileParseError
            );
            try{
                inputFileStream.close();
            }
            catch (IOException ex){
                return false;
            }
            return false;
        }

        // Attempt to close the input stream as it is no longer needed.
        try{
            inputFileStream.close();
        }
        catch (IOException ignored){
        }



        // Convert document content into DrinkTemplates to store in DrinkTemplateManager.
        // Verify DrinkTemplateManager Header as root
        root = d.getDocumentElement();
        root.normalize();
        if (!root.getTagName().equals(Universals.XMLTags.DrinkTemplateManagerTags.Header)){
            Log.d(
                    Universals.ErrorMessages.ErrorMessageTag,
                    Universals.ErrorMessages.DrinkTemplateManagerErrorMessages.ReadTemplatesErrorInvalidXMLFile
            );
            return false;
        }

        // Parse contents of root. Add to a temporary list for holding.
        templatesRaw = root.getElementsByTagName(Universals.XMLTags.DrinkTemplateTags.Header);
        for (int i = 0; i < templatesRaw.getLength(); i++){

            // Construct a DrinkTemplate from elements within the current raw template
            tempDrinkTemplate = new DrinkTemplate();
            tempDrinkTemplateRaw = templatesRaw.item(i);
            tempDrinkTemplateRawFields = tempDrinkTemplateRaw.getChildNodes();

            // Represents whether the template is valid or not. If a value parses incorrectly, this
            //  is set to false. If at the end of parsing if invalidTemplate is false, the template
            //  isn't invalid and is added to the holding list. Otherwise it is ignored.
            invalidTemplate = false;

            for (int j = 0; j < tempDrinkTemplateRawFields.getLength() && !invalidTemplate; j++){

                rawField = tempDrinkTemplateRawFields.item(j);

                // Name
                if (rawField.getNodeName().equals(Universals.XMLTags.DrinkTemplateTags.Name)){
                    tempDrinkTemplate.SetName(rawField.getTextContent());
                }
                // Servings
                else if (rawField.getNodeName().equals(Universals.XMLTags.DrinkTemplateTags.Servings)){
                    try{
                        tempDrinkTemplate.SetServings(Short.parseShort(rawField.getTextContent()));
                    }
                    catch (NumberFormatException ignored){
                        invalidTemplate = true;
                    }
                }
                // Type
                else if (rawField.getNodeName().equals(Universals.XMLTags.DrinkTemplateTags.Type)){
                    try{
                        tempDrinkTemplate.SetType(Short.parseShort(rawField.getTextContent()));
                    }
                    catch (NumberFormatException ignored){
                        invalidTemplate = true;
                    }
                }
                // APV
                else if (rawField.getNodeName().equals(Universals.XMLTags.DrinkTemplateTags.APV)){
                    try{
                        tempDrinkTemplate.SetAPV(Float.parseFloat(rawField.getTextContent()));
                    }
                    catch (NumberFormatException ignored){
                        invalidTemplate = true;
                    }
                }
                // Calories
                else if (rawField.getNodeName().equals(Universals.XMLTags.DrinkTemplateTags.Calories)){
                    try{
                        tempDrinkTemplate.SetCalories(Float.parseFloat(rawField.getTextContent()));
                    }
                    catch (NumberFormatException ignored){
                        invalidTemplate = true;
                    }
                }
                // Price
                else if (rawField.getNodeName().equals(Universals.XMLTags.DrinkTemplateTags.Price)){
                    try{
                        tempDrinkTemplate.SetPrice(Float.parseFloat(rawField.getTextContent()));
                    }
                    catch (NumberFormatException ignored){
                        invalidTemplate = true;
                    }
                }
                // ImageFilePath
                else if (rawField.getNodeName().equals(Universals.XMLTags.DrinkTemplateTags.ImageFilePath)){
                    tempDrinkTemplate.SetImageFilePath(rawField.getTextContent());
                }
                // Invalid tag encountered in else statement. Set invalid template to true
                else{
                    invalidTemplate = true;
                }
            }

            // If the template is valid, add it to the holding list
            if (!invalidTemplate){
                holderList.add(tempDrinkTemplate);
            }
        }

        // If append is set to false, empty contents of DrinkTemplate HashMap
        //  Then add the contents of the holding list to the DrinkTemplateManager
        if (!append){
            this.templateHashMap.clear();
        }
        // Add contents of the holder list to the DrinkTemplateManager
        for (int i = 0; i < holderList.size(); i++){
            this.PutTemplate(holderList.get(i));
        }

        // Return true when finished
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

    // Test Put Template System method
    public static void TestPutTemplate(boolean printAllMessages){

        // Test Locals
        DrinkTemplateManager testManager = new DrinkTemplateManager();
        DrinkTemplate testTemplate = new DrinkTemplate();
        String testTemplateName = "test name ";
        short testShort = 0;

        // Non-exception cases
        //  -Case 1, normal values
        for (testShort = 0; testShort < 10; testShort++){
            testTemplate.SetName(testTemplateName + testShort);
            testManager.PutTemplate(testTemplate);
        }
        for (testShort = 0; testShort < 10; testShort++){
            if (testManager.GetTemplate(testTemplateName + testShort) == null) {
                testShort = 11;
                Log.d(
                        Universals.TestMessages.TestMessageTag,
                        Universals.TestMessages.DrinkTemplateManagerMessages.TemplatePutMessage(false, 1));
            }
        }
        if (testShort == 10 && printAllMessages){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkTemplateManagerMessages.TemplatePutMessage(true, 1));
        }

        // -Case 2, put template while key of template exists within dictionary (expect false return)
        testManager = new DrinkTemplateManager();
        for (testShort = 0; testShort < 10; testShort++){
            testTemplate.SetName(testTemplateName + testShort);
            testManager.PutTemplate(testTemplate);
        }
        for (testShort = 0; testShort < 10; testShort++){
            testTemplate.SetName(testTemplateName + testShort);
            if (testManager.PutTemplate(testTemplate)) {
                testShort = 11;
                Log.d(
                        Universals.TestMessages.TestMessageTag,
                        Universals.TestMessages.DrinkTemplateManagerMessages.TemplatePutMessage(false, 12));
            }
        }
        if (testShort == 10 && printAllMessages){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkTemplateManagerMessages.TemplatePutMessage(true, 2));
        }
    }
    // Test Modify Template System method
    public static void TestModifyTemplate(boolean printAllMessages){

        // Test Locals
        DrinkTemplateManager testManager = new DrinkTemplateManager();
        DrinkTemplate testTemplate = new DrinkTemplate();
        String testTemplateName = "test name ";
        short testShort = 0;

        // Non-exception cases
        //  -Case 1, normal values
        testTemplate.SetServings((short)1);
        for (testShort = 0; testShort < 10; testShort++){
            testTemplate.SetName(testTemplateName + testShort);
            testManager.PutTemplate(testTemplate);
        }
        for (testShort = 0; testShort < 10; testShort++){
            testTemplate.SetName(testTemplateName + testShort);
            testTemplate.SetServings((short)10);
            if (!testManager.ModifyTemplate(testTemplate)) {
                testShort = 11;
                Log.d(
                        Universals.TestMessages.TestMessageTag,
                        Universals.TestMessages.DrinkTemplateManagerMessages.TemplateModifyMessage(false, 1));
            }
            else if (testManager.GetTemplate(testTemplate.GetName()).GetServings() != 10){
                testShort = 11;
                Log.d(
                        Universals.TestMessages.TestMessageTag,
                        Universals.TestMessages.DrinkTemplateManagerMessages.TemplateModifyMessage(false, 1));
            }
        }
        if (testShort == 10 && printAllMessages){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkTemplateManagerMessages.TemplateModifyMessage(true, 1));
        }

        // -Case 2, modify a non-existent template (expected false return)
        testManager = new DrinkTemplateManager();
        for (testShort = 0; testShort < 10; testShort++){
            testTemplate.SetName(testTemplateName + testShort);
            if (testManager.ModifyTemplate(testTemplate)) {
                testShort = 11;
                Log.d(
                        Universals.TestMessages.TestMessageTag,
                        Universals.TestMessages.DrinkTemplateManagerMessages.TemplateModifyMessage(false, 2));
            }
        }
        if (testShort == 10 && printAllMessages){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkTemplateManagerMessages.TemplateModifyMessage(true, 2));
        }
    }

    // Test Remove Template System Method
    public static void TestRemoveTemplate(boolean printAllMessages){
        // Test Locals
        DrinkTemplateManager testManager = new DrinkTemplateManager();
        DrinkTemplate testTemplate = new DrinkTemplate();
        String testTemplateName = "test name ";
        short testShort = 0;

        // Non-exception cases
        //  -Case 1, normal values
        for (testShort = 0; testShort < 10; testShort++){
            testTemplate.SetName(testTemplateName + testShort);
            testManager.PutTemplate(testTemplate);
        }
        for (testShort = 0; testShort < 10; testShort++){
            testTemplate.SetName(testTemplateName + testShort);
            if (!testManager.RemoveTemplate(testTemplate.GetName())) {
                testShort = 11;
                Log.d(
                        Universals.TestMessages.TestMessageTag,
                        Universals.TestMessages.DrinkTemplateManagerMessages.TemplateRemoveMessage(false, 1));
            }
        }
        if (testShort == 10 && printAllMessages){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkTemplateManagerMessages.TemplateRemoveMessage(true, 1));
        }

        // -Case 2, remove a non-existent template (expected false return)
        testManager = new DrinkTemplateManager();
        for (testShort = 0; testShort < 10; testShort++){
            testTemplate.SetName(testTemplateName + testShort);
            if (testManager.RemoveTemplate(testTemplate.GetName())) {
                testShort = 11;
                Log.d(
                        Universals.TestMessages.TestMessageTag,
                        Universals.TestMessages.DrinkTemplateManagerMessages.TemplateRemoveMessage(false, 2));
            }
        }
        if (testShort == 10 && printAllMessages){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkTemplateManagerMessages.TemplateRemoveMessage(true, 2));
        }
    }

    // Test WriteTemplateList Backend Method
    public static void TestReadWriteTemplateList(boolean printAllMessages, Context context) {

        // Locals
        DatabaseManager dbm = new DatabaseManager(context);
        DrinkTemplateManager testManager;
        DrinkTemplate testTemplate;
        int case3TemplatesGenerated = 100;
        int i;

        // Non-exception cases
        //  -Case 1, Write. store to app root directory. No read verification
        testManager = new DrinkTemplateManager();
        testTemplate = new DrinkTemplate();
        testTemplate.SetName("testName");
        testManager.PutTemplate(testTemplate);
        if (testManager.WriteTemplateList(dbm.GetAppRootDirectory(),"testDrinkTemplateFile")) {
            if (printAllMessages)
                Log.d(
                        Universals.TestMessages.TestMessageTag,
                        Universals.TestMessages.DrinkTemplateManagerMessages.TemplateReadWriteTemplateListMessage(true, 1)
                );
        }
        else {
                Log.d(
                        Universals.TestMessages.TestMessageTag,
                        Universals.TestMessages.DrinkTemplateManagerMessages.TemplateReadWriteTemplateListMessage(false, 1)
                );
        }
        //  -Case 2, Read Write. Store to app root directory. Read verification without append of written content (small)
        testManager = new DrinkTemplateManager();
        testTemplate = new DrinkTemplate();
        testTemplate.SetName("testName");
        testManager.PutTemplate(testTemplate);
        if (testManager.WriteTemplateList(dbm.GetAppRootDirectory(),"testDrinkTemplateFile2")) {
            testManager.ReadTemplateList(dbm.GetAppRootDirectory(), "testDrinkTemplateFile2", false);
            if (testManager.GetTemplate("testName") != null) {
                if (printAllMessages)
                    Log.d(
                            Universals.TestMessages.TestMessageTag,
                            Universals.TestMessages.DrinkTemplateManagerMessages.TemplateReadWriteTemplateListMessage(true, 2)
                    );
            } else {
                Log.d(
                        Universals.TestMessages.TestMessageTag,
                        Universals.TestMessages.DrinkTemplateManagerMessages.TemplateReadWriteTemplateListMessage(false, 2)
                );
            }
        }
        else {
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkTemplateManagerMessages.TemplateReadWriteTemplateListMessage(false, 2)
            );
        }

        //  -Case 3. Read Write. Store to app root directory. Read verification without append of written content (large)
        testManager = new DrinkTemplateManager();
        testTemplate = new DrinkTemplate();
        testTemplate.SetName("testName");
        testTemplate.SetPrice((float)1.1);
        testTemplate.SetAPV((float)1.1);
        testTemplate.SetCalories((float)1.1);
        testTemplate.SetServings((short)1);
        testTemplate.SetImageFilePath("testPath");
        testTemplate.SetType((short)1);
        for ( i = 0; i < case3TemplatesGenerated; i++){
            testTemplate.SetName("testName " + String.valueOf(i));
            testManager.PutTemplate(testTemplate);
        }
        if (testManager.WriteTemplateList(dbm.GetAppRootDirectory(),"testDrinkTemplateFile3")){
            testManager.ReadTemplateList(dbm.GetAppRootDirectory(), "testDrinkTemplateFile3", false);
            for ( i = 0; i < case3TemplatesGenerated; i++){
                testTemplate = testManager.GetTemplate("testName " + String.valueOf(i));
                if (testTemplate == null){
                    i = case3TemplatesGenerated + 1;
                }
                else{
                    if (
                            !(testTemplate.GetName().equals("testName " + String.valueOf(i)) &&
                            testTemplate.GetServings() == 1 &&
                            testTemplate.GetAPV() == 1.1 &&
                            testTemplate.GetCalories() == 1.1 &&
                            testTemplate.GetType().GetValue() == (short)1 &&
                            testTemplate.GetPrice() == 1.1 &&
                            testTemplate.GetImageFilePath().equals("testPath")      )
                    ) i = case3TemplatesGenerated + 1;
                }
            }
            if (i == case3TemplatesGenerated && printAllMessages){
                Log.d(
                        Universals.TestMessages.TestMessageTag,
                        Universals.TestMessages.DrinkTemplateManagerMessages.TemplateReadWriteTemplateListMessage(true, 3)
                );
            }
            else if (i == case3TemplatesGenerated + 1){
                Log.d(
                        Universals.TestMessages.TestMessageTag,
                        Universals.TestMessages.DrinkTemplateManagerMessages.TemplateReadWriteTemplateListMessage(false, 3)
                );
            }
        }

        //  -Case 4. Read Write. Store to app root directory. Read verification with append of written content (large)
        testManager = new DrinkTemplateManager();
        testTemplate = new DrinkTemplate();
        testTemplate.SetName("testName");
        testTemplate.SetPrice((float)1.1);
        testTemplate.SetAPV((float)1.1);
        testTemplate.SetCalories((float)1.1);
        testTemplate.SetServings((short)1);
        testTemplate.SetImageFilePath("testPath");
        testTemplate.SetType((short)1);
        for ( i = 0; i < case3TemplatesGenerated; i++){
            testTemplate.SetName("testName " + String.valueOf(i));
            testManager.PutTemplate(testTemplate);
        }
        if (testManager.WriteTemplateList(dbm.GetAppRootDirectory(),"testDrinkTemplateFile4")){
            testManager.ReadTemplateList(dbm.GetAppRootDirectory(), "testDrinkTemplateFile4", true);
            for ( i = 0; i < case3TemplatesGenerated; i++){
                testTemplate = testManager.GetTemplate("testName " + String.valueOf(i));
                if (testTemplate != null){
                    i = case3TemplatesGenerated + 1;
                }
            }
            if (i == case3TemplatesGenerated && printAllMessages){
                Log.d(
                        Universals.TestMessages.TestMessageTag,
                        Universals.TestMessages.DrinkTemplateManagerMessages.TemplateReadWriteTemplateListMessage(true, 4)
                );
            }
            else if (i == case3TemplatesGenerated + 1){
                Log.d(
                        Universals.TestMessages.TestMessageTag,
                        Universals.TestMessages.DrinkTemplateManagerMessages.TemplateReadWriteTemplateListMessage(false, 4)
                );
            }
        }


        // Exception cases

    }

}
