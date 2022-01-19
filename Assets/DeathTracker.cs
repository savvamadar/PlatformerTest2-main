using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathTracker : MonoBehaviour
{
    public static DeathTracker instance;

    public static int sessionDeathCount = -1; //how many deaths have we had before we were able to complete this level
    public static float lastDeathTime = 0f; //how long have we been alive for

    public Sprite[] splats; //just some splats to keep things interesting, everytime you die a new splat will be chosen from this list

    public Image DeathCounterSplat; // the death counter splat backgroudn we'll be updating using the above list
    public Text DeathCounterText; //the text element that we will update with our incremented deaths
    public Text DeathTimeText; //the text element that we will update with our tim we've been alive

    private static bool pauseDeathTimer = true; //variable used to stop updating the DeatTimeText automatically

    public void OnEnable()
    {
        if (instance == null)
        {
            //reset variables
            lastDeathTime = 0f;
            sessionDeathCount = -1;

            //grab instance
            instance = this;

            //incrementdeath because it's -1
            //this is an easy way to get things sort of reset
            IncrementDeath();

            //start the timer for the first time we play
            StartTimer();
        }
    }

    //name is self explainable - increment our death counter
    public static void IncrementDeath()
    {
        sessionDeathCount++;
        PauseTimer();
        if (instance)
        {
            instance.UpdateDeathCounterVisuals();
            instance.UpdateDeathTimeVisual();
        }
    }

    //update the visuals of the death counter
    public void UpdateDeathCounterVisuals()
    {
        DeathCounterText.text = sessionDeathCount + "";
        DeathCounterSplat.sprite = splats[Random.Range(0, splats.Length)];
    }

    //update the visuals of the timer
    public void UpdateDeathTimeVisual()
    {
        DeathTimeText.text = GetTimeAlive().ToString("F2");
    }

    //method to both set a new time of being alive and
    //flip the variable so that the daeth visual is automatically updated
    public static void StartTimer()
    {
        pauseDeathTimer = false;
        lastDeathTime = Time.realtimeSinceStartup;
    }

    //flip the variable so that the daeth visual is NOT automatically updated
    //and updated it to the latest time difference
    public static void PauseTimer()
    {
        pauseDeathTimer = true;
        if (instance)
        {
            instance.UpdateDeathTimeVisual();
        }
    }

    //how much time has passed between now and since we last spawned?
    public static float GetTimeAlive()
    {
        return Time.realtimeSinceStartup - lastDeathTime;
    }

    //auto update the timer
    public void Update()
    {
        if (!pauseDeathTimer)
        {
            UpdateDeathTimeVisual();
        }
    }

}
