package com.example.alcoholconsumptiontracker;

import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageButton;

import androidx.fragment.app.Fragment;

public class Logging_Intermediary extends Fragment {


    // Represents the button to transition to alc programming
    private static ImageButton sendToAlcProgramming;

    // Represents the button to transition to alc select
    private static ImageButton sendToAlcSelect;

    // Represents the button to transition to drink tab
    private static ImageButton sendToDrinkTab;

    public Logging_Intermediary() {
        super(R.layout.logging_intermediary);
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View root = inflater.inflate(R.layout.logging_intermediary, container, false);


        // Set globals to null
        Logging_Intermediary.sendToAlcSelect = null;
        Logging_Intermediary.sendToAlcProgramming = null;


        // Initialize send to alc programming button
        Logging_Intermediary.sendToAlcProgramming = root.findViewById(R.id.loggingIntermediaryToAlcProgramming);
        Logging_Intermediary.sendToAlcProgramming.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        MainActivity.ChangeActiveFragment(R.id.alc_Programming, MainActivity.FragmentAnimationType.NONE);
                    }
                }
        );

        // Initialize send to alc select button
        Logging_Intermediary.sendToAlcSelect = root.findViewById(R.id.loggingIntermediaryToAlcSelect);
        Logging_Intermediary.sendToAlcSelect.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        MainActivity.ChangeActiveFragment(R.id.alc_Select, MainActivity.FragmentAnimationType.NONE);
                    }
                }
        );

        // Initialize send to drink tab button
        Logging_Intermediary.sendToDrinkTab = root.findViewById(R.id.loggingIntermediaryToDrinkTab);
        Logging_Intermediary.sendToDrinkTab.setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        MainActivity.ChangeActiveFragment(R.id.alc_drink_tab, MainActivity.FragmentAnimationType.NONE);
                    }
                }
        );

        return root;
    }
}
