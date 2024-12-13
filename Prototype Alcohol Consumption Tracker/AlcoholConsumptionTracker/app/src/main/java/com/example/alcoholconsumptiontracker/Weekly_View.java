package com.example.alcoholconsumptiontracker;

import static com.example.alcoholconsumptiontracker.R.*;

import android.annotation.SuppressLint;
import android.os.Bundle;

import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.TextView;

import com.github.mikephil.charting.charts.LineChart;
import com.github.mikephil.charting.components.XAxis;
import com.github.mikephil.charting.components.YAxis;
import com.github.mikephil.charting.data.Entry;
import com.github.mikephil.charting.data.LineData;
import com.github.mikephil.charting.data.LineDataSet;
import com.github.mikephil.charting.formatter.ValueFormatter;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Locale;

public class Weekly_View extends Fragment {

    private LineChart lineChart;
    private TextView totalTextView;
    private ImageButton buttonCalories, buttonUnits, buttonBAC, buttonMoney;

    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    private String mParam1;
    private String mParam2;

    public Weekly_View() {
        super(layout.fragment_weekly__view);
    }

    public static Weekly_View newInstance(String param1, String param2) {
        Weekly_View fragment = new Weekly_View();
        Bundle args = new Bundle();
        fragment.setArguments(args);
        return fragment;
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

        View view = inflater.inflate(R.layout.fragment_weekly__view, container, false);

        // Get current week start and end dates
        EditText editTextWeekRange = view.findViewById(R.id.editTextWeekRange);
        String weekRange = getCurrentWeekRange();
        editTextWeekRange.setText(weekRange);

        // Find views
        lineChart = view.findViewById(R.id.line_chart_w);
        totalTextView = view.findViewById(R.id.total_value_w);
        buttonCalories = view.findViewById(R.id.button_calories_w);
        buttonUnits = view.findViewById(R.id.button_units_w);
        buttonBAC = view.findViewById(R.id.button_bac_w);
        buttonMoney = view.findViewById(R.id.button_money_w);

        // Set up event listeners
        buttonCalories.setOnClickListener(v ->
        { setupLineChart(getCaloriesData(),"Calories", buttonCalories);
        });
        buttonUnits.setOnClickListener(v ->
        { setupLineChart(getUnitsData(), "Units", buttonUnits);
        });
        buttonBAC.setOnClickListener(v ->
        { setupLineChart(getBACData(), "BAC", buttonBAC);
        });
        buttonMoney.setOnClickListener(v ->
        { setupLineChart(getMoneyData(),"Money", buttonMoney);
        });

        // Use calorie line chart as default
        setupLineChart(getCaloriesData(),"Calories", buttonCalories);


        return view;
    }

    // Gets the range of the current week
    private String getCurrentWeekRange() {

        Calendar calendar = Calendar.getInstance();

        // Set to the start of the week (e.g., Sunday)
        calendar.set(Calendar.DAY_OF_WEEK, calendar.getFirstDayOfWeek());
        SimpleDateFormat dateFormat = new SimpleDateFormat("MM/dd", Locale.getDefault());
        String startOfWeek = dateFormat.format(calendar.getTime());

        // Move to the end of the week (Saturday)
        calendar.add(Calendar.DAY_OF_WEEK, 6);
        String endOfWeek = dateFormat.format(calendar.getTime());

        return startOfWeek + " - " + endOfWeek;
    }

