/// <summary>
///  File: DrinkType.java
///     Description: Contains the class DrinkType which is meant
///        to simulate an enum containing 4 members, Beer, Wine,
///        Liquor, and Cocktail respectively
///
/// </summary>

package com.example.alcoholconsumptiontracker.system;


import android.widget.ArrayAdapter;

public class DrinkType {

    private short value;

    /// Represents static values corresponding to drink value
    //  DrinkType as a short when value isn't in drinkTypeValues
    private static short unknownDrinkValue = -1;
    //  DrinkType as a String
    private static String[] drinkTypeValueNames = new String[]{"Beer", "Wine", "Cocktail", "Liquor"};
    //  DrinkType as a String when value isn't in drinkTypeValueNames
    private static String unknownDrinkValueName = "n/a";

    /// <summary>
    ///  Default constructor
    /// </summary>
    public DrinkType(){
        this.value = (char)0;
    }

    /// <summary>
    ///  Getter. Returns value as a string
    /// </summary>
    public String Get(){
       return DrinkTypeFromShort(this.value);
    }
    public short GetValue(){return this.value;}
    /// <summary>
    ///  Setter
    /// </summary>
    public void Set(short value){
        if (value < 0) this.value = 0;
        else this.value = value;
    }
    public void Set(String value){
        if (DrinkTypeFromString(value) < 0) this.value = 0;
        else this.value = DrinkTypeFromString(value);
    }


    /// <summary>
    ///  Given an inputted short, this function returns the string Drink Type
    ///     the short represents. If no string is represented by the inputted value,
    ///     "n/a" is returned instead.
    /// </summary>
    public static String DrinkTypeFromShort(short value){
        if (value < DrinkType.drinkTypeValueNames.length){
            return DrinkType.drinkTypeValueNames[value];
        }
        else return DrinkType.unknownDrinkValueName;
    }

    /// <summary>
    ///  Given an inputted string, this function returns the short Drink Type
    ///     the string represents. If no short is represented by the inputted value,
    ///     -1 is returned instead.
    /// </summary>
    public static short DrinkTypeFromString(String value){
        for (int i = 0; i < DrinkType.drinkTypeValueNames.length; i++){
            if (DrinkType.drinkTypeValueNames[i].equals(value)){
                return (short)i;
            }
        }
        return DrinkType.unknownDrinkValue;
    }

    /// <summary>
    ///     Returns the array of all possible drink types as names
    /// </summary>
    public static String[] DrinkTypeNames(){
        return DrinkType.drinkTypeValueNames;
    }
}
