using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;
using UnityEngine.UI;

[System.Serializable]
public class Controls
{
    public RectTransform moveArea, fireArea;
    public Button forwardBoost, backwardBoost, parkingBrake;
}

[System.Serializable]
public class Windows
{
    public TaskWindow taskWindow;
}

[System.Serializable]
public class OtherUI
{
    public Slider playerConditionValue;
    public Slider targetConditionValue;

    public Button changeCameraBehaviour;
    public Button restartButton;
    public Button returnToMenuButton;
}

public class GeneralGameUIHolder : Singleton<GeneralGameUIHolder>
{
    public Controls controls;

    public Windows windows;

    public OtherUI otherUI;
}
