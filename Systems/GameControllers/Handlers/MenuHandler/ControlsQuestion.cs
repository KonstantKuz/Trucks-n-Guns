﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ControlsQuestion : MonoBehaviour
{
    public void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(() => WarningWindow.Instance.ShowWarning(GetComponent<LocalizedText>().GetLocalizedText()));
    }
}
