using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    private Text text;

    [TextArea(2, 6)]
    public string RU;
    [TextArea(2,6)]
    public string ENG;

    private void Awake()
    {
        if (!ReferenceEquals(GetComponent<Text>(), null))
        {
            text = GetComponent<Text>();
            ResetText();
            Localization.OnLanguageChanged += ResetText;
        }
    }
    //private void OnEnable()
    //{
    //    if(!ReferenceEquals(GetComponent<Text>(), null))
    //    {
    //        text = GetComponent<Text>();
    //        ResetText();
    //        Localization.OnLanguageChanged += ResetText;
    //    }
    //}

    private void OnDisable()
    {
        Localization.OnLanguageChanged -= ResetText;
    }

    public void ResetText()
    {
        if(!ReferenceEquals(text, null))
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
