package com.example.alcoholconsumptiontracker;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.Toast;

import com.example.alcoholconsumptiontracker.system.PersonalInfoEntry;

/**
 * A simple {@link Fragment} subclass.
 * create an instance of this fragment.
 */
public class Personal_Info extends Fragment {

    public static EditText nameInput;
    public static EditText ageInput;
    public static EditText sexInput;
    public static EditText weightInput;
    // public static EditText heightInput;

    public static PersonalInfoEntry savedPersonalInfo = new PersonalInfoEntry();
    public static String savedName;
    public static String savedWeight;
    public static String savedSex;
    public static String savedAge;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        // Inflate the layout for this fragment
        View root = inflater.inflate(R.layout.fragment_personal__info, container, false);

        ImageButton gearIcon = null;
        ImageButton finishIcon = null;

        nameInput = root.findViewById(R.id.edittextnameid);
        ageInput = root.findViewById(R.id.edittextageid);
        sexInput = root.findViewById(R.id.edittextsexid);
        weightInput = root.findViewById(R.id.edittextweightid);

//        if (nameInput != null && ageInput != null && sexInput != null && weightInput != null) {
//            if (savedName != null) nameInput.setText(savedName);
//            if (savedSex != null) sexInput.setText(savedSex);
//            if (savedAge > 0) ageInput.setText(String.valueOf(savedAge)); // Assuming 0 is an invalid age.
//            if (savedWeight > 0) weightInput.setText(String.valueOf(savedWeight)); // Assuming 0 is invalid.
//        }


        nameInput.setText(savedPersonalInfo.getUserName());
        weightInput.setText(savedPersonalInfo.getWeight());
        sexInput.setText(savedPersonalInfo.getSex());
        ageInput.setText(savedPersonalInfo.getAge());


        gearIcon = root.findViewById(R.id.geariconid);
        finishIcon = root.findViewById(R.id.finishbuttonid);

        nameInput.setEnabled(false);
        ageInput.setEnabled(false);
        sexInput.setEnabled(false);
        weightInput.setEnabled(false);


        gearIcon.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View view) {
                nameInput.setEnabled(true);
                ageInput.setEnabled(true);
                sexInput.setEnabled(true);
                weightInput.setEnabled(true);
            }
        });

        finishIcon.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View view) {

                try {
                    savedName = String.valueOf(nameInput.getText()).trim();
                    savedWeight = String.valueOf(weightInput.getText()).trim();
                    savedSex = String.valueOf(sexInput.getText()).trim();
                    savedAge = String.valueOf(ageInput.getText()).trim();


                    savedPersonalInfo.setUserName(savedName);
                    savedPersonalInfo.setWeight(savedWeight);
                    savedPersonalInfo.setSex(savedSex);
                    savedPersonalInfo.setAge(savedAge);

                    nameInput.setText(savedName);
                    weightInput.setText(savedWeight);
                    ageInput.setText(savedAge);
                    sexInput.setText(savedSex);

                    nameInput.setEnabled(false);
                    ageInput.setEnabled(false);
                    sexInput.setEnabled(false);
                    weightInput.setEnabled(false);

                } catch (NumberFormatException e) {
                    Log.e("Personal_Info", "Invalid input for numeric fields.", e);
                }
            }
        });
        return root;
    }
}