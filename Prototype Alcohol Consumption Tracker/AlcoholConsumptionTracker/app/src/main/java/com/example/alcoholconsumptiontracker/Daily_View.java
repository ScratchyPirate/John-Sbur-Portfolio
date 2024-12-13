package com.example.alcoholconsumptiontracker;

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
import android.widget.ImageView;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

import com.github.mikephil.charting.charts.LineChart;
import com.github.mikephil.charting.components.XAxis;
import com.github.mikephil.charting.components.YAxis;
import com.github.mikephil.charting.data.Entry;
import com.github.mikephil.charting.data.LineData;
import com.github.mikephil.charting.data.LineDataSet;
import com.github.mikephil.charting.formatter.ValueFormatter;
import com.github.mikephil.charting.interfaces.datasets.ILineDataSet;

import java.util.ArrayList;


public class Daily_View extends Fragment {

    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    private String mParam1;
    private String mParam2;

    public Daily_View() {
        super(R.layout.fragment_daily__view);
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param param1 Parameter 1.
     * @param param2 Parameter 2.
     * @return A new instance of fragment Daily_View.
     */
    // TODO: Rename and change types and number of parameters
    public static Daily_View newInstance(String param1, String param2) {
        Daily_View fragment = new Daily_View();
        Bundle args = new Bundle();
        args.putString(ARG_PARAM1, param1);
        args.putString(ARG_PARAM2, param2);
        fragment.setArguments(args);
        return fragment;
    }

    private ImageView imageViewDataType;
    private LineChart lineChart;
    private TextView totalTextView;
    private ImageButton buttonCalories, buttonUnits, buttonBAC, buttonMoney;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

        View view = inflater.inflate(R.layout.fragment_daily__view, container, false);

        imageViewDataType = view.findViewById(R.id.imageViewDataType);

        //  Get id and set the current date to the EditText
        EditText editTextDate = view.findViewById(R.id.editTextDate2);
        String currentDate = new SimpleDateFormat("MM/dd/yyyy", Locale.getDefault()).format(new Date());
        editTextDate.setText(currentDate);

        // Find views
        lineChart = view.findViewById(R.id.lineChart);
        totalTextView = view.findViewById(R.id.total_value);
        buttonCalories = view.findViewById(R.id.button_calories);
        buttonUnits = view.findViewById(R.id.button_units);
        buttonBAC = view.findViewById(R.id.button_bac);
        buttonMoney = view.findViewById(R.id.button_money);

        // Set up event listeners
        buttonCalories.setOnClickListener(v -> {
                setupLineChart(getCaloriesData(),"Calories", buttonCalories);
                updateImageView(R.drawable.totalcaloriesbar);
            });
        buttonUnits.setOnClickListener(v -> {
                setupLineChart(getUnitsData(), "Units", buttonUnits);
                updateImageView(R.drawable.totalunitsbar);
            });
        buttonBAC.setOnClickListener(v -> {
                setupLineChart(getBACData(), "BAC", buttonBAC);
                updateImageView(R.drawable.totalbacbar);
            });
        buttonMoney.setOnClickListener(v -> {
                setupLineChart(getMoneyData(),"Money", buttonMoney);
                updateImageView(R.drawable.totalpricebar);
            });

        // Use calorie line chart as default
        setupLineChart(getCaloriesData(),"Calories", buttonCalories);
        updateImageView(R.drawable.totalcaloriesbar);

        return view;
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
        xAxis.setGranularity(1f);
        xAxis.setGranularityEnabled(true);
        xAxis.setValueFormatter(new Daily_View.dayValueFormatter()); // custom formatter
        YAxis rightAxis = lineChart.getAxisRight();
        rightAxis.setEnabled(false);

        // Refresh the chart
        lineChart.invalidate();

        // Calculate total
        float total = calculateTotal(values);
        if (activeButton == buttonMoney){
            totalTextView.setText("$" + total);
            lineChart.getDescription().setText("Money spent today");
        }
        if (activeButton == buttonBAC){
            totalTextView.setText(total + "%");
            lineChart.getDescription().setText("BAC levels today");
        }
        if (activeButton == buttonCalories){
            totalTextView.setText(total + " ");
            lineChart.getDescription().setText("Calories consumed today");
        }
        if (activeButton == buttonUnits){
            totalTextView.setText("#" + total);
            lineChart.getDescription().setText("Units drank today");
        }

    }