    // Set up the LineChart
    private void setupLineChart(ArrayList<Entry> values, String label, ImageButton activeButton) {
        LineDataSet lineDataSet = new LineDataSet(values, label);
        lineDataSet.setLineWidth(2f);
        lineDataSet.setCircleRadius(4f);
        lineDataSet.setColor(getResources().getColor(R.color.purple_500));
        lineDataSet.setCircleColor(getResources().getColor(R.color.teal_200));
        lineDataSet.setValueTextSize(10f);

        // Create a LineData object and set it to the chart
        LineData lineData = new LineData(lineDataSet);
        lineChart.setData(lineData);
        lineChart.animateXY(500, 500);
        lineChart.setScaleEnabled(true);
        lineChart.setPinchZoom(true);

        // Customize the X-axis for weekdays
        XAxis xAxis = lineChart.getXAxis();
        xAxis.setPosition(XAxis.XAxisPosition.BOTTOM);
        xAxis.setGranularity(1f); // One label per unit
        xAxis.setGranularityEnabled(true);
        xAxis.setValueFormatter(new weekdayValueFormatter()); // custom formatter
        YAxis rightAxis = lineChart.getAxisRight();
        rightAxis.setEnabled(false);

        // Refresh the chart
        lineChart.invalidate();

        // Calculate total
        float total = calculateTotal(values);
        if (activeButton == buttonMoney){
            totalTextView.setText("$" + total);
            lineChart.getDescription().setText("Money spent this week");
        }
        if (activeButton == buttonBAC){
            totalTextView.setText(total + "%");
            lineChart.getDescription().setText("BAC levels this week");
        }
        if (activeButton == buttonCalories){
            totalTextView.setText(total + " ");
            lineChart.getDescription().setText("Calories consumed this week");
        }
        if (activeButton == buttonUnits){
            totalTextView.setText("#" + total);
            lineChart.getDescription().setText("Units drank this week");
        }

    }

    private ArrayList<Entry> getCaloriesData() {
        ArrayList<Entry> values = new ArrayList<>();
        values.add(new Entry(1, 0)); // Sunday
        values.add(new Entry(2, 0)); // Monday
        values.add(new Entry(3, 100)); // Tuesday
        values.add(new Entry(4, 0)); // Wednesday
        values.add(new Entry(5, 0)); // Thursday
        values.add(new Entry(6, 350)); // Friday
        values.add(new Entry(7, 250)); // Saturday
        return values;
    }

    private ArrayList<Entry> getUnitsData() {
        ArrayList<Entry> values = new ArrayList<>();
        values.add(new Entry(1, 0)); // Sunday
        values.add(new Entry(2, 0)); // Monday
        values.add(new Entry(3, 1)); // Tuesday
        values.add(new Entry(4, 0)); // Wednesday
        values.add(new Entry(5, 0)); // Thursday
        values.add(new Entry(6, 3)); // Friday
        values.add(new Entry(7, 2)); // Saturday
        return values;
    }

    private ArrayList<Entry> getBACData() {
        ArrayList<Entry> values = new ArrayList<>();
        values.add(new Entry(1, 0)); // Sunday
        values.add(new Entry(2, 0)); // Monday
        values.add(new Entry(3, 0.08F)); // Tuesday
        values.add(new Entry(4, 0)); // Wednesday
        values.add(new Entry(5, 0)); // Thursday
        values.add(new Entry(6, 0.12F)); // Friday
        values.add(new Entry(7, 0.11F)); // Saturday
        return values;
    }

    private ArrayList<Entry> getMoneyData() {
        ArrayList<Entry> values = new ArrayList<>();
        values.add(new Entry(1, 0)); // Sunday
        values.add(new Entry(2, 0)); // Monday
        values.add(new Entry(3, 5.5F)); // Tuesday
        values.add(new Entry(4, 0)); // Wednesday
        values.add(new Entry(5, 0)); // Thursday
        values.add(new Entry(6, 21)); // Friday
        values.add(new Entry(7, 18.50F)); // Saturday
        return values;
    }

    // Custom label values for x-axis
    private static class weekdayValueFormatter extends ValueFormatter {
        private final String[] weekdays = {"S", "M", "T", "W", "T", "F", "S"};

        @Override
        public String getFormattedValue(float value) {
            int index = (int) value - 1; // Convert value to index
            if (index >= 0 && index < weekdays.length) {
                return weekdays[index];
            }

            // Edge case
            else {
                return "";
            }
        }
    }

    private float calculateTotal(ArrayList<Entry> values) {
        float total = 0.0F;
        for (Entry entry : values) {
            total += entry.getY(); // Sum up the Y values
        }

        total = (Math.round(total * 100F) / 100F);

        return total;
    }
}