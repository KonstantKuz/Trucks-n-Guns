using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Singleton;

public static class WarningStrings
{
    public const string internetConnectionWarning_Ru = "Что-то пошло не так. Пожалуйста, проверьте подключение к интернету и попробуйте снова.";
    public const string internetConnectionWarning_Eng = "Something went wrong. Please, check your internet connection and try again.";

    public const string logInRequestWarning_Ru = "Пожалуйста, войдите в систему с помощью кнопки ''Войти'' в главном меню, чтобы увидеть лидеров и свою позицию в рейтингах.";
    public const string logInRequestWarning_Eng = "Please login with ''LogIn'' button in main menu to look at leaders and your position in leaderboards.";

    public const string loadGeneralGameWarning_Ru = "Пожалуйста, подождите пока сгенерируется мир. Это может занять до одной минуты.";
    public const string loadGeneralGameWarning_Eng = "Please wait while the world is generated. This may take up to one minute.";

    public const string thaksForSendReview_Ru = "отзыв успешно отправлен! спасибо за уделенное вами время на тестирование данной версии игры.";
    public const string thaksForSendReview_Eng = "Feedback sent successfully! Thank you for your time in testing this version of the game.";

    public const string fillStringsRequest_Ru = "пожалуйста, заполните поле вашей почты по примеру example@box.com, а так же укажите ваше устройство, если оно не указано.";
    public const string fillStringsRequest_Eng = "please fill in your mail field as in example @box.com, and indicate your device if it is not specified.";

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

    public static string LoadGeneralGameWarning()
    {
        if (Localization.currentLanguage == GameEnums.Language.RU)
        {
            return loadGeneralGameWarning_Ru;
        }
        else
        {
            return loadGeneralGameWarning_Eng;
        }
    }

    public static string ThanksForReview()
    {
        if (Localization.currentLanguage == GameEnums.Language.RU)
        {
            return thaksForSendReview_Ru;
        }
        else
        {
            return thaksForSendReview_Eng;
        }
    }

    public static string FillStringsRequest()
    {
        if (Localization.currentLanguage == GameEnums.Language.RU)
        {
            return fillStringsRequest_Ru;
        }
        else
        {
            return fillStringsRequest_Eng;
        }
    }
}

public class WarningWindow : Singleton<WarningWindow>
{
    public Text warningText;

    public Button okButton;
    
    public void ShowWarning(string message)
    {
        ShowDetails(true);
        warningText.text = message;
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() => ShowDetails(false));
    }

    private void ShowDetails(bool enabled)
    {
        foreach (Transform child  in transform)
        {
            child.gameObject.SetActive(enabled);
        }
        if(enabled == false)
        {
            okButton.onClick.RemoveAllListeners();
        }
    }
}
