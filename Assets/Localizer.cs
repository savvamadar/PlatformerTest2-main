using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localization
{
    //an enum containg possible language choices
    public enum Language
    {
        English,
        Spanish
    }

    //a dictionary representing LANGUAGE to TRANSLATION
    //where you input a LANGUAGE as the key and get the appropriate translation as the value
    private Dictionary<Language, string> language_to_translation = new Dictionary<Language, string>();

    //The method used to actually Add to the above dictionary
    public void Add(Language language, string translation)
    {
        language_to_translation[language] = translation;
    }

    //The method use to safely Get a translation from the dictionary
    public string Get(Language language, string default_value = "")
    {
        string translation = default_value;
        language_to_translation.TryGetValue(language, out translation);
        return translation;
    }

}

//this object is used to basically perform a localization update on existing already localized objects
public class GameObject_LocalizationKey{
    public GameObject go; //the object that needs to be localized
    public string key; //the look up key for the localization
    public string default_value; //the default value if the key or language isn't found

    public GameObject_LocalizationKey(GameObject _g, string _key, string _dv)
    {
        go = _g;
        key = _key;
        default_value = _dv;
    }
}

public class Localizer : MonoBehaviour
{

    public static Localizer instance;

    public static Localization.Language selectedLanguage = Localization.Language.English; //current language

    //a dictionary representing KEY to Localization
    //where you input a string as the key and get the Localization(s) for that key as the value
    public static Dictionary<string, Localization> key_to_translation = new Dictionary<string, Localization>();

    //List of gameobjects that need to be updated if localization changes
    public static List<GameObject_LocalizationKey> localized_items = new List<GameObject_LocalizationKey>();

    public void OnEnable()
    {
        if(instance == null)
        {
            instance = this;

            //these are some default localizations that
            //the game uses
            Add("_title", Localization.Language.English, "GRAMMAR BOY!");
            Add("_title", Localization.Language.Spanish, "¡EL NIÑO GRAMÁTICO!");

            Add("_bt", Localization.Language.English, "Best Time: ");
            Add("_bt", Localization.Language.Spanish, "Mejor Momento: ");

            Add("_hs", Localization.Language.English, "High Scores");
            Add("_hs", Localization.Language.Spanish, "Puntuaciones Máximas");

            Add("_lang", Localization.Language.English, "English");
            Add("_lang", Localization.Language.Spanish, "Español");
        }

    }

    //method to add a Language and it a localization to a key
    public static void Add(string key, Localization.Language language, string value)
    {
        Localization _l = null;
        if(!key_to_translation.TryGetValue(key, out _l))
        {
            key_to_translation[key] = new Localization();
        }
        key_to_translation[key].Add(language, value);
    }

    //method to get a Localization for a language using a key
    public static string Get(string key, Localization.Language language, string default_value)
    {
        Localization localization = null;
        string translation = default_value;
        if(key_to_translation.TryGetValue(key, out localization))
        {
            translation = localization.Get(language, default_value);
        }
        return translation;


    }

    //the method used to actually Add objects into the list of objects to be localized
    public static void Localize(GameObject go, string key, string dv)
    {
        //if the object is null do nothing because we can't update it now or later
        if(go == null)
        {
            return;
        }

        //we need to keep track of objects that we've already added
        //if they are added then just update their key and default value
        bool already_added = false;
        for(int i = 0; i < localized_items.Count; i++)
        {
            if(localized_items[i].go == go)
            {
                localized_items[i].key = key;
                localized_items[i].default_value = dv;
                already_added = true;
                break;
            }
        }

        //otherwise create a new entry for this object
        if (!already_added)
        {
            localized_items.Add(new GameObject_LocalizationKey(go, key, dv));
        }

        //update the localizations for all objects in the localized_items list
        UpdateLocalizations();
    }

    //toggles the language
    //if it's english make it spanish
    //if it's spanish make it english
    [ContextMenu("Toggle Language")]
    public void LanguageToggle()
    {
        if(selectedLanguage == Localization.Language.English)
        {
            SwitchLocalization(Localization.Language.Spanish);
        }
        else
        {
            SwitchLocalization(Localization.Language.English);
        }
    }

    //switches language and updates the localizations 
    public static void SwitchLocalization(Localization.Language language)
    {
        selectedLanguage = language;

        UpdateLocalizations();
    }

    //go through all the items in the list
    //if they exist use their key and default value to get a localization
    //send them a message to update themselves
    private static void UpdateLocalizations()
    {
        if (instance)
        {
            for (int i = 0; i < localized_items.Count; i++)
            {
                if (localized_items[i].go != null)
                {
                    instance.SendTranslation(
                        localized_items[i].go,
                        Get(localized_items[i].key,
                        selectedLanguage,
                        localized_items[i].default_value
                    ));
                }
            }
        }
    }

    //sends a message to a gameobject with the final translation/ localization
    public void SendTranslation(GameObject go, string value)
    {
        go.SendMessage("localize", value, SendMessageOptions.DontRequireReceiver);
    }

}
