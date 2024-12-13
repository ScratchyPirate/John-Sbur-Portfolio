package com.example.alcoholconsumptiontracker.system;

import android.util.Log;

import com.example.alcoholconsumptiontracker.Personal_Info;

public class PersonalInfoEntry {
    private static String userName;
    private static String weight;
    private static String sex;
    private static String age;

    public PersonalInfoEntry() {
        this.userName = null;
        this.weight = null;
        this.sex = null;
        this.age = null;
    }

    // Setterz and getterz

    // Getters - when u wanna know
    public static String getUserName() {
        return userName;
    }
    public static String getWeight() {
        return weight;
    }
    public static String getSex() {
        return sex;
    }
    public static String getAge() {
        return age;
    }

    // Setters - when u wanna change
    public static void setUserName(String newUserName) {
        PersonalInfoEntry.userName = newUserName;
    }

    public static void setWeight(String newWeight) {
        PersonalInfoEntry.weight = newWeight;
    }
    
    public static void setSex(String newSex) {
        PersonalInfoEntry.sex = newSex;
    }

    public static void setAge(String newAge) {
        PersonalInfoEntry.age = newAge;
    }

}
