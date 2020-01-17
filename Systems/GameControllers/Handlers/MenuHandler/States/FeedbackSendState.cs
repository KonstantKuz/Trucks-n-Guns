using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using UnityEngine.UI;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class FeedbackSendState : State<MenuHandler>
{
    public static FeedbackSendState _instance;

    private FeedbackSendState()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public static FeedbackSendState Instance
    {
        get
        {
            if (_instance == null)
                new FeedbackSendState();

            return _instance;
        }
    }

    public override void EnterState(MenuHandler _owner)
    {
        _owner.feedBack.FeedbackWindow.SetActive(true);

        _owner.feedBack.GoToGoogleButton.GetComponent<Button>().onClick.AddListener(() => GoToGooglePlayPage());

        _owner.feedBack.SendButton.GetComponent<Button>().onClick.AddListener(() => SendReview(_owner));

        _owner.BackButton.SetActive(true);
        _owner.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _owner.BackButton.GetComponent<Button>().onClick.AddListener(() => BackToMainMenu(_owner));
    }

    public override void ExitState(MenuHandler _owner)
    {
        _owner.feedBack.FeedbackWindow.SetActive(false);

        _owner.feedBack.GoToGoogleButton.GetComponent<Button>().onClick.RemoveAllListeners();

        _owner.feedBack.SendButton.GetComponent<Button>().onClick.RemoveAllListeners();
        _owner.BackButton.SetActive(false);

    }

    public override void UpdateState(MenuHandler _owner)
    {
        throw new System.NotImplementedException();
    }

    public void GoToGooglePlayPage()
    {
        Application.OpenURL("market://details?id=" + Application.identifier);
    }

    public void SendReview(MenuHandler _owner)
    {
        if(!_owner.feedBack.senderMail_Adress.text.Contains("@") || !_owner.feedBack.senderMail_Adress.text.Contains("."))
        {
            WarningWindow.Instance.ShowWarning(WarningStrings.FillStringsRequest());
            return;
        }
        if(_owner.feedBack.senderMail_Adress.text != "" && _owner.feedBack.senderDevice.text != "")
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_owner.feedBack.senderMail_Adress.text);
            mail.To.Add(new MailAddress(GoogleDataHolder.Support.log));
            mail.Subject = _owner.feedBack.senderMail_Adress.text + "'s Review";

            for (int i = 0; i < _owner.feedBack.QuickQuestions.Length; i++)
            {
                mail.Body += $"\n {_owner.feedBack.QuickQuestions[i].GetAnswer()}";
            }
            mail.Body += $"\n Device : {_owner.feedBack.senderDevice.text}";
            mail.Body += "\n----------------------------------------------------------- \n";
            mail.Body += _owner.feedBack.senderReview.text;

            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.EnableSsl = true;
            smtpServer.UseDefaultCredentials = false;
            smtpServer.Credentials = new NetworkCredential(GoogleDataHolder.Support.log, GoogleDataHolder.Support.pass) as ICredentialsByHost;
            smtpServer.Timeout = 20000;

            ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };

            try
            {
                smtpServer.Send(mail);
                WarningWindow.Instance.ShowWarning(WarningStrings.ThanksForReview());
            }
            catch (System.Exception ex)
            {
                WarningWindow.Instance.ShowWarning(ex.Message);
            }
        }
        else
        {
            WarningWindow.Instance.ShowWarning(WarningStrings.FillStringsRequest());
        }
    }

    public void BackToMainMenu(MenuHandler _owner)
    {
        _owner.menu.ChangeState(MainMenuState.Instance);
    }
}
