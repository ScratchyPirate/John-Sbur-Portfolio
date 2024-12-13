/// <summary>
///  File: Drink.java
///     Description: Meant to contain the object representing a logged Drink and its methods
/// </summary>

package com.example.alcoholconsumptiontracker.system;

import android.content.Intent;
import android.util.Log;

import java.time.LocalTime;

public class Drink {

    ///
    /// Local variables
    ///

    /// These locals have a 1 to 1 correspondence with Drink Template
    //
    private String name; // Name of drink
    private DrinkType type; // Type of drink
    private short servings; // Number of servings ( 0 >= )
    private float aPV; // Alcohol Per volume ( 0.0 >= )
    private float price; // Price ( 0.0 >= , dollars USD)
    private float calories; // Calories (0.0 >=, kcal)
    private String imageFilePath; // Image from gallery. Path to image, but not the image itself.
    // End corresponding locals

    private String occasion; // Occasion for drinking
    private short hourOfConsumption; // Time drink was drunk in the form hour, minute. Military time
    private short minuteOfConsumption;
    //


    ///
    /// Constructors
    ///
    // Default
    Drink(){
        this.name = Universals.General.EmptyString();
        this.type = new DrinkType();
        this.servings = 0;
        this.aPV = 0;
        this.price = 0;
        this.calories = 0;
        this.imageFilePath = Universals.General.EmptyString();
        this.occasion = Universals.General.EmptyString();
        this.hourOfConsumption = 0;
        this.minuteOfConsumption = 0;
    }
    // Construct from Drink Template
    public Drink(DrinkTemplate template){
        this.name = template.GetName();
        this.type = template.GetType();
        this.servings = template.GetServings();
        this.aPV = template.GetAPV();
        this.price = template.GetPrice();
        this.calories = template.GetCalories();
        this.imageFilePath = template.GetImageFilePath();
    }

    ///
    /// Setters and Getters
    ///
    // Name
    public String GetName(){return this.name;}
    // Type
    public DrinkType GetType(){return this.type;}
    // Servings
    public short GetServings(){
        return this.servings;
    }
    // APV
    public float GetAPV(){return this.aPV;}
    // Price
    public float GetPrice(){return this.price;}
    // Calories
    public float GetCalories(){return this.calories;}
    // Image
    public String GetImageFilePath(){return this.imageFilePath;}
    // Occasion
    public String GetOccasion(){
        return this.occasion;
    }
    public void SetOccasion(String newOccasion){
        this.occasion = newOccasion;
    }
    // Hour of Consumption
    public short GetHourOfConsumption(){
        return this.hourOfConsumption;
    }
    public void SetHourOfConsumption(short newHour){
        if (newHour < 0) this.hourOfConsumption = 0;
        else this.hourOfConsumption = newHour;
    }
    // Minute of Consumption
    public short GetMinuteOfConsumption(){
        return this.minuteOfConsumption;
    }
    public void SetMinuteOfConsumption(short newMinute){
        if (newMinute < 0) this.minuteOfConsumption = 0;
        else this.minuteOfConsumption = newMinute;
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
    // Test Getters and Setters
    public static void TestGettersAndSetters(boolean printAllMessages){

        // Testing locals
        Drink testingDrink = new Drink();
        short testingShort = -1;
        String testingString = Universals.General.EmptyString();

        // Non-exception cases
        //  -Case 1, normal values, minute of consumption get and set
        testingDrink.SetMinuteOfConsumption((short)1);
        testingShort = testingDrink.GetMinuteOfConsumption();
        if (testingShort != 1){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkMessages.GetterSetterMessage(false, 1)
            );
        }
        else if (printAllMessages){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkMessages.GetterSetterMessage(true, 1)
            );
        }

        //  - Case 2, normal values, hour of consumption get and set
        testingDrink.SetHourOfConsumption((short)1);
        testingShort = testingDrink.GetHourOfConsumption();
        if (testingShort != 1){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkMessages.GetterSetterMessage(false, 2)
            );
        }
        else if (printAllMessages){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkMessages.GetterSetterMessage(true, 2)
            );
        }

        //  - Case 3, normal values, occasion get and set
        testingDrink.SetOccasion("testing string");
        testingString = testingDrink.GetOccasion();
        if (!testingString.equals("testing string")){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkMessages.GetterSetterMessage(false, 3)
            );
        }
        else if (printAllMessages){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkMessages.GetterSetterMessage(true, 3)
            );
        }

        //  - Case 4, abnormal values, minute of consumption get set
        testingDrink.SetMinuteOfConsumption((short)-1);
        testingShort = testingDrink.GetMinuteOfConsumption();
        if (testingShort != 0){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkMessages.GetterSetterMessage(false, 4)
            );
        }
        else if (printAllMessages){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkMessages.GetterSetterMessage(true, 4)
            );
        }

        //  - Case 5, abnormal values, hour of consumption get and set
        testingDrink.SetHourOfConsumption((short)-1);
        testingShort = testingDrink.GetHourOfConsumption();
        if (testingShort != 0){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkMessages.GetterSetterMessage(false, 5)
            );
        }
        else if (printAllMessages){
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkMessages.GetterSetterMessage(true, 5)
            );
        }

    }


}
