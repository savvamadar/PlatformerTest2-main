using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextMeshProLocalizer : MonoBehaviour
{
    public TextMeshPro txt; //varialbe that points to what text we should be modifying

    public string localization_key; //variable representing the key to look up a word in the localization
    public string default_value; //variable representing the default value to set to if the above key doesn't exist

    //enables the object
    //updates the word being displayed and
    //starts a coroutine that will disable the object
    public void Display(float time)
    {
        //Add the current object with this script into the list of items that need to be localized
        Localizer.Localize(gameObject, localization_key, default_value);

        gameObject.SetActive(true);
        time_before_close = time;
        if (!closing)
        {
            StartCoroutine(WaitToClose());
        }
    }

    private float time_before_close = 0f; //keeps track of how long should each new word be displayed for
    private bool closing = false; //keeps track of if we're hiding the text. We don't want to trigger the coroutine multiple times.

    //the coroutine that stops the word from being displayed
    //can be delayed by adjusting time_before_close (useful if the player says a new word before the old word is hidden)
    IEnumerator WaitToClose()
    {
        closing = true;

        //chip away at the time variable each from so that we can modify it outside of here if need be
        while(time_before_close > 0f)
        {
            yield return new WaitForEndOfFrame();
            time_before_close -= Time.deltaTime;
        }

        yield return new WaitForEndOfFrame();


        closing = false;
        gameObject.SetActive(false);
    }

    //function that acts like a call back
    public void localize(string localization)
    {
        txt.text = localization;
    }
}
