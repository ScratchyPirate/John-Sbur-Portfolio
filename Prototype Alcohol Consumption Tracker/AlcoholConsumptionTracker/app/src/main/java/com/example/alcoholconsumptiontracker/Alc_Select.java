package com.example.alcoholconsumptiontracker;

import android.content.Context;
import android.content.res.ColorStateList;
import android.graphics.Color;
import android.net.Uri;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AbsListView;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.RadioButton;
import android.widget.TextView;

import com.example.alcoholconsumptiontracker.system.DrinkTemplate;

import java.io.File;

/**
 * A simple {@link Fragment} subclass.
 * Use the {@link Alc_Select#newInstance} factory method to
 * create an instance of this fragment.
 */
public class Alc_Select extends Fragment {


    ///
    ///  Locals
    ///
    // List to be populated with drinks and button that adds more drinks
    private static ListView alcSelectListView;

    // Represents the selected drink template's row
    private static View selectedTemplateRow;

    // Represents the confirm choice button
    private static ImageButton confirmChoiceButton;
    // Represents the image of the confirm choice button
    private static ImageButton backButton;

    // Represents the help box
    private static ImageView helpSquare;
    private static TextView helpTextbox;

    // Represents the drink template selected during alcohol select
    private static DrinkTemplate selectedTemplate;

    public Alc_Select() {
        super(R.layout.fragment_alc__select);

        }


