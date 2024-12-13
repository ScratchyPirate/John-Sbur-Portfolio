package com.example.alcoholconsumptiontracker;

import static android.app.Activity.RESULT_OK;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Bundle;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.PickVisualMediaRequest;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.fragment.app.Fragment;

import android.provider.MediaStore;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.example.alcoholconsumptiontracker.system.DrinkTemplate;
import com.example.alcoholconsumptiontracker.system.DrinkType;
import com.example.alcoholconsumptiontracker.system.Universals;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;

/**
 * A simple {@link Fragment} subclass.
 * Use the {@link Alc_Create_Edit#newInstance} factory method to
 * create an instance of this fragment.
 */
public class Alc_Create_Edit extends Fragment {


    ///
    ///  Globals
    ///
    // Represents the text title for the fragment
    private static TextView templateEditingTitle;
    // Represents the template being edited
    private static DrinkTemplate templateEditing;

    // Represents the template image of the template
    private static ImageView templateImage;

    // Represents the template image change button
    private static ImageButton templateChangeImageButton;

    // Represents the template name text entry
    private static EditText templateNameTextbox;
    // Represents the original name of the template before editing
    private static String originalTemplateName;

    // Represents the template servings text entry
    private static EditText templateServingsTextbox;

    // Represents the template type text entry
    private static AutoCompleteTextView templateTypeAutoTextbox;

    // Represents the template calories text entry
    private static EditText templateCaloriesTextbox;

    // Represents the template price text entry
    private static EditText templatePriceTextbox;

    // Represents the finish editing button
    private static ImageButton alcEditFinishEditingButton;

    // Represents the cancel button
    private static ImageButton alcEditCancelEditButton;

    // Represents the current alc_edit_create fragment
    private static Alc_Create_Edit currentFragment;

    private static String temp;

    public Alc_Create_Edit() {
        // Required empty public constructor
    }

