package com.example.alcoholconsumptiontracker;


import android.content.Context;
import android.content.DialogInterface;
import android.content.res.ColorStateList;
import android.graphics.Color;
import android.net.Uri;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewParent;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.RadioButton;
import android.widget.TextView;

import androidx.appcompat.app.AlertDialog;
import androidx.fragment.app.Fragment;

import com.example.alcoholconsumptiontracker.system.DrinkTemplate;
import com.example.alcoholconsumptiontracker.system.Universals;

import java.io.File;

public class Alc_Programming extends Fragment {

    ///
    /// Globals
    ///
    // List to be populated with drinks and button that adds more drinks
    private static ListView alcProgrammingListView;

    // Represents the button that transitions from alcProgramming to alcCreate
    private static ImageButton alcProgrammingSendToAlcCreate;

    // Represents the button that transitions from alcProgramming to alcSelect
    private static ImageButton alcProgrammingBackButton;

    // Represents the selected drink template's row
    private static View selectedTemplateRow;

    // Represents the help box
    private static ImageView helpSquare;
    private static TextView helpTextbox;

    // Represents whether the app is programming a template by creating a new one or editing an existing one
    public enum ProgrammingMode {
        NONE,
        CREATING,
        EDITING
    }
    private static ProgrammingMode programmingMode;


    // Represents the selected drink template
    private static DrinkTemplate selectedTemplate;


    public Alc_Programming() {
        super(R.layout.fragment_alc__programming);
    }


