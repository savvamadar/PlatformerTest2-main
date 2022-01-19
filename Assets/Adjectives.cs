using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adjectives : MonoBehaviour
{
    public TextAsset englishAsset; //file containing english adjectives
    public TextAsset spanishAsset; //file containing spanish adjectives

    public static string[] bigger_array; //we need to store this to pull adjectives from this later

    void Start()
    {
        string[] english_parse = englishAsset.text.Split('\n'); //split by new lines
        string[] spanish_parse = spanishAsset.text.Split('\n');


        //we need to find out what file contains the most amount of lines so we can maximize how many words
        //the player will say
        //I did check the files and they're the same length but I figured why not - maybe you'll try to break my code
        //oh and we need to save the largets array because we'll be using it's entries to choose a random adjactive to say
        bigger_array = english_parse.Length >= spanish_parse.Length ? english_parse : spanish_parse;

        string[] smaller_array = bigger_array == english_parse ? spanish_parse : english_parse;

        Localization.Language big_language = bigger_array == english_parse ? Localization.Language.English : Localization.Language.Spanish;
        Localization.Language small_language = big_language == Localization.Language.English ? Localization.Language.Spanish : Localization.Language.English;

        for (int i = 0; i < bigger_array.Length; i++)
        {
            Localizer.Add(bigger_array[i], big_language, bigger_array[i]);
            if(i < smaller_array.Length)
            {
                Localizer.Add(bigger_array[i], small_language, smaller_array[i]);
            }
        }
    }

    //pretty simple - get a random key that represents an adjactive
    public static string RandomAdjactive()
    {
        string adj = "";
        if(bigger_array != null && bigger_array.Length > 0)
        {
            adj = bigger_array[Random.Range(0, bigger_array.Length)];
        }
        return adj;
    }

}
