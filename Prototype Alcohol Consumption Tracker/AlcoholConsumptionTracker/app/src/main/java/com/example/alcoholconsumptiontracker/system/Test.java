/// <summary>
///     File: Test.java
///         Description: Contains the methods used for testing different classes
///             and systems within the app.
/// </summary>
package com.example.alcoholconsumptiontracker.system;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

///
///  Class responsible for testing each class's test methods
///
public class Test {

    ///
    ///  Tests all methods present from all classes.
    ///     Always prints failure cases
    ///     Optionally prints success cases
    ///  printAllMessages:
    ///     If true, enables printing of all test messages for when a method passes or fails
    ///     If false, only method failure messages print.
    ///  testContext:
    ///     The context for which backend functions are tested.
    ///
    public static void TestAll(boolean printAllMessages, Context testContext){

        // Notify begin testing
        Log.d(Universals.TestMessages.TestMessageTag, "-------Begin Testing-------");

        // ---- Backend
        // Database Manager Methods
        DatabaseManager.TestInitializeDatabase(printAllMessages, testContext);

        // ---- Alcohol Logging
        // DrinkTemplate Methods
        DrinkTemplate.TestProduceDrink(printAllMessages);

        // Drink Methods
        Drink.TestGettersAndSetters(printAllMessages);

        // DrinkTemplateManager Methods
        DrinkTemplateManager.TestPutTemplate(printAllMessages);
        DrinkTemplateManager.TestModifyTemplate(printAllMessages);
        DrinkTemplateManager.TestRemoveTemplate(printAllMessages);
        DrinkTemplateManager.TestReadWriteTemplateList(printAllMessages, testContext);

        // ----
        // Notify end testing
        Log.d(Universals.TestMessages.TestMessageTag, "-------End Testing-------");

    }
}
