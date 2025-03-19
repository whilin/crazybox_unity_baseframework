using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using TMPro;

public class cxUIInputWidget : MonoBehaviour {
}

public class cxUIDateInputEvent : UnityEvent<DateTime> {}

public class cxUIDateInputWidget : cxUIInputWidget
{
    [SerializeField]
    private TMP_Dropdown yearDropdown;

    [SerializeField]
    private TMP_Dropdown monthDropdown;

    [SerializeField]
    private TMP_Dropdown dayDropdown;

    [SerializeField]
    public Vector2Int yearRange;

    public cxUIDateInputEvent OnInputEvent {get; private set;} = new cxUIDateInputEvent();
    public DateTime Value => GetDateTime();

    private List<string> years = new List<string>();
    private List<string> months = new List<string>();
    private List<string> days = new List<string>();

    private void Awake() {

        for(int i= yearRange.x ; i <= yearRange.y ; i++)
            years.Add(i.ToString());
        
        for(int i= 1 ; i <= 12 ; i++)
            months.Add(i.ToString());
        
        for(int i= 1 ; i <= 31 ; i++)
            days.Add(i.ToString());

        yearDropdown.ClearOptions();
        monthDropdown.ClearOptions();
        dayDropdown.ClearOptions();

        yearDropdown.AddOptions(years);
        monthDropdown.AddOptions(months);
        dayDropdown.AddOptions(days);

        yearDropdown.onValueChanged.AddListener((idx) => {
            Notify();
        });

        monthDropdown.onValueChanged.AddListener((idx) => {
            Notify();
        });

        dayDropdown.onValueChanged.AddListener((idx) => {
            Notify();
        });
    }

    public void Set(int year, int month, int day, bool silent = false) {
        DateTime date = new DateTime();

        try {
            date = new DateTime(year, month, day);
        } catch(System.Exception ex ) {

        }

        Set(date, silent);
    }

    public void Set(DateTime date, bool silent = false) {
        
        int yearIdx = years.FindIndex(q => int.Parse(q) == date.Year);
        int monthIdx = months.FindIndex(q => int.Parse(q) == date.Month);
        int dayIdx = days.FindIndex(q => int.Parse(q) == date.Day);

        if(silent) {
            yearDropdown.SetValueWithoutNotify(yearIdx);
            monthDropdown.SetValueWithoutNotify(monthIdx);
            dayDropdown.SetValueWithoutNotify(dayIdx);
        } else {
            yearDropdown.value = yearIdx < 0 ?  0 : yearIdx;
            monthDropdown.value = monthIdx < 0 ? 0 : monthIdx;
            dayDropdown.value = dayIdx < 0 ? 0 : dayIdx;
        }
    }

    DateTime GetDateTime(){
        string year = years[yearDropdown.value];
        string month = months[monthDropdown.value];
        string day = days[dayDropdown.value];
        
        int yearInt = int.Parse(year);
        int monthInt = int.Parse(month);
        int dayInt = int.Parse(day);

        DateTime dateTime = new DateTime(yearInt, monthInt, dayInt);
        return dateTime;
    }

    void Notify(){
       
        OnInputEvent.Invoke(GetDateTime());
    }


    public void SetEnable(bool enable) {
        yearDropdown.interactable = enable;
        monthDropdown.interactable = enable;
        dayDropdown.interactable = enable;
    }

}
