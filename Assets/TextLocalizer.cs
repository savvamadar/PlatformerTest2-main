using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLocalizer : MonoBehaviour
{
    public Text txt; //varialbe that points to what text we should be modifying

    public string localization_key; //variable representing the key to look up a word in the localization
    public string default_value; //variable representing the default value to set to if the above key doesn't exist

    public void Start()
    {
        //Add the current object with this script into the list of items that need to be localized
        Localizer.Localize(gameObject, localization_key, default_value);
    }

    public void localize(string localization)
    {
        txt.text = localization;
    }
}
