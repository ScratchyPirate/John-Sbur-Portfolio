package com.example.alcoholconsumptiontracker;

import android.app.Activity;
import android.app.TimePickerDialog;
import android.graphics.Color;
import android.media.Image;
import android.net.Uri;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AbsListView;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.TimePicker;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.StyleRes;
import androidx.fragment.app.Fragment;

import com.example.alcoholconsumptiontracker.system.Drink;
import com.example.alcoholconsumptiontracker.system.DrinkTemplate;
import com.example.alcoholconsumptiontracker.system.Universals;
import com.google.android.material.timepicker.MaterialTimePicker;
import com.google.android.material.timepicker.TimeFormat;

import java.io.File;
import java.sql.Time;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

public class Alc_Logging extends Fragment {

    ///
    ///  Locals
    ///

    ///
    /// Globals
    ///
    // Represents the textbox where drink occasion is entered
    private static EditText drinkOccasionTextbox;

    // Represents the button that triggers a time dialog to set the time of drink consumption
    private static Button drinkTimeOfDrinkDialogButton;

    // Represents the textview that contains the time of consumption
    private static TextView drinkTimeOfDrinkText;

    // Represents the button that adds logs a drink based on the selected template, time chosen, and occasion
    private static ImageButton addOneDrink;

    // Represents the button that sends from alc_logging to the home page (finish)
    private static ImageButton finishedLoggingDrinks;

    // Represents the back button that sends from alc_logging to alc_select
    private static ImageButton backButton;


    private static int drinkMinute;
    private static int drinkHour;

    // Represents an image of the selected template
    private static ImageView selectedTemplateImage;

    // Represents the template received from alcohol select
    private static DrinkTemplate selectedTemplate;


    public Alc_Logging() {
        super(R.layout.fragment_alc__logging);
    }


