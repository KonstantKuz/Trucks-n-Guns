using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    private Text text;

    [TextArea]
    public string RU;
    [TextArea]
    public string ENG;

    private void OnEnable()
    {
        if(!ReferenceEquals(GetComponent<Text>(), null))
        {
            text = GetComponent<Text>();
            ResetText();
            Localization.OnLanguageChanged += ResetText;
        }
    }

    private void OnDisable()
    {
        Localization.OnLanguageChanged -= ResetText;
    }

    public void ResetText()
    {
        switch (Localization.currentLanguage)
        {
            case GameEnums.Language.RU:
                text.text = RU;
                break;
            case GameEnums.Language.ENG:
                text.text = ENG;
                break;
            default:
                break;
        }
    }

    public string GetLocalizedText()
    {
        switch (Localization.currentLanguage)
        {
            case GameEnums.Language.RU:
                return RU;
            case GameEnums.Language.ENG:
                return ENG;
        }
        return "";
    }
}
