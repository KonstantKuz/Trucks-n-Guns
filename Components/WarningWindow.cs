using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Singleton;

public static class WarningStrings
{
    public const string internetConnectionWarning_Ru = "Что-то пошло не так. Пожалуйста, проверьте подключение к интернету и попробуйте снова.";
    public const string internetConnectionWarning_Eng = "Something went wrong. Please, check your internet connection and try again.";

    public const string logInRequestWarning_Ru = "Пожалуйста, войдите в систему с помощью кнопки ''Войти'' в главном меню, чтобы увидеть лидеров и свою позицию в рейтингах";
    public const string logInRequestWarning_Eng = "Please login with ''LogIn'' button in main menu to look at leaders and your position in leaderboards";

    public static string InternetConnectionWarning()
    {
        if(Localization.currentLanguage == GameEnums.Language.RU)
        {
            return internetConnectionWarning_Ru;
        }
        else
        {
            return internetConnectionWarning_Eng;
        }
    }

    public static string LogInRequestWarning()
    {
        if (Localization.currentLanguage == GameEnums.Language.RU)
        {
            return logInRequestWarning_Ru;
        }
        else
        {
            return logInRequestWarning_Eng;
        }
    }

}

public class WarningWindow : Singleton<WarningWindow>
{
    public Text warningText;

    public Button okButton;
    
    public void ShowWarning(string message)
    {
        Show(true);
        warningText.text = message;
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() => Show(false));
    }

    public void Show(bool enabled)
    {
        foreach (Transform child  in transform)
        {
            child.gameObject.SetActive(enabled);
        }
    }
}
