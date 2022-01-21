using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class TimeUtil
{
    public static string RemainedQuestTime(float min)
    {
        if (min < 0)
            min = 0;

        int hour = (int)(min / 60.0f);
        int m = (int)min - (hour * 60);
        int s = (int)((min - (int)min) * 60.0f);

        string timeText = string.Format("{0:00}:{1:00}:{2:00}", hour, m, s);

        return timeText;
    }

	public static string RemainedQuestTime2(float min)
	{
		if (min < 0)
			min = 0;

		int hour = (int)(min / 60.0f);
		int m = (int)min - (hour * 60);
		int s = (int)((min - (int)min) * 60.0f);

		string timeText = string.Format("{1:00}:{2:00}", hour, m, s);

		return timeText;
	}

	public static string RemainedQuestTime3(float min)
	{
		if (min < 0)
			min = 0;

		int hour = (int)(min / 60.0f);
		int m = (int)min - (hour * 60);
		int s = (int)((min - (int)min) * 60.0f);

        string timeText;

        if(hour > 0)
            timeText= string.Format("{0:00}:{1:00}:{2:00}", hour, m, s);
        else if(m > 0)
			timeText = string.Format("{1:00}:{2:00}", hour, m, s);
        else
			timeText = string.Format("{2:00}", hour, m, s);


		return timeText;
	}

    public static string RemainedQuestTimeFriendly(float min)
	{
		if (min < 0)
			min = 0;

		int hour = (int)(min / 60.0f);
		int m = (int)min - (hour * 60);
		int s = (int)((min - (int)min) * 60.0f);

        string timeText;

        if(hour > 0)
            timeText= string.Format("{0}시간 {1}분", hour, m, s);
        else if(m > 0)
			timeText = string.Format("{1}분", hour, m, s);
        else
			timeText = string.Format("{2}초", hour, m, s);

		return timeText;
	}

    public static bool IsValidDate(DateTime date)
    {
        return !(date == null || date.Year == 1);
    }

    public static int PropInt(float p)
    {
        //int b = (int)(p + 0.5f);

        int b = Mathf.CeilToInt(p);
        float c = b - p;

        if (UnityEngine.Random.value < c)
            return (int)p;
        else
            return (int)b;
    }

    public static DateTime GetTodayStart(DateTime Now)
    {
        return new DateTime(Now.Year, Now.Month, Now.Day, 0, 0, 0);
    }

    public static int RemainedEndOfDay(DateTime Now)
    {
        DateTime endDay = new DateTime(Now.Year, Now.Month, Now.Day, 23, 59, 59);

        int min = (int)(endDay - Now).TotalMinutes;

        return Math.Max(min, 0);
    }

    public static bool IsNextDay(DateTime Now, DateTime start)
    {
        return (start.Day < Now.Day);
    }

    public static bool IsToday(DateTime Now, DateTime target)
    {
        return target.Day == Now.Day;
    }

	public static float ElapsedTimeH(DateTime startTime)
	{
		//if (!IsValidDate (startTime))
		//	startTime = DateTime.Now;

        if(startTime ==null)
            startTime = DateTime.MaxValue;

		float f = (float) (DateTime.Now - startTime).TotalHours;

		return f;
	}

	public static float ElapsedTimeM(DateTime startTime)
	{
		if (!IsValidDate (startTime))
			startTime = DateTime.Now;

		float f = (float) (DateTime.Now - startTime).TotalMinutes;

		return f;
	}
}

public static class TextUtil
{

    public static Color buttonDisableColor = new Color(146f / 255f, 146f / 255f, 146f / 255f);
    public static Color buttonEnableColor = new Color(205f / 255f, 254f / 255f, 127f / 255f);


    public static string TextToImphasis(string text, char letter, int size, Color color)
    {
        System.Text.StringBuilder builder = new StringBuilder();

        string colorStr = ColorUtility.ToHtmlStringRGB(color);

        bool skip= false;
        foreach (char c in text)
        {
            if (c == letter && !skip)
            {
                //  int size = 50;
                //  string color = "#ff00ff";
                builder.AppendFormat("<size={0}><color=#{1}>{2}</color></size>", size, colorStr, c.ToString());
                skip = true;
            }
            else
            {
                builder.Append(c.ToString());
            }
        }

        return builder.ToString();
    }
}