    public static Alc_Programming newInstance(String param1, String param2) {
        Alc_Programming fragment = new Alc_Programming();
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
        View root =  inflater.inflate(R.layout.fragment_alc__programming, container, false);

        // Reset globals to nul
        Alc_Programming.selectedTemplate = null;
        Alc_Programming.selectedTemplateRow = null;
        Alc_Programming.alcProgrammingListView = null;
        Alc_Programming.alcProgrammingSendToAlcCreate = null;
        Alc_Programming.alcProgrammingBackButton = null;
        Alc_Programming.programmingMode = ProgrammingMode.NONE;
        Alc_Programming.helpSquare = null;
        Alc_Programming.helpTextbox = null;

        // Initialize helpbox
        Alc_Programming.helpTextbox = root.findViewById(R.id.alcProgrammingHelpTextbox);
        Alc_Programming.helpSquare = root.findViewById(R.id.alcProgrammingHelpSquare);

        // Initialize alc programming list view and help box
        Alc_Programming.alcProgrammingListView = root.findViewById(R.id.alc_programming_selected_template);
        // If template exist, populate the listview. Otherwise, display the help text
        if (!MainActivity.GetDrinkTemplateManager().GetTemplateList().isEmpty()){
            Alc_Programming.alcProgrammingListView.setVisibility(View.VISIBLE);
            Alc_Programming.helpSquare.setVisibility(View.INVISIBLE);
            Alc_Programming.helpTextbox.setVisibility(View.INVISIBLE);
            Alc_Programming.alcProgrammingListView.setAdapter(new Alc_Programming.alcProgrammingListAdapter(MainActivity.GetContentView().getContext()));
        }
        else{
            Alc_Programming.alcProgrammingListView.setVisibility(View.INVISIBLE);
            Alc_Programming.helpSquare.setVisibility(View.VISIBLE);
            Alc_Programming.helpTextbox.setVisibility(View.VISIBLE);
        }

        // Initialize alc create button
        Alc_Programming.alcProgrammingSendToAlcCreate = root.findViewById(R.id.alcProgrammingCreateDrink);
        Alc_Programming.alcProgrammingSendToAlcCreate.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        Alc_Programming.SetProgrammingMode(ProgrammingMode.CREATING);
                        MainActivity.ChangeActiveFragment(R.id.alc_Create_Edit, MainActivity.FragmentAnimationType.NONE);
                    }
                }
        );

        // Initialize back button
        Alc_Programming.alcProgrammingBackButton = root.findViewById(R.id.alcProgrammingBackButton);
        Alc_Programming.alcProgrammingBackButton.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        MainActivity.ChangeActiveFragment(R.id.logging_intermediary, MainActivity.FragmentAnimationType.FADE);
                    }
                }
        );

        // Save the templates before any changes are made for error prevention.
        MainActivity.SaveDrinkTemplates();

        // Return root when finished
        return root;
    }



    ///
    /// Colors
    ///
    public static int RowSelectedColor(){
        return Color.parseColor("#fff9d1");
    }
    public static int RowUnselectedColor(){
        return Color.parseColor("#ebebeb");
    }
    public static int ButtonSelectedColor(){
        return Color.parseColor("#e8c200");
    }
    public static int ButtonUnselectedColor(){
        return Color.parseColor("#FF000000");
    }

    ///
    /// Setters and Getters
    ///

    /// Sets the selected template of alc Select
    public static void SetSelectedTemplate(DrinkTemplate newSelectedTemplate){
        Alc_Programming.selectedTemplate = newSelectedTemplate;
    }

    /// Gets the selected template from alc Select if there is one.
    public static DrinkTemplate GetSelectedTemplate(){
        return Alc_Programming.selectedTemplate;
    }

    /// Gets or Sets the programming mode
    public static ProgrammingMode GetProgrammingMode(){
        return Alc_Programming.programmingMode;
    }
    public static void SetProgrammingMode(ProgrammingMode newMode){
        Alc_Programming.programmingMode = newMode;
    }


    ///
    ///  Methods
    ///
    /// <summary>
    ///  Assumes a row object of type shown in alc_select_list_item.xml
    ///     Adjusts row content to show the row is selected.
    ///     *Note, if the type isn't a row from alc_select_list_item.xml, the method will
    ///     throw a null pointer exception
    /// </summary>
    public static void ShowSelectRow(View targetRow){
        RadioButton selectButton = targetRow.findViewById(R.id.alcSelectListViewRadioButton);
        selectButton.setChecked(true);
        selectButton.setButtonTintList(ColorStateList.valueOf(Alc_Select.ButtonSelectedColor()));
        targetRow.setBackgroundColor(Alc_Select.RowSelectedColor());
    }
    /// <summary>
    ///  Assumes a row object of type shown in alc_select_list_item.xml
    ///     Adjusts row content to show the row is not selected.
    ///     *Note, if the type isn't a row from alc_select_list_item.xml, the method will
    ///     throw a null pointer exception
    /// </summary>
    public static void ShowUnselectRow(View targetRow){
        RadioButton selectButton = targetRow.findViewById(R.id.alcSelectListViewRadioButton);
        selectButton.setChecked(false);
        selectButton.setButtonTintList(ColorStateList.valueOf(Alc_Select.ButtonUnselectedColor()));
        targetRow.setBackgroundColor(Alc_Select.RowUnselectedColor());
    }




    private class alcProgrammingListAdapter extends BaseAdapter {

        private Context listContext;
        private LayoutInflater inflater;
        private DrinkTemplate[] templateList;

        public alcProgrammingListAdapter(Context context) {
            this.templateList = MainActivity.GetDrinkTemplateManager().GetTemplateList().values().toArray(new DrinkTemplate[0]);
            this.listContext = context;
        }

        public alcProgrammingListAdapter(Context context, DrinkTemplate[] templateList) {
            this.templateList = templateList;
            this.listContext = context;
        }

        public int getCount() {
            return this.templateList.length;
        }

        public Object getItem(int arg0) {
            // TODO Auto-generated method stub
            return null;
        }

        public long getItemId(int position) {
            // TODO Auto-generated method stub
            return position;
        }

        ///
        ///  Gets a row from the list as its different objects. Returns the result as a view
        ///
        public View getView(int position, View convertView, ViewGroup parent) {

            inflater = (LayoutInflater) this.listContext.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            View row;
            row = inflater.inflate(R.layout.alc_programming_list_item, parent, false);

            TextView drinkName;
            TextView drinkType;
            TextView drinkServings;
            TextView drinkCalories;
            TextView drinkPrice;
            ImageView drinkImage;
            ImageButton editDrinkTemplate;
            ImageButton deleteDrinkTemplate;

            // Name
            drinkName = (TextView) row.findViewById(R.id.alcProgrammingTemplateName);
            drinkName.setText(
                    this.templateList[position].GetName()
            );

            // Type
            drinkType = (TextView) row.findViewById(R.id.alcProgrammingTemplateType);
            drinkType.setText(
                    this.templateList[position].GetType().Get()
            );

            // Servings
            drinkServings = (TextView) row.findViewById(R.id.alcProgrammingTemplateServings);
            drinkServings.setText(
                    Short.toString(this.templateList[position].GetServings())
            );

            // Calories
            drinkCalories = (TextView) row.findViewById(R.id.alcProgrammingTemplateCalories);
            drinkCalories.setText(
                    Double.toString(
                            Math.round(this.templateList[position].GetCalories() * 100.0) / 100.0
                    )
            );

            // Price
            drinkPrice = (TextView) row.findViewById(R.id.alcProgrammingTemplatePrice);
            drinkPrice.setText(
                    Double.toString(
                            Math.round(this.templateList[position].GetPrice() * 100.0) / 100.0
                    )
            );

            // Image
            drinkImage = (ImageView) row.findViewById(R.id.alcProgrammingTemplateImage);
            if (!this.templateList[position].GetImageFilePath().isEmpty()){
                drinkImage.setImageAlpha(255);
                drinkImage.setImageURI(Uri.fromFile(
                        new File(this.templateList[position].GetImageFilePath())
                ));
            }
            else{
                drinkImage.setImageAlpha(0);
            }


            // Edit button
            editDrinkTemplate = (ImageButton)  row.findViewById(R.id.drinkProgrammingTemplateEditButton);
            editDrinkTemplate.setTag(this.templateList[position].GetName());
            editDrinkTemplate.setOnClickListener(
                    new View.OnClickListener() {
                        @Override
                        public void onClick(View v) {
                            // Set the selected template of alc_programming
                            String templateName = (String)v.getTag();
                            Alc_Programming.SetSelectedTemplate(
                                    MainActivity.GetDrinkTemplateManager().GetTemplate(templateName)
                            );
                            Alc_Programming.SetProgrammingMode(ProgrammingMode.EDITING);
                            // Change to alcohol edit
                            MainActivity.ChangeActiveFragment(R.id.alc_Create_Edit, MainActivity.FragmentAnimationType.NONE);
                        }
                    }
            );


            // Delete button
            deleteDrinkTemplate = (ImageButton) row.findViewById(R.id.drinkProgrammingTemplateDeleteButton);
            deleteDrinkTemplate.setTag(this.templateList[position].GetName());
            deleteDrinkTemplate.setOnClickListener(
                    new View.OnClickListener() {
                        @Override
                        public void onClick(View v) {
                            // Set the selected template of alcohol programming
                            String templateName = (String)v.getTag();
                            Alc_Programming.SetSelectedTemplate(
                                    MainActivity.GetDrinkTemplateManager().GetTemplate(templateName)
                            );

                            //  If the button was the delete template button, open a dialog and confirm the user wants to delete the template
                            //      If yes, delete the template from main. Then reload alc programming
                            //      If no, cancel then reset selected template
                            new AlertDialog.Builder(MainActivity.GetContentView().getContext())
                                    .setMessage(
                                            "Delete the '" +
                                                    Alc_Programming.selectedTemplate.GetName() +
                                                    "' template?")
                                    .setPositiveButton(
                                            "Delete",
                                            new DialogInterface.OnClickListener() {
                                                @Override
                                                public void onClick(DialogInterface dialog, int which) {
                                                    // Remove the template
                                                    MainActivity.GetDrinkTemplateManager().RemoveTemplate(
                                                            Alc_Programming.GetSelectedTemplate().GetName()
                                                    );
                                                    // Force fragment reload
                                                    MainActivity.ReloadActiveFragment();
                                                }
                                            }
                                    )
                                    .setNegativeButton(
                                            "Cancel",
                                            new DialogInterface.OnClickListener() {
                                                @Override
                                                public void onClick(DialogInterface dialog, int which) {
                                                    Alc_Programming.selectedTemplate = null;
                                                }
                                            }
                                    )
                                    .show();

                        }
                    }
            );

            // Return the row
            return (row);
        }
    }
}
