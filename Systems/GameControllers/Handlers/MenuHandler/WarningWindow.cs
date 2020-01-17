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

    public const string loadGeneralGameWarning_Ru = "Пожалуйста, подождите пока сгенерируется мир. при недавнем запуске игры Это может занять немного времени.";
    public const string loadGeneralGameWarning_Eng = "Please wait while the world is generated. on a recent launch of the game This may take a little time.";

    public const string thaksForSendReview_Ru = "отзыв успешно отправлен! спасибо за уделенное вами время на тестирование данной версии игры.";
    public const string thaksForSendReview_Eng = "Feedback sent successfully! Thank you for your time in testing this version of the game.";

    public const string fillStringsRequest_Ru = "пожалуйста, заполните поле вашей почты (например : adress@example.ex), а так же укажите ваше устройство, если оно не указано.";
    public const string fillStringsRequest_Eng = "please fill in your mail field (for example : adress@example.ex), and indicate your device if it is not specified.";

    public const string notImplementedFunc_Ru = "извините, эта функция пока недоступна, но появится в скором времени.";
    public const string notImplementedFunc_Eng = "sorry, this feature is not yet available, but will appear shortly.";

    public const string dontHaveMoney_Ru = "у вас недостаточно денег";
    public const string dontHaveMoney_Eng = "you do not have enough money";

    public const string dontHaveExperience_Ru = "у вас недостаточно опыта";
    public const string dontHaveExperience_Eng = "you do not have enough experience";

    public const string canImproveTruck_Ru = "Вы достигли нового уровня! Теперь вы можете улучшить свой грузовик и добавить слот для оружия";
    public const string canImproveTruck_Eng = "You have reached a new level! Now you can upgrade your truck and add a weapon slot";

    public const string googlePSLogInSuccessfully_Ru = "вход выполнен успешно! теперь вы можете участвовать в рейтингах среди игроков в разделе ''статистика''!";
    public const string googlePSLogInSuccessfully_Eng = "Login successful! Now you can participate in ratings among players in the section '' statistics ''!";

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

    public static string NotImplementedFunc()
    {
        if (Localization.currentLanguage == GameEnums.Language.RU)
        {
            return notImplementedFunc_Ru;
        }
        else
        {
            return notImplementedFunc_Eng;
        }
    }

    public static string DontHaveMoney()
    {
        if (Localization.currentLanguage == GameEnums.Language.RU)
        {
            return dontHaveMoney_Ru;
        }
        else
        {
            return dontHaveMoney_Eng;
        }
    }

    public static string DontHaveExperience()
    {
        if (Localization.currentLanguage == GameEnums.Language.RU)
        {
            return dontHaveExperience_Ru;
        }
        else
        {
            return dontHaveExperience_Eng;
        }
    }

    public static string CanImproveTruck()
    {
        if (Localization.currentLanguage == GameEnums.Language.RU)
        {
            return canImproveTruck_Ru;
        }
        else
        {
            return canImproveTruck_Eng;
        }
    }

    public static string GooglePSLogInSuccessfully()
    {
        if (Localization.currentLanguage == GameEnums.Language.RU)
        {
            return googlePSLogInSuccessfully_Ru;
        }
        else
        {
            return googlePSLogInSuccessfully_Eng;
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
