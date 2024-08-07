using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LanguageSwitcher : MonoBehaviour
{
    [SerializeField] Button lanBTN;
    int currentLanguageIndex=0;
    private void Awake()
    {
        lanBTN.onClick.AddListener(SwitchLanguage);
    }
    public void SwitchLanguage()
    {
        currentLanguageIndex = (currentLanguageIndex==0)?1:0;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[currentLanguageIndex];
    }
}
