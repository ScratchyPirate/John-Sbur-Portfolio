/// <summary>
///  File: DrinkTemplate.java
///     Description: Contains the class and methods for drink templates. Templates enable easier creation of drinks and
///         allow for programmable drinks in the app.
/// </summary>
///
package com.example.alcoholconsumptiontracker.system;

import android.util.Log;

public class DrinkTemplate {

    //
    private String name; // Name of drink
    private DrinkType type; // Type of drink
    private short servings; // Number of servings ( 0 >= )
    private float aPV; // Alcohol Per volume ( 0.0 >= )
    private float price; // Price ( 0.0 >= , dollars USD)
    private float calories; // Calories (0.0 >=, kcal)
    private String imageFilePath; // File path to image within internal system.

    ///
    /// Constructors
    ///
    // Default
    public DrinkTemplate(){
        this.name = Universals.General.EmptyString();
        this.type = new DrinkType();
        this.servings = 0;
        this.aPV = 0;
        this.price = 0;
        this.imageFilePath = Universals.General.EmptyString();
    }

    ///
    ///  Setters and Getters
    ///
    // Name
    public String GetName(){return this.name;}
    public void SetName(String newName){this.name = newName;}
    // Type
    public DrinkType GetType(){return this.type;}
    public void SetType(short newType){
        this.type.Set(newType);
    }
    // Servings
    public short GetServings(){
        return this.servings;
    }
    public void SetServings(short newServings){
        if (newServings < 0) this.servings = 0;
        else this.servings = newServings;
    }
    // APV
    public float GetAPV(){return this.aPV;}
    public void SetAPV(float newAPV){
        if (newAPV < 0) this.aPV = 0;
        else this.aPV = newAPV;
    }
    // Price
    public float GetPrice(){return this.price;}
    public void SetPrice(float newPrice){
        if (newPrice < 0) this.price = 0;
        else this.price = newPrice;
    }
    // Calories
    public float GetCalories(){return this.calories;}
    public void SetCalories(float newCalories){
        if (newCalories < 0) this.calories = 0;
        else this.calories = newCalories;
    }
    // Image
    public String GetImageFilePath(){return this.imageFilePath;}
    public void SetImageFilePath(String newFilePath){ this.imageFilePath = newFilePath;}

    ///
    /// Methods
    ///
    /// <summary>
    ///  Given an occasion, hour, and minute, produces a new drink object using
    ///     the contents of this drink template.
    /// </summary>
    public Drink ProduceDrink(String occasion, short hour, short minute){
        Drink newDrink = new Drink(this);
        newDrink.SetOccasion(occasion);
        newDrink.SetHourOfConsumption(hour);
        newDrink.SetMinuteOfConsumption(minute);
        return newDrink;
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

    // Test ProduceDrink method
    public static void TestProduceDrink(boolean printAllMessages){

        DrinkTemplate temp = new DrinkTemplate();
        Drink tempDrink;

        // Non-Exception Tests
        //  -Case 1, produce drink from valid template
        if (temp.ProduceDrink("test string", (short)1, (short)1) != null){
            if (printAllMessages) Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkTemplateMessages.ProduceDrinkMessage(true, 1));
        }
        else{
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkTemplateMessages.ProduceDrinkMessage(false, 1));
        }
        //  -Case 2, produce drink with initialized values
        temp.SetName("test");
        temp.SetAPV((float)1.1);
        temp.SetPrice((float)1.1);
        temp.SetType((short)1);
        temp.SetServings((short)1);
        temp.SetCalories((float)1.1);
        if (temp.ProduceDrink("test string", (short)1, (short)1) != null){
            if (printAllMessages) Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkTemplateMessages.ProduceDrinkMessage(true, 2));
        }
        else{
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkTemplateMessages.ProduceDrinkMessage(false, 2));
        }

        //  -Case 3, produce drink with abnormal initialized values. Verify values implemented into drink
        temp.SetName("test");
        temp.SetAPV((float)-1);
        temp.SetPrice((float)-1);
        temp.SetType((short)-1);
        temp.SetServings((short)-1);
        temp.SetCalories((float)-1.1);
        temp.SetImageFilePath("test");
        tempDrink = temp.ProduceDrink("test string", (short)-1, (short)-1);
        if (tempDrink.GetAPV() == 0
                && tempDrink.GetName().equals("test")
                && tempDrink.GetPrice() == 0
                && tempDrink.GetOccasion().equals("test string")
                && tempDrink.GetAPV() == 0
                && tempDrink.GetType().Get().equals(DrinkType.DrinkTypeFromShort((short)0))
                && tempDrink.GetServings() == 0
                && tempDrink.GetHourOfConsumption() == 0
                && tempDrink.GetMinuteOfConsumption() == 0
                && tempDrink.GetCalories() == 0
                && tempDrink.GetImageFilePath().equals("test")
        ){
            if (printAllMessages) Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkTemplateMessages.ProduceDrinkMessage(true, 3));
        }
        else{
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkTemplateMessages.ProduceDrinkMessage(false, 3));
        }

        //  -Case 4, produce drink with normal initialized values. Verify values implemented into drink
        temp.SetName("test");
        temp.SetAPV((float)1);
        temp.SetPrice((float)1);
        temp.SetType((short)1);
        temp.SetServings((short)1);
        temp.SetCalories((float)1.1);
        tempDrink = temp.ProduceDrink("test string", (short)1, (short)1);
        if (    tempDrink.GetAPV() == 1.0
                && tempDrink.GetName().equals("test")
                && tempDrink.GetPrice() == 1.0
                && tempDrink.GetOccasion().equals("test string")
                && tempDrink.GetType().Get().equals(DrinkType.DrinkTypeFromShort((short)1))
                && tempDrink.GetServings() == 1
                && tempDrink.GetHourOfConsumption() == 1
                && tempDrink.GetMinuteOfConsumption() == 1
                && tempDrink.GetCalories() == (float)1.1
        ){
            if (printAllMessages) Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkTemplateMessages.ProduceDrinkMessage(true, 4));
        }
        else{
            Log.d(
                    Universals.TestMessages.TestMessageTag,
                    Universals.TestMessages.DrinkTemplateMessages.ProduceDrinkMessage(false, 4));
        }

        // Exception Tests

    }
}
