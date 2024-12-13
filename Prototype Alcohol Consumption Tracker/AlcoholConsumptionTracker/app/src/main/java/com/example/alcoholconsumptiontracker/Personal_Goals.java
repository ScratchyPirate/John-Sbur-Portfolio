package com.example.alcoholconsumptiontracker;

import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.util.Log;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.ImageButton;

import com.example.alcoholconsumptiontracker.system.PersonalGoalEntry;

public class Personal_Goals extends Fragment {
    // Constructor for personal goals - daily, monthly, weekly

    public static EditText goalPriceDailyInput;
    public static EditText goalServingsDailyInput;
    public static EditText goalCalorieDailyInput;
    public static EditText dailyWrittenGoals;

    public static EditText goalPriceWeeklyInput;
    public static EditText goalServingsWeeklyInput;
    public static EditText goalCalorieWeeklyInput;
    public static EditText weeklyWrittenGoals;


    public static EditText goalPriceMonthlyInput;
    public static EditText goalServingsMonthlyInput;
    public static EditText goalCalorieMonthlyInput;
    public static EditText monthlyWrittenGoals;

    public static PersonalGoalEntry dailyGoalEntry = new PersonalGoalEntry();
    public static String savedDailyGoalPrice;
    public static String savedDailyGoalServings;
    public static String savedDailyGoalCalorie;
    public static String savedDailyWrittenGoal;

    public static PersonalGoalEntry weeklyGoalEntry = new PersonalGoalEntry();
    public static String savedWeeklyGoalPrice;
    public static String savedWeeklyGoalServings;
    public static String savedWeeklyGoalCalorie;
    public static String savedWeeklyWrittenGoal;

    public static PersonalGoalEntry monthlyGoalEntry = new PersonalGoalEntry();
    public static String savedMonthlyGoalPrice;
    public static String savedMonthlyGoalServings;
    public static String savedMonthlyGoalCalorie;
    public static String savedMonthlyWrittenGoal;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View root = inflater.inflate(R.layout.fragment_personal__goals, container, false);


        // Initalize buttons
        ImageButton gearIconPersonalGoals = null;
        ImageButton finishButtonPersonalGoals = null;

        // Initialize EditTexts with their IDs
        goalPriceDailyInput = root.findViewById(R.id.goalpricedailyid);
        goalServingsDailyInput = root.findViewById(R.id.goalservingsdailyid);
        goalCalorieDailyInput = root.findViewById(R.id.goalcaloriesdailyid);
        dailyWrittenGoals = root.findViewById(R.id.goalinfodailyid);

        goalPriceWeeklyInput = root.findViewById(R.id.goalpriceweeklyid);
        goalServingsWeeklyInput = root.findViewById(R.id.goalservingsweeklyid);
        goalCalorieWeeklyInput = root.findViewById(R.id.goalcaloriesweeklyid);
        weeklyWrittenGoals = root.findViewById(R.id.goalinfoweeklyid);

        goalPriceMonthlyInput = root.findViewById(R.id.goalpricemonthlyid);
        goalServingsMonthlyInput = root.findViewById(R.id.goalservingsmonthlyid);
        goalCalorieMonthlyInput = root.findViewById(R.id.goalcaloriesmonthlyid);
        monthlyWrittenGoals = root.findViewById(R.id.goalinfomonthlyid);


        // Setting text for the initalization scene
        goalPriceDailyInput.setText(dailyGoalEntry.getGoalPrice());
        goalServingsDailyInput.setText(dailyGoalEntry.getGoalServing());
        goalCalorieDailyInput.setText(dailyGoalEntry.getGoalCalorie());
        dailyWrittenGoals.setText(dailyGoalEntry.getWrittenStatement());

        goalPriceWeeklyInput.setText(weeklyGoalEntry.getGoalPrice());
        goalServingsWeeklyInput.setText(weeklyGoalEntry.getGoalServing());
        goalCalorieWeeklyInput.setText(weeklyGoalEntry.getGoalCalorie());
        weeklyWrittenGoals.setText(weeklyGoalEntry.getWrittenStatement());

        goalPriceMonthlyInput.setText(monthlyGoalEntry.getGoalPrice());
        goalServingsMonthlyInput.setText(monthlyGoalEntry.getGoalServing());
        goalCalorieMonthlyInput.setText(monthlyGoalEntry.getGoalCalorie());
        monthlyWrittenGoals.setText(monthlyGoalEntry.getWrittenStatement());


        gearIconPersonalGoals = root.findViewById(R.id.geariconPGid);
        finishButtonPersonalGoals = root.findViewById(R.id.finishbuttonPGid);

        // Disabled EditTexts
        goalPriceDailyInput.setEnabled(false);
        goalServingsDailyInput.setEnabled(false);
        goalCalorieDailyInput.setEnabled(false);
        dailyWrittenGoals.setEnabled(false);

        goalPriceWeeklyInput.setEnabled(false);
        goalServingsWeeklyInput.setEnabled(false);
        goalCalorieWeeklyInput.setEnabled(false);
        weeklyWrittenGoals.setEnabled(false);

        goalPriceMonthlyInput.setEnabled(false);
        goalServingsMonthlyInput.setEnabled(false);
        goalCalorieMonthlyInput.setEnabled(false);
        monthlyWrittenGoals.setEnabled(false);