    @NonNull
    public static Alc_Logging newInstance() {
        Alc_Logging fragment = new Alc_Logging();
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
        View root =  inflater.inflate(R.layout.fragment_alc__logging, container, false);

        // Set globals to null (reset)
        Alc_Logging.selectedTemplateImage = null;
        Alc_Logging.selectedTemplate = null;
        Alc_Logging.drinkOccasionTextbox = null;
        Alc_Logging.drinkTimeOfDrinkDialogButton = null;
        Alc_Logging.drinkTimeOfDrinkText = null;


        // Get the selected template from alcohol select (should not be null at this point)
        Alc_Logging.selectedTemplate = Alc_Select.GetSelectedTemplate();
        // Initialize displayed information about the template
        if (Alc_Logging.selectedTemplate != null){

            //  Name
            TextView templateName = root.findViewById(R.id.alcLoggingName);
            templateName.setTextColor(Alc_Logging.PrimaryLoggingTextColor());
            templateName.setText(Alc_Logging.selectedTemplate.GetName());

            //  Type
            TextView templateType = root.findViewById(R.id.alcLoggingType);
            templateType.setTextColor(Alc_Logging.PrimaryLoggingTextColor());
            templateType.setText(Alc_Logging.selectedTemplate.GetType().Get());

            //  Servings
            TextView templateServings = root.findViewById(R.id.alcLoggingServings);
            templateServings.setTextColor(Alc_Logging.PrimaryLoggingTextColor());
            templateServings.setText(String.valueOf(Alc_Logging.selectedTemplate.GetServings()));

            //  Price
            TextView templatePrice = root.findViewById(R.id.alcLoggingPrice);
            templatePrice.setTextColor(Alc_Logging.PrimaryLoggingTextColor());
            templatePrice.setText(String.valueOf(Alc_Logging.selectedTemplate.GetPrice()));

            //  Calories
            TextView templateCalories = root.findViewById(R.id.alcLoggingCalories);
            templateCalories.setTextColor(Alc_Logging.PrimaryLoggingTextColor());
            templateCalories.setText(String.valueOf(Alc_Logging.selectedTemplate.GetCalories()));

            //  Image
            ImageView templateImage = root.findViewById(R.id.drinkLoggingImage);
            if (!Alc_Logging.selectedTemplate.GetImageFilePath().isEmpty()){
                templateImage.setImageAlpha(255);
                templateImage.setImageURI(Uri.fromFile(
                        new File(Alc_Logging.selectedTemplate.GetImageFilePath())
                ));
            }
            else{
                templateImage.setImageAlpha(0);
            }
        }

        // Set up the occasion textbox
        Alc_Logging.drinkOccasionTextbox = root.findViewById((R.id.drinkLoggingOccassionInput));
        Alc_Logging.drinkOccasionTextbox.setText("");
        Alc_Logging.drinkOccasionTextbox.setTextColor(Alc_Logging.PrimaryLoggingTextColor());
        Alc_Logging.drinkOccasionTextbox.setSingleLine(false);
        Alc_Logging.drinkOccasionTextbox.setVerticalScrollBarEnabled(true);
        Alc_Logging.drinkOccasionTextbox.setHorizontallyScrolling(false);

        // Set up the time of drink textbox
        Alc_Logging.drinkTimeOfDrinkText = root.findViewById(R.id.alcLoggingDrinkTime);
        Date currentTime = Calendar.getInstance().getTime();
        Alc_Logging.drinkHour = currentTime.getHours();
        Alc_Logging.drinkMinute = currentTime.getMinutes();
        Alc_Logging.drinkTimeOfDrinkText.setText(MainActivity.FormatTimeString(Alc_Logging.drinkHour, Alc_Logging.drinkMinute));

        // Set up the time of drink button
        Alc_Logging.drinkTimeOfDrinkDialogButton = root.findViewById(R.id.alcLoggingTimeSelectorButton);
        Alc_Logging.drinkTimeOfDrinkDialogButton.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        TimePickerDialog.OnTimeSetListener timeSetListener = new TimePickerDialog.OnTimeSetListener(
                        ) {
                            @Override
                            public void onTimeSet(TimePicker view, int hourOfDay, int minute) {

                            }
                        };
                        MaterialTimePicker picker = new MaterialTimePicker.Builder().
                                setHour(Alc_Logging.drinkHour).
                                setMinute(Alc_Logging.drinkMinute).
                                setInputMode(MaterialTimePicker.INPUT_MODE_KEYBOARD).
                                setTimeFormat(TimeFormat.CLOCK_12H).
                                build();
                        picker.addOnPositiveButtonClickListener(new View.OnClickListener() {
                            @Override
                            public void onClick(View v) {
                                Alc_Logging.drinkMinute = picker.getMinute();
                                Alc_Logging.drinkHour = picker.getHour();
                                Alc_Logging.drinkTimeOfDrinkText.setText(MainActivity.FormatTimeString(Alc_Logging.drinkHour, Alc_Logging.drinkMinute));
                            }
                        });
                        picker.show(getActivity().getSupportFragmentManager(), "Select Time of Drink");
                    }
                }
        );

        // Set up the log drink button
        Alc_Logging.addOneDrink = root.findViewById(R.id.alcLoggingAddOneDrink);
        // When the button is pressed, add one drink which is stored to the daily log
        Alc_Logging.addOneDrink.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        Drink loggedDrink = Alc_Logging.selectedTemplate.ProduceDrink(
                                String.valueOf(Alc_Logging.drinkOccasionTextbox.getText()),
                                (short)Alc_Logging.drinkHour,
                                (short)Alc_Logging.drinkMinute
                        );
                        Toast.makeText(
                                MainActivity.GetContentView().getContext(),
                                "Added 1 " + Alc_Logging.selectedTemplate.GetName(),
                                Toast.LENGTH_SHORT
                        ).show();
                        MainActivity.PutDrinkInDrinkList(loggedDrink);
                    }

                }
        );

        // Set up the finished logging drinks button
        Alc_Logging.finishedLoggingDrinks = root.findViewById(R.id.alcLoggingFinishLoggingButton);
        Alc_Logging.finishedLoggingDrinks.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        // When the back button is pressed, set the nav view to home
                        //  In MainActivity, this is set to also change the active fragment
                        MainActivity.GetNavView().setSelectedItemId(R.id.navigation_home);
                    }

                }
        );

        // Set up the back button
        Alc_Logging.backButton = root.findViewById(R.id.alcLoggingBackButton);
        Alc_Logging.backButton.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        MainActivity.ChangeActiveFragment(R.id.alc_Select, MainActivity.FragmentAnimationType.NONE);
                    }

                }
        );

        return root;
    }

    ///
    /// Class colors
    ///
    private static int PrimaryLoggingTextColor(){
        return Color.parseColor("#000000");
    }
    private static int SecondaryLoggingTextColor(){ return Color.parseColor("#666666");}


}
