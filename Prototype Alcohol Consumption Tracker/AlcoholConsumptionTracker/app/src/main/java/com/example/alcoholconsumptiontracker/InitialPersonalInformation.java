package com.example.alcoholconsumptiontracker;

import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;


public class InitialPersonalInformation extends Fragment {

    public class personalInformation {
        // Personal information intake thingy -- Constructor
        private String userName;
        private int weight;
        private double height;
        private char sex;
        private int age;

        public personalInformation(String userName, int weight, double height, char sex, int age) {
            this.userName = userName;
            this.weight = weight;
            this.height = height;
            this.sex = sex;
            this.age = age;
        }

        // Setterz and getterz

        // Getters - when u wanna know
        public String getName() {
            return userName;
        }
        public int getWeight() {
            return weight;
        }
        public double getHeight() {
            return height;
        }
        public char getSex() {
            return sex;
        }
        public int getAge() {
            return age;
        }

        // Setters - when u wanna change
        public void setUserName(String newUserName) {
            this.userName = newUserName;
        }

        public void setWeight(int newWeight) {
            this.weight = newWeight;
        }

        public void setHeight(int newHeight) {
            this.height = newHeight;
        }

        public void setSex(char newSex) {
            this.sex = newSex;
        }

        public void setAge(int newAge) {
            this.age = newAge;
        }

    }



    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_initial_personal_information, container, false);
    }
}