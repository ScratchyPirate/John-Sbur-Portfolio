package com.example.alcoholconsumptiontracker;

import android.content.Context;
import android.content.DialogInterface;
import android.net.Uri;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;

import androidx.appcompat.app.AlertDialog;
import androidx.fragment.app.Fragment;

import com.example.alcoholconsumptiontracker.system.Drink;
import com.example.alcoholconsumptiontracker.system.DrinkTemplate;

import java.io.File;

public class Drink_Tab extends Fragment {

    /// Globals
    private static ImageButton backButton;
    private static ListView drinkList;
    // Represents the help box
    private static ImageView helpSquare;
    private static TextView helpTextbox;
    private static int selectedDrinkPosition;

    public Drink_Tab() {
        super(R.layout.alc_logging_drink_tab);
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.alc_logging_drink_tab, container, false);


        // Reset globals
        Drink_Tab.selectedDrinkPosition = -1;
        Drink_Tab.backButton = null;
        Drink_Tab.drinkList = null;
        Drink_Tab.helpSquare = null;
        Drink_Tab.helpTextbox = null;

        // Initialize back button
        Drink_Tab.backButton = root.findViewById(R.id.alcLoggingBackButton2);
        Drink_Tab.backButton.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        MainActivity.ChangeActiveFragment(R.id.logging_intermediary, MainActivity.FragmentAnimationType.FADE);
                    }
                }
        );

        // Initialize the help box
        Drink_Tab.helpTextbox = root.findViewById(R.id.alcDrinkLogHelpTextbox);
        Drink_Tab.helpSquare = root.findViewById(R.id.alcDrinkLogHelpSquare);

        // Initialize drink list
        Drink_Tab.drinkList = root.findViewById(R.id.drinkTabListView);
        // If template exist, populate the listview. Otherwise, display the help text
        if (!MainActivity.GetDrinkList().isEmpty()){
            Drink_Tab.drinkList.setVisibility(View.VISIBLE);
            Drink_Tab.helpSquare.setVisibility(View.INVISIBLE);
            Drink_Tab.helpTextbox.setVisibility(View.INVISIBLE);
            Drink_Tab.drinkList.setAdapter(new drinkTabListAdapter(MainActivity.GetContentView().getContext()));
        }
        else{
            Drink_Tab.drinkList.setVisibility(View.INVISIBLE);
            Drink_Tab.helpSquare.setVisibility(View.VISIBLE);
            Drink_Tab.helpTextbox.setVisibility(View.VISIBLE);
        }
        return root;
    }

    ///
    /// Gets or sets the drink position
    ///
    public static void SetSelectedDrinkPosition(int newPosition){
        if (newPosition < 0 || newPosition >= Drink_Tab.drinkList.getCount()){
            Drink_Tab.selectedDrinkPosition = -1;
        }
        else{
            Drink_Tab.selectedDrinkPosition = newPosition;
        }
    }
    public static int GetSelectedDrinkPosition(){
        return Drink_Tab.selectedDrinkPosition;
    }

    private class drinkTabListAdapter extends BaseAdapter {

        private Context listContext;
        private LayoutInflater inflater;
        private Drink[] drinkList;

        public drinkTabListAdapter(Context context) {
            this.drinkList = MainActivity.GetDrinkList().toArray(new Drink[0]);
            this.listContext = context;
        }

        public drinkTabListAdapter(Context context, Drink[] newDrinkList) {
            this.drinkList = newDrinkList;
            this.listContext = context;
        }

        public int getCount() {
            return this.drinkList.length;
        }

        public Object getItem(int arg0) {
            // TODO Auto-generated method stub
            return null;
        }

        public long getItemId(int position) {
            // TODO Auto-generated method stub
            return position;
        }

        ///  Gets a row from the list as its different objects. Returns the result as a view
        public View getView(int position, View convertView, ViewGroup parent) {

            inflater = (LayoutInflater) this.listContext.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            View row;
            row = inflater.inflate(R.layout.drink_tab_list_item, parent, false);

            TextView drinkName;
            TextView drinkType;
            TextView drinkServings;
            TextView drinkCalories;
            TextView drinkPrice;
            TextView drinkTime;
            TextView drinkOccasion;
            ImageView drinkImage;
            ImageButton deleteDrinkButton;

            // Name
            drinkName = (TextView) row.findViewById(R.id.drinkTabListItemName);
            drinkName.setText(
                    this.drinkList[position].GetName()
            );

            // Type
            drinkType = (TextView) row.findViewById(R.id.drinkTabListItemType);
            drinkType.setText(
                    this.drinkList[position].GetType().Get()
            );

            // Servings
            drinkServings = (TextView) row.findViewById(R.id.drinkTabListItemServings);
            drinkServings.setText(
                    Short.toString(this.drinkList[position].GetServings())
            );

            // Calories
            drinkCalories = (TextView) row.findViewById(R.id.drinkTabListItemCalories);
            drinkCalories.setText(
                    Double.toString(
                            Math.round(this.drinkList[position].GetCalories() * 100.0) / 100.0
                    )
            );

            // Price
            drinkPrice = (TextView) row.findViewById(R.id.drinkTabListItemPrice);
            drinkPrice.setText(
                    Double.toString(
                            Math.round(this.drinkList[position].GetPrice() * 100.0) / 100.0
                    )
            );

            // Occasion
            drinkPrice = (TextView) row.findViewById(R.id.drinkTabListItemOccasion);
            drinkPrice.setText(
                    this.drinkList[position].GetOccasion()
            );

            // Time
            drinkPrice = (TextView) row.findViewById(R.id.drinkTabListItemTime);
            drinkPrice.setText(
                    MainActivity.FormatTimeString(
                            this.drinkList[position].GetHourOfConsumption(),
                            this.drinkList[position].GetMinuteOfConsumption()
                    )
            );

            // Image
            drinkImage = (ImageView) row.findViewById(R.id.drinkTabListItemImage);
            if (!this.drinkList[position].GetImageFilePath().isEmpty()){
                drinkImage.setImageAlpha(255);
                drinkImage.setImageURI(Uri.fromFile(
                        new File(this.drinkList[position].GetImageFilePath())
                ));
            }
            else{
                drinkImage.setImageAlpha(0);
            }


            // Delete button
            deleteDrinkButton = (ImageButton) row.findViewById(R.id.drinkTabListItemDeleteButton);
            deleteDrinkButton.setTag(position);
            deleteDrinkButton.setOnClickListener(
                    new View.OnClickListener() {
                        @Override
                        public void onClick(View v) {

                            int drinkPosition = (int)v.getTag();
                            Drink_Tab.SetSelectedDrinkPosition(drinkPosition);

                            new AlertDialog.Builder(MainActivity.GetContentView().getContext())
                                    .setMessage(
                                            "Delete this drink?")
                                    .setPositiveButton(
                                            "Delete",
                                            new DialogInterface.OnClickListener() {
                                                @Override
                                                public void onClick(DialogInterface dialog, int which) {
                                                    // Remove the template
                                                    MainActivity.GetDrinkList().remove(Drink_Tab.GetSelectedDrinkPosition());
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
                                                    Drink_Tab.SetSelectedDrinkPosition(-1);
                                                }
                                            }
                                    )
                                    .show();
                        }
                    }
            );

            return row;

        }
    }
}