    public static Alc_Create_Edit newInstance(String param1, String param2) {
        Alc_Create_Edit fragment = new Alc_Create_Edit();
        Bundle args = new Bundle();
        fragment.setArguments(args);
        return fragment;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View root = inflater.inflate(R.layout.fragment_alc__create_edit, container, false);

        // Reset globals to null
        Alc_Create_Edit.templateEditingTitle = null;
        Alc_Create_Edit.templateEditing = null;
        Alc_Create_Edit.templateImage = null;
        Alc_Create_Edit.templateChangeImageButton = null;
        Alc_Create_Edit.templateNameTextbox = null;
        Alc_Create_Edit.templateServingsTextbox = null;
        Alc_Create_Edit.templateTypeAutoTextbox = null;
        Alc_Create_Edit.templateCaloriesTextbox = null;
        Alc_Create_Edit.templatePriceTextbox = null;
        Alc_Create_Edit.alcEditFinishEditingButton = null;
        Alc_Create_Edit.alcEditCancelEditButton = null;
        Alc_Create_Edit.temp = "";

        // Set the current fragment to this
        Alc_Create_Edit.currentFragment = this;

        // Set up drink template (pull from alc programming)
        //      -If editing an existing template, load that template
        if (Alc_Programming.GetProgrammingMode() == Alc_Programming.ProgrammingMode.EDITING){


            Alc_Create_Edit.templateEditing = Alc_Programming.GetSelectedTemplate();

        }
        //      -If creating a new template, create a new template to populate the fragment with
        else if (Alc_Programming.GetProgrammingMode() == Alc_Programming.ProgrammingMode.CREATING){

            DrinkTemplate newTemplate = new DrinkTemplate();
            // Set the name such that it isn't repeating
            //  Name in format as "drink template X" where X is an integer greater than 0
            String newTemplateName = "drink template";
            int i;
            for (i = 1; MainActivity.GetDrinkTemplateManager().ContainsTemplate(newTemplateName+ " " + String.valueOf(i)); i++);
            newTemplate.SetName(newTemplateName);
            newTemplate.SetServings((short)1);
            Alc_Create_Edit.templateEditing = newTemplate;
        }
        // Save the original name of the template;
        // If no mode is selected, return to alcohol programming
        else{
            MainActivity.ChangeActiveFragment(R.id.alc_Programming, MainActivity.FragmentAnimationType.NONE);
        }
        Alc_Create_Edit.originalTemplateName = Alc_Create_Edit.templateEditing.GetName();

        // set up the template title based on the mode
        String title = "";
        if (Alc_Programming.GetProgrammingMode() == Alc_Programming.ProgrammingMode.EDITING){
            title = "Modify Template";
        }
        else if (Alc_Programming.GetProgrammingMode() == Alc_Programming.ProgrammingMode.CREATING) {
            title = "Create a Template";
        }
        Alc_Create_Edit.templateEditingTitle = root.findViewById(R.id.alcEditTitle);
        Alc_Create_Edit.templateEditingTitle.setText(title);

        // Set up template name textbox
        Alc_Create_Edit.templateNameTextbox = root.findViewById(R.id.alcEditName);
        Alc_Create_Edit.templateNameTextbox.setText(String.valueOf(Alc_Create_Edit.GetEditingTemplate().GetName()));

        // Set up template servings textbox
        Alc_Create_Edit.templateServingsTextbox = root.findViewById(R.id.alcEditServings);
        Alc_Create_Edit.templateServingsTextbox.setText(String.valueOf(Alc_Create_Edit.GetEditingTemplate().GetServings()));

        // Set up template calories textbox
        Alc_Create_Edit.templateCaloriesTextbox = root.findViewById(R.id.alcEditCalories);
        Alc_Create_Edit.templateCaloriesTextbox.setText(String.valueOf(Alc_Create_Edit.GetEditingTemplate().GetCalories()));

        // Set up template price textbox
        Alc_Create_Edit.templatePriceTextbox = root.findViewById(R.id.alcEditPrice);
        Alc_Create_Edit.templatePriceTextbox.setText(String.valueOf(Alc_Create_Edit.GetEditingTemplate().GetPrice()));

        // Set up template type box
        Alc_Create_Edit.templateTypeAutoTextbox = root.findViewById(R.id.alcEditTypeInputTextbox);
        Alc_Create_Edit.templateTypeAutoTextbox.setText(Alc_Create_Edit.templateEditing.GetType().Get());
        String[] drinkTypes = DrinkType.DrinkTypeNames();
        ArrayAdapter adapter = new ArrayAdapter(
                MainActivity.GetContentView().getContext(),
                R.layout.alc_edit_create_dropdown_type,
                drinkTypes);
        Alc_Create_Edit.templateTypeAutoTextbox.setAdapter(adapter);


        // Set up back button
        Alc_Create_Edit.alcEditCancelEditButton = root.findViewById(R.id.alcEditBackButton);
        Alc_Create_Edit.alcEditCancelEditButton.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        // Go to previous fragment without saving changes
                        MainActivity.ChangeActiveFragment(R.id.alc_Programming, MainActivity.FragmentAnimationType.NONE);
                    }
                }
        );

        // Set up finish button
        Alc_Create_Edit.alcEditFinishEditingButton = root.findViewById(R.id.alcEditFinishButton);
        Alc_Create_Edit.alcEditFinishEditingButton.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {

                        // Locals
                        DrinkTemplate newTemplate = Alc_Create_Edit.GetEditingTemplate();
                        float newPrice = 0;
                        String newName = "";
                        float newCalories = 0;
                        String newType = "";
                        short newServings = 0;

                        // Check to make sure template values are valid. If not, return. If so, proceed
                        //  Name
                        EditText nameNextBox = (EditText) Alc_Create_Edit.GetTemplateNameTextbox();
                        newName = String.valueOf(nameNextBox.getText());

                        // If the name is blank, notify that the name cannot be blank.
                        if(newName.isEmpty()){
                            Toast.makeText(
                                    MainActivity.GetContentView().getContext(),
                                    "Template name cannot be blank",
                                    Toast.LENGTH_LONG
                            ).show();
                            nameNextBox.setBackgroundColor(getResources().getColor(R.color.drink_template_selected) );
                            return;
                        }

                        // If the name is contained within another template and isn't the same name as before editing, return and notify
                        //  templates cannot repeat names
                        if (MainActivity.GetDrinkTemplateManager().ContainsTemplate(newName)){
                            if ((Alc_Programming.GetProgrammingMode() == Alc_Programming.ProgrammingMode.EDITING && !newName.equals(Alc_Create_Edit.OriginalEditingTemplateName())) ||
                                    Alc_Programming.GetProgrammingMode() == Alc_Programming.ProgrammingMode.CREATING){
                                Toast.makeText(
                                        MainActivity.GetContentView().getContext(),
                                        "Template name already exists",
                                        Toast.LENGTH_LONG
                                ).show();
                                nameNextBox.setBackgroundColor(getResources().getColor(R.color.drink_template_selected) );
                                return;
                            }
                        }


                        // If the name isn't contained and not a repeat name, change the name
                        //  of the template.
                        newTemplate.SetName(newName);
                        nameNextBox.setBackgroundColor(getResources().getColor(R.color.white) );


                        //  Servings
                        EditText servingsTextbox = (EditText) Alc_Create_Edit.GetTemplateServingsTextbox();
                        try{
                            newServings = Short.parseShort(String.valueOf(servingsTextbox.getText()));
                            if (newServings < 1){
                                Toast.makeText(
                                        MainActivity.GetContentView().getContext(),
                                        "Minimum of 1 servings required",
                                        Toast.LENGTH_LONG
                                ).show();
                                servingsTextbox.setBackgroundColor(getResources().getColor(R.color.drink_template_selected) );
                                return;
                            }
                        }
                        catch (NumberFormatException e)
                        {
                            Toast.makeText(
                                    MainActivity.GetContentView().getContext(),
                                    "Invalid servings entered. Try entering a whole number.",
                                    Toast.LENGTH_LONG
                            ).show();
                            servingsTextbox.setBackgroundColor(getResources().getColor(R.color.drink_template_selected) );
                            return;                        }
                        //  -Set the servings of the template
                        newTemplate.SetServings(newServings);
                        servingsTextbox.setBackgroundColor(getResources().getColor(R.color.white) );


                        //  Price
                        EditText priceTextBox = (EditText) Alc_Create_Edit.GetTemplatePriceTextbox();
                        try{
                            newPrice = Float.parseFloat(String.valueOf(priceTextBox.getText()));
                            if (newPrice < 0){
                                Toast.makeText(
                                        MainActivity.GetContentView().getContext(),
                                        "Minimum of 0 dollars required",
                                        Toast.LENGTH_LONG
                                ).show();
                                priceTextBox.setBackgroundColor(getResources().getColor(R.color.drink_template_selected) );
                                return;
                            }
                        } catch (NumberFormatException e)
                        {
                            Toast.makeText(
                                    MainActivity.GetContentView().getContext(),
                                    "Invalid price entered. Try entering a decimal number.",
                                    Toast.LENGTH_LONG
                            ).show();
                            priceTextBox.setBackgroundColor(getResources().getColor(R.color.drink_template_selected) );
                            return;                        }
                        //  -Set the servings of the template
                        newTemplate.SetPrice(newPrice);
                        priceTextBox.setBackgroundColor(getResources().getColor(R.color.white) );



                        //  Calories
                        EditText caloriesTextbox = (EditText) Alc_Create_Edit.GetTemplateCaloriesTextbox();
                        try{
                            newCalories = Float.parseFloat(String.valueOf(caloriesTextbox.getText()));
                            if (newCalories < 0){
                                Toast.makeText(
                                        MainActivity.GetContentView().getContext(),
                                        "Minimum of 0 calories",
                                        Toast.LENGTH_LONG
                                ).show();
                                caloriesTextbox.setBackgroundColor(getResources().getColor(R.color.drink_template_selected) );
                                return;
                            }
                        } catch (NumberFormatException e)
                        {
                            Toast.makeText(
                                    MainActivity.GetContentView().getContext(),
                                    "Invalid calories entered. Try entering a decimal number.",
                                    Toast.LENGTH_LONG
                            ).show();
                            caloriesTextbox.setBackgroundColor(getResources().getColor(R.color.drink_template_selected) );
                            return;
                        }
                        // Set the calories of the template
                        newTemplate.SetCalories(newCalories);
                        caloriesTextbox.setBackgroundColor(getResources().getColor(R.color.white) );

                        // Type
                        AutoCompleteTextView typeText = (AutoCompleteTextView) Alc_Create_Edit.GetTemplateTypeAutoTextView();
                        newType = String.valueOf(typeText.getText());
                        newTemplate.SetType(DrinkType.DrinkTypeFromString(newType));


                        // Notify saved changes or created template based on programming mode
                        if (Alc_Programming.GetProgrammingMode() == Alc_Programming.ProgrammingMode.EDITING){
                            if (MainActivity.GetDrinkTemplateManager().ContainsTemplate(Alc_Create_Edit.OriginalEditingTemplateName())){
                                MainActivity.GetDrinkTemplateManager().RemoveTemplate(Alc_Create_Edit.OriginalEditingTemplateName());
                            }
                            MainActivity.GetDrinkTemplateManager().PutTemplate(newTemplate);
                            Toast.makeText(
                                    MainActivity.GetContentView().getContext(),
                                    "Saved changes to template",
                                    Toast.LENGTH_SHORT
                            ).show();
                        }
                        else if (Alc_Programming.GetProgrammingMode() == Alc_Programming.ProgrammingMode.CREATING){
                            MainActivity.GetDrinkTemplateManager().PutTemplate(newTemplate);
                            Toast.makeText(
                                    MainActivity.GetContentView().getContext(),
                                    "Created new template",
                                    Toast.LENGTH_SHORT
                            ).show();
                        }


                        // Go to alc programming
                        MainActivity.ChangeActiveFragment(R.id.alc_Programming, MainActivity.FragmentAnimationType.NONE);
                    }
                }
        );

        // Set up template image
        Alc_Create_Edit.templateImage = root.findViewById(R.id.alcEditImage);
        // If there is  path associated with the image, set the source of the template
        if (!Alc_Create_Edit.templateEditing.GetImageFilePath().equals(Universals.General.EmptyString())){
            Alc_Create_Edit.templateImage.setImageAlpha(255);
            Alc_Create_Edit.templateImage.setImageURI(Uri.fromFile(
                    new File(Alc_Create_Edit.templateEditing.GetImageFilePath())
            ));
        }
        else{
            Alc_Create_Edit.templateImage.setImageAlpha(0);
        }

        // Set up template change image button
        Alc_Create_Edit.templateChangeImageButton = root.findViewById(R.id.alcEditUploadImageButton);
        // Set up save image dialog
        Alc_Create_Edit.templateChangeImageButton.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {

                        Alc_Create_Edit.GetCurrentFragment().SaveTemplateImageDialog();


                        // Save the image dialog. Assign saved image to template image if returned
                        //Alc_Create_Edit.GetCurrentFragment().SaveTemplateImageDialog(fileName + i + ".png");

                        // Update the template image when finished if image was selected
                        /*
                        if (!imageFilePathReturn.isEmpty()){
                            Alc_Create_Edit.GetTemplateImage().setImageURI(Uri.fromFile(
                                    new File(imageFilePathReturn)
                            ));
                            Alc_Create_Edit.GetTemplateImage().setImageAlpha(1);
                            Alc_Create_Edit.GetEditingTemplate().SetImageFilePath(imageFilePathReturn);
                        }

                         */
                    }
                }
        );


        return root;
    }


    ///
    /// Getters and Setters
    ///
    public static Alc_Create_Edit GetCurrentFragment(){
        return Alc_Create_Edit.currentFragment;
    }
    /// <summary>
    ///     Represents the template being edited's name
    ///     before editing began.
    /// </summary>
    public static String OriginalEditingTemplateName(){
        return Alc_Create_Edit.originalTemplateName;
    }

    /// <summary>
    ///     Represents the template being edited
    /// </summary>
    public static DrinkTemplate GetEditingTemplate(){
        return Alc_Create_Edit.templateEditing;
    }
    public static void SetEditingTemplate(DrinkTemplate newTemplate){
        Alc_Create_Edit.templateEditing = newTemplate;}

    /// <summary>
    ///     Getters for textbox components of Alc_Create_Edit
    /// </summary>
    public static EditText GetTemplateNameTextbox(){
        return Alc_Create_Edit.templateNameTextbox;
    }
    public static EditText GetTemplateServingsTextbox(){
        return Alc_Create_Edit.templateServingsTextbox;
    }
    public static AutoCompleteTextView GetTemplateTypeAutoTextView(){
        return Alc_Create_Edit.templateTypeAutoTextbox;
    }
    public static EditText GetTemplatePriceTextbox(){
        return Alc_Create_Edit.templatePriceTextbox;
    }
    public static EditText GetTemplateCaloriesTextbox(){
        return Alc_Create_Edit.templateCaloriesTextbox;
    }
    public static ImageView GetTemplateImage(){
        return Alc_Create_Edit.templateImage;
    }

    /// <summary>
    /// Opens a save image dialog and saves an image from the phone
    ///     or the camera to the database manager's image directory.
    ///     Returns the file path to that image.
    /// </summary>
    ActivityResultLauncher<Intent> pickerMedia
            = registerForActivityResult(
            new ActivityResultContracts
                    .StartActivityForResult(),
            result -> {

                if (result.getResultCode()
                        == Activity.RESULT_OK) {
                    Intent data = result.getData();
                    // do your operation from here....
                    if (data != null
                            && data.getData() != null) {

                        Uri selectedImageUri = data.getData();
                        /*
                        Bitmap selectedImageBitmap;
                        try {
                            selectedImageBitmap
                                    = MediaStore.Images.Media.getBitmap(
                                    MainActivity.GetCurrentActivity().getContentResolver(),
                                    selectedImageUri);
                            Alc_Create_Edit.GetTemplateImage().setImageBitmap(
                                    selectedImageBitmap);
                            Alc_Create_Edit.GetTemplateImage().setImageAlpha(255);
                        }
                        catch (IOException e) {
                            e.printStackTrace();
                        }

                         */
                        InputStream input;
                        Bitmap imageInput;

                        // Get the image from storage and transform it into a bitmap
                        try {

                            input = MainActivity.GetContentView().getContext().getContentResolver().openInputStream(selectedImageUri);
                            if (input == null) {
                                Alc_Create_Edit.GetEditingTemplate().SetImageFilePath("");
                                Alc_Create_Edit.GetTemplateImage().setImageAlpha(0);
                                return;
                            }
                            imageInput = BitmapFactory.decodeStream(input);
                        }
                        catch (FileNotFoundException e) {
                            Alc_Create_Edit.GetTemplateImage().setImageAlpha(0);
                            throw new RuntimeException(e);
                        }

                        boolean uniqueFile = false;
                        String fileName = "drinkImage";
                        File uniqueFileChecker;
                        int i;
                        // Generate a unique file name for the template image
                        for(i = 0; !uniqueFile;){
                            uniqueFileChecker = new File(
                                    MainActivity.GetDatabaseManager().GetImageDirectory(),
                                    fileName + i + ".png"
                            );
                            if (!uniqueFileChecker.exists()){
                                uniqueFile = true;
                            }
                            else{
                                i++;
                            }
                        }
                        fileName = fileName + i + ".png";

                        // Save the bitmap to internal storage as an image
                        File imageDir = MainActivity.GetDatabaseManager().GetImageDirectory();
                        File newImagefile = new File(imageDir, fileName);
                        try{
                            FileOutputStream out = new FileOutputStream(newImagefile);
                            imageInput.compress(Bitmap.CompressFormat.PNG, 90, out);
                            out.flush();
                            out.close();
                        } catch (IOException e) {
                            Alc_Create_Edit.GetTemplateImage().setImageAlpha(0);
                            throw new RuntimeException(e);
                        }

                        // Set the temp string of MainActivity as the file path
                        Alc_Create_Edit.GetEditingTemplate().SetImageFilePath(newImagefile.getAbsolutePath());

                        File test = new File(newImagefile.getAbsolutePath());

                        // Retrieve the file and load to template image
                        Bitmap newTemplateImageFile = BitmapFactory.decodeFile(newImagefile.getAbsolutePath());
                        Alc_Create_Edit.GetTemplateImage().setImageAlpha(255);
                        Alc_Create_Edit.GetTemplateImage().setImageBitmap(newTemplateImageFile);


                    }
                }
            });
    public void SaveTemplateImageDialog(){
        Intent i = new Intent();
        i.setType("image/*");
        i.setAction(Intent.ACTION_GET_CONTENT);

        pickerMedia.launch(i);
    }

    public static String GetTemp(){
        return Alc_Create_Edit.temp;
    }
    public static void SetTemp(String newTemp){
        Alc_Create_Edit.temp = newTemp;
    }
}