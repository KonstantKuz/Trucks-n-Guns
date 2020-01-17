using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackQuestion : MonoBehaviour
{
    public Text questionText;
    public UnityEngine.UI.Toggle yes;
    public UnityEngine.UI.Toggle no;

    private void Awake()
    {
        yes.isOn = false;
        no.isOn = false;
        yes.onValueChanged.AddListener((bool enabled) => ChangeMirrorAnswer(yes,no));
        no.onValueChanged.AddListener((bool enabled) => ChangeMirrorAnswer(no,yes));
    }

    public void ChangeMirrorAnswer(UnityEngine.UI.Toggle changedToggle, UnityEngine.UI.Toggle toggleToChange)
    {
        toggleToChange.isOn = !changedToggle.isOn;
    }

    public string GetAnswer()
    {
        if(yes.isOn)
        {
            return $"{questionText.text} : {yes.GetComponentInChildren<Text>().text}";
        }
        else
        {
            return $"{questionText.text} : {no.GetComponentInChildren<Text>().text}";
        }
    }

}
