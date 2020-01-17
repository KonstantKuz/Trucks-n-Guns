using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localization : MonoBehaviour
{
    public static GameEnums.Language currentLanguage;

    public delegate void LanguageChange();
    public static event LanguageChange OnLanguageChanged;

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += delegate { SetUpLanguage(); } ;
    }

    public static void SetUpLanguage()
    {
        if(Application.systemLanguage == SystemLanguage.Russian)
        {
            currentLanguage = GameEnums.Language.RU;
        }
        else
        {
            currentLanguage = GameEnums.Language.ENG;
        }

        OnLanguageChanged?.Invoke();
    }
    public static void SetUpLanguage(GameEnums.Language language)
    {
        currentLanguage = language;
        OnLanguageChanged?.Invoke();
    }
}
