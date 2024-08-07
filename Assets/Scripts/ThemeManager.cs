using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    public ThemeData darkTheme;
    public ThemeData lightTheme;
    private ThemeData currentTheme;

    public List<Image> backgroundImages;
    public List<Image> foregroundImages;
    public List<TextMeshProUGUI> texts;

    [SerializeField] Button toggelButton;
    TextMeshProUGUI toggleBTNtxt;

    void Awake()
    {
        toggleBTNtxt = toggelButton.GetComponentInChildren<TextMeshProUGUI>();
        // Set default theme
        ApplyTheme(lightTheme);
    }
    private void Start()
    {
        toggelButton.onClick.AddListener(ToggleTheme);
    }
    public void ToggleTheme()
    {
        if (currentTheme == darkTheme)
            ApplyTheme(lightTheme);
        else
            ApplyTheme(darkTheme);
    }

    private void ApplyTheme(ThemeData theme)
    {
        SetLocalizedText(theme.name);
        currentTheme = theme;
        backgroundImages.ForEach(img => img.color = theme.background);
        foregroundImages.ForEach(img => img.color = theme.foreground);
        texts.ForEach(txt => txt.color = theme.text);
    }
    public  void SetLocalizedText(string _key)
    {
        var localizedString = LocalizationSettings.StringDatabase.GetLocalizedString("EN-HI", _key);
        toggleBTNtxt.text = localizedString;
    }
}
