package com.example.alcoholconsumptiontracker.ui.home;

import android.os.Handler;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProvider;

import com.example.alcoholconsumptiontracker.Daily_View;
import com.example.alcoholconsumptiontracker.MainActivity;
import com.example.alcoholconsumptiontracker.R;
import com.example.alcoholconsumptiontracker.databinding.FragmentHomeBinding;
import android.text.format.DateFormat;
import androidx.fragment.app.Fragment;

import java.util.Calendar;

public class HomeFragment extends Fragment {

    private FragmentHomeBinding binding;
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";
    private final Handler handler = new Handler();

    private String mParam1;
    private String mParam2;

    public HomeFragment() {
        super(R.layout.fragment_home);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_home, container, false);

        TextView textViewDate = view.findViewById(R.id.textViewDate);
        TextView textViewTime = view.findViewById(R.id.textViewTime);

        // Update current time
        updateDate(textViewDate);
        updateTime(textViewTime);

        return view;
    }


    @Override
    public void onViewCreated(View view, Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);


        // Set up nav for daily view
        view.findViewById(R.id.button_to_personal_info).setOnClickListener(v -> {
            navigateToFragment(R.id.personal_Info);
        });

        // Set up nav for weekly view
        view.findViewById(R.id.button_to_create_goal).setOnClickListener(v -> {
            navigateToFragment(R.id.personal_Goals);
        });


    }

    // Helper method to navigate to a fragment
    private void navigateToFragment(int fragmentId) {
            MainActivity.ChangeActiveFragment(fragmentId, MainActivity.FragmentAnimationType.NONE);
    }


    public static HomeFragment newInstance(String param1, String param2) {
        HomeFragment fragment = new HomeFragment();
        Bundle args = new Bundle();
        args.putString(ARG_PARAM1, param1);
        args.putString(ARG_PARAM2, param2);
        fragment.setArguments(args);
        return fragment;
    }

    // Updates to current date
    private void updateDate(TextView textViewDate) {
        Calendar calendar = Calendar.getInstance();
        String dayOfWeek = DateFormat.format("EEEE", calendar).toString();
        String dayOfMonth = DateFormat.format("d", calendar).toString();
        String monthName = DateFormat.format("MMMM", calendar).toString();

        String formattedDate = dayOfWeek + ", " + monthName + " " + dayOfMonth;
        textViewDate.setText(formattedDate);
    }

    // Updates current clock time
    private void updateTime(TextView textViewTime) {
        Calendar calendar = Calendar.getInstance();
        String currentTime = DateFormat.format("hh:mm:ss a", calendar).toString();
        textViewTime.setText(currentTime);

        // Updates every second
        new Handler().postDelayed(() -> updateTime(textViewTime), 1000);
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }
}