package com.example.alcoholconsumptiontracker.system;

import android.app.Person;

public class PersonalGoalEntry {
    private static String goalPrice;
    private static String goalServing;
    private static String goalCalorie;
    private static String writtenStatement;

    public PersonalGoalEntry() {
        this.goalPrice = null;
        this.goalServing = null;
        this.goalCalorie = null;
        this.writtenStatement = null;
    }

    // Setterz and getterz

    // Getters - when u wanna know
    public static String getGoalPrice() {
        return goalPrice;
    }
    public static String getGoalServing() {
        return goalServing;
    }
    public static String getGoalCalorie() {
        return goalCalorie;
    }
    public static String getWrittenStatement() {
        return writtenStatement;
    }

    // Setters - when u wanna change
    public static void setGoalPrice(String newGoalPrice) {
        PersonalGoalEntry.goalPrice = newGoalPrice;
    }

    public static void setGoalServings(String newGoalServing) {
        PersonalGoalEntry.goalServing = newGoalServing;
    }

    public static void setGoalCalorie(String newGoalCalorie) {
        PersonalGoalEntry.goalCalorie = newGoalCalorie;
    }

    public static void setWrittenStatement(String newWrittenStatement) {
        PersonalGoalEntry.writtenStatement = newWrittenStatement;
    }

}
