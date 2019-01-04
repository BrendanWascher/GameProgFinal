using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [HideInInspector]
    public bool isTimeUp;
    public Text timeDisplayText;
    public float secondsTilEnd = 120f;
    private float countDownTime;
    private float calculatedSeconds;
    private int calculatedMinutes;
    private string calcSecondsString;

    private void Start()
    {
        ResetTimer();
    }

    void Update ()
    {
        if (!isTimeUp)
        {
            DecrementTime();
            CalculateTime();
            DisplyTime();
        }
	}

    //decrease the timer by how much time has passed since last called
    private void DecrementTime()
    {
        countDownTime -= Time.deltaTime;
        if (countDownTime <= 0f)
        {
            isTimeUp = true;
            countDownTime = 0f;
        }
    }

    //Calculate the time to be displayed and format it in an easily displayable way
    private void CalculateTime()
    {
        calculatedMinutes = (int)(countDownTime / 60);
        calculatedSeconds = countDownTime % 60;
        calcSecondsString = calculatedSeconds.ToString("00.00");
    }

    //give the text the time that should be displayed
    private void DisplyTime()
    {
        timeDisplayText.text = (calculatedMinutes+":"+calcSecondsString).ToString();
    }

    //reset the timer to normal start time and say that time is not up
    public void ResetTimer()
    {
        countDownTime = secondsTilEnd;
        isTimeUp = false;
    }
}