    private ArrayList<Entry> getCaloriesData() {
        ArrayList<Entry> values = new ArrayList<>();
        values.add(new Entry(1, 0));
        values.add(new Entry(2, 0));
        values.add(new Entry(3, 0));
        values.add(new Entry(4, 0));
        values.add(new Entry(5, 0));
        values.add(new Entry(6, 0));
        values.add(new Entry(7, 200));
        values.add(new Entry(8, 155));
        values.add(new Entry(9, 0));
        values.add(new Entry(10, 150));
        values.add(new Entry(11, 0));
        values.add(new Entry(12, 150));
        values.add(new Entry(13, 0));
        return values;
    }

    private ArrayList<Entry> getUnitsData() {
        ArrayList<Entry> values = new ArrayList<>();
        values.add(new Entry(1, 0));
        values.add(new Entry(2, 0));
        values.add(new Entry(3, 0));
        values.add(new Entry(4, 0));
        values.add(new Entry(5, 0));
        values.add(new Entry(6, 0));
        values.add(new Entry(7, 2));
        values.add(new Entry(8, 1));
        values.add(new Entry(9, 0));
        values.add(new Entry(10, 2));
        values.add(new Entry(11, 0));
        values.add(new Entry(12, 1));
        values.add(new Entry(13, 0));
        return values;
    }

    private ArrayList<Entry> getBACData() {
        ArrayList<Entry> values = new ArrayList<>();
        values.add(new Entry(1, 0));
        values.add(new Entry(2, 0));
        values.add(new Entry(3, 0));
        values.add(new Entry(4, 0));
        values.add(new Entry(5, 0));
        values.add(new Entry(6, 0));
        values.add(new Entry(7, 0));
        values.add(new Entry(8, 0.11F));
        values.add(new Entry(9, 0));
        values.add(new Entry(10, 0.08F));
        values.add(new Entry(11, 0));
        values.add(new Entry(12, 0.12F));
        values.add(new Entry(13, 0));
        return values;
    }

    private ArrayList<Entry> getMoneyData() {
        ArrayList<Entry> values = new ArrayList<>();
        values.add(new Entry(1, 0));
        values.add(new Entry(2, 0));
        values.add(new Entry(3, 0));
        values.add(new Entry(4, 0));
        values.add(new Entry(5, 0));
        values.add(new Entry(6, 0));
        values.add(new Entry(7, 5.12F));
        values.add(new Entry(8, 10));
        values.add(new Entry(9, 0));
        values.add(new Entry(10, 0));
        values.add(new Entry(11, 23.99F));
        values.add(new Entry(12, 0));
        values.add(new Entry(13, 0));
        return values;
    }

    // Custom label values for x-axis
    private static class dayValueFormatter extends ValueFormatter {
        private final String[] weekdays = {"3pm", "4pm", "5pm", "6pm", "7pm", "8pm", "9pm", "10pm", "11pm", "12am", "1am", "2am", "3am"};

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

    // Calculates total number of given metric
    private float calculateTotal(ArrayList<Entry> values) {
        float total = 0.0F;
        for (Entry entry : values) {
            total += entry.getY(); // Sum up the Y values
        }

        total = (Math.round(total * 100F) / 100F);

        return total;
    }

    // Update images for totals
    private void updateImageView(int imageResId) {

        imageViewDataType.setImageResource(imageResId);
    }

}