        gearIconPersonalGoals.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View view) {
                // Enable EdiTexts
                goalPriceDailyInput.setEnabled(true);
                goalServingsDailyInput.setEnabled(true);
                goalCalorieDailyInput.setEnabled(true);
                dailyWrittenGoals.setEnabled(true);

                goalPriceWeeklyInput.setEnabled(true);
                goalServingsWeeklyInput.setEnabled(true);
                goalCalorieWeeklyInput.setEnabled(true);
                weeklyWrittenGoals.setEnabled(true);

                goalPriceMonthlyInput.setEnabled(true);
                goalServingsMonthlyInput.setEnabled(true);
                goalCalorieMonthlyInput.setEnabled(true);
                monthlyWrittenGoals.setEnabled(true);
            }
        });

        finishButtonPersonalGoals.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View view) {

                try {
                    // Daily
                    savedDailyGoalPrice = String.valueOf(goalPriceDailyInput.getText()).trim();
                    savedDailyGoalServings = String.valueOf(goalServingsDailyInput.getText()).trim();
                    savedDailyGoalCalorie = String.valueOf(goalCalorieDailyInput.getText()).trim();
                    savedDailyWrittenGoal = String.valueOf(dailyWrittenGoals.getText()).trim();

                    // Weekly
                    savedWeeklyGoalPrice = String.valueOf(goalPriceWeeklyInput.getText()).trim();
                    savedWeeklyGoalServings = String.valueOf(goalServingsWeeklyInput.getText()).trim();
                    savedWeeklyGoalCalorie = String.valueOf(goalCalorieWeeklyInput.getText()).trim();
                    savedWeeklyWrittenGoal = String.valueOf(weeklyWrittenGoals.getText()).trim();

                    // Monthly
                    savedMonthlyGoalPrice = String.valueOf(goalPriceMonthlyInput.getText()).trim();
                    savedMonthlyGoalServings = String.valueOf(goalServingsMonthlyInput.getText()).trim();
                    savedMonthlyGoalCalorie = String.valueOf(goalCalorieMonthlyInput.getText()).trim();
                    savedMonthlyWrittenGoal = String.valueOf(monthlyWrittenGoals.getText()).trim();

                    // Daily
                    dailyGoalEntry.setGoalPrice(savedDailyGoalPrice);
                    dailyGoalEntry.setGoalServings(savedDailyGoalServings);
                    dailyGoalEntry.setGoalCalorie(savedDailyGoalCalorie);
                    dailyGoalEntry.setWrittenStatement(savedDailyWrittenGoal);

                    // Weekly
                    weeklyGoalEntry.setGoalPrice(savedWeeklyGoalPrice);
                    weeklyGoalEntry.setGoalServings(savedDailyGoalServings);
                    weeklyGoalEntry.setGoalCalorie(savedWeeklyGoalCalorie);
                    weeklyGoalEntry.setWrittenStatement(savedWeeklyWrittenGoal);

                    // Monthly
                    monthlyGoalEntry.setGoalPrice(savedMonthlyGoalPrice);
                    monthlyGoalEntry.setGoalServings(savedMonthlyGoalServings);
                    monthlyGoalEntry.setGoalCalorie(savedMonthlyGoalCalorie);
                    monthlyGoalEntry.setWrittenStatement(savedMonthlyWrittenGoal);

                    // Daily
                    goalPriceDailyInput.setText("$" + savedDailyGoalPrice);
                    goalServingsDailyInput.setText(savedDailyGoalServings);
                    goalCalorieDailyInput.setText(savedDailyGoalCalorie);
                    dailyWrittenGoals.setText(savedDailyWrittenGoal);

                    // Weekly
                    goalPriceWeeklyInput.setText("$" + savedWeeklyGoalPrice);
                    goalServingsWeeklyInput.setText(savedWeeklyGoalServings);
                    goalCalorieWeeklyInput.setText(savedWeeklyGoalCalorie);
                    weeklyWrittenGoals.setText(savedWeeklyWrittenGoal);

                    // Monthly
                    goalPriceMonthlyInput.setText("$" + savedMonthlyGoalPrice);
                    goalServingsMonthlyInput.setText(savedMonthlyGoalServings);
                    goalCalorieMonthlyInput.setText(savedMonthlyGoalCalorie);
                    monthlyWrittenGoals.setText(savedMonthlyWrittenGoal);

                    // Daily
                    goalPriceDailyInput.setEnabled(false);
                    goalServingsDailyInput.setEnabled(false);
                    goalCalorieDailyInput.setEnabled(false);
                    dailyWrittenGoals.setEnabled(false);

                    // Weekly
                    goalPriceWeeklyInput.setEnabled(false);
                    goalServingsWeeklyInput.setEnabled(false);
                    goalCalorieWeeklyInput.setEnabled(false);
                    weeklyWrittenGoals.setEnabled(false);

                    // Monthly
                    goalPriceMonthlyInput.setEnabled(false);
                    goalServingsMonthlyInput.setEnabled(false);
                    goalCalorieMonthlyInput.setEnabled(false);
                    monthlyWrittenGoals.setEnabled(false);
                } catch (NumberFormatException e) {
                    Log.e("Personal_Info", "Invalid input for numeric fields.", e);
                }
            }
        });
        return root;
    }
}