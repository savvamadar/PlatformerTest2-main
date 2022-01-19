using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScore : MonoBehaviour
{
    public static HighScore instance;

    public GameObject BestTimeBanner;
    public Text BestTimeText;

    public RectTransform LeaderBoard;
    public RectTransform[] LeaderBoardPositions; // [closed, open]

    public Text[] LeaderBoardScores;

    public float transitionSpeed = 1f;

    public void OnEnable()
    {
        if (instance == null)
        {
            instance = this;

            UpdateScores();

            //it starts open so after a bit of time let's close it
            InvokeMethod("CloseLeaderBoard", 2.2f);

            //This object has some text we need to localize so let's register it
            Localizer.Localize(gameObject, "_bt", "Best Time: ");
        }
    }

    //a static method to allow Invoke to be used sort of statically
    //I thought this would be perfect for delayed opening/ closing of the leaderboards 
    public static void InvokeMethod(string method, float delay)
    {
        if (instance)
        {
            instance.InvokeMethodInstance(method, delay);
        }
    }

    //What actually calls the Invoke method
    public void InvokeMethodInstance(string method, float delay)
    {
        Invoke(method, delay);
    }

    //a hidden method meant to be used via 'InvokeMethod' to restart the game
    private void ReloadLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    //a hidden method meant to be used via 'InvokeMethod' to close the leaderboard
    private void CloseLeaderBoard()
    {
        open = false;
    }

    //a hidden method meant to be used via 'InvokeMethod' to open the leaderboard
    private void OpenLeaderBoard()
    {
        open = true;
    }


    public bool open = true; //dicates whether the leaderboard should be open or closed
    public void Update()
    {
        //based on the above bool - should we be moving towards the open position or the close position?
        LeaderBoard.anchoredPosition = Vector3.Lerp(LeaderBoard.anchoredPosition, LeaderBoardPositions[(open ? 1 : 0)].anchoredPosition, Time.deltaTime * transitionSpeed);
    }

    //a static method to be called from anywhere that may need to update the scores visually
    public static void UpdateScores()
    {
        if (instance)
        {
            instance.UpdateHSBannerInstance();
            instance.UpdateLeaderBoardScoresInstance();
        }
    }

    //this is the string we need to localize
    string localizeBestTimeString = "Best Time: ";
    public void localize(string localization)
    {
        localizeBestTimeString = localization; //store the localization
        UpdateHSBannerInstance(); //update the visual
    }

    public void UpdateHSBannerInstance()
    {
        float fastestTime = PlayerPrefs.GetFloat("hi_score_0", -1f); //get the best score
        if (fastestTime >= 0f) //if it's >=0 that means it's an actual score, anything else and that means we haven't set it yet
        {
            //show us the best time banner with a localized string
            BestTimeBanner.SetActive(true);
            BestTimeText.text = localizeBestTimeString + fastestTime.ToString("F2");
        }
        else
        {
            //hide the best time banner because we don't have a best time yet
            BestTimeBanner.SetActive(false);
        }
    }

    //method to update the leaderboard visuals
    public void UpdateLeaderBoardScoresInstance()
    {
        //loop through all the high scores
        //the first time we hit a high schore that's less than 0 (which means we haven't filled that position in the leaderboard yet)
        //display some dashes rather than a time and then  stop
        //otherwise displayt the actual time
        for (int i = 0; i < LeaderBoardScores.Length; i++)
        {
            float _s = PlayerPrefs.GetFloat("hi_score_" + i, -1f);
            LeaderBoardScores[i].gameObject.SetActive(true);
            if (_s < 0f)
            {
                LeaderBoardScores[i].text = (i + 1) + ". - - - -";
                break;
            }
            else
            {
                LeaderBoardScores[i].text = (i + 1) + ". " + _s.ToString("F2");
            }
        }
    }

    public ParticleSystem highScoreParticles; //stores the diamonds that fall out of the sky particle system

    //give it a score to record - if it's in the top 5 save it
    public static void RecordScore(float timeAlive)
    {
        bool new_score_set = false; //tracks if we've just saved a new score

        float prev_time = 0f; //tracks the previous time that got overwritten if we have to "insert" a new high score/
                              //move down some scores because a new high score has shifted them

        for (int i = 0; i < 5; i++)
        {
            //if we haven't inserted a new score then get the current score for this index
            //otherwise use the previous indexes old time
            float recorded_time = new_score_set ? prev_time : PlayerPrefs.GetFloat("hi_score_" + i, -1f);

            //if the current indexes recorded time is slower than our new time or it's less than 0 (eg. the index has not yet has a score written to it)
            //and we aren't shifting scores around then...
            if ((recorded_time >= timeAlive || recorded_time < 0f) && new_score_set == false)
            {
                //save the new score
                PlayerPrefs.SetFloat("hi_score_" + i, timeAlive);
                //update the variable to signify we need to start shifting scores down
                new_score_set = true;
            }

            //if we are shifting scores down
            if (new_score_set)
            {
                prev_time = PlayerPrefs.GetFloat("hi_score_" + (i + 1), -1f); //record the score for the next index

                PlayerPrefs.SetFloat("hi_score_" + (i + 1), recorded_time); //shift the old score from the index down to the next index

                if (i == 0 && instance) //if we have an instanced reference to this script and the highscore recorder index is 0 (which means this is our best score)
                {
                    instance.highScoreParticles.Play(true); //rain diamonds!
                }
            }
        }
    }
}
