package com.example.alcoholconsumptiontracker.ui.notifications;

import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProvider;

import com.example.alcoholconsumptiontracker.MainActivity;
import com.example.alcoholconsumptiontracker.databinding.FragmentNotificationsBinding;
import androidx.navigation.Navigation;
import com.example.alcoholconsumptiontracker.R;

public class NotificationsFragment extends Fragment {

    private FragmentNotificationsBinding binding;

    public NotificationsFragment() {

    }

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {
        NotificationsViewModel notificationsViewModel =
                new ViewModelProvider(this).get(NotificationsViewModel.class);

        binding = FragmentNotificationsBinding.inflate(inflater, container, false);
        View root = binding.getRoot();

        return root;
    }


    @Override
    public void onViewCreated(View view, Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);


        // Set up nav for daily view
        view.findViewById(R.id.button_to_daily_view).setOnClickListener(v -> {
            navigateToFragment(R.id.daily_View);
        });

        // Set up nav for weekly view
        view.findViewById(R.id.button_to_weekly_view).setOnClickListener(v -> {
            navigateToFragment(R.id.weekly_View);
        });

        // Set up nav for monthly view
        view.findViewById(R.id.button_to_monthly_view).setOnClickListener(v -> {
            navigateToFragment(R.id.monthly_View);
        });

    }

    // Helper method to navigate to a fragment
    private void navigateToFragment(int fragmentId) {

            MainActivity.ChangeActiveFragment(fragmentId, MainActivity.FragmentAnimationType.NONE);
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }
}