    @NonNull
    public static Alc_Select newInstance() {
        Alc_Select fragment = new Alc_Select();
        Bundle args = new Bundle();
        return fragment;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {


        // Instantiate fragment contents from fragment_alc__select to the parent container
        View root = inflater.inflate(R.layout.fragment_alc__select, container, false);

        // Set globals to null
        Alc_Select.alcSelectListView = null;
        Alc_Select.selectedTemplate = null;
        Alc_Select.selectedTemplateRow = null;
        Alc_Select.helpSquare = null;
        Alc_Select.helpTextbox = null;

        // Initialize help box
        Alc_Select.helpTextbox = root.findViewById(R.id.alcSelectHelpTextbox);
        Alc_Select.helpSquare = root.findViewById(R.id.alcSelectHelpSquare);

        // Initialize buttons and images
        Alc_Select.confirmChoiceButton = root.findViewById(R.id.alcSelectToAlcLogging);
        Alc_Select.backButton = root.findViewById(R.id.alcSelectBackButton);
        //  Set send to alcohol programming to not selectable (until template is selected)
        Alc_Select.confirmChoiceButton.setEnabled(false);
        Alc_Select.confirmChoiceButton.setImageAlpha(125);


        // Initialize alcSelect list
        Alc_Select.alcSelectListView = root.findViewById(R.id.alc_logging_selected_template);
        // If template exist, populate the listview. Otherwise, display the help text
        if (!MainActivity.GetDrinkTemplateManager().GetTemplateList().isEmpty()){
            Alc_Select.alcSelectListView.setVisibility(View.VISIBLE);
            Alc_Select.alcSelectListView.setChoiceMode(AbsListView.CHOICE_MODE_SINGLE);
            Alc_Select.helpSquare.setVisibility(View.INVISIBLE);
            Alc_Select.helpTextbox.setVisibility(View.INVISIBLE);
            Alc_Select.alcSelectListView.setAdapter(new alcSelectListAdapter(MainActivity.GetContentView().getContext()));

            // Set an on click listener for when an item is selected. This represents the user
            //  selecting a drink template.
            Alc_Select.alcSelectListView.setOnItemClickListener(
                    new AdapterView.OnItemClickListener() {
                        @Override
                        public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

                            // If a row was previously selected, show it is unselected
                            if (Alc_Select.selectedTemplateRow != null){
                                Alc_Select.ShowUnselectRow(Alc_Select.selectedTemplateRow);
                            }


                            // Set the new selected row
                            TextView incomingView = (TextView)view.findViewById(R.id.alcSelectTemplateName);
                            Alc_Select.SetSelectedTemplate(MainActivity.GetDrinkTemplateManager().GetTemplate(
                                    (String) incomingView.getText()
                            ));

                            // Show the selected row is selected
                            Alc_Select.selectedTemplateRow = view;
                            Alc_Select.ShowSelectRow(Alc_Select.selectedTemplateRow);
                        }
                    }
            );
        }
        else{
            Alc_Select.alcSelectListView.setVisibility(View.INVISIBLE);
            Alc_Select.helpSquare.setVisibility(View.VISIBLE);
            Alc_Select.helpTextbox.setVisibility(View.VISIBLE);
        }


        // Set confirmation button to go to alc_logging if pressed
        Alc_Select.confirmChoiceButton.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        MainActivity.ChangeActiveFragment(R.id.alcLoggingBackButton, MainActivity.FragmentAnimationType.NONE);
                    }
                }
        );

        // Set back button to go to logging_intermediary if pressed
        Alc_Select.backButton.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        MainActivity.ChangeActiveFragment(R.id.logging_intermediary, MainActivity.FragmentAnimationType.FADE);
                    }
                }
        );


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
    private static void SetSelectedTemplate(DrinkTemplate newSelectedTemplate){
        Alc_Select.selectedTemplate = newSelectedTemplate;
        if (!Alc_Select.confirmChoiceButton.isEnabled()){
            Alc_Select.confirmChoiceButton.setEnabled(true);
            Alc_Select.confirmChoiceButton.setImageAlpha(255);
        }
    }

    /// Gets the selected template from alc Select if there is one.
    public static DrinkTemplate GetSelectedTemplate(){
        return Alc_Select.selectedTemplate;
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
    }


    private class alcSelectListAdapter extends BaseAdapter {

        Context listContext;
        LayoutInflater inflater;
        DrinkTemplate[] templateList;

        public alcSelectListAdapter(Context context) {
            this.templateList = MainActivity.GetDrinkTemplateManager().GetTemplateList().values().toArray(new DrinkTemplate[0]);
            this.listContext = context;
        }

        public alcSelectListAdapter(Context context, DrinkTemplate[] templateList) {
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
            row = inflater.inflate(R.layout.alc_select_list_item, parent, false);

            TextView drinkName;
            TextView drinkType;
            TextView drinkServings;
            TextView drinkCalories;
            TextView drinkPrice;
            ImageView drinkImage;

            // Name
            drinkName = (TextView) row.findViewById(R.id.alcSelectTemplateName);
            drinkName.setText(
                    this.templateList[position].GetName()
            );

            // Type
            drinkType = (TextView) row.findViewById(R.id.alcSelectTemplateType);
            drinkType.setText(
                    this.templateList[position].GetType().Get()
            );

            // Servings
            drinkServings = (TextView) row.findViewById(R.id.alcSelectTemplateServings);
            drinkServings.setText(
                    Short.toString(this.templateList[position].GetServings())
            );

            // Calories
            drinkCalories = (TextView) row.findViewById(R.id.alcSelectTemplateCalories);
            drinkCalories.setText(
                    Double.toString(
                            Math.round(this.templateList[position].GetCalories() * 100.0) / 100.0
                    )
            );

            // Price
            drinkPrice = (TextView) row.findViewById(R.id.alcSelectTemplatePrice);
            drinkPrice.setText(
                    Double.toString(
                            Math.round(this.templateList[position].GetPrice() * 100.0) / 100.0
                            )
            );
            drinkImage = (ImageView) row.findViewById(R.id.alcSelectTemplateImage);
            if (!this.templateList[position].GetImageFilePath().isEmpty()){
                drinkImage.setImageAlpha(255);
                drinkImage.setImageURI(Uri.fromFile(
                        new File(this.templateList[position].GetImageFilePath())
                ));
            }
            else{
                drinkImage.setImageAlpha(0);
            }

            // Set the row as unselected
            row.setBackgroundColor(Alc_Select.RowUnselectedColor());
            RadioButton rowButton  = (RadioButton) row.findViewById(R.id.alcSelectListViewRadioButton);
            rowButton.setButtonTintList(ColorStateList.valueOf(Alc_Select.ButtonUnselectedColor()));



            // Return the row
            return (row);
        }
    }